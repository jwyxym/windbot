# WindBot

WindBot is a C# duel bot for [YGOPro](https://github.com/Fluorohydride/ygopro). It is compatible with [YGOSharp](https://github.com/IceYGO/ygosharp) and [SRVPro](https://github.com/mycard/srvpro) servers.

## Usage

1. Install the .NET 10 SDK (or a newer SDK) and run `dotnet build WindBot.sln -c Release`.
2. Read the CDB outside WindBot using `ygopro_cdb_reader`, then save `serde_json::to_vec(&cards)` as `cards.json`. WindBot contains no SQLite implementation or native SQLite dependency.
3. Start a YGOPro host.
4. Run WindBot with options in `Key=Value` format, for example:
   ```text
   dotnet bin/Release/net10.0/WindBot.dll Name=WindBot Deck=Blue-Eyes Host=127.0.0.1 Port=7911 CardDataPath=cards.json
   ```
5. The bot will connect to the host and start dueling.
6. Refer to `BotWrapper/bot.conf` or the `[Deck]` declarations of each file in `Game/AI/Decks` for available decks.

Contributor guidance for creating and modifying deck AIs is available in `AGENTS.md`.

## Command-line options

- `Name`: Optional nickname override. If omitted, uses the selected bot's Chinese name from `Game/AI/DecksManager.Registry.cs`.
- `Deck`: The built-in deck AI to use. Leave this empty to select a random deck with the `Normal` level. Deck names are defined by the executors in `Game/AI/Decks`.
- `DeckFile`: Optional external `.ydk` override. If omitted, the selected AI uses its embedded deck, regardless of files in the working directory. `Lucky` and `Test` are custom-deck test AIs: their `AI_Test.ydk` is absent from the repository, so they require `DeckFile`.
- `Dialog`: Optional external JSON file or embedded dialog name. If omitted, uses the selected bot's embedded dialog configured in `Game/AI/DecksManager.Registry.cs`.
- `Host`: The host name or IP address of the YGOPro server.
- `Port`: The server port.
- `HostInfo`: The room password.
- `Version`: The YGOPro protocol version.
- `Hand`: For testing your deck, forces the bot's rock-paper-scissors choice: `1` for Scissors, `2` for Rock, or `3` for Paper. The default behavior is random for most deck AIs.
- `Chat`: Set to `False` to disable bot chat. The default is `True`.
- `Debug`: Set to `True` to print detailed card-movement logs. The default is `False`.
- `Config`: Loads additional options from a configuration file. Command-line options take precedence.
- `CardDataPath`: Path to the UTF-8 JSON array serialized from `Vec<ygopro_cdb_reader::Card>`. Default: `cards.json`. The native library accepts the same data directly from memory.
- `ServerMode` and `ServerPort`: Enable the HTTP server mode and set its port. The default port is `2399`.

## Server mode

Server mode exposes an HTTP endpoint that starts a bot for each valid request. Example:

```text
http://127.0.0.1:2399/?name=%E2%91%A8&deck=Blue-Eyes&host=127.0.0.1&port=7911&dialog=cirno.zh-CN
```

Query parameter names are lowercase. Supported parameters are `name`, `deck`, `host`, `port`, `dialog`, `version`, `password`, `hand`, `debug`, and `chat`. The `host` and `port` parameters are required. If `name` is omitted, uses the selected bot's Chinese name. `deckfile` is not supported.

The HTTP listener binds to all interfaces. On Windows, run with sufficient privileges or reserve the URL for the selected port (using `2399` here):

```text
netsh http add urlacl url=http://+:2399/ user=Everyone
```

## Known issue

- When a chain contains multiple activations that rely on preselected `AI.SelectCard` choices, a later activation may consume the wrong selection.

## Native static library

Publish on a host with the matching native toolchain:

```sh
dotnet publish WindBot.csproj -c Release -r linux-x64 -p:NativeLib=Static -o artifacts/native/linux-x64
```

| Platform | RIDs | Toolchain / output |
| --- | --- | --- |
| Windows | `win-x64`, `win-arm64` | Windows + Visual Studio C++ tools / `WindBot.lib` |
| macOS | `osx-x64`, `osx-arm64` | macOS + Xcode command line tools / `WindBot.a` |
| Linux | `linux-x64`, `linux-arm64` | matching Linux host + clang + zlib headers / `WindBot.a` |
| Android | `linux-bionic-arm` (`armeabi-v7a`), `linux-bionic-arm64` (`arm64-v8a`), `linux-bionic-x64` (`x86_64`) | Linux + Android NDK clang, llvm-ar and sysroot / `WindBot.a` |
| iOS | `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64` | macOS + Xcode / `WindBot.a` |

