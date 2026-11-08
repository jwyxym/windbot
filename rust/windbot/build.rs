use std::{
	env,
	error::Error,
	ffi::{OsStr, OsString},
	fs,
	hash::{DefaultHasher, Hash, Hasher},
	path::{Component, Path, PathBuf},
	process::{Command, Output},
};

fn main() -> Result<(), Box<dyn Error>> {
	println!("cargo:rerun-if-env-changed=WINDBOT_NATIVE_DIR");
	println!("cargo:rerun-if-env-changed=WINDBOT_NATIVE_ARCHIVE");
	println!("cargo:rerun-if-env-changed=WINDBOT_DOTNET");
	println!("cargo:rerun-if-env-changed=WINDBOT_CPP_COMPILER");
	println!("cargo:rerun-if-env-changed=WINDBOT_CPP_LIB_CREATOR");
	println!("cargo:rerun-if-env-changed=WINDBOT_SYSROOT");
	println!("cargo:rerun-if-env-changed=ANDROID_NDK_HOME");
	println!("cargo:rerun-if-env-changed=ANDROID_NDK_ROOT");
	println!("cargo:rerun-if-env-changed=DOTNET_ROOT");
	println!("cargo:rerun-if-env-changed=SDKROOT");
	println!("cargo:rerun-if-env-changed=AR");
	println!("cargo:rerun-if-env-changed=TARGET_AR");
	if env::var_os("CARGO_FEATURE_LINK_NATIVE").is_none() {
		return Ok(());
	}

	let target: String = env::var("TARGET")?;
	let rid: &str = match target.as_str() {
		"x86_64-pc-windows-msvc" => "win-x64",
		"aarch64-pc-windows-msvc" => "win-arm64",
		"x86_64-unknown-linux-gnu" => "linux-x64",
		"aarch64-unknown-linux-gnu" => "linux-arm64",
		"x86_64-apple-darwin" => "osx-x64",
		"aarch64-apple-darwin" => "osx-arm64",
		"aarch64-apple-ios" => "ios-arm64",
		"aarch64-apple-ios-sim" => "iossimulator-arm64",
		"x86_64-apple-ios" => "iossimulator-x64",
		"armv7-linux-androideabi" => "linux-bionic-arm",
		"aarch64-linux-android" => "linux-bionic-arm64",
		"x86_64-linux-android" => "linux-bionic-x64",
		_ => return Err(format!("No WindBot NativeAOT RID is configured for Rust target {target}.").into()),
	};
	let windows: bool = rid.starts_with("win-");
	let apple: bool = target.contains("-apple-");
	let target_features: String = env::var("CARGO_CFG_TARGET_FEATURE").unwrap_or_default();
	if windows && !target_features.split(',').any(|feature: &str| feature == "crt-static") {
		return Err("WindBot Windows archives require the static CRT. Set RUSTFLAGS=-C target-feature=+crt-static for the final application and its dependencies.".into());
	}
	let host: String = env::var("HOST")?;
	if apple && !host.contains("-apple-") {
		return Err("Apple targets require a macOS host with Xcode command line tools.".into());
	}

	let output: PathBuf = PathBuf::from(env::var_os("OUT_DIR").ok_or("Missing OUT_DIR")?);
	let package: PathBuf = PathBuf::from(env::var_os("CARGO_MANIFEST_DIR").ok_or("Missing CARGO_MANIFEST_DIR")?);
	let native_override: Option<OsString> = env::var_os("WINDBOT_NATIVE_DIR");
	let archive_override: Option<OsString> = env::var_os("WINDBOT_NATIVE_ARCHIVE");
	let watch_native_inputs: bool = native_override.is_some();
	let native: PathBuf = match native_override {
		Some(path) => PathBuf::from(path).canonicalize()?,
		None if archive_override.is_some() => {
			let archive: PathBuf = PathBuf::from(archive_override.ok_or("Missing WINDBOT_NATIVE_ARCHIVE")?).canonicalize()?;
			println!("cargo:rerun-if-changed={}", archive.display());
			let archive_bytes: Vec<u8> = fs::read(&archive)?;
			let mut hasher: DefaultHasher = DefaultHasher::new();
			archive_bytes.hash(&mut hasher);
			let extracted: PathBuf = output.join(format!("native-{:016x}", hasher.finish()));
			fs::create_dir_all(&extracted)?;
			if host.contains("-windows-") {
				run(Command::new("tar.exe").arg("-xf").arg(&archive).arg("-C").arg(&extracted))?;
			} else {
				run(Command::new("unzip").arg("-q").arg("-o").arg(&archive).arg("-d").arg(&extracted))?;
			}
			extracted
		}
		None => build_from_source(&package, &output, rid, &host)?,
	};
	let rid_path: PathBuf = native.join("native-rid.txt");
	if watch_native_inputs {
		println!("cargo:rerun-if-changed={}", rid_path.display());
	}
	if rid_path.exists() && fs::read_to_string(&rid_path)?.trim() != rid {
		return Err(format!("The artifact's native-rid.txt does not match {rid}.").into());
	}
	let manifest_path: PathBuf = native.join("native-dependencies.txt");
	if watch_native_inputs {
		println!("cargo:rerun-if-changed={}", manifest_path.display());
	}
	let manifest: String = fs::read_to_string(&manifest_path)?;
	let windbot_filename: &str = if windows { "WindBot.lib" } else { "WindBot.a" };
	let windbot: PathBuf = native.join(windbot_filename);
	if !windbot.is_file() {
		return Err(format!("Missing {windbot_filename} in {}; expected the {rid} artifact.", native.display()).into());
	}
	let bootstrapper_name: &str = if windows { "runtime/bootstrapperdll.obj" } else { "runtime/libbootstrapperdll.o" };
	let bootstrapper: PathBuf = native.join(bootstrapper_name);
	let object: Vec<u8> = fs::read(&bootstrapper)?;
	validate_architecture(&object, rid)?;
	println!("cargo:rustc-link-search=native={}", output.display());

	// Bundle WindBot and initialization objects into the rlib for downstream crates.
	// Linux runtime archives stay separate to preserve lazy extraction and link order.
	if watch_native_inputs {
		println!("cargo:rerun-if-changed={}", windbot.display());
	}
	fs::copy(&windbot, output.join(if windows { "windbot_native.lib" } else { "libwindbot_native.a" }))?;
	println!("cargo:rustc-link-lib=static:+whole-archive=windbot_native");

	let mut has_bootstrapper: bool = false;
	for dependency in manifest.lines() {
		let line: &str = dependency.trim();
		if line.is_empty() {
			continue;
		}
		if line.starts_with("-framework ") && apple {
			let name: &str = line.trim_start_matches("-framework ");
			validate_name(name)?;
			println!("cargo:rustc-link-lib=framework={name}");
		} else if line.starts_with("-l") && !windows {
			let name: &str = &line[2..];
			validate_name(name)?;
			println!("cargo:rustc-link-lib=dylib={name}");
		} else if windows && !line.contains('/') && !line.contains('\\') && line.ends_with(".lib") {
			let name: &str = &line[..line.len() - 4];
			validate_name(name)?;
			println!("cargo:rustc-link-lib=dylib={name}");
		} else {
			let relative: &Path = Path::new(line);
			if !line.starts_with("runtime/")
				|| relative.components().any(|part: Component<'_>| !matches!(part, Component::Normal(_)))
			{
				return Err(format!("Unsupported native dependency: {line}").into());
			}
			let source: PathBuf = native.join(relative);
			if watch_native_inputs {
				println!("cargo:rerun-if-changed={}", source.display());
			}
			let filename: &str = relative.file_name().and_then(|name: &OsStr| name.to_str()).ok_or("Invalid dependency filename")?;
			if filename.ends_with(".obj") || filename.ends_with(".o") {
				if line != bootstrapper_name {
					let object: Vec<u8> = fs::read(&source)?;
					validate_architecture(&object, rid)?;
				}
				let stem: &str = relative.file_stem().and_then(|name: &OsStr| name.to_str()).ok_or("Invalid object filename")?;
				let name: String = format!("windbot_object_{stem}");
				validate_name(&name)?;
				archive_object(&source, &output, &target, windows, &name)?;
				println!("cargo:rustc-link-lib=static:+whole-archive={name}");
				has_bootstrapper |= line == bootstrapper_name;
			} else {
				let name: &str = if windows && filename.ends_with(".lib") {
					&filename[..filename.len() - 4]
				} else if !windows && filename.starts_with("lib") && filename.ends_with(".a") {
					&filename[3..filename.len() - 2]
				} else {
					return Err(format!("Unsupported native dependency: {line}").into());
				};
				validate_name(name)?;
				fs::copy(&source, output.join(filename))?;
				if rid == "linux-x64" || rid == "linux-arm64" {
					// Emit each runtime archive after WindBot instead of merging its
					// objects into the earlier rlib. Extract only required members:
					// Runtime.WorkstationGC and aotminipal contain overlapping symbols.
					println!("cargo:rustc-link-lib=static:-bundle={name}");
				} else {
					println!("cargo:rustc-link-lib=static={name}");
				}
			}
		}
	}
	if !has_bootstrapper {
		return Err("native-dependencies.txt does not list the NativeAOT bootstrapper.".into());
	}
	if rid.starts_with("linux-") {
		// Whole-archive extracts module data, but ELF section GC can still discard it.
		// NativeAOT's bootstrapper requires the __modules start/stop symbols.
		println!("cargo:rustc-link-arg=-Wl,-z,nostart-stop-gc");
	}
	if apple {
		let sdk_name: &str = if rid == "ios-arm64" { "iphoneos" } else if rid.starts_with("iossimulator-") { "iphonesimulator" } else { "macosx" };
		let sdk_override: Option<OsString> = env::var_os("SDKROOT");
		let sdk: PathBuf = match sdk_override {
			Some(path) => PathBuf::from(path),
			None => {
				let result: Output = run(Command::new("xcrun").args(["--sdk", sdk_name, "--show-sdk-path"]))?;
				PathBuf::from(String::from_utf8(result.stdout)?.trim())
			}
		};
		if !sdk.join("usr/lib/swift").is_dir() {
			return Err(format!("Apple SDK lacks usr/lib/swift: {}", sdk.display()).into());
		}
		println!("cargo:rustc-link-search=native={}", sdk.join("usr/lib/swift").display());
		println!("cargo:rustc-link-search=framework={}", sdk.join("System/Library/Frameworks").display());
	}
	Ok(())
}

fn build_from_source(package: &Path, output: &Path, rid: &str, host: &str) -> Result<PathBuf, Box<dyn Error>> {
	let mut inputs: Vec<PathBuf> = Vec::new();
	for entry in fs::read_dir(package)? {
		let entry: fs::DirEntry = entry?;
		let path: PathBuf = entry.path();
		let extension: &str = path.extension().and_then(|value: &OsStr| value.to_str()).unwrap_or("");
		if path.is_file() && matches!(extension, "cs" | "csproj" | "config") {
			println!("cargo:rerun-if-changed={}", path.display());
			inputs.push(path);
		}
	}
	for directory in ["Game", "YGOSharp.Network", "YGOSharp.OCGWrapper", "YGOSharp.OCGWrapper.Enums", "Properties", "Decks", "Dialogs"] {
		println!("cargo:rerun-if-changed={}", package.join(directory).display());
		collect_input_files(&package.join(directory), &mut inputs)?;
	}
	let publish: PathBuf = output.join("native-built");
	let dotnet: OsString = env::var_os("WINDBOT_DOTNET").unwrap_or_else(|| OsString::from("dotnet"));
	let mut command: Command = Command::new(dotnet);
	command.current_dir(package)
		.arg("publish").arg(package.join("WindBot.csproj"))
		.args(["-c", "Release", "-r", rid, "-p:NativeLib=Static"])
		.arg("--artifacts-path").arg(output.join("dotnet"))
		.arg("-o").arg(&publish);
	if rid.starts_with("linux-bionic-") {
		let ndk: OsString = env::var_os("ANDROID_NDK_HOME").or_else(|| env::var_os("ANDROID_NDK_ROOT"))
			.ok_or("Android source builds require ANDROID_NDK_HOME or ANDROID_NDK_ROOT.")?;
		let prebuilt: &str = if host.contains("-windows-") { "windows-x86_64" } else if host.contains("-apple-") { "darwin-x86_64" } else { "linux-x86_64" };
		let toolchain: PathBuf = PathBuf::from(ndk).join("toolchains/llvm/prebuilt").join(prebuilt);
		let suffix: &str = if host.contains("-windows-") { ".exe" } else { "" };
		let compiler: PathBuf = toolchain.join(format!("bin/clang{suffix}"));
		let archiver: PathBuf = toolchain.join(format!("bin/llvm-ar{suffix}"));
		if !compiler.is_file() || !archiver.is_file() || !toolchain.join("sysroot").is_dir() {
			return Err(format!("Incomplete Android NDK toolchain: {}", toolchain.display()).into());
		}
		command.arg(format!("-p:CppCompilerAndLinker={}", compiler.display()))
			.arg(format!("-p:CppLibCreator={}", archiver.display()))
			.arg(format!("-p:SysRoot={}", toolchain.join("sysroot").display()));
	}
	for (key, property) in [
		("WINDBOT_CPP_COMPILER", "CppCompilerAndLinker"),
		("WINDBOT_CPP_LIB_CREATOR", "CppLibCreator"),
		("WINDBOT_SYSROOT", "SysRoot"),
	] {
		let value: Option<OsString> = env::var_os(key);
		if let Some(value) = value {
			let mut argument: OsString = OsString::from(format!("-p:{property}="));
			argument.push(value);
			command.arg(argument);
		}
	}
	let mut hasher: DefaultHasher = DefaultHasher::new();
	inputs.sort();
	for input in inputs {
		input.strip_prefix(package)?.hash(&mut hasher);
		fs::read(input)?.hash(&mut hasher);
	}
	command.get_program().hash(&mut hasher);
	for argument in command.get_args() {
		argument.hash(&mut hasher);
	}
	for key in ["DOTNET_ROOT", "SDKROOT", "ANDROID_NDK_HOME", "ANDROID_NDK_ROOT"] {
		env::var_os(key).hash(&mut hasher);
	}
	let fingerprint: String = format!("{:016x}", hasher.finish());
	let fingerprint_path: PathBuf = publish.join(".windbot-source-fingerprint");
	if fs::read_to_string(&fingerprint_path).ok().as_deref() == Some(fingerprint.as_str())
		&& native_outputs_exist(&publish, rid)
	{
		eprintln!("Reusing cached WindBot NativeAOT output for {rid}; source inputs and compiler settings are unchanged.");
		return Ok(publish);
	}
	eprintln!("Compiling bundled WindBot C# sources for {rid} with .NET NativeAOT.");
	run(&mut command).map_err(|error: Box<dyn Error>| format!("WindBot NativeAOT publish failed. Install the .NET 10 SDK and the native toolchain for {rid}, or set WINDBOT_NATIVE_DIR to a matching prebuilt artifact. {error}"))?;
	fs::write(publish.join("native-rid.txt"), format!("{rid}\n"))?;
	if !native_outputs_exist(&publish, rid) {
		return Err("NativeAOT publish did not produce a complete static library and dependency set.".into());
	}
	fs::write(fingerprint_path, fingerprint)?;
	Ok(publish)
}

fn collect_input_files(directory: &Path, files: &mut Vec<PathBuf>) -> Result<(), Box<dyn Error>> {
	for entry in fs::read_dir(directory)
		.map_err(|error: std::io::Error| format!("Cannot read source directory {}: {error}", directory.display()))? {
		let entry: fs::DirEntry = entry?;
		let path: PathBuf = entry.path();
		if entry.file_type()?.is_dir() {
			collect_input_files(&path, files)?;
		} else if entry.file_type()?.is_file() {
			files.push(path);
		}
	}
	Ok(())
}

fn native_outputs_exist(directory: &Path, rid: &str) -> bool {
	let library: &str = if rid.starts_with("win-") { "WindBot.lib" } else { "WindBot.a" };
	let bootstrapper: &str = if rid.starts_with("win-") { "runtime/bootstrapperdll.obj" } else { "runtime/libbootstrapperdll.o" };
	let dependencies: Option<String> = fs::read_to_string(directory.join("native-dependencies.txt")).ok();
	directory.join(library).is_file()
		&& directory.join(bootstrapper).is_file()
		&& fs::read_to_string(directory.join("native-rid.txt")).map(|value: String| value.trim() == rid).unwrap_or(false)
		&& dependencies.map(|value: String| {
			value.lines().any(|line: &str| line.trim() == bootstrapper)
				&& value.lines().map(str::trim).filter(|line: &&str| line.starts_with("runtime/"))
				.all(|line: &str| directory.join(line).is_file())
		}).unwrap_or(false)
}

fn archive_object(source: &Path, output: &Path, target: &str, windows: bool, library: &str) -> Result<(), Box<dyn Error>> {
	let target_key: String = format!("AR_{}", target.replace('-', "_"));
	println!("cargo:rerun-if-env-changed={target_key}");
	let archiver_override: Option<OsString> = env::var_os(&target_key)
		.or_else(|| env::var_os("TARGET_AR"))
		.or_else(|| env::var_os("AR"));
	let llvm_available: bool = Command::new("llvm-ar").arg("--version").output()
		.map(|result: Output| result.status.success()).unwrap_or(false);
	let archiver: OsString = match archiver_override {
		Some(path) => path,
		None if llvm_available => OsString::from("llvm-ar"),
		None if windows => {
			let installer: PathBuf = PathBuf::from(env::var_os("ProgramFiles(x86)").ok_or("Set AR to llvm-ar or lib.exe")?)
				.join("Microsoft Visual Studio/Installer/vswhere.exe");
			let result: Output = run(Command::new(installer).args([
				"-latest", "-products", "*", "-requires", "Microsoft.VisualStudio.Component.VC.Tools.x86.x64",
				"-find", "VC/Tools/MSVC/**/bin/Hostx64/x64/lib.exe",
			]))?;
			let paths: String = String::from_utf8(result.stdout)?;
			OsString::from(paths.lines().next().ok_or("Cannot locate lib.exe; set AR explicitly")?)
		}
		None => OsString::from("ar"),
	};
	let name: String = Path::new(&archiver).file_stem().and_then(|value: &OsStr| value.to_str()).unwrap_or("").to_ascii_lowercase();
	let archive: PathBuf = output.join(if windows { format!("{library}.lib") } else { format!("lib{library}.a") });
	if name == "lib" || name == "llvm-lib" {
		run(Command::new(archiver).arg("/NOLOGO").arg(format!("/OUT:{}", archive.display())).arg(source))?;
	} else {
		run(Command::new(archiver).arg("crs").arg(&archive).arg(source))?;
	}
	Ok(())
}

fn validate_architecture(object: &[u8], rid: &str) -> Result<(), Box<dyn Error>> {
	let machine: u32 = if object.len() >= 20 && object[..4] == *b"\x7fELF" && object[5] == 1 {
		u32::from(u16::from_le_bytes([object[18], object[19]]))
	} else if object.len() >= 8 && object[..4] == [0xcf, 0xfa, 0xed, 0xfe] {
		u32::from_le_bytes([object[4], object[5], object[6], object[7]])
	} else if object.len() >= 8 && object[..4] == [0, 0, 0xff, 0xff] {
		u32::from(u16::from_le_bytes([object[6], object[7]]))
	} else if object.len() >= 20 {
		u32::from(u16::from_le_bytes([object[0], object[1]]))
	} else {
		return Err("NativeAOT bootstrapper is not a recognized object file.".into());
	};
	let expected: u32 = if rid.starts_with("win-") {
		if rid.ends_with("-x64") { 0x8664 } else { 0xaa64 }
	} else if rid.starts_with("osx-") || rid.starts_with("ios") {
		if rid.ends_with("-x64") { 0x01000007 } else { 0x0100000c }
	} else if rid.ends_with("-x64") {
		62
	} else if rid.ends_with("-arm64") {
		183
	} else {
		40
	};
	if machine != expected {
		return Err(format!("Native object architecture {machine:#x} does not match {rid} ({expected:#x}).").into());
	}
	if rid.starts_with("osx-") || rid.starts_with("ios") {
		if object.len() < 32 {
			return Err("Incomplete Mach-O header.".into());
		}
		let expected_platform: u32 = if rid.starts_with("osx-") { 1 } else if rid.starts_with("iossimulator-") { 7 } else { 2 };
		let mut commands: u32 = u32::from_le_bytes(object[16..20].try_into()?);
		let mut offset: usize = 32;
		while commands > 0 && offset + 8 <= object.len() {
			let command: u32 = u32::from_le_bytes(object[offset..offset + 4].try_into()?);
			let size: usize = u32::from_le_bytes(object[offset + 4..offset + 8].try_into()?) as usize;
			if size < 8 || size > object.len() - offset {
				return Err("Invalid Mach-O load command.".into());
			}
			if command == 0x32 && size >= 24 {
				let platform: u32 = u32::from_le_bytes(object[offset + 8..offset + 12].try_into()?);
				if platform != expected_platform {
					return Err(format!("Mach-O platform {platform} does not match {rid} ({expected_platform}).").into());
				}
				return Ok(());
			}
			offset += size;
			commands -= 1;
		}
		return Err("Mach-O bootstrapper does not identify its Apple platform.".into());
	}
	Ok(())
}

fn validate_name(name: &str) -> Result<(), Box<dyn Error>> {
	if name.is_empty() || !name.bytes().all(|byte: u8| byte.is_ascii_alphanumeric() || b"._+-".contains(&byte)) {
		return Err(format!("Invalid native library name: {name}").into());
	}
	Ok(())
}

fn run(command: &mut Command) -> Result<Output, Box<dyn Error>> {
	let output: Output = command.output()?;
	if !output.status.success() {
		return Err(format!("{command:?} failed ({}):\n{}\n{}", output.status,
			String::from_utf8_lossy(&output.stdout), String::from_utf8_lossy(&output.stderr)).into());
	}
	Ok(output)
}

