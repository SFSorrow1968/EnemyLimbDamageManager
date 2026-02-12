using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EnemyLimbDamageManager.Configuration;
using ThunderRoad;
using UnityEngine;

namespace EnemyLimbDamageManager.Core
{
    internal sealed class EnemyLimbManager
    {
        private enum LimbGroup
        {
            Legs = 0,
            Arms = 1,
            Feet = 2,
            Hands = 3,
            WingLeft = 4,
            WingRight = 5,
            Hip = 6,
            Head = 7,
            Neck = 8,
            Torso = 9,
        }

        private struct LimbState
        {
            public float AccumulatedDamage;
            public bool Disabled;
            public float DisabledUntil;
            public float DamageAtDisable;
            public int DisableCount;
            public bool PermanentDisabled;
        }

        private enum EyeMode
        {
            None = 0,
            Closed = 1,
            Rolled = 2,
        }

        private enum MouthMode
        {
            None = 0,
            Subtle = 1,
            Open = 2,
            WideOpen = 3,
        }

        private sealed class CreatureState
        {
            public Creature Creature;
            public float LastTouchedTime;
            public bool LocomotionClampApplied;
            public float LastAppliedSpeedMultiplier;
            public float LastAppliedAnimSpeedMultiplier;
            public bool NoStandModifierApplied;
            public bool KnockoutActive;
            public float KnockoutUntil;
            public bool IsDeathRecoveryKnockout;
            public bool FullBodyPinClampApplied;
            public bool EyeClipOverrideApplied;
            public bool EyeClipAutoWasActive;
            public EyeMode ActiveEyeMode;
            public bool PendingEyeRestore;
            public bool PendingDeadRecovery;
            public float DeadRecoveryAt;
            public float DeadRecoveryAnimationHoldUntil;
            public int SuccessfulDeadRecoveries;
            public float KnockoutSlowUntil;
            public float DeadRecoverySlowUntil;
            public int StandAttemptCount;
            public float NextStandAttemptAt;
            public float LastPierceSlashSoundTime;
            public float LastBluntBreakSoundTime;
            public float LastDisableSoundTime;
            public bool WeightApplied;
            public float LastAppliedWeightMultiplier;
            public float EyeRestoreAt;
            public MouthMode ActiveMouthMode;
            public bool PendingMouthRestore;
            public Quaternion JawOriginalLocalRotation;
            public bool JawOriginalLocalRotationCaptured;
            public bool WeightApplyNoPartsLogged;
            public bool MissingLocomotionLogged;
            public bool AnimatorSpeedOverrideApplied;
            public float AnimatorOriginalSpeed;
            public bool AnimatorDynamicSpeedOverrideApplied;
            public float AnimatorDynamicSpeedOriginal;
            public int AnimatorDynamicSpeedParamHash;
            public bool AnimatorDynamicSpeedParamResolved;
            public bool AnimatorDynamicSpeedParamAvailable;
            public bool MissingAnimatorLogged;
            public bool MissingAnimatorDynamicSpeedLogged;
            public bool MissingDeathAnimationModuleLogged;
            public bool DeathAnimationDataSpeedApplied;
            public float LastDeathAnimationDataSpeed;
            public float TorsoParalyzedUntil;
            public float InjuryBehaviorSlowUntil;
            public float InjuryBehaviorSlowFactor;
            public float DeathDespawnExtendedUntil;
            public float LastKillAudioMouthFactor;
            public float LastKillHandledAt;
            public bool HasLastInjuryGroup;
            public LimbGroup LastInjuryGroup;
            public LimbGroup LastDeathInjuryGroup;
            public float LastInjuryAt;
            public float StandupSlowUntil;
            public float StandupBlendStartAt;
            public float StandupBlendEndAt;
            public bool PendingLastStandInterruption;
            public bool LastStandInterruptionTriggered;
            public float LastStandInterruptionAt;
            public float LastStandInterruptedUntil;
            public float MockDeathRollEyesAt;
            public float MockDeathClosedAmount;
            public float RolledEyeFallbackCloseAmount;
            public float RecoveredGazeCloseAmount;
            public bool KeepRecoveredGazeEyes;
            public bool RolledEyeClipApplied;
            public readonly Dictionary<LimbGroup, LimbState> Limbs = new Dictionary<LimbGroup, LimbState>();
            public readonly Dictionary<LimbGroup, List<HandleRagdoll>> DisabledHandles = new Dictionary<LimbGroup, List<HandleRagdoll>>();
            public readonly Dictionary<LimbGroup, float> ReactionCooldownUntil = new Dictionary<LimbGroup, float>();
            public readonly Dictionary<LimbGroup, int> HitSlowStacks = new Dictionary<LimbGroup, int>();
            public readonly Dictionary<LimbGroup, float> HitSlowExpireAt = new Dictionary<LimbGroup, float>();
            public readonly Dictionary<Rigidbody, float> OriginalPartMasses = new Dictionary<Rigidbody, float>();
        }

        private const float StaleCreatureStateSeconds = 45f;
        private const float PierceSlashSoundMinIntervalSeconds = 0.08f;
        private const float BluntBreakSoundMinIntervalSeconds = 0.35f;
        private const float StandRetryIntervalSeconds = 0.60f;
        private const int MaxStandRetryAttempts = 40;
        private const float EyeRestoreLeadSeconds = 3f;
        private const float MouthPresentationChance = 0.20f;
        private const float MinimumRecoveryDeathAnimSpeed = 0.10f;
        private const float TorsoParalysisTier1Seconds = 0.60f;
        private const float TorsoParalysisTier2Seconds = 3.25f;
        private const float PerformanceThrottleLogCooldownSeconds = 2f;
        private const float DeathMouthOpenBaseFactor = 0.20f;
        private const float KillEventDedupWindowSeconds = 0.25f;
        private const float CombatSlowReductionCap = 0.30f;
        private const float StandupSlowReductionCap = 0.50f;
        private const float DefaultStandupSlowSeconds = 5.0f;
        private const float StandupBlendStartSeconds = 1.6f;
        private const float StandupBlendDurationSeconds = 1.8f;
        private const float LastStandInterruptedKneelSeconds = 4.0f;
        private const float MinRecoveredGazeCloseAmount = 0.08f;
        private const float MaxRecoveredGazeCloseAmount = 0.70f;
        private const float RecoveredGazeOpenOffset = 0.08f;
        private const float DeadRecoveryQueueSeconds = 0.25f;

        private static readonly LimbGroup[] AllLimbGroups =
        {
            LimbGroup.Legs,
            LimbGroup.Arms,
            LimbGroup.Feet,
            LimbGroup.Hands,
            LimbGroup.WingLeft,
            LimbGroup.WingRight,
            LimbGroup.Hip,
            LimbGroup.Head,
            LimbGroup.Neck,
            LimbGroup.Torso,
        };

        private static readonly LimbGroup[] LegStyleGroups =
        {
            LimbGroup.Legs,
            LimbGroup.Feet,
            LimbGroup.Hip,
        };

        private static readonly LimbGroup[] ArmStyleGroups =
        {
            LimbGroup.Arms,
            LimbGroup.Hands,
            LimbGroup.WingLeft,
            LimbGroup.WingRight,
        };

        private static readonly string[] LegKeywords =
        {
            "LEG",
            "FOOT",
            "KNEE",
            "THIGH",
            "CALF",
            "SHIN",
            "ANKLE",
            "HOOF",
            "PAW",
            "HIND",
            "TAIL",
            "FLIPPER",
            "FIN"
        };

        private static readonly string[] ArmKeywords =
        {
            "ARM",
            "HAND",
            "WRIST",
            "ELBOW",
            "FOREARM",
            "UPPERARM",
            "WING",
            "CLAW",
            "TENTACLE",
            "FORE",
            "TALON",
            "PINCER"
        };

        private static readonly string[] FootKeywords =
        {
            "FOOT",
            "ANKLE",
            "HOOF",
            "PAW",
            "TOE",
        };

        private static readonly string[] HandKeywords =
        {
            "HAND",
            "WRIST",
            "PALM",
            "FINGER",
            "CLAW",
            "PINCER",
            "TALON",
        };

        private static readonly string[] WingKeywords =
        {
            "WING",
            "FEATHER",
        };

        private static readonly string[] TailKeywords =
        {
            "TAIL",
            "FLIPPER",
            "FIN",
        };

        private static readonly string[] HeadKeywords =
        {
            "HEAD",
            "SKULL",
            "FACE",
            "JAW",
        };

        private static readonly string[] NeckKeywords =
        {
            "NECK",
            "THROAT",
            "CERVICAL",
        };

        private static readonly string[] TorsoKeywords =
        {
            "TORSO",
            "CHEST",
            "SPINE",
            "BODY",
            "ABDOMEN",
            "PELVIS",
            "HIP",
        };

        private static readonly string[] LeftKeywords =
        {
            "LEFT",
            "_L",
            ".L",
            "L_",
        };

        private static readonly string[] RightKeywords =
        {
            "RIGHT",
            "_R",
            ".R",
            "R_",
        };

        public static EnemyLimbManager Instance { get; } = new EnemyLimbManager();

        private readonly Dictionary<int, CreatureState> trackedCreatures = new Dictionary<int, CreatureState>();
        private readonly Dictionary<AnimationData.Clip, float> originalDeathAnimationClipSpeeds = new Dictionary<AnimationData.Clip, float>();
        private float lastPerformanceThrottleLogAt;

        private EnemyLimbManager()
        {
        }

        public void Initialize()
        {
            ClearAllTrackedState();
        }

        public void Shutdown()
        {
            ClearAllTrackedState();
            RestorePatchedDeathAnimationClipSpeeds();
        }

        public void Update()
        {
            if (!ELDMModOptions.EnableMod)
            {
                if (trackedCreatures.Count > 0)
                {
                    ClearAllTrackedState();
                }
                else
                {
                    RestorePatchedDeathAnimationClipSpeeds();
                }

                return;
            }

            if (!ELDMModOptions.UseDeathAnimationSpeedSystem() && originalDeathAnimationClipSpeeds.Count > 0)
            {
                RestorePatchedDeathAnimationClipSpeeds();
            }

            float now = Time.unscaledTime;
            ProcessDeadRecoveries(now);
            ProcessRecoveries(now);
            ProcessKnockouts(now);
            ProcessStandRetries(now);
            ProcessMovementEffects(now);
            ProcessWeightMultipliers();
            CleanupStaleCreatures(now);
            ELDMTelemetry.RecordTrackedCreatures(trackedCreatures.Count);
        }

        public void OnCreatureSpawn(Creature creature)
        {
            if (!ELDMModOptions.EnableMod || !IsValidTargetCreature(creature))
            {
                return;
            }

            CreatureState state = EnsureCreatureState(creature);
            if (state != null)
            {
                ApplyWeightMultiplier(state, ELDMModOptions.GetWeightMultiplier());
                if (ELDMModOptions.UseDeathAnimationSpeedSystem())
                {
                    ApplyDeathAnimationDataSpeed(state, ResolveDeathAnimationSpeedForState(state), "spawn_sync");
                }
            }
        }

        public void OnCreatureDespawn(Creature creature, EventTime eventTime)
        {
            if (eventTime != EventTime.OnStart || creature == null)
            {
                return;
            }

            RemoveCreatureState(creature.GetInstanceID());
        }

        public void OnLevelUnload(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                ClearAllTrackedState();
            }
        }

        public void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (!ELDMModOptions.EnableMod || eventTime != EventTime.OnEnd || creature == null || creature.isPlayer)
            {
                return;
            }

            if (!IsRecoverableDeadCreature(creature))
            {
                return;
            }

            CreatureState state = EnsureCreatureStateAnyState(creature);
            float now = Time.unscaledTime;
            state.LastTouchedTime = now;
            if (now < state.LastKillHandledAt + KillEventDedupWindowSeconds &&
                !state.IsDeathRecoveryKnockout)
            {
                if (ELDMLog.DiagnosticsEnabled)
                {
                    ELDMLog.Diag(
                        "kill_event_deduped creature=" + SafeCreatureName(creature) +
                        " window=" + KillEventDedupWindowSeconds.ToString("0.00") + "s",
                        verboseOnly: true);
                }
                return;
            }
            state.LastKillHandledAt = now;
            RagdollPart killPart = collisionInstance != null ? collisionInstance.damageStruct.hitRagdollPart : null;
            DamageType killDamageType = collisionInstance != null ? collisionInstance.damageStruct.damageType : DamageType.Blunt;
            state.LastDeathInjuryGroup = ResolveDeathAnimationGroup(state, killPart, now);

            if (state.IsDeathRecoveryKnockout)
            {
                state.PendingDeadRecovery = false;
                state.KnockoutActive = false;
                state.KnockoutUntil = 0f;
                state.IsDeathRecoveryKnockout = false;
                state.DeadRecoveryAt = 0f;
                state.DeadRecoveryAnimationHoldUntil = 0f;
                state.NextStandAttemptAt = 0f;
                state.PendingLastStandInterruption = false;
                state.LastStandInterruptionTriggered = false;
                state.LastStandInterruptionAt = 0f;
                state.LastStandInterruptedUntil = 0f;
                state.KeepRecoveredGazeEyes = false;
                state.RecoveredGazeCloseAmount = 0f;
                ApplyPermanentDeathEyePresentation(state, "death_recovery_knockout_killed");
                ELDMLog.Info(
                    "death_recovery_knockout_killed creature=" + SafeCreatureName(creature) +
                    " part=" + SafePartName(killPart) +
                    " damageType=" + killDamageType);
                return;
            }

            if (!ELDMModOptions.CanAttemptDeadRecovery())
            {
                ApplyPermanentDeathEyePresentation(state, "last_stand_disabled_or_zero");
                return;
            }

            if (IsKillDisqualifyingForRecovery(collisionInstance, killPart))
            {
                ELDMLog.Info(
                    "dead_recovery_skipped creature=" + SafeCreatureName(creature) +
                    " reason=excluded_kill_part" +
                    " part=" + SafePartName(killPart) +
                    " damageType=" + (collisionInstance != null ? collisionInstance.damageStruct.damageType.ToString() : "Unknown"),
                    verboseOnly: true);
                ApplyPermanentDeathEyePresentation(state, "excluded_kill_part");
                return;
            }

            int maxRecoveries = ELDMModOptions.GetMaxDeadRecoveries();
            if (state.SuccessfulDeadRecoveries >= maxRecoveries)
            {
                ELDMLog.Info(
                    "dead_recovery_skipped creature=" + SafeCreatureName(creature) +
                    " reason=max_recoveries_reached" +
                    " successful=" + state.SuccessfulDeadRecoveries +
                    " max=" + maxRecoveries,
                    verboseOnly: true);
                ApplyPermanentDeathEyePresentation(state, "max_recoveries_reached");
                return;
            }

            float chance = ELDMModOptions.GetEffectiveDeadRecoveryChanceRatio(state.SuccessfulDeadRecoveries);
            float roll = UnityEngine.Random.value;
            if (roll > chance)
            {
                ELDMLog.Info(
                    "dead_recovery_skipped creature=" + SafeCreatureName(creature) +
                    " reason=chance_failed" +
                    " roll=" + roll.ToString("0.000") +
                    " chance=" + chance.ToString("0.000"),
                    verboseOnly: true);
                ApplyPermanentDeathEyePresentation(state, "chance_failed");
                return;
            }

