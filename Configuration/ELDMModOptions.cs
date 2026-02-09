using System;
using ThunderRoad;
using UnityEngine;

namespace EnemyLimbDamageManager.Configuration
{
    public static class ELDMModOptions
    {
        public const string VERSION = "0.5.0";

        public const string CategoryLimbPresets = "Limb Presets";
        public const string CategoryLastStandPresets = "Last Stand Presets";
        public const string CategoryLegs = "Legs";
        public const string CategoryArms = "Arms";
        public const string CategoryLastStand = "Last Stand";
        public const string CategoryOptional = "Optional";
        public const string CategoryDiagnostics = "Advanced";

        public const string OptionEnableMod = "Enable Mod";
        public const string OptionPresetDamageModel = "Damage Preset";
        public const string OptionPresetLimbSlowModel = "Limb Slow Preset";
        public const string OptionPresetLastStandModel = "Last Stand Preset";
        public const string OptionPresetLastStandSlowModel = "Last Stand Slow Preset";

        public const string OptionLegThreshold = "Leg Damage To Disable";
        public const string OptionLegDisableDuration = "Leg Disable Duration";
        public const string OptionLegSlowDebuffPercent = "Leg Slow Debuff";
        public const string OptionLegSlowStacks = "Leg Slow Stacking";

        public const string OptionArmThreshold = "Arm Damage To Disable";
        public const string OptionArmDisableDuration = "Arm Disable Duration";
        public const string OptionArmSlowDebuffPercent = "Arm Slow Debuff";
        public const string OptionArmSlowStacks = "Arm Slow Stacking";

        public const string OptionRecoveryDelayMultiplier = "Recovery Delay Multiplier";
        public const string OptionRecoveryDamageReductionPercent = "Recovery Damage Reduction";
        public const string OptionRecoveryRestoresPinForces = "Recovery Restores Pin Forces";
        public const string OptionDeadRevivalChancePercent = "Dead Recovery Chance";
        public const string OptionMaxDeadRecoveries = "Max Death Recoveries";
        public const string OptionDeadRecoverySlowPercent = "Death Recovery Slow Debuff";
        public const string OptionKnockoutDurationSeconds = "Knockout Duration";
        public const string OptionKnockoutRecoverySlowPercent = "Knockout Recovery Slow Debuff";
        public const string OptionSlowDebuffsStack = "Slow Debuffs Stack";

        public const string OptionFallFromLegInjury = "Fall From Leg Injury";
        public const string OptionLegImmobilization = "Leg Immobilization";
        public const string OptionArmImmobilization = "Arm Immobilization";
        public const string OptionLastStandEnabled = "Enable Last Stand";
        public const string OptionKnockoutEnabled = "Enable Knockout";

        public const string OptionEnableBasicLogging = "Basic Logs";
        public const string OptionEnableDiagnosticsLogging = "Diagnostics Logs";
        public const string OptionEnableVerboseLogging = "Verbose Logs";

        public const string PresetDamageLenient = "Lenient";
        public const string PresetDamageDefault = "Default";
        public const string PresetDamageSevere = "Severe";
        public const string PresetDamageBrutal = "Brutal";
        public const string PresetDamageExtreme = "Extreme";

        public const string PresetLimbSlowLow = "Low";
        public const string PresetLimbSlowDefault = "Default";
        public const string PresetLimbSlowHigh = "High";

        public const string PresetLastStandLess = "Less";
        public const string PresetLastStandDefault = "Default";
        public const string PresetLastStandMore = "More";
        private const string PresetLastStandOffLegacy = "OffLegacy";

        public const string PresetLastStandSlowLight = "Light";
        public const string PresetLastStandSlowDefault = "Default";
        public const string PresetLastStandSlowHeavy = "Heavy";
        public const string PresetLastStandSlowExtreme = "Extreme";

        private const float HardcodedMinimumTrackedHitDamage = 0.10f;
        private const bool HardcodedHitsRefreshDisableTimer = true;

        public struct SourceSnapshot
        {
            public float LegsThresholdDamage;
            public float LegsDisableDurationSeconds;
            public float LegsSlowDebuffPercent;
            public bool LegsSlowStacks;
            public float ArmsThresholdDamage;
            public float ArmsDisableDurationSeconds;
            public float ArmsSlowDebuffPercent;
            public bool ArmsSlowStacks;
            public bool LastStandEnabled;
            public bool FallFromLegInjury;
            public bool LegImmobilization;
            public bool ArmImmobilization;
            public bool KnockoutEnabled;
            public float RecoveryDelayMultiplier;
            public float RecoveryDamageReductionPercent;
            public bool RecoveryRestoresPinForces;
            public float DeadRevivalChancePercent;
            public int MaxDeadRecoveries;
            public float DeadRecoverySlowPercent;
            public float KnockoutDurationSeconds;
            public float KnockoutRecoverySlowPercent;
            public bool SlowDebuffsStack;
        }

