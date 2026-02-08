using EnemyLimbDamageManager.Configuration;
using UnityEngine;

namespace EnemyLimbDamageManager.Core
{
    internal static class ELDMLog
    {
        private const string Prefix = "[ELDM] ";

        public static bool DiagnosticsEnabled => ELDMModOptions.EnableDiagnosticsLogging || VerboseEnabled;
        public static bool StructuredDiagnosticsEnabled => DiagnosticsEnabled;
        public static bool VerboseEnabled => ELDMModOptions.EnableVerboseLogging;
        public static bool BasicEnabled => ELDMModOptions.EnableBasicLogging || DiagnosticsEnabled;

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

            Debug.Log(Prefix + message);
        }
    }
}
