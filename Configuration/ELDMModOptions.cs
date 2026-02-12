using System;
using ThunderRoad;
using UnityEngine;

namespace EnemyLimbDamageManager.Configuration
{
    public static class ELDMModOptions
    {
        public const string VERSION = "0.6.1";

        // â”€â”€ Preset categories â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string CategoryPresetCore = "Presets";
        public const string CategoryPresetTiming = CategoryPresetCore;
        public const string CategoryPresetSystems = CategoryPresetCore;
        public const string CategoryPresets = CategoryPresetCore;
        public const string CategoryInjuryPresets = CategoryPresetCore;
        public const string CategoryKnockoutLastStand = CategoryPresetCore;
        public const string CategoryNpcWeight = CategoryPresetCore;

        // Keep legacy names referenced in sync and tests
        public const string CategoryLimbPresets = CategoryPresets;
        public const string CategoryLastStandPresets = CategoryPresets;

        // â”€â”€ Collapsible categories â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string CategoryLegs = "Legs";
        public const string CategoryArms = "Arms";
        public const string CategoryHands = "Hands";
        public const string CategoryFeet = "Feet";
        public const string CategoryWings = "Wings";
        public const string CategoryHip = "Hip";
        public const string CategoryTorso = "Torso";
        public const string CategoryLastStandRevive = "Last Stand";
        public const string CategoryLastStandKnockout = "Knockout";
        public const string CategoryLastStand = CategoryLastStandRevive;
        public const string CategoryWeight = "Weight";
        public const string CategorySystems = "Systems";
        public const string CategoryOptional = CategorySystems;
        public const string CategoryDiagnostics = "Advanced";

        // â”€â”€ Option names â€“ presets â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string OptionEnableMod = "Enable Mod";
        public const string OptionPresetModel = "Last Stand Frequency";
        public const string OptionPresetDamageModel = OptionPresetModel;
        public const string OptionPresetInjuryModel = "Limb Fragility";
        public const string OptionPresetLimbSlowModel = "Recovery";
        public const string OptionPresetKnockoutModel = "Knockout Severity";
        public const string OptionPresetLastStandModel = OptionPresetModel;
        public const string OptionPresetRecoveryModel = OptionPresetModel;
        public const string OptionPresetLastStandSlowModel = OptionPresetModel; // legacy alias
        public const string OptionPresetWeightModel = OptionPresetModel;
        public const string OptionPresetDeathAnimationSpeedModel = "Animation Speed";
        public const string OptionPresetDurationModel = "Injury Duration";
        public const string OptionPresetLimpnessModel = "Limpness Preset";
        public const string OptionPresetPerformanceProfileModel = "Performance Profile";
        public const string OptionPresetLoggingProfileModel = "Logging Profile";
        public const string OptionPresetWeaponCurveModel = "Weapon Curve Preset";

        // â”€â”€ Option names â€“ legs â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string OptionLegThreshold = "Leg Disable Threshold";
        public const string OptionLegDisableDuration = "Leg Disable Duration";
        public const string OptionLegSlowDebuffPercent = "Leg Slow Debuff";
        public const string OptionLegSlowStacks = "Leg Slow Stacking";
        public const string OptionLegTier2Threshold = "Leg Tier 2 Threshold";
        public const string OptionLegTier2DurationMultiplier = "Leg Tier 2 Duration";
        public const string OptionLegTier2SlowDebuffPercent = "Leg Tier 2 Slow Debuff";
        public const string OptionLegLimpnessPercent = "Leg Limpness";

        // â”€â”€ Option names â€“ arms â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string OptionArmThreshold = "Arm Disable Threshold";
        public const string OptionArmDisableDuration = "Arm Disable Duration";
        public const string OptionArmSlowDebuffPercent = "Arm Slow Debuff";
        public const string OptionArmSlowStacks = "Arm Slow Stacking";
        public const string OptionArmTier2Threshold = "Arm Tier 2 Threshold";
        public const string OptionArmTier2DurationMultiplier = "Arm Tier 2 Duration";
        public const string OptionArmTier2SlowDebuffPercent = "Arm Tier 2 Slow Debuff";
        public const string OptionArmLimpnessPercent = "Arm Limpness";

        // â”€â”€ Option names â€“ last stand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string OptionRecoveryDamageReductionPercent = "Revive Health";
        public const string OptionRecoveryRestoresPinForces = "Restore Limb Strength";
        public const string OptionDeadRevivalChancePercent = "Revive Chance";
        public const string OptionMaxDeadRecoveries = "Max Revives";
        public const string OptionDeadRecoverySlowPercent = "Revive Slow Multiplier";
        public const string OptionReviveStandupFailChancePercent = "Revive Stand-Up Fail Chance";
        public const string OptionKnockoutDurationSeconds = "KO Duration";
        public const string OptionKnockoutRecoverySlowPercent = "KO Slow";
        public const string OptionKnockoutTier1HitDamage = "KO Threshold";
        public const string OptionKnockoutTier2HitDamage = "KO Tier 2 Threshold";
        public const string OptionKnockoutTier2DurationMultiplier = "KO Tier 2 Duration";
        public const string OptionSlowDebuffsStack = "Stack Slow Effects";
        public const string OptionTorsoSlowDebuffPercent = "Torso Slow Debuff";

        // â”€â”€ Option names â€“ weight â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string OptionWeightBonusPercent = "NPC Weight Bonus";

        // â”€â”€ Option names â€“ death recovery â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string OptionDeathRecoveryKnockoutMultiplier = "Death KO Duration (Seconds)";
        public const string OptionDeathAnimationSpeedMultiplier = "Death Anim Speed";

        // â”€â”€ Option names â€“ optional / diagnostics â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public const string OptionFallFromLegInjury = "Fall From Leg Injury";
        public const string OptionLegImmobilization = "Leg Immobilization";
        public const string OptionArmImmobilization = "Arm Immobilization";
        public const string OptionHandImmobilization = "Hand Disable";
        public const string OptionFootImmobilization = "Foot Disable";
        public const string OptionWingImmobilization = "Wing Disable";
        public const string OptionTailImmobilization = "Hip Disable";
        public const string OptionHeadNeckKnockout = "Head/Neck KO";
        public const string OptionTorsoPlaceholderLogs = "Torso Injury";
        public const string OptionHandLimpnessPercent = "Hand Limpness";
        public const string OptionFootLimpnessPercent = "Foot Limpness";
        public const string OptionWingLimpnessPercent = "Wing Limpness";
        public const string OptionHipLimpnessPercent = "Hip Limpness";
        public const string OptionTorsoLimpnessPercent = "Torso Limpness";
        public const string OptionLastStandEnabled = "Enable Last Stand";
        public const string OptionKnockoutEnabled = "Enable Knockout";
        public const string OptionProgressiveTraumaEnabled = "Progressive Trauma";
        public const string OptionTraumaDurationIncreasePercent = "Trauma Duration Per Disable";
        public const string OptionTraumaMaxDurationBonusPercent = "Trauma Duration Max Bonus";
        public const string OptionPermanentDisableAfterCount = "Permanent Disable After";
        public const string OptionAiBehaviorReactions = "Injury AI Reactions";
        public const string OptionForceDropOnArmDisable = "Drop Items On Arm Disable";
        public const string OptionInjuryReactionSlowPercent = "Reaction Slow Debuff";
        public const string OptionInjuryReactionDurationSeconds = "Reaction Slow Duration";
        public const string OptionPerformanceSafeguardsEnabled = "Performance Safeguards";
        public const string OptionMaxCreaturesPerUpdate = "Max Creatures Per Update";
        public const string OptionLimbDisableSystemEnabled = "Enable Limb Disable";
        public const string OptionSlowDebuffSystemEnabled = "Enable Slow Debuffs";
        public const string OptionWeightSystemEnabled = "Enable Weight System";
        public const string OptionDeathAnimationSpeedSystemEnabled = "Enable Death Animation Speed";
        public const string OptionMouthPresentationEnabled = "Enable Eye/Mouth Presentation";
        public const string OptionReactionCooldownLegSeconds = "Leg Reaction Cooldown";
        public const string OptionReactionCooldownArmSeconds = "Arm Reaction Cooldown";
        public const string OptionReactionCooldownOtherSeconds = "Other Reaction Cooldown";
        public const string OptionWeaponVelocityCurveExponentPercent = "Weapon Curve Strength";

        public const string OptionEnableBasicLogging = "Basic Logs";
        public const string OptionEnableDiagnosticsLogging = "Diagnostics Logs";
        public const string OptionEnableVerboseLogging = "Verbose Logs";
        public const string OptionRecoveredMockDeathEyeOpenPercent = "Recovered Eye Openness";
        public const string OptionMockDeathAnimSpeedMultiplier = "Mock Death Anim Speed";

        // â”€â”€ Preset label constants (all families: 5 options, #3 = Default) â”€â”€

        private const string PresetScaleVeryLow = "Very Low";
        private const string PresetScaleLow = "Low";
        private const string PresetScaleDefault = "Default";
        private const string PresetScaleHigh = "High";
        private const string PresetScaleVeryHigh = "Very High";

        private const string PresetDurationVeryShort = "Very Short";
        private const string PresetDurationShort = "Short";
        private const string PresetDurationDefault = "Default";
        private const string PresetDurationLong = "Long";
        private const string PresetDurationVeryLong = "Very Long";

        private const string PresetLimpnessVeryLight = "Very Light";
        private const string PresetLimpnessLight = "Light";
        private const string PresetLimpnessDefault = "Default";
        private const string PresetLimpnessHeavy = "Heavy";
        private const string PresetLimpnessVeryHeavy = "Very Heavy";

        private const string PresetPerformanceVerySafe = "Very Safe";
        private const string PresetPerformanceSafe = "Safe";
        private const string PresetPerformanceDefault = "Default";
        private const string PresetPerformanceFast = "Fast";
        private const string PresetPerformanceVeryFast = "Very Fast";

        private const string PresetLoggingMinimal = "Minimal";
        private const string PresetLoggingBasic = "Basic";
        private const string PresetLoggingDefault = "Default";
        private const string PresetLoggingDiagnostic = "Diagnostic";
        private const string PresetLoggingVerbose = "Verbose";

        private const string PresetWeaponCurveVeryFlat = "Very Flat";
        private const string PresetWeaponCurveFlat = "Flat";
        private const string PresetWeaponCurveDefault = "Default";
        private const string PresetWeaponCurveSteep = "Steep";
        private const string PresetWeaponCurveVerySteep = "Very Steep";


        // Damage
        public const string PresetDamageLenient = PresetScaleVeryLow;
        public const string PresetDamageMild = PresetScaleLow;
        public const string PresetDamageDefault = PresetScaleDefault;
        public const string PresetDamageSevere = PresetScaleHigh;
        public const string PresetDamageBrutal = PresetScaleVeryHigh;
        public const string PresetDamageExtreme = PresetScaleVeryHigh; // legacy alias

        // Injury Slowness
        public const string PresetInjurySlowMinimal = PresetScaleVeryLow;
        public const string PresetInjurySlowLight = PresetScaleLow;
        public const string PresetInjurySlowDefault = PresetScaleDefault;
        public const string PresetInjurySlowHeavy = PresetScaleHigh;
        public const string PresetInjurySlowCrippling = PresetScaleVeryHigh;

        // Legacy limb-slow aliases used by tests
        public const string PresetLimbSlowLow = PresetScaleVeryLow;
        public const string PresetLimbSlowDefault = PresetScaleDefault;
        public const string PresetLimbSlowHigh = PresetScaleHigh;

        // Knockout
        public const string PresetKnockoutBrief = PresetScaleVeryLow;
        public const string PresetKnockoutShort = PresetScaleLow;
        public const string PresetKnockoutDefault = PresetScaleDefault;
        public const string PresetKnockoutLong = PresetScaleHigh;
        public const string PresetKnockoutExtended = PresetScaleVeryHigh;

        // Last Stand Chance
        public const string PresetLastStandRare = PresetScaleVeryLow;
        public const string PresetLastStandUnlikely = PresetScaleLow;
        public const string PresetLastStandDefault = PresetScaleDefault;
        public const string PresetLastStandLikely = PresetScaleHigh;
        public const string PresetLastStandFrequent = PresetScaleVeryHigh;

        // Legacy last-stand aliases
        public const string PresetLastStandLess = PresetScaleLow;
        public const string PresetLastStandMore = PresetScaleHigh;
        private const string PresetLastStandOffLegacy = "OffLegacy";

        // Recovery
        public const string PresetRecoveryWeak = PresetScaleVeryLow;
        public const string PresetRecoveryFrail = PresetScaleLow;
        public const string PresetRecoveryDefault = PresetScaleDefault;
        public const string PresetRecoveryStrong = PresetScaleHigh;
        public const string PresetRecoveryResilient = PresetScaleVeryHigh;

        // Legacy last-stand-slow aliases
        public const string PresetLastStandSlowLight = PresetScaleVeryLow;
        public const string PresetLastStandSlowDefault = PresetScaleDefault;
        public const string PresetLastStandSlowHeavy = PresetScaleHigh;
        public const string PresetLastStandSlowExtreme = PresetScaleVeryHigh;

        // Weight
        public const string PresetWeightLight = PresetScaleVeryLow;
        public const string PresetWeightModerate = PresetScaleLow;
        public const string PresetWeightDefault = PresetScaleDefault;
        public const string PresetWeightHeavy = PresetScaleHigh;
        public const string PresetWeightMassive = PresetScaleVeryHigh;

        // Death animation speed
        public const string PresetDeathAnimationSpeedVeryLow = PresetScaleVeryLow;
        public const string PresetDeathAnimationSpeedLow = PresetScaleLow;
        public const string PresetDeathAnimationSpeedDefault = PresetScaleDefault;
        public const string PresetDeathAnimationSpeedHigh = PresetScaleHigh;
        public const string PresetDeathAnimationSpeedVeryHigh = PresetScaleVeryHigh;

        // â”€â”€ Hardcoded constants â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private const float HardcodedMinimumTrackedHitDamage = 0.10f;
        private const bool HardcodedHitsRefreshDisableTimer = true;

