using ThunderRoad;
using UnityEngine;

namespace StandaloneReactionAnimationMod.Core
{
    public static class SRAMOptions
    {
        public const string Version = "0.1.1";

        public const string CategoryPresentation = "Death Presentation";
        public const string CategoryAnimation = "Animation";
        public const string CategoryDiagnostics = "Diagnostics";

        [ModOption(name = "Enable Mod", order = 0, defaultValueIndex = 1)]
        public static bool EnableMod = true;

        [ModOption(name = "Death Animation Speed Multiplier", category = CategoryAnimation, categoryOrder = 110, order = 0, defaultValueIndex = 3, valueSourceName = nameof(AnimationSpeedProvider), interactionType = (ModOption.InteractionType)2)]
        public static float DeathAnimationSpeedMultiplier = 0.50f;

        [ModOption(name = "Basic Logs", category = CategoryDiagnostics, categoryOrder = 200, order = 0, defaultValueIndex = 0)]
        public static bool BasicLogs = false;

        [ModOption(name = "Diagnostics Logs", category = CategoryDiagnostics, categoryOrder = 200, order = 1, defaultValueIndex = 0)]
        public static bool DiagnosticsLogs = false;

        [ModOption(name = "Verbose Logs", category = CategoryDiagnostics, categoryOrder = 200, order = 2, defaultValueIndex = 0)]
        public static bool VerboseLogs = false;

        public static float GetDeathAnimationSpeedMultiplier()
        {
            return Mathf.Clamp(DeathAnimationSpeedMultiplier, 0.05f, 2f);
        }

        public static ModOptionFloat[] AnimationSpeedProvider() => new[]
        {
            new ModOptionFloat("0.20x", 0.20f), new ModOptionFloat("0.30x", 0.30f), new ModOptionFloat("0.40x", 0.40f),
            new ModOptionFloat("0.50x", 0.50f), new ModOptionFloat("0.60x", 0.60f), new ModOptionFloat("0.70x", 0.70f),
            new ModOptionFloat("0.80x", 0.80f), new ModOptionFloat("0.90x", 0.90f), new ModOptionFloat("1.00x", 1.00f),
        };
    }
}
