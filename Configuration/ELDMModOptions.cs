using System;
using ThunderRoad;
using UnityEngine;

namespace EnemyLimbDamageManager.Configuration
{
    public static class ELDMModOptions
    {
        public const string VERSION = "0.1.0";

        public const string CategoryPresets = "Limb Damage Presets";
        public const string CategoryGlobal = "Global";
        public const string CategoryLeftLeg = "Left Leg";
        public const string CategoryRightLeg = "Right Leg";
        public const string CategoryLeftArm = "Left Arm";
        public const string CategoryRightArm = "Right Arm";
        public const string CategoryLastStand = "Last Stand";
        public const string CategoryDiagnostics = "Advanced";

        public const string OptionEnableMod = "Enable Mod";
        public const string OptionPresetDamageModel = "Damage Model Preset";
        public const string OptionPresetRecoveryModel = "Recovery Preset";
        public const string OptionPresetResponseModel = "Part Response Preset";

        public const string OptionGlobalDamageScale = "Global Damage Scale";
        public const string OptionMinimumTrackedHitDamage = "Minimum Tracked Hit Damage";
        public const string OptionHitsRefreshDisableTimer = "Hits Refresh Disable Timer";
        public const string OptionLegMoveMultiplierWhileDisabled = "Leg Move Multiplier While Disabled";
        public const string OptionLegDisableForcesFall = "Leg Disable Forces Fall";
        public const string OptionDisableLegPinForces = "Disable Leg Pin Forces";
        public const string OptionDisableArmPinForces = "Disable Arm Pin Forces";

        public const string OptionLeftLegThreshold = "Damage To Disable";
        public const string OptionLeftLegDisableDuration = "Disable Duration Seconds";

        public const string OptionRightLegThreshold = "Damage To Disable";
        public const string OptionRightLegDisableDuration = "Disable Duration Seconds";

        public const string OptionLeftArmThreshold = "Damage To Disable";
        public const string OptionLeftArmDisableDuration = "Disable Duration Seconds";

        public const string OptionRightArmThreshold = "Damage To Disable";
        public const string OptionRightArmDisableDuration = "Disable Duration Seconds";

        public const string OptionLastStandEnabled = "Enable Last Stand";
        public const string OptionRecoveryDelayMultiplier = "Recovery Delay Multiplier";
        public const string OptionRecoveryClearsAccumulatedDamage = "Recovery Clears Damage";
        public const string OptionRecoveryDamageRetainedPercent = "Damage Retained After Recovery";
        public const string OptionRecoveryRestoresPinForces = "Recovery Restores Pin Forces";

        public const string OptionEnableBasicLogging = "Basic Logs";
        public const string OptionEnableDiagnosticsLogging = "Diagnostics Logs";
        public const string OptionEnableVerboseLogging = "Verbose Logs";
        public const string OptionSessionDiagnostics = "Session Diagnostics";
        public const string OptionSummaryInterval = "Summary Interval";
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

        public const string PresetResponseConservative = "Conservative";
        public const string PresetResponseStandard = "Standard";
        public const string PresetResponseTotalShutdown = "TotalShutdown";
        public const string PresetResponseRagdollHeavy = "RagdollHeavy";

        [ModOption(name = OptionEnableMod, order = 0, defaultValueIndex = 1, tooltip = "Master switch for limb damage disable logic")]
        public static bool EnableMod = true;

        [ModOption(name = OptionPresetDamageModel, category = CategoryPresets, categoryOrder = 0, order = 5, defaultValueIndex = 1, valueSourceName = nameof(DamagePresetProvider), tooltip = "Batch writes per-limb thresholds and disable durations")]
        public static string PresetDamageModel = PresetDamageDefaultPlus;

        [ModOption(name = OptionPresetRecoveryModel, category = CategoryPresets, categoryOrder = 0, order = 10, defaultValueIndex = 1, valueSourceName = nameof(RecoveryPresetProvider), tooltip = "Batch writes last-stand recovery controls")]
        public static string PresetRecoveryModel = PresetRecoveryDefault;

        [ModOption(name = OptionPresetResponseModel, category = CategoryPresets, categoryOrder = 0, order = 15, defaultValueIndex = 1, valueSourceName = nameof(ResponsePresetProvider), tooltip = "Batch writes movement and ragdoll response behavior")]
        public static string PresetResponseModel = PresetResponseStandard;