            QueueDeadRecovery(state, now, chance, roll, killPart, killDamageType);
        }

        public void OnCreatureHit(Creature creature, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (!ELDMModOptions.EnableMod || eventTime != EventTime.OnEnd)
            {
                return;
            }

            if (!IsValidHitTargetCreature(creature) || collisionInstance == null || !collisionInstance.damageStruct.active)
            {
                return;
            }

            RagdollPart hitPart = collisionInstance.damageStruct.hitRagdollPart;
            if (!TryMapPartToGroup(hitPart, out LimbGroup limbGroup))
            {
                return;
            }

            DamageType damageType = collisionInstance.damageStruct.damageType;
            float rawDamage = Mathf.Max(0f, collisionInstance.damageStruct.damage);
            float trackedDamage = rawDamage;
            float impactScale = 1f;
            float impactVelocity = 0f;
            float now = Time.unscaledTime;
            CreatureState state = EnsureCreatureStateAnyState(creature);
            state.LastTouchedTime = now;
            state.HasLastInjuryGroup = true;
            state.LastInjuryGroup = limbGroup;
            state.LastInjuryAt = now;
            bool creatureAlive = IsCreatureAlive(creature);
            if (!creatureAlive)
            {
                return;
            }

            if (rawDamage < ELDMModOptions.GetMinimumTrackedHitDamage())
            {
                return;
            }
            bool suppressInjurySounds = state.IsDeathRecoveryKnockout;

            if (!suppressInjurySounds)
            {
                if ((damageType == DamageType.Pierce || damageType == DamageType.Slash) &&
                    ELDMLog.DiagnosticsEnabled &&
                    now >= state.LastPierceSlashSoundTime + PierceSlashSoundMinIntervalSeconds)
                {
                    state.LastPierceSlashSoundTime = now;
                    ELDMLog.Diag(
                        "sfx_collision_replay_suppressed creature=" + SafeCreatureName(creature) +
                        " part=" + SafePartName(hitPart) +
                        " damageType=" + damageType,
                        verboseOnly: true);
                }
            }
            else if (ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "sfx_suppressed creature=" + SafeCreatureName(creature) +
                    " reason=death_recovery_knockout" +
                    " part=" + SafePartName(hitPart),
                    verboseOnly: true);
            }

            int knockoutTier = DetermineKnockoutTier(collisionInstance, damageType, trackedDamage, limbGroup, hitPart);
            if (knockoutTier > 0)
            {
                if (creatureAlive)
                {
                    string knockoutReason = "hit_knockout_" + damageType;
                    BeginKnockout(state, now, knockoutReason);
                }
                else if (ELDMLog.DiagnosticsEnabled)
                {
                    ELDMLog.Diag(
                        "knockout_skipped creature=" + SafeCreatureName(creature) +
                        " reason=creature_dead" +
                        " knockout=true" +
                        " part=" + SafePartName(hitPart),
                        verboseOnly: true);
                }
            }

            LimbState limbState = state.Limbs[limbGroup];
            limbState.AccumulatedDamage += trackedDamage;
            state.Limbs[limbGroup] = limbState;
            ELDMTelemetry.RecordHit();

            int disablePassCount = DetermineDisablePassCountForHit(
                collisionInstance,
                damageType,
                trackedDamage,
                limbState.AccumulatedDamage,
                limbGroup,
                out string disableReason,
                out float thresholdDamage);
            RegisterHitSlowDebuff(state, limbGroup, trackedDamage, thresholdDamage, now);

            if (limbGroup == LimbGroup.Torso && ELDMModOptions.ShouldLogTorsoPlaceholderHits() && ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "torso_hit creature=" + SafeCreatureName(creature) +
                    " part=" + SafePartName(hitPart) +
                    " damage=" + trackedDamage.ToString("0.00") +
                    " damageType=" + damageType +
                    " reason=" + disableReason,
                    verboseOnly: true);
            }

            if (disablePassCount <= 0)
            {
                if (ELDMLog.VerboseEnabled && ELDMTelemetry.ShouldLogHit(creature.GetInstanceID(), GroupName(limbGroup), now))
                {
                    ELDMLog.Info(
                        "hit_no_disable creature=" + SafeCreatureName(creature) +
                        " group=" + GroupName(limbGroup) +
                        " damage=" + trackedDamage.ToString("0.00") +
                        " raw=" + rawDamage.ToString("0.00") +
                        " scale=" + impactScale.ToString("0.00") +
                        " threshold=" + thresholdDamage.ToString("0.0") +
                        " reason=" + disableReason,
                        verboseOnly: true);
                }
                return;
            }

            if (!CanDisableGroup(limbGroup))
            {
                if (ELDMLog.DiagnosticsEnabled)
                {
                    ELDMLog.Diag(
                        "hit_disable_skipped creature=" + SafeCreatureName(creature) +
                        " group=" + GroupName(limbGroup) +
                        " reason=knockout_only_group" +
                        " damage=" + trackedDamage.ToString("0.00") +
                        " passCount=" + disablePassCount,
                        verboseOnly: true);
                }
                return;
            }

            int passesCrossed = Mathf.Max(0, disablePassCount - limbState.DisableCount);
            if (passesCrossed <= 0)
            {
                if (limbState.Disabled && ELDMModOptions.ShouldRefreshDisableTimerOnHit())
                {
                    RefreshDisableTimer(state, limbGroup, now);
                }

                if (ELDMLog.VerboseEnabled && ELDMTelemetry.ShouldLogHit(creature.GetInstanceID(), GroupName(limbGroup), now))
                {
                    ELDMLog.Info(
                        "hit_on_disabled creature=" + SafeCreatureName(creature) +
                        " group=" + GroupName(limbGroup) +
                        " damage=" + trackedDamage.ToString("0.00") +
                        " raw=" + rawDamage.ToString("0.00") +
                        " scale=" + impactScale.ToString("0.00") +
                        " disableCount=" + limbState.DisableCount +
                        " passCount=" + disablePassCount,
                        verboseOnly: true);
                }
                return;
            }

            if (ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "hit_scaled creature=" + SafeCreatureName(creature) +
                    " group=" + GroupName(limbGroup) +
                    " raw=" + rawDamage.ToString("0.00") +
                    " effective=" + trackedDamage.ToString("0.00") +
                    " scale=" + impactScale.ToString("0.00") +
                    " velocity=" + impactVelocity.ToString("0.00") +
                    " twoHanded=false" +
                    " token=none",
                    verboseOnly: true);
            }

            DisableLimbAtPassCount(state, limbGroup, hitPart, collisionInstance, damageType, now, disablePassCount);
        }
        private void ProcessDeadRecoveries(float now)
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            List<int> invalidIds = null;

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                CreatureState state = entry.Value;
                if (!state.PendingDeadRecovery)
                {
                    continue;
                }

                Creature creature = state.Creature;
                if (creature == null)
                {
                    if (invalidIds == null)
                    {
                        invalidIds = new List<int>();
                    }

                    invalidIds.Add(entry.Key);
                    continue;
                }

                if (now < state.DeadRecoveryAt)
                {
                    ApplyEyePresentation(state, state.ActiveEyeMode);
                    ApplyMouthOpenFactor(state, ResolvePresentationMouthFactor(state, 0f), "dead_recovery_queue");
                    continue;
                }

                if (now < state.DeadRecoveryAnimationHoldUntil)
                {
                    ApplyEyePresentation(state, state.ActiveEyeMode);
                    ApplyMouthOpenFactor(state, ResolvePresentationMouthFactor(state, 0f), "dead_recovery_hold");
                    continue;
                }

                if (!IsRecoverableDeadCreature(creature))
                {
                    state.PendingDeadRecovery = false;
                    state.DeadRecoveryAt = 0f;
                    state.DeadRecoveryAnimationHoldUntil = 0f;
                    state.KeepRecoveredGazeEyes = false;
                    RestoreEyePresentation(state);
                    ELDMLog.Info(
                        "dead_recovery_queue_cleared creature=" + SafeCreatureName(creature) +
                        " reason=no_longer_dead",
                        verboseOnly: true);
                    continue;
                }

                if (!TryPerformDeadRecovery(state))
                {
                    state.PendingDeadRecovery = false;
                    state.DeadRecoveryAt = 0f;
                    state.DeadRecoveryAnimationHoldUntil = 0f;
                    state.KeepRecoveredGazeEyes = false;
                    RestoreEyePresentation(state);
                    ELDMLog.Warn(
                        "dead_recovery_queue_cleared creature=" + SafeCreatureName(creature) +
                        " reason=recovery_failed");
                }
            }

            if (invalidIds == null)
            {
                return;
            }

            for (int i = 0; i < invalidIds.Count; i++)
            {
                RemoveCreatureState(invalidIds[i]);
            }
        }

        private void QueueDeadRecovery(
            CreatureState state,
            float now,
            float chance,
            float roll,
            RagdollPart killPart,
            DamageType killDamageType)
        {
            if (state?.Creature == null)
            {
                return;
            }

            state.PendingDeadRecovery = true;
            state.DeadRecoveryAt = now + DeadRecoveryQueueSeconds;
            state.DeadRecoveryAnimationHoldUntil = state.DeadRecoveryAt;
            state.KnockoutActive = false;
            state.KnockoutUntil = 0f;
            state.IsDeathRecoveryKnockout = false;
            state.NextStandAttemptAt = 0f;
            state.StandAttemptCount = 0;
            state.PendingLastStandInterruption = false;
            state.LastStandInterruptionTriggered = false;
            state.LastStandInterruptionAt = 0f;
            state.LastStandInterruptedUntil = 0f;
            state.KeepRecoveredGazeEyes = false;
            state.RecoveredGazeCloseAmount = 0f;

            ApplyDeathPresentation(state, "dead_recovery_queued", shouldAutoRestoreAfterRecovery: true);
            float deathAnimSpeed = ResolveDeathAnimationSpeedForState(state);
            ApplyDeathAnimationDataSpeed(state, deathAnimSpeed, "dead_recovery_queued");
            EnsureDeathDespawnWindow(state, deathAnimSpeed, now);

            ELDMLog.Info(
                "dead_recovery_queued creature=" + SafeCreatureName(state.Creature) +
                " queue=" + DeadRecoveryQueueSeconds.ToString("0.00") + "s" +
                " recoverAt=" + state.DeadRecoveryAt.ToString("0.00") +
                " roll=" + roll.ToString("0.000") +
                " chance=" + chance.ToString("0.000") +
                " part=" + SafePartName(killPart) +
                " damageType=" + killDamageType +
                " deathAnimSpeed=" + deathAnimSpeed.ToString("0.00"));
        }

        private bool TryPerformDeadRecovery(CreatureState state)
        {
            Creature creature = state.Creature;
            if (creature == null)
            {
                return false;
            }

            float now = Time.unscaledTime;
            float health = Mathf.Max(1f, creature.maxHealth * ELDMModOptions.GetDeadRevivalHealth());
            try
            {
                creature.Resurrect(health, creature);
            }
            catch (Exception ex)
            {
                ELDMLog.Warn("dead_recovery_error creature=" + SafeCreatureName(creature) + " error=" + ex.Message);
                return false;
            }

            if (creature.state == Creature.State.Dead)
            {
                ELDMLog.Warn("dead_recovery_failed creature=" + SafeCreatureName(creature) + " reason=still_dead");
                return false;
            }

            try
            {
                creature.CancelInvoke("Despawn");
            }
            catch
            {
            }

            state.PendingDeadRecovery = false;
            state.DeadRecoveryAt = 0f;
            state.DeadRecoveryAnimationHoldUntil = 0f;
            state.DeathDespawnExtendedUntil = 0f;
            state.SuccessfulDeadRecoveries++;
            state.StandAttemptCount = 0;
            state.NextStandAttemptAt = 0f;
            state.LastTouchedTime = now;

            float deadSlowDuration = ELDMModOptions.GetDeadRecoverySlowDurationSeconds();
            float deathRecoveryKnockoutDuration = ELDMModOptions.GetDeathRecoveryKnockoutDuration();
            bool useDeathRecoveryKnockout = deathRecoveryKnockoutDuration > 0.05f;
            float standFailChanceRatio = ELDMModOptions.GetReviveStandupFailChanceRatio();

            if (state.KnockoutActive)
            {
                EndKnockout(state, now, "dead_recovery");
            }

            state.DeadRecoverySlowUntil = now + deadSlowDuration + (useDeathRecoveryKnockout ? deathRecoveryKnockoutDuration : 0f);
            state.IsDeathRecoveryKnockout = useDeathRecoveryKnockout;
            state.KnockoutActive = useDeathRecoveryKnockout;
            state.KnockoutUntil = useDeathRecoveryKnockout ? now + deathRecoveryKnockoutDuration : 0f;

            float standAt = state.KnockoutActive
                ? Mathf.Max(state.KnockoutUntil + 0.35f, now + EyeRestoreLeadSeconds + 0.35f)
                : now + EyeRestoreLeadSeconds + 0.35f;
            state.NextStandAttemptAt = standAt;
            float standFailRoll = UnityEngine.Random.value;
            state.PendingLastStandInterruption = useDeathRecoveryKnockout && standFailRoll < standFailChanceRatio;
            state.LastStandInterruptionTriggered = false;
            state.LastStandInterruptionAt = state.PendingLastStandInterruption
                ? standAt
                : 0f;
            state.LastStandInterruptedUntil = 0f;
            state.KeepRecoveredGazeEyes = useDeathRecoveryKnockout;
            state.RecoveredGazeCloseAmount = ResolveRecoveredGazeCloseAmount(state);
            bool hasQueuedPresentation = state.ActiveEyeMode != EyeMode.None || state.ActiveMouthMode != MouthMode.None;
            if (useDeathRecoveryKnockout)
            {
                if (!hasQueuedPresentation)
                {
                    ApplyDeathPresentation(state, "dead_recovery_mimic", shouldAutoRestoreAfterRecovery: true);
                }
                else
                {
                    state.PendingMouthRestore = state.ActiveMouthMode != MouthMode.None;
                }

                state.PendingEyeRestore = true;
                state.EyeRestoreAt = state.KnockoutUntil + 0.80f;
            }
            else
            {
                state.PendingEyeRestore = true;
                state.EyeRestoreAt = Mathf.Max(now, standAt - EyeRestoreLeadSeconds);
                state.ActiveEyeMode = EyeMode.Rolled;
                state.MockDeathRollEyesAt = 0f;
                if (state.ActiveMouthMode == MouthMode.None)
                {
                    MaybeApplyRandomMouthPresentation(state, "dead_recovery_alive", shouldAutoRestore: true);
                }
                else
                {
                    state.PendingMouthRestore = true;
                }
            }
            ApplyEyePresentation(state, state.ActiveEyeMode);
            ApplyMouthOpenFactor(state, ResolvePresentationMouthFactor(state, 0f), "dead_recovery_active");
            if (useDeathRecoveryKnockout)
            {
                ApplyKnockoutPose(state);
                ForceReleaseHeldItems(creature);
            }

            float retainedRatio = ELDMModOptions.GetRetainedDamageRatioAfterRecovery();
            bool legsStillDisabled = false;
            bool armsStillDisabled = false;
            for (int i = 0; i < AllLimbGroups.Length; i++)
            {
                LimbGroup group = AllLimbGroups[i];
                LimbState limbState = state.Limbs[group];
                limbState.AccumulatedDamage *= retainedRatio;
                state.Limbs[group] = limbState;

                if (limbState.Disabled)
                {
                    if (group == LimbGroup.Legs || group == LimbGroup.Feet)
                    {
                        legsStillDisabled = true;
                        SetGroupPinsDisabled(state, group);
                    }
                    else if (group == LimbGroup.Hip)
                    {
                        SetGroupPinsDisabled(state, group);
                    }
                    else if (IsArmStyleGroup(group))
                    {
                        armsStillDisabled = true;
                        ApplyArmDisabled(state, group);
                    }
                }
                else if (ELDMModOptions.RecoveryRestoresPinForces)
                {
                    RestorePinsAndHandlesForGroup(state, group);
                }
                else
                {
                    ClearHandleBucket(state, group);
                }
            }

            UpdateMovementEffects(state, forceFall: false);
            bool standAttempted = false;
            if (!legsStillDisabled)
            {
                standAttempted = true;
                TryStandUpAfterRecovery(creature);
            }
            else
            {
                state.NextStandAttemptAt = 0f;
            }
            ELDMTelemetry.RecordRecover();

            ELDMLog.Info(
                "dead_recovery_success creature=" + SafeCreatureName(creature) +
                " health=" + health.ToString("0.0") +
                " recoveries=" + state.SuccessfulDeadRecoveries + "/" + ELDMModOptions.GetMaxDeadRecoveries() +
                " mimicDeath=" + useDeathRecoveryKnockout +
                " mimicDuration=" + (useDeathRecoveryKnockout ? deathRecoveryKnockoutDuration.ToString("0.0") + "s" : "0.0s") +
                " legsDisabled=" + legsStillDisabled +
                " armsDisabled=" + armsStillDisabled +
                " standAttempted=" + standAttempted +
                " standFailPending=" + state.PendingLastStandInterruption +
                " standFailRoll=" + standFailRoll.ToString("0.000") +
                " standFailChance=" + standFailChanceRatio.ToString("0.000") +
                " mouthMode=" + state.ActiveMouthMode +
                " deadSlowUntil=" + state.DeadRecoverySlowUntil.ToString("0.00") +
                " deadSlowPercent=" + (ELDMModOptions.GetDeadRecoverySlowDebuffRatio() * 100f).ToString("0") + "%" +
                " deathAnimSpeed=" + ELDMModOptions.GetDeathAnimationSpeedMultiplier().ToString("0.00"));

            return true;
        }

        private void ProcessRecoveries(float now)
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            List<int> invalidIds = null;
            int processed = 0;
            int budget = GetCreatureProcessBudget();

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                processed++;
                if (processed > budget)
                {
                    LogPerformanceThrottle("recoveries", processed, budget);
                    break;
                }

                CreatureState state = entry.Value;

                if (state.Creature == null)
                {
                    if (invalidIds == null)
                    {
                        invalidIds = new List<int>();
                    }

                    invalidIds.Add(entry.Key);
                    continue;
                }

                if (state.PendingDeadRecovery)
                {
                    continue;
                }

                if (!IsCreatureAlive(state.Creature))
                {
                    continue;
                }

                for (int i = 0; i < AllLimbGroups.Length; i++)
                {
                    LimbGroup group = AllLimbGroups[i];
                    LimbState limbState = state.Limbs[group];
                    if (!limbState.Disabled)
                    {
                        continue;
                    }

                    if (limbState.PermanentDisabled)
                    {
                        if (ELDMLog.DiagnosticsEnabled)
                        {
                            ELDMLog.Diag(
                                "recovery_skipped_permanent creature=" + SafeCreatureName(state.Creature) +
                                " group=" + GroupName(group),
                                verboseOnly: true);
                        }
                        continue;
                    }

                    if (!IsFinite(limbState.DisabledUntil) || now < limbState.DisabledUntil)
                    {
                        continue;
                    }

                    RecoverLimb(state, group);
                }
            }

            if (invalidIds == null)
            {
                return;
            }

            for (int i = 0; i < invalidIds.Count; i++)
            {
                RemoveCreatureState(invalidIds[i]);
            }
        }

        private void ProcessKnockouts(float now)
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            int processed = 0;
            int budget = GetCreatureProcessBudget();
            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                processed++;
                if (processed > budget)
                {
                    LogPerformanceThrottle("knockouts", processed, budget);
                    break;
                }

                CreatureState state = entry.Value;
                if (!state.KnockoutActive)
                {
                    continue;
                }

                Creature creature = state.Creature;
                if (creature == null)
                {
                    continue;
                }

                if ((!ELDMModOptions.KnockoutEnabled && !state.IsDeathRecoveryKnockout) || now >= state.KnockoutUntil || !IsCreatureAlive(creature))
                {
                    EndKnockout(state, now, ELDMModOptions.KnockoutEnabled ? "timer" : "disabled");
                    continue;
                }

                state.LastTouchedTime = now;
                ApplyKnockoutPose(state);
                ForceReleaseHeldItems(creature);
                UpdateMockDeathVisualPhase(state, now, state.IsDeathRecoveryKnockout ? "last_stand_mimic" : "knockout");
                ApplyEyePresentation(state, state.ActiveEyeMode);
                ApplyMouthOpenFactor(state, ResolvePresentationMouthFactor(state, 0f), "knockout_hold");
                UpdateMovementEffects(state, forceFall: false);
            }
        }

        private void ProcessStandRetries(float now)
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            int processed = 0;
            int budget = GetCreatureProcessBudget();
            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                processed++;
                if (processed > budget)
                {
                    LogPerformanceThrottle("stand_retries", processed, budget);
                    break;
                }

                CreatureState state = entry.Value;
                if (state.NextStandAttemptAt <= 0f || now < state.NextStandAttemptAt)
                {
                    continue;
                }

                Creature creature = state.Creature;
                if (creature == null || !IsCreatureAlive(creature) || creature.ragdoll == null)
                {
                    state.NextStandAttemptAt = 0f;
                    state.StandAttemptCount = 0;
                    continue;
                }

                if (now < state.LastStandInterruptedUntil)
                {
                    state.NextStandAttemptAt = state.LastStandInterruptedUntil + 0.25f;
                    continue;
                }

                if (state.PendingDeadRecovery || state.KnockoutActive || IsLegOrFootDisabled(state))
                {
                    state.NextStandAttemptAt = now + StandRetryIntervalSeconds;
                    continue;
                }

                if (creature.ragdoll.state == Ragdoll.State.Standing)
                {
                    state.NextStandAttemptAt = 0f;
                    state.StandAttemptCount = 0;
                    continue;
                }

                TryStandUpAfterRecovery(creature);
                state.StandAttemptCount++;
                state.LastTouchedTime = now;

                if (creature.ragdoll.state == Ragdoll.State.Standing)
                {
                    state.NextStandAttemptAt = 0f;
                    state.StandAttemptCount = 0;
                    continue;
                }

                if (state.StandAttemptCount >= MaxStandRetryAttempts)
                {
                    state.NextStandAttemptAt = 0f;
                    ELDMLog.Warn(
                        "stand_retry_exhausted creature=" + SafeCreatureName(creature) +
                        " attempts=" + state.StandAttemptCount,
                        verboseOnly: true);
                    continue;
                }

                state.NextStandAttemptAt = now + StandRetryIntervalSeconds;
            }
        }

        private void ProcessMovementEffects(float now)
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            int processed = 0;
            int budget = GetCreatureProcessBudget();
            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                processed++;
                if (processed > budget)
                {
                    LogPerformanceThrottle("movement", processed, budget);
                    break;
                }

                CreatureState state = entry.Value;
                Creature creature = state.Creature;
                if (creature == null)
                {
                    continue;
                }

                UpdateMovementEffects(state, forceFall: false);
            }
        }

        private void ProcessWeightMultipliers()
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            if (!ELDMModOptions.UseWeightSystem())
            {
                foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
                {
                    RestoreWeightMultiplier(entry.Value);
                    entry.Value.WeightApplied = false;
                    entry.Value.LastAppliedWeightMultiplier = 1f;
                }

                return;
            }

            float multiplier = ELDMModOptions.GetWeightMultiplier();
            int processed = 0;
            int budget = GetCreatureProcessBudget();
            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                processed++;
                if (processed > budget)
                {
                    LogPerformanceThrottle("weight", processed, budget);
                    break;
                }

                ApplyWeightMultiplier(entry.Value, multiplier);
            }
        }

        private void BeginKnockout(CreatureState state, float now, string reason)
        {
            if (state == null || !ELDMModOptions.KnockoutEnabled || !IsCreatureAlive(state.Creature))
            {
                return;
            }

            float duration = Mathf.Clamp(ELDMModOptions.GetKnockoutDurationSeconds(), 1f, 180f);
            float knockoutUntil = now + duration;
            bool wasActive = state.KnockoutActive;

            state.KnockoutActive = true;
            state.KnockoutUntil = wasActive ? Mathf.Max(state.KnockoutUntil, knockoutUntil) : knockoutUntil;
            state.LastTouchedTime = now;
            state.StandupSlowUntil = 0f;
            state.StandupBlendStartAt = 0f;
            state.StandupBlendEndAt = 0f;
            state.PendingEyeRestore = true;
            state.EyeRestoreAt = float.PositiveInfinity;
            if (!wasActive)
            {
                ApplyDeathPresentation(state, "knockout", shouldAutoRestoreAfterRecovery: true);
            }
            else
            {
                UpdateMockDeathVisualPhase(state, now, "knockout_extend");
            }

            ApplyKnockoutPose(state);
            ForceReleaseHeldItems(state.Creature);
            ApplyEyePresentation(state, state.ActiveEyeMode);
            UpdateMovementEffects(state, forceFall: true);

            ELDMLog.Info(
                (wasActive ? "knockout_extended" : "knockout_started") +
                " creature=" + SafeCreatureName(state.Creature) +
                " reason=" + reason +
                " duration=" + duration.ToString("0.0") + "s" +
                " until=" + state.KnockoutUntil.ToString("0.00") +
                " eyeMode=" + state.ActiveEyeMode +
                " mouthMode=" + state.ActiveMouthMode,
                verboseOnly: wasActive);
        }

        private void EndKnockout(CreatureState state, float now, string reason)
        {
            if (state == null || !state.KnockoutActive)
            {
                return;
            }

            bool wasDeathRecoveryKnockout = state.IsDeathRecoveryKnockout;
            state.KnockoutActive = false;
            state.KnockoutUntil = 0f;
            state.IsDeathRecoveryKnockout = false;
            state.MockDeathRollEyesAt = 0f;
            state.KnockoutSlowUntil = now + ELDMModOptions.GetKnockoutRecoverySlowDurationSeconds();
            float standupSlowDuration = Mathf.Clamp(ELDMModOptions.GetKnockoutRecoverySlowDurationSeconds() * 0.55f, 3f, 8f);
            state.StandupSlowUntil = now + standupSlowDuration;
            state.StandupBlendStartAt = now + StandupBlendStartSeconds;
            state.StandupBlendEndAt = Mathf.Min(state.StandupSlowUntil, state.StandupBlendStartAt + StandupBlendDurationSeconds);

            Creature creature = state.Creature;
            if (creature != null)
            {
                if (state.FullBodyPinClampApplied && creature.ragdoll != null)
                {
                    creature.ragdoll.ResetPinForce(true, false, (RagdollPart.Type)0);
                    ELDMLog.Info(
                        "pins_fullbody_reset creature=" + SafeCreatureName(creature) +
                        " reason=knockout_end",
                        verboseOnly: true);
                }

                if (wasDeathRecoveryKnockout && state.KeepRecoveredGazeEyes)
                {
                    ApplyRecoveredGazeEyePresentation(state, "dead_recovery_knockout_end");
                    RestoreMouthPresentation(state, force: false);
                }
                else if (wasDeathRecoveryKnockout && state.PendingEyeRestore)
                {
                    state.EyeRestoreAt = Mathf.Max(state.EyeRestoreAt, now + 0.80f);
                }
                else if (!state.PendingEyeRestore || state.EyeRestoreAt <= now || float.IsPositiveInfinity(state.EyeRestoreAt))
                {
                    RestoreEyePresentation(state);
                }
            }

            state.FullBodyPinClampApplied = false;

            for (int i = 0; i < AllLimbGroups.Length; i++)
            {
                LimbGroup group = AllLimbGroups[i];
                if (IsLimbDisabled(state, group))
                {
                    SetGroupPinsDisabled(state, group);
                }
            }

            if (!IsLegOrFootDisabled(state))
            {
                float desiredStandAt = now + 0.25f;
                if (state.NextStandAttemptAt <= 0f || state.NextStandAttemptAt < desiredStandAt)
                {
                    state.NextStandAttemptAt = desiredStandAt;
                }
                state.StandAttemptCount = 0;
                if (wasDeathRecoveryKnockout && creature != null)
                {
                    TryStandUpAfterRecovery(creature);
                }
            }

            UpdateMovementEffects(state, forceFall: false);

            ELDMLog.Info(
                "knockout_ended creature=" + SafeCreatureName(state.Creature) +
                " reason=" + reason +
                " at=" + now.ToString("0.00") +
                " koSlowUntil=" + state.KnockoutSlowUntil.ToString("0.00") +
                " koSlowPercent=" + (ELDMModOptions.GetKnockoutRecoverySlowDebuffRatio() * 100f).ToString("0") + "%");
        }

        private void ApplyKnockoutPose(CreatureState state)
        {
            if (state?.Creature == null || state.Creature.ragdoll == null)
            {
                return;
            }

            if (!state.FullBodyPinClampApplied)
            {
                state.Creature.ragdoll.SetPinForceMultiplier(0f, 0f, 0f, 0f, true, false, (RagdollPart.Type)0, null);
                state.FullBodyPinClampApplied = true;
                ELDMLog.Info(
                    "pins_fullbody_clamp creature=" + SafeCreatureName(state.Creature) +
                    " reason=knockout_pose",
                    verboseOnly: true);
            }

            if (state.Creature.ragdoll.state == Ragdoll.State.Standing)
            {
                state.Creature.ragdoll.SetState(Ragdoll.State.Destabilized, true);
            }
        }

        private static void TryStandUpAfterRecovery(Creature creature)
        {
            if (creature?.ragdoll == null || creature.state == Creature.State.Dead)
            {
                return;
            }

            if (creature.ragdoll.standingUp)
            {
                return;
            }

            if (creature.ragdoll.state != Ragdoll.State.Standing)
            {
                creature.ragdoll.StandUp();
            }
        }

        private void TryPlayPierceSlashHitSound(CreatureState state, CollisionInstance collisionInstance, DamageType damageType, float now, RagdollPart hitPart)
        {
            if (state == null || (damageType != DamageType.Pierce && damageType != DamageType.Slash))
            {
                return;
            }

            if (now < state.LastPierceSlashSoundTime + PierceSlashSoundMinIntervalSeconds)
            {
                return;
            }

            if (!TryPlayCollisionSound(collisionInstance))
            {
                ELDMLog.Diag(
                    "sfx_skip kind=pierce_slash reason=no_effect creature=" + SafeCreatureName(state.Creature) +
                    " part=" + SafePartName(hitPart),
                    verboseOnly: true);
                return;
            }

            state.LastPierceSlashSoundTime = now;
            ELDMLog.Diag(
                "sfx_play kind=pierce_slash creature=" + SafeCreatureName(state.Creature) +
                " part=" + SafePartName(hitPart),
                verboseOnly: true);
        }

        private void TryPlayBluntBreakSound(CreatureState state, CollisionInstance collisionInstance, float now, RagdollPart hitPart)
        {
            if (state == null)
            {
                return;
            }

            if (now < state.LastBluntBreakSoundTime + BluntBreakSoundMinIntervalSeconds)
            {
                return;
            }

            if (!TryPlayCollisionSound(collisionInstance))
            {
                ELDMLog.Diag(
                    "sfx_skip kind=blunt_break reason=no_effect creature=" + SafeCreatureName(state.Creature) +
                    " part=" + SafePartName(hitPart),
                    verboseOnly: true);
                return;
            }

            state.LastBluntBreakSoundTime = now;
            ELDMLog.Diag(
                "sfx_play kind=blunt_break creature=" + SafeCreatureName(state.Creature) +
                " part=" + SafePartName(hitPart),
                verboseOnly: true);
        }

        private static int DetermineDisablePassCountForHit(
            CollisionInstance collisionInstance,
            DamageType damageType,
            float trackedDamage,
            float accumulatedDamage,
            LimbGroup limbGroup,
            out string reason,
            out float thresholdDamage)
        {
            thresholdDamage = ELDMModOptions.GetLimbThresholdDamage(GroupToRepresentativePart(limbGroup));

            if (!ELDMModOptions.UseLimbDisableSystem())
            {
                reason = "limb_system_disabled";
                return 0;
            }

            if (!IsGroupHitEnabled(limbGroup))
            {
                reason = "group_disabled";
                return 0;
            }

            if (damageType == DamageType.Pierce)
            {
                if (limbGroup != LimbGroup.Torso)
                {
                    float passThreshold = Mathf.Max(1f, thresholdDamage);
                    int passCount = Mathf.Max(1, Mathf.FloorToInt(accumulatedDamage / passThreshold));
                    reason = "pierce_forced_disable";
                    return passCount;
                }

                if (trackedDamage >= thresholdDamage)
                {
                    float passThreshold = Mathf.Max(1f, thresholdDamage);
                    int passCount = Mathf.Max(1, Mathf.FloorToInt(accumulatedDamage / passThreshold));
                    reason = "pierce_torso_hit_gte_threshold";
                    return passCount;
                }

                reason = "pierce_torso_below_threshold";
                return 0;
            }

            bool bluntLike = IsBluntLikeDamageType(collisionInstance, damageType);
            bool slashLike = damageType == DamageType.Slash;
            if (!bluntLike && !slashLike)
            {
                reason = "damage_type_not_supported";
                return 0;
            }

            if (trackedDamage >= thresholdDamage)
            {
                float passThreshold = Mathf.Max(1f, thresholdDamage);
                int passCount = Mathf.Max(1, Mathf.FloorToInt(accumulatedDamage / passThreshold));
                reason = "hit_gte_threshold";
                return passCount;
            }

            reason = "below_threshold";
            return 0;
        }

        private static int DetermineKnockoutTier(
            CollisionInstance collisionInstance,
            DamageType damageType,
            float trackedDamage,
            LimbGroup limbGroup,
            RagdollPart hitPart)
        {
            if (!ELDMModOptions.KnockoutEnabled || collisionInstance == null)
            {
                return 0;
            }

            if (limbGroup != LimbGroup.Head && limbGroup != LimbGroup.Neck)
            {
                return 0;
            }

            if (!ELDMModOptions.IsHeadNeckKnockoutEnabled())
            {
                return 0;
            }

            bool bluntLike = IsBluntLikeDamageType(collisionInstance, damageType);
            bool slashLike = damageType == DamageType.Slash;
            if (!bluntLike && !slashLike)
            {
                return 0;
            }

            float threshold = ELDMModOptions.GetKnockoutTier1HitDamage();
            threshold *= 0.70f;

            if (trackedDamage >= threshold)
            {
                return 1;
            }

            if (bluntLike && collisionInstance.damageStruct.pushLevel >= 2 && trackedDamage >= 0.5f)
            {
                return 1;
            }

            if (ELDMLog.DiagnosticsEnabled && (limbGroup == LimbGroup.Head || limbGroup == LimbGroup.Neck))
            {
                ELDMLog.Diag(
                    "head_neck_ko_below_threshold part=" + SafePartName(hitPart) +
                    " damage=" + trackedDamage.ToString("0.00") +
                    " threshold=" + threshold.ToString("0.0"),
                    verboseOnly: true);
            }

            return 0;
        }

        private static bool IsGroupHitEnabled(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Hands:
                    return ELDMModOptions.ShouldDisableHands();
                case LimbGroup.Feet:
                    return ELDMModOptions.ShouldDisableFeet();
                case LimbGroup.WingLeft:
                case LimbGroup.WingRight:
                    return ELDMModOptions.ShouldDisableWings();
                case LimbGroup.Hip:
                    return ELDMModOptions.ShouldDisableTail();
                case LimbGroup.Head:
                case LimbGroup.Neck:
                    return ELDMModOptions.IsHeadNeckKnockoutEnabled();
                case LimbGroup.Torso:
                    return ELDMModOptions.ShouldLogTorsoPlaceholderHits();
                default:
                    return true;
            }
        }

        private static bool IsBluntLikeDamageType(CollisionInstance collisionInstance, DamageType damageType)
        {
            if (damageType == DamageType.Blunt)
            {
                return true;
            }

            if (damageType != DamageType.Unknown)
            {
                return false;
            }

            Damager damager = null;
            if (collisionInstance != null)
            {
                damager = collisionInstance.damageStruct.damager;
            }
            if (damager?.data?.damageModifierData != null && damager.data.damageModifierData.damageType == DamageType.Blunt)
            {
                return true;
            }

            return collisionInstance != null && collisionInstance.damageStruct.pushLevel >= 2;
        }

        private static bool IsKillDisqualifyingForRecovery(CollisionInstance collisionInstance, RagdollPart killPart)
        {
            if (!ELDMModOptions.IsExcludedDeadRecoveryPart(killPart))
            {
                return false;
            }

            DamageType damageType = collisionInstance != null ? collisionInstance.damageStruct.damageType : DamageType.Unknown;
            return !IsBluntLikeDamageType(collisionInstance, damageType);
        }

        private static bool TryPlayCollisionSound(CollisionInstance collisionInstance)
        {
            if (collisionInstance == null || collisionInstance.sourceMaterial == null || collisionInstance.targetMaterial == null)
            {
                return false;
            }

            EffectInstance effectInstance;
            bool spawned;
            try
            {
                spawned = collisionInstance.SpawnEffect(collisionInstance.sourceMaterial, collisionInstance.targetMaterial, false, out effectInstance);
            }
            catch
            {
                return false;
            }

            if (!spawned || effectInstance == null)
            {
                return false;
            }

            try
            {
                effectInstance.Play(0, false, false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ForceReleaseHeldItems(Creature creature)
        {
            if (creature == null)
            {
                return;
            }

            if (creature.handLeft != null)
            {
                creature.handLeft.TryRelease();
            }

            if (creature.handRight != null)
            {
                creature.handRight.TryRelease();
            }
        }

        private void ApplyInjuryBehaviorReaction(CreatureState state, LimbGroup group, int passCount, float now)
        {
            if (state == null || state.Creature == null || !ELDMModOptions.UseAiBehaviorReactions())
            {
                return;
            }

            float cooldownSeconds = ELDMModOptions.GetReactionCooldownSecondsForPart(GroupToRepresentativePart(group));
            if (!state.ReactionCooldownUntil.TryGetValue(group, out float nextAllowedAt))
            {
                nextAllowedAt = 0f;
            }

            if (cooldownSeconds > 0f && now < nextAllowedAt)
            {
                ELDMLog.Diag(
                    "behavior_reaction_cooldown creature=" + SafeCreatureName(state.Creature) +
                    " group=" + GroupName(group) +
                    " passCount=" + passCount +
                    " nextAllowedIn=" + (nextAllowedAt - now).ToString("0.00") + "s",
                    verboseOnly: true);
                return;
            }

            state.ReactionCooldownUntil[group] = now + cooldownSeconds;

            bool droppedItems = false;

            if (state.Creature.ragdoll != null && (group == LimbGroup.Legs || group == LimbGroup.Feet))
            {
                bool forceFall = ELDMModOptions.FallFromLegInjury;
                if (forceFall && state.Creature.ragdoll.state == Ragdoll.State.Standing)
                {
                    state.Creature.ragdoll.SetState(Ragdoll.State.Destabilized, true);
                }
            }

            float reactionDuration = 0f;
            if (ELDMModOptions.UseSlowDebuffSystem())
            {
                reactionDuration = ELDMModOptions.GetInjuryReactionDurationSeconds();
                float reactionFactor = Mathf.Clamp(1f - ELDMModOptions.GetInjuryReactionSlowRatio(), 0.05f, 1f);
                state.InjuryBehaviorSlowUntil = Mathf.Max(state.InjuryBehaviorSlowUntil, now + reactionDuration);
                state.InjuryBehaviorSlowFactor = Mathf.Min(state.InjuryBehaviorSlowFactor <= 0f ? 1f : state.InjuryBehaviorSlowFactor, reactionFactor);
            }

            ELDMLog.Info(
                "behavior_reaction creature=" + SafeCreatureName(state.Creature) +
                " group=" + GroupName(group) +
                " passCount=" + passCount +
                " droppedItems=" + droppedItems +
                " cooldown=" + cooldownSeconds.ToString("0.0") + "s" +
                " slowFactor=" + state.InjuryBehaviorSlowFactor.ToString("0.00") +
                " slowUntil=" + state.InjuryBehaviorSlowUntil.ToString("0.00"));
        }

        private static bool ReleaseHeldItemsForGroup(Creature creature, LimbGroup group)
        {
            if (creature == null)
            {
                return false;
            }

            bool released = false;
            switch (group)
            {
                case LimbGroup.WingLeft:
                    released |= TryReleaseHand(creature.handLeft);
                    break;
                case LimbGroup.WingRight:
                    released |= TryReleaseHand(creature.handRight);
                    break;
                default:
                    released |= TryReleaseHand(creature.handLeft);
                    released |= TryReleaseHand(creature.handRight);
                    break;
            }

            return released;
        }

        private static bool TryReleaseHand(RagdollHand hand)
        {
            if (hand == null)
            {
                return false;
            }

            hand.TryRelease();
            return true;
        }

        private static void ApplyWeightMultiplier(CreatureState state, float multiplier)
        {
            if (state == null)
            {
                return;
            }

            Creature creature = state.Creature;
            if (creature == null || creature.ragdoll == null || creature.ragdoll.parts == null)
            {
                return;
            }

            bool multiplierChanged = !state.WeightApplied || Mathf.Abs(state.LastAppliedWeightMultiplier - multiplier) >= 0.001f;
            int partsModified = 0;
            for (int i = 0; i < creature.ragdoll.parts.Count; i++)
            {
                RagdollPart part = creature.ragdoll.parts[i];
                if (part == null)
                {
                    continue;
                }

                if (part.physicBody == null || part.physicBody.rigidBody == null)
                {
                    continue;
                }

                Rigidbody rb = part.physicBody.rigidBody;
                if (!state.OriginalPartMasses.TryGetValue(rb, out float originalMass))
                {
                    originalMass = rb.mass;
                    state.OriginalPartMasses[rb] = originalMass;
                }

                rb.mass = originalMass * multiplier;
                partsModified++;
            }

            if (partsModified == 0)
            {
                if (!state.WeightApplyNoPartsLogged)
                {
                    ELDMLog.Info(
                        "weight_apply_skipped creature=" + SafeCreatureName(creature) +
                        " reason=no_rigidbody_parts");
                    state.WeightApplyNoPartsLogged = true;
                }
                return;
            }

            state.WeightApplied = true;
            state.WeightApplyNoPartsLogged = false;
            state.LastAppliedWeightMultiplier = multiplier;

            if (partsModified > 0 && multiplierChanged)
            {
                ELDMLog.Info(
                    "weight_applied creature=" + SafeCreatureName(creature) +
                    " multiplier=" + multiplier.ToString("0.00") +
                    " parts=" + partsModified);
            }
        }

        private void ApplyDeathAnimationDataSpeed(CreatureState state, float targetSpeed, string reason)
        {
            if (state?.Creature == null || state.Creature.brain == null || state.Creature.brain.instance == null)
            {
                return;
            }

            BrainModuleDeath deathModule = null;
            try
            {
                deathModule = state.Creature.brain.instance.GetModule<BrainModuleDeath>(false);
            }
            catch (Exception ex)
            {
                if (!state.MissingDeathAnimationModuleLogged)
                {
                    ELDMLog.Warn(
                        "death_anim_data_speed_skipped creature=" + SafeCreatureName(state.Creature) +
                        " reason=module_lookup_failed error=" + ex.Message);
                    state.MissingDeathAnimationModuleLogged = true;
                }
                return;
            }

            if (deathModule == null)
            {
                if (!state.MissingDeathAnimationModuleLogged && ELDMLog.DiagnosticsEnabled)
                {
                    ELDMLog.Diag(
                        "death_anim_data_speed_skipped creature=" + SafeCreatureName(state.Creature) +
                        " reason=missing_brain_module_death",
                        verboseOnly: true);
                    state.MissingDeathAnimationModuleLogged = true;
                }
                return;
            }

            state.MissingDeathAnimationModuleLogged = false;

            float clamped = Mathf.Clamp(targetSpeed, 0.05f, 5f);
            int clipsTouched = 0;
            int clipsChanged = 0;
            bool changed = false;
            changed |= ApplyAnimationDataSpeed(deathModule.defaultAnimationData, clamped, ref clipsTouched, ref clipsChanged);
            changed |= ApplyAnimationDataSpeed(deathModule.stabAnimationData, clamped, ref clipsTouched, ref clipsChanged);
            changed |= ApplyAnimationDataSpeed(deathModule.fireAnimationData, clamped, ref clipsTouched, ref clipsChanged);
            changed |= ApplyAnimationDataSpeed(deathModule.lightningAnimationData, clamped, ref clipsTouched, ref clipsChanged);

            bool speedChanged = !state.DeathAnimationDataSpeedApplied || Mathf.Abs(state.LastDeathAnimationDataSpeed - clamped) > 0.001f;
            state.DeathAnimationDataSpeedApplied = true;
            state.LastDeathAnimationDataSpeed = clamped;

            if (speedChanged || changed)
            {
                ELDMLog.Info(
                    "death_anim_data_speed_applied creature=" + SafeCreatureName(state.Creature) +
                    " speed=" + clamped.ToString("0.00") +
                    " clipsTouched=" + clipsTouched +
                    " clipsChanged=" + clipsChanged +
                    " reason=" + reason);
            }
        }

        private bool ApplyAnimationDataSpeed(AnimationData animationData, float speed, ref int clipsTouched, ref int clipsChanged)
        {
            if (animationData?.animationClips == null)
            {
                return false;
            }

            bool changed = false;
            for (int i = 0; i < animationData.animationClips.Count; i++)
            {
                AnimationData.Clip clip = animationData.animationClips[i];
                if (clip == null)
                {
                    continue;
                }

                clipsTouched++;
                if (!originalDeathAnimationClipSpeeds.ContainsKey(clip))
                {
                    originalDeathAnimationClipSpeeds[clip] = clip.animationSpeed;
                }

                if (Mathf.Abs(clip.animationSpeed - speed) <= 0.0001f)
                {
                    continue;
                }

                clip.animationSpeed = speed;
                clipsChanged++;
                changed = true;
            }

            return changed;
        }

        private void RestorePatchedDeathAnimationClipSpeeds()
        {
            if (originalDeathAnimationClipSpeeds.Count == 0)
            {
                return;
            }

            int restored = 0;
            foreach (KeyValuePair<AnimationData.Clip, float> entry in originalDeathAnimationClipSpeeds)
            {
                AnimationData.Clip clip = entry.Key;
                if (clip == null)
                {
                    continue;
                }

                if (Mathf.Abs(clip.animationSpeed - entry.Value) <= 0.0001f)
                {
                    continue;
                }

                clip.animationSpeed = entry.Value;
                restored++;
            }

            originalDeathAnimationClipSpeeds.Clear();
            if (restored > 0 && ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag("death_anim_data_speed_restored clips=" + restored, verboseOnly: true);
            }
        }

        private static void ApplyAnimatorSpeedOverride(CreatureState state, float targetSpeed, string reason)
        {
            if (state == null)
            {
                return;
            }

            Creature creature = state?.Creature;
            Animator animator = creature != null ? creature.animator : null;
            if (animator == null)
            {
                if (!state.MissingAnimatorLogged)
                {
                    ELDMLog.Warn(
                        "death_anim_speed_skipped creature=" + SafeCreatureName(creature) +
                        " reason=missing_animator");
                    state.MissingAnimatorLogged = true;
                }
                return;
            }

            state.MissingAnimatorLogged = false;
            if (!state.AnimatorSpeedOverrideApplied)
            {
                state.AnimatorOriginalSpeed = animator.speed;
                state.AnimatorSpeedOverrideApplied = true;
            }

            float clamped = Mathf.Clamp(targetSpeed, 0.05f, 5f);
            bool changed = Mathf.Abs(animator.speed - clamped) > 0.001f;
            animator.speed = clamped;
            bool dynamicApplied = false;
            if (TryResolveAnimatorDynamicSpeedParam(state, animator, creature, out int speedParamHash))
            {
                if (!state.AnimatorDynamicSpeedOverrideApplied)
                {
                    state.AnimatorDynamicSpeedOriginal = animator.GetFloat(speedParamHash);
                    state.AnimatorDynamicSpeedOverrideApplied = true;
                }

                float previousDynamic = animator.GetFloat(speedParamHash);
                if (Mathf.Abs(previousDynamic - clamped) > 0.001f)
                {
                    changed = true;
                }

                animator.SetFloat(speedParamHash, clamped);
                dynamicApplied = true;
                state.MissingAnimatorDynamicSpeedLogged = false;
            }
            else if (!state.MissingAnimatorDynamicSpeedLogged && ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "death_anim_speed_param_missing creature=" + SafeCreatureName(creature) +
                    " reason=" + reason,
                    verboseOnly: true);
                state.MissingAnimatorDynamicSpeedLogged = true;
            }

            if (changed)
            {
                ELDMLog.Info(
                    "death_anim_speed_applied creature=" + SafeCreatureName(creature) +
                    " speed=" + clamped.ToString("0.00") +
                    " dynamic=" + dynamicApplied +
                    " reason=" + reason);
            }
        }

        private static void ClearAnimatorSpeedOverride(CreatureState state, string reason, bool force)
        {
            if (state == null)
            {
                return;
            }

            Creature creature = state?.Creature;
            Animator animator = creature != null ? creature.animator : null;
            if (!state.AnimatorSpeedOverrideApplied && !state.AnimatorDynamicSpeedOverrideApplied && !force)
            {
                return;
            }

            if (animator != null)
            {
                float restoreSpeed = state.AnimatorOriginalSpeed > 0f ? state.AnimatorOriginalSpeed : 1f;
                bool changed = Mathf.Abs(animator.speed - restoreSpeed) > 0.001f;
                animator.speed = restoreSpeed;
                if (changed && ELDMLog.DiagnosticsEnabled)
                {
                    ELDMLog.Diag(
                        "death_anim_speed_restored creature=" + SafeCreatureName(creature) +
                        " speed=" + restoreSpeed.ToString("0.00") +
                        " reason=" + reason,
                        verboseOnly: true);
                }

                if (state.AnimatorDynamicSpeedOverrideApplied &&
                    state.AnimatorDynamicSpeedParamAvailable &&
                    state.AnimatorDynamicSpeedParamHash != 0)
                {
                    float restoreDynamic = state.AnimatorDynamicSpeedOriginal > 0f ? state.AnimatorDynamicSpeedOriginal : 1f;
                    animator.SetFloat(state.AnimatorDynamicSpeedParamHash, restoreDynamic);
                }
            }

            state.AnimatorSpeedOverrideApplied = false;
            state.AnimatorOriginalSpeed = 1f;
            state.AnimatorDynamicSpeedOverrideApplied = false;
            state.AnimatorDynamicSpeedOriginal = 1f;
            state.MissingAnimatorLogged = false;
            state.MissingAnimatorDynamicSpeedLogged = false;
        }

        private static bool TryResolveAnimatorDynamicSpeedParam(CreatureState state, Animator animator, Creature creature, out int hash)
        {
            hash = 0;
            if (state == null || animator == null)
            {
                return false;
            }

            if (state.AnimatorDynamicSpeedParamResolved)
            {
                if (!state.AnimatorDynamicSpeedParamAvailable)
                {
                    return false;
                }

                hash = state.AnimatorDynamicSpeedParamHash;
                return hash != 0;
            }

            int candidateA = 0;
            if (creature != null)
            {
                try
                {
                    FieldInfo field = typeof(Creature).GetField("hashDynamicSpeedMultiplier", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null && field.FieldType == typeof(int))
                    {
                        candidateA = (int)field.GetValue(creature);
                    }
                }
                catch
                {
                    candidateA = 0;
                }
            }
            int candidateB = Animator.StringToHash("DynamicSpeedMultiplier");
            int candidateC = Animator.StringToHash("dynamicSpeedMultiplier");
            int candidateD = Animator.StringToHash("SpeedMultiplier");

            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter parameter = parameters[i];
                if (parameter.type != AnimatorControllerParameterType.Float)
                {
                    continue;
                }

                int parameterHash = parameter.nameHash;
                if ((candidateA != 0 && parameterHash == candidateA) ||
                    parameterHash == candidateB ||
                    parameterHash == candidateC ||
                    parameterHash == candidateD)
                {
                    state.AnimatorDynamicSpeedParamResolved = true;
                    state.AnimatorDynamicSpeedParamAvailable = true;
                    state.AnimatorDynamicSpeedParamHash = parameterHash;
                    hash = parameterHash;
                    return true;
                }
            }

            state.AnimatorDynamicSpeedParamResolved = true;
            state.AnimatorDynamicSpeedParamAvailable = false;
            state.AnimatorDynamicSpeedParamHash = 0;
            return false;
        }

        private static void EnsureDeathDespawnWindow(CreatureState state, float deathAnimSpeed, float now)
        {
            if (state?.Creature == null)
            {
                return;
            }

            bool requireHoldForPendingRecovery = state.PendingDeadRecovery;
            if (!requireHoldForPendingRecovery && deathAnimSpeed >= 0.99f)
            {
                return;
            }

            if (now < state.DeathDespawnExtendedUntil)
            {
                return;
            }

            float delay = ComputeDeathAnimationHoldSeconds(deathAnimSpeed);
            try
            {
                state.Creature.CancelInvoke("Despawn");
                state.Creature.Despawn(delay);
                state.DeathDespawnExtendedUntil = now + Mathf.Max(0.75f, delay - 0.50f);
            }
            catch (Exception ex)
            {
                ELDMLog.Warn(
                    "death_anim_despawn_extend_failed creature=" + SafeCreatureName(state.Creature) +
                    " error=" + ex.Message);
            }
        }

        private static float ComputeDeathAnimationHoldSeconds(float deathAnimSpeed)
        {
            float clampedSpeed = Mathf.Max(0.10f, deathAnimSpeed);
            return Mathf.Clamp((3.6f / clampedSpeed) + 0.35f, 3.5f, 14f);
        }

        private static void DisableAllEyeClips(Creature creature)
        {
            if (creature?.eyeClips == null)
            {
                return;
            }

            for (int i = 0; i < creature.eyeClips.Count; i++)
            {
                CreatureData.EyeClip eyeClip = creature.eyeClips[i];
                if (eyeClip != null)
                {
                    eyeClip.active = false;
                }
            }
        }

        private static void ApplyEyeClipLock(CreatureState state, bool active)
        {
            Creature creature = state?.Creature;
            if (creature == null)
            {
                return;
            }

            if (active)
            {
                if (!state.EyeClipOverrideApplied)
                {
                    state.EyeClipAutoWasActive = creature.autoEyeClipsActive;
                    state.EyeClipOverrideApplied = true;
                    creature.autoEyeClipsActive = false;
                    DisableAllEyeClips(creature);
                }
                else if (creature.autoEyeClipsActive)
                {
                    creature.autoEyeClipsActive = false;
                }

                return;
            }

            if (!state.EyeClipOverrideApplied)
            {
                return;
            }

            creature.autoEyeClipsActive = state.EyeClipAutoWasActive;
            state.EyeClipOverrideApplied = false;
        }

        private static void SetEyesClosed(Creature creature, bool closed)
        {
            SetEyesClosedAmount(creature, closed ? 1f : 0f);
        }

        private static void SetEyesClosedAmount(Creature creature, float closeAmount)
        {
            if (creature == null || creature.allEyes == null)
            {
                return;
            }

            List<CreatureEye> eyes = creature.allEyes;
            for (int i = 0; i < eyes.Count; i++)
            {
                CreatureEye eye = eyes[i];
                if (eye == null)
                {
                    continue;
                }

                eye.closeAmount = closeAmount;
                eye.SetClose();
            }
        }

        private static EyeMode SelectRandomEyeMode()
        {
            return UnityEngine.Random.value < 0.5f ? EyeMode.Closed : EyeMode.Rolled;
        }

        private static void BeginMockDeathVisualWindow(CreatureState state, float now, float duration, string reason)
        {
            if (state == null)
            {
                return;
            }

            float rollLead = UnityEngine.Random.Range(2f, 4f);
            float windowDuration = Mathf.Max(0.25f, duration);
            bool startRolled = UnityEngine.Random.value < 0.55f;
            bool transitionToRoll = !startRolled && UnityEngine.Random.value < 0.90f;
            state.ActiveEyeMode = startRolled ? EyeMode.Rolled : EyeMode.Closed;
            state.RolledEyeClipApplied = false;
            state.MockDeathRollEyesAt = transitionToRoll ? now + Mathf.Max(0.35f, windowDuration - rollLead) : 0f;
            state.MockDeathClosedAmount = UnityEngine.Random.Range(0.78f, 1.0f);
            state.RolledEyeFallbackCloseAmount = UnityEngine.Random.Range(0.58f, 0.95f);
            if (ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "mock_death_visual_start creature=" + SafeCreatureName(state.Creature) +
                    " reason=" + reason +
                    " duration=" + windowDuration.ToString("0.00") +
                    " startMode=" + state.ActiveEyeMode +
                    " rollEyesIn=" + Mathf.Max(0f, state.MockDeathRollEyesAt - now).ToString("0.00") + "s" +
                    " closeAmount=" + state.MockDeathClosedAmount.ToString("0.00") +
                    " fallbackClose=" + state.RolledEyeFallbackCloseAmount.ToString("0.00"),
                    verboseOnly: true);
            }
        }

        private static void UpdateMockDeathVisualPhase(CreatureState state, float now, string reason)
        {
            if (state == null || state.ActiveEyeMode == EyeMode.Rolled || state.MockDeathRollEyesAt <= 0f || now < state.MockDeathRollEyesAt)
            {
                return;
            }

            state.ActiveEyeMode = EyeMode.Rolled;
            state.MockDeathRollEyesAt = 0f;
            state.RolledEyeClipApplied = false;
            if (ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "mock_death_visual_roll creature=" + SafeCreatureName(state.Creature) +
                    " reason=" + reason,
                    verboseOnly: true);
            }
        }

        private static void ApplyEyePresentation(CreatureState state, EyeMode mode)
        {
            if (state?.Creature == null || mode == EyeMode.None)
            {
                return;
            }

            if (!ELDMModOptions.UseMouthPresentationSystem())
            {
                return;
            }

            ApplyEyeClipLock(state, true);

            if (mode == EyeMode.Closed)
            {
                state.RolledEyeClipApplied = false;
                DisableAllEyeClips(state.Creature);
                float closeAmount = Mathf.Clamp(state.MockDeathClosedAmount > 0f ? state.MockDeathClosedAmount : 1f, 0.15f, 1f);
                SetEyesClosedAmount(state.Creature, closeAmount);
                return;
            }

            if (!state.RolledEyeClipApplied)
            {
                DisableAllEyeClips(state.Creature);
                state.RolledEyeClipApplied = TryPlayRolledEyeClip(state.Creature);
            }

            float fallbackClose = Mathf.Clamp(state.RolledEyeFallbackCloseAmount > 0f ? state.RolledEyeFallbackCloseAmount : 0.35f, MinRecoveredGazeCloseAmount, 0.95f);
            SetEyesClosedAmount(state.Creature, fallbackClose);
        }

        private static void MaybeRestoreEyes(CreatureState state, float now)
        {
            if (state == null || !state.PendingEyeRestore)
            {
                return;
            }

            if (state.KnockoutActive && state.IsDeathRecoveryKnockout)
            {
                return;
            }

            if (state.EyeRestoreAt > 0f && now < state.EyeRestoreAt)
            {
                return;
            }

            RestoreEyePresentation(state);
        }

        private static float ResolveRecoveredGazeCloseAmount(CreatureState state)
        {
            float configuredClose = 1f - ELDMModOptions.GetMockDeathRecoveredEyeOpenRatio();
            float mockDeathClose = Mathf.Clamp(
                state != null && state.MockDeathClosedAmount > 0f ? state.MockDeathClosedAmount : 0.60f,
                0.18f,
                1f);
            float slightlyMoreOpen = mockDeathClose - RecoveredGazeOpenOffset;
            float targetClose = Mathf.Min(configuredClose, slightlyMoreOpen);
            return Mathf.Clamp(targetClose, MinRecoveredGazeCloseAmount, MaxRecoveredGazeCloseAmount);
        }

        private static void ApplyRecoveredGazeEyePresentation(CreatureState state, string reason)
        {
            if (state?.Creature == null)
            {
                return;
            }

            if (!ELDMModOptions.UseMouthPresentationSystem())
            {
                ForceClearPresentation(state);
                return;
            }

            state.ActiveEyeMode = EyeMode.Rolled;
            state.MockDeathRollEyesAt = 0f;
            state.MockDeathClosedAmount = 1f;
            state.RolledEyeFallbackCloseAmount = Mathf.Clamp(
                state.RecoveredGazeCloseAmount > 0f ? state.RecoveredGazeCloseAmount : ResolveRecoveredGazeCloseAmount(state),
                MinRecoveredGazeCloseAmount,
                MaxRecoveredGazeCloseAmount);
            state.PendingEyeRestore = false;
            state.EyeRestoreAt = 0f;
            state.RolledEyeClipApplied = false;
            state.KeepRecoveredGazeEyes = false;
            ApplyEyePresentation(state, state.ActiveEyeMode);

            if (ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "mock_death_recovered_gaze creature=" + SafeCreatureName(state.Creature) +
                    " reason=" + reason +
                    " closeAmount=" + state.RolledEyeFallbackCloseAmount.ToString("0.00"),
                    verboseOnly: true);
            }
        }

        private static void RestoreEyePresentation(CreatureState state)
        {
            if (state == null || !state.PendingEyeRestore)
            {
                return;
            }

            ApplyEyeClipLock(state, false);
            DisableAllEyeClips(state.Creature);
            SetEyesClosedAmount(state.Creature, 0f);
            state.ActiveEyeMode = EyeMode.None;
            state.PendingEyeRestore = false;
            state.EyeRestoreAt = 0f;
            state.MockDeathRollEyesAt = 0f;
            state.MockDeathClosedAmount = 1f;
            state.RolledEyeFallbackCloseAmount = 0.35f;
            state.RolledEyeClipApplied = false;
            state.KeepRecoveredGazeEyes = false;
            state.RecoveredGazeCloseAmount = 0f;
            RestoreMouthPresentation(state, force: false);
        }

        private static void ForceClearPresentation(CreatureState state)
        {
            if (state == null)
            {
                return;
            }

            ApplyEyeClipLock(state, false);
            DisableAllEyeClips(state.Creature);
            SetEyesClosedAmount(state.Creature, 0f);
            state.ActiveEyeMode = EyeMode.None;
            state.PendingEyeRestore = false;
            state.EyeRestoreAt = 0f;
            state.MockDeathRollEyesAt = 0f;
            state.MockDeathClosedAmount = 1f;
            state.RolledEyeFallbackCloseAmount = 0.35f;
            state.RolledEyeClipApplied = false;
            state.KeepRecoveredGazeEyes = false;
            state.RecoveredGazeCloseAmount = 0f;
            RestoreMouthPresentation(state, force: true);
        }

        private static MouthMode SelectRandomMouthMode(out float chanceRoll, out float splitRoll)
        {
            chanceRoll = UnityEngine.Random.value;
            if (chanceRoll >= MouthPresentationChance)
            {
                splitRoll = 0f;
                return MouthMode.None;
            }

            splitRoll = UnityEngine.Random.value;
            if (splitRoll < 0.60f)
            {
                return MouthMode.Subtle;
            }

            if (splitRoll < 0.90f)
            {
                return MouthMode.Open;
            }

            return MouthMode.WideOpen;
        }

        private static float GetMouthFactorForMode(MouthMode mode)
        {
            switch (mode)
            {
                case MouthMode.Subtle:
                    return 0.30f;
                case MouthMode.Open:
                    return 0.60f;
                case MouthMode.WideOpen:
                    return 0.90f;
                default:
                    return 0f;
            }
        }

        private static float ResolvePresentationMouthFactor(CreatureState state, float fallbackFactor)
        {
            float modeFactor = GetMouthFactorForMode(state != null ? state.ActiveMouthMode : MouthMode.None);
            return modeFactor > 0.001f ? modeFactor : Mathf.Clamp01(fallbackFactor);
        }

        private static void MaybeApplyRandomMouthPresentation(CreatureState state, string reason, bool shouldAutoRestore)
        {
            if (state?.Creature == null)
            {
                return;
            }

            if (!ELDMModOptions.UseMouthPresentationSystem())
            {
                ForceClearPresentation(state);
                ELDMLog.Diag(
                    "mouth_roll_skipped creature=" + SafeCreatureName(state.Creature) +
                    " reason=" + reason +
                    " skip=system_disabled",
                    verboseOnly: true);
                return;
            }

            MouthMode mode = SelectRandomMouthMode(out float chanceRoll, out float splitRoll);
            state.ActiveMouthMode = mode;
            state.PendingMouthRestore = shouldAutoRestore && mode != MouthMode.None;

            ELDMLog.Info(
                "mouth_roll creature=" + SafeCreatureName(state.Creature) +
                " reason=" + reason +
                " mode=" + mode +
                " autoRestore=" + state.PendingMouthRestore +
                " chanceRoll=" + chanceRoll.ToString("0.000") +
                " chance=" + MouthPresentationChance.ToString("0.00") +
                " splitRoll=" + splitRoll.ToString("0.000"));

            if (mode == MouthMode.None)
            {
                RestoreMouthPresentation(state, force: false);
                return;
            }

            ApplyMouthPresentation(state, mode, reason);
        }

        private static void ApplyMouthPresentation(CreatureState state, MouthMode mode, string reason)
        {
            Creature creature = state?.Creature;
            if (creature == null || creature.jaw == null)
            {
                ELDMLog.Warn(
                    "mouth_apply_skipped creature=" + SafeCreatureName(creature) +
                    " reason=" + reason +
                    " mode=" + mode +
                    " skip=missing_jaw");
                return;
            }

            if (!state.JawOriginalLocalRotationCaptured)
            {
                state.JawOriginalLocalRotation = creature.jaw.localRotation;
                state.JawOriginalLocalRotationCaptured = true;
            }

            float factor = ResolvePresentationMouthFactor(state, DeathMouthOpenBaseFactor);
            Vector3 jawMax = creature.jawMaxRotation;
            if (jawMax.sqrMagnitude < 0.01f)
            {
                jawMax = new Vector3(-18f, 0f, 0f);
            }

            creature.jaw.localRotation = state.JawOriginalLocalRotation * Quaternion.Euler(jawMax * factor);

            ELDMLog.Info(
                "mouth_applied creature=" + SafeCreatureName(creature) +
                " reason=" + reason +
                " mode=" + mode +
                " jawMax=(" + jawMax.x.ToString("0.0") + "," + jawMax.y.ToString("0.0") + "," + jawMax.z.ToString("0.0") + ")" +
                " factor=" + factor.ToString("0.00"));
        }

        private static void MaybeRestoreMouth(CreatureState state, float now)
        {
            if (state == null || !state.PendingMouthRestore)
            {
                return;
            }

            if (state.EyeRestoreAt > 0f && now < state.EyeRestoreAt)
            {
                return;
            }

            RestoreMouthPresentation(state, force: false);
        }

        private static void RestoreMouthPresentation(CreatureState state, bool force)
        {
            if (state?.Creature == null)
            {
                return;
            }

            if (!force && !state.PendingMouthRestore && state.ActiveMouthMode == MouthMode.None)
            {
                return;
            }

            if (state.Creature.jaw != null && state.JawOriginalLocalRotationCaptured)
            {
                state.Creature.jaw.localRotation = state.JawOriginalLocalRotation;
            }

            if (ELDMLog.DiagnosticsEnabled && state.ActiveMouthMode != MouthMode.None)
            {
                ELDMLog.Diag(
                    "mouth_restored creature=" + SafeCreatureName(state.Creature) +
                    " mode=" + state.ActiveMouthMode,
                    verboseOnly: true);
            }

            state.ActiveMouthMode = MouthMode.None;
            state.PendingMouthRestore = false;
            state.LastKillAudioMouthFactor = 0f;
        }

        private static void ApplyDeathPresentation(CreatureState state, string reason, bool shouldAutoRestoreAfterRecovery)
        {
            if (state?.Creature == null)
            {
                return;
            }

            if (!ELDMModOptions.UseMouthPresentationSystem())
            {
                ForceClearPresentation(state);
                return;
            }

            float now = Time.unscaledTime;
            float visualDuration = Mathf.Max(ELDMModOptions.GetDeathRecoveryKnockoutDuration(), ELDMModOptions.GetKnockoutDurationSeconds());
            BeginMockDeathVisualWindow(state, now, visualDuration, reason);
            state.RolledEyeClipApplied = false;
            state.KeepRecoveredGazeEyes = false;
            state.RecoveredGazeCloseAmount = 0f;
            state.PendingEyeRestore = shouldAutoRestoreAfterRecovery;
            state.EyeRestoreAt = shouldAutoRestoreAfterRecovery ? float.PositiveInfinity : 0f;
            MaybeApplyRandomMouthPresentation(state, reason, shouldAutoRestoreAfterRecovery);
            if (!shouldAutoRestoreAfterRecovery)
            {
                state.PendingMouthRestore = false;
            }
            ApplyMouthOpenFactor(state, ResolvePresentationMouthFactor(state, 0f), reason + "_static");
            ApplyEyePresentation(state, state.ActiveEyeMode);

            ELDMLog.Info(
                "death_presentation_applied creature=" + SafeCreatureName(state.Creature) +
                " eyeMode=" + state.ActiveEyeMode +
                " mouthMode=" + state.ActiveMouthMode +
                " autoRestore=" + shouldAutoRestoreAfterRecovery +
                " reason=" + reason,
                verboseOnly: true);
        }

        private static void ApplyPermanentDeathEyePresentation(CreatureState state, string reason)
        {
            if (state?.Creature == null)
            {
                return;
            }

            if (state.PendingDeadRecovery)
            {
                return;
            }

            ApplyDeathPresentation(state, reason, shouldAutoRestoreAfterRecovery: false);
        }

        private static bool TryPlayRolledEyeClip(Creature creature)
        {
            if (creature == null || creature.eyeClips == null || creature.eyeClips.Count == 0)
            {
                return false;
            }

            string[] rankedTags = { "ROLL", "DEAD", "UP", "FAINT", "STUN", "SLEEP" };
            for (int t = 0; t < rankedTags.Length; t++)
            {
                string tag = rankedTags[t];
                for (int i = 0; i < creature.eyeClips.Count; i++)
                {
                    CreatureData.EyeClip eyeClip = creature.eyeClips[i];
                    if (eyeClip == null || string.IsNullOrWhiteSpace(eyeClip.clipName))
                    {
                        continue;
                    }

                    string clipNameUpper = eyeClip.clipName.ToUpperInvariant();
                    if (clipNameUpper.IndexOf(tag, StringComparison.Ordinal) >= 0)
                    {
                        creature.PlayEyeClip(eyeClip);
                        return true;
                    }
                }
            }

            return false;
        }

        private void CleanupStaleCreatures(float now)
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            List<int> staleIds = null;

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                CreatureState state = entry.Value;
                if (now - state.LastTouchedTime <= StaleCreatureStateSeconds)
                {
                    continue;
                }

                if (AnyLimbDisabled(state) ||
                    state.PendingDeadRecovery ||
                    state.KnockoutActive ||
                    state.PendingEyeRestore ||
                    state.PendingMouthRestore ||
                    now < state.DeadRecoverySlowUntil ||
                    now < state.KnockoutSlowUntil ||
                    now < state.StandupSlowUntil ||
                    now < state.LastStandInterruptedUntil ||
                    HasActiveHitSlowDebuff(state, now) ||
                    state.NextStandAttemptAt > 0f)
                {
                    continue;
                }

                if (staleIds == null)
                {
                    staleIds = new List<int>();
                }

                staleIds.Add(entry.Key);
            }

            if (staleIds == null)
            {
                return;
            }

            for (int i = 0; i < staleIds.Count; i++)
            {
                RemoveCreatureState(staleIds[i]);
            }
        }

        private void RefreshDisableTimer(CreatureState state, LimbGroup group, float now)
        {
            if (state == null || !state.Limbs.TryGetValue(group, out LimbState limbState) || !limbState.Disabled)
            {
                return;
            }

            if (limbState.PermanentDisabled)
            {
                return;
            }

            float baseDuration = ELDMModOptions.GetLimbDisableDurationSeconds(GroupToRepresentativePart(group));
            float duration = ComputeDisableDurationForPass(baseDuration, Mathf.Max(1, limbState.DisableCount));

            if (!IsFinite(duration))
            {
                return;
            }

            limbState.DisabledUntil = Mathf.Max(limbState.DisabledUntil, now + duration);
            state.Limbs[group] = limbState;
        }

        private void DisableLimbAtPassCount(CreatureState state, LimbGroup group, RagdollPart hitPart, CollisionInstance collisionInstance, DamageType damageType, float now, int passCount)
        {
            LimbState limbState = state.Limbs[group];
            int previousDisableCount = Mathf.Max(0, limbState.DisableCount);
            if (passCount <= previousDisableCount)
            {
                return;
            }

            int passesAdded = passCount - previousDisableCount;
            bool wasDisabled = limbState.Disabled;
            limbState.Disabled = true;
            if (previousDisableCount <= 0)
            {
                limbState.DamageAtDisable = limbState.AccumulatedDamage;
            }
            limbState.DisableCount = passCount;

            RagdollPart.Type repPart = GroupToRepresentativePart(group);
            float baseDuration = ELDMModOptions.GetLimbDisableDurationSeconds(repPart);
            float traumaMultiplier = 1f;
            bool permanentByTrauma = false;
            float stackedDuration = 0f;
            float lastPassDuration = 0f;
            for (int pass = previousDisableCount + 1; pass <= passCount; pass++)
            {
                float durationForPass = ComputeDisableDurationForPass(baseDuration, pass);
                if (ELDMModOptions.UseProgressiveTrauma())
                {
                    traumaMultiplier = ELDMInjurySynergyModel.ComputeTraumaDurationMultiplier(
                        pass,
                        ELDMModOptions.GetTraumaDurationIncreaseRatioPerDisable(),
                        ELDMModOptions.GetTraumaMaxDurationBonusRatio());
                    if (IsFinite(durationForPass))
                    {
                        durationForPass *= traumaMultiplier;
                    }

                    permanentByTrauma = ELDMInjurySynergyModel.ShouldBecomePermanentDisable(
                        pass,
                        ELDMModOptions.GetPermanentDisableAfterCount());
                }

                if (IsFinite(durationForPass))
                {
                    stackedDuration += durationForPass;
                    lastPassDuration = durationForPass;
                }
            }

            if (ELDMModOptions.UseProgressiveTrauma())
            {
                traumaMultiplier = ELDMInjurySynergyModel.ComputeTraumaDurationMultiplier(
                    passCount,
                    ELDMModOptions.GetTraumaDurationIncreaseRatioPerDisable(),
                    ELDMModOptions.GetTraumaMaxDurationBonusRatio());
                permanentByTrauma = ELDMInjurySynergyModel.ShouldBecomePermanentDisable(
                    passCount,
                    ELDMModOptions.GetPermanentDisableAfterCount());
            }

            if (group == LimbGroup.Hip)
            {
                stackedDuration = IsFinite(stackedDuration) ? Mathf.Clamp(stackedDuration * 0.85f, 4f, 600f) : float.PositiveInfinity;
                lastPassDuration = IsFinite(lastPassDuration) ? Mathf.Clamp(lastPassDuration * 0.85f, 4f, 300f) : float.PositiveInfinity;
            }

            if (permanentByTrauma)
            {
                limbState.PermanentDisabled = true;
                limbState.DisabledUntil = float.PositiveInfinity;
            }
            else if (IsFinite(stackedDuration))
            {
                float extendFrom = wasDisabled && IsFinite(limbState.DisabledUntil) && limbState.DisabledUntil > now
                    ? limbState.DisabledUntil
                    : now;
                limbState.DisabledUntil = extendFrom + stackedDuration;
            }
            else
            {
                limbState.DisabledUntil = float.PositiveInfinity;
            }

            state.Limbs[group] = limbState;

            if (group == LimbGroup.Torso)
            {
                state.TorsoParalyzedUntil = 0f;
                UpdateMovementEffects(state, forceFall: false);

                ELDMLog.Info(
                    "torso_injury creature=" + SafeCreatureName(state.Creature) +
                    " passes=" + passCount +
                    " staggerOnly=true" +
                    " slow=" + (ELDMModOptions.GetTorsoSlowDebuffRatio() * 100f).ToString("0") + "%");
            }

            if (IsLegStyleGroup(group))
            {
                SetGroupPinsDisabled(state, group);
                bool forceFallForGroup = (group == LimbGroup.Legs || group == LimbGroup.Feet) && ELDMModOptions.FallFromLegInjury;
                UpdateMovementEffects(state, forceFall: forceFallForGroup);
            }
            else if (IsArmStyleGroup(group))
            {
                if (group != LimbGroup.WingLeft && group != LimbGroup.WingRight)
                {
                    ApplyArmDisabled(state, group);
                }
                UpdateMovementEffects(state, forceFall: false);
            }
            else
            {
                SetGroupPinsDisabled(state, group);
                UpdateMovementEffects(state, forceFall: false);
            }

            ELDMTelemetry.RecordDisable();
            ELDMLog.Info(
                "limb_disabled creature=" + SafeCreatureName(state.Creature) +
                " group=" + GroupName(group) +
                " passCount=" + passCount +
                " newPasses=" + passesAdded +
                " accumulated=" + limbState.AccumulatedDamage.ToString("0.00") +
                " disableCount=" + limbState.DisableCount +
                " traumaMult=" + traumaMultiplier.ToString("0.00") +
                " permanent=" + limbState.PermanentDisabled +
                " addedDuration=" + (IsFinite(stackedDuration) ? stackedDuration.ToString("0.0") + "s" : "infinite") +
                " lastPassDuration=" + (IsFinite(lastPassDuration) ? lastPassDuration.ToString("0.0") + "s" : "infinite") +
                " sourcePart=" + SafePartName(hitPart) +
                " damageType=" + damageType);

            ApplyInjuryBehaviorReaction(state, group, passCount, now);

            if (!state.IsDeathRecoveryKnockout)
            {
                for (int pass = previousDisableCount + 1; pass <= passCount; pass++)
                {
                    PlayDisableSound(state, group, hitPart, collisionInstance, damageType, pass, now);
                }
                state.LastDisableSoundTime = now;
            }
        }
        private void PlayDisableSound(CreatureState state, LimbGroup group, RagdollPart hitPart, CollisionInstance collisionInstance, DamageType damageType, int passCount, float now)
        {
            if (state == null)
            {
                return;
            }

            Vector3 position = Vector3.zero;
            if (hitPart != null && hitPart.transform != null)
            {
                position = hitPart.transform.position;
            }
            else if (state.Creature != null && state.Creature.transform != null)
            {
                position = state.Creature.transform.position;
            }

            if (group == LimbGroup.Hip)
            {
                float duration = ELDMSoundManager.Instance.PlayDisableSound(position, DamageType.Blunt, Mathf.Max(1, passCount), collisionInstance);
                if (duration <= 0.01f)
                {
                    ELDMLog.Warn(
                        "sfx_disable_skip creature=" + SafeCreatureName(state.Creature) +
                        " group=" + GroupName(group) +
                        " reason=no_audio_played");
                }
            }
            else
            {
                float duration = ELDMSoundManager.Instance.PlayDisableSound(position, damageType, Mathf.Max(1, passCount), collisionInstance);
                if (duration <= 0.01f)
                {
                    ELDMLog.Warn(
                        "sfx_disable_skip creature=" + SafeCreatureName(state.Creature) +
                        " group=" + GroupName(group) +
                        " reason=no_audio_played");
                }
            }

            ELDMLog.Diag(
                "sfx_disable creature=" + SafeCreatureName(state.Creature) +
                " group=" + GroupName(group) +
                " damageType=" + damageType +
                " passCount=" + passCount +
                " part=" + SafePartName(hitPart),
                verboseOnly: true);
        }

        private void RecoverLimb(CreatureState state, LimbGroup group)
        {
            LimbState limbState = state.Limbs[group];
            if (!limbState.Disabled || limbState.PermanentDisabled)
            {
                return;
            }

            limbState.Disabled = false;
            limbState.DisabledUntil = 0f;
            limbState.AccumulatedDamage *= ELDMModOptions.GetRetainedDamageRatioAfterRecovery();
            state.Limbs[group] = limbState;
            if (group == LimbGroup.Torso)
            {
                state.TorsoParalyzedUntil = 0f;
            }

            if (ELDMModOptions.RecoveryRestoresPinForces)
            {
                RestorePinsAndHandlesForGroup(state, group);
            }
            else
            {
                ClearHandleBucket(state, group);
            }

            if (IsLegStyleGroup(group))
            {
                state.NextStandAttemptAt = Time.unscaledTime + 0.25f;
                state.StandAttemptCount = 0;
                UpdateMovementEffects(state, forceFall: false);
            }

            ELDMTelemetry.RecordRecover();
            ELDMLog.Info(
                "limb_recovered creature=" + SafeCreatureName(state.Creature) +
                " group=" + GroupName(group) +
                " retainedDamage=" + limbState.AccumulatedDamage.ToString("0.00"));
        }

        private void ApplyArmDisabled(CreatureState state, LimbGroup group)
        {
            SetGroupPinsDisabled(state, group);

            Creature creature = state.Creature;
            if (creature == null || creature.ragdoll == null || creature.ragdoll.parts == null)
            {
                return;
            }

            List<HandleRagdoll> bucket = GetHandleBucket(state, group);
            bucket.Clear();

            RagdollPart.Type mask = GroupToPinMask(group);
            bool useArmKeywordFallback = group == LimbGroup.Arms || group == LimbGroup.Hands;
            List<RagdollPart> parts = creature.ragdoll.parts;
            for (int i = 0; i < parts.Count; i++)
            {
                RagdollPart part = parts[i];
                if (part == null || part.handles == null)
                {
                    continue;
                }

                bool groupMatch = (part.type & mask) != 0 || (useArmKeywordFallback && LooksLikeArmPart(part));
                if (!groupMatch)
                {
                    continue;
                }

                List<HandleRagdoll> handles = part.handles;
                for (int j = 0; j < handles.Count; j++)
                {
                    HandleRagdoll handle = handles[j];
                    if (handle == null)
                    {
                        continue;
                    }

                    bucket.Add(handle);
                    handle.SetTouch(false);
                }
            }
        }

        private void UpdateMovementEffects(CreatureState state, bool forceFall)
        {
            Creature creature = state.Creature;
            if (creature == null)
            {
                return;
            }

            float now = Time.unscaledTime;
            bool presentationEnabled = ELDMModOptions.UseMouthPresentationSystem();
            if (presentationEnabled)
            {
                MaybeRestoreEyes(state, now);
                MaybeRestoreMouth(state, now);
            }
            else
            {
                ForceClearPresentation(state);
            }
            TryTriggerLastStandInterruption(state, now);
            bool creatureAlive = IsCreatureAlive(creature);
            bool deathAnimationSpeedEnabled = ELDMModOptions.UseDeathAnimationSpeedSystem();

            if (!creatureAlive)
            {
                if (deathAnimationSpeedEnabled)
                {
                    float deadAnimSpeed = ResolveDeathAnimationSpeedForState(state);
                    string deadReason = state.PendingDeadRecovery ? "pending_dead_recovery_queue" : "permadeath_incapacitated";
                    ApplyDeathAnimationDataSpeed(state, deadAnimSpeed, deadReason);
                    ClearAnimatorSpeedOverride(state, "dead_data_driven", force: false);
                    EnsureDeathDespawnWindow(state, deadAnimSpeed, now);
                }
                else
                {
                    ClearAnimatorSpeedOverride(state, "system_disabled", force: false);
                }

                Locomotion deadLocomotion = creature.currentLocomotion != null ? creature.currentLocomotion : creature.locomotion;
                if (state.LocomotionClampApplied && deadLocomotion != null)
                {
                    deadLocomotion.RemoveSpeedModifier(this);
                    state.LocomotionClampApplied = false;
                    state.LastAppliedSpeedMultiplier = 1f;
                }
                if (state.NoStandModifierApplied && creature.brain != null)
                {
                    creature.brain.RemoveNoStandUpModifier(this);
                }
                state.NoStandModifierApplied = false;

                if (presentationEnabled)
                {
                    UpdateMockDeathVisualPhase(state, now, "permadeath");
                    ApplyEyePresentation(state, state.ActiveEyeMode);
                    ApplyMouthOpenFactor(state, ResolvePresentationMouthFactor(state, 0f), "death_static");
                }

                return;
            }

            bool legsOrFeetDisabled = IsLimbDisabled(state, LimbGroup.Legs) || IsLimbDisabled(state, LimbGroup.Feet);
            bool legDisabled = IsAnyGroupDisabled(state, LegStyleGroups);
            bool armDisabled = IsAnyGroupDisabled(state, ArmStyleGroups);
            bool hipDisabled = IsLimbDisabled(state, LimbGroup.Hip);
            bool torsoDisabled = IsLimbDisabled(state, LimbGroup.Torso);
            bool torsoParalyzed = torsoDisabled && now < state.TorsoParalyzedUntil;
            bool knockoutActive = state.KnockoutActive && (ELDMModOptions.KnockoutEnabled || state.IsDeathRecoveryKnockout);
            bool slowDebuffsEnabled = ELDMModOptions.UseSlowDebuffSystem();
            bool hasKnockoutRecoverySlow = slowDebuffsEnabled && now < state.KnockoutSlowUntil;
            bool hasDeadRecoverySlow = slowDebuffsEnabled && now < state.DeadRecoverySlowUntil;
            bool hasBehaviorSlow = slowDebuffsEnabled && now < state.InjuryBehaviorSlowUntil;
            bool pendingDeadRecovery = state.PendingDeadRecovery;
            bool isInDeathRecoveryKnockout = knockoutActive && state.IsDeathRecoveryKnockout;
            ELDMConflictResolution conflict = ELDMInjurySynergyModel.ResolveConflict(new ELDMConflictInput
            {
                PendingDeadRecovery = pendingDeadRecovery,
                KnockoutActive = knockoutActive,
                HipDisabled = hipDisabled,
                TorsoParalyzed = torsoParalyzed,
                LegDisabled = legsOrFeetDisabled,
                LegImmobilizationEnabled = ELDMModOptions.LegImmobilization,
            });
            bool interruptionActive = now < state.LastStandInterruptedUntil;
            bool standupAnimationActive = creature.ragdoll != null && creature.ragdoll.standingUp;
            bool applyDeathAnimSpeedOverride = deathAnimationSpeedEnabled && knockoutActive;
            float animSpeed = 1f;
            string animSpeedReason = "none";
            if (pendingDeadRecovery)
            {
                animSpeed = ResolveDeathAnimationSpeedForState(state);
                animSpeedReason = "pending_dead_recovery";
            }
            else if (knockoutActive)
            {
                animSpeed = ResolveKnockoutAnimationSpeedForState();
                animSpeedReason = isInDeathRecoveryKnockout ? "death_recovery_knockout" : "knockout";
            }
            bool hasHitSlowDebuff = HasActiveHitSlowDebuff(state, now);
            bool shouldApplySpeedClamp = legDisabled || armDisabled || torsoDisabled || hipDisabled || knockoutActive || hasKnockoutRecoverySlow || hasDeadRecoverySlow || hasBehaviorSlow || hasHitSlowDebuff || interruptionActive;

            Locomotion locomotion = creature.currentLocomotion != null ? creature.currentLocomotion : creature.locomotion;
            if (shouldApplySpeedClamp && locomotion != null)
            {
                state.MissingLocomotionLogged = false;
                string speedDetails = null;
                float speed = knockoutActive
                    ? 0f
                    : CalculateSpeedMultiplier(state, legsOrFeetDisabled, armDisabled, torsoDisabled, hipDisabled, hasKnockoutRecoverySlow, hasDeadRecoverySlow, hasBehaviorSlow, now, ELDMLog.DiagnosticsEnabled, out speedDetails);
                if (conflict.HardLockSpeed)
                {
                    speed = 0f;
                    if (ELDMLog.DiagnosticsEnabled)
                    {
                        speedDetails = pendingDeadRecovery
                            ? "pending_dead_recovery"
                            : (hipDisabled ? "hip_disabled" : (torsoParalyzed ? "torso_paralyzed" : "conflict_hardlock"));
                    }
                }
                else if (knockoutActive && ELDMLog.DiagnosticsEnabled)
                {
                    speedDetails = "knockout_active";
                }
                speed = Mathf.Clamp(speed, 0f, 1f);

                // Prevent sliding: force speed to 0 while creature is not yet standing
                // and has pending stand retries (e.g. just resurrected from dead recovery).
                if (!knockoutActive && speed > 0f && state.NextStandAttemptAt > 0f &&
                    creature.ragdoll != null && creature.ragdoll.state != Ragdoll.State.Standing)
                {
                    speed = Mathf.Min(speed, 0.35f);
                }

                locomotion.SetSpeedModifier(this, speed, speed, speed, speed, 1f, 1f);
                state.LocomotionClampApplied = true;
                if (ELDMLog.DiagnosticsEnabled &&
                    (Mathf.Abs(speed - state.LastAppliedSpeedMultiplier) > 0.02f ||
                     Mathf.Abs(animSpeed - state.LastAppliedAnimSpeedMultiplier) > 0.02f))
                {
                    ELDMLog.Diag(
                        "speed_modifier creature=" + SafeCreatureName(creature) +
                        " speed=" + speed.ToString("0.00") +
                        " legDisabled=" + legDisabled +
                        " armDisabled=" + armDisabled +
                        " hipDisabled=" + hipDisabled +
                        " torsoDisabled=" + torsoDisabled +
                        " torsoParalyzed=" + torsoParalyzed +
                        " knockoutActive=" + knockoutActive +
                        " knockoutSlow=" + hasKnockoutRecoverySlow +
                        " deadSlow=" + hasDeadRecoverySlow +
                        " behaviorSlow=" + hasBehaviorSlow +
                        " deadPending=" + pendingDeadRecovery +
                        " animSpeed=" + animSpeed.ToString("0.00") +
                        " animReason=" + animSpeedReason +
                        " details=" + (speedDetails ?? "none"));
                }
                state.LastAppliedSpeedMultiplier = speed;
                state.LastAppliedAnimSpeedMultiplier = animSpeed;
            }
            else if (shouldApplySpeedClamp && locomotion == null)
            {
                if (!state.MissingLocomotionLogged && ELDMLog.DiagnosticsEnabled)
                {
                    ELDMLog.Diag(
                        "speed_modifier_skipped creature=" + SafeCreatureName(creature) +
                        " reason=missing_locomotion" +
                        " legDisabled=" + legDisabled +
                        " armDisabled=" + armDisabled +
                        " hipDisabled=" + hipDisabled +
                        " torsoDisabled=" + torsoDisabled +
                        " torsoParalyzed=" + torsoParalyzed +
                        " knockoutActive=" + knockoutActive +
                        " knockoutSlow=" + hasKnockoutRecoverySlow +
                        " deadSlow=" + hasDeadRecoverySlow +
                        " behaviorSlow=" + hasBehaviorSlow +
                        " deadPending=" + pendingDeadRecovery,
                        verboseOnly: true);
                    state.MissingLocomotionLogged = true;
                }
            }
            else if (state.LocomotionClampApplied && locomotion != null)
            {
                state.MissingLocomotionLogged = false;
                if (ELDMLog.DiagnosticsEnabled && Mathf.Abs(state.LastAppliedSpeedMultiplier - 1f) > 0.02f)
                {
                    ELDMLog.Diag(
                        "speed_modifier_cleared creature=" + SafeCreatureName(creature) +
                        " previous=" + state.LastAppliedSpeedMultiplier.ToString("0.00"));
                }
                locomotion.RemoveSpeedModifier(this);
                state.LocomotionClampApplied = false;
                state.LastAppliedSpeedMultiplier = 1f;
                state.LastAppliedAnimSpeedMultiplier = 1f;
            }

            if (applyDeathAnimSpeedOverride)
            {
                ApplyAnimatorSpeedOverride(state, animSpeed, animSpeedReason);
            }
            else if (interruptionActive)
            {
                // Hold get-up motion almost static while a failed revive standup is active.
                ApplyAnimatorSpeedOverride(state, 0.05f, "revive_standup_freeze");
                state.LastAppliedAnimSpeedMultiplier = 0.05f;
            }
            else if (standupAnimationActive && !knockoutActive && now < state.StandupSlowUntil)
            {
                float standupAnimSpeed = Mathf.Clamp(ELDMModOptions.GetMockDeathAnimationSpeedMultiplier(), 0.05f, 1f);
                ApplyAnimatorSpeedOverride(state, standupAnimSpeed, "standup_slow");
                state.LastAppliedAnimSpeedMultiplier = standupAnimSpeed;
            }
            else if (shouldApplySpeedClamp && !knockoutActive && state.LastAppliedSpeedMultiplier < 0.999f)
            {
                bool standupPhase = now < state.StandupSlowUntil;
                float capFloor = standupPhase ? (1f - StandupSlowReductionCap) : (1f - CombatSlowReductionCap);
                float combatAnimSpeed = Mathf.Clamp(state.LastAppliedSpeedMultiplier, capFloor, 1f);
                if (standupPhase)
                {
                    float standupAnimCap = Mathf.Clamp(ELDMModOptions.GetMockDeathAnimationSpeedMultiplier(), 0.05f, 1f);
                    combatAnimSpeed = Mathf.Min(combatAnimSpeed, standupAnimCap);
                }
                ApplyAnimatorSpeedOverride(state, combatAnimSpeed, "combat_slow");
                state.LastAppliedAnimSpeedMultiplier = combatAnimSpeed;
            }
            else
            {
                ClearAnimatorSpeedOverride(state, "inactive", force: false);
                state.LastAppliedAnimSpeedMultiplier = 1f;
            }

            bool shouldPreventStand = conflict.PreventStand || interruptionActive;
            if (shouldPreventStand)
            {
                if (creature.brain != null && !state.NoStandModifierApplied)
                {
                    creature.brain.AddNoStandUpModifier(this);
                    state.NoStandModifierApplied = true;
                }

                if ((forceFall || conflict.ForceDestabilize || interruptionActive) && creature.ragdoll != null && creature.ragdoll.state == Ragdoll.State.Standing)
                {
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized, !interruptionActive);
                }
            }
            else
            {
                if (state.NoStandModifierApplied && creature.brain != null)
                {
                    creature.brain.RemoveNoStandUpModifier(this);
                }

                state.NoStandModifierApplied = false;
            }

            if (legDisabled)
            {
                ApplyLegPinState(state);
            }
            else if (!knockoutActive)
            {
                if (ELDMModOptions.RecoveryRestoresPinForces)
                {
                    for (int i = 0; i < LegStyleGroups.Length; i++)
                    {
                        RestorePinsAndHandlesForGroup(state, LegStyleGroups[i]);
                    }
                }
            }

            if (torsoParalyzed && creature.ragdoll != null)
            {
                creature.ragdoll.SetState(Ragdoll.State.Destabilized, false);
            }

            if (!hasBehaviorSlow && state.InjuryBehaviorSlowFactor < 0.999f)
            {
                state.InjuryBehaviorSlowFactor = 1f;
            }

            if (presentationEnabled && (pendingDeadRecovery || isInDeathRecoveryKnockout))
            {
                ApplyMouthOpenFactor(state, ResolvePresentationMouthFactor(state, 0f), "mimic_hold");
            }
        }

        private float CalculateSpeedMultiplier(
            CreatureState state,
            bool legsOrFeetDisabled,
            bool armDisabled,
            bool torsoDisabled,
            bool hipDisabled,
            bool hasKnockoutRecoverySlow,
            bool hasDeadRecoverySlow,
            bool hasBehaviorSlow,
            float now,
            bool captureDetails,
            out string details)
        {
            details = null;
            if (!ELDMModOptions.UseSlowDebuffSystem())
            {
                if (legsOrFeetDisabled && ELDMModOptions.LegImmobilization)
                {
                    if (captureDetails)
                    {
                        details = "leg_immobilization";
                    }

                    return 0f;
                }

                if (captureDetails)
                {
                    details = "slow_system_disabled";
                }

                return 1f;
            }

            StringBuilder sb = captureDetails ? new StringBuilder(128) : null;
            float combatReduction = ComputeCombatReductionFromHitStacks(state, now, captureDetails, sb);

            if (legsOrFeetDisabled)
            {
                if (captureDetails)
                {
                    sb.Append("legs/feet ");
                }
                combatReduction += 0.08f;
            }

            if (armDisabled)
            {
                if (captureDetails)
                {
                    sb.Append("arms ");
                }
                combatReduction += 0.05f;
            }

            if (torsoDisabled)
            {
                combatReduction += 0.07f;
                if (captureDetails) sb.Append("torso ");
            }

            if (hipDisabled)
            {
                combatReduction += 0.06f;
                if (captureDetails) sb.Append("hip ");
            }

            if (hasBehaviorSlow)
            {
                float behaviorReduction = Mathf.Clamp01(1f - Mathf.Clamp(state.InjuryBehaviorSlowFactor, 0.05f, 1f));
                combatReduction += behaviorReduction;
                if (captureDetails) sb.Append("behavior=").Append(behaviorReduction.ToString("0.00")).Append(' ');
            }

            combatReduction = Mathf.Clamp(combatReduction, 0f, CombatSlowReductionCap);
            float totalReduction = combatReduction;
            bool standupSlowActive = now < state.StandupSlowUntil;
            if (standupSlowActive)
            {
                float recoveryReduction = 0f;
                if (hasKnockoutRecoverySlow)
                {
                    recoveryReduction = Mathf.Max(recoveryReduction, ELDMModOptions.GetKnockoutRecoverySlowDebuffRatio());
                }
                if (hasDeadRecoverySlow)
                {
                    recoveryReduction = Mathf.Max(recoveryReduction, ELDMModOptions.GetDeadRecoverySlowDebuffRatio());
                }

                float standupReduction = Mathf.Clamp(Mathf.Max(combatReduction, recoveryReduction), 0f, StandupSlowReductionCap);
                if (state.StandupBlendStartAt > 0f && now >= state.StandupBlendStartAt)
                {
                    float blendT = state.StandupBlendEndAt > state.StandupBlendStartAt
                        ? Mathf.Clamp01((now - state.StandupBlendStartAt) / Mathf.Max(0.01f, state.StandupBlendEndAt - state.StandupBlendStartAt))
                        : 1f;
                    standupReduction = Mathf.Lerp(standupReduction, combatReduction, blendT);
                }

                totalReduction = Mathf.Clamp(standupReduction, 0f, StandupSlowReductionCap);
                if (captureDetails)
                {
                    sb.Append("standup=").Append(totalReduction.ToString("0.00")).Append(' ');
                }
            }
            else if (captureDetails)
            {
                sb.Append("combat=").Append(combatReduction.ToString("0.00")).Append(' ');
            }

            if (captureDetails)
            {
                details = sb.ToString();
            }

            return Mathf.Clamp01(1f - totalReduction);
        }

        private float ComputeCombatReductionFromHitStacks(CreatureState state, float now, bool captureDetails, StringBuilder sb)
        {
            if (state == null)
            {
                return 0f;
            }

            float reduction = 0f;
            for (int i = 0; i < AllLimbGroups.Length; i++)
            {
                LimbGroup group = AllLimbGroups[i];
                if (!state.HitSlowStacks.TryGetValue(group, out int stacks) || stacks <= 0)
                {
                    continue;
                }

                if (!state.HitSlowExpireAt.TryGetValue(group, out float expireAt) || expireAt <= now)
                {
                    state.HitSlowStacks[group] = 0;
                    state.HitSlowExpireAt[group] = 0f;
                    continue;
                }

                float contribution = GetHitSlowPerStack(group) * stacks;
                reduction += contribution;
                if (captureDetails)
                {
                    sb.Append(GroupName(group))
                        .Append('=')
                        .Append(contribution.ToString("0.00"))
                        .Append("(x")
                        .Append(stacks)
                        .Append(") ");
                }
            }

            return reduction;
        }

        private static float GetHitSlowPerStack(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Torso:
                    return 0.07f;
                case LimbGroup.Hip:
                    return 0.06f;
                case LimbGroup.WingLeft:
                case LimbGroup.WingRight:
                    return 0.04f;
                case LimbGroup.Legs:
                    return 0.05f;
                case LimbGroup.Feet:
                    return 0.035f;
                case LimbGroup.Arms:
                    return 0.03f;
                case LimbGroup.Hands:
                    return 0.025f;
                case LimbGroup.Neck:
                    return 0.04f;
                case LimbGroup.Head:
                    return 0.03f;
                default:
                    return 0.03f;
            }
        }

        private static int GetHitSlowStackCap(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Torso:
                case LimbGroup.Hip:
                    return 4;
                case LimbGroup.WingLeft:
                case LimbGroup.WingRight:
                    return 3;
                default:
                    return 5;
            }
        }

        private static float GetHitSlowDurationSeconds(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Torso:
                case LimbGroup.Hip:
                    return 14f;
                case LimbGroup.WingLeft:
                case LimbGroup.WingRight:
                    return 10f;
                case LimbGroup.Neck:
                case LimbGroup.Head:
                    return 9f;
                default:
                    return 12f;
            }
        }

        private void RegisterHitSlowDebuff(CreatureState state, LimbGroup group, float trackedDamage, float thresholdDamage, float now)
        {
            if (state == null || !ELDMModOptions.UseSlowDebuffSystem())
            {
                return;
            }

            if (trackedDamage < Mathf.Max(1f, thresholdDamage))
            {
                return;
            }

            int currentStacks = state.HitSlowStacks.TryGetValue(group, out int existingStacks) ? existingStacks : 0;
            float expireAt = state.HitSlowExpireAt.TryGetValue(group, out float existingExpireAt) ? existingExpireAt : 0f;
            if (expireAt <= now)
            {
                currentStacks = 0;
            }

            int newStacks = Mathf.Clamp(currentStacks + 1, 1, GetHitSlowStackCap(group));
            float newExpireAt = now + GetHitSlowDurationSeconds(group);
            state.HitSlowStacks[group] = newStacks;
            state.HitSlowExpireAt[group] = Mathf.Max(expireAt, newExpireAt);

            if (ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "hit_slow_stack creature=" + SafeCreatureName(state.Creature) +
                    " group=" + GroupName(group) +
                    " stacks=" + newStacks + "/" + GetHitSlowStackCap(group) +
                    " perStack=" + (GetHitSlowPerStack(group) * 100f).ToString("0.0") + "%" +
                    " expireAt=" + state.HitSlowExpireAt[group].ToString("0.00"),
                    verboseOnly: true);
            }
        }

        private static bool HasActiveHitSlowDebuff(CreatureState state, float now)
        {
            if (state == null)
            {
                return false;
            }

            for (int i = 0; i < AllLimbGroups.Length; i++)
            {
                LimbGroup group = AllLimbGroups[i];
                if (!state.HitSlowStacks.TryGetValue(group, out int stacks) || stacks <= 0)
                {
                    continue;
                }

                if (state.HitSlowExpireAt.TryGetValue(group, out float expireAt) && expireAt > now)
                {
                    return true;
                }
            }

            return false;
        }

        private static LimbGroup ResolveDeathAnimationGroup(CreatureState state, RagdollPart killPart, float now)
        {
            if (killPart != null && TryMapPartToGroup(killPart, out LimbGroup killGroup))
            {
                return killGroup;
            }

            if (state != null && state.HasLastInjuryGroup && now <= state.LastInjuryAt + 12f)
            {
                return state.LastInjuryGroup;
            }

            return LimbGroup.Torso;
        }

        private float ResolveDeathAnimationSpeedForState(CreatureState state)
        {
            float baseSpeed = Mathf.Clamp(ELDMModOptions.GetDeathAnimationSpeedMultiplier(), 0.05f, 5f);
            LimbGroup group = state != null ? state.LastDeathInjuryGroup : LimbGroup.Torso;
            float partFactor = ResolveDeathAnimationPartFactor(group);
            return Mathf.Max(MinimumRecoveryDeathAnimSpeed, Mathf.Clamp(baseSpeed * partFactor, 0.05f, 5f));
        }

        private float ResolveKnockoutAnimationSpeedForState()
        {
            float configuredMockSpeed = Mathf.Clamp(ELDMModOptions.GetMockDeathAnimationSpeedMultiplier(), 0.05f, 2f);
            return Mathf.Clamp(configuredMockSpeed, 0.05f, 1f);
        }

        private static float ResolveDeathAnimationPartFactor(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Torso:
                    return 0.55f;
                case LimbGroup.Hip:
                    return 0.65f;
                case LimbGroup.WingLeft:
                case LimbGroup.WingRight:
                    return 0.70f;
                case LimbGroup.Legs:
                    return 0.80f;
                case LimbGroup.Feet:
                    return 0.85f;
                case LimbGroup.Arms:
                    return 0.90f;
                case LimbGroup.Hands:
                    return 0.95f;
                case LimbGroup.Neck:
                    return 0.75f;
                case LimbGroup.Head:
                    return 0.88f;
                default:
                    return 0.90f;
            }
        }

        private void TryTriggerLastStandInterruption(CreatureState state, float now)
        {
            if (state == null ||
                !state.PendingLastStandInterruption ||
                state.LastStandInterruptionTriggered ||
                now < state.LastStandInterruptionAt ||
                state.Creature == null ||
                !IsCreatureAlive(state.Creature))
            {
                return;
            }

            Ragdoll ragdoll = state.Creature.ragdoll;
            if (ragdoll == null)
            {
                return;
            }

            // Only fail the revive while the NPC is actually trying to stand up.
            // This makes the interruption read as a failed get-up instead of a random cancel.
            bool standupInProgress = ragdoll.standingUp;
            bool fastStandFallback = !standupInProgress &&
                                     ragdoll.state == Ragdoll.State.Standing &&
                                     now >= state.LastStandInterruptionAt + 0.15f;
            if (!standupInProgress && !fastStandFallback)
            {
                return;
            }

            state.LastStandInterruptionTriggered = true;
            state.LastStandInterruptedUntil = now + LastStandInterruptedKneelSeconds;
            state.NextStandAttemptAt = state.LastStandInterruptedUntil + 0.25f;
            state.StandAttemptCount = 0;
            state.StandupSlowUntil = Mathf.Max(state.StandupSlowUntil, now + DefaultStandupSlowSeconds);
            state.StandupBlendStartAt = now + StandupBlendStartSeconds;
            state.StandupBlendEndAt = Mathf.Min(state.StandupSlowUntil, state.StandupBlendStartAt + StandupBlendDurationSeconds);

            ragdoll.SetState(Ragdoll.State.Destabilized, false);

            ELDMLog.Info(
                "last_stand_failed_standup creature=" + SafeCreatureName(state.Creature) +
                " standupInProgress=" + standupInProgress +
                " until=" + state.LastStandInterruptedUntil.ToString("0.00") +
                "s");
        }

        private int GetCreatureProcessBudget()
        {
            if (!ELDMModOptions.UsePerformanceSafeguards())
            {
                return int.MaxValue;
            }

            return ELDMModOptions.GetMaxCreaturesPerUpdate();
        }

        private void LogPerformanceThrottle(string stage, int processed, int budget)
        {
            if (!ELDMLog.DiagnosticsEnabled)
            {
                return;
            }

            float now = Time.unscaledTime;
            if (now < lastPerformanceThrottleLogAt + PerformanceThrottleLogCooldownSeconds)
            {
                return;
            }

            lastPerformanceThrottleLogAt = now;
            ELDMLog.Diag(
                "perf_throttle stage=" + stage +
                " processed=" + processed +
                " budget=" + budget +
                " tracked=" + trackedCreatures.Count,
                verboseOnly: true);
        }

        private void ApplyLegPinState(CreatureState state)
        {
            if (!ELDMModOptions.LegImmobilization)
            {
                return;
            }

            if (IsAnyLegStyleDisabled(state))
            {
                for (int i = 0; i < LegStyleGroups.Length; i++)
                {
                    if (IsLimbDisabled(state, LegStyleGroups[i]))
                    {
                        SetGroupPinsDisabled(state, LegStyleGroups[i]);
                    }
                }
            }
            else if (ELDMModOptions.RecoveryRestoresPinForces)
            {
                for (int i = 0; i < LegStyleGroups.Length; i++)
                {
                    RestorePinsAndHandlesForGroup(state, LegStyleGroups[i]);
                }
            }
        }

        private void SetGroupPinsDisabled(CreatureState state, LimbGroup group)
        {
            if (state?.Creature == null || state.Creature.ragdoll == null)
            {
                return;
            }

            bool dismemberedArmOrHand = (group == LimbGroup.Arms || group == LimbGroup.Hands) && IsGroupDismembered(state, group);
            bool shouldDisablePins = ShouldUsePinDisable(group) || dismemberedArmOrHand;
            if (!shouldDisablePins)
            {
                return;
            }

            float limpnessPercent = ELDMModOptions.GetLimpnessPercentForPart(GroupToRepresentativePart(group));
            float limpnessRatio = Mathf.Clamp01(limpnessPercent / 100f);
            float pinMultiplier = Mathf.Clamp01(Mathf.Pow(1f - limpnessRatio, 2.2f));
            if (limpnessRatio >= 0.95f)
            {
                pinMultiplier = 0f;
            }
            RagdollPart.Type mask = GroupToPinMask(group);
            state.Creature.ragdoll.SetPinForceMultiplier(pinMultiplier, pinMultiplier, pinMultiplier, pinMultiplier, true, false, mask, null);

            ELDMLog.Info(
                "pins_limp_applied creature=" + SafeCreatureName(state.Creature) +
                " group=" + GroupName(group) +
                " limpness=" + limpnessPercent.ToString("0") + "%" +
                " pinMultiplier=" + pinMultiplier.ToString("0.00") +
                " dismembered=" + dismemberedArmOrHand);

            if (ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Diag(
                    "pins_set_limp creature=" + SafeCreatureName(state.Creature) +
                    " group=" + GroupName(group) +
                    " limpness=" + limpnessPercent.ToString("0") + "%" +
                    " pinMultiplier=" + pinMultiplier.ToString("0.00") +
                    " dismembered=" + dismemberedArmOrHand,
                    verboseOnly: true);
            }
        }

        private void RestorePinsAndHandlesForGroup(CreatureState state, LimbGroup group)
        {
            if (state?.Creature != null && state.Creature.ragdoll != null)
            {
                RagdollPart.Type mask = GroupToPinMask(group);
                if (mask != (RagdollPart.Type)0)
                {
                    state.Creature.ragdoll.ResetPinForce(true, false, mask);
                    ELDMLog.Info(
                        "pins_group_reset creature=" + SafeCreatureName(state.Creature) +
                        " group=" + GroupName(group),
                        verboseOnly: true);
                }
            }

            if (!state.DisabledHandles.TryGetValue(group, out List<HandleRagdoll> bucket))
            {
                return;
            }

            for (int i = 0; i < bucket.Count; i++)
            {
                HandleRagdoll handle = bucket[i];
                if (handle != null)
                {
                    handle.SetTouch(true);
                }
            }

            bucket.Clear();
        }

        private void ClearHandleBucket(CreatureState state, LimbGroup group)
        {
            if (!state.DisabledHandles.TryGetValue(group, out List<HandleRagdoll> bucket))
            {
                return;
            }

            bucket.Clear();
        }

        private CreatureState EnsureCreatureState(Creature creature)
        {
            if (!IsValidTargetCreature(creature))
            {
                return null;
            }

            return EnsureCreatureStateAnyState(creature);
        }

        private CreatureState EnsureCreatureStateAnyState(Creature creature)
        {
            int creatureId = creature.GetInstanceID();
            if (!trackedCreatures.TryGetValue(creatureId, out CreatureState state))
            {
                state = new CreatureState
                {
                    Creature = creature,
                    LastTouchedTime = Time.unscaledTime,
                    LocomotionClampApplied = false,
                    LastAppliedSpeedMultiplier = 1f,
                    LastAppliedAnimSpeedMultiplier = 1f,
                    NoStandModifierApplied = false,
                    KnockoutActive = false,
                    KnockoutUntil = 0f,
                    FullBodyPinClampApplied = false,
                    EyeClipOverrideApplied = false,
                    EyeClipAutoWasActive = true,
                    ActiveEyeMode = EyeMode.None,
                    PendingEyeRestore = false,
                    PendingDeadRecovery = false,
                    DeadRecoveryAt = 0f,
                    DeadRecoveryAnimationHoldUntil = 0f,
                    SuccessfulDeadRecoveries = 0,
                    KnockoutSlowUntil = 0f,
                    DeadRecoverySlowUntil = 0f,
                    StandAttemptCount = 0,
                    NextStandAttemptAt = 0f,
                    LastPierceSlashSoundTime = -999f,
                    LastBluntBreakSoundTime = -999f,
                    LastDisableSoundTime = -999f,
                    WeightApplied = false,
                    LastAppliedWeightMultiplier = 1f,
                    IsDeathRecoveryKnockout = false,
                    EyeRestoreAt = 0f,
                    ActiveMouthMode = MouthMode.None,
                    PendingMouthRestore = false,
                    JawOriginalLocalRotation = Quaternion.identity,
                    JawOriginalLocalRotationCaptured = false,
                    WeightApplyNoPartsLogged = false,
                    MissingLocomotionLogged = false,
                    AnimatorSpeedOverrideApplied = false,
                    AnimatorOriginalSpeed = 1f,
                    AnimatorDynamicSpeedOverrideApplied = false,
                    AnimatorDynamicSpeedOriginal = 1f,
                    AnimatorDynamicSpeedParamHash = 0,
                    AnimatorDynamicSpeedParamResolved = false,
                    AnimatorDynamicSpeedParamAvailable = false,
                    MissingAnimatorLogged = false,
                    MissingAnimatorDynamicSpeedLogged = false,
                    MissingDeathAnimationModuleLogged = false,
                    DeathAnimationDataSpeedApplied = false,
                    LastDeathAnimationDataSpeed = 1f,
                    TorsoParalyzedUntil = 0f,
                    InjuryBehaviorSlowUntil = 0f,
                    InjuryBehaviorSlowFactor = 1f,
                    DeathDespawnExtendedUntil = 0f,
                    LastKillAudioMouthFactor = 0f,
                    LastKillHandledAt = -999f,
                    HasLastInjuryGroup = false,
                    LastInjuryGroup = LimbGroup.Torso,
                    LastDeathInjuryGroup = LimbGroup.Torso,
                    LastInjuryAt = -999f,
                    StandupSlowUntil = 0f,
                    StandupBlendStartAt = 0f,
                    StandupBlendEndAt = 0f,
                    PendingLastStandInterruption = false,
                    LastStandInterruptionTriggered = false,
                    LastStandInterruptionAt = 0f,
                    LastStandInterruptedUntil = 0f,
                    MockDeathRollEyesAt = 0f,
                    MockDeathClosedAmount = 1f,
                    RolledEyeFallbackCloseAmount = 0.35f,
                    RecoveredGazeCloseAmount = 0f,
                    KeepRecoveredGazeEyes = false,
                    RolledEyeClipApplied = false,
                };

                for (int i = 0; i < AllLimbGroups.Length; i++)
                {
                    state.Limbs[AllLimbGroups[i]] = new LimbState
                    {
                        AccumulatedDamage = 0f,
                        Disabled = false,
                        DisabledUntil = 0f,
                        DamageAtDisable = 0f,
                        DisableCount = 0,
                        PermanentDisabled = false,
                    };
                    state.DisabledHandles[AllLimbGroups[i]] = new List<HandleRagdoll>();
                    state.ReactionCooldownUntil[AllLimbGroups[i]] = 0f;
                    state.HitSlowStacks[AllLimbGroups[i]] = 0;
                    state.HitSlowExpireAt[AllLimbGroups[i]] = 0f;
                }

                trackedCreatures[creatureId] = state;

                if (ELDMLog.DiagnosticsEnabled)
                {
                    ELDMLog.Info("track_start creature=" + SafeCreatureName(creature));
                }
            }
            else
            {
                state.Creature = creature;
                state.LastTouchedTime = Time.unscaledTime;
            }

            return state;
        }
        private void RemoveCreatureState(int creatureId)
        {
            if (!trackedCreatures.TryGetValue(creatureId, out CreatureState state))
            {
                return;
            }

            RestoreCreatureState(state);
            trackedCreatures.Remove(creatureId);

            if (ELDMLog.DiagnosticsEnabled)
            {
                ELDMLog.Info("track_stop creature=" + SafeCreatureName(state.Creature) + " reason=despawn_or_cleanup");
            }
        }

        private void ClearAllTrackedState()
        {
            if (trackedCreatures.Count == 0)
            {
                RestorePatchedDeathAnimationClipSpeeds();
                return;
            }

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                RestoreCreatureState(entry.Value);
            }

            trackedCreatures.Clear();
            ELDMTelemetry.RecordTrackedCreatures(0);
            RestorePatchedDeathAnimationClipSpeeds();
        }

        private void RestoreCreatureState(CreatureState state)
        {
            if (state == null || state.Creature == null)
            {
                return;
            }

            Locomotion locomotion = state.Creature.currentLocomotion != null ? state.Creature.currentLocomotion : state.Creature.locomotion;
            if (state.LocomotionClampApplied && locomotion != null)
            {
                locomotion.RemoveSpeedModifier(this);
            }

            if (state.NoStandModifierApplied && state.Creature.brain != null)
            {
                state.Creature.brain.RemoveNoStandUpModifier(this);
            }

            if (state.FullBodyPinClampApplied && state.Creature.ragdoll != null)
            {
                state.Creature.ragdoll.ResetPinForce(true, false, (RagdollPart.Type)0);
            }

            RestoreEyePresentation(state);
            RestoreMouthPresentation(state, force: true);
            RestoreWeightMultiplier(state);
            ClearAnimatorSpeedOverride(state, "state_restore", force: true);

            state.LocomotionClampApplied = false;
            state.LastAppliedSpeedMultiplier = 1f;
            state.LastAppliedAnimSpeedMultiplier = 1f;
            state.NoStandModifierApplied = false;
            state.KnockoutActive = false;
            state.KnockoutUntil = 0f;
            state.IsDeathRecoveryKnockout = false;
            state.FullBodyPinClampApplied = false;
            state.EyeClipOverrideApplied = false;
            state.ActiveEyeMode = EyeMode.None;
            state.PendingEyeRestore = false;
            state.PendingDeadRecovery = false;
            state.DeadRecoveryAt = 0f;
            state.DeadRecoveryAnimationHoldUntil = 0f;
            state.KnockoutSlowUntil = 0f;
            state.DeadRecoverySlowUntil = 0f;
            state.StandAttemptCount = 0;
            state.NextStandAttemptAt = 0f;
            state.WeightApplied = false;
            state.LastAppliedWeightMultiplier = 1f;
            state.EyeRestoreAt = 0f;
            state.ActiveMouthMode = MouthMode.None;
            state.PendingMouthRestore = false;
            state.JawOriginalLocalRotation = Quaternion.identity;
            state.JawOriginalLocalRotationCaptured = false;
            state.WeightApplyNoPartsLogged = false;
            state.MissingLocomotionLogged = false;
            state.AnimatorSpeedOverrideApplied = false;
            state.AnimatorOriginalSpeed = 1f;
            state.AnimatorDynamicSpeedOverrideApplied = false;
            state.AnimatorDynamicSpeedOriginal = 1f;
            state.AnimatorDynamicSpeedParamHash = 0;
            state.AnimatorDynamicSpeedParamResolved = false;
            state.AnimatorDynamicSpeedParamAvailable = false;
            state.MissingAnimatorLogged = false;
            state.MissingAnimatorDynamicSpeedLogged = false;
            state.MissingDeathAnimationModuleLogged = false;
            state.DeathAnimationDataSpeedApplied = false;
            state.LastDeathAnimationDataSpeed = 1f;
            state.TorsoParalyzedUntil = 0f;
            state.InjuryBehaviorSlowUntil = 0f;
            state.InjuryBehaviorSlowFactor = 1f;
            state.DeathDespawnExtendedUntil = 0f;
            state.LastKillAudioMouthFactor = 0f;
            state.LastKillHandledAt = -999f;
            state.HasLastInjuryGroup = false;
            state.LastInjuryGroup = LimbGroup.Torso;
            state.LastDeathInjuryGroup = LimbGroup.Torso;
            state.LastInjuryAt = -999f;
            state.StandupSlowUntil = 0f;
            state.StandupBlendStartAt = 0f;
            state.StandupBlendEndAt = 0f;
            state.PendingLastStandInterruption = false;
            state.LastStandInterruptionTriggered = false;
            state.LastStandInterruptionAt = 0f;
            state.LastStandInterruptedUntil = 0f;
            state.MockDeathRollEyesAt = 0f;
            state.MockDeathClosedAmount = 1f;
            state.RolledEyeFallbackCloseAmount = 0.35f;
            state.RecoveredGazeCloseAmount = 0f;
            state.KeepRecoveredGazeEyes = false;
            state.RolledEyeClipApplied = false;
            state.OriginalPartMasses.Clear();

            for (int i = 0; i < AllLimbGroups.Length; i++)
            {
                RestorePinsAndHandlesForGroup(state, AllLimbGroups[i]);
                state.ReactionCooldownUntil[AllLimbGroups[i]] = 0f;
                state.HitSlowStacks[AllLimbGroups[i]] = 0;
                state.HitSlowExpireAt[AllLimbGroups[i]] = 0f;
            }
        }

        private static bool IsValidTargetCreature(Creature creature)
        {
            return creature != null && !creature.isPlayer && creature.state != Creature.State.Dead && creature.ragdoll != null;
        }

        private static bool IsValidHitTargetCreature(Creature creature)
        {
            return creature != null && !creature.isPlayer && creature.ragdoll != null;
        }

        private static void RestoreWeightMultiplier(CreatureState state)
        {
            Creature creature = state?.Creature;
            if (creature == null || creature.ragdoll == null || creature.ragdoll.parts == null)
            {
                return;
            }

            if (state.OriginalPartMasses == null || state.OriginalPartMasses.Count == 0)
            {
                return;
            }

            for (int i = 0; i < creature.ragdoll.parts.Count; i++)
            {
                RagdollPart part = creature.ragdoll.parts[i];
                if (part == null || part.physicBody == null || part.physicBody.rigidBody == null)
                {
                    continue;
                }

                Rigidbody rb = part.physicBody.rigidBody;
                if (state.OriginalPartMasses.TryGetValue(rb, out float originalMass))
                {
                    rb.mass = originalMass;
                }
            }
        }

        private static bool IsRecoverableDeadCreature(Creature creature)
        {
            return creature != null && !creature.isPlayer && creature.ragdoll != null && creature.state == Creature.State.Dead;
        }

        private static bool IsCreatureAlive(Creature creature)
        {
            return creature != null && creature.state != Creature.State.Dead;
        }

        private static bool TryMapPartToGroup(RagdollPart part, out LimbGroup group)
        {
            group = LimbGroup.Legs;
            if (part == null)
            {
                return false;
            }

            switch (part.type)
            {
                case RagdollPart.Type.LeftLeg:
                case RagdollPart.Type.RightLeg:
                    group = LimbGroup.Legs;
                    return true;
                case RagdollPart.Type.LeftHand:
                case RagdollPart.Type.RightHand:
                    group = LimbGroup.Hands;
                    return true;
                case RagdollPart.Type.LeftArm:
                case RagdollPart.Type.RightArm:
                    group = LimbGroup.Arms;
                    return true;
                case RagdollPart.Type.LeftFoot:
                case RagdollPart.Type.RightFoot:
                    group = LimbGroup.Feet;
                    return true;
                case RagdollPart.Type.LeftWing:
                    group = LimbGroup.WingLeft;
                    return true;
                case RagdollPart.Type.RightWing:
                    group = LimbGroup.WingRight;
                    return true;
                case RagdollPart.Type.Tail:
                    group = LimbGroup.Hip;
                    return true;
                case RagdollPart.Type.Head:
                    group = LimbGroup.Head;
                    return true;
                case RagdollPart.Type.Neck:
                    group = LimbGroup.Neck;
                    return true;
                case RagdollPart.Type.Torso:
                    group = LimbGroup.Torso;
                    return true;
            }

            string token = BuildPartToken(part);
            if (ContainsAny(token, HeadKeywords))
            {
                group = LimbGroup.Head;
                return true;
            }

            if (ContainsAny(token, NeckKeywords))
            {
                group = LimbGroup.Neck;
                return true;
            }

            if (ContainsAny(token, TorsoKeywords))
            {
                group = LimbGroup.Torso;
                return true;
            }

            if (ContainsAny(token, TailKeywords))
            {
                group = LimbGroup.Hip;
                return true;
            }

            if (ContainsAny(token, WingKeywords))
            {
                if (LooksLikeRightSide(token))
                {
                    group = LimbGroup.WingRight;
                }
                else if (LooksLikeLeftSide(token))
                {
                    group = LimbGroup.WingLeft;
                }
                else
                {
                    group = LimbGroup.WingLeft;
                }
                return true;
            }

            if (ContainsAny(token, HandKeywords))
            {
                group = LimbGroup.Hands;
                return true;
            }

            if (ContainsAny(token, FootKeywords))
            {
                group = LimbGroup.Feet;
                return true;
            }

            if (ContainsAny(token, LegKeywords))
            {
                group = LimbGroup.Legs;
                return true;
            }

            if (ContainsAny(token, ArmKeywords))
            {
                group = LimbGroup.Arms;
                return true;
            }

            return false;
        }

        private static bool LooksLikeArmPart(RagdollPart part)
        {
            return ContainsAny(BuildPartToken(part), ArmKeywords);
        }

        private static bool LooksLikeRightSide(string token)
        {
            return ContainsAny(token, RightKeywords);
        }

        private static bool LooksLikeLeftSide(string token)
        {
            return ContainsAny(token, LeftKeywords);
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

        private static bool IsGroupDismembered(CreatureState state, LimbGroup group)
        {
            Creature creature = state?.Creature;
            if (creature == null || creature.ragdoll == null || creature.ragdoll.parts == null)
            {
                return false;
            }

            RagdollPart.Type mask = GroupToPinMask(group);
            bool useArmKeywordFallback = group == LimbGroup.Arms || group == LimbGroup.Hands;
            List<RagdollPart> parts = creature.ragdoll.parts;
            for (int i = 0; i < parts.Count; i++)
            {
                RagdollPart part = parts[i];
                if (part == null)
                {
                    continue;
                }

                bool groupMatch = (mask != (RagdollPart.Type)0 && (part.type & mask) != 0) || (useArmKeywordFallback && LooksLikeArmPart(part));
                if (!groupMatch)
                {
                    continue;
                }

                if (IsPartDismembered(part))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPartDismembered(RagdollPart part)
        {
            if (part == null)
            {
                return false;
            }

            bool? sliced = TryReadMemberAsBool(part, "isSliced") ??
                           TryReadMemberAsBool(part, "sliced") ??
                           TryReadMemberAsBool(part, "isDismembered") ??
                           TryReadMemberAsBool(part, "dismembered") ??
                           TryReadMemberAsBool(part, "isCut") ??
                           TryReadMemberAsBool(part, "severed");
            if (sliced.HasValue)
            {
                return sliced.Value;
            }

            return false;
        }

        private static object TryReadMemberAsObject(object target, string memberName)
        {
            if (target == null || string.IsNullOrWhiteSpace(memberName))
            {
                return null;
            }

            Type type = target.GetType();
            try
            {
                PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null && property.CanRead)
                {
                    return property.GetValue(target, null);
                }

                FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (field != null)
                {
                    return field.GetValue(target);
                }
            }
            catch
            {
            }

            return null;
        }

        private static bool? TryReadMemberAsBool(object target, string memberName)
        {
            object value = TryReadMemberAsObject(target, memberName);
            if (value == null)
            {
                return null;
            }

            if (value is bool b)
            {
                return b;
            }

            if (value is string s && bool.TryParse(s, out bool parsed))
            {
                return parsed;
            }

            return null;
        }

        private static bool ContainsAny(string token, string[] values)
        {
            if (string.IsNullOrWhiteSpace(token) || values == null)
            {
                return false;
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (token.IndexOf(values[i], StringComparison.Ordinal) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static RagdollPart.Type GroupToRepresentativePart(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Legs:
                    return RagdollPart.Type.LeftLeg;
                case LimbGroup.Arms:
                    return RagdollPart.Type.LeftArm;
                case LimbGroup.Feet:
                    return RagdollPart.Type.LeftFoot;
                case LimbGroup.Hands:
                    return RagdollPart.Type.LeftHand;
                case LimbGroup.WingLeft:
                    return RagdollPart.Type.LeftWing;
                case LimbGroup.WingRight:
                    return RagdollPart.Type.RightWing;
                case LimbGroup.Hip:
                    return RagdollPart.Type.Tail;
                case LimbGroup.Head:
                    return RagdollPart.Type.Head;
                case LimbGroup.Neck:
                    return RagdollPart.Type.Neck;
                case LimbGroup.Torso:
                    return RagdollPart.Type.Torso;
                default:
                    return RagdollPart.Type.Torso;
            }
        }

        private static RagdollPart.Type GroupToPinMask(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Legs:
                    return RagdollPart.Type.LeftLeg | RagdollPart.Type.RightLeg;
                case LimbGroup.Feet:
                    return RagdollPart.Type.LeftFoot | RagdollPart.Type.RightFoot;
                case LimbGroup.Hip:
                    return RagdollPart.Type.Tail;
                case LimbGroup.Arms:
                    return RagdollPart.Type.LeftArm | RagdollPart.Type.RightArm;
                case LimbGroup.Hands:
                    return RagdollPart.Type.LeftHand | RagdollPart.Type.RightHand;
                case LimbGroup.WingLeft:
                    return RagdollPart.Type.LeftWing | RagdollPart.Type.LeftArm | RagdollPart.Type.LeftHand;
                case LimbGroup.WingRight:
                    return RagdollPart.Type.RightWing | RagdollPart.Type.RightArm | RagdollPart.Type.RightHand;
                case LimbGroup.Torso:
                    return RagdollPart.Type.Torso;
                default:
                    return (RagdollPart.Type)0;
            }
        }

        private static bool IsLegStyleGroup(LimbGroup group)
        {
            return group == LimbGroup.Legs || group == LimbGroup.Feet || group == LimbGroup.Hip;
        }

        private static bool IsArmStyleGroup(LimbGroup group)
        {
            return group == LimbGroup.Arms || group == LimbGroup.Hands || group == LimbGroup.WingLeft || group == LimbGroup.WingRight;
        }

        private static bool CanDisableGroup(LimbGroup group)
        {
            return IsLegStyleGroup(group) || IsArmStyleGroup(group) || group == LimbGroup.Torso;
        }

        private static bool ShouldUsePinDisable(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Legs:
                    return ELDMModOptions.LegImmobilization;
                case LimbGroup.Feet:
                    return ELDMModOptions.ShouldDisableFeet();
                case LimbGroup.Hip:
                    return ELDMModOptions.ShouldDisableTail();
                case LimbGroup.Arms:
                    return ELDMModOptions.ArmImmobilization;
                case LimbGroup.Hands:
                    return ELDMModOptions.ShouldDisableHands();
                case LimbGroup.WingLeft:
                case LimbGroup.WingRight:
                    return ELDMModOptions.ShouldDisableWings();
                case LimbGroup.Torso:
                    return false;
                default:
                    return false;
            }
        }

        private static bool IsAnyGroupDisabled(CreatureState state, LimbGroup[] groups)
        {
            if (state == null || groups == null)
            {
                return false;
            }

            for (int i = 0; i < groups.Length; i++)
            {
                if (IsLimbDisabled(state, groups[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsAnyLegStyleDisabled(CreatureState state)
        {
            return IsAnyGroupDisabled(state, LegStyleGroups);
        }

        private static bool IsLegOrFootDisabled(CreatureState state)
        {
            return IsLimbDisabled(state, LimbGroup.Legs) || IsLimbDisabled(state, LimbGroup.Feet);
        }

        private static bool AnyLimbDisabled(CreatureState state)
        {
            if (state == null)
            {
                return false;
            }

            for (int i = 0; i < AllLimbGroups.Length; i++)
            {
                if (IsLimbDisabled(state, AllLimbGroups[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsLimbDisabled(CreatureState state, LimbGroup group)
        {
            if (state == null || !state.Limbs.TryGetValue(group, out LimbState limbState))
            {
                return false;
            }

            return limbState.Disabled;
        }

        private static List<HandleRagdoll> GetHandleBucket(CreatureState state, LimbGroup group)
        {
            if (!state.DisabledHandles.TryGetValue(group, out List<HandleRagdoll> bucket))
            {
                bucket = new List<HandleRagdoll>();
                state.DisabledHandles[group] = bucket;
            }

            return bucket;
        }

        private static float ComputeDisableDurationForPass(float baseDurationSeconds, int passIndex)
        {
            float baseDuration = Mathf.Clamp(baseDurationSeconds, 0.25f, 600f);
            return Mathf.Clamp(baseDuration, 0.25f, 1800f);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f;
        }

        private static string GroupName(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.Legs:
                    return "legs";
                case LimbGroup.Arms:
                    return "arms";
                case LimbGroup.Feet:
                    return "feet";
                case LimbGroup.Hands:
                    return "hands";
                case LimbGroup.WingLeft:
                    return "wing_left";
                case LimbGroup.WingRight:
                    return "wing_right";
                case LimbGroup.Hip:
                    return "hip";
                case LimbGroup.Head:
                    return "head";
                case LimbGroup.Neck:
                    return "neck";
                case LimbGroup.Torso:
                    return "torso";
                default:
                    return "unknown";
            }
        }

        private static string SafePartName(RagdollPart part)
        {
            if (part == null)
            {
                return "unknown";
            }

            if (!string.IsNullOrWhiteSpace(part.name))
            {
                return part.name;
            }

            return part.type.ToString();
        }

        private static void ApplyMouthOpenFactor(CreatureState state, float factor, string reason)
        {
            Creature creature = state?.Creature;
            if (creature == null || creature.jaw == null)
            {
                if (ELDMLog.DiagnosticsEnabled)
                {
                    ELDMLog.Diag(
                        "mouth_apply_skipped creature=" + SafeCreatureName(creature) +
                        " reason=" + reason +
                        " skip=missing_jaw",
                        verboseOnly: true);
                }
                return;
            }

            if (!state.JawOriginalLocalRotationCaptured)
            {
                state.JawOriginalLocalRotation = creature.jaw.localRotation;
                state.JawOriginalLocalRotationCaptured = true;
            }

            float clamped = Mathf.Clamp01(factor);
            float previous = state.LastKillAudioMouthFactor;
            if (previous > 0.001f)
            {
                float smoothing = Mathf.Clamp01(Time.unscaledDeltaTime * 1.2f);
                clamped = Mathf.Lerp(previous, clamped, smoothing);
            }
            Vector3 jawMax = creature.jawMaxRotation;
            if (jawMax.sqrMagnitude < 0.01f)
            {
                jawMax = new Vector3(-18f, 0f, 0f);
            }

            creature.jaw.localRotation = state.JawOriginalLocalRotation * Quaternion.Euler(jawMax * clamped);

            MouthMode mode = clamped < 0.08f
                ? MouthMode.None
                : (clamped < 0.45f ? MouthMode.Subtle : (clamped < 0.75f ? MouthMode.Open : MouthMode.WideOpen));

            state.ActiveMouthMode = mode;
            state.LastKillAudioMouthFactor = clamped;
            if (ELDMLog.DiagnosticsEnabled && Mathf.Abs(previous - clamped) > 0.18f)
            {
                ELDMLog.Diag(
                    "mouth_audio_sync creature=" + SafeCreatureName(creature) +
                    " reason=" + reason +
                    " factor=" + clamped.ToString("0.00") +
                    " mode=" + mode,
                    verboseOnly: true);
            }
        }

        private static string SafeCreatureName(Creature creature)
        {
            if (creature == null)
            {
                return "null";
            }

            if (!string.IsNullOrWhiteSpace(creature.creatureId))
            {
                return creature.creatureId;
            }

            if (creature.data != null && !string.IsNullOrWhiteSpace(creature.data.id))
            {
                return creature.data.id;
            }

            return "instance_" + creature.GetInstanceID();
        }
    }
}