        // â”€â”€ Source snapshot â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public struct SourceSnapshot
        {
            public float LegsThresholdDamage;
            public float LegsDisableDurationSeconds;
            public float LegsSlowDebuffPercent;
            public bool LegsSlowStacks;
            public float LegsTier2ThresholdDamage;
            public float LegsTier2DurationSeconds;
            public float LegsTier2SlowDebuffPercent;
            public float LegsLimpnessPercent;
            public float ArmsThresholdDamage;
            public float ArmsDisableDurationSeconds;
            public float ArmsSlowDebuffPercent;
            public bool ArmsSlowStacks;
            public float ArmsTier2ThresholdDamage;
            public float ArmsTier2DurationSeconds;
            public float ArmsTier2SlowDebuffPercent;
            public float ArmsLimpnessPercent;
            public bool LastStandEnabled;
            public bool LimbDisableSystemEnabled;
            public bool SlowDebuffSystemEnabled;
            public bool WeightSystemEnabled;
            public bool DeathAnimationSpeedSystemEnabled;
            public bool MouthPresentationEnabled;
            public bool FallFromLegInjury;
            public bool LegImmobilization;
            public bool ArmImmobilization;
            public bool HandImmobilization;
            public bool FootImmobilization;
            public bool WingImmobilization;
            public bool TailImmobilization;
            public bool HeadNeckKnockout;
            public bool TorsoPlaceholderLogs;
            public float HandsLimpnessPercent;
            public float FeetLimpnessPercent;
            public float WingsLimpnessPercent;
            public float HipLimpnessPercent;
            public float TorsoLimpnessPercent;
            public float WeaponVelocityCurveExponentPercent;
            public bool ProgressiveTraumaEnabled;
            public float TraumaDurationIncreasePercent;
            public float TraumaMaxDurationBonusPercent;
            public int PermanentDisableAfterCount;
            public bool AiBehaviorReactions;
            public bool ForceDropOnArmDisable;
            public float InjuryReactionSlowPercent;
            public float InjuryReactionDurationSeconds;
            public float ReactionCooldownLegSeconds;
            public float ReactionCooldownArmSeconds;
            public float ReactionCooldownOtherSeconds;
            public bool PerformanceSafeguardsEnabled;
            public int MaxCreaturesPerUpdate;
            public bool KnockoutEnabled;
            public bool EnableBasicLogging;
            public bool EnableDiagnosticsLogging;
            public bool EnableVerboseLogging;
            public float RecoveryDamageReductionPercent;
            public bool RecoveryRestoresPinForces;
            public float DeadRevivalChancePercent;
            public int MaxDeadRecoveries;
            public float DeadRecoverySlowPercent;
            public float ReviveStandupFailChancePercent;
            public float KnockoutDurationSeconds;
            public float KnockoutRecoverySlowPercent;
            public float KnockoutTier1HitDamage;
            public float KnockoutTier2HitDamage;
            public float KnockoutTier2DurationMultiplier;
            public float DeathRecoveryKnockoutDurationSeconds;
            public float TorsoSlowDebuffPercent;
            public bool SlowDebuffsStack;
            public float WeightBonusPercent;
            public float DeathAnimationSpeedMultiplier;
            public float RecoveredMockDeathEyeOpenPercent;
            public float MockDeathAnimationSpeedMultiplier;
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  ModOption fields
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        [ModOption(name = OptionEnableMod, order = 0, defaultValueIndex = 1, tooltip = "Master switch for limb disable, knockout, and last-stand systems")]
        public static bool EnableMod = true;

        // Presets

        [ModOption(name = OptionPresetModel, category = CategoryPresetCore, categoryOrder = 0, order = 0, defaultValueIndex = 2, valueSourceName = nameof(LastStandFrequencyPresetProvider), tooltip = "How often dead enemies attempt a last-stand recovery")]
        public static string PresetModel = PresetScaleDefault;

        [ModOption(name = OptionPresetInjuryModel, category = CategoryPresetCore, categoryOrder = 0, order = 1, defaultValueIndex = 2, valueSourceName = nameof(LimbFragilityPresetProvider), tooltip = "Controls disable and knockout fragility thresholds")]
        public static string PresetInjuryModel = PresetScaleDefault;

        [ModOption(name = OptionPresetLimbSlowModel, category = CategoryPresetCore, categoryOrder = 0, order = 2, defaultValueIndex = 2, valueSourceName = nameof(RecoveryPresetProvider), tooltip = "Controls post-injury and post-revive slowdown intensity")]
        public static string PresetLimbSlowModel = PresetScaleDefault;

        [ModOption(name = OptionPresetKnockoutModel, category = CategoryPresetCore, categoryOrder = 0, order = 3, defaultValueIndex = 2, valueSourceName = nameof(KnockoutPresetThemeProvider), tooltip = "Controls knockout strength and duration profile")]
        public static string PresetKnockoutModel = PresetScaleDefault;

        [ModOption(name = OptionPresetDeathAnimationSpeedModel, category = CategoryPresetCore, categoryOrder = 0, order = 4, defaultValueIndex = 2, valueSourceName = nameof(AnimationSpeedThemeProvider), tooltip = "Controls death animation playback speed")]
        public static string PresetDeathAnimationSpeedModel = PresetScaleDefault;

        [ModOption(name = OptionPresetDurationModel, category = CategoryPresetTiming, categoryOrder = 0, order = 5, defaultValueIndex = 2, valueSourceName = nameof(DurationPresetProvider), tooltip = "Duration profile for disable, knockout, and revive timing")]
        public static string PresetDurationModel = PresetDurationDefault;

        [ModOption(name = OptionPresetLimpnessModel, category = CategoryPresetTiming, categoryOrder = 0, order = 6, defaultValueIndex = 2, valueSourceName = nameof(LimpnessPresetProvider), tooltip = "How limp disabled body parts become")]
        public static string PresetLimpnessModel = PresetLimpnessDefault;

        // Compatibility aliases used by older configs, tests, and sync code.
        public static string PresetDamageModel { get => PresetInjuryModel; set => PresetInjuryModel = value; }
        public static string PresetInjurySlowModel { get => PresetLimbSlowModel; set => PresetLimbSlowModel = value; }
        public static string PresetLastStandChanceModel { get => PresetModel; set => PresetModel = value; }
        public static string PresetLastStandModel { get => PresetModel; set => PresetModel = value; }
        public static string PresetRecoveryModel { get => PresetLimbSlowModel; set => PresetLimbSlowModel = value; }
        public static string PresetLastStandSlowModel { get => PresetLimbSlowModel; set => PresetLimbSlowModel = value; }
        public static string PresetWeightModel = PresetScaleDefault;
        public static string PresetPerformanceProfileModel = PresetScaleDefault;
        public static string PresetLoggingProfileModel = PresetScaleDefault;
        public static string PresetWeaponCurveModel = PresetScaleDefault;
        public static string PresetPerformanceModel { get => PresetPerformanceProfileModel; set => PresetPerformanceProfileModel = value; }
        public static string PresetLoggingModel { get => PresetLoggingProfileModel; set => PresetLoggingProfileModel = value; }
        public static string PresetWeaponCurveProfileModel { get => PresetWeaponCurveModel; set => PresetWeaponCurveModel = value; }
        public static float DeadRecoverySlowPercent { get => DeadRecoverySlowMultiplier; set => DeadRecoverySlowMultiplier = value; }
        [ModOption(name = OptionDeathRecoveryKnockoutMultiplier, category = CategoryLastStandRevive, categoryOrder = 120, order = 75, defaultValueIndex = 5, valueSourceName = nameof(DeathRecoveryDurationSecondsProvider), interactionType = (ModOption.InteractionType)2, tooltip = "How long the dead-looking mimic knockout lasts before revive")]
        public static float DeathRecoveryKnockoutMultiplier = 15f;


        // â”€â”€ Legs â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [ModOption(name = OptionLegThreshold, category = CategoryLegs, categoryOrder = 100, order = 0, defaultValueIndex = 8, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Single-hit damage needed for leg disable (blunt/slash)")]
        public static float LegsThresholdDamage = 12f;

        [ModOption(name = OptionLegDisableDuration, category = CategoryLegs, categoryOrder = 100, order = 10, defaultValueIndex = 8, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LegsDisableDurationSeconds = 25f;

        [ModOption(name = OptionLegSlowDebuffPercent, category = CategoryLegs, categoryOrder = 100, order = 20, defaultValueIndex = 2, valueSourceName = nameof(FivePercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float LegsSlowDebuffPercent = 25f;

        [ModOption(name = OptionLegSlowStacks, category = CategoryLegs, categoryOrder = 100, order = 30, defaultValueIndex = 1)]
        public static bool LegsSlowStacks = true;

        public static float LegsTier2ThresholdDamage = 20f;

        public static float LegsTier2DurationMultiplier = 35f;

        public static float LegsTier2SlowDebuffPercent = 50f;

        [ModOption(name = OptionLegLimpnessPercent, category = CategoryLegs, categoryOrder = 100, order = 38, defaultValueIndex = 8, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2, tooltip = "How limp disabled legs become (higher = more limp)")]
        public static float LegsLimpnessPercent = 80f;

        // â”€â”€ Arms â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [ModOption(name = OptionArmThreshold, category = CategoryArms, categoryOrder = 110, order = 0, defaultValueIndex = 10, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Single-hit damage needed for arm disable (blunt/slash)")]
        public static float ArmsThresholdDamage = 14f;

        [ModOption(name = OptionArmDisableDuration, category = CategoryArms, categoryOrder = 110, order = 10, defaultValueIndex = 6, valueSourceName = nameof(DisableDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float ArmsDisableDurationSeconds = 20f;

        [ModOption(name = OptionArmSlowDebuffPercent, category = CategoryArms, categoryOrder = 110, order = 20, defaultValueIndex = 1, valueSourceName = nameof(FivePercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float ArmsSlowDebuffPercent = 18f;

        [ModOption(name = OptionArmSlowStacks, category = CategoryArms, categoryOrder = 110, order = 30, defaultValueIndex = 1)]
        public static bool ArmsSlowStacks = true;

        public static float ArmsTier2ThresholdDamage = 21f;

        public static float ArmsTier2DurationMultiplier = 20f;

        public static float ArmsTier2SlowDebuffPercent = 40f;

        [ModOption(name = OptionArmLimpnessPercent, category = CategoryArms, categoryOrder = 110, order = 38, defaultValueIndex = 6, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2, tooltip = "How limp disabled arms become (higher = more limp)")]
        public static float ArmsLimpnessPercent = 60f;

        // â”€â”€ Last Stand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [ModOption(name = OptionRecoveryDamageReductionPercent, category = CategoryLastStandRevive, categoryOrder = 120, order = 20, defaultValueIndex = 4, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float RecoveryDamageReductionPercent = 40f;

        // Always disabled: disabled limbs should not auto-restore full strength.
        public static bool RecoveryRestoresPinForces = false;

        [ModOption(name = OptionDeadRevivalChancePercent, category = CategoryLastStandRevive, categoryOrder = 120, order = 40, defaultValueIndex = 2, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float DeadRevivalChancePercent = 20f;

        [ModOption(name = OptionMaxDeadRecoveries, category = CategoryLastStandRevive, categoryOrder = 120, order = 50, defaultValueIndex = 3, valueSourceName = nameof(MaxDeadRecoveriesProvider), interactionType = (ModOption.InteractionType)2)]
        public static float MaxDeadRecoveriesValue = 3f;

        [ModOption(name = OptionDeadRecoverySlowPercent, category = CategoryLastStandRevive, categoryOrder = 120, order = 60, defaultValueIndex = 3, valueSourceName = nameof(ReviveSlowMultiplierProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Movement slowdown multiplier after revive (1.0 = none, higher = slower)")]
        public static float DeadRecoverySlowMultiplier = 1.3f;

        [ModOption(name = OptionReviveStandupFailChancePercent, category = CategoryLastStandRevive, categoryOrder = 120, order = 65, defaultValueIndex = 10, valueSourceName = nameof(FivePercentProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Chance that a revive stand-up attempt visibly fails and stalls before retrying")]
        public static float ReviveStandupFailChancePercent = 50f;

        [ModOption(name = OptionLastStandEnabled, category = CategoryLastStandRevive, categoryOrder = 120, order = 0, defaultValueIndex = 1)]
        public static bool LastStandEnabled = true;

        [ModOption(name = OptionKnockoutDurationSeconds, category = CategoryLastStandKnockout, categoryOrder = 130, order = 10, defaultValueIndex = 2, valueSourceName = nameof(KnockoutDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float KnockoutDurationSeconds = 6f;

        [ModOption(name = OptionKnockoutEnabled, category = CategoryLastStandKnockout, categoryOrder = 130, order = 0, defaultValueIndex = 1)]
        public static bool KnockoutEnabled = true;

        [ModOption(name = OptionKnockoutRecoverySlowPercent, category = CategoryLastStandKnockout, categoryOrder = 130, order = 20, defaultValueIndex = 4, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float KnockoutRecoverySlowPercent = 30f;

        [ModOption(name = OptionKnockoutTier1HitDamage, category = CategoryLastStandKnockout, categoryOrder = 130, order = 30, defaultValueIndex = 3, valueSourceName = nameof(ThresholdDamageProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Single-hit damage needed for knockout (blunt/slash)")]
        public static float KnockoutTier1HitDamage = 20f;

        public static float KnockoutTier2HitDamage = 30f;

        public static float KnockoutTier2DurationMultiplier = 12.0f;

        [ModOption(name = OptionTorsoSlowDebuffPercent, category = CategoryTorso, categoryOrder = 116, order = 40, defaultValueIndex = 6, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Slow debuff while torso injury is active")]
        public static float TorsoSlowDebuffPercent = 60f;

        [ModOption(name = OptionSlowDebuffsStack, category = CategoryLastStandKnockout, categoryOrder = 130, order = 70, defaultValueIndex = 1)]
        public static bool SlowDebuffsStack = true;

        // â”€â”€ Weight â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [ModOption(name = OptionWeightBonusPercent, category = CategoryWeight, categoryOrder = 130, order = 0, defaultValueIndex = 5, valueSourceName = nameof(WeightBonusPercentProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Additional weight added to all NPC body parts (% of original mass)")]
        public static float WeightBonusPercent = 50f;

        // â”€â”€ Death Recovery â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [ModOption(name = OptionDeathAnimationSpeedMultiplier, category = CategoryLastStandRevive, categoryOrder = 120, order = 80, defaultValueIndex = 3, valueSourceName = nameof(DeathAnimationSpeedProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Animation speed multiplier for death animations (lower = slower deaths)")]
        public static float DeathAnimationSpeedMultiplier = 0.75f;

        [ModOption(name = OptionRecoveredMockDeathEyeOpenPercent, category = CategoryLastStandRevive, categoryOrder = 120, order = 85, defaultValueIndex = 5, valueSourceName = nameof(RecoveredEyeOpenPercentProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Eye openness after mock-death recovery (keeps a rolled, gazy look)")]
        public static float RecoveredMockDeathEyeOpenPercent = 45f;

        [ModOption(name = OptionMockDeathAnimSpeedMultiplier, category = CategoryLastStandKnockout, categoryOrder = 130, order = 25, defaultValueIndex = 6, valueSourceName = nameof(MockDeathAnimationSpeedProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Animator speed during knockout/mock-death (lower = longer animations)")]
        public static float MockDeathAnimSpeedMultiplier = 0.40f;

        // â”€â”€ Optional â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [ModOption(name = OptionLimbDisableSystemEnabled, category = CategoryOptional, categoryOrder = 180, order = 0, defaultValueIndex = 1, tooltip = "Master toggle for applying limb disable states")]
        public static bool LimbDisableSystemEnabled = true;

        [ModOption(name = OptionSlowDebuffSystemEnabled, category = CategoryOptional, categoryOrder = 180, order = 1, defaultValueIndex = 1, tooltip = "Master toggle for movement slow debuffs")]
        public static bool SlowDebuffSystemEnabled = true;

        [ModOption(name = OptionWeightSystemEnabled, category = CategoryOptional, categoryOrder = 180, order = 2, defaultValueIndex = 1, tooltip = "Master toggle for NPC weight scaling")]
        public static bool WeightSystemEnabled = true;

        [ModOption(name = OptionDeathAnimationSpeedSystemEnabled, category = CategoryOptional, categoryOrder = 180, order = 3, defaultValueIndex = 1, tooltip = "Master toggle for death animation speed override")]
        public static bool DeathAnimationSpeedSystemEnabled = true;

        [ModOption(name = OptionMouthPresentationEnabled, category = CategoryOptional, categoryOrder = 180, order = 4, defaultValueIndex = 1, tooltip = "Master toggle for eye and mouth death/KO presentation")]
        public static bool MouthPresentationEnabled = true;

        [ModOption(name = OptionFallFromLegInjury, category = CategoryLegs, categoryOrder = 100, order = 2, defaultValueIndex = 1)]
        public static bool FallFromLegInjury = true;

        [ModOption(name = OptionLegImmobilization, category = CategoryLegs, categoryOrder = 100, order = 3, defaultValueIndex = 1)]
        public static bool LegImmobilization = true;

        [ModOption(name = OptionArmImmobilization, category = CategoryArms, categoryOrder = 110, order = 2, defaultValueIndex = 1)]
        public static bool ArmImmobilization = true;

        [ModOption(name = OptionHandImmobilization, category = CategoryHands, categoryOrder = 112, order = 0, defaultValueIndex = 1, tooltip = "Enable disables for left/right hands")]
        public static bool HandImmobilization = true;

        [ModOption(name = OptionFootImmobilization, category = CategoryFeet, categoryOrder = 113, order = 0, defaultValueIndex = 1, tooltip = "Enable disables for left/right feet")]
        public static bool FootImmobilization = true;

        [ModOption(name = OptionWingImmobilization, category = CategoryWings, categoryOrder = 114, order = 0, defaultValueIndex = 1, tooltip = "Enable disables for left/right wings")]
        public static bool WingImmobilization = true;

        [ModOption(name = OptionTailImmobilization, category = CategoryHip, categoryOrder = 115, order = 0, defaultValueIndex = 1, tooltip = "Enable disables for hip hits")]
        public static bool TailImmobilization = true;

        [ModOption(name = OptionHeadNeckKnockout, category = CategoryLastStandKnockout, categoryOrder = 130, order = 80, defaultValueIndex = 1, tooltip = "Allow head/neck hits to use knockout tiers")]
        public static bool HeadNeckKnockout = true;

        [ModOption(name = OptionTorsoPlaceholderLogs, category = CategoryTorso, categoryOrder = 116, order = 0, defaultValueIndex = 1, tooltip = "Enable torso injury behavior")]
        public static bool TorsoPlaceholderLogs = true;

        [ModOption(name = OptionHandLimpnessPercent, category = CategoryHands, categoryOrder = 112, order = 10, defaultValueIndex = 8, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float HandsLimpnessPercent = 80f;

        [ModOption(name = OptionFootLimpnessPercent, category = CategoryFeet, categoryOrder = 113, order = 10, defaultValueIndex = 9, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float FeetLimpnessPercent = 90f;

        [ModOption(name = OptionWingLimpnessPercent, category = CategoryWings, categoryOrder = 114, order = 10, defaultValueIndex = 8, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float WingsLimpnessPercent = 80f;

        [ModOption(name = OptionHipLimpnessPercent, category = CategoryHip, categoryOrder = 115, order = 10, defaultValueIndex = 10, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float HipLimpnessPercent = 100f;

        [ModOption(name = OptionTorsoLimpnessPercent, category = CategoryTorso, categoryOrder = 116, order = 50, defaultValueIndex = 8, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float TorsoLimpnessPercent = 80f;

        public static float WeaponVelocityCurveExponentPercent = 100f;

        [ModOption(name = OptionProgressiveTraumaEnabled, category = CategoryOptional, categoryOrder = 180, order = 42, defaultValueIndex = 1, tooltip = "Repeated disables increase future disable duration")]
        public static bool ProgressiveTraumaEnabled = true;

        [ModOption(name = OptionTraumaDurationIncreasePercent, category = CategoryOptional, categoryOrder = 180, order = 43, defaultValueIndex = 2, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float TraumaDurationIncreasePercent = 20f;

        [ModOption(name = OptionTraumaMaxDurationBonusPercent, category = CategoryOptional, categoryOrder = 180, order = 44, defaultValueIndex = 8, valueSourceName = nameof(TwentyPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float TraumaMaxDurationBonusPercent = 160f;

        [ModOption(name = OptionPermanentDisableAfterCount, category = CategoryOptional, categoryOrder = 180, order = 45, defaultValueIndex = 0, valueSourceName = nameof(PermanentDisableCountProvider), interactionType = (ModOption.InteractionType)2, tooltip = "Set to 0 to disable permanent injuries")]
        public static float PermanentDisableAfterCountValue = 0f;

        [ModOption(name = OptionAiBehaviorReactions, category = CategoryOptional, categoryOrder = 180, order = 46, defaultValueIndex = 1, tooltip = "Apply AI behavior reactions when limbs are disabled")]
        public static bool AiBehaviorReactions = true;

        [ModOption(name = OptionForceDropOnArmDisable, category = CategoryOptional, categoryOrder = 180, order = 47, defaultValueIndex = 1)]
        public static bool ForceDropOnArmDisable = true;

        [ModOption(name = OptionInjuryReactionSlowPercent, category = CategoryOptional, categoryOrder = 180, order = 48, defaultValueIndex = 2, valueSourceName = nameof(TenPercentProvider), interactionType = (ModOption.InteractionType)2)]
        public static float InjuryReactionSlowPercent = 20f;

        [ModOption(name = OptionInjuryReactionDurationSeconds, category = CategoryOptional, categoryOrder = 180, order = 49, defaultValueIndex = 4, valueSourceName = nameof(ReactionDurationProvider), interactionType = (ModOption.InteractionType)2)]
        public static float InjuryReactionDurationSeconds = 6f;

        [ModOption(name = OptionReactionCooldownLegSeconds, category = CategoryOptional, categoryOrder = 180, order = 50, defaultValueIndex = 4, valueSourceName = nameof(ReactionCooldownProvider), interactionType = (ModOption.InteractionType)2)]
        public static float ReactionCooldownLegSeconds = 2.0f;

        [ModOption(name = OptionReactionCooldownArmSeconds, category = CategoryOptional, categoryOrder = 180, order = 51, defaultValueIndex = 3, valueSourceName = nameof(ReactionCooldownProvider), interactionType = (ModOption.InteractionType)2)]
        public static float ReactionCooldownArmSeconds = 1.5f;

        [ModOption(name = OptionReactionCooldownOtherSeconds, category = CategoryOptional, categoryOrder = 180, order = 52, defaultValueIndex = 3, valueSourceName = nameof(ReactionCooldownProvider), interactionType = (ModOption.InteractionType)2)]
        public static float ReactionCooldownOtherSeconds = 1.5f;

        [ModOption(name = OptionPerformanceSafeguardsEnabled, category = CategoryDiagnostics, categoryOrder = 200, order = 6, defaultValueIndex = 1)]
        public static bool PerformanceSafeguardsEnabled = true;

        [ModOption(name = OptionMaxCreaturesPerUpdate, category = CategoryDiagnostics, categoryOrder = 200, order = 8, defaultValueIndex = 7, valueSourceName = nameof(MaxCreaturesPerUpdateProvider), interactionType = (ModOption.InteractionType)2)]
        public static float MaxCreaturesPerUpdateValue = 80f;

        // â”€â”€ Diagnostics â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [ModOption(name = OptionEnableBasicLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 0, defaultValueIndex = 0, tooltip = "Master debug toggle for focused mod logs")]
        public static bool EnableBasicLogging = false;

        [ModOption(name = OptionEnableDiagnosticsLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 1, defaultValueIndex = 0, tooltip = "Enable detailed diagnostics logs")]
        public static bool EnableDiagnosticsLogging = false;

        [ModOption(name = OptionEnableVerboseLogging, category = CategoryDiagnostics, categoryOrder = 200, order = 2, defaultValueIndex = 0, tooltip = "Enable high-volume verbose diagnostics")]
        public static bool EnableVerboseLogging = false;

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  Preset providers (dropdown values)
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        public static ModOptionString[] PresetProvider() => new[]
        {
            new ModOptionString(PresetScaleVeryLow, PresetScaleVeryLow),
            new ModOptionString(PresetScaleLow, PresetScaleLow),
            new ModOptionString(PresetScaleDefault, PresetScaleDefault),
            new ModOptionString(PresetScaleHigh, PresetScaleHigh),
            new ModOptionString(PresetScaleVeryHigh, PresetScaleVeryHigh),
        };

        public static ModOptionString[] LimbFragilityPresetProvider() => new[]
        {
            new ModOptionString("Off", PresetScaleVeryLow),
            new ModOptionString("Weak", PresetScaleLow),
            new ModOptionString("Default", PresetScaleDefault),
            new ModOptionString("Strong", PresetScaleHigh),
            new ModOptionString("Durable", PresetScaleVeryHigh),
        };

        public static ModOptionString[] LastStandFrequencyPresetProvider() => new[]
        {
            new ModOptionString("Off", "Off"),
            new ModOptionString("Rare", "Rare"),
            new ModOptionString("Default", "Default"),
            new ModOptionString("Frequent", "Frequent"),
            new ModOptionString("Always", "Always"),
        };

        public static ModOptionString[] RecoveryPresetProvider() => new[]
        {
            new ModOptionString("Minimal", PresetScaleVeryLow),
            new ModOptionString("Light", PresetScaleLow),
            new ModOptionString("Default", PresetScaleDefault),
            new ModOptionString("Heavy", PresetScaleHigh),
            new ModOptionString("Extreme", PresetScaleVeryHigh),
        };

        public static ModOptionString[] KnockoutPresetThemeProvider() => new[]
        {
            new ModOptionString("Brief", PresetScaleVeryLow),
            new ModOptionString("Short", PresetScaleLow),
            new ModOptionString("Default", PresetScaleDefault),
            new ModOptionString("Long", PresetScaleHigh),
            new ModOptionString("Extended", PresetScaleVeryHigh),
        };

        public static ModOptionString[] AnimationSpeedThemeProvider() => new[]
        {
            new ModOptionString("Very Slow", PresetScaleVeryLow),
            new ModOptionString("Slow", PresetScaleLow),
            new ModOptionString("Default", PresetScaleDefault),
            new ModOptionString("Fast", PresetScaleHigh),
            new ModOptionString("Very Fast", PresetScaleVeryHigh),
        };

        public static ModOptionString[] DurationPresetProvider() => new[]
        {
            new ModOptionString(PresetDurationVeryShort, PresetDurationVeryShort),
            new ModOptionString(PresetDurationShort, PresetDurationShort),
            new ModOptionString(PresetDurationDefault, PresetDurationDefault),
            new ModOptionString(PresetDurationLong, PresetDurationLong),
            new ModOptionString(PresetDurationVeryLong, PresetDurationVeryLong),
        };

        public static ModOptionString[] LimpnessPresetProvider() => new[]
        {
            new ModOptionString(PresetLimpnessVeryLight, PresetLimpnessVeryLight),
            new ModOptionString(PresetLimpnessLight, PresetLimpnessLight),
            new ModOptionString(PresetLimpnessDefault, PresetLimpnessDefault),
            new ModOptionString(PresetLimpnessHeavy, PresetLimpnessHeavy),
            new ModOptionString(PresetLimpnessVeryHeavy, PresetLimpnessVeryHeavy),
        };

        public static ModOptionString[] PerformanceProfilePresetProvider() => new[]
        {
            new ModOptionString(PresetPerformanceVerySafe, PresetPerformanceVerySafe),
            new ModOptionString(PresetPerformanceSafe, PresetPerformanceSafe),
            new ModOptionString(PresetPerformanceDefault, PresetPerformanceDefault),
            new ModOptionString(PresetPerformanceFast, PresetPerformanceFast),
            new ModOptionString(PresetPerformanceVeryFast, PresetPerformanceVeryFast),
        };

        public static ModOptionString[] LoggingProfilePresetProvider() => new[]
        {
            new ModOptionString(PresetLoggingMinimal, PresetLoggingMinimal),
            new ModOptionString(PresetLoggingBasic, PresetLoggingBasic),
            new ModOptionString(PresetLoggingDefault, PresetLoggingDefault),
            new ModOptionString(PresetLoggingDiagnostic, PresetLoggingDiagnostic),
            new ModOptionString(PresetLoggingVerbose, PresetLoggingVerbose),
        };

        public static ModOptionString[] WeaponCurvePresetProvider() => new[]
        {
            new ModOptionString(PresetWeaponCurveVeryFlat, PresetWeaponCurveVeryFlat),
            new ModOptionString(PresetWeaponCurveFlat, PresetWeaponCurveFlat),
            new ModOptionString(PresetWeaponCurveDefault, PresetWeaponCurveDefault),
            new ModOptionString(PresetWeaponCurveSteep, PresetWeaponCurveSteep),
            new ModOptionString(PresetWeaponCurveVerySteep, PresetWeaponCurveVerySteep),
        };

        // Compatibility aliases for legacy preset families.
        public static ModOptionString[] DamagePresetProvider() => LimbFragilityPresetProvider();
        public static ModOptionString[] InjurySlowPresetProvider() => RecoveryPresetProvider();
        public static ModOptionString[] LimbSlowPresetProvider() => RecoveryPresetProvider();
        public static ModOptionString[] KnockoutPresetProvider() => KnockoutPresetThemeProvider();
        public static ModOptionString[] LastStandChancePresetProvider() => LastStandFrequencyPresetProvider();
        public static ModOptionString[] LastStandPresetProvider() => LastStandFrequencyPresetProvider();
        public static ModOptionString[] LastStandSlowPresetProvider() => RecoveryPresetProvider();
        public static ModOptionString[] WeightPresetProvider() => PresetProvider();
        public static ModOptionString[] DeathAnimationSpeedPresetProvider() => AnimationSpeedThemeProvider();


        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  Collapsible value providers
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

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
            new ModOptionFloat("4s", 4f), new ModOptionFloat("6s", 6f), new ModOptionFloat("8s", 8f),
            new ModOptionFloat("10s", 10f), new ModOptionFloat("12s", 12f), new ModOptionFloat("15s", 15f),
            new ModOptionFloat("18s", 18f), new ModOptionFloat("20s", 20f), new ModOptionFloat("25s", 25f),
            new ModOptionFloat("30s", 30f), new ModOptionFloat("35s", 35f), new ModOptionFloat("40s", 40f),
            new ModOptionFloat("50s", 50f), new ModOptionFloat("60s", 60f), new ModOptionFloat("80s", 80f),
            new ModOptionFloat("120s", 120f),
        };

        public static ModOptionFloat[] ReviveSlowMultiplierProvider() => new[]
        {
            new ModOptionFloat("1.0x", 1.0f), new ModOptionFloat("1.1x", 1.1f), new ModOptionFloat("1.2x", 1.2f),
            new ModOptionFloat("1.3x", 1.3f), new ModOptionFloat("1.4x", 1.4f), new ModOptionFloat("1.5x", 1.5f),
            new ModOptionFloat("1.6x", 1.6f), new ModOptionFloat("1.8x", 1.8f), new ModOptionFloat("2.0x", 2.0f),
            new ModOptionFloat("2.2x", 2.2f), new ModOptionFloat("2.5x", 2.5f), new ModOptionFloat("3.0x", 3.0f),
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

        public static ModOptionFloat[] TwentyPercentProvider()
        {
            ModOptionFloat[] values = new ModOptionFloat[16];
            for (int i = 0; i < values.Length; i++)
            {
                float value = i * 20f;
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
            new ModOptionFloat("16s", 16f), new ModOptionFloat("20s", 20f), new ModOptionFloat("25s", 25f),
        };

        public static ModOptionFloat[] PermanentDisableCountProvider() => new[]
        {
            new ModOptionFloat("0 (Off)", 0f), new ModOptionFloat("1", 1f), new ModOptionFloat("2", 2f),
            new ModOptionFloat("3", 3f), new ModOptionFloat("4", 4f), new ModOptionFloat("5", 5f),
            new ModOptionFloat("6", 6f),
        };

        public static ModOptionFloat[] ReactionDurationProvider() => new[]
        {
            new ModOptionFloat("2s", 2f), new ModOptionFloat("3s", 3f), new ModOptionFloat("4s", 4f),
            new ModOptionFloat("5s", 5f), new ModOptionFloat("6s", 6f), new ModOptionFloat("8s", 8f),
            new ModOptionFloat("10s", 10f), new ModOptionFloat("12s", 12f), new ModOptionFloat("15s", 15f),
        };

        public static ModOptionFloat[] ReactionCooldownProvider() => new[]
        {
            new ModOptionFloat("0.0s", 0.0f), new ModOptionFloat("0.5s", 0.5f), new ModOptionFloat("1.0s", 1.0f),
            new ModOptionFloat("1.5s", 1.5f), new ModOptionFloat("2.0s", 2.0f), new ModOptionFloat("3.0s", 3.0f),
            new ModOptionFloat("4.0s", 4.0f), new ModOptionFloat("5.0s", 5.0f), new ModOptionFloat("7.5s", 7.5f),
            new ModOptionFloat("10.0s", 10.0f),
        };

        public static ModOptionFloat[] WeaponCurveStrengthProvider() => new[]
        {
            new ModOptionFloat("40%", 40f), new ModOptionFloat("50%", 50f), new ModOptionFloat("60%", 60f),
            new ModOptionFloat("70%", 70f), new ModOptionFloat("80%", 80f), new ModOptionFloat("90%", 90f),
            new ModOptionFloat("100%", 100f), new ModOptionFloat("110%", 110f), new ModOptionFloat("120%", 120f),
            new ModOptionFloat("140%", 140f), new ModOptionFloat("160%", 160f), new ModOptionFloat("180%", 180f),
            new ModOptionFloat("200%", 200f),
        };

        public static ModOptionFloat[] MaxCreaturesPerUpdateProvider() => new[]
        {
            new ModOptionFloat("20", 20f), new ModOptionFloat("30", 30f), new ModOptionFloat("40", 40f),
            new ModOptionFloat("50", 50f), new ModOptionFloat("60", 60f), new ModOptionFloat("70", 70f),
            new ModOptionFloat("80", 80f), new ModOptionFloat("100", 100f), new ModOptionFloat("120", 120f),
            new ModOptionFloat("160", 160f), new ModOptionFloat("200", 200f),
        };

        public static ModOptionFloat[] Tier2DurationMultiplierProvider() => new[]
        {
            new ModOptionFloat("1.2x", 1.2f), new ModOptionFloat("1.5x", 1.5f), new ModOptionFloat("1.8x", 1.8f),
            new ModOptionFloat("2.0x", 2.0f), new ModOptionFloat("2.5x", 2.5f), new ModOptionFloat("3.0x", 3.0f),
            new ModOptionFloat("4.0x", 4.0f), new ModOptionFloat("5.0x", 5.0f),
        };

        public static ModOptionFloat[] WeightBonusPercentProvider() => new[]
        {
            new ModOptionFloat("0%", 0f), new ModOptionFloat("10%", 10f), new ModOptionFloat("20%", 20f),
            new ModOptionFloat("30%", 30f), new ModOptionFloat("40%", 40f), new ModOptionFloat("50%", 50f),
            new ModOptionFloat("60%", 60f), new ModOptionFloat("70%", 70f), new ModOptionFloat("80%", 80f),
            new ModOptionFloat("100%", 100f), new ModOptionFloat("150%", 150f), new ModOptionFloat("200%", 200f),
        };

        public static ModOptionFloat[] DeathRecoveryDurationSecondsProvider() => new[]
        {
            new ModOptionFloat("4s", 4f), new ModOptionFloat("6s", 6f), new ModOptionFloat("8s", 8f),
            new ModOptionFloat("10s", 10f), new ModOptionFloat("12s", 12f), new ModOptionFloat("15s", 15f),
            new ModOptionFloat("18s", 18f), new ModOptionFloat("20s", 20f), new ModOptionFloat("25s", 25f),
            new ModOptionFloat("30s", 30f),
        };

        public static ModOptionFloat[] DeathRecoveryKnockoutMultiplierProvider() => DeathRecoveryDurationSecondsProvider();

        public static ModOptionFloat[] DeathAnimationSpeedProvider() => new[]
        {
            new ModOptionFloat("0.10x", 0.10f), new ModOptionFloat("0.25x", 0.25f), new ModOptionFloat("0.50x", 0.50f),
            new ModOptionFloat("0.70x", 0.70f), new ModOptionFloat("0.75x", 0.75f), new ModOptionFloat("1.00x", 1.00f),
            new ModOptionFloat("1.25x", 1.25f), new ModOptionFloat("1.50x", 1.50f), new ModOptionFloat("2.00x", 2.00f),
        };

        public static ModOptionFloat[] RecoveredEyeOpenPercentProvider() => new[]
        {
            new ModOptionFloat("20%", 20f), new ModOptionFloat("25%", 25f), new ModOptionFloat("30%", 30f),
            new ModOptionFloat("35%", 35f), new ModOptionFloat("40%", 40f), new ModOptionFloat("45%", 45f),
            new ModOptionFloat("50%", 50f), new ModOptionFloat("55%", 55f), new ModOptionFloat("60%", 60f),
            new ModOptionFloat("65%", 65f), new ModOptionFloat("70%", 70f), new ModOptionFloat("75%", 75f),
            new ModOptionFloat("80%", 80f),
        };

        public static ModOptionFloat[] MockDeathAnimationSpeedProvider() => new[]
        {
            new ModOptionFloat("0.10x", 0.10f), new ModOptionFloat("0.15x", 0.15f), new ModOptionFloat("0.20x", 0.20f),
            new ModOptionFloat("0.25x", 0.25f), new ModOptionFloat("0.30x", 0.30f), new ModOptionFloat("0.35x", 0.35f),
            new ModOptionFloat("0.40x", 0.40f), new ModOptionFloat("0.50x", 0.50f), new ModOptionFloat("0.60x", 0.60f),
            new ModOptionFloat("0.75x", 0.75f), new ModOptionFloat("1.00x", 1.00f),
        };

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  ApplySelectedPresets
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        public static bool ApplySelectedPresets()
        {
            string selectedPreset = NormalizeLastStandChancePreset(PresetModel);
            string selectedInjuryPreset = NormalizeDamagePreset(PresetInjuryModel);
            string selectedSlownessPreset = NormalizeInjurySlowPreset(PresetLimbSlowModel);
            string selectedKnockoutPreset = NormalizeKnockoutPreset(PresetKnockoutModel);
            string selectedAnimationSpeedPreset = NormalizeDeathAnimationSpeedPreset(PresetDeathAnimationSpeedModel);
            string selectedDurationPreset = NormalizeDurationPreset(PresetDurationModel);
            string selectedLimpnessPreset = NormalizeLimpnessPreset(PresetLimpnessModel);

            ResolveDamagePreset(selectedInjuryPreset, out float legsThreshold, out float armsThreshold, out float legsDuration, out float armsDuration);
            ResolveInjurySlowPreset(selectedSlownessPreset, out float legsSlow, out float armsSlow, out float legsTier2Slow, out float armsTier2Slow, out bool legsSlowStacks, out bool armsSlowStacks);
            ResolveKnockoutPreset(
                selectedKnockoutPreset,
                out float knockoutDuration,
                out float knockoutSlow,
                out float knockoutTier1HitDamage,
                out float knockoutTier2HitDamage,
                out float knockoutTier2DurationMultiplier);
            ResolveLastStandChancePreset(selectedPreset, out float deadChance, out int maxRecoveries, out bool? forceLastStandEnabled);
            ResolveRecoveryPreset(NormalizeRecoveryPreset(PresetLimbSlowModel), out float recoveryReduction, out bool restorePins, out float deadSlow, out bool slowDebuffsStack);
            ResolveDurationPreset(selectedDurationPreset, out float presetLegDuration, out float presetArmDuration, out float presetKnockoutDuration);
            ResolveLimpnessPreset(selectedLimpnessPreset, out float legLimpness, out float armLimpness, out float handLimpness, out float footLimpness, out float wingLimpness, out float hipLimpness, out float torsoLimpness);
            ResolveSynergyPreset(
                selectedInjuryPreset,
                out float impactVelocityInfluence,
                out float twoHandedBonus,
                out float heavyWeaponBonus,
                out float axeSlashBonus,
                out float traumaIncrease,
                out float traumaMaxBonus,
                out int permanentAfterCount,
                out float reactionSlow,
                    out float reactionDuration,
                    out float reactionLegCooldown,
                    out float reactionArmCooldown,
                    out float reactionOtherCooldown);
            ResolveDeathRecoveryKnockoutMultiplierPreset(selectedKnockoutPreset, out float deathRecoveryKnockoutDurationSeconds);
            ResolveDeathAnimationSpeedPreset(selectedAnimationSpeedPreset, out float deathAnimationSpeed);

            legsDuration = presetLegDuration;
            armsDuration = presetArmDuration;
            knockoutDuration = presetKnockoutDuration;
            knockoutTier2DurationMultiplier = Mathf.Max(knockoutDuration, knockoutTier2DurationMultiplier);
            float tier2ThresholdGap = ResolveTier2ThresholdGap(selectedInjuryPreset);
            float legsTier2Threshold = Mathf.Clamp(legsThreshold + tier2ThresholdGap, legsThreshold + 1f, 200f);
            float armsTier2Threshold = Mathf.Clamp(armsThreshold + tier2ThresholdGap, armsThreshold + 1f, 200f);
            legsTier2Threshold = Mathf.Round(legsTier2Threshold / 2f) * 2f;
            armsTier2Threshold = Mathf.Round(armsTier2Threshold / 2f) * 2f;

            bool changed = false;
            // Damage
            changed |= SetFloat(ref LegsThresholdDamage, legsThreshold, 1f, 200f);
            changed |= SetFloat(ref ArmsThresholdDamage, armsThreshold, 1f, 200f);
            changed |= SetFloat(ref LegsTier2ThresholdDamage, legsTier2Threshold, 1f, 200f);
            changed |= SetFloat(ref ArmsTier2ThresholdDamage, armsTier2Threshold, 1f, 200f);
            changed |= SetFloat(ref LegsDisableDurationSeconds, legsDuration, 1f, 120f);
            changed |= SetFloat(ref ArmsDisableDurationSeconds, armsDuration, 1f, 120f);

            // Injury slowness
            changed |= SetFloat(ref LegsSlowDebuffPercent, legsSlow, 0f, 100f);
            changed |= SetFloat(ref ArmsSlowDebuffPercent, armsSlow, 0f, 100f);
            changed |= SetFloat(ref LegsTier2SlowDebuffPercent, legsTier2Slow, 0f, 100f);
            changed |= SetFloat(ref ArmsTier2SlowDebuffPercent, armsTier2Slow, 0f, 100f);
            changed |= SetBool(ref LegsSlowStacks, legsSlowStacks);
            changed |= SetBool(ref ArmsSlowStacks, armsSlowStacks);
            changed |= SetFloat(ref LegsLimpnessPercent, legLimpness, 0f, 100f);
            changed |= SetFloat(ref ArmsLimpnessPercent, armLimpness, 0f, 100f);

            // Knockout
            changed |= SetFloat(ref KnockoutDurationSeconds, knockoutDuration, 1f, 60f);
            changed |= SetFloat(ref KnockoutRecoverySlowPercent, knockoutSlow, 0f, 100f);
            changed |= SetFloat(ref KnockoutTier1HitDamage, knockoutTier1HitDamage, 1f, 200f);
            changed |= SetFloat(ref KnockoutTier2HitDamage, knockoutTier2HitDamage, 1f, 200f);
            changed |= SetFloat(ref KnockoutTier2DurationMultiplier, knockoutTier2DurationMultiplier, 1.0f, 120f);
            changed |= SetFloat(ref DeathRecoveryKnockoutMultiplier, deathRecoveryKnockoutDurationSeconds, 1f, 180f);
            changed |= SetFloat(ref TorsoSlowDebuffPercent, Mathf.Max(50f, legsTier2Slow), 0f, 100f);

            // Last stand chance
            changed |= SetFloat(ref DeadRevivalChancePercent, deadChance, 0f, 100f);
            changed |= SetFloat(ref MaxDeadRecoveriesValue, maxRecoveries, 0f, 10f);
            if (forceLastStandEnabled.HasValue)
            {
                changed |= SetBool(ref LastStandEnabled, forceLastStandEnabled.Value);
            }

            // Recovery
            changed |= SetFloat(ref RecoveryDamageReductionPercent, recoveryReduction, 0f, 100f);
            changed |= SetBool(ref RecoveryRestoresPinForces, restorePins);
            changed |= SetFloat(ref DeadRecoverySlowMultiplier, deadSlow, 1.0f, 3.0f);
            changed |= SetBool(ref SlowDebuffsStack, slowDebuffsStack);
            changed |= SetFloat(ref HandsLimpnessPercent, handLimpness, 0f, 100f);
            changed |= SetFloat(ref FeetLimpnessPercent, footLimpness, 0f, 100f);
            changed |= SetFloat(ref WingsLimpnessPercent, wingLimpness, 0f, 100f);
            changed |= SetFloat(ref HipLimpnessPercent, hipLimpness, 0f, 100f);
            changed |= SetFloat(ref TorsoLimpnessPercent, torsoLimpness, 0f, 100f);
            changed |= SetBool(ref ProgressiveTraumaEnabled, true);
            changed |= SetFloat(ref TraumaDurationIncreasePercent, traumaIncrease, 0f, 100f);
            changed |= SetFloat(ref TraumaMaxDurationBonusPercent, traumaMaxBonus, 0f, 300f);
            changed |= SetFloat(ref PermanentDisableAfterCountValue, permanentAfterCount, 0f, 6f);
            changed |= SetBool(ref AiBehaviorReactions, true);
            changed |= SetBool(ref ForceDropOnArmDisable, true);
            changed |= SetFloat(ref InjuryReactionSlowPercent, reactionSlow, 0f, 100f);
            changed |= SetFloat(ref InjuryReactionDurationSeconds, reactionDuration, 1f, 30f);
            changed |= SetFloat(ref ReactionCooldownLegSeconds, reactionLegCooldown, 0f, 15f);
            changed |= SetFloat(ref ReactionCooldownArmSeconds, reactionArmCooldown, 0f, 15f);
            changed |= SetFloat(ref ReactionCooldownOtherSeconds, reactionOtherCooldown, 0f, 15f);

            changed |= SetFloat(ref DeathAnimationSpeedMultiplier, deathAnimationSpeed, 0.05f, 5f);

            return changed;
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  Public getters
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        public static float GetMinimumTrackedHitDamage() => HardcodedMinimumTrackedHitDamage;
        public static bool ShouldRefreshDisableTimerOnHit() => HardcodedHitsRefreshDisableTimer;
        public static bool UseLimbDisableSystem() => LimbDisableSystemEnabled;
        public static bool UseSlowDebuffSystem() => SlowDebuffSystemEnabled;
        public static bool UseWeightSystem() => WeightSystemEnabled;
        public static bool UseDeathAnimationSpeedSystem() => DeathAnimationSpeedSystemEnabled;
        public static bool UseMouthPresentationSystem() => MouthPresentationEnabled;

        public static float GetLimbThresholdDamage(RagdollPart.Type limbType)
        {
            return IsLegType(limbType)
                ? Mathf.Max(1f, LegsThresholdDamage)
                : Mathf.Max(1f, ArmsThresholdDamage);
        }

        public static float GetLimbDisableDurationSeconds(RagdollPart.Type limbType)
        {
            float selected = IsLegType(limbType)
                ? LegsDisableDurationSeconds
                : ArmsDisableDurationSeconds;

            float baseDuration = Mathf.Max(1f, selected);

            return baseDuration;
        }

        public static float GetLimbSlowDebuffRatio(RagdollPart.Type limbType)
        {
            return IsLegType(limbType)
                ? Mathf.Clamp01(LegsSlowDebuffPercent / 100f)
                : Mathf.Clamp01(ArmsSlowDebuffPercent / 100f);
        }

        public static float GetTier2LimbSlowDebuffRatio(RagdollPart.Type limbType)
        {
            return IsLegType(limbType)
                ? Mathf.Clamp01(LegsTier2SlowDebuffPercent / 100f)
                : Mathf.Clamp01(ArmsTier2SlowDebuffPercent / 100f);
        }

        public static float GetTier2ThresholdDamage(RagdollPart.Type limbType)
        {
            return IsLegType(limbType)
                ? Mathf.Max(1f, LegsTier2ThresholdDamage)
                : Mathf.Max(1f, ArmsTier2ThresholdDamage);
        }

        public static float GetTier2DurationMultiplier(RagdollPart.Type limbType)
        {
            float selected = IsLegType(limbType)
                ? LegsTier2DurationMultiplier
                : ArmsTier2DurationMultiplier;

            return Mathf.Clamp(selected, 1f, 600f);
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
            float multiplier = Mathf.Clamp(DeadRecoverySlowMultiplier, 1.0f, 3.0f);
            float speedFactor = 1f / multiplier;
            return Mathf.Clamp01(1f - speedFactor);
        }

        public static float GetKnockoutRecoverySlowDurationSeconds()
        {
            float duration = GetKnockoutDurationSeconds() * 0.75f;
            return Mathf.Clamp(duration, 4f, 35f);
        }

        public static float GetDeadRecoverySlowDurationSeconds()
        {
            float duration = GetKnockoutDurationSeconds() + 10f;
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

        public static float GetKnockoutDurationSeconds()
        {
            return Mathf.Clamp(KnockoutDurationSeconds, 1f, 60f);
        }

        public static float GetKnockoutTier1HitDamage()
        {
            return Mathf.Max(1f, KnockoutTier1HitDamage);
        }

        public static float GetKnockoutTier2HitDamage()
        {
            return Mathf.Max(GetKnockoutTier1HitDamage(), KnockoutTier2HitDamage);
        }

        public static float GetKnockoutTier2DurationMultiplier()
        {
            return Mathf.Clamp(KnockoutTier2DurationMultiplier, 1.0f, 120f);
        }

        public static bool IsHeadNeckKnockoutEnabled() => HeadNeckKnockout;
        public static bool ShouldLogTorsoPlaceholderHits() => TorsoPlaceholderLogs;
        public static bool ShouldDisableHands() => HandImmobilization;
        public static bool ShouldDisableFeet() => FootImmobilization;
        public static bool ShouldDisableWings() => WingImmobilization;
        public static bool ShouldDisableTail() => TailImmobilization;
        public static float GetWeaponVelocityCurveExponent() => 1.0f;
        public static bool UseProgressiveTrauma() => ProgressiveTraumaEnabled;
        public static float GetTraumaDurationIncreaseRatioPerDisable() => Mathf.Clamp01(TraumaDurationIncreasePercent / 100f);
        public static float GetTraumaMaxDurationBonusRatio() => Mathf.Clamp(TraumaMaxDurationBonusPercent, 0f, 1000f) / 100f;
        public static int GetPermanentDisableAfterCount() => Mathf.Clamp(Mathf.RoundToInt(PermanentDisableAfterCountValue), 0, 6);
        public static bool UseAiBehaviorReactions() => AiBehaviorReactions;
        public static bool ShouldForceDropOnArmDisable() => ForceDropOnArmDisable;
        public static float GetInjuryReactionSlowRatio() => Mathf.Clamp01(InjuryReactionSlowPercent / 100f);
        public static float GetInjuryReactionDurationSeconds() => Mathf.Clamp(InjuryReactionDurationSeconds, 1f, 30f);
        public static float GetReactionCooldownSecondsForPart(RagdollPart.Type limbType)
        {
            if (IsLegType(limbType))
            {
                return Mathf.Clamp(ReactionCooldownLegSeconds, 0f, 15f);
            }

            if (IsArmType(limbType))
            {
                return Mathf.Clamp(ReactionCooldownArmSeconds, 0f, 15f);
            }

            return Mathf.Clamp(ReactionCooldownOtherSeconds, 0f, 15f);
        }
        public static bool UsePerformanceSafeguards() => PerformanceSafeguardsEnabled;
        public static int GetMaxCreaturesPerUpdate() => Mathf.Clamp(Mathf.RoundToInt(MaxCreaturesPerUpdateValue), 10, 400);

        public static float GetTorsoSlowDebuffRatio()
        {
            return Mathf.Clamp01(TorsoSlowDebuffPercent / 100f);
        }

        public static float GetLimpnessPercentForPart(RagdollPart.Type limbType)
        {
            switch (limbType)
            {
                case RagdollPart.Type.LeftLeg:
                case RagdollPart.Type.RightLeg:
                    return Mathf.Clamp(LegsLimpnessPercent, 0f, 100f);
                case RagdollPart.Type.LeftFoot:
                case RagdollPart.Type.RightFoot:
                    return Mathf.Clamp(FeetLimpnessPercent, 0f, 100f);
                case RagdollPart.Type.LeftArm:
                case RagdollPart.Type.RightArm:
                    return Mathf.Clamp(ArmsLimpnessPercent, 0f, 100f);
                case RagdollPart.Type.LeftHand:
                case RagdollPart.Type.RightHand:
                    return Mathf.Clamp(HandsLimpnessPercent, 0f, 100f);
                case RagdollPart.Type.LeftWing:
                case RagdollPart.Type.RightWing:
                    return Mathf.Clamp(WingsLimpnessPercent, 0f, 100f);
                case RagdollPart.Type.Tail:
                    return Mathf.Clamp(HipLimpnessPercent, 0f, 100f);
                case RagdollPart.Type.Torso:
                    return Mathf.Clamp(TorsoLimpnessPercent, 0f, 100f);
                default:
                    return Mathf.Clamp(ArmsLimpnessPercent, 0f, 100f);
            }
        }

        public static float GetDeadRevivalSpeedMultiplier()
        {
            return Mathf.Clamp(1f - GetDeadRecoverySlowDebuffRatio(), 0f, 1f);
        }

        public static float GetReviveStandupFailChanceRatio()
        {
            return Mathf.Clamp01(ReviveStandupFailChancePercent / 100f);
        }

        /// <summary>Returns the weight multiplier to apply to all NPC rigidbody masses (e.g. 1.5 for +50%).</summary>
        public static float GetWeightMultiplier()
        {
            if (!WeightSystemEnabled)
            {
                return 1f;
            }

            return 1f + Mathf.Clamp(WeightBonusPercent, 0f, 500f) / 100f;
        }

        public static float GetDeathRecoveryKnockoutDuration()
        {
            if (!LastStandEnabled)
            {
                return 0f;
            }

            return Mathf.Clamp(DeathRecoveryKnockoutMultiplier, 1f, 180f);
        }

        /// <summary>Returns the death animation speed multiplier (lower = slower).</summary>
        public static float GetDeathAnimationSpeedMultiplier()
        {
            if (!DeathAnimationSpeedSystemEnabled)
            {
                return 1f;
            }

            return Mathf.Clamp(DeathAnimationSpeedMultiplier, 0.05f, 5f);
        }

        public static float GetMockDeathRecoveredEyeOpenRatio()
        {
            return Mathf.Clamp01(RecoveredMockDeathEyeOpenPercent / 100f);
        }

        public static float GetMockDeathAnimationSpeedMultiplier()
        {
            return Mathf.Clamp(MockDeathAnimSpeedMultiplier, 0.05f, 2f);
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

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  Hashes & snapshot
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        public static int GetPresetSelectionHash()
        {
            int hash = 17;
            hash = CombineHash(hash, StringHash(NormalizeLastStandChancePreset(PresetModel)));
            hash = CombineHash(hash, StringHash(NormalizeDamagePreset(PresetInjuryModel)));
            hash = CombineHash(hash, StringHash(NormalizeInjurySlowPreset(PresetLimbSlowModel)));
            hash = CombineHash(hash, StringHash(NormalizeKnockoutPreset(PresetKnockoutModel)));
            hash = CombineHash(hash, StringHash(NormalizeDeathAnimationSpeedPreset(PresetDeathAnimationSpeedModel)));
            hash = CombineHash(hash, StringHash(NormalizeDurationPreset(PresetDurationModel)));
            hash = CombineHash(hash, StringHash(NormalizeLimpnessPreset(PresetLimpnessModel)));
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
            hash = CombineHash(hash, PercentHash(snapshot.LegsTier2ThresholdDamage));
            hash = CombineHash(hash, PercentHash(snapshot.LegsTier2DurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.LegsTier2SlowDebuffPercent));
            hash = CombineHash(hash, PercentHash(snapshot.LegsLimpnessPercent));

            hash = CombineHash(hash, PercentHash(snapshot.ArmsThresholdDamage));
            hash = CombineHash(hash, PercentHash(snapshot.ArmsDisableDurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.ArmsSlowDebuffPercent));
            hash = CombineHash(hash, snapshot.ArmsSlowStacks ? 1 : 0);
            hash = CombineHash(hash, PercentHash(snapshot.ArmsTier2ThresholdDamage));
            hash = CombineHash(hash, PercentHash(snapshot.ArmsTier2DurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.ArmsTier2SlowDebuffPercent));
            hash = CombineHash(hash, PercentHash(snapshot.ArmsLimpnessPercent));

            hash = CombineHash(hash, snapshot.LastStandEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.LimbDisableSystemEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.SlowDebuffSystemEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.WeightSystemEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.DeathAnimationSpeedSystemEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.MouthPresentationEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.FallFromLegInjury ? 1 : 0);
            hash = CombineHash(hash, snapshot.LegImmobilization ? 1 : 0);
            hash = CombineHash(hash, snapshot.ArmImmobilization ? 1 : 0);
            hash = CombineHash(hash, snapshot.HandImmobilization ? 1 : 0);
            hash = CombineHash(hash, snapshot.FootImmobilization ? 1 : 0);
            hash = CombineHash(hash, snapshot.WingImmobilization ? 1 : 0);
            hash = CombineHash(hash, snapshot.TailImmobilization ? 1 : 0);
            hash = CombineHash(hash, snapshot.HeadNeckKnockout ? 1 : 0);
            hash = CombineHash(hash, snapshot.TorsoPlaceholderLogs ? 1 : 0);
            hash = CombineHash(hash, PercentHash(snapshot.HandsLimpnessPercent));
            hash = CombineHash(hash, PercentHash(snapshot.FeetLimpnessPercent));
            hash = CombineHash(hash, PercentHash(snapshot.WingsLimpnessPercent));
            hash = CombineHash(hash, PercentHash(snapshot.HipLimpnessPercent));
            hash = CombineHash(hash, PercentHash(snapshot.TorsoLimpnessPercent));
            hash = CombineHash(hash, PercentHash(snapshot.WeaponVelocityCurveExponentPercent));
            hash = CombineHash(hash, snapshot.ProgressiveTraumaEnabled ? 1 : 0);
            hash = CombineHash(hash, PercentHash(snapshot.TraumaDurationIncreasePercent));
            hash = CombineHash(hash, PercentHash(snapshot.TraumaMaxDurationBonusPercent));
            hash = CombineHash(hash, snapshot.PermanentDisableAfterCount);
            hash = CombineHash(hash, snapshot.AiBehaviorReactions ? 1 : 0);
            hash = CombineHash(hash, snapshot.ForceDropOnArmDisable ? 1 : 0);
            hash = CombineHash(hash, PercentHash(snapshot.InjuryReactionSlowPercent));
            hash = CombineHash(hash, PercentHash(snapshot.InjuryReactionDurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.ReactionCooldownLegSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.ReactionCooldownArmSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.ReactionCooldownOtherSeconds));
            hash = CombineHash(hash, snapshot.PerformanceSafeguardsEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.MaxCreaturesPerUpdate);
            hash = CombineHash(hash, snapshot.KnockoutEnabled ? 1 : 0);
            hash = CombineHash(hash, snapshot.EnableBasicLogging ? 1 : 0);
            hash = CombineHash(hash, snapshot.EnableDiagnosticsLogging ? 1 : 0);
            hash = CombineHash(hash, snapshot.EnableVerboseLogging ? 1 : 0);

            hash = CombineHash(hash, PercentHash(snapshot.RecoveryDamageReductionPercent));
            hash = CombineHash(hash, snapshot.RecoveryRestoresPinForces ? 1 : 0);
            hash = CombineHash(hash, PercentHash(snapshot.DeadRevivalChancePercent));
            hash = CombineHash(hash, snapshot.MaxDeadRecoveries);
            hash = CombineHash(hash, PercentHash(snapshot.DeadRecoverySlowPercent));
            hash = CombineHash(hash, PercentHash(snapshot.ReviveStandupFailChancePercent));
            hash = CombineHash(hash, PercentHash(snapshot.KnockoutDurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.KnockoutRecoverySlowPercent));
            hash = CombineHash(hash, PercentHash(snapshot.KnockoutTier1HitDamage));
            hash = CombineHash(hash, PercentHash(snapshot.KnockoutTier2HitDamage));
            hash = CombineHash(hash, PercentHash(snapshot.KnockoutTier2DurationMultiplier));
            hash = CombineHash(hash, PercentHash(snapshot.DeathRecoveryKnockoutDurationSeconds));
            hash = CombineHash(hash, PercentHash(snapshot.TorsoSlowDebuffPercent));
            hash = CombineHash(hash, snapshot.SlowDebuffsStack ? 1 : 0);
            hash = CombineHash(hash, PercentHash(snapshot.WeightBonusPercent));
            hash = CombineHash(hash, PercentHash(snapshot.DeathAnimationSpeedMultiplier));
            hash = CombineHash(hash, PercentHash(snapshot.RecoveredMockDeathEyeOpenPercent));
            hash = CombineHash(hash, PercentHash(snapshot.MockDeathAnimationSpeedMultiplier));

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
                LegsTier2ThresholdDamage = LegsTier2ThresholdDamage,
                LegsTier2DurationSeconds = LegsTier2DurationMultiplier,
                LegsTier2SlowDebuffPercent = LegsTier2SlowDebuffPercent,
                LegsLimpnessPercent = LegsLimpnessPercent,
                ArmsThresholdDamage = ArmsThresholdDamage,
                ArmsDisableDurationSeconds = ArmsDisableDurationSeconds,
                ArmsSlowDebuffPercent = ArmsSlowDebuffPercent,
                ArmsSlowStacks = ArmsSlowStacks,
                ArmsTier2ThresholdDamage = ArmsTier2ThresholdDamage,
                ArmsTier2DurationSeconds = ArmsTier2DurationMultiplier,
                ArmsTier2SlowDebuffPercent = ArmsTier2SlowDebuffPercent,
                ArmsLimpnessPercent = ArmsLimpnessPercent,
                LastStandEnabled = LastStandEnabled,
                LimbDisableSystemEnabled = LimbDisableSystemEnabled,
                SlowDebuffSystemEnabled = SlowDebuffSystemEnabled,
                WeightSystemEnabled = WeightSystemEnabled,
                DeathAnimationSpeedSystemEnabled = DeathAnimationSpeedSystemEnabled,
                MouthPresentationEnabled = MouthPresentationEnabled,
                FallFromLegInjury = FallFromLegInjury,
                LegImmobilization = LegImmobilization,
                ArmImmobilization = ArmImmobilization,
                HandImmobilization = HandImmobilization,
                FootImmobilization = FootImmobilization,
                WingImmobilization = WingImmobilization,
                TailImmobilization = TailImmobilization,
                HeadNeckKnockout = HeadNeckKnockout,
                TorsoPlaceholderLogs = TorsoPlaceholderLogs,
                HandsLimpnessPercent = HandsLimpnessPercent,
                FeetLimpnessPercent = FeetLimpnessPercent,
                WingsLimpnessPercent = WingsLimpnessPercent,
                HipLimpnessPercent = HipLimpnessPercent,
                TorsoLimpnessPercent = TorsoLimpnessPercent,
                WeaponVelocityCurveExponentPercent = WeaponVelocityCurveExponentPercent,
                ProgressiveTraumaEnabled = ProgressiveTraumaEnabled,
                TraumaDurationIncreasePercent = TraumaDurationIncreasePercent,
                TraumaMaxDurationBonusPercent = TraumaMaxDurationBonusPercent,
                PermanentDisableAfterCount = GetPermanentDisableAfterCount(),
                AiBehaviorReactions = AiBehaviorReactions,
                ForceDropOnArmDisable = ForceDropOnArmDisable,
                InjuryReactionSlowPercent = InjuryReactionSlowPercent,
                InjuryReactionDurationSeconds = InjuryReactionDurationSeconds,
                ReactionCooldownLegSeconds = ReactionCooldownLegSeconds,
                ReactionCooldownArmSeconds = ReactionCooldownArmSeconds,
                ReactionCooldownOtherSeconds = ReactionCooldownOtherSeconds,
                PerformanceSafeguardsEnabled = PerformanceSafeguardsEnabled,
                MaxCreaturesPerUpdate = GetMaxCreaturesPerUpdate(),
                KnockoutEnabled = KnockoutEnabled,
                EnableBasicLogging = EnableBasicLogging,
                EnableDiagnosticsLogging = EnableDiagnosticsLogging,
                EnableVerboseLogging = EnableVerboseLogging,
                RecoveryDamageReductionPercent = RecoveryDamageReductionPercent,
                RecoveryRestoresPinForces = RecoveryRestoresPinForces,
                DeadRevivalChancePercent = DeadRevivalChancePercent,
                MaxDeadRecoveries = GetMaxDeadRecoveries(),
                DeadRecoverySlowPercent = DeadRecoverySlowPercent,
                ReviveStandupFailChancePercent = ReviveStandupFailChancePercent,
                KnockoutDurationSeconds = KnockoutDurationSeconds,
                KnockoutRecoverySlowPercent = KnockoutRecoverySlowPercent,
                KnockoutTier1HitDamage = KnockoutTier1HitDamage,
                KnockoutTier2HitDamage = KnockoutTier2HitDamage,
                KnockoutTier2DurationMultiplier = KnockoutTier2DurationMultiplier,
                DeathRecoveryKnockoutDurationSeconds = DeathRecoveryKnockoutMultiplier,
                TorsoSlowDebuffPercent = TorsoSlowDebuffPercent,
                SlowDebuffsStack = SlowDebuffsStack,
                WeightBonusPercent = WeightBonusPercent,
                DeathAnimationSpeedMultiplier = DeathAnimationSpeedMultiplier,
                RecoveredMockDeathEyeOpenPercent = RecoveredMockDeathEyeOpenPercent,
                MockDeathAnimationSpeedMultiplier = MockDeathAnimSpeedMultiplier,
            };
        }

        public static bool SourceSnapshotsEqual(SourceSnapshot left, SourceSnapshot right)
        {
            return Mathf.Abs(left.LegsThresholdDamage - right.LegsThresholdDamage) < 0.0001f &&
                   Mathf.Abs(left.LegsDisableDurationSeconds - right.LegsDisableDurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.LegsSlowDebuffPercent - right.LegsSlowDebuffPercent) < 0.0001f &&
                   left.LegsSlowStacks == right.LegsSlowStacks &&
                   Mathf.Abs(left.LegsTier2ThresholdDamage - right.LegsTier2ThresholdDamage) < 0.0001f &&
                   Mathf.Abs(left.LegsTier2DurationSeconds - right.LegsTier2DurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.LegsTier2SlowDebuffPercent - right.LegsTier2SlowDebuffPercent) < 0.0001f &&
                   Mathf.Abs(left.LegsLimpnessPercent - right.LegsLimpnessPercent) < 0.0001f &&
                   Mathf.Abs(left.ArmsThresholdDamage - right.ArmsThresholdDamage) < 0.0001f &&
                   Mathf.Abs(left.ArmsDisableDurationSeconds - right.ArmsDisableDurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.ArmsSlowDebuffPercent - right.ArmsSlowDebuffPercent) < 0.0001f &&
                   left.ArmsSlowStacks == right.ArmsSlowStacks &&
                   Mathf.Abs(left.ArmsTier2ThresholdDamage - right.ArmsTier2ThresholdDamage) < 0.0001f &&
                   Mathf.Abs(left.ArmsTier2DurationSeconds - right.ArmsTier2DurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.ArmsTier2SlowDebuffPercent - right.ArmsTier2SlowDebuffPercent) < 0.0001f &&
                   Mathf.Abs(left.ArmsLimpnessPercent - right.ArmsLimpnessPercent) < 0.0001f &&
                   left.LastStandEnabled == right.LastStandEnabled &&
                   left.LimbDisableSystemEnabled == right.LimbDisableSystemEnabled &&
                   left.SlowDebuffSystemEnabled == right.SlowDebuffSystemEnabled &&
                   left.WeightSystemEnabled == right.WeightSystemEnabled &&
                   left.DeathAnimationSpeedSystemEnabled == right.DeathAnimationSpeedSystemEnabled &&
                   left.MouthPresentationEnabled == right.MouthPresentationEnabled &&
                   left.FallFromLegInjury == right.FallFromLegInjury &&
                   left.LegImmobilization == right.LegImmobilization &&
                   left.ArmImmobilization == right.ArmImmobilization &&
                   left.HandImmobilization == right.HandImmobilization &&
                   left.FootImmobilization == right.FootImmobilization &&
                   left.WingImmobilization == right.WingImmobilization &&
                   left.TailImmobilization == right.TailImmobilization &&
                   left.HeadNeckKnockout == right.HeadNeckKnockout &&
                   left.TorsoPlaceholderLogs == right.TorsoPlaceholderLogs &&
                   Mathf.Abs(left.HandsLimpnessPercent - right.HandsLimpnessPercent) < 0.0001f &&
                   Mathf.Abs(left.FeetLimpnessPercent - right.FeetLimpnessPercent) < 0.0001f &&
                   Mathf.Abs(left.WingsLimpnessPercent - right.WingsLimpnessPercent) < 0.0001f &&
                   Mathf.Abs(left.HipLimpnessPercent - right.HipLimpnessPercent) < 0.0001f &&
                   Mathf.Abs(left.TorsoLimpnessPercent - right.TorsoLimpnessPercent) < 0.0001f &&
                   Mathf.Abs(left.WeaponVelocityCurveExponentPercent - right.WeaponVelocityCurveExponentPercent) < 0.0001f &&
                   left.ProgressiveTraumaEnabled == right.ProgressiveTraumaEnabled &&
                   Mathf.Abs(left.TraumaDurationIncreasePercent - right.TraumaDurationIncreasePercent) < 0.0001f &&
                   Mathf.Abs(left.TraumaMaxDurationBonusPercent - right.TraumaMaxDurationBonusPercent) < 0.0001f &&
                   left.PermanentDisableAfterCount == right.PermanentDisableAfterCount &&
                   left.AiBehaviorReactions == right.AiBehaviorReactions &&
                   left.ForceDropOnArmDisable == right.ForceDropOnArmDisable &&
                   Mathf.Abs(left.InjuryReactionSlowPercent - right.InjuryReactionSlowPercent) < 0.0001f &&
                   Mathf.Abs(left.InjuryReactionDurationSeconds - right.InjuryReactionDurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.ReactionCooldownLegSeconds - right.ReactionCooldownLegSeconds) < 0.0001f &&
                   Mathf.Abs(left.ReactionCooldownArmSeconds - right.ReactionCooldownArmSeconds) < 0.0001f &&
                   Mathf.Abs(left.ReactionCooldownOtherSeconds - right.ReactionCooldownOtherSeconds) < 0.0001f &&
                   left.PerformanceSafeguardsEnabled == right.PerformanceSafeguardsEnabled &&
                   left.MaxCreaturesPerUpdate == right.MaxCreaturesPerUpdate &&
                   left.KnockoutEnabled == right.KnockoutEnabled &&
                   left.EnableBasicLogging == right.EnableBasicLogging &&
                   left.EnableDiagnosticsLogging == right.EnableDiagnosticsLogging &&
                   left.EnableVerboseLogging == right.EnableVerboseLogging &&
                   Mathf.Abs(left.RecoveryDamageReductionPercent - right.RecoveryDamageReductionPercent) < 0.0001f &&
                   left.RecoveryRestoresPinForces == right.RecoveryRestoresPinForces &&
                   Mathf.Abs(left.DeadRevivalChancePercent - right.DeadRevivalChancePercent) < 0.0001f &&
                   left.MaxDeadRecoveries == right.MaxDeadRecoveries &&
                   Mathf.Abs(left.DeadRecoverySlowPercent - right.DeadRecoverySlowPercent) < 0.0001f &&
                   Mathf.Abs(left.ReviveStandupFailChancePercent - right.ReviveStandupFailChancePercent) < 0.0001f &&
                   Mathf.Abs(left.KnockoutDurationSeconds - right.KnockoutDurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.KnockoutRecoverySlowPercent - right.KnockoutRecoverySlowPercent) < 0.0001f &&
                   Mathf.Abs(left.KnockoutTier1HitDamage - right.KnockoutTier1HitDamage) < 0.0001f &&
                   Mathf.Abs(left.KnockoutTier2HitDamage - right.KnockoutTier2HitDamage) < 0.0001f &&
                   Mathf.Abs(left.KnockoutTier2DurationMultiplier - right.KnockoutTier2DurationMultiplier) < 0.0001f &&
                   Mathf.Abs(left.DeathRecoveryKnockoutDurationSeconds - right.DeathRecoveryKnockoutDurationSeconds) < 0.0001f &&
                   Mathf.Abs(left.TorsoSlowDebuffPercent - right.TorsoSlowDebuffPercent) < 0.0001f &&
                   left.SlowDebuffsStack == right.SlowDebuffsStack &&
                   Mathf.Abs(left.WeightBonusPercent - right.WeightBonusPercent) < 0.0001f &&
                   Mathf.Abs(left.DeathAnimationSpeedMultiplier - right.DeathAnimationSpeedMultiplier) < 0.0001f &&
                   Mathf.Abs(left.RecoveredMockDeathEyeOpenPercent - right.RecoveredMockDeathEyeOpenPercent) < 0.0001f &&
                   Mathf.Abs(left.MockDeathAnimationSpeedMultiplier - right.MockDeathAnimationSpeedMultiplier) < 0.0001f;
        }

        public static string GetSourceOfTruthSummary()
        {
            return "legs=" + LegsThresholdDamage.ToString("0") + "/" + LegsDisableDurationSeconds.ToString("0") + "s t2=" + LegsTier2ThresholdDamage.ToString("0") + "/" + LegsTier2DurationMultiplier.ToString("0") + "s slow=" + LegsSlowDebuffPercent.ToString("0") + "% t2Slow=" + LegsTier2SlowDebuffPercent.ToString("0") + "% stack=" + LegsSlowStacks +
                   " arms=" + ArmsThresholdDamage.ToString("0") + "/" + ArmsDisableDurationSeconds.ToString("0") + "s t2=" + ArmsTier2ThresholdDamage.ToString("0") + "/" + ArmsTier2DurationMultiplier.ToString("0") + "s slow=" + ArmsSlowDebuffPercent.ToString("0") + "% t2Slow=" + ArmsTier2SlowDebuffPercent.ToString("0") + "% stack=" + ArmsSlowStacks +
                   " systems(limb/slow/weight/deathAnim/mouth)=" + LimbDisableSystemEnabled + "/" + SlowDebuffSystemEnabled + "/" + WeightSystemEnabled + "/" + DeathAnimationSpeedSystemEnabled + "/" + MouthPresentationEnabled +
                   " lastStand=" + LastStandEnabled +
                   " recover=" + RecoveryDamageReductionPercent.ToString("0") + "%" +
                   " deadChance=" + DeadRevivalChancePercent.ToString("0") + "%" +
                   " maxRecoveries=" + GetMaxDeadRecoveries() +
                   " deadSlow=" + DeadRecoverySlowMultiplier.ToString("0.0") + "x" +
                   " standFailChance=" + ReviveStandupFailChancePercent.ToString("0") + "%" +
                   " koDur=" + KnockoutDurationSeconds.ToString("0") + "s" +
                   " koSlow=" + KnockoutRecoverySlowPercent.ToString("0") + "%" +
                   " koHit=" + KnockoutTier1HitDamage.ToString("0") + "/" + KnockoutTier2HitDamage.ToString("0") +
                   " koT2Dur=" + KnockoutTier2DurationMultiplier.ToString("0.0") + "s" +
                   " deathKoDur=" + DeathRecoveryKnockoutMultiplier.ToString("0.0") + "s" +
                   " torsoSlow=" + TorsoSlowDebuffPercent.ToString("0") + "%" +
                   " limp(leg/arm/hip)=" + LegsLimpnessPercent.ToString("0") + "/" + ArmsLimpnessPercent.ToString("0") + "/" + HipLimpnessPercent.ToString("0") + "%" +
                   " trauma(enabled/per/inc/max)=" + ProgressiveTraumaEnabled + "/" + GetPermanentDisableAfterCount() + "/" + TraumaDurationIncreasePercent.ToString("0") + "/" + TraumaMaxDurationBonusPercent.ToString("0") + "%" +
                   " react(enabled/drop/slow/dur/cooldowns)=" + AiBehaviorReactions + "/" + ForceDropOnArmDisable + "/" + InjuryReactionSlowPercent.ToString("0") + "%/" + InjuryReactionDurationSeconds.ToString("0.0") + "s/" + ReactionCooldownLegSeconds.ToString("0.0") + "/" + ReactionCooldownArmSeconds.ToString("0.0") + "/" + ReactionCooldownOtherSeconds.ToString("0.0") +
                   " perf(enabled/maxC)=" + PerformanceSafeguardsEnabled + "/" + GetMaxCreaturesPerUpdate() +
                   " logs(basic/diag/verbose)=" + EnableBasicLogging + "/" + EnableDiagnosticsLogging + "/" + EnableVerboseLogging +
                   " slowStack=" + SlowDebuffsStack +
                   " weight=+" + WeightBonusPercent.ToString("0") + "%" +
                   " deathAnimSpeed=" + DeathAnimationSpeedMultiplier.ToString("0.00") + "x" +
                   " mockDeathAnimSpeed=" + MockDeathAnimSpeedMultiplier.ToString("0.00") + "x" +
                   " recoveredEyeOpen=" + RecoveredMockDeathEyeOpenPercent.ToString("0") + "%";
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  Preset normalization
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        public static string NormalizePresetModel(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("OFF") || token.Contains("DISABLE")) return PresetScaleVeryLow;

            if (token.Contains("VERYLOW") ||
                token.Contains("LENIENT") ||
                token.Contains("MINIMAL") ||
                token.Contains("BRIEF") ||
                token.Contains("RARE") ||
                token.Contains("WEAK") ||
                token.Contains("LIGHT") ||
                token.Contains("20") ||
                token.Contains("025") ||
                token.Contains("010")) return PresetScaleVeryLow;

            if (token == "LOW" ||
                token.Contains("MILD") ||
                token.Contains("SHORT") ||
                token.Contains("UNLIKELY") ||
                token.Contains("FRAIL") ||
                token.Contains("MODERATE") ||
                token.Contains("30") ||
                token.Contains("050")) return PresetScaleLow;

            if (token.Contains("VERYHIGH") ||
                token.Contains("BRUTAL") ||
                token.Contains("CRIPPLING") ||
                token.Contains("EXTENDED") ||
                token.Contains("FREQUENT") ||
                token.Contains("ALWAYS") ||
                token.Contains("RESILIENT") ||
                token.Contains("MASSIVE") ||
                token.Contains("100") ||
                token.Contains("200")) return PresetScaleVeryHigh;

            if (token == "HIGH" ||
                token.Contains("SEVERE") ||
                token.Contains("HEAVY") ||
                token.Contains("LONG") ||
                token.Contains("LIKELY") ||
                token.Contains("STRONG") ||
                token.Contains("70") ||
                token.Contains("125") ||
                token.Contains("150")) return PresetScaleHigh;

            return PresetScaleDefault;
        }

        public static string NormalizeDurationPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYSHORT") || token.Contains("VERYLOW")) return PresetDurationVeryShort;
            if (token.Contains("SHORT") || token == "LOW") return PresetDurationShort;
            if (token.Contains("VERYLONG") || token.Contains("VERYHIGH")) return PresetDurationVeryLong;
            if (token.Contains("LONG") || token == "HIGH") return PresetDurationLong;
            return PresetDurationDefault;
        }

        public static string NormalizeLimpnessPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYLIGHT") || token.Contains("VERYLOW")) return PresetLimpnessVeryLight;
            if (token.Contains("LIGHT") || token == "LOW") return PresetLimpnessLight;
            if (token.Contains("VERYHEAVY") || token.Contains("VERYHIGH")) return PresetLimpnessVeryHeavy;
            if (token.Contains("HEAVY") || token == "HIGH") return PresetLimpnessHeavy;
            return PresetLimpnessDefault;
        }

        public static string NormalizePerformanceProfilePreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYSAFE") || token.Contains("VERYLIGHT") || token.Contains("VERYLOW")) return PresetPerformanceVerySafe;
            if (token == "SAFE" || token == "LOW" || token.Contains("LIGHT")) return PresetPerformanceSafe;
            if (token.Contains("VERYFAST") || token.Contains("VERYHIGH") || token.Contains("MAX")) return PresetPerformanceVeryFast;
            if (token == "FAST" || token == "HIGH") return PresetPerformanceFast;
            return PresetPerformanceDefault;
        }

        public static string NormalizeLoggingProfilePreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("MINIMAL") || token.Contains("OFF") || token.Contains("VERYLOW")) return PresetLoggingMinimal;
            if (token.Contains("BASIC") || token == "LOW") return PresetLoggingBasic;
            if (token.Contains("VERBOSE") || token.Contains("VERYHIGH")) return PresetLoggingVerbose;
            if (token.Contains("DIAG") || token == "HIGH" || token.Contains("DEBUG")) return PresetLoggingDiagnostic;
            return PresetLoggingDefault;
        }

        public static string NormalizeWeaponCurvePreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYFLAT") || token.Contains("VERYLOW")) return PresetWeaponCurveVeryFlat;
            if (token.Contains("FLAT") || token == "LOW") return PresetWeaponCurveFlat;
            if (token.Contains("VERYSTEEP") || token.Contains("VERYHIGH")) return PresetWeaponCurveVerySteep;
            if (token.Contains("STEEP") || token == "HIGH") return PresetWeaponCurveSteep;
            return PresetWeaponCurveDefault;
        }

        public static string NormalizeDamagePreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYLOW") || token.Contains("LENIENT") || token.Contains("LIGHT")) return PresetDamageLenient;
            if (token == "LOW" || token.Contains("MILD") || token.Contains("EASY")) return PresetDamageMild;
            if (token.Contains("VERYHIGH") || token.Contains("BRUTAL") || token.Contains("HARD") || token.Contains("EXTREME")) return PresetDamageBrutal;
            if (token == "HIGH" || token.Contains("SEVERE") || token.Contains("TACTIC")) return PresetDamageSevere;
            return PresetDamageDefault;
        }

        public static string NormalizeInjurySlowPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYLOW") || token.Contains("MINIMAL") || token.Contains("NONE")) return PresetInjurySlowMinimal;
            if (token == "LOW" || token.Contains("LIGHT")) return PresetInjurySlowLight;
            if (token.Contains("VERYHIGH") || token.Contains("CRIPPLING") || token.Contains("MAX") || token.Contains("EXTREME")) return PresetInjurySlowCrippling;
            if (token == "HIGH" || token.Contains("HEAVY")) return PresetInjurySlowHeavy;
            return PresetInjurySlowDefault;
        }

        // Legacy alias
        public static string NormalizeLimbSlowPreset(string preset) => NormalizeInjurySlowPreset(preset);

        public static string NormalizeKnockoutPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYLOW") || token.Contains("BRIEF") || token.Contains("TINY")) return PresetKnockoutBrief;
            if (token == "LOW" || token.Contains("SHORT")) return PresetKnockoutShort;
            if (token.Contains("VERYHIGH") || token.Contains("EXTENDED") || token.Contains("MAX")) return PresetKnockoutExtended;
            if (token == "HIGH" || token.Contains("LONG")) return PresetKnockoutLong;
            return PresetKnockoutDefault;
        }

        public static string NormalizeLastStandChancePreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("OFF") || token.Contains("DISABLE")) return PresetLastStandOffLegacy;
            if (token.Contains("ALWAY")) return PresetLastStandFrequent;
            if (token.Contains("VERYLOW") || token.Contains("RARE") || token.Contains("NEVER")) return PresetLastStandRare;
            if (token == "LOW" || token.Contains("UNLIKELY") || token.Contains("LESS")) return PresetLastStandUnlikely;
            if (token.Contains("VERYHIGH")) return PresetLastStandFrequent;
            if (token.Contains("FREQUENT") || token == "HIGH" || token.Contains("LIKELY") || token.Contains("MORE") || token.Contains("AGGRESSIVE")) return PresetLastStandLikely;
            return PresetLastStandDefault;
        }

        // Legacy alias
        public static string NormalizeLastStandPreset(string preset) => NormalizeLastStandChancePreset(preset);

        public static string NormalizeRecoveryPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYLOW") || token.Contains("MINIMAL") || token.Contains("WEAK") || token.Contains("FRAGILE")) return PresetRecoveryWeak;
            if (token == "LOW" || token.Contains("FRAIL") || token.Contains("LIGHT")) return PresetRecoveryFrail;
            if (token.Contains("VERYHIGH") || token.Contains("RESILIENT") || token.Contains("EXTREME") || token.Contains("MAX")) return PresetRecoveryResilient;
            if (token == "HIGH" || token.Contains("STRONG") || token.Contains("HEAVY")) return PresetRecoveryStrong;
            return PresetRecoveryDefault;
        }

        // Legacy alias
        public static string NormalizeLastStandSlowPreset(string preset) => NormalizeRecoveryPreset(preset);

        public static string NormalizeWeightPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYLOW") || token.Contains("LIGHT") || token.Contains("20")) return PresetWeightLight;
            if (token == "LOW" || token.Contains("MODERATE") || token.Contains("30")) return PresetWeightModerate;
            if (token.Contains("VERYHIGH") || token.Contains("MASSIVE") || token.Contains("100")) return PresetWeightMassive;
            if (token == "HIGH" || token.Contains("HEAVY") || token.Contains("70")) return PresetWeightHeavy;
            return PresetWeightDefault;
        }

        public static string NormalizeDeathAnimationSpeedPreset(string preset)
        {
            string token = NormalizeToken(preset);
            if (token.Contains("VERYLOW") || token.Contains("SLOWEST") || token.Contains("025") || token.Contains("010")) return PresetDeathAnimationSpeedVeryLow;
            if (token == "LOW" || token.Contains("SLOW") || token.Contains("050")) return PresetDeathAnimationSpeedLow;
            if (token.Contains("VERYHIGH") || token.Contains("FASTEST") || token.Contains("200")) return PresetDeathAnimationSpeedVeryHigh;
            if (token == "HIGH" || token.Contains("FAST") || token.Contains("125") || token.Contains("150")) return PresetDeathAnimationSpeedHigh;
            return PresetDeathAnimationSpeedDefault;
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  Preset resolvers (map preset names â†’ numeric values)
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        //  Damage: Lenient / Mild / Default / Severe / Brutal
        //  Durations increased across the board
        private static float ResolveTier2ThresholdGap(string preset)
        {
            switch (preset)
            {
                case PresetScaleVeryLow:
                    return 10f;
                case PresetScaleLow:
                    return 8f;
                case PresetScaleHigh:
                    return 5f;
                case PresetScaleVeryHigh:
                    return 4f;
                default:
                    return 7f;
            }
        }

        private static void ResolveDamagePreset(string preset, out float legsThreshold, out float armsThreshold, out float legsDuration, out float armsDuration)
        {
            legsThreshold = 12f; armsThreshold = 14f; legsDuration = 25f; armsDuration = 20f; // Default

            switch (preset)
            {
                case PresetDamageLenient:
                    legsThreshold = 20f; armsThreshold = 22f; legsDuration = 12f; armsDuration = 8f;
                    break;
                case PresetDamageMild:
                    legsThreshold = 16f; armsThreshold = 18f; legsDuration = 18f; armsDuration = 14f;
                    break;
                case PresetDamageSevere:
                    legsThreshold = 10f; armsThreshold = 12f; legsDuration = 40f; armsDuration = 32f;
                    break;
                case PresetDamageBrutal:
                    legsThreshold = 8f; armsThreshold = 10f; legsDuration = 60f; armsDuration = 50f;
                    break;
            }
        }

        //  Injury Slowness: Minimal / Light / Default / Heavy / Crippling
        private static void ResolveInjurySlowPreset(string preset, out float legsSlow, out float armsSlow, out float legsTier2Slow, out float armsTier2Slow, out bool legsSlowStacks, out bool armsSlowStacks)
        {
            // Defaults
            legsSlow = 25f; armsSlow = 20f; legsTier2Slow = 50f; armsTier2Slow = 40f;
            legsSlowStacks = true; armsSlowStacks = true;

            switch (preset)
            {
                case PresetInjurySlowMinimal:
                    legsSlow = 10f; armsSlow = 5f; legsTier2Slow = 20f; armsTier2Slow = 10f;
                    legsSlowStacks = false; armsSlowStacks = false;
                    break;
                case PresetInjurySlowLight:
                    legsSlow = 15f; armsSlow = 10f; legsTier2Slow = 30f; armsTier2Slow = 20f;
                    legsSlowStacks = true; armsSlowStacks = true;
                    break;
                case PresetInjurySlowHeavy:
                    legsSlow = 30f; armsSlow = 25f; legsTier2Slow = 60f; armsTier2Slow = 50f;
                    legsSlowStacks = true; armsSlowStacks = true;
                    break;
                case PresetInjurySlowCrippling:
                    legsSlow = 40f; armsSlow = 30f; legsTier2Slow = 75f; armsTier2Slow = 60f;
                    legsSlowStacks = true; armsSlowStacks = true;
                    break;
            }
        }

        //  Knockout: Brief / Short / Default / Long / Extended
        private static void ResolveKnockoutPreset(
            string preset,
            out float knockoutDuration,
            out float knockoutSlow,
            out float knockoutTier1HitDamage,
            out float knockoutTier2HitDamage,
            out float knockoutTier2DurationMultiplier)
        {
            knockoutDuration = 6f;
            knockoutSlow = 30f;
            knockoutTier1HitDamage = 20f;
            knockoutTier2HitDamage = 30f;
            knockoutTier2DurationMultiplier = 10f;

            switch (preset)
            {
                case PresetKnockoutBrief:
                    knockoutDuration = 2f;
                    knockoutSlow = 15f;
                    knockoutTier1HitDamage = 30f;
                    knockoutTier2HitDamage = 40f;
                    knockoutTier2DurationMultiplier = 4f;
                    break;
                case PresetKnockoutShort:
                    knockoutDuration = 4f;
                    knockoutSlow = 20f;
                    knockoutTier1HitDamage = 24f;
                    knockoutTier2HitDamage = 35f;
                    knockoutTier2DurationMultiplier = 8f;
                    break;
                case PresetKnockoutLong:
                    knockoutDuration = 10f;
                    knockoutSlow = 40f;
                    knockoutTier1HitDamage = 16f;
                    knockoutTier2HitDamage = 24f;
                    knockoutTier2DurationMultiplier = 12f;
                    break;
                case PresetKnockoutExtended:
                    knockoutDuration = 16f;
                    knockoutSlow = 50f;
                    knockoutTier1HitDamage = 12f;
                    knockoutTier2HitDamage = 18f;
                    knockoutTier2DurationMultiplier = 20f;
                    break;
            }
        }

        //  Last Stand Chance: Rare / Unlikely / Default / Likely / Frequent
        private static void ResolveLastStandChancePreset(string preset, out float deadChance, out int maxRecoveries, out bool? forceLastStandEnabled)
        {
            deadChance = 30f; maxRecoveries = 3; forceLastStandEnabled = true; // Default

            switch (preset)
            {
                case PresetLastStandOffLegacy:
                    forceLastStandEnabled = false;
                    deadChance = 0f; maxRecoveries = 0;
                    break;
                case PresetLastStandRare:
                    deadChance = 10f; maxRecoveries = 1;
                    break;
                case PresetLastStandUnlikely:
                    deadChance = 20f; maxRecoveries = 2;
                    break;
                case PresetLastStandLikely:
                    deadChance = 60f; maxRecoveries = 4;
                    break;
                case PresetLastStandFrequent:
                    deadChance = 100f; maxRecoveries = 5;
                    break;
            }
        }

        //  Recovery: Weak / Frail / Default / Strong / Resilient
        private static void ResolveRecoveryPreset(string preset, out float recoveryReduction, out bool restorePins, out float deadSlow, out bool slowDebuffsStack)
        {
            recoveryReduction = 40f; restorePins = false; deadSlow = 1.30f; slowDebuffsStack = true; // Default

            switch (preset)
            {
                case PresetRecoveryWeak:
                    recoveryReduction = 20f; deadSlow = 1.10f; slowDebuffsStack = false;
                    break;
                case PresetRecoveryFrail:
                    recoveryReduction = 30f; deadSlow = 1.20f; slowDebuffsStack = true;
                    break;
                case PresetRecoveryStrong:
                    recoveryReduction = 55f; deadSlow = 1.50f; slowDebuffsStack = true;
                    break;
                case PresetRecoveryResilient:
                    recoveryReduction = 70f; deadSlow = 1.80f; slowDebuffsStack = true;
                    break;
            }
        }

        private static void ResolveDurationPreset(string preset, out float legsDuration, out float armsDuration, out float knockoutDuration)
        {
            legsDuration = 25f;
            armsDuration = 20f;
            knockoutDuration = 6f;

            switch (preset)
            {
                case PresetDurationVeryShort:
                    legsDuration = 8f;
                    armsDuration = 6f;
                    knockoutDuration = 2f;
                    break;
                case PresetDurationShort:
                    legsDuration = 15f;
                    armsDuration = 12f;
                    knockoutDuration = 4f;
                    break;
                case PresetDurationLong:
                    legsDuration = 35f;
                    armsDuration = 30f;
                    knockoutDuration = 10f;
                    break;
                case PresetDurationVeryLong:
                    legsDuration = 50f;
                    armsDuration = 40f;
                    knockoutDuration = 16f;
                    break;
            }
        }

        private static void ResolveLimpnessPreset(
            string preset,
            out float legLimpness,
            out float armLimpness,
            out float handLimpness,
            out float footLimpness,
            out float wingLimpness,
            out float hipLimpness,
            out float torsoLimpness)
        {
            legLimpness = 80f;
            armLimpness = 60f;
            handLimpness = 70f;
            footLimpness = 80f;
            wingLimpness = 70f;
            hipLimpness = 100f;
            torsoLimpness = 70f;

            switch (preset)
            {
                case PresetLimpnessVeryLight:
                    legLimpness = 40f;
                    armLimpness = 30f;
                    handLimpness = 35f;
                    footLimpness = 40f;
                    wingLimpness = 35f;
                    hipLimpness = 60f;
                    torsoLimpness = 35f;
                    break;
                case PresetLimpnessLight:
                    legLimpness = 60f;
                    armLimpness = 45f;
                    handLimpness = 50f;
                    footLimpness = 60f;
                    wingLimpness = 50f;
                    hipLimpness = 80f;
                    torsoLimpness = 50f;
                    break;
                case PresetLimpnessHeavy:
                    legLimpness = 90f;
                    armLimpness = 75f;
                    handLimpness = 80f;
                    footLimpness = 90f;
                    wingLimpness = 80f;
                    hipLimpness = 100f;
                    torsoLimpness = 80f;
                    break;
                case PresetLimpnessVeryHeavy:
                    legLimpness = 100f;
                    armLimpness = 90f;
                    handLimpness = 95f;
                    footLimpness = 100f;
                    wingLimpness = 95f;
                    hipLimpness = 100f;
                    torsoLimpness = 90f;
                    break;
            }
        }

        private static void ResolveSynergyPreset(
            string preset,
            out float impactVelocityInfluence,
            out float twoHandedBonus,
            out float heavyWeaponBonus,
            out float axeSlashBonus,
            out float traumaIncrease,
            out float traumaMaxBonus,
            out int permanentAfterCount,
            out float reactionSlow,
            out float reactionDuration,
            out float reactionLegCooldown,
            out float reactionArmCooldown,
            out float reactionOtherCooldown)
        {
            impactVelocityInfluence = 40f;
            twoHandedBonus = 20f;
            heavyWeaponBonus = 20f;
            axeSlashBonus = 10f;
            traumaIncrease = 20f;
            traumaMaxBonus = 160f;
            permanentAfterCount = 0;
            reactionSlow = 20f;
            reactionDuration = 6f;
            reactionLegCooldown = 2.0f;
            reactionArmCooldown = 1.5f;
            reactionOtherCooldown = 1.5f;

            switch (preset)
            {
                case PresetScaleVeryLow:
                    impactVelocityInfluence = 20f;
                    twoHandedBonus = 10f;
                    heavyWeaponBonus = 10f;
                    axeSlashBonus = 5f;
                    traumaIncrease = 10f;
                    traumaMaxBonus = 80f;
                    permanentAfterCount = 0;
                    reactionSlow = 10f;
                    reactionDuration = 4f;
                    reactionLegCooldown = 3.0f;
                    reactionArmCooldown = 2.0f;
                    reactionOtherCooldown = 2.0f;
                    break;
                case PresetScaleLow:
                    impactVelocityInfluence = 30f;
                    twoHandedBonus = 15f;
                    heavyWeaponBonus = 15f;
                    axeSlashBonus = 8f;
                    traumaIncrease = 15f;
                    traumaMaxBonus = 120f;
                    permanentAfterCount = 0;
                    reactionSlow = 15f;
                    reactionDuration = 5f;
                    reactionLegCooldown = 2.5f;
                    reactionArmCooldown = 1.8f;
                    reactionOtherCooldown = 1.8f;
                    break;
                case PresetScaleHigh:
                    impactVelocityInfluence = 50f;
                    twoHandedBonus = 25f;
                    heavyWeaponBonus = 25f;
                    axeSlashBonus = 15f;
                    traumaIncrease = 25f;
                    traumaMaxBonus = 220f;
                    permanentAfterCount = 4;
                    reactionSlow = 25f;
                    reactionDuration = 8f;
                    reactionLegCooldown = 1.5f;
                    reactionArmCooldown = 1.0f;
                    reactionOtherCooldown = 1.0f;
                    break;
                case PresetScaleVeryHigh:
                    impactVelocityInfluence = 60f;
                    twoHandedBonus = 30f;
                    heavyWeaponBonus = 30f;
                    axeSlashBonus = 20f;
                    traumaIncrease = 30f;
                    traumaMaxBonus = 300f;
                    permanentAfterCount = 3;
                    reactionSlow = 30f;
                    reactionDuration = 10f;
                    reactionLegCooldown = 1.0f;
                    reactionArmCooldown = 0.5f;
                    reactionOtherCooldown = 0.5f;
                    break;
            }
        }

        //  Weight: Light (+20%) / Moderate (+30%) / Default (+50%) / Heavy (+70%) / Massive (+100%)
        private static void ResolveWeightPreset(string preset, out float weightBonus)
        {
            weightBonus = 50f; // Default

            switch (preset)
            {
                case PresetWeightLight:
                    weightBonus = 20f;
                    break;
                case PresetWeightModerate:
                    weightBonus = 30f;
                    break;
                case PresetWeightHeavy:
                    weightBonus = 70f;
                    break;
                case PresetWeightMassive:
                    weightBonus = 100f;
                    break;
            }
        }

        private static void ResolveDeathRecoveryKnockoutMultiplierPreset(string preset, out float knockoutMultiplier)
        {
            knockoutMultiplier = 15f;

            switch (preset)
            {
                case PresetScaleVeryLow:
                    knockoutMultiplier = 8f;
                    break;
                case PresetScaleLow:
                    knockoutMultiplier = 12f;
                    break;
                case PresetScaleHigh:
                    knockoutMultiplier = 20f;
                    break;
                case PresetScaleVeryHigh:
                    knockoutMultiplier = 25f;
                    break;
            }
        }

        private static void ResolveDeathAnimationSpeedPreset(string preset, out float animationSpeed)
        {
            animationSpeed = 0.75f;

            switch (preset)
            {
                case PresetDeathAnimationSpeedVeryLow:
                    animationSpeed = 0.25f;
                    break;
                case PresetDeathAnimationSpeedLow:
                    animationSpeed = 0.50f;
                    break;
                case PresetDeathAnimationSpeedHigh:
                    animationSpeed = 1.00f;
                    break;
                case PresetDeathAnimationSpeedVeryHigh:
                    animationSpeed = 1.25f;
                    break;
            }
        }

        private static void ResolvePerformanceProfilePreset(string preset, out bool performanceSafeguards, out float maxCreatures, out float maxHipTicks)
        {
            performanceSafeguards = true;
            maxCreatures = 80f;
            maxHipTicks = 12f;

            switch (preset)
            {
                case PresetPerformanceVerySafe:
                    maxCreatures = 40f;
                    maxHipTicks = 6f;
                    break;
                case PresetPerformanceSafe:
                    maxCreatures = 60f;
                    maxHipTicks = 8f;
                    break;
                case PresetPerformanceFast:
                    maxCreatures = 120f;
                    maxHipTicks = 18f;
                    break;
                case PresetPerformanceVeryFast:
                    maxCreatures = 180f;
                    maxHipTicks = 24f;
                    break;
            }
        }

        private static void ResolveLoggingProfilePreset(string preset, out bool basic, out bool diagnostics, out bool verbose)
        {
            basic = true;
            diagnostics = false;
            verbose = false;

            switch (preset)
            {
                case PresetLoggingMinimal:
                    basic = false;
                    diagnostics = false;
                    verbose = false;
                    break;
                case PresetLoggingBasic:
                case PresetLoggingDefault:
                    basic = true;
                    diagnostics = false;
                    verbose = false;
                    break;
                case PresetLoggingDiagnostic:
                    basic = true;
                    diagnostics = true;
                    verbose = false;
                    break;
                case PresetLoggingVerbose:
                    basic = true;
                    diagnostics = true;
                    verbose = true;
                    break;
            }
        }

        private static void ResolveWeaponCurvePreset(string preset, out float exponentPercent)
        {
            exponentPercent = 100f;

            switch (preset)
            {
                case PresetWeaponCurveVeryFlat:
                    exponentPercent = 60f;
                    break;
                case PresetWeaponCurveFlat:
                    exponentPercent = 80f;
                    break;
                case PresetWeaponCurveSteep:
                    exponentPercent = 130f;
                    break;
                case PresetWeaponCurveVerySteep:
                    exponentPercent = 160f;
                    break;
            }
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        //  Private helpers
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

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

        private static bool IsArmType(RagdollPart.Type limbType)
        {
            switch (limbType)
            {
                case RagdollPart.Type.LeftArm:
                case RagdollPart.Type.RightArm:
                case RagdollPart.Type.LeftHand:
                case RagdollPart.Type.RightHand:
                case RagdollPart.Type.LeftWing:
                case RagdollPart.Type.RightWing:
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

        private static bool SetString(ref string field, string value)
        {
            string next = value ?? string.Empty;
            if (string.Equals(field, next, StringComparison.Ordinal))
            {
                return false;
            }

            field = next;
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
