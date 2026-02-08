using System;
using ThunderRoad;
using UnityEngine;

namespace EnemyLimbDamageManager.Configuration
{
    public static class ELDMModOptions
    {
        public const string VERSION = "0.2.0";

        public const string CategoryPresets = "Limb Damage Presets";
        public const string CategoryLeftLeg = "Left Leg";
        public const string CategoryRightLeg = "Right Leg";
        public const string CategoryLeftArm = "Left Arm";
        public const string CategoryRightArm = "Right Arm";
        public const string CategoryLastStand = "Last Stand";
        public const string CategoryOptional = "Optional";
        public const string CategoryDiagnostics = "Advanced";

        public const string OptionEnableMod = "Enable Mod";
        public const string OptionPresetDamageModel = "Damage Model Preset";
        public const string OptionPresetRecoveryModel = "Recovery Preset";
        public const string OptionPresetSquirmModel = "Squirm Preset";

        public const string OptionLeftLegThreshold = "Left Leg Damage To Disable";
        public const string OptionLeftLegDisableDuration = "Left Leg Disable Duration Seconds";
        public const string OptionLeftLegSquirmMultiplier = "Left Leg Squirm Multiplier";

        public const string OptionRightLegThreshold = "Right Leg Damage To Disable";
        public const string OptionRightLegDisableDuration = "Right Leg Disable Duration Seconds";
        public const string OptionRightLegSquirmMultiplier = "Right Leg Squirm Multiplier";

        public const string OptionLeftArmThreshold = "Left Arm Damage To Disable";
        public const string OptionLeftArmDisableDuration = "Left Arm Disable Duration Seconds";
        public const string OptionLeftArmSquirmMultiplier = "Left Arm Squirm Multiplier";

        public const string OptionRightArmThreshold = "Right Arm Damage To Disable";
        public const string OptionRightArmDisableDuration = "Right Arm Disable Duration Seconds";
        public const string OptionRightArmSquirmMultiplier = "Right Arm Squirm Multiplier";

        public const string OptionRecoveryDelayMultiplier = "Recovery Delay Multiplier";
        public const string OptionRecoveryDamageRetainedPercent = "Damage Retained After Recovery";

        public const string OptionFallFromLegInjury = "Fall From Leg Injury";
        public const string OptionLegImmobilization = "Leg Immobilization";
        public const string OptionArmImmobilization = "Arm Immobilization";
        public const string OptionLastStandEnabled = "Enable Last Stand";
        public const string OptionRecoveryClearsAccumulatedDamage = "Recovery Clears Damage";
        public const string OptionRecoveryRestoresPinForces = "Recovery Restores Pin Forces";

        public const string OptionEnableBasicLogging = "Basic Logs";
        public const string OptionEnableDiagnosticsLogging = "Diagnostics Logs";
        public const string OptionEnableVerboseLogging = "Verbose Logs";
        public const string OptionResetTracking = "Reset Tracking";

        public const string PresetDamageForgiving = "Forgiving";
        public const string PresetDamageDefaultPlus = "DefaultPlus";
        public const string PresetDamageTactical = "Tactical";
        public const string PresetDamageBrutal = "Brutal";
        public const string PresetDamageSevered = "Severed";

        public const string PresetRecoveryDisabled = "Disabled";
        public const string PresetRecoveryDefault = "DefaultLastStand";
        public const string PresetRecoveryQuick = "QuickRecovery";
        public const string PresetRecoverySlow = "SlowRecovery";
        public const string PresetRecoveryComeback = "Comeback";

        public const string PresetSquirmLocked = "Locked";
        public const string PresetSquirmTight = "Tight";
        public const string PresetSquirmStandard = "Standard";
        public const string PresetSquirmLoose = "Loose";
        public const string PresetSquirmWild = "Wild";

        private const float HardcodedMinimumTrackedHitDamage = 0.10f;
        private const bool HardcodedHitsRefreshDisableTimer = true;

        [ModOption(name = OptionEnableMod, order = 0, defaultValueIndex = 1, tooltip = "Master switch for limb damage disable logic")]
        public static bool EnableMod = true;

        [ModOption(name = OptionPresetDamageModel, category = CategoryPresets, categoryOrder = 0, order = 5, defaultValueIndex = 1, valueSourceName = nameof(DamagePresetProvider), tooltip = "Batch writes per-limb damage and duration values")]
        public static string PresetDamageModel = PresetDamageDefaultPlus;

