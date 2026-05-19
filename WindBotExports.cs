using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WindBot
{
    public static class WindBotExports
    {
        [UnmanagedCallersOnly(EntryPoint = "windbot_start")]
        public static int WindBotStart(IntPtr argumentsUtf8)
        {
            try
            {
                string arguments = Marshal.PtrToStringUTF8(argumentsUtf8) ?? String.Empty;
                Program.Main(SplitCommandLine(arguments));
                return 0;
            }
            catch (Exception ex)
            {
                try
                {
                    Logger.WriteErrorLine("windbot_start failed: " + ex);
                }
                catch
                {
                    Console.Error.WriteLine("windbot_start failed: " + ex);
                }

                return 1;
            }
        }

        private static string[] SplitCommandLine(string commandLine)
        {
            if (String.IsNullOrWhiteSpace(commandLine))
                return Array.Empty<string>();

            List<string> args = new List<string>();
            StringBuilder current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < commandLine.Length; i++)
            {
                char c = commandLine[i];

                if (c == '\\' && i + 1 < commandLine.Length && commandLine[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (Char.IsWhiteSpace(c) && !inQuotes)
                {
                    AddArgument(args, current);
                    continue;
                }

                current.Append(c);
            }

            AddArgument(args, current);
            return args.ToArray();
        }

        private static void AddArgument(List<string> args, StringBuilder current)
        {
            if (current.Length == 0)
                return;

            args.Add(current.ToString());
            current.Clear();
        }
    }
}
