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

        public static ELDMModOptionSync Instance { get; } = new ELDMModOptionSync();

        private readonly Dictionary<string, ModOption> modOptionsByKey = new Dictionary<string, ModOption>(StringComparer.Ordinal);
        private readonly HashSet<string> missingOptionKeysLogged = new HashSet<string>(StringComparer.Ordinal);

        private ModManager.ModData modData;
        private bool initialized;
        private float nextUpdateTime;
        private int lastPresetHash;

        private ELDMModOptions.SourceSnapshot lastSnapshot;

        private ELDMModOptionSync()
        {
        }

        public void Initialize()
        {
            initialized = false;
            nextUpdateTime = 0f;
            lastPresetHash = int.MinValue;
            lastSnapshot = default(ELDMModOptions.SourceSnapshot);
            modData = null;
            modOptionsByKey.Clear();
            missingOptionKeysLogged.Clear();

            TryInitialize();
            if (!initialized)
            {
                return;
            }

            lastSnapshot = ELDMModOptions.CaptureSourceSnapshot();
            lastPresetHash = ELDMModOptions.GetPresetSelectionHash();
        }

        public void Shutdown()
        {
            initialized = false;
            modData = null;
            modOptionsByKey.Clear();
            missingOptionKeysLogged.Clear();
            nextUpdateTime = 0f;
            lastPresetHash = int.MinValue;
            lastSnapshot = default(ELDMModOptions.SourceSnapshot);
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
                lastPresetHash = ELDMModOptions.GetPresetSelectionHash();
                return;
            }

            float now = Time.unscaledTime;
            if (now < nextUpdateTime)
            {
                return;
            }

            nextUpdateTime = now + UpdateIntervalSeconds;

            bool changed = ApplyPresetsIfChanged(force: false, source: "preset");

            if (changed)
            {
                ModManager.RefreshModOptionsUI();
            }
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
            if (!force && presetHash == lastPresetHash)
            {
                return false;
            }

            string selectedPreset = ELDMModOptions.NormalizeLastStandChancePreset(ELDMModOptions.PresetModel);
            string selectedInjuryPreset = ELDMModOptions.NormalizeDamagePreset(ELDMModOptions.PresetInjuryModel);
            string selectedSlownessPreset = ELDMModOptions.NormalizeInjurySlowPreset(ELDMModOptions.PresetLimbSlowModel);
            string selectedKnockoutPreset = ELDMModOptions.NormalizeKnockoutPreset(ELDMModOptions.PresetKnockoutModel);
            string selectedAnimSpeedPreset = ELDMModOptions.NormalizeDeathAnimationSpeedPreset(ELDMModOptions.PresetDeathAnimationSpeedModel);
            string selectedDurationPreset = ELDMModOptions.NormalizeDurationPreset(ELDMModOptions.PresetDurationModel);
            string selectedLimpnessPreset = ELDMModOptions.NormalizeLimpnessPreset(ELDMModOptions.PresetLimpnessModel);

            ELDMModOptions.SourceSnapshot before = ELDMModOptions.CaptureSourceSnapshot();

            bool valuesChanged;
            bool uiChanged;
            valuesChanged = ELDMModOptions.ApplySelectedPresets();
            uiChanged = SyncSourceOfTruthOptions();

            ELDMModOptions.SourceSnapshot after = ELDMModOptions.CaptureSourceSnapshot();
            if (!ELDMModOptions.SourceSnapshotsEqual(before, after))
            {
                LogSnapshotDiff(before, after, source);
            }

            lastSnapshot = after;
            lastPresetHash = ELDMModOptions.GetPresetSelectionHash();

            if (force || valuesChanged || uiChanged)
            {
                ELDMLog.Info(
                    "Preset batch wrote source-of-truth collapsibles: preset=" + selectedPreset +
                    " injury=" + selectedInjuryPreset +
                    " slowness=" + selectedSlownessPreset +
                    " knockout=" + selectedKnockoutPreset +
                    " animSpeed=" + selectedAnimSpeedPreset +
                    " duration=" + selectedDurationPreset +
                    " limpness=" + selectedLimpnessPreset +
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
            changed |= SyncFloatOption(ELDMModOptions.CategoryLegs, ELDMModOptions.OptionLegLimpnessPercent, ELDMModOptions.LegsLimpnessPercent);

            changed |= SyncFloatOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmThreshold, ELDMModOptions.ArmsThresholdDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmDisableDuration, ELDMModOptions.ArmsDisableDurationSeconds);
            changed |= SyncFloatOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmSlowDebuffPercent, ELDMModOptions.ArmsSlowDebuffPercent);
            changed |= SyncBoolOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmSlowStacks, ELDMModOptions.ArmsSlowStacks);
            changed |= SyncFloatOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmLimpnessPercent, ELDMModOptions.ArmsLimpnessPercent);

            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionLastStandEnabled, ELDMModOptions.LastStandEnabled);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionRecoveryDamageReductionPercent, ELDMModOptions.RecoveryDamageReductionPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionDeadRevivalChancePercent, ELDMModOptions.DeadRevivalChancePercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionMaxDeadRecoveries, ELDMModOptions.MaxDeadRecoveriesValue);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionDeadRecoverySlowPercent, ELDMModOptions.DeadRecoverySlowMultiplier);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionReviveStandupFailChancePercent, ELDMModOptions.ReviveStandupFailChancePercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionDeathRecoveryKnockoutMultiplier, ELDMModOptions.DeathRecoveryKnockoutMultiplier);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionDeathAnimationSpeedMultiplier, ELDMModOptions.DeathAnimationSpeedMultiplier);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandRevive, ELDMModOptions.OptionRecoveredMockDeathEyeOpenPercent, ELDMModOptions.RecoveredMockDeathEyeOpenPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandKnockout, ELDMModOptions.OptionMockDeathAnimSpeedMultiplier, ELDMModOptions.MockDeathAnimSpeedMultiplier);

            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStandKnockout, ELDMModOptions.OptionKnockoutEnabled, ELDMModOptions.KnockoutEnabled);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandKnockout, ELDMModOptions.OptionKnockoutDurationSeconds, ELDMModOptions.KnockoutDurationSeconds);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandKnockout, ELDMModOptions.OptionKnockoutRecoverySlowPercent, ELDMModOptions.KnockoutRecoverySlowPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryLastStandKnockout, ELDMModOptions.OptionKnockoutTier1HitDamage, ELDMModOptions.KnockoutTier1HitDamage);
            changed |= SyncFloatOption(ELDMModOptions.CategoryTorso, ELDMModOptions.OptionTorsoSlowDebuffPercent, ELDMModOptions.TorsoSlowDebuffPercent);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStandKnockout, ELDMModOptions.OptionSlowDebuffsStack, ELDMModOptions.SlowDebuffsStack);

            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionLimbDisableSystemEnabled, ELDMModOptions.LimbDisableSystemEnabled);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionSlowDebuffSystemEnabled, ELDMModOptions.SlowDebuffSystemEnabled);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionWeightSystemEnabled, ELDMModOptions.WeightSystemEnabled);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionDeathAnimationSpeedSystemEnabled, ELDMModOptions.DeathAnimationSpeedSystemEnabled);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionMouthPresentationEnabled, ELDMModOptions.MouthPresentationEnabled);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLegs, ELDMModOptions.OptionFallFromLegInjury, ELDMModOptions.FallFromLegInjury);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLegs, ELDMModOptions.OptionLegImmobilization, ELDMModOptions.LegImmobilization);
            changed |= SyncBoolOption(ELDMModOptions.CategoryArms, ELDMModOptions.OptionArmImmobilization, ELDMModOptions.ArmImmobilization);
            changed |= SyncBoolOption(ELDMModOptions.CategoryHands, ELDMModOptions.OptionHandImmobilization, ELDMModOptions.HandImmobilization);
            changed |= SyncBoolOption(ELDMModOptions.CategoryFeet, ELDMModOptions.OptionFootImmobilization, ELDMModOptions.FootImmobilization);
            changed |= SyncBoolOption(ELDMModOptions.CategoryWings, ELDMModOptions.OptionWingImmobilization, ELDMModOptions.WingImmobilization);
            changed |= SyncBoolOption(ELDMModOptions.CategoryHip, ELDMModOptions.OptionTailImmobilization, ELDMModOptions.TailImmobilization);
            changed |= SyncBoolOption(ELDMModOptions.CategoryLastStandKnockout, ELDMModOptions.OptionHeadNeckKnockout, ELDMModOptions.HeadNeckKnockout);
            changed |= SyncBoolOption(ELDMModOptions.CategoryTorso, ELDMModOptions.OptionTorsoPlaceholderLogs, ELDMModOptions.TorsoPlaceholderLogs);
            changed |= SyncFloatOption(ELDMModOptions.CategoryHands, ELDMModOptions.OptionHandLimpnessPercent, ELDMModOptions.HandsLimpnessPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryFeet, ELDMModOptions.OptionFootLimpnessPercent, ELDMModOptions.FeetLimpnessPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryWings, ELDMModOptions.OptionWingLimpnessPercent, ELDMModOptions.WingsLimpnessPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryHip, ELDMModOptions.OptionHipLimpnessPercent, ELDMModOptions.HipLimpnessPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryTorso, ELDMModOptions.OptionTorsoLimpnessPercent, ELDMModOptions.TorsoLimpnessPercent);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionProgressiveTraumaEnabled, ELDMModOptions.ProgressiveTraumaEnabled);
            changed |= SyncFloatOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionTraumaDurationIncreasePercent, ELDMModOptions.TraumaDurationIncreasePercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionTraumaMaxDurationBonusPercent, ELDMModOptions.TraumaMaxDurationBonusPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionPermanentDisableAfterCount, ELDMModOptions.PermanentDisableAfterCountValue);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionAiBehaviorReactions, ELDMModOptions.AiBehaviorReactions);
            changed |= SyncBoolOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionForceDropOnArmDisable, ELDMModOptions.ForceDropOnArmDisable);
            changed |= SyncFloatOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionInjuryReactionSlowPercent, ELDMModOptions.InjuryReactionSlowPercent);
            changed |= SyncFloatOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionInjuryReactionDurationSeconds, ELDMModOptions.InjuryReactionDurationSeconds);
            changed |= SyncFloatOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionReactionCooldownLegSeconds, ELDMModOptions.ReactionCooldownLegSeconds);
            changed |= SyncFloatOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionReactionCooldownArmSeconds, ELDMModOptions.ReactionCooldownArmSeconds);
            changed |= SyncFloatOption(ELDMModOptions.CategoryOptional, ELDMModOptions.OptionReactionCooldownOtherSeconds, ELDMModOptions.ReactionCooldownOtherSeconds);

            changed |= SyncFloatOption(ELDMModOptions.CategoryWeight, ELDMModOptions.OptionWeightBonusPercent, ELDMModOptions.WeightBonusPercent);

            changed |= SyncBoolOption(ELDMModOptions.CategoryDiagnostics, ELDMModOptions.OptionPerformanceSafeguardsEnabled, ELDMModOptions.PerformanceSafeguardsEnabled);
            changed |= SyncFloatOption(ELDMModOptions.CategoryDiagnostics, ELDMModOptions.OptionMaxCreaturesPerUpdate, ELDMModOptions.MaxCreaturesPerUpdateValue);
            changed |= SyncBoolOption(ELDMModOptions.CategoryDiagnostics, ELDMModOptions.OptionEnableBasicLogging, ELDMModOptions.EnableBasicLogging);
            changed |= SyncBoolOption(ELDMModOptions.CategoryDiagnostics, ELDMModOptions.OptionEnableDiagnosticsLogging, ELDMModOptions.EnableDiagnosticsLogging);
            changed |= SyncBoolOption(ELDMModOptions.CategoryDiagnostics, ELDMModOptions.OptionEnableVerboseLogging, ELDMModOptions.EnableVerboseLogging);

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

        private bool SyncStringOption(string category, string optionName, string value)
        {
            if (!TryGetOption(category, optionName, out ModOption option))
            {
                return false;
            }

            if (option.parameterValues == null || option.parameterValues.Length == 0)
            {
                option.LoadModOptionParameters();
            }

            int index = FindStringIndex(option.parameterValues, value);
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
            bool found = modOptionsByKey.TryGetValue(key, out option);
            if (!found && ELDMLog.DiagnosticsEnabled && missingOptionKeysLogged.Add(key))
            {
                ELDMLog.Diag("mod_option_missing key=" + key, verboseOnly: true);
            }

            return found;
        }

        private void RefreshOptionCache()
        {
            modOptionsByKey.Clear();
            missingOptionKeysLogged.Clear();
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

        private static int FindStringIndex(ModOptionParameter[] parameters, string value)
        {
            if (parameters == null)
            {
                return -1;
            }

            string expected = value ?? string.Empty;
            for (int i = 0; i < parameters.Length; i++)
            {
                object parameterValue = parameters[i]?.value;
                if (parameterValue is string s && string.Equals(s, expected, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static void LogSnapshotDiff(ELDMModOptions.SourceSnapshot before, ELDMModOptions.SourceSnapshot after, string source)
        {
            LogFloatDiff(source, "legs.threshold", before.LegsThresholdDamage, after.LegsThresholdDamage);
            LogFloatDiff(source, "legs.duration", before.LegsDisableDurationSeconds, after.LegsDisableDurationSeconds);
            LogFloatDiff(source, "legs.slowDebuff", before.LegsSlowDebuffPercent, after.LegsSlowDebuffPercent);
            LogBoolDiff(source, "legs.slowStacks", before.LegsSlowStacks, after.LegsSlowStacks);
            LogFloatDiff(source, "legs.limpness", before.LegsLimpnessPercent, after.LegsLimpnessPercent);
            LogFloatDiff(source, "arms.threshold", before.ArmsThresholdDamage, after.ArmsThresholdDamage);
            LogFloatDiff(source, "arms.duration", before.ArmsDisableDurationSeconds, after.ArmsDisableDurationSeconds);
            LogFloatDiff(source, "arms.slowDebuff", before.ArmsSlowDebuffPercent, after.ArmsSlowDebuffPercent);
            LogBoolDiff(source, "arms.slowStacks", before.ArmsSlowStacks, after.ArmsSlowStacks);
            LogFloatDiff(source, "arms.limpness", before.ArmsLimpnessPercent, after.ArmsLimpnessPercent);
            LogBoolDiff(source, "optional.lastStand", before.LastStandEnabled, after.LastStandEnabled);
            LogBoolDiff(source, "systems.limbDisable", before.LimbDisableSystemEnabled, after.LimbDisableSystemEnabled);
            LogBoolDiff(source, "systems.slowDebuffs", before.SlowDebuffSystemEnabled, after.SlowDebuffSystemEnabled);
            LogBoolDiff(source, "systems.weight", before.WeightSystemEnabled, after.WeightSystemEnabled);
            LogBoolDiff(source, "systems.deathAnimSpeed", before.DeathAnimationSpeedSystemEnabled, after.DeathAnimationSpeedSystemEnabled);
            LogBoolDiff(source, "systems.mouthPresentation", before.MouthPresentationEnabled, after.MouthPresentationEnabled);
            LogBoolDiff(source, "optional.fallFromLegInjury", before.FallFromLegInjury, after.FallFromLegInjury);
            LogBoolDiff(source, "optional.legImmobilization", before.LegImmobilization, after.LegImmobilization);
            LogBoolDiff(source, "optional.armImmobilization", before.ArmImmobilization, after.ArmImmobilization);
            LogBoolDiff(source, "optional.handDisable", before.HandImmobilization, after.HandImmobilization);
            LogBoolDiff(source, "optional.footDisable", before.FootImmobilization, after.FootImmobilization);
            LogBoolDiff(source, "optional.wingDisable", before.WingImmobilization, after.WingImmobilization);
            LogBoolDiff(source, "optional.hipDisable", before.TailImmobilization, after.TailImmobilization);
            LogBoolDiff(source, "optional.headNeckKO", before.HeadNeckKnockout, after.HeadNeckKnockout);
            LogBoolDiff(source, "optional.torsoPlaceholderLogs", before.TorsoPlaceholderLogs, after.TorsoPlaceholderLogs);
            LogFloatDiff(source, "optional.handLimpness", before.HandsLimpnessPercent, after.HandsLimpnessPercent);
            LogFloatDiff(source, "optional.footLimpness", before.FeetLimpnessPercent, after.FeetLimpnessPercent);
            LogFloatDiff(source, "optional.wingLimpness", before.WingsLimpnessPercent, after.WingsLimpnessPercent);
            LogFloatDiff(source, "optional.hipLimpness", before.HipLimpnessPercent, after.HipLimpnessPercent);
            LogFloatDiff(source, "optional.torsoLimpness", before.TorsoLimpnessPercent, after.TorsoLimpnessPercent);
            LogBoolDiff(source, "optional.progressiveTrauma", before.ProgressiveTraumaEnabled, after.ProgressiveTraumaEnabled);
            LogFloatDiff(source, "optional.traumaIncrease", before.TraumaDurationIncreasePercent, after.TraumaDurationIncreasePercent);
            LogFloatDiff(source, "optional.traumaMax", before.TraumaMaxDurationBonusPercent, after.TraumaMaxDurationBonusPercent);
            LogFloatDiff(source, "optional.permanentAfter", before.PermanentDisableAfterCount, after.PermanentDisableAfterCount);
            LogBoolDiff(source, "optional.aiReactions", before.AiBehaviorReactions, after.AiBehaviorReactions);
            LogBoolDiff(source, "optional.forceDropArm", before.ForceDropOnArmDisable, after.ForceDropOnArmDisable);
            LogFloatDiff(source, "optional.reactionSlow", before.InjuryReactionSlowPercent, after.InjuryReactionSlowPercent);
            LogFloatDiff(source, "optional.reactionDuration", before.InjuryReactionDurationSeconds, after.InjuryReactionDurationSeconds);
            LogFloatDiff(source, "optional.reactionCooldownLeg", before.ReactionCooldownLegSeconds, after.ReactionCooldownLegSeconds);
            LogFloatDiff(source, "optional.reactionCooldownArm", before.ReactionCooldownArmSeconds, after.ReactionCooldownArmSeconds);
            LogFloatDiff(source, "optional.reactionCooldownOther", before.ReactionCooldownOtherSeconds, after.ReactionCooldownOtherSeconds);
            LogFloatDiff(source, "lastStand.recoveryPercent", before.RecoveryDamageReductionPercent, after.RecoveryDamageReductionPercent);
            LogBoolDiff(source, "lastStand.restorePins", before.RecoveryRestoresPinForces, after.RecoveryRestoresPinForces);
            LogFloatDiff(source, "lastStand.deadChance", before.DeadRevivalChancePercent, after.DeadRevivalChancePercent);
            LogFloatDiff(source, "lastStand.maxDeadRecoveries", before.MaxDeadRecoveries, after.MaxDeadRecoveries);
            LogFloatDiff(source, "lastStand.deadRecoverySlow", before.DeadRecoverySlowPercent, after.DeadRecoverySlowPercent);
            LogFloatDiff(source, "lastStand.standFailChance", before.ReviveStandupFailChancePercent, after.ReviveStandupFailChancePercent);
            LogFloatDiff(source, "lastStand.knockoutDuration", before.KnockoutDurationSeconds, after.KnockoutDurationSeconds);
            LogFloatDiff(source, "lastStand.knockoutRecoverySlow", before.KnockoutRecoverySlowPercent, after.KnockoutRecoverySlowPercent);
            LogFloatDiff(source, "lastStand.knockoutTier1Hit", before.KnockoutTier1HitDamage, after.KnockoutTier1HitDamage);
            LogFloatDiff(source, "lastStand.torsoSlow", before.TorsoSlowDebuffPercent, after.TorsoSlowDebuffPercent);
            LogBoolDiff(source, "lastStand.slowDebuffsStack", before.SlowDebuffsStack, after.SlowDebuffsStack);
            LogBoolDiff(source, "optional.knockout", before.KnockoutEnabled, after.KnockoutEnabled);
            LogFloatDiff(source, "weight.bonus", before.WeightBonusPercent, after.WeightBonusPercent);
            LogFloatDiff(source, "lastStand.deathAnimSpeed", before.DeathAnimationSpeedMultiplier, after.DeathAnimationSpeedMultiplier);
            LogFloatDiff(source, "lastStand.mockDeathAnimSpeed", before.MockDeathAnimationSpeedMultiplier, after.MockDeathAnimationSpeedMultiplier);
            LogFloatDiff(source, "lastStand.recoveredEyeOpen", before.RecoveredMockDeathEyeOpenPercent, after.RecoveredMockDeathEyeOpenPercent);
            LogBoolDiff(source, "diag.performanceSafeguards", before.PerformanceSafeguardsEnabled, after.PerformanceSafeguardsEnabled);
            LogFloatDiff(source, "diag.maxCreaturesPerUpdate", before.MaxCreaturesPerUpdate, after.MaxCreaturesPerUpdate);
            LogBoolDiff(source, "diag.debugLogs", before.EnableBasicLogging, after.EnableBasicLogging);
            LogBoolDiff(source, "diag.diagnosticsLogs", before.EnableDiagnosticsLogging, after.EnableDiagnosticsLogging);
            LogBoolDiff(source, "diag.verboseLogs", before.EnableVerboseLogging, after.EnableVerboseLogging);
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
