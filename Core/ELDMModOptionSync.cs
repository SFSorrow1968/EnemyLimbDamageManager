using System;
using System.Collections.Generic;
using System.Reflection;
using EnemyLimbDamageManager.Configuration;
using ThunderRoad;
using UnityEngine;

namespace EnemyLimbDamageManager.Core
{
    internal sealed class ELDMModOptionSync
    {
        private const string OptionKeySeparator = "||";
        private const float UpdateIntervalSeconds = 0.15f;
        private const float StartupOverwriteWatchSeconds = 4f;

        public static ELDMModOptionSync Instance { get; } = new ELDMModOptionSync();

        private readonly Dictionary<string, ModOption> modOptionsByKey = new Dictionary<string, ModOption>(StringComparer.Ordinal);

        private ModManager.ModData modData;
        private bool initialized;
        private float nextUpdateTime;
        private int lastPresetHash;
        private int startupSourceOfTruthHash;
        private bool startupOverwriteWatchActive;
        private float startupOverwriteWatchEndTime;

        private bool applyingPresetBatch;
        private bool snapshotInitialized;
        private ELDMModOptions.SourceSnapshot lastSnapshot;
        private bool manualSourceOverrideActive;

        private ELDMModOptionSync()
        {
        }

        public void Initialize()
        {
            initialized = false;
            nextUpdateTime = 0f;
            lastPresetHash = int.MinValue;
            startupSourceOfTruthHash = int.MinValue;
            startupOverwriteWatchActive = false;
            startupOverwriteWatchEndTime = 0f;
            applyingPresetBatch = false;
            snapshotInitialized = false;
            lastSnapshot = default(ELDMModOptions.SourceSnapshot);
            manualSourceOverrideActive = false;
            modData = null;
            modOptionsByKey.Clear();

            TryInitialize();
            if (!initialized)
            {
                return;
            }

            lastSnapshot = ELDMModOptions.CaptureSourceSnapshot();
            snapshotInitialized = true;

            bool changed = ApplyPresetsIfChanged(force: true, source: "preset");
            if (changed)
            {
                ModManager.RefreshModOptionsUI();
            }

            startupSourceOfTruthHash = ELDMModOptions.GetSourceOfTruthHash();
            startupOverwriteWatchActive = true;
            startupOverwriteWatchEndTime = Time.unscaledTime + StartupOverwriteWatchSeconds;
        }

        public void Shutdown()
        {
            initialized = false;
            modData = null;
            modOptionsByKey.Clear();
            nextUpdateTime = 0f;
            lastPresetHash = int.MinValue;
            startupSourceOfTruthHash = int.MinValue;
            startupOverwriteWatchActive = false;
            startupOverwriteWatchEndTime = 0f;
            applyingPresetBatch = false;
            snapshotInitialized = false;
            lastSnapshot = default(ELDMModOptions.SourceSnapshot);
            manualSourceOverrideActive = false;
        }

        public void Update()
        {
            if (!initialized)
            {
                TryInitialize();
                if (!initialized)
                {
                    return;
                }

                lastSnapshot = ELDMModOptions.CaptureSourceSnapshot();
                snapshotInitialized = true;

                bool initChanged = ApplyPresetsIfChanged(force: true, source: "preset");
                if (initChanged)
                {
                    ModManager.RefreshModOptionsUI();
                }
                return;
            }

            float now = Time.unscaledTime;
            if (now < nextUpdateTime)
            {
                return;
            }

            nextUpdateTime = now + UpdateIntervalSeconds;

            DetectManualSourceEdits(now);

            bool changed = false;
            changed |= ReapplyPresetsIfStartupOverwriteDetected(now);
            changed |= ApplyPresetsIfChanged(force: false, source: "preset");

            if (changed)
            {
                ModManager.RefreshModOptionsUI();
            }
        }

