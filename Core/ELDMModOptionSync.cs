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
        private const float StartupOverwriteWatchSeconds = 8f;

        public static ELDMModOptionSync Instance { get; } = new ELDMModOptionSync();

        private readonly Dictionary<string, ModOption> modOptionsByKey = new Dictionary<string, ModOption>(StringComparer.Ordinal);

        private ModManager.ModData modData;
        private bool initialized;
        private float nextUpdateTime;
        private int lastPresetHash;
        private int startupSourceOfTruthHash;
        private bool startupOverwriteWatchActive;
        private float startupOverwriteWatchEndTime;

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
            modData = null;
            modOptionsByKey.Clear();

            TryInitialize();
            if (!initialized)
            {
                return;
            }

            bool changed = ApplyPresetsIfChanged(force: true);
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

                bool initChanged = ApplyPresetsIfChanged(force: true);
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
            bool changed = false;
            changed |= ReapplyPresetsIfStartupOverwriteDetected(now);
            changed |= ApplyPresetsIfChanged(force: false);

            if (changed)
            {
                ModManager.RefreshModOptionsUI();
            }
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

            bool changed = ApplyPresetsIfChanged(force: true);
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

        private bool ApplyPresetsIfChanged(bool force)
        {
            int presetHash = ELDMModOptions.GetPresetSelectionHash();
            if (!force && presetHash == lastPresetHash)
            {
                return false;
            }

            string damagePreset = ELDMModOptions.NormalizeDamagePreset(ELDMModOptions.PresetDamageModel);
            string recoveryPreset = ELDMModOptions.NormalizeRecoveryPreset(ELDMModOptions.PresetRecoveryModel);
            string responsePreset = ELDMModOptions.NormalizeResponsePreset(ELDMModOptions.PresetResponseModel);

            bool valuesChanged = ELDMModOptions.ApplySelectedPresets();
            bool uiChanged = SyncSourceOfTruthOptions();
            lastPresetHash = ELDMModOptions.GetPresetSelectionHash();

            if (force || valuesChanged || uiChanged)
            {
                ELDMLog.Info(
                    "Preset batch wrote source-of-truth collapsibles: damage=" + damagePreset +
                    " recovery=" + recoveryPreset +
                    " response=" + responsePreset +
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

            changed |= SyncFloatOption(ELDMModOptions.CategoryGlobal, ELDMModOptions.OptionGlobalDamageScale, ELDMModOptions.GlobalDamageScale);
            changed |= SyncFloatOption(ELDMModOptions.CategoryGlobal, ELDMModOptions.OptionMinimumTrackedHitDamage, ELDMModOptions.MinimumTrackedHitDamage);
            changed |= SyncBoolOption(ELDMModOptions.CategoryGlobal, ELDMModOptions.OptionHitsRefreshDisableTimer, ELDMModOptions.HitsRefreshDisableTimer);
            changed |= SyncFloatOption(ELDMModOptions.CategoryGlobal, ELDMModOptions.OptionLegMoveMultiplierWhileDisabled, ELDMModOptions.LegMoveMultiplierWhileDisabled);
            changed |= SyncBoolOption(ELDMModOptions.CategoryGlobal, ELDMModOptions.OptionLegDisableForcesFall, ELDMModOptions.LegDisableForcesFall);
            changed |= SyncBoolOption(ELDMModOptions.CategoryGlobal, ELDMModOptions.OptionDisableLegPinForces, ELDMModOptions.DisableLegPinForces);
            changed |= SyncBoolOption(ELDMModOptions.CategoryGlobal, ELDMModOptions.OptionDisableArmPinForces, ELDMModOptions.DisableArmPinForces);

            changed |= SyncFloatOption(ELDMModOptions.CategoryLeftLeg, ELDMModOptions.OptionLeftLegThreshold, ELDMModOptions.LeftLegThresholdDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLeftLeg, ELDMModOptions.OptionLeftLegDisableDuration, ELDMModOptions.LeftLegDisableDurationSeconds);

            changed |= SyncFloatOption(ELDMModOptions.CategoryRightLeg, ELDMModOptions.OptionRightLegThreshold, ELDMModOptions.RightLegThresholdDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryRightLeg, ELDMModOptions.OptionRightLegDisableDuration, ELDMModOptions.RightLegDisableDurationSeconds);

            changed |= SyncFloatOption(ELDMModOptions.CategoryLeftArm, ELDMModOptions.OptionLeftArmThreshold, ELDMModOptions.LeftArmThresholdDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLeftArm, ELDMModOptions.OptionLeftArmDisableDuration, ELDMModOptions.LeftArmDisableDurationSeconds);

            changed |= SyncFloatOption(ELDMModOptions.CategoryRightArm, ELDMModOptions.OptionRightArmThreshold, ELDMModOptions.RightArmThresholdDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryRightArm, ELDMModOptions.OptionRightArmDisableDuration, ELDMModOptions.RightArmDisableDurationSeconds);

            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionLastStandEnabled, ELDMModOptions.LastStandEnabled);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionRecoveryDelayMultiplier, ELDMModOptions.RecoveryDelayMultiplier);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionRecoveryClearsAccumulatedDamage, ELDMModOptions.RecoveryClearsAccumulatedDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionRecoveryDamageRetainedPercent, ELDMModOptions.RecoveryDamageRetainedPercent);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStand, ELDMModOptions.OptionRecoveryRestoresPinForces, ELDMModOptions.RecoveryRestoresPinForces);

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

            for (int i = 0; i < parameters.Length; i++)
            {
                object parameterValue = parameters[i]?.value;
                if (parameterValue is float f && Mathf.Abs(f - value) < 0.0001f)
                {
                    return i;
                }
                if (parameterValue is double d && Mathf.Abs((float)d - value) < 0.0001f)
                {
                    return i;
                }
                if (parameterValue is int n && Mathf.Abs(n - value) < 0.0001f)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
