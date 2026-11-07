using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;

namespace WindBot
{
    [JsonSerializable(typeof(WindBotInfo))]
    [JsonSerializable(typeof(string[][]))]
    internal partial class NativeJsonContext : JsonSerializerContext
    {
    }

    public static class WindBotExports
    {
        private static readonly object Sync = new object();
        private static int _runningBots;
        private static bool _initialized;
        [ThreadStatic]
        private static string _lastError;

        // Accepts serde_json::to_vec(&Vec<ygopro_cdb_reader::Card>).
        [UnmanagedCallersOnly(EntryPoint = "windbot_load_cards", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static int LoadCards(IntPtr json, UIntPtr length)
        {
            try
            {
                lock (Sync)
                {
                    if (_runningBots != 0)
                        throw new InvalidOperationException("Cannot replace card data while bots are running.");
                    EnsureRuntime();
                    NamedCardsManager.InitJson(CopyInput(json, length));
                }
                _lastError = null;
                return 0;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return 1;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "windbot_run", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static int Run(IntPtr json, UIntPtr length)
        {
            bool registered = false;
            try
            {
                WindBotInfo info = JsonSerializer.Deserialize(CopyInput(json, length), NativeJsonContext.Default.WindBotInfo);
                if (info == null || String.IsNullOrWhiteSpace(info.Host) || info.Port < 1 || info.Port > 65535)
                    throw new ArgumentException("Invalid bot options.");
                lock (Sync)
                {
                    if (!NamedCardsManager.IsInitialized)
                        throw new InvalidOperationException("Call windbot_load_cards before windbot_run.");
                    if (info.Deck != null && !DecksManager.HasDeck(info.Deck))
                        throw new ArgumentException("Unknown deck: " + info.Deck);
                    _runningBots++;
                    registered = true;
                }
                int result = Program.RunBot(info);
                _lastError = result == 0 ? null : "Bot failed; see the WindBot log for details.";
                return result;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return 1;
            }
            finally
            {
                if (registered)
                {
                    lock (Sync)
                        _runningBots--;
                }
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "windbot_last_error", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr LastError()
        {
            return Marshal.StringToCoTaskMemUTF8(_lastError ?? String.Empty);
        }

        [UnmanagedCallersOnly(EntryPoint = "windbot_list", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr List()
        {
            try
            {
                lock (Sync)
                {
                    EnsureRuntime();
                    string json = JsonSerializer.Serialize(DecksManager.GetDecks(), NativeJsonContext.Default.StringArrayArray);
                    _lastError = null;
                    return Marshal.StringToCoTaskMemUTF8(json);
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "windbot_free", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void Free(IntPtr value)
        {
            Marshal.FreeCoTaskMem(value);
        }

        private static void EnsureRuntime()
        {
            if (_initialized)
                return;
            Config.Load(new string[0]);
            Program.Rand = Random.Shared;
            DecksManager.Init();
            _initialized = true;
        }

        private static byte[] CopyInput(IntPtr data, UIntPtr length)
        {
            ulong size = length.ToUInt64();
            if (data == IntPtr.Zero || size == 0 || size > Int32.MaxValue)
                throw new ArgumentException("Expected a nonempty UTF-8 JSON buffer of at most Int32.MaxValue bytes.");
            byte[] bytes = new byte[(int)size];
            Marshal.Copy(data, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
