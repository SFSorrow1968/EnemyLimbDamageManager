using EnemyLimbDamageManager.Configuration;
using UnityEngine;

namespace EnemyLimbDamageManager.Core
{
    internal static class ELDMLog
    {
        private const string Prefix = "[ELDM] ";
        private const bool FocusedLoggingEnabled = true;
        private static readonly string[] FocusKeywords =
        {
            "speed_modifier",
            "combat_slow",
            "standup",
            "death_anim",
            "dead_recovery",
            "last_stand",
            "mock_death",
            "knockout",
            "mimic",
            "limp",
            "pins_",
        };

        public static bool DiagnosticsEnabled => ELDMModOptions.EnableBasicLogging && ELDMModOptions.EnableDiagnosticsLogging;
        public static bool StructuredDiagnosticsEnabled => DiagnosticsEnabled;
        public static bool VerboseEnabled => DiagnosticsEnabled && ELDMModOptions.EnableVerboseLogging;
        public static bool BasicEnabled => ELDMModOptions.EnableBasicLogging;

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

            if (ShouldApplyFocusFilter() && !MatchesFocus(message))
            {
                return;
            }

            Debug.Log(Prefix + message);
        }

        public static void Warn(string message, bool verboseOnly = false)
        {
            if (string.IsNullOrWhiteSpace(message) || !BasicEnabled)
            {
                return;
            }

            if (verboseOnly && !VerboseEnabled)
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

        public static void Diag(string message, bool verboseOnly = false)
        {
            if (string.IsNullOrWhiteSpace(message) || !StructuredDiagnosticsEnabled)
            {
                return;
            }

            if (verboseOnly && !VerboseEnabled)
            {
                return;
            }

            if (ShouldApplyFocusFilter() && !MatchesFocus(message))
            {
                return;
            }

            Debug.Log(Prefix + message);
        }

        private static bool MatchesFocus(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            for (int i = 0; i < FocusKeywords.Length; i++)
            {
                if (message.IndexOf(FocusKeywords[i], System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ShouldApplyFocusFilter()
        {
            return FocusedLoggingEnabled && !DiagnosticsEnabled;
        }
    }
}