        private void DetectManualSourceEdits(float now)
        {
            if (!snapshotInitialized || applyingPresetBatch)
            {
                return;
            }

            ELDMModOptions.SourceSnapshot current = ELDMModOptions.CaptureSourceSnapshot();
            if (ELDMModOptions.SourceSnapshotsEqual(lastSnapshot, current))
            {
                return;
            }

            bool withinStartupWatch = startupOverwriteWatchActive && now <= startupOverwriteWatchEndTime;
            if (!withinStartupWatch)
            {
                manualSourceOverrideActive = true;
                LogSnapshotDiff(lastSnapshot, current, "user");
            }

            lastSnapshot = current;
        }
        private bool ReapplyPresetsIfStartupOverwriteDetected(float now)
        {
            if (!startupOverwriteWatchActive)
            {
                return false;
            }

            if (now > startupOverwriteWatchEndTime)
            {
                startupOverwriteWatchActive = false;
                return false;
            }

            int currentHash = ELDMModOptions.GetSourceOfTruthHash();
            if (currentHash == startupSourceOfTruthHash)
            {
                return false;
            }

            bool changed = ApplyPresetsIfChanged(force: true, source: "startup_reapply");
            startupSourceOfTruthHash = ELDMModOptions.GetSourceOfTruthHash();
            startupOverwriteWatchActive = false;

            ELDMLog.Info("Detected post-load mod-option overwrite; re-applied selected presets once.");
            return changed;
        }

        private void TryInitialize()
        {
            if (initialized)
            {
                return;
            }

            if (!ModManager.TryGetModData(Assembly.GetExecutingAssembly(), out modData))
            {
                return;
            }

            if (modData?.modOptions == null || modData.modOptions.Count == 0)
            {
                return;
            }

            RefreshOptionCache();
            initialized = true;
        }

        private bool ApplyPresetsIfChanged(bool force, string source)
        {
            int presetHash = ELDMModOptions.GetPresetSelectionHash();
            if (!force)
            {
                if (manualSourceOverrideActive && presetHash == lastPresetHash)
                {
                    return false;
                }

                if (presetHash == lastPresetHash)
                {
                    return false;
                }
            }

            if (!force && manualSourceOverrideActive && presetHash != lastPresetHash)
            {
                manualSourceOverrideActive = false;
                ELDMLog.Info("manual_override_cleared reason=preset_changed");
            }

            string damagePreset = ELDMModOptions.NormalizeDamagePreset(ELDMModOptions.PresetDamageModel);
            string limbSlowPreset = ELDMModOptions.NormalizeLimbSlowPreset(ELDMModOptions.PresetLimbSlowModel);
            string lastStandPreset = ELDMModOptions.NormalizeLastStandPreset(ELDMModOptions.PresetLastStandModel);
            string lastStandSlowPreset = ELDMModOptions.NormalizeLastStandSlowPreset(ELDMModOptions.PresetLastStandSlowModel);

            ELDMModOptions.SourceSnapshot before = ELDMModOptions.CaptureSourceSnapshot();

            applyingPresetBatch = true;
            bool valuesChanged;
            bool uiChanged;
            try
            {
                valuesChanged = ELDMModOptions.ApplySelectedPresets();
                uiChanged = SyncSourceOfTruthOptions();
            }
            finally
            {
                applyingPresetBatch = false;
            }

            ELDMModOptions.SourceSnapshot after = ELDMModOptions.CaptureSourceSnapshot();
            if (!ELDMModOptions.SourceSnapshotsEqual(before, after))
            {
                LogSnapshotDiff(before, after, source);
            }

            lastSnapshot = after;
            snapshotInitialized = true;
            lastPresetHash = ELDMModOptions.GetPresetSelectionHash();

            if (force || valuesChanged || uiChanged)
            {
                ELDMLog.Info(
                    "Preset batch wrote source-of-truth collapsibles: damage=" + damagePreset +
                    " limbSlow=" + limbSlowPreset +
                    " lastStand=" + lastStandPreset +
                    " lastStandSlow=" + lastStandSlowPreset +
                    " valuesChanged=" + valuesChanged +
                    " uiSynced=" + uiChanged +
                    " snapshot={" + ELDMModOptions.GetSourceOfTruthSummary() + "}",
                    verboseOnly: !valuesChanged && !uiChanged);
            }

            return valuesChanged || uiChanged;
        }

        private bool SyncSourceOfTruthOptions()
        {
            bool changed = false;

            changed |= SyncFloatOption(ELDMModOptions.CategoryLegs, ELDMModOptions.OptionLegThreshold, ELDMModOptions.LegsThresholdDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLegs, ELDMModOptions.OptionLegDisableDuration, ELDMModOptions.LegsDisableDurationSeconds);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLegs, ELDMModOptions.OptionLegSlowDebuffPercent, ELDMModOptions.LegsSlowDebuffPercent);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLegs, ELDMModOptions.OptionLegSlowStacks, ELDMModOptions.LegsSlowStacks);

