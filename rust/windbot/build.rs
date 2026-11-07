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
		_ => return Err(format!("No CI NativeAOT artifact is available for Rust target {target}.").into()),
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
	let native: PathBuf = match native_override {
		Some(path) => PathBuf::from(path).canonicalize()?,
		None => {
			let archive: PathBuf = package.join(format!("../../windbot-{rid}-static.zip")).canonicalize()
				.map_err(|error: std::io::Error| format!("Place windbot-{rid}-static.zip in the repository root or set WINDBOT_NATIVE_DIR to an extracted artifact: {error}"))?;
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
	};
	let rid_path: PathBuf = native.join("native-rid.txt");
	println!("cargo:rerun-if-changed={}", rid_path.display());
	if rid_path.exists() && fs::read_to_string(&rid_path)?.trim() != rid {
		return Err(format!("The artifact's native-rid.txt does not match {rid}.").into());
	}
	let manifest_path: PathBuf = native.join("native-dependencies.txt");
	println!("cargo:rerun-if-changed={}", manifest_path.display());
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

	// Bundle native libraries into the rlib so downstream crates inherit them.
	// Whole-archive retains WindBot's module data and the bootstrapper constructor.
	println!("cargo:rerun-if-changed={}", windbot.display());
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
			println!("cargo:rerun-if-changed={}", source.display());
			let filename: &str = relative.file_name().and_then(|name: &OsStr| name.to_str()).ok_or("Invalid dependency filename")?;
			if line == bootstrapper_name {
				archive_bootstrapper(&source, &output, &target, windows)?;
				println!("cargo:rustc-link-lib=static:+whole-archive=windbot_bootstrapper");
				has_bootstrapper = true;
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
				println!("cargo:rustc-link-lib=static={name}");
			}
		}
	}
	if !has_bootstrapper {
		return Err("native-dependencies.txt does not list the NativeAOT bootstrapper.".into());
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

fn archive_bootstrapper(source: &Path, output: &Path, target: &str, windows: bool) -> Result<(), Box<dyn Error>> {
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
	let archive: PathBuf = output.join(if windows { "windbot_bootstrapper.lib" } else { "libwindbot_bootstrapper.a" });
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
		return Err(format!("{command:?} failed: {}", String::from_utf8_lossy(&output.stderr)).into());
	}
	Ok(output)
}