Android and iOS use experimental NativeAOT support, not Mono. Windows archives use the native `.lib` format; renaming one to `.a` does not make it a Unix archive. Separate artifacts are built for each RID by `.github/workflows/native-static.yml`. Device and simulator archives must remain separate.

Both projects target `net10.0`; CI installs the .NET 10 SDK on GitHub runners. Android publishing explicitly selects NDK clang, llvm-ar and sysroot; .NET selects the architecture and Android target triple from the RID. Android 32-bit `x86` is not included because .NET 10 does not provide the corresponding NativeAOT runtime pack. The .NET 10 configuration has not been compiled locally; build and final-link validation remain pending.

NativeAOT static publishing produces the WindBot object archive, not a complete merged runtime. Distribute the adjacent `runtime/` directory and `native-dependencies.txt` with it. Link the listed runtime objects/libraries and the platform SDK libraries into the host, retain the NativeAOT module initialization sections, and use a compatible C/C++ toolchain. The Windows .NET 8 archive was verified with MSVC `/MT`; mixing `/MD` introduces a CRT conflict. Only one NativeAOT runtime can be linked into a loadable module. See the [official static library sample](https://github.com/dotnet/samples/tree/main/core/nativeaot/NativeLibrary) for linker details. Static libraries are not an officially supported .NET deployment contract; validate the final linked app on each target.

The C ABI is declared in `native/windbot.h`:

1. Read and merge CDBs in the host using `ygopro_cdb_reader`. Serialize the resulting `Vec<Card>` using `serde_json::to_vec(&cards)`.
2. Call `windbot_load_cards(bytes.as_ptr(), bytes.len())`. WindBot copies the data before returning. It accepts the reader's exact JSON field names, including `card_type`, `link_marker`, `hint: [String; 16]` and `setcode: [u16; 16]`. `level`, `lscale`, `rscale` and `link_marker` are already decoded by the reader and are not decoded again. All sixteen set codes are retained. Duplicate card codes use the last record; an invalid batch leaves the previous database intact.
3. Call `windbot_run` with UTF-8 JSON options, for example `{"Deck":"Blue-Eyes","Host":"127.0.0.1","Port":7911}`. Option names match `WindBotInfo` and are case-sensitive; omitted options retain their constructor defaults. The call blocks until the bot disconnects. Use a host worker thread per bot. The host controls process lifetime; this ABI does not provide cancellation of a running bot.
4. Nonzero status means failure. Call `windbot_last_error` on the same thread and release its returned string using `windbot_free`.
5. `windbot_list` returns allocated UTF-8 JSON rows `[Chinese bot name, AI name, embedded deck name, embedded dialog name, level]`; release the result with `windbot_free`. Chinese names follow the `ygopro3` bot list; pass `AI name` as the `Deck` option. For example, `Blue-Eyes` has the default nickname `复制植物-青眼`.

Card data can be replaced between native runs, but `windbot_load_cards` rejects replacement while any native bot is running. Managed hosts can instead call `NamedCardsManager.Init(IEnumerable<CardRecord>)` before creating bots; they must likewise avoid replacing data during duels. JSON loading uses source-generated serializers for AOT compatibility. The ABI takes serialized data, not pointers to Rust `String` or `Vec` values.

Decks and dialogs are embedded directly by MSBuild. Maintain the AOT-safe factory registry in `Game/AI/DecksManager.Registry.cs` by hand: each entry defines the AI identifier, Chinese bot name, embedded deck, executor factory, level and embedded dialog. No generation script or Node.js is required. When adding an AI or changing its metadata, update the registry and its `[Deck]` declaration together. Preserve each AI's original level and the `Normal` random pool.

## TODO list

- Add and update deck AIs.
- Add linked-zone and non-linked-zone preferences to `AI.SelectPlace`.
- Add an `AI.SelectTribute` preselection interface.
- Keep the known-card enums up to date.
- Add more executors for commonly used cards.

## Related projects

### BotWrapper

`BotWrapper` is a lightweight launcher for YGOPro's built-in bot mode. It converts the arguments supplied by YGOPro into WindBot options, supports random bot selection, and starts `WindBot.exe`.

`BotWrapper/bot.conf` is in the format that YGOPro expects for its bot mode.

### Template generator

The [WindBot template generator](https://mercury233.me/windbot/gen.html) creates a deck AI template from a YGOPro `.ydk` file.

### WindBot Arena

[WindBot Arena](https://github.com/mercury233/windbot-arena) is a self-hosted automated duel experimentation console for running regression tests, deck challenges, smoke tests, and win-rate rankings with WindBot.