        [ModOption(name = OptionGlobalDamageScale, category = CategoryGlobal, categoryOrder = 20, order = 0, defaultValueIndex = 5, valueSourceName = nameof(DamageScaleProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Scales tracked incoming limb damage before threshold checks")]
        public static float GlobalDamageScale = 1.00f;

        [ModOption(name = OptionMinimumTrackedHitDamage, category = CategoryGlobal, categoryOrder = 20, order = 10, defaultValueIndex = 1, valueSourceName = nameof(MinimumDamageProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Ignore tiny hits below this value to reduce noise")]
        public static float MinimumTrackedHitDamage = 0.50f;

        [ModOption(name = OptionHitsRefreshDisableTimer, category = CategoryGlobal, categoryOrder = 20, order = 20, defaultValueIndex = 1, tooltip = "When a disabled limb is hit again, refresh its disable timer")]
        public static bool HitsRefreshDisableTimer = true;

        [ModOption(name = OptionLegMoveMultiplierWhileDisabled, category = CategoryGlobal, categoryOrder = 20, order = 30, defaultValueIndex = 0, valueSourceName = nameof(MoveMultiplierProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Applied to locomotion while at least one leg is disabled")]
        public static float LegMoveMultiplierWhileDisabled = 0.00f;

        [ModOption(name = OptionLegDisableForcesFall, category = CategoryGlobal, categoryOrder = 20, order = 40, defaultValueIndex = 1, tooltip = "Force a destabilize/fall when a leg disables")]
        public static bool LegDisableForcesFall = true;

        [ModOption(name = OptionDisableLegPinForces, category = CategoryGlobal, categoryOrder = 20, order = 50, defaultValueIndex = 1, tooltip = "Zero leg pin forces while legs are disabled")]
        public static bool DisableLegPinForces = true;

        [ModOption(name = OptionDisableArmPinForces, category = CategoryGlobal, categoryOrder = 20, order = 60, defaultValueIndex = 1, tooltip = "Zero arm pin forces while arms are disabled")]
        public static bool DisableArmPinForces = true;

        [ModOption(name = OptionLeftLegThreshold, category = CategoryLeftLeg, categoryOrder = 100, order = 0, defaultValueIndex = 9, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftLegThresholdDamage = 22f;

        [ModOption(name = OptionLeftLegDisableDuration, category = CategoryLeftLeg, categoryOrder = 100, order = 10, defaultValueIndex = 6, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftLegDisableDurationSeconds = 14f;

        [ModOption(name = OptionRightLegThreshold, category = CategoryRightLeg, categoryOrder = 110, order = 0, defaultValueIndex = 9, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightLegThresholdDamage = 22f;

        [ModOption(name = OptionRightLegDisableDuration, category = CategoryRightLeg, categoryOrder = 110, order = 10, defaultValueIndex = 6, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightLegDisableDurationSeconds = 14f;

        [ModOption(name = OptionLeftArmThreshold, category = CategoryLeftArm, categoryOrder = 120, order = 0, defaultValueIndex = 11, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftArmThresholdDamage = 26f;

        [ModOption(name = OptionLeftArmDisableDuration, category = CategoryLeftArm, categoryOrder = 120, order = 10, defaultValueIndex = 4, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LeftArmDisableDurationSeconds = 10f;

        [ModOption(name = OptionRightArmThreshold, category = CategoryRightArm, categoryOrder = 130, order = 0, defaultValueIndex = 11, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightArmThresholdDamage = 26f;

        [ModOption(name = OptionRightArmDisableDuration, category = CategoryRightArm, categoryOrder = 130, order = 10, defaultValueIndex = 4, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RightArmDisableDurationSeconds = 10f;

        [ModOption(name = OptionLastStandEnabled, category = CategoryLastStand, categoryOrder = 160, order = 0, defaultValueIndex = 1, tooltip = "If enabled, disabled limbs recover after their timer expires")]
        public static bool LastStandEnabled = true;

        [ModOption(name = OptionRecoveryDelayMultiplier, category = CategoryLastStand, categoryOrder = 160, order = 10, defaultValueIndex = 3, valueSourceName = nameof(RecoveryDelayProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Multiplies per-limb disable duration before recovery")]
        public static float RecoveryDelayMultiplier = 1.00f;

        [ModOption(name = OptionRecoveryClearsAccumulatedDamage, category = CategoryLastStand, categoryOrder = 160, order = 20, defaultValueIndex = 1, tooltip = "If enabled, recovered limbs reset accumulated damage to zero")]
        public static bool RecoveryClearsAccumulatedDamage = true;

        [ModOption(name = OptionRecoveryDamageRetainedPercent, category = CategoryLastStand, categoryOrder = 160, order = 30, defaultValueIndex = 5, valueSourceName = nameof(RecoveryRetainedDamageProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Used only when Recovery Clears Damage is disabled")]
        public static float RecoveryDamageRetainedPercent = 35f;

        [ModOption(name = OptionRecoveryRestoresPinForces, category = CategoryLastStand, categoryOrder = 160, order = 40, defaultValueIndex = 1, tooltip = "If enabled, recovery restores pin forces and interaction handles")]
        public static bool RecoveryRestoresPinForces = true;

        [ModOption(name = OptionEnableBasicLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 0, defaultValueIndex = 1, tooltip = "Enable high-level state logs")]
        public static bool EnableBasicLogging = true;

        [ModOption(name = OptionEnableDiagnosticsLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 2, defaultValueIndex = 0, tooltip = "Enable deeper decision-path logs")]
        public static bool EnableDiagnosticsLogging = false;

        [ModOption(name = OptionEnableVerboseLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 4, defaultValueIndex = 0, tooltip = "Enable high-volume per-hit logs")]
        public static bool EnableVerboseLogging = false;

        [ModOption(name = OptionSessionDiagnostics, category = CategoryDiagnostics, categoryOrder = 200, order = 6, defaultValueIndex = 0, tooltip = "Emit structured periodic summaries")]
        public static bool SessionDiagnostics = false;

        [ModOption(name = OptionSummaryInterval, category = CategoryDiagnostics, categoryOrder = 200, order = 10, defaultValueIndex = 1, valueSourceName = nameof(SummaryIntervalProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Structured summary cadence")]
        public static float SummaryIntervalSeconds = 5f;

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

        public static ModOptionString[] ResponsePresetProvider()
        {
            return new[]
            {
                new ModOptionString("Conservative", PresetResponseConservative),
                new ModOptionString("Standard", PresetResponseStandard),
                new ModOptionString("Total Shutdown", PresetResponseTotalShutdown),
                new ModOptionString("Ragdoll Heavy", PresetResponseRagdollHeavy),
            };
        }

        public static ModOptionFloat[] DamageScaleProvider()
        {
            return new[]
            {
                new ModOptionFloat("0.50x", 0.50f),
                new ModOptionFloat("0.65x", 0.65f),
                new ModOptionFloat("0.75x", 0.75f),
                new ModOptionFloat("0.85x", 0.85f),
                new ModOptionFloat("0.95x", 0.95f),
                new ModOptionFloat("1.00x", 1.00f),
                new ModOptionFloat("1.10x", 1.10f),
                new ModOptionFloat("1.25x", 1.25f),
                new ModOptionFloat("1.40x", 1.40f),
                new ModOptionFloat("1.60x", 1.60f),
                new ModOptionFloat("1.80x", 1.80f),
                new ModOptionFloat("2.00x", 2.00f),
            };
        }

        public static ModOptionFloat[] MinimumDamageProvider()
        {
            return new[]
            {
                new ModOptionFloat("0.10", 0.10f),
                new ModOptionFloat("0.50", 0.50f),
                new ModOptionFloat("1.00", 1.00f),
                new ModOptionFloat("2.00", 2.00f),
                new ModOptionFloat("3.00", 3.00f),
                new ModOptionFloat("5.00", 5.00f),
            };
        }

        public static ModOptionFloat[] MoveMultiplierProvider()
        {
            return new[]
            {
                new ModOptionFloat("0.00", 0.00f),
                new ModOptionFloat("0.05", 0.05f),
                new ModOptionFloat("0.10", 0.10f),
                new ModOptionFloat("0.15", 0.15f),
                new ModOptionFloat("0.25", 0.25f),
                new ModOptionFloat("0.35", 0.35f),
                new ModOptionFloat("0.50", 0.50f),
                new ModOptionFloat("0.65", 0.65f),
                new ModOptionFloat("0.80", 0.80f),
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

        public static ModOptionFloat[] SummaryIntervalProvider()
        {
            return new[]
            {
                new ModOptionFloat("2s", 2f),
                new ModOptionFloat("5s", 5f),
                new ModOptionFloat("10s", 10f),
                new ModOptionFloat("15s", 15f),
                new ModOptionFloat("30s", 30f),
            };
        }

        public static bool ApplySelectedPresets()
        {
            string damagePreset = NormalizeDamagePreset(PresetDamageModel);
            string recoveryPreset = NormalizeRecoveryPreset(PresetRecoveryModel);
            string responsePreset = NormalizeResponsePreset(PresetResponseModel);

            ResolveDamagePreset(
                damagePreset,
                out float globalScale,
                out float minHit,
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

            ResolveResponsePreset(
                responsePreset,
                out float legMoveMultiplier,
                out bool forceFall,
                out bool disableLegPins,
                out bool disableArmPins);

            bool changed = false;
            changed |= SetFloat(ref GlobalDamageScale, globalScale, 0.25f, 3f);
            changed |= SetFloat(ref MinimumTrackedHitDamage, minHit, 0.1f, 10f);

            changed |= SetFloat(ref LeftLegThresholdDamage, leftLegThreshold, 1f, 200f);
            changed |= SetFloat(ref RightLegThresholdDamage, rightLegThreshold, 1f, 200f);
            changed |= SetFloat(ref LeftArmThresholdDamage, leftArmThreshold, 1f, 200f);
            changed |= SetFloat(ref RightArmThresholdDamage, rightArmThreshold, 1f, 200f);

            changed |= SetFloat(ref LeftLegDisableDurationSeconds, leftLegDuration, 1f, 600f);
            changed |= SetFloat(ref RightLegDisableDurationSeconds, rightLegDuration, 1f, 600f);
            changed |= SetFloat(ref LeftArmDisableDurationSeconds, leftArmDuration, 1f, 600f);
            changed |= SetFloat(ref RightArmDisableDurationSeconds, rightArmDuration, 1f, 600f);

            changed |= SetFloat(ref LegMoveMultiplierWhileDisabled, legMoveMultiplier, 0f, 1f);
            changed |= SetBool(ref LegDisableForcesFall, forceFall);
            changed |= SetBool(ref DisableLegPinForces, disableLegPins);
            changed |= SetBool(ref DisableArmPinForces, disableArmPins);

            changed |= SetBool(ref LastStandEnabled, lastStandEnabled);
            changed |= SetFloat(ref RecoveryDelayMultiplier, recoveryDelayMultiplier, 0.1f, 10f);
            changed |= SetBool(ref RecoveryClearsAccumulatedDamage, clearsDamage);
            changed |= SetFloat(ref RecoveryDamageRetainedPercent, retainedDamage, 0f, 100f);
            changed |= SetBool(ref RecoveryRestoresPinForces, restorePins);

            return changed;
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
            hash = CombineHash(hash, StringHash(NormalizeResponsePreset(PresetResponseModel)));
            return hash;
        }

        public static int GetSourceOfTruthHash()
        {
            int hash = 17;
            hash = CombineHash(hash, PercentHash(GlobalDamageScale));
            hash = CombineHash(hash, PercentHash(MinimumTrackedHitDamage));
            hash = CombineHash(hash, HitsRefreshDisableTimer ? 1 : 0);
            hash = CombineHash(hash, PercentHash(LegMoveMultiplierWhileDisabled));
            hash = CombineHash(hash, LegDisableForcesFall ? 1 : 0);
            hash = CombineHash(hash, DisableLegPinForces ? 1 : 0);
            hash = CombineHash(hash, DisableArmPinForces ? 1 : 0);

            hash = CombineHash(hash, PercentHash(LeftLegThresholdDamage));
            hash = CombineHash(hash, PercentHash(RightLegThresholdDamage));
            hash = CombineHash(hash, PercentHash(LeftArmThresholdDamage));
            hash = CombineHash(hash, PercentHash(RightArmThresholdDamage));

            hash = CombineHash(hash, PercentHash(LeftLegDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(RightLegDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(LeftArmDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(RightArmDisableDurationSeconds));

            hash = CombineHash(hash, LastStandEnabled ? 1 : 0);
            hash = CombineHash(hash, PercentHash(RecoveryDelayMultiplier));
            hash = CombineHash(hash, RecoveryClearsAccumulatedDamage ? 1 : 0);
            hash = CombineHash(hash, PercentHash(RecoveryDamageRetainedPercent));
            hash = CombineHash(hash, RecoveryRestoresPinForces ? 1 : 0);
            return hash;
        }
        public static string GetSourceOfTruthSummary()
        {
            return "damageScale=" + GlobalDamageScale.ToString("0.00") +
                   " minHit=" + MinimumTrackedHitDamage.ToString("0.00") +
                   " LL=" + LeftLegThresholdDamage.ToString("0") + "/" + LeftLegDisableDurationSeconds.ToString("0") + "s" +
                   " RL=" + RightLegThresholdDamage.ToString("0") + "/" + RightLegDisableDurationSeconds.ToString("0") + "s" +
                   " LA=" + LeftArmThresholdDamage.ToString("0") + "/" + LeftArmDisableDurationSeconds.ToString("0") + "s" +
                   " RA=" + RightArmThresholdDamage.ToString("0") + "/" + RightArmDisableDurationSeconds.ToString("0") + "s" +
                   " moveWhileLegDisabled=" + LegMoveMultiplierWhileDisabled.ToString("0.00") +
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

        public static string NormalizeResponsePreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("CONSERV")) return PresetResponseConservative;
            if (token.Contains("TOTAL") || token.Contains("SHUTDOWN")) return PresetResponseTotalShutdown;
            if (token.Contains("RAGDOLL") || token.Contains("HEAVY")) return PresetResponseRagdollHeavy;
            return PresetResponseStandard;
        }

        private static void ResolveDamagePreset(
            string preset,
            out float globalScale,
            out float minHit,
            out float leftLegThreshold,
            out float rightLegThreshold,
            out float leftArmThreshold,
            out float rightArmThreshold,
            out float leftLegDuration,
            out float rightLegDuration,
            out float leftArmDuration,
            out float rightArmDuration)
        {
            globalScale = 1.00f;
            minHit = 0.50f;
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
                    globalScale = 0.85f;
                    minHit = 1.0f;
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
                    globalScale = 1.10f;
                    minHit = 0.5f;
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
                    globalScale = 1.35f;
                    minHit = 0.25f;
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
                    globalScale = 1.60f;
                    minHit = 0.10f;
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
                    delayMultiplier = 1.0f;
                    clearsDamage = false;
                    retainedDamage = 100f;
                    restorePins = false;
                    break;
                case PresetRecoveryQuick:
                    enabled = true;
                    delayMultiplier = 0.60f;
                    clearsDamage = true;
                    retainedDamage = 10f;
                    restorePins = true;
                    break;
                case PresetRecoverySlow:
                    enabled = true;
                    delayMultiplier = 1.80f;
                    clearsDamage = true;
                    retainedDamage = 20f;
                    restorePins = true;
                    break;
                case PresetRecoveryComeback:
                    enabled = true;
                    delayMultiplier = 0.40f;
                    clearsDamage = false;
                    retainedDamage = 60f;
                    restorePins = true;
                    break;
            }
        }

        private static void ResolveResponsePreset(
            string preset,
            out float legMoveMultiplier,
            out bool forceFall,
            out bool disableLegPins,
            out bool disableArmPins)
        {
            legMoveMultiplier = 0f;
            forceFall = true;
            disableLegPins = true;
            disableArmPins = true;

            switch (preset)
            {
                case PresetResponseConservative:
                    legMoveMultiplier = 0.25f;
                    forceFall = false;
                    disableLegPins = false;
                    disableArmPins = false;
                    break;
                case PresetResponseTotalShutdown:
                    legMoveMultiplier = 0f;
                    forceFall = true;
                    disableLegPins = true;
                    disableArmPins = true;
                    break;
                case PresetResponseRagdollHeavy:
                    legMoveMultiplier = 0.05f;
                    forceFall = true;
                    disableLegPins = true;
                    disableArmPins = true;
                    break;
            }
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