        [ModOption(name = OptionPresetRecoveryModel, category = CategoryPresets, categoryOrder = 0, order = 10, defaultValueIndex = 1, valueSourceName = nameof(RecoveryPresetProvider), tooltip = "Batch writes recovery behavior values")]
        public static string PresetRecoveryModel = PresetRecoveryDefault;

        [ModOption(name = OptionPresetSquirmModel, category = CategoryPresets, categoryOrder = 0, order = 15, defaultValueIndex = 1, valueSourceName = nameof(SquirmPresetProvider), tooltip = "Batch writes all limb squirm multipliers")]
        public static string PresetSquirmModel = PresetSquirmTight;

        [ModOption(name = OptionLeftLegThreshold, category = CategoryLeftLeg, categoryOrder = 100, order = 0, defaultValueIndex = 9, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftLegThresholdDamage = 22f;

        [ModOption(name = OptionLeftLegDisableDuration, category = CategoryLeftLeg, categoryOrder = 100, order = 10, defaultValueIndex = 6, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftLegDisableDurationSeconds = 14f;

        [ModOption(name = OptionLeftLegSquirmMultiplier, category = CategoryLeftLeg, categoryOrder = 100, order = 20, defaultValueIndex = 1, valueSourceName = nameof(SquirmMultiplierProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftLegSquirmMultiplier = 0.10f;

        [ModOption(name = OptionRightLegThreshold, category = CategoryRightLeg, categoryOrder = 110, order = 0, defaultValueIndex = 9, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightLegThresholdDamage = 22f;

        [ModOption(name = OptionRightLegDisableDuration, category = CategoryRightLeg, categoryOrder = 110, order = 10, defaultValueIndex = 6, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightLegDisableDurationSeconds = 14f;

        [ModOption(name = OptionRightLegSquirmMultiplier, category = CategoryRightLeg, categoryOrder = 110, order = 20, defaultValueIndex = 1, valueSourceName = nameof(SquirmMultiplierProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightLegSquirmMultiplier = 0.10f;

        [ModOption(name = OptionLeftArmThreshold, category = CategoryLeftArm, categoryOrder = 120, order = 0, defaultValueIndex = 11, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftArmThresholdDamage = 26f;

        [ModOption(name = OptionLeftArmDisableDuration, category = CategoryLeftArm, categoryOrder = 120, order = 10, defaultValueIndex = 4, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftArmDisableDurationSeconds = 10f;

        [ModOption(name = OptionLeftArmSquirmMultiplier, category = CategoryLeftArm, categoryOrder = 120, order = 20, defaultValueIndex = 1, valueSourceName = nameof(SquirmMultiplierProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftArmSquirmMultiplier = 0.10f;

        [ModOption(name = OptionRightArmThreshold, category = CategoryRightArm, categoryOrder = 130, order = 0, defaultValueIndex = 11, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightArmThresholdDamage = 26f;

        [ModOption(name = OptionRightArmDisableDuration, category = CategoryRightArm, categoryOrder = 130, order = 10, defaultValueIndex = 4, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightArmDisableDurationSeconds = 10f;

        [ModOption(name = OptionRightArmSquirmMultiplier, category = CategoryRightArm, categoryOrder = 130, order = 20, defaultValueIndex = 1, valueSourceName = nameof(SquirmMultiplierProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightArmSquirmMultiplier = 0.10f;

        [ModOption(name = OptionRecoveryDelayMultiplier, category = CategoryLastStand, categoryOrder = 160, order = 10, defaultValueIndex = 3, valueSourceName = nameof(RecoveryDelayProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RecoveryDelayMultiplier = 1.00f;

        [ModOption(name = OptionRecoveryDamageRetainedPercent, category = CategoryLastStand, categoryOrder = 160, order = 30, defaultValueIndex = 5, valueSourceName = nameof(RecoveryRetainedDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RecoveryDamageRetainedPercent = 35f;

        [ModOption(name = OptionFallFromLegInjury, category = CategoryOptional, categoryOrder = 180, order = 0, defaultValueIndex = 1)]
        public static bool FallFromLegInjury = true;

        [ModOption(name = OptionLegImmobilization, category = CategoryOptional, categoryOrder = 180, order = 5, defaultValueIndex = 1)]
        public static bool LegImmobilization = true;

        [ModOption(name = OptionArmImmobilization, category = CategoryOptional, categoryOrder = 180, order = 10, defaultValueIndex = 1)]
        public static bool ArmImmobilization = true;

        [ModOption(name = OptionLastStandEnabled, category = CategoryOptional, categoryOrder = 180, order = 15, defaultValueIndex = 1)]
        public static bool LastStandEnabled = true;

        [ModOption(name = OptionRecoveryClearsAccumulatedDamage, category = CategoryOptional, categoryOrder = 180, order = 20, defaultValueIndex = 1)]
        public static bool RecoveryClearsAccumulatedDamage = true;

        [ModOption(name = OptionRecoveryRestoresPinForces, category = CategoryOptional, categoryOrder = 180, order = 25, defaultValueIndex = 1)]
        public static bool RecoveryRestoresPinForces = true;

        [ModOption(name = OptionEnableBasicLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 0, defaultValueIndex = 1, tooltip = "Enable high-level state logs")]
        public static bool EnableBasicLogging = true;

        [ModOption(name = OptionEnableDiagnosticsLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 2, defaultValueIndex = 0, tooltip = "Enable deeper decision-path logs")]
        public static bool EnableDiagnosticsLogging = false;

        [ModOption(name = OptionEnableVerboseLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 4, defaultValueIndex = 0, tooltip = "Enable high-volume per-hit logs")]
        public static bool EnableVerboseLogging = false;

        [ModOption(name = OptionResetTracking, category = CategoryDiagnostics, categoryOrder = 200, order = 20, defaultValueIndex = 0, tooltip = "Clear all runtime creature/limb tracking")]
        [ModOptionDontSave]
        public static bool ResetTracking;

        public static ModOptionString[] DamagePresetProvider()
        {
            return new[]
            {
                new ModOptionString("Forgiving", PresetDamageForgiving),
                new ModOptionString("Default Plus", PresetDamageDefaultPlus),
                new ModOptionString("Tactical", PresetDamageTactical),
                new ModOptionString("Brutal", PresetDamageBrutal),
                new ModOptionString("Severed", PresetDamageSevered),
            };
        }

        public static ModOptionString[] RecoveryPresetProvider()
        {
            return new[]
            {
                new ModOptionString("Default Last Stand", PresetRecoveryDefault),
                new ModOptionString("Quick Recovery", PresetRecoveryQuick),
                new ModOptionString("Slow Recovery", PresetRecoverySlow),
                new ModOptionString("Comeback", PresetRecoveryComeback),
                new ModOptionString("Disabled", PresetRecoveryDisabled),
            };
        }

        public static ModOptionString[] SquirmPresetProvider()
        {
            return new[]
            {
                new ModOptionString("Locked", PresetSquirmLocked),
                new ModOptionString("Tight", PresetSquirmTight),
                new ModOptionString("Standard", PresetSquirmStandard),
                new ModOptionString("Loose", PresetSquirmLoose),
                new ModOptionString("Wild", PresetSquirmWild),
            };
        }

        public static ModOptionFloat[] ThresholdDamageProvider()
        {
            return new[]
            {
                new ModOptionFloat("6", 6f),
                new ModOptionFloat("8", 8f),
                new ModOptionFloat("10", 10f),
                new ModOptionFloat("12", 12f),
                new ModOptionFloat("14", 14f),
                new ModOptionFloat("16", 16f),
                new ModOptionFloat("18", 18f),
                new ModOptionFloat("20", 20f),
                new ModOptionFloat("21", 21f),
                new ModOptionFloat("22", 22f),
                new ModOptionFloat("24", 24f),
                new ModOptionFloat("26", 26f),
                new ModOptionFloat("28", 28f),
                new ModOptionFloat("30", 30f),
                new ModOptionFloat("35", 35f),
                new ModOptionFloat("40", 40f),
                new ModOptionFloat("50", 50f),
            };
        }

        public static ModOptionFloat[] DisableDurationProvider()
        {
            return new[]
            {
                new ModOptionFloat("2s", 2f),
                new ModOptionFloat("4s", 4f),
                new ModOptionFloat("6s", 6f),
                new ModOptionFloat("8s", 8f),
                new ModOptionFloat("10s", 10f),
                new ModOptionFloat("12s", 12f),
                new ModOptionFloat("14s", 14f),
                new ModOptionFloat("16s", 16f),
                new ModOptionFloat("20s", 20f),
                new ModOptionFloat("25s", 25f),
                new ModOptionFloat("30s", 30f),
                new ModOptionFloat("40s", 40f),
                new ModOptionFloat("60s", 60f),
                new ModOptionFloat("120s", 120f),
            };
        }

        public static ModOptionFloat[] SquirmMultiplierProvider()
        {
            return new[]
            {
                new ModOptionFloat("0.00", 0.00f),
                new ModOptionFloat("0.10", 0.10f),
                new ModOptionFloat("0.20", 0.20f),
                new ModOptionFloat("0.30", 0.30f),
                new ModOptionFloat("0.40", 0.40f),
                new ModOptionFloat("0.50", 0.50f),
                new ModOptionFloat("0.65", 0.65f),
                new ModOptionFloat("0.80", 0.80f),
            };
        }

        public static ModOptionFloat[] RecoveryDelayProvider()
        {
            return new[]
            {
                new ModOptionFloat("0.25x", 0.25f),
                new ModOptionFloat("0.40x", 0.40f),
                new ModOptionFloat("0.60x", 0.60f),
                new ModOptionFloat("1.00x", 1.00f),
                new ModOptionFloat("1.25x", 1.25f),
                new ModOptionFloat("1.50x", 1.50f),
                new ModOptionFloat("1.80x", 1.80f),
                new ModOptionFloat("2.20x", 2.20f),
                new ModOptionFloat("3.00x", 3.00f),
            };
        }

        public static ModOptionFloat[] RecoveryRetainedDamageProvider()
        {
            return new[]
            {
                new ModOptionFloat("0%", 0f),
                new ModOptionFloat("10%", 10f),
                new ModOptionFloat("20%", 20f),
                new ModOptionFloat("25%", 25f),
                new ModOptionFloat("30%", 30f),
                new ModOptionFloat("35%", 35f),
                new ModOptionFloat("40%", 40f),
                new ModOptionFloat("50%", 50f),
                new ModOptionFloat("60%", 60f),
                new ModOptionFloat("75%", 75f),
                new ModOptionFloat("100%", 100f),
            };
        }

        public static bool ApplySelectedPresets()
        {
            string damagePreset = NormalizeDamagePreset(PresetDamageModel);
            string recoveryPreset = NormalizeRecoveryPreset(PresetRecoveryModel);
            string squirmPreset = NormalizeSquirmPreset(PresetSquirmModel);

            ResolveDamagePreset(
                damagePreset,
                out float leftLegThreshold,
                out float rightLegThreshold,
                out float leftArmThreshold,
                out float rightArmThreshold,
                out float leftLegDuration,
                out float rightLegDuration,
                out float leftArmDuration,
                out float rightArmDuration);

            ResolveRecoveryPreset(
                recoveryPreset,
                out bool lastStandEnabled,
                out float recoveryDelayMultiplier,
                out bool clearsDamage,
                out float retainedDamage,
                out bool restorePins);

            ResolveSquirmPreset(
                squirmPreset,
                out float leftLegSquirm,
                out float rightLegSquirm,
                out float leftArmSquirm,
                out float rightArmSquirm);

            bool changed = false;
            changed |= SetFloat(ref LeftLegThresholdDamage, leftLegThreshold, 1f, 200f);
            changed |= SetFloat(ref RightLegThresholdDamage, rightLegThreshold, 1f, 200f);
            changed |= SetFloat(ref LeftArmThresholdDamage, leftArmThreshold, 1f, 200f);
            changed |= SetFloat(ref RightArmThresholdDamage, rightArmThreshold, 1f, 200f);

            changed |= SetFloat(ref LeftLegDisableDurationSeconds, leftLegDuration, 1f, 600f);
            changed |= SetFloat(ref RightLegDisableDurationSeconds, rightLegDuration, 1f, 600f);
            changed |= SetFloat(ref LeftArmDisableDurationSeconds, leftArmDuration, 1f, 600f);
            changed |= SetFloat(ref RightArmDisableDurationSeconds, rightArmDuration, 1f, 600f);

            changed |= SetFloat(ref LeftLegSquirmMultiplier, leftLegSquirm, 0f, 1f);
            changed |= SetFloat(ref RightLegSquirmMultiplier, rightLegSquirm, 0f, 1f);
            changed |= SetFloat(ref LeftArmSquirmMultiplier, leftArmSquirm, 0f, 1f);
            changed |= SetFloat(ref RightArmSquirmMultiplier, rightArmSquirm, 0f, 1f);

            changed |= SetBool(ref LastStandEnabled, lastStandEnabled);
            changed |= SetFloat(ref RecoveryDelayMultiplier, recoveryDelayMultiplier, 0.1f, 10f);
            changed |= SetBool(ref RecoveryClearsAccumulatedDamage, clearsDamage);
            changed |= SetFloat(ref RecoveryDamageRetainedPercent, retainedDamage, 0f, 100f);
            changed |= SetBool(ref RecoveryRestoresPinForces, restorePins);

            return changed;
        }

        public static float GetMinimumTrackedHitDamage()
        {
            return HardcodedMinimumTrackedHitDamage;
        }

        public static bool ShouldRefreshDisableTimerOnHit()
        {
            return HardcodedHitsRefreshDisableTimer;
        }

        public static float GetLimbThresholdDamage(RagdollPart.Type limbType)
        {
            switch (limbType)
            {
                case RagdollPart.Type.LeftLeg:
                case RagdollPart.Type.LeftFoot:
                    return Mathf.Max(1f, LeftLegThresholdDamage);
                case RagdollPart.Type.RightLeg:
                case RagdollPart.Type.RightFoot:
                    return Mathf.Max(1f, RightLegThresholdDamage);
                case RagdollPart.Type.LeftArm:
                case RagdollPart.Type.LeftHand:
                    return Mathf.Max(1f, LeftArmThresholdDamage);
                case RagdollPart.Type.RightArm:
                case RagdollPart.Type.RightHand:
                    return Mathf.Max(1f, RightArmThresholdDamage);
                default:
                    return float.PositiveInfinity;
            }
        }

        public static float GetLimbDisableDurationSeconds(RagdollPart.Type limbType)
        {
            float duration;
            switch (limbType)
            {
                case RagdollPart.Type.LeftLeg:
                case RagdollPart.Type.LeftFoot:
                    duration = LeftLegDisableDurationSeconds;
                    break;
                case RagdollPart.Type.RightLeg:
                case RagdollPart.Type.RightFoot:
                    duration = RightLegDisableDurationSeconds;
                    break;
                case RagdollPart.Type.LeftArm:
                case RagdollPart.Type.LeftHand:
                    duration = LeftArmDisableDurationSeconds;
                    break;
                case RagdollPart.Type.RightArm:
                case RagdollPart.Type.RightHand:
                    duration = RightArmDisableDurationSeconds;
                    break;
                default:
                    return 0f;
            }

            float baseDuration = Mathf.Max(1f, duration);
            if (!LastStandEnabled)
            {
                return float.PositiveInfinity;
            }

            return baseDuration * Mathf.Clamp(RecoveryDelayMultiplier, 0.1f, 10f);
        }

        public static float GetLimbSquirmMultiplier(RagdollPart.Type limbType)
        {
            switch (limbType)
            {
                case RagdollPart.Type.LeftLeg:
                case RagdollPart.Type.LeftFoot:
                    return Mathf.Clamp01(LeftLegSquirmMultiplier);
                case RagdollPart.Type.RightLeg:
                case RagdollPart.Type.RightFoot:
                    return Mathf.Clamp01(RightLegSquirmMultiplier);
                case RagdollPart.Type.LeftArm:
                case RagdollPart.Type.LeftHand:
                    return Mathf.Clamp01(LeftArmSquirmMultiplier);
                case RagdollPart.Type.RightArm:
                case RagdollPart.Type.RightHand:
                    return Mathf.Clamp01(RightArmSquirmMultiplier);
                default:
                    return 0f;
            }
        }

        public static float GetRetainedDamageRatioAfterRecovery()
        {
            if (RecoveryClearsAccumulatedDamage)
            {
                return 0f;
            }

            return Mathf.Clamp01(RecoveryDamageRetainedPercent / 100f);
        }

        public static int GetPresetSelectionHash()
        {
            int hash = 17;
            hash = CombineHash(hash, StringHash(NormalizeDamagePreset(PresetDamageModel)));
            hash = CombineHash(hash, StringHash(NormalizeRecoveryPreset(PresetRecoveryModel)));
            hash = CombineHash(hash, StringHash(NormalizeSquirmPreset(PresetSquirmModel)));
            return hash;
        }

        public static int GetSourceOfTruthHash()
        {
            int hash = 17;
            hash = CombineHash(hash, PercentHash(LeftLegThresholdDamage));
            hash = CombineHash(hash, PercentHash(RightLegThresholdDamage));
            hash = CombineHash(hash, PercentHash(LeftArmThresholdDamage));
            hash = CombineHash(hash, PercentHash(RightArmThresholdDamage));

            hash = CombineHash(hash, PercentHash(LeftLegDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(RightLegDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(LeftArmDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(RightArmDisableDurationSeconds));

            hash = CombineHash(hash, PercentHash(LeftLegSquirmMultiplier));
            hash = CombineHash(hash, PercentHash(RightLegSquirmMultiplier));
            hash = CombineHash(hash, PercentHash(LeftArmSquirmMultiplier));
            hash = CombineHash(hash, PercentHash(RightArmSquirmMultiplier));

            hash = CombineHash(hash, FallFromLegInjury ? 1 : 0);
            hash = CombineHash(hash, LegImmobilization ? 1 : 0);
            hash = CombineHash(hash, ArmImmobilization ? 1 : 0);
            hash = CombineHash(hash, LastStandEnabled ? 1 : 0);
            hash = CombineHash(hash, RecoveryClearsAccumulatedDamage ? 1 : 0);
            hash = CombineHash(hash, RecoveryRestoresPinForces ? 1 : 0);
            hash = CombineHash(hash, PercentHash(RecoveryDelayMultiplier));
            hash = CombineHash(hash, PercentHash(RecoveryDamageRetainedPercent));
            return hash;
        }

        public static string GetSourceOfTruthSummary()
        {
            return "LL=" + LeftLegThresholdDamage.ToString("0") + "/" + LeftLegDisableDurationSeconds.ToString("0") + "s@" + LeftLegSquirmMultiplier.ToString("0.00") +
                   " RL=" + RightLegThresholdDamage.ToString("0") + "/" + RightLegDisableDurationSeconds.ToString("0") + "s@" + RightLegSquirmMultiplier.ToString("0.00") +
                   " LA=" + LeftArmThresholdDamage.ToString("0") + "/" + LeftArmDisableDurationSeconds.ToString("0") + "s@" + LeftArmSquirmMultiplier.ToString("0.00") +
                   " RA=" + RightArmThresholdDamage.ToString("0") + "/" + RightArmDisableDurationSeconds.ToString("0") + "s@" + RightArmSquirmMultiplier.ToString("0.00") +
                   " lastStand=" + LastStandEnabled +
                   " delay=" + RecoveryDelayMultiplier.ToString("0.00") + "x";
        }

        public static string NormalizeDamagePreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("FORGIVE") || token.Contains("LIGHT")) return PresetDamageForgiving;
            if (token.Contains("TACTIC")) return PresetDamageTactical;
            if (token.Contains("BRUTAL") || token.Contains("HARD")) return PresetDamageBrutal;
            if (token.Contains("SEVER") || token.Contains("EXTREME")) return PresetDamageSevered;
            return PresetDamageDefaultPlus;
        }

        public static string NormalizeRecoveryPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("DISABLE") || token.Contains("OFF")) return PresetRecoveryDisabled;
            if (token.Contains("QUICK") || token.Contains("FAST")) return PresetRecoveryQuick;
            if (token.Contains("SLOW")) return PresetRecoverySlow;
            if (token.Contains("COMEBACK") || token.Contains("RETENTION")) return PresetRecoveryComeback;
            return PresetRecoveryDefault;
        }

        public static string NormalizeSquirmPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("LOCK")) return PresetSquirmLocked;
            if (token.Contains("TIGHT")) return PresetSquirmTight;
            if (token.Contains("LOOSE")) return PresetSquirmLoose;
            if (token.Contains("WILD")) return PresetSquirmWild;
            return PresetSquirmStandard;
        }

        private static void ResolveDamagePreset(
            string preset,
            out float leftLegThreshold,
            out float rightLegThreshold,
            out float leftArmThreshold,
            out float rightArmThreshold,
            out float leftLegDuration,
            out float rightLegDuration,
            out float leftArmDuration,
            out float rightArmDuration)
        {
            leftLegThreshold = 22f;
            rightLegThreshold = 22f;
            leftArmThreshold = 26f;
            rightArmThreshold = 26f;
            leftLegDuration = 14f;
            rightLegDuration = 14f;
            leftArmDuration = 10f;
            rightArmDuration = 10f;

            switch (preset)
            {
                case PresetDamageForgiving:
                    leftLegThreshold = 30f;
                    rightLegThreshold = 30f;
                    leftArmThreshold = 35f;
                    rightArmThreshold = 35f;
                    leftLegDuration = 8f;
                    rightLegDuration = 8f;
                    leftArmDuration = 6f;
                    rightArmDuration = 6f;
                    break;
                case PresetDamageTactical:
                    leftLegThreshold = 18f;
                    rightLegThreshold = 18f;
                    leftArmThreshold = 22f;
                    rightArmThreshold = 22f;
                    leftLegDuration = 16f;
                    rightLegDuration = 16f;
                    leftArmDuration = 12f;
                    rightArmDuration = 12f;
                    break;
                case PresetDamageBrutal:
                    leftLegThreshold = 14f;
                    rightLegThreshold = 14f;
                    leftArmThreshold = 16f;
                    rightArmThreshold = 16f;
                    leftLegDuration = 25f;
                    rightLegDuration = 25f;
                    leftArmDuration = 20f;
                    rightArmDuration = 20f;
                    break;
                case PresetDamageSevered:
                    leftLegThreshold = 10f;
                    rightLegThreshold = 10f;
                    leftArmThreshold = 12f;
                    rightArmThreshold = 12f;
                    leftLegDuration = 40f;
                    rightLegDuration = 40f;
                    leftArmDuration = 30f;
                    rightArmDuration = 30f;
                    break;
            }
        }

        private static void ResolveRecoveryPreset(
            string preset,
            out bool enabled,
            out float delayMultiplier,
            out bool clearsDamage,
            out float retainedDamage,
            out bool restorePins)
        {
            enabled = true;
            delayMultiplier = 1.0f;
            clearsDamage = true;
            retainedDamage = 35f;
            restorePins = true;

            switch (preset)
            {
                case PresetRecoveryDisabled:
                    enabled = false;
                    clearsDamage = false;
                    retainedDamage = 100f;
                    restorePins = false;
                    break;
                case PresetRecoveryQuick:
                    delayMultiplier = 0.60f;
                    retainedDamage = 10f;
                    break;
                case PresetRecoverySlow:
                    delayMultiplier = 1.80f;
                    retainedDamage = 20f;
                    break;
                case PresetRecoveryComeback:
                    delayMultiplier = 0.40f;
                    clearsDamage = false;
                    retainedDamage = 60f;
                    break;
            }
        }

        private static void ResolveSquirmPreset(
            string preset,
            out float leftLegSquirm,
            out float rightLegSquirm,
            out float leftArmSquirm,
            out float rightArmSquirm)
        {
            float shared = 0.20f;
            switch (preset)
            {
                case PresetSquirmLocked:
                    shared = 0.00f;
                    break;
                case PresetSquirmTight:
                    shared = 0.10f;
                    break;
                case PresetSquirmLoose:
                    shared = 0.40f;
                    break;
                case PresetSquirmWild:
                    shared = 0.65f;
                    break;
            }

            leftLegSquirm = shared;
            rightLegSquirm = shared;
            leftArmSquirm = shared;
            rightArmSquirm = shared;
        }

        private static bool SetBool(ref bool field, bool value)
        {
            if (field == value)
            {
                return false;
            }

            field = value;
            return true;
        }

        private static bool SetFloat(ref float field, float value, float min, float max)
        {
            float clamped = Mathf.Clamp(value, min, max);
            if (Mathf.Abs(field - clamped) < 0.0001f)
            {
                return false;
            }

            field = clamped;
            return true;
        }

        private static string NormalizeToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string trimmed = value.Trim();
            char[] buffer = new char[trimmed.Length];
            int length = 0;
            for (int i = 0; i < trimmed.Length; i++)
            {
                char c = trimmed[i];
                if (char.IsLetterOrDigit(c))
                {
                    buffer[length++] = char.ToUpperInvariant(c);
                }
            }

            return new string(buffer, 0, length);
        }

        private static int CombineHash(int seed, int value)
        {
            unchecked
            {
                return (seed * 397) ^ value;
            }
        }

        private static int StringHash(string value)
        {
            return string.IsNullOrEmpty(value) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(value);
        }

        private static int PercentHash(float value)
        {
            return Mathf.RoundToInt(value * 100f);
        }
    }
}