        [ModOption(name = OptionEnableMod, order = 0, defaultValueIndex = 1, tooltip = "Master switch for limb disable, knockout, and last-stand systems")]
        public static bool EnableMod = true;

        [ModOption(name = OptionPresetDamageModel, category = CategoryLimbPresets, categoryOrder = 0, order = 0, defaultValueIndex = 1, valueSourceName = nameof(DamagePresetProvider), tooltip = "Batch writes limb thresholds + disable durations")]
        public static string PresetDamageModel = PresetDamageDefault;

        [ModOption(name = OptionPresetLimbSlowModel, category = CategoryLimbPresets, categoryOrder = 0, order = 10, defaultValueIndex = 1, valueSourceName = nameof(LimbSlowPresetProvider), tooltip = "Batch writes limb slow debuffs and limb stacking toggles")]
        public static string PresetLimbSlowModel = PresetLimbSlowDefault;

        [ModOption(name = OptionPresetLastStandModel, category = CategoryLastStandPresets, categoryOrder = 1, order = 0, defaultValueIndex = 1, valueSourceName = nameof(LastStandPresetProvider), tooltip = "Batch writes revive and recovery behavior")]
        public static string PresetLastStandModel = PresetLastStandDefault;

        [ModOption(name = OptionPresetLastStandSlowModel, category = CategoryLastStandPresets, categoryOrder = 1, order = 10, defaultValueIndex = 1, valueSourceName = nameof(LastStandSlowPresetProvider), tooltip = "Batch writes knockout/death-recovery slowness and stacking style")]
        public static string PresetLastStandSlowModel = PresetLastStandSlowDefault;

