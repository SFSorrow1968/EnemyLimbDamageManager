using UnityEngine;

namespace StandaloneReactionAnimationMod.Core
{
    internal static class SRAMLog
    {
        private const string Prefix = "[SRAM] ";

        public static bool BasicEnabled => SRAMOptions.BasicLogs;
        public static bool DiagnosticsEnabled => SRAMOptions.BasicLogs && SRAMOptions.DiagnosticsLogs;
        public static bool VerboseEnabled => DiagnosticsEnabled && SRAMOptions.VerboseLogs;

        public static void Info(string message, bool verboseOnly = false)
        {
            if (string.IsNullOrWhiteSpace(message) || !BasicEnabled)
            {
                return;
            }

            if (verboseOnly && !VerboseEnabled)
            {
                return;
            }

            Debug.Log(Prefix + message);
        }

        public static void Diag(string message, bool verboseOnly = false)
        {
            if (string.IsNullOrWhiteSpace(message) || !DiagnosticsEnabled)
            {
                return;
            }

            if (verboseOnly && !VerboseEnabled)
            {
                return;
            }

            Debug.Log(Prefix + message);
        }

        public static void Warn(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || !BasicEnabled)
            {
                return;
            }

            Debug.LogWarning(Prefix + message);
        }

        public static void Error(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Debug.LogError(Prefix + message);
        }
    }
}