            changed |= SyncFloatOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmThreshold, ELDMModOptions.ArmsThresholdDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmDisableDuration, ELDMModOptions.ArmsDisableDurationSeconds);
            changed |= SyncFloatOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmSlowDebuffPercent, ELDMModOptions.ArmsSlowDebuffPercent);
            changed |= SyncBoolOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmSlowStacks, ELDMModOptions.ArmsSlowStacks);

            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionRecoveryDelayMultiplier, ELDMModOptions.RecoveryDelayMultiplier);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionRecoveryDamageReductionPercent, ELDMModOptions.RecoveryDamageReductionPercent);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionRecoveryRestoresPinForces, ELDMModOptions.RecoveryRestoresPinForces);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionDeadRevivalChancePercent, ELDMModOptions.DeadRevivalChancePercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionMaxDeadRecoveries, ELDMModOptions.MaxDeadRecoveriesValue);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionDeadRecoverySlowPercent, ELDMModOptions.DeadRecoverySlowPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionKnockoutDurationSeconds, ELDMModOptions.KnockoutDurationSeconds);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionKnockoutRecoverySlowPercent, ELDMModOptions.KnockoutRecoverySlowPercent);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionSlowDebuffsStack, ELDMModOptions.SlowDebuffsStack);

            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionFallFromLegInjury, ELDMModOptions.FallFromLegInjury);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionLegImmobilization, ELDMModOptions.LegImmobilization);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionArmImmobilization, ELDMModOptions.ArmImmobilization);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionLastStandEnabled, ELDMModOptions.LastStandEnabled);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionKnockoutEnabled, ELDMModOptions.KnockoutEnabled);

            return changed;
        }
        private bool SyncBoolOption(string category, string optionName, bool value)
        {
            if (!TryGetOption(category, optionName, out ModOption option))
            {
                return false;
            }

            if (option.parameterValues == null || option.parameterValues.Length == 0)
            {
                option.LoadModOptionParameters();
            }

            int index = FindBoolIndex(option.parameterValues, value);
            if (index < 0 || option.currentValueIndex == index)
            {
                return false;
            }

            option.Apply(index);
            option.RefreshUI();
            return true;
        }

        private bool SyncFloatOption(string category, string optionName, float value)
        {
            if (!TryGetOption(category, optionName, out ModOption option))
            {
                return false;
            }

            if (option.parameterValues == null || option.parameterValues.Length == 0)
            {
                option.LoadModOptionParameters();
            }

            int index = FindFloatIndex(option.parameterValues, value);
            if (index < 0 || option.currentValueIndex == index)
            {
                return false;
            }

            option.Apply(index);
            option.RefreshUI();
            return true;
        }

        private bool TryGetOption(string category, string optionName, out ModOption option)
        {
            option = null;
            if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(optionName))
            {
                return false;
            }

            string key = MakeKey(category, optionName);
            if (modOptionsByKey.TryGetValue(key, out option))
            {
                return true;
            }

            RefreshOptionCache();
            return modOptionsByKey.TryGetValue(key, out option);
        }

        private void RefreshOptionCache()
        {
            modOptionsByKey.Clear();
            if (modData?.modOptions == null)
            {
                return;
            }

            foreach (ModOption option in modData.modOptions)
            {
                if (option == null || string.IsNullOrWhiteSpace(option.name))
                {
                    continue;
                }

                modOptionsByKey[MakeKey(option.category, option.name)] = option;
            }
        }

        private static string MakeKey(string category, string name)
        {
            return (category ?? string.Empty) + OptionKeySeparator + (name ?? string.Empty);
        }

        private static int FindBoolIndex(ModOptionParameter[] parameters, bool value)
        {
            if (parameters == null)
            {
                return -1;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                object parameterValue = parameters[i]?.value;
                if (parameterValue is bool b && b == value)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindFloatIndex(ModOptionParameter[] parameters, float value)
        {
            if (parameters == null)
            {
                return -1;
            }

            int bestIndex = -1;
            float bestDelta = float.MaxValue;

            for (int i = 0; i < parameters.Length; i++)
            {
                object parameterValue = parameters[i]?.value;
                if (parameterValue is float f)
                {
                    float delta = Mathf.Abs(f - value);
                    if (delta < 0.0001f)
                    {
                        return i;
                    }

                    if (delta < bestDelta)
                    {
                        bestDelta = delta;
                        bestIndex = i;
                    }
                }
            }

            return bestIndex;
        }

        private static void LogSnapshotDiff(ELDMModOptions.SourceSnapshot before, ELDMModOptions.SourceSnapshot after, string source)
        {
            LogFloatDiff(source, "legs.threshold", before.LegsThresholdDamage, after.LegsThresholdDamage);
            LogFloatDiff(source, "legs.duration", before.LegsDisableDurationSeconds, after.LegsDisableDurationSeconds);
            LogFloatDiff(source, "legs.slowDebuff", before.LegsSlowDebuffPercent, after.LegsSlowDebuffPercent);
            LogBoolDiff(source, "legs.slowStacks", before.LegsSlowStacks, after.LegsSlowStacks);
            LogFloatDiff(source, "arms.threshold", before.ArmsThresholdDamage, after.ArmsThresholdDamage);
            LogFloatDiff(source, "arms.duration", before.ArmsDisableDurationSeconds, after.ArmsDisableDurationSeconds);
            LogFloatDiff(source, "arms.slowDebuff", before.ArmsSlowDebuffPercent, after.ArmsSlowDebuffPercent);
            LogBoolDiff(source, "arms.slowStacks", before.ArmsSlowStacks, after.ArmsSlowStacks);
            LogBoolDiff(source, "optional.lastStand", before.LastStandEnabled, after.LastStandEnabled);
            LogBoolDiff(source, "optional.fallFromLegInjury", before.FallFromLegInjury, after.FallFromLegInjury);
            LogBoolDiff(source, "optional.legImmobilization", before.LegImmobilization, after.LegImmobilization);
            LogBoolDiff(source, "optional.armImmobilization", before.ArmImmobilization, after.ArmImmobilization);
            LogFloatDiff(source, "lastStand.delay", before.RecoveryDelayMultiplier, after.RecoveryDelayMultiplier);
            LogFloatDiff(source, "lastStand.recoveryPercent", before.RecoveryDamageReductionPercent, after.RecoveryDamageReductionPercent);
            LogBoolDiff(source, "lastStand.restorePins", before.RecoveryRestoresPinForces, after.RecoveryRestoresPinForces);
            LogFloatDiff(source, "lastStand.deadChance", before.DeadRevivalChancePercent, after.DeadRevivalChancePercent);
            LogFloatDiff(source, "lastStand.maxDeadRecoveries", before.MaxDeadRecoveries, after.MaxDeadRecoveries);
            LogFloatDiff(source, "lastStand.deadRecoverySlow", before.DeadRecoverySlowPercent, after.DeadRecoverySlowPercent);
            LogFloatDiff(source, "lastStand.knockoutDuration", before.KnockoutDurationSeconds, after.KnockoutDurationSeconds);
            LogFloatDiff(source, "lastStand.knockoutRecoverySlow", before.KnockoutRecoverySlowPercent, after.KnockoutRecoverySlowPercent);
            LogBoolDiff(source, "lastStand.slowDebuffsStack", before.SlowDebuffsStack, after.SlowDebuffsStack);
            LogBoolDiff(source, "optional.knockout", before.KnockoutEnabled, after.KnockoutEnabled);
        }

        private static void LogFloatDiff(string source, string optionKey, float before, float after)
        {
            if (Mathf.Abs(before - after) < 0.0001f)
            {
                return;
            }

            ELDMLog.Info(
                "option_changed source=" + source +
                " option=" + optionKey +
                " old=" + before.ToString("0.###") +
                " new=" + after.ToString("0.###"));
        }

        private static void LogBoolDiff(string source, string optionKey, bool before, bool after)
        {
            if (before == after)
            {
                return;
            }

            ELDMLog.Info(
                "option_changed source=" + source +
                " option=" + optionKey +
                " old=" + before +
                " new=" + after);
        }
    }
}