        [ModOption(name = OptionLegThreshold, category = CategoryLegs, categoryOrder = 100, order = 0, defaultValueIndex = 8, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LegsThresholdDamage = 22f;

        [ModOption(name = OptionLegDisableDuration, category = CategoryLegs, categoryOrder = 100, order = 10, defaultValueIndex = 6, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LegsDisableDurationSeconds = 14f;

        [ModOption(name = OptionLegSlowDebuffPercent, category = CategoryLegs, categoryOrder = 100, order = 20, defaultValueIndex = 1, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LegsSlowDebuffPercent = 10f;

        [ModOption(name = OptionLegSlowStacks, category = CategoryLegs, categoryOrder = 100, order = 30, defaultValueIndex = 1)]
        public static bool LegsSlowStacks = true;

        [ModOption(name = OptionArmThreshold, category = CategoryArms, categoryOrder = 110, order = 0, defaultValueIndex = 10, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2)]
        public static float ArmsThresholdDamage = 26f;

        [ModOption(name = OptionArmDisableDuration, category = CategoryArms, categoryOrder = 110, order = 10, defaultValueIndex = 4, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float ArmsDisableDurationSeconds = 10f;

        [ModOption(name = OptionArmSlowDebuffPercent, category = CategoryArms, categoryOrder = 110, order = 20, defaultValueIndex = 1, valueSourceName = nameof(FivePercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float ArmsSlowDebuffPercent = 5f;

        [ModOption(name = OptionArmSlowStacks, category = CategoryArms, categoryOrder = 110, order = 30, defaultValueIndex = 1)]
        public static bool ArmsSlowStacks = true;

        [ModOption(name = OptionRecoveryDelayMultiplier, category = CategoryLastStand, categoryOrder = 120, order = 0, defaultValueIndex = 3, valueSourceName = nameof(RecoveryDelayProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RecoveryDelayMultiplier = 1.00f;

        [ModOption(name = OptionRecoveryDamageReductionPercent, category = CategoryLastStand, categoryOrder = 120, order = 10, defaultValueIndex = 4, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RecoveryDamageReductionPercent = 40f;

        [ModOption(name = OptionRecoveryRestoresPinForces, category = CategoryLastStand, categoryOrder = 120, order = 20, defaultValueIndex = 1)]
        public static bool RecoveryRestoresPinForces = true;

        [ModOption(name = OptionDeadRevivalChancePercent, category = CategoryLastStand, categoryOrder = 120, order = 30, defaultValueIndex = 1, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float DeadRevivalChancePercent = 10f;

        [ModOption(name = OptionMaxDeadRecoveries, category = CategoryLastStand, categoryOrder = 120, order = 35, defaultValueIndex = 2, valueSourceName = nameof(MaxDeadRecoveriesProvider), interactionType = (ModOption.InteractionType)2)]
        public static float MaxDeadRecoveriesValue = 2f;

        [ModOption(name = OptionDeadRecoverySlowPercent, category = CategoryLastStand, categoryOrder = 120, order = 40, defaultValueIndex = 6, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float DeadRecoverySlowPercent = 60f;

        [ModOption(name = OptionKnockoutDurationSeconds, category = CategoryLastStand, categoryOrder = 120, order = 50, defaultValueIndex = 3, valueSourceName = nameof(KnockoutDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float KnockoutDurationSeconds = 8f;

        [ModOption(name = OptionKnockoutRecoverySlowPercent, category = CategoryLastStand, categoryOrder = 120, order = 60, defaultValueIndex = 4, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float KnockoutRecoverySlowPercent = 40f;

        [ModOption(name = OptionSlowDebuffsStack, category = CategoryLastStand, categoryOrder = 120, order = 70, defaultValueIndex = 1)]
        public static bool SlowDebuffsStack = true;

        [ModOption(name = OptionFallFromLegInjury, category = CategoryOptional, categoryOrder = 180, order = 0, defaultValueIndex = 1)]
        public static bool FallFromLegInjury = true;

        [ModOption(name = OptionLegImmobilization, category = CategoryOptional, categoryOrder = 180, order = 10, defaultValueIndex = 1)]
        public static bool LegImmobilization = true;

        [ModOption(name = OptionArmImmobilization, category = CategoryOptional, categoryOrder = 180, order = 20, defaultValueIndex = 1)]
        public static bool ArmImmobilization = true;

        [ModOption(name = OptionLastStandEnabled, category = CategoryOptional, categoryOrder = 180, order = 30, defaultValueIndex = 1)]
        public static bool LastStandEnabled = true;

        [ModOption(name = OptionKnockoutEnabled, category = CategoryOptional, categoryOrder = 180, order = 40, defaultValueIndex = 1)]
        public static bool KnockoutEnabled = true;

        [ModOption(name = OptionEnableBasicLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 0, defaultValueIndex = 1, tooltip = "Enable high-level state logs")]
        public static bool EnableBasicLogging = true;

        [ModOption(name = OptionEnableDiagnosticsLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 2, defaultValueIndex = 0, tooltip = "Enable deeper decision-path logs")]
        public static bool EnableDiagnosticsLogging = false;

        [ModOption(name = OptionEnableVerboseLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 4, defaultValueIndex = 0, tooltip = "Enable high-volume per-hit logs")]
        public static bool EnableVerboseLogging = false;

        public static ModOptionString[] DamagePresetProvider() => new[]
        {
            new ModOptionString("Lenient", PresetDamageLenient),
            new ModOptionString("Default", PresetDamageDefault),
            new ModOptionString("Severe", PresetDamageSevere),
            new ModOptionString("Brutal", PresetDamageBrutal),
            new ModOptionString("Extreme", PresetDamageExtreme),
        };

        public static ModOptionString[] LimbSlowPresetProvider() => new[]
        {
            new ModOptionString("Low", PresetLimbSlowLow),
            new ModOptionString("Default", PresetLimbSlowDefault),
            new ModOptionString("High", PresetLimbSlowHigh),
        };

        public static ModOptionString[] LastStandPresetProvider() => new[]
        {
            new ModOptionString("Less", PresetLastStandLess),
            new ModOptionString("Default", PresetLastStandDefault),
            new ModOptionString("More", PresetLastStandMore),
        };

        public static ModOptionString[] LastStandSlowPresetProvider() => new[]
        {
            new ModOptionString("Light", PresetLastStandSlowLight),
            new ModOptionString("Default", PresetLastStandSlowDefault),
            new ModOptionString("Heavy", PresetLastStandSlowHeavy),
            new ModOptionString("Extreme", PresetLastStandSlowExtreme),
        };

        public static ModOptionFloat[] ThresholdDamageProvider() => new[]
        {
            new ModOptionFloat("6", 6f), new ModOptionFloat("8", 8f), new ModOptionFloat("10", 10f),
            new ModOptionFloat("12", 12f), new ModOptionFloat("14", 14f), new ModOptionFloat("16", 16f),
            new ModOptionFloat("18", 18f), new ModOptionFloat("20", 20f), new ModOptionFloat("22", 22f),
            new ModOptionFloat("24", 24f), new ModOptionFloat("26", 26f), new ModOptionFloat("28", 28f),
            new ModOptionFloat("30", 30f), new ModOptionFloat("35", 35f), new ModOptionFloat("40", 40f), new ModOptionFloat("50", 50f),
        };

        public static ModOptionFloat[] DisableDurationProvider() => new[]
        {
            new ModOptionFloat("2s", 2f), new ModOptionFloat("4s", 4f), new ModOptionFloat("6s", 6f),
            new ModOptionFloat("8s", 8f), new ModOptionFloat("10s", 10f), new ModOptionFloat("12s", 12f),
            new ModOptionFloat("14s", 14f), new ModOptionFloat("16s", 16f), new ModOptionFloat("20s", 20f),
            new ModOptionFloat("25s", 25f), new ModOptionFloat("30s", 30f), new ModOptionFloat("40s", 40f),
            new ModOptionFloat("60s", 60f), new ModOptionFloat("120s", 120f),
        };

        public static ModOptionFloat[] RecoveryDelayProvider() => new[]
        {
            new ModOptionFloat("0.40x", 0.40f), new ModOptionFloat("0.60x", 0.60f), new ModOptionFloat("0.80x", 0.80f),
            new ModOptionFloat("1.00x", 1.00f), new ModOptionFloat("1.20x", 1.20f), new ModOptionFloat("1.40x", 1.40f),
            new ModOptionFloat("1.80x", 1.80f), new ModOptionFloat("2.20x", 2.20f), new ModOptionFloat("3.00x", 3.00f),
        };

        public static ModOptionFloat[] FivePercentProvider()
        {
            ModOptionFloat[] values = new ModOptionFloat[21];
            for (int i = 0; i < values.Length; i++)
            {
                float value = i * 5f;
                values[i] = new ModOptionFloat(value.ToString("0") + "%", value);
            }

            return values;
        }

        public static ModOptionFloat[] TenPercentProvider()
        {
            ModOptionFloat[] values = new ModOptionFloat[11];
            for (int i = 0; i < values.Length; i++)
            {
                float value = i * 10f;
                values[i] = new ModOptionFloat(value.ToString("0") + "%", value);
            }

            return values;
        }

        public static ModOptionFloat[] MaxDeadRecoveriesProvider() => new[]
        {
            new ModOptionFloat("0", 0f), new ModOptionFloat("1", 1f), new ModOptionFloat("2", 2f),
            new ModOptionFloat("3", 3f), new ModOptionFloat("4", 4f), new ModOptionFloat("5", 5f),
        };

        public static ModOptionFloat[] KnockoutDurationProvider() => new[]
        {
            new ModOptionFloat("2s", 2f), new ModOptionFloat("4s", 4f), new ModOptionFloat("6s", 6f),
            new ModOptionFloat("8s", 8f), new ModOptionFloat("10s", 10f), new ModOptionFloat("12s", 12f),
            new ModOptionFloat("15s", 15f), new ModOptionFloat("20s", 20f),
        };

        public static bool ApplySelectedPresets()
        {
            string damagePreset = NormalizeDamagePreset(PresetDamageModel);
            string limbSlowPreset = NormalizeLimbSlowPreset(PresetLimbSlowModel);
            string lastStandPreset = NormalizeLastStandPreset(PresetLastStandModel);
            string lastStandSlowPreset = NormalizeLastStandSlowPreset(PresetLastStandSlowModel);

            ResolveDamagePreset(damagePreset, out float legsThreshold, out float armsThreshold, out float legsDuration, out float armsDuration);
            ResolveLimbSlowPreset(limbSlowPreset, out float legsSlow, out float armsSlow, out bool legsSlowStacks, out bool armsSlowStacks);

            ResolveLastStandPreset(
                lastStandPreset,
                out float recoveryDelay,
                out float recoveryReduction,
                out bool restorePins,
                out float deadChance,
                out int maxRecoveries,
                out float deadSlow,
                out float knockoutDuration,
                out float knockoutSlow,
                out bool? forceLastStandEnabled);

            ResolveLastStandSlowPreset(lastStandSlowPreset, out float slowPresetKnockout, out float slowPresetDead, out bool slowPresetStacking);

            bool changed = false;
            changed |= SetFloat(ref LegsThresholdDamage, legsThreshold, 1f, 200f);
            changed |= SetFloat(ref ArmsThresholdDamage, armsThreshold, 1f, 200f);
            changed |= SetFloat(ref LegsDisableDurationSeconds, legsDuration, 1f, 600f);
            changed |= SetFloat(ref ArmsDisableDurationSeconds, armsDuration, 1f, 600f);
            changed |= SetFloat(ref LegsSlowDebuffPercent, legsSlow, 0f, 100f);
            changed |= SetFloat(ref ArmsSlowDebuffPercent, armsSlow, 0f, 100f);
            changed |= SetBool(ref LegsSlowStacks, legsSlowStacks);
            changed |= SetBool(ref ArmsSlowStacks, armsSlowStacks);

            changed |= SetFloat(ref RecoveryDelayMultiplier, recoveryDelay, 0.1f, 10f);
            changed |= SetFloat(ref RecoveryDamageReductionPercent, recoveryReduction, 0f, 100f);
            changed |= SetBool(ref RecoveryRestoresPinForces, restorePins);
            changed |= SetFloat(ref DeadRevivalChancePercent, deadChance, 0f, 100f);
            changed |= SetFloat(ref MaxDeadRecoveriesValue, maxRecoveries, 0f, 10f);
            changed |= SetFloat(ref DeadRecoverySlowPercent, deadSlow, 0f, 100f);
            changed |= SetFloat(ref KnockoutDurationSeconds, knockoutDuration, 1f, 60f);
            changed |= SetFloat(ref KnockoutRecoverySlowPercent, knockoutSlow, 0f, 100f);

            changed |= SetFloat(ref KnockoutRecoverySlowPercent, slowPresetKnockout, 0f, 100f);
            changed |= SetFloat(ref DeadRecoverySlowPercent, slowPresetDead, 0f, 100f);
            changed |= SetBool(ref SlowDebuffsStack, slowPresetStacking);

            if (forceLastStandEnabled.HasValue)
            {
                changed |= SetBool(ref LastStandEnabled, forceLastStandEnabled.Value);
            }

            return changed;
        }

        public static float GetMinimumTrackedHitDamage() => HardcodedMinimumTrackedHitDamage;
        public static bool ShouldRefreshDisableTimerOnHit() => HardcodedHitsRefreshDisableTimer;

        public static float GetLimbThresholdDamage(RagdollPart.Type limbType)
        {
            return IsLegType(limbType)
                ? Mathf.Max(1f, LegsThresholdDamage)
                : Mathf.Max(1f, ArmsThresholdDamage);
        }

        public static float GetLimbDisableDurationSeconds(RagdollPart.Type limbType)
        {
            float baseDuration = IsLegType(limbType)
                ? Mathf.Max(1f, LegsDisableDurationSeconds)
                : Mathf.Max(1f, ArmsDisableDurationSeconds);

            if (!LastStandEnabled)
            {
                return float.PositiveInfinity;
            }

            return baseDuration * Mathf.Clamp(RecoveryDelayMultiplier, 0.1f, 10f);
        }

        public static float GetLimbSlowDebuffRatio(RagdollPart.Type limbType)
        {
            return IsLegType(limbType)
                ? Mathf.Clamp01(LegsSlowDebuffPercent / 100f)
                : Mathf.Clamp01(ArmsSlowDebuffPercent / 100f);
        }

        public static bool GetLimbSlowStacks(RagdollPart.Type limbType)
        {
            return IsLegType(limbType) ? LegsSlowStacks : ArmsSlowStacks;
        }

        public static float GetKnockoutRecoverySlowDebuffRatio()
        {
            return Mathf.Clamp01(KnockoutRecoverySlowPercent / 100f);
        }

        public static float GetDeadRecoverySlowDebuffRatio()
        {
            return Mathf.Clamp01(DeadRecoverySlowPercent / 100f);
        }

        public static float GetKnockoutRecoverySlowDurationSeconds()
        {
            float duration = 10f * Mathf.Clamp(RecoveryDelayMultiplier, 0.1f, 10f);
            return Mathf.Clamp(duration, 4f, 35f);
        }

        public static float GetDeadRecoverySlowDurationSeconds()
        {
            float duration = 20f * Mathf.Clamp(RecoveryDelayMultiplier, 0.1f, 10f);
            return Mathf.Clamp(duration, 8f, 60f);
        }

        public static bool UseSlowDebuffStacking() => SlowDebuffsStack;
        public static int GetMaxDeadRecoveries() => Mathf.Clamp(Mathf.RoundToInt(MaxDeadRecoveriesValue), 0, 10);

        public static float GetRetainedDamageRatioAfterRecovery()
        {
            float reductionRatio = Mathf.Clamp01(RecoveryDamageReductionPercent / 100f);
            return 1f - reductionRatio;
        }

        public static bool CanAttemptDeadRecovery()
        {
            return LastStandEnabled && DeadRevivalChancePercent > 0f && GetMaxDeadRecoveries() > 0;
        }

        public static float GetDeadRevivalChanceRatio()
        {
            return Mathf.Clamp01(DeadRevivalChancePercent / 100f);
        }

        public static float GetEffectiveDeadRecoveryChanceRatio(int successfulRecoveries)
        {
            float baseChance = GetDeadRevivalChanceRatio();
            int tiers = Mathf.Max(0, successfulRecoveries);
            return Mathf.Clamp01(baseChance * Mathf.Pow(0.5f, tiers));
        }

        public static float GetDeadRevivalHealth()
        {
            float reductionRatio = Mathf.Clamp01(RecoveryDamageReductionPercent / 100f);
            return Mathf.Clamp(0.25f + (reductionRatio * 0.50f), 0.20f, 0.75f);
        }

        public static float GetDeadRevivalDelaySeconds()
        {
            float delay = 4f * Mathf.Clamp(RecoveryDelayMultiplier, 0.1f, 10f);
            return Mathf.Clamp(delay, 1.5f, 15f);
        }

        public static float GetKnockoutDurationSeconds()
        {
            return Mathf.Clamp(KnockoutDurationSeconds, 1f, 60f);
        }

        public static bool IsExcludedDeadRecoveryPart(RagdollPart part)
        {
            if (part == null)
            {
                return false;
            }

            if (part.type == RagdollPart.Type.Head || part.type == RagdollPart.Type.Neck)
            {
                return true;
            }

            string token = BuildPartToken(part);
            if (token.Length == 0)
            {
                return false;
            }

            return token.IndexOf("HEAD", StringComparison.Ordinal) >= 0 ||
                   token.IndexOf("NECK", StringComparison.Ordinal) >= 0 ||
                   token.IndexOf("THROAT", StringComparison.Ordinal) >= 0;
        }

        public static int GetPresetSelectionHash()
        {
            int hash = 17;
            hash = CombineHash(hash, StringHash(NormalizeDamagePreset(PresetDamageModel)));
            hash = CombineHash(hash, StringHash(NormalizeLimbSlowPreset(PresetLimbSlowModel)));
            hash = CombineHash(hash, StringHash(NormalizeLastStandPreset(PresetLastStandModel)));
            hash = CombineHash(hash, StringHash(NormalizeLastStandSlowPreset(PresetLastStandSlowModel)));
            return hash;
        }

        public static int GetSourceOfTruthHash()
        {
            SourceSnapshot snapshot = CaptureSourceSnapshot();
            int hash = 17;

            hash = CombineHash(hash, PercentHash(snapshot.LegsThresholdDamage));
            hash = CombineHash(hash, PercentHash(snapshot.LegsDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.LegsSlowDebuffPercent));
            hash = CombineHash(hash, snapshot.LegsSlowStacks ? 1 : 0);

            hash = CombineHash(hash, PercentHash(snapshot.ArmsThresholdDamage));
            hash = CombineHash(hash, PercentHash(snapshot.ArmsDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.ArmsSlowDebuffPercent));
            hash = CombineHash(hash, snapshot.ArmsSlowStacks ? 1 : 0);

            hash = CombineHash(hash, snapshot.LastStandEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.FallFromLegInjury ? 1 : 0);
            hash = CombineHash(hash, snapshot.LegImmobilization ? 1 : 0);
            hash = CombineHash(hash, snapshot.ArmImmobilization ? 1 : 0);
            hash = CombineHash(hash, snapshot.KnockoutEnabled ? 1 : 0);

            hash = CombineHash(hash, PercentHash(snapshot.RecoveryDelayMultiplier));
            hash = CombineHash(hash, PercentHash(snapshot.RecoveryDamageReductionPercent));
            hash = CombineHash(hash, snapshot.RecoveryRestoresPinForces ? 1 : 0);
            hash = CombineHash(hash, PercentHash(snapshot.DeadRevivalChancePercent));
            hash = CombineHash(hash, snapshot.MaxDeadRecoveries);
            hash = CombineHash(hash, PercentHash(snapshot.DeadRecoverySlowPercent));
            hash = CombineHash(hash, PercentHash(snapshot.KnockoutDurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.KnockoutRecoverySlowPercent));
            hash = CombineHash(hash, snapshot.SlowDebuffsStack ? 1 : 0);

            return hash;
        }

        public static SourceSnapshot CaptureSourceSnapshot()
        {
            return new SourceSnapshot
            {
                LegsThresholdDamage = LegsThresholdDamage,
                LegsDisableDurationSeconds = LegsDisableDurationSeconds,
                LegsSlowDebuffPercent = LegsSlowDebuffPercent,
                LegsSlowStacks = LegsSlowStacks,
                ArmsThresholdDamage = ArmsThresholdDamage,
                ArmsDisableDurationSeconds = ArmsDisableDurationSeconds,
                ArmsSlowDebuffPercent = ArmsSlowDebuffPercent,
                ArmsSlowStacks = ArmsSlowStacks,
                LastStandEnabled = LastStandEnabled,
                FallFromLegInjury = FallFromLegInjury,
                LegImmobilization = LegImmobilization,
                ArmImmobilization = ArmImmobilization,
                KnockoutEnabled = KnockoutEnabled,
                RecoveryDelayMultiplier = RecoveryDelayMultiplier,
                RecoveryDamageReductionPercent = RecoveryDamageReductionPercent,
                RecoveryRestoresPinForces = RecoveryRestoresPinForces,
                DeadRevivalChancePercent = DeadRevivalChancePercent,
                MaxDeadRecoveries = GetMaxDeadRecoveries(),
                DeadRecoverySlowPercent = DeadRecoverySlowPercent,
                KnockoutDurationSeconds = KnockoutDurationSeconds,
                KnockoutRecoverySlowPercent = KnockoutRecoverySlowPercent,
                SlowDebuffsStack = SlowDebuffsStack,
            };
        }

        public static bool SourceSnapshotsEqual(SourceSnapshot left, SourceSnapshot right)
        {
            return Mathf.Abs(left.LegsThresholdDamage - right.LegsThresholdDamage) < 0.0001f &&
                   Mathf.Abs(left.LegsDisableDurationSeconds - right.LegsDisableDurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.LegsSlowDebuffPercent - right.LegsSlowDebuffPercent) < 0.0001f &&
                   left.LegsSlowStacks == right.LegsSlowStacks &&
                   Mathf.Abs(left.ArmsThresholdDamage - right.ArmsThresholdDamage) < 0.0001f &&
                   Mathf.Abs(left.ArmsDisableDurationSeconds - right.ArmsDisableDurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.ArmsSlowDebuffPercent - right.ArmsSlowDebuffPercent) < 0.0001f &&
                   left.ArmsSlowStacks == right.ArmsSlowStacks &&
                   left.LastStandEnabled == right.LastStandEnabled &&
                   left.FallFromLegInjury == right.FallFromLegInjury &&
                   left.LegImmobilization == right.LegImmobilization &&
                   left.ArmImmobilization == right.ArmImmobilization &&
                   left.KnockoutEnabled == right.KnockoutEnabled &&
                   Mathf.Abs(left.RecoveryDelayMultiplier - right.RecoveryDelayMultiplier) < 0.0001f &&
                   Mathf.Abs(left.RecoveryDamageReductionPercent - right.RecoveryDamageReductionPercent) < 0.0001f &&
                   left.RecoveryRestoresPinForces == right.RecoveryRestoresPinForces &&
                   Mathf.Abs(left.DeadRevivalChancePercent - right.DeadRevivalChancePercent) < 0.0001f &&
                   left.MaxDeadRecoveries == right.MaxDeadRecoveries &&
                   Mathf.Abs(left.DeadRecoverySlowPercent - right.DeadRecoverySlowPercent) < 0.0001f &&
                   Mathf.Abs(left.KnockoutDurationSeconds - right.KnockoutDurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.KnockoutRecoverySlowPercent - right.KnockoutRecoverySlowPercent) < 0.0001f &&
                   left.SlowDebuffsStack == right.SlowDebuffsStack;
        }

        public static string GetSourceOfTruthSummary()
        {
            return "legs=" + LegsThresholdDamage.ToString("0") + "/" + LegsDisableDurationSeconds.ToString("0") + "s slow=" + LegsSlowDebuffPercent.ToString("0") + "% stack=" + LegsSlowStacks +
                   " arms=" + ArmsThresholdDamage.ToString("0") + "/" + ArmsDisableDurationSeconds.ToString("0") + "s slow=" + ArmsSlowDebuffPercent.ToString("0") + "% stack=" + ArmsSlowStacks +
                   " lastStand=" + LastStandEnabled +
                   " delay=" + RecoveryDelayMultiplier.ToString("0.00") + "x" +
                   " recover=" + RecoveryDamageReductionPercent.ToString("0") + "%" +
                   " deadChance=" + DeadRevivalChancePercent.ToString("0") + "%" +
                   " maxRecoveries=" + GetMaxDeadRecoveries() +
                   " deadSlow=" + DeadRecoverySlowPercent.ToString("0") + "%" +
                   " koDur=" + KnockoutDurationSeconds.ToString("0") + "s" +
                   " koSlow=" + KnockoutRecoverySlowPercent.ToString("0") + "%" +
                   " slowStack=" + SlowDebuffsStack;
        }

        public static string NormalizeDamagePreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("LENIENT") || token.Contains("LIGHT")) return PresetDamageLenient;
            if (token.Contains("SEVERE") || token.Contains("TACTIC")) return PresetDamageSevere;
            if (token.Contains("BRUTAL") || token.Contains("HARD")) return PresetDamageBrutal;
            if (token.Contains("EXTREME") || token.Contains("SEVER")) return PresetDamageExtreme;
            return PresetDamageDefault;
        }

        public static string NormalizeLimbSlowPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("LOW") || token.Contains("LIGHT")) return PresetLimbSlowLow;
            if (token.Contains("HIGH") || token.Contains("HEAVY")) return PresetLimbSlowHigh;
            return PresetLimbSlowDefault;
        }

        public static string NormalizeLastStandPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("OFF") || token.Contains("DISABLE")) return PresetLastStandOffLegacy;
            if (token.Contains("LESS") || token.Contains("LIGHT")) return PresetLastStandLess;
            if (token.Contains("MORE") || token.Contains("HIGH") || token.Contains("AGGRESSIVE")) return PresetLastStandMore;
            return PresetLastStandDefault;
        }

        public static string NormalizeLastStandSlowPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("LIGHT") || token.Contains("LOW")) return PresetLastStandSlowLight;
            if (token.Contains("HEAVY") || token.Contains("HIGH")) return PresetLastStandSlowHeavy;
            if (token.Contains("EXTREME") || token.Contains("MAX")) return PresetLastStandSlowExtreme;
            return PresetLastStandSlowDefault;
        }

        private static void ResolveDamagePreset(string preset, out float legsThreshold, out float armsThreshold, out float legsDuration, out float armsDuration)
        {
            legsThreshold = 22f;
            armsThreshold = 26f;
            legsDuration = 14f;
            armsDuration = 10f;

            switch (preset)
            {
                case PresetDamageLenient:
                    legsThreshold = 30f;
                    armsThreshold = 35f;
                    legsDuration = 8f;
                    armsDuration = 6f;
                    break;
                case PresetDamageSevere:
                    legsThreshold = 18f;
                    armsThreshold = 22f;
                    legsDuration = 18f;
                    armsDuration = 14f;
                    break;
                case PresetDamageBrutal:
                    legsThreshold = 14f;
                    armsThreshold = 16f;
                    legsDuration = 30f;
                    armsDuration = 24f;
                    break;
                case PresetDamageExtreme:
                    legsThreshold = 10f;
                    armsThreshold = 12f;
                    legsDuration = 45f;
                    armsDuration = 36f;
                    break;
            }
        }

        private static void ResolveLimbSlowPreset(string preset, out float legsSlow, out float armsSlow, out bool legsSlowStacks, out bool armsSlowStacks)
        {
            switch (preset)
            {
                case PresetLimbSlowLow:
                    legsSlow = 5f;
                    armsSlow = 3f;
                    legsSlowStacks = false;
                    armsSlowStacks = false;
                    break;
                case PresetLimbSlowHigh:
                    legsSlow = 20f;
                    armsSlow = 10f;
                    legsSlowStacks = true;
                    armsSlowStacks = true;
                    break;
                default:
                    legsSlow = 10f;
                    armsSlow = 5f;
                    legsSlowStacks = true;
                    armsSlowStacks = true;
                    break;
            }
        }

        private static void ResolveLastStandPreset(
            string preset,
            out float recoveryDelay,
            out float recoveryReduction,
            out bool restorePins,
            out float deadChance,
            out int maxRecoveries,
            out float deadSlow,
            out float knockoutDuration,
            out float knockoutSlow,
            out bool? forceLastStandEnabled)
        {
            recoveryDelay = 1.00f;
            recoveryReduction = 40f;
            restorePins = true;
            deadChance = 10f;
            maxRecoveries = 2;
            deadSlow = 60f;
            knockoutDuration = 8f;
            knockoutSlow = 40f;
            forceLastStandEnabled = true;

            switch (preset)
            {
                case PresetLastStandOffLegacy:
                    forceLastStandEnabled = false;
                    deadChance = 0f;
                    maxRecoveries = 0;
                    break;
                case PresetLastStandLess:
                    recoveryDelay = 1.30f;
                    recoveryReduction = 30f;
                    deadChance = 10f;
                    maxRecoveries = 1;
                    deadSlow = 55f;
                    knockoutDuration = 6f;
                    knockoutSlow = 30f;
                    break;
                case PresetLastStandMore:
                    recoveryDelay = 0.75f;
                    recoveryReduction = 50f;
                    deadChance = 30f;
                    maxRecoveries = 4;
                    deadSlow = 65f;
                    knockoutDuration = 12f;
                    knockoutSlow = 50f;
                    break;
            }
        }

        private static void ResolveLastStandSlowPreset(string preset, out float knockoutSlow, out float deadSlow, out bool stackDebuffs)
        {
            switch (preset)
            {
                case PresetLastStandSlowLight:
                    knockoutSlow = 20f;
                    deadSlow = 35f;
                    stackDebuffs = false;
                    break;
                case PresetLastStandSlowHeavy:
                    knockoutSlow = 55f;
                    deadSlow = 70f;
                    stackDebuffs = true;
                    break;
                case PresetLastStandSlowExtreme:
                    knockoutSlow = 70f;
                    deadSlow = 85f;
                    stackDebuffs = true;
                    break;
                default:
                    knockoutSlow = 40f;
                    deadSlow = 60f;
                    stackDebuffs = true;
                    break;
            }
        }

        public static float GetDeadRevivalSpeedMultiplier()
        {
            return Mathf.Clamp(1f - GetDeadRecoverySlowDebuffRatio(), 0f, 1f);
        }

        private static bool IsLegType(RagdollPart.Type limbType)
        {
            switch (limbType)
            {
                case RagdollPart.Type.LeftLeg:
                case RagdollPart.Type.LeftFoot:
                case RagdollPart.Type.RightLeg:
                case RagdollPart.Type.RightFoot:
                case RagdollPart.Type.Tail:
                    return true;
                default:
                    return false;
            }
        }

        private static string BuildPartToken(RagdollPart part)
        {
            if (part == null)
            {
                return string.Empty;
            }

            string token = string.Empty;
            if (!string.IsNullOrWhiteSpace(part.name))
            {
                token = part.name;
            }

            if (part.transform != null && !string.IsNullOrWhiteSpace(part.transform.name))
            {
                token += "|" + part.transform.name;
            }

            if (part.bone != null && part.bone.animation != null && !string.IsNullOrWhiteSpace(part.bone.animation.name))
            {
                token += "|" + part.bone.animation.name;
            }

            return token.ToUpperInvariant();
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
