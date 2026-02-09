using System;
using System.Collections.Generic;
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
        }

        private struct LimbState
        {
            public float AccumulatedDamage;
            public bool Disabled;
            public float DisabledUntil;
        }

        private enum EyeMode
        {
            None = 0,
            Closed = 1,
            Rolled = 2,
        }

        private sealed class CreatureState
        {
            public Creature Creature;
            public float LastTouchedTime;
            public bool LocomotionClampApplied;
            public float LastAppliedSpeedMultiplier;
            public bool NoStandModifierApplied;
            public bool KnockoutActive;
            public float KnockoutUntil;
            public bool FullBodyPinClampApplied;
            public bool EyeClipOverrideApplied;
            public bool EyeClipAutoWasActive;
            public EyeMode ActiveEyeMode;
            public bool PendingEyeRestore;
            public bool PendingDeadRecovery;
            public float DeadRecoveryAt;
            public int SuccessfulDeadRecoveries;
            public float KnockoutSlowUntil;
            public float DeadRecoverySlowUntil;
            public int StandAttemptCount;
            public float NextStandAttemptAt;
            public float LastPierceSlashSoundTime;
            public float LastBluntBreakSoundTime;
            public readonly Dictionary<LimbGroup, LimbState> Limbs = new Dictionary<LimbGroup, LimbState>();
            public readonly Dictionary<LimbGroup, List<HandleRagdoll>> DisabledHandles = new Dictionary<LimbGroup, List<HandleRagdoll>>();
        }

        private const float StaleCreatureStateSeconds = 45f;
        private const float PierceSlashSoundMinIntervalSeconds = 0.08f;
        private const float BluntBreakSoundMinIntervalSeconds = 0.35f;
        private const float StandRetryIntervalSeconds = 0.60f;
        private const int MaxStandRetryAttempts = 40;

        private static readonly LimbGroup[] AllLimbGroups =
        {
            LimbGroup.Legs,
            LimbGroup.Arms,
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

        public static EnemyLimbManager Instance { get; } = new EnemyLimbManager();

        private readonly Dictionary<int, CreatureState> trackedCreatures = new Dictionary<int, CreatureState>();

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
        }

        public void Update()
        {
            if (!ELDMModOptions.EnableMod)
            {
                if (trackedCreatures.Count > 0)
                {
                    ClearAllTrackedState();
                }

                return;
            }

            float now = Time.unscaledTime;
            ProcessDeadRecoveries(now);
            ProcessRecoveries(now);
            ProcessKnockouts(now);
            ProcessStandRetries(now);
            ProcessMovementEffects(now);
            CleanupStaleCreatures(now);
            ELDMTelemetry.RecordTrackedCreatures(trackedCreatures.Count);
        }

        public void OnCreatureSpawn(Creature creature)
        {
            if (!ELDMModOptions.EnableMod || !IsValidTargetCreature(creature))
            {
                return;
            }

            EnsureCreatureState(creature);
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
            state.LastTouchedTime = Time.unscaledTime;

            if (state.PendingDeadRecovery)
            {
                return;
            }

            if (!ELDMModOptions.CanAttemptDeadRecovery())
            {
                ApplyPermanentDeathEyePresentation(state, "last_stand_disabled_or_zero");
                return;
            }

            RagdollPart killPart = collisionInstance != null ? collisionInstance.damageStruct.hitRagdollPart : null;
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

            state.PendingDeadRecovery = true;
            state.DeadRecoveryAt = Time.unscaledTime + ELDMModOptions.GetDeadRevivalDelaySeconds();
            state.ActiveEyeMode = SelectRandomEyeMode();
            state.PendingEyeRestore = true;
            ApplyEyePresentation(state, state.ActiveEyeMode);

            ELDMLog.Info(
                "dead_recovery_queued creature=" + SafeCreatureName(creature) +
                " executeIn=" + ELDMModOptions.GetDeadRevivalDelaySeconds().ToString("0.0") + "s" +
                " chance=" + (chance * 100f).ToString("0") + "%" +
                " successfulRecoveries=" + state.SuccessfulDeadRecoveries +
                " eyeMode=" + state.ActiveEyeMode);
        }

        public void OnCreatureHit(Creature creature, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (!ELDMModOptions.EnableMod || eventTime != EventTime.OnEnd)
            {
                return;
            }

            if (!IsValidTargetCreature(creature) || collisionInstance == null || !collisionInstance.damageStruct.active)
            {
                return;
            }

            RagdollPart hitPart = collisionInstance.damageStruct.hitRagdollPart;
            if (!TryMapPartToGroup(hitPart, out LimbGroup limbGroup))
            {
                return;
            }

            float trackedDamage = Mathf.Max(0f, collisionInstance.damageStruct.damage);
            if (trackedDamage < ELDMModOptions.GetMinimumTrackedHitDamage())
            {
                return;
            }

            DamageType damageType = collisionInstance.damageStruct.damageType;
            float now = Time.unscaledTime;
            CreatureState state = EnsureCreatureState(creature);
            state.LastTouchedTime = now;

            TryPlayPierceSlashHitSound(state, collisionInstance, damageType, now, hitPart);

            if (ShouldTriggerBluntKnockout(collisionInstance, damageType, trackedDamage, limbGroup))
            {
                BeginKnockout(state, now, "blunt_hit");
            }

            LimbState limbState = state.Limbs[limbGroup];
            if (limbState.Disabled)
            {
                if (ELDMModOptions.ShouldRefreshDisableTimerOnHit())
                {
                    float duration = ELDMModOptions.GetLimbDisableDurationSeconds(GroupToRepresentativePart(limbGroup));
                    if (IsFinite(duration))
                    {
                        limbState.DisabledUntil = now + duration;
                        state.Limbs[limbGroup] = limbState;
                    }
                }

                if (ELDMLog.VerboseEnabled && ELDMTelemetry.ShouldLogHit(creature.GetInstanceID(), GroupName(limbGroup), now))
                {
                    ELDMLog.Info(
                        "hit_ignored_already_disabled creature=" + SafeCreatureName(creature) +
                        " group=" + GroupName(limbGroup) +
                        " damage=" + trackedDamage.ToString("0.00"),
                        verboseOnly: true);
                }

                return;
            }

            limbState.AccumulatedDamage += trackedDamage;
            state.Limbs[limbGroup] = limbState;
            ELDMTelemetry.RecordHit();

            if (ELDMLog.VerboseEnabled && ELDMTelemetry.ShouldLogHit(creature.GetInstanceID(), GroupName(limbGroup), now))
            {
                ELDMLog.Info(
                    "hit creature=" + SafeCreatureName(creature) +
                    " group=" + GroupName(limbGroup) +
                    " damage=" + trackedDamage.ToString("0.00") +
                    " accumulated=" + limbState.AccumulatedDamage.ToString("0.00") +
                    " threshold=" + ELDMModOptions.GetLimbThresholdDamage(GroupToRepresentativePart(limbGroup)).ToString("0.00"),
                    verboseOnly: true);
            }

            float threshold = ELDMModOptions.GetLimbThresholdDamage(GroupToRepresentativePart(limbGroup));
            if (limbState.AccumulatedDamage >= threshold)
            {
                DisableLimb(state, limbGroup, hitPart, collisionInstance, damageType, now);
            }
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
                    continue;
                }

                if (!IsRecoverableDeadCreature(creature))
                {
                    state.PendingDeadRecovery = false;
                    RestoreEyePresentation(state);
                    continue;
                }

                if (!TryPerformDeadRecovery(state))
                {
                    state.PendingDeadRecovery = false;
                    RestoreEyePresentation(state);
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

        private bool TryPerformDeadRecovery(CreatureState state)
        {
            Creature creature = state.Creature;
            if (creature == null)
            {
                return false;
            }

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

            state.PendingDeadRecovery = false;
            state.SuccessfulDeadRecoveries++;
            state.DeadRecoverySlowUntil = Time.unscaledTime + ELDMModOptions.GetDeadRecoverySlowDurationSeconds();
            state.StandAttemptCount = 0;
            state.NextStandAttemptAt = Time.unscaledTime + 0.35f;
            RestoreEyePresentation(state);

            if (state.KnockoutActive)
            {
                EndKnockout(state, Time.unscaledTime, "dead_recovery");
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
                    if (group == LimbGroup.Legs)
                    {
                        legsStillDisabled = true;
                        SetGroupPinsDisabled(state, LimbGroup.Legs);
                    }
                    else
                    {
                        armsStillDisabled = true;
                        ApplyArmDisabled(state, LimbGroup.Arms);
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
                " legsDisabled=" + legsStillDisabled +
                " armsDisabled=" + armsStillDisabled +
                " standAttempted=" + standAttempted +
                " deadSlowUntil=" + state.DeadRecoverySlowUntil.ToString("0.00"));

            return true;
        }

        private void ProcessRecoveries(float now)
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            List<int> invalidIds = null;

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                CreatureState state = entry.Value;

                if (state.PendingDeadRecovery)
                {
                    continue;
                }

                if (!IsCreatureAlive(state.Creature))
                {
                    if (invalidIds == null)
                    {
                        invalidIds = new List<int>();
                    }

                    invalidIds.Add(entry.Key);
                    continue;
                }

                if (!ELDMModOptions.LastStandEnabled)
                {
                    continue;
                }

                for (int i = 0; i < AllLimbGroups.Length; i++)
                {
                    LimbGroup group = AllLimbGroups[i];
                    LimbState limbState = state.Limbs[group];
                    if (!limbState.Disabled || !IsFinite(limbState.DisabledUntil) || now < limbState.DisabledUntil)
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

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
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

                if (!ELDMModOptions.KnockoutEnabled || now >= state.KnockoutUntil || !IsCreatureAlive(creature))
                {
                    EndKnockout(state, now, ELDMModOptions.KnockoutEnabled ? "timer" : "disabled");
                    continue;
                }

                state.LastTouchedTime = now;
                ApplyKnockoutPose(state);
                ForceReleaseHeldItems(creature);
                ApplyEyePresentation(state, state.ActiveEyeMode);
                UpdateMovementEffects(state, forceFall: false);
            }
        }

        private void ProcessStandRetries(float now)
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
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

                if (state.PendingDeadRecovery || state.KnockoutActive || IsLimbDisabled(state, LimbGroup.Legs))
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

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                CreatureState state = entry.Value;
                Creature creature = state.Creature;
                if (creature == null || !IsCreatureAlive(creature) || state.PendingDeadRecovery)
                {
                    continue;
                }

                UpdateMovementEffects(state, forceFall: false);
            }
        }

        private void BeginKnockout(CreatureState state, float now, string reason)
        {
            if (state == null || !ELDMModOptions.KnockoutEnabled || !IsCreatureAlive(state.Creature))
            {
                return;
            }

            float duration = ELDMModOptions.GetKnockoutDurationSeconds();
            float knockoutUntil = now + duration;
            bool wasActive = state.KnockoutActive;

            state.KnockoutActive = true;
            state.KnockoutUntil = wasActive ? Mathf.Max(state.KnockoutUntil, knockoutUntil) : knockoutUntil;
            state.LastTouchedTime = now;
            state.ActiveEyeMode = wasActive ? state.ActiveEyeMode : SelectRandomEyeMode();
            state.PendingEyeRestore = true;

            ApplyKnockoutPose(state);
            ForceReleaseHeldItems(state.Creature);
            ApplyEyePresentation(state, state.ActiveEyeMode);
            UpdateMovementEffects(state, forceFall: true);

            ELDMLog.Info(
                (wasActive ? "knockout_extended" : "knockout_started") +
                " creature=" + SafeCreatureName(state.Creature) +
                " reason=" + reason +
                " until=" + state.KnockoutUntil.ToString("0.00") +
                " eyeMode=" + state.ActiveEyeMode,
                verboseOnly: wasActive);
        }

        private void EndKnockout(CreatureState state, float now, string reason)
        {
            if (state == null || !state.KnockoutActive)
            {
                return;
            }

            state.KnockoutActive = false;
            state.KnockoutUntil = 0f;
            state.KnockoutSlowUntil = now + ELDMModOptions.GetKnockoutRecoverySlowDurationSeconds();

            Creature creature = state.Creature;
            if (creature != null)
            {
                if (state.FullBodyPinClampApplied && creature.ragdoll != null)
                {
                    creature.ragdoll.ResetPinForce(true, false, (RagdollPart.Type)0);
                }

                RestoreEyePresentation(state);
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

            if (!IsLimbDisabled(state, LimbGroup.Legs))
            {
                state.NextStandAttemptAt = now + 0.25f;
                state.StandAttemptCount = 0;
            }

            UpdateMovementEffects(state, forceFall: false);

            ELDMLog.Info(
                "knockout_ended creature=" + SafeCreatureName(state.Creature) +
                " reason=" + reason +
                " at=" + now.ToString("0.00"),
                verboseOnly: true);
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

        private static bool ShouldTriggerBluntKnockout(CollisionInstance collisionInstance, DamageType damageType, float trackedDamage, LimbGroup limbGroup)
        {
            if (!ELDMModOptions.KnockoutEnabled || collisionInstance == null)
            {
                return false;
            }

            if (!IsBluntLikeDamageType(collisionInstance, damageType))
            {
                return false;
            }

            float threshold = ELDMModOptions.GetLimbThresholdDamage(GroupToRepresentativePart(limbGroup));
            float requiredDamage = Mathf.Clamp(threshold * 0.25f, 2f, 10f);
            int pushLevel = collisionInstance.damageStruct.pushLevel;

            return trackedDamage >= requiredDamage || (pushLevel >= 1 && trackedDamage >= 0.5f);
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
                }

                creature.autoEyeClipsActive = false;
                if (creature.eyeClips != null)
                {
                    for (int i = 0; i < creature.eyeClips.Count; i++)
                    {
                        CreatureData.EyeClip eyeClip = creature.eyeClips[i];
                        if (eyeClip != null)
                        {
                            eyeClip.active = false;
                        }
                    }
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

        private static void ApplyEyePresentation(CreatureState state, EyeMode mode)
        {
            if (state?.Creature == null || mode == EyeMode.None)
            {
                return;
            }

            ApplyEyeClipLock(state, true);

            if (mode == EyeMode.Closed)
            {
                SetEyesClosedAmount(state.Creature, 1f);
                return;
            }

            if (!TryPlayRolledEyeClip(state.Creature))
            {
                SetEyesClosedAmount(state.Creature, 0.35f);
            }
        }

        private static void RestoreEyePresentation(CreatureState state)
        {
            if (state == null || !state.PendingEyeRestore)
            {
                return;
            }

            ApplyEyeClipLock(state, false);
            SetEyesClosedAmount(state.Creature, 0f);
            state.ActiveEyeMode = EyeMode.None;
            state.PendingEyeRestore = false;
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

            state.ActiveEyeMode = SelectRandomEyeMode();
            state.PendingEyeRestore = false;
            ApplyEyePresentation(state, state.ActiveEyeMode);

            ELDMLog.Info(
                "permadeath_eye_applied creature=" + SafeCreatureName(state.Creature) +
                " eyeMode=" + state.ActiveEyeMode +
                " reason=" + reason,
                verboseOnly: true);
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
                    now < state.DeadRecoverySlowUntil ||
                    now < state.KnockoutSlowUntil ||
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

        private void DisableLimb(CreatureState state, LimbGroup group, RagdollPart hitPart, CollisionInstance collisionInstance, DamageType damageType, float now)
        {
            LimbState limbState = state.Limbs[group];
            if (limbState.Disabled)
            {
                return;
            }

            limbState.Disabled = true;
            float duration = ELDMModOptions.GetLimbDisableDurationSeconds(GroupToRepresentativePart(group));
            limbState.DisabledUntil = IsFinite(duration) ? now + duration : float.PositiveInfinity;
            state.Limbs[group] = limbState;

            if (group == LimbGroup.Legs)
            {
                SetGroupPinsDisabled(state, group);
                UpdateMovementEffects(state, forceFall: ELDMModOptions.FallFromLegInjury);
            }
            else
            {
                ApplyArmDisabled(state, group);
            }

            ELDMTelemetry.RecordDisable();
            ELDMLog.Info(
                "limb_disabled creature=" + SafeCreatureName(state.Creature) +
                " group=" + GroupName(group) +
                " accumulated=" + limbState.AccumulatedDamage.ToString("0.00") +
                " duration=" + (IsFinite(duration) ? duration.ToString("0.0") + "s" : "infinite") +
                " sourcePart=" + SafePartName(hitPart) +
                " damageType=" + damageType);

            if (damageType == DamageType.Blunt)
            {
                TryPlayBluntBreakSound(state, collisionInstance, now, hitPart);
                BeginKnockout(state, now, "blunt_limb_disable");
            }
        }
        private void RecoverLimb(CreatureState state, LimbGroup group)
        {
            LimbState limbState = state.Limbs[group];
            if (!limbState.Disabled)
            {
                return;
            }

            limbState.Disabled = false;
            limbState.DisabledUntil = 0f;
            limbState.AccumulatedDamage *= ELDMModOptions.GetRetainedDamageRatioAfterRecovery();
            state.Limbs[group] = limbState;

            if (ELDMModOptions.RecoveryRestoresPinForces)
            {
                RestorePinsAndHandlesForGroup(state, group);
            }
            else
            {
                ClearHandleBucket(state, group);
            }

            if (group == LimbGroup.Legs)
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
            List<RagdollPart> parts = creature.ragdoll.parts;
            for (int i = 0; i < parts.Count; i++)
            {
                RagdollPart part = parts[i];
                if (part == null || part.handles == null)
                {
                    continue;
                }

                bool groupMatch = (part.type & mask) != 0 || (group == LimbGroup.Arms && LooksLikeArmPart(part));
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
            bool legDisabled = IsLimbDisabled(state, LimbGroup.Legs);
            bool armDisabled = IsLimbDisabled(state, LimbGroup.Arms);
            bool knockoutActive = state.KnockoutActive && ELDMModOptions.KnockoutEnabled;
            bool hasKnockoutRecoverySlow = now < state.KnockoutSlowUntil;
            bool hasDeadRecoverySlow = now < state.DeadRecoverySlowUntil;
            bool shouldApplySpeedClamp = legDisabled || armDisabled || knockoutActive || hasKnockoutRecoverySlow || hasDeadRecoverySlow;

            Locomotion locomotion = creature.currentLocomotion != null ? creature.currentLocomotion : creature.locomotion;
            if (shouldApplySpeedClamp && locomotion != null)
            {
                float speed = knockoutActive ? 0f : CalculateSpeedMultiplier(state, legDisabled, armDisabled, hasKnockoutRecoverySlow, hasDeadRecoverySlow);
                speed = Mathf.Clamp(speed, 0f, 1f);
                locomotion.SetSpeedModifier(this, speed, speed, speed, speed, 1f, speed);
                state.LocomotionClampApplied = true;
                if (ELDMLog.DiagnosticsEnabled && Mathf.Abs(speed - state.LastAppliedSpeedMultiplier) > 0.02f)
                {
                    ELDMLog.Diag(
                        "speed_modifier creature=" + SafeCreatureName(creature) +
                        " speed=" + speed.ToString("0.00") +
                        " legDisabled=" + legDisabled +
                        " armDisabled=" + armDisabled +
                        " knockoutActive=" + knockoutActive +
                        " knockoutSlow=" + hasKnockoutRecoverySlow +
                        " deadSlow=" + hasDeadRecoverySlow);
                }
                state.LastAppliedSpeedMultiplier = speed;
            }
            else if (state.LocomotionClampApplied && locomotion != null)
            {
                if (ELDMLog.DiagnosticsEnabled && Mathf.Abs(state.LastAppliedSpeedMultiplier - 1f) > 0.02f)
                {
                    ELDMLog.Diag(
                        "speed_modifier_cleared creature=" + SafeCreatureName(creature) +
                        " previous=" + state.LastAppliedSpeedMultiplier.ToString("0.00"));
                }
                locomotion.RemoveSpeedModifier(this);
                state.LocomotionClampApplied = false;
                state.LastAppliedSpeedMultiplier = 1f;
            }

            bool shouldPreventStand = knockoutActive || (legDisabled && ELDMModOptions.LegImmobilization);
            if (shouldPreventStand)
            {
                if (creature.brain != null && !state.NoStandModifierApplied)
                {
                    creature.brain.AddNoStandUpModifier(this);
                    state.NoStandModifierApplied = true;
                }

                if ((forceFall || knockoutActive) && creature.ragdoll != null && creature.ragdoll.state == Ragdoll.State.Standing)
                {
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized, true);
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
                    RestorePinsAndHandlesForGroup(state, LimbGroup.Legs);
                }
            }
        }

        private static float CalculateSpeedMultiplier(CreatureState state, bool legDisabled, bool armDisabled, bool hasKnockoutRecoverySlow, bool hasDeadRecoverySlow)
        {
            bool globalStack = ELDMModOptions.UseSlowDebuffStacking();
            float stackedProduct = 1f;
            float strongestNonStacking = 1f;
            bool hasAnyEffect = false;

            if (legDisabled)
            {
                if (ELDMModOptions.LegImmobilization)
                {
                    return 0f;
                }

                float legFactor = 1f - ELDMModOptions.GetLimbSlowDebuffRatio(RagdollPart.Type.LeftLeg);
                ApplySlowFactor(ref stackedProduct, ref strongestNonStacking, ref hasAnyEffect, legFactor, globalStack && ELDMModOptions.GetLimbSlowStacks(RagdollPart.Type.LeftLeg));
            }

            if (armDisabled)
            {
                float armFactor = 1f - ELDMModOptions.GetLimbSlowDebuffRatio(RagdollPart.Type.LeftArm);
                ApplySlowFactor(ref stackedProduct, ref strongestNonStacking, ref hasAnyEffect, armFactor, globalStack && ELDMModOptions.GetLimbSlowStacks(RagdollPart.Type.LeftArm));
            }

            if (hasKnockoutRecoverySlow)
            {
                float knockoutFactor = 1f - ELDMModOptions.GetKnockoutRecoverySlowDebuffRatio();
                ApplySlowFactor(ref stackedProduct, ref strongestNonStacking, ref hasAnyEffect, knockoutFactor, globalStack);
            }

            if (hasDeadRecoverySlow)
            {
                float deadFactor = 1f - ELDMModOptions.GetDeadRecoverySlowDebuffRatio();
                ApplySlowFactor(ref stackedProduct, ref strongestNonStacking, ref hasAnyEffect, deadFactor, globalStack);
            }

            if (!hasAnyEffect)
            {
                return 1f;
            }

            if (!globalStack)
            {
                return Mathf.Clamp01(strongestNonStacking);
            }

            return Mathf.Clamp01(Mathf.Min(stackedProduct, strongestNonStacking));
        }

        private static void ApplySlowFactor(ref float stackedProduct, ref float strongestNonStacking, ref bool hasAnyEffect, float factor, bool stack)
        {
            float clampedFactor = Mathf.Clamp(factor, 0.05f, 1f);
            hasAnyEffect = true;

            if (stack)
            {
                stackedProduct *= clampedFactor;
            }
            else
            {
                strongestNonStacking = Mathf.Min(strongestNonStacking, clampedFactor);
            }
        }

        private void ApplyLegPinState(CreatureState state)
        {
            if (!ELDMModOptions.LegImmobilization)
            {
                return;
            }

            if (IsLimbDisabled(state, LimbGroup.Legs))
            {
                SetGroupPinsDisabled(state, LimbGroup.Legs);
            }
            else if (ELDMModOptions.RecoveryRestoresPinForces)
            {
                RestorePinsAndHandlesForGroup(state, LimbGroup.Legs);
            }
        }

        private void SetGroupPinsDisabled(CreatureState state, LimbGroup group)
        {
            if (state?.Creature == null || state.Creature.ragdoll == null)
            {
                return;
            }

            bool shouldDisablePins = group == LimbGroup.Legs ? ELDMModOptions.LegImmobilization : ELDMModOptions.ArmImmobilization;
            if (!shouldDisablePins)
            {
                return;
            }

            state.Creature.ragdoll.SetPinForceMultiplier(0f, 0f, 0f, 0f, true, false, GroupToPinMask(group), null);
        }

        private void RestorePinsAndHandlesForGroup(CreatureState state, LimbGroup group)
        {
            if (state?.Creature != null && state.Creature.ragdoll != null)
            {
                bool hadPinned = group == LimbGroup.Legs ? ELDMModOptions.LegImmobilization : ELDMModOptions.ArmImmobilization;
                if (hadPinned)
                {
                    state.Creature.ragdoll.ResetPinForce(true, false, GroupToPinMask(group));
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
                    SuccessfulDeadRecoveries = 0,
                    KnockoutSlowUntil = 0f,
                    DeadRecoverySlowUntil = 0f,
                    StandAttemptCount = 0,
                    NextStandAttemptAt = 0f,
                    LastPierceSlashSoundTime = -999f,
                    LastBluntBreakSoundTime = -999f,
                };

                for (int i = 0; i < AllLimbGroups.Length; i++)
                {
                    state.Limbs[AllLimbGroups[i]] = new LimbState
                    {
                        AccumulatedDamage = 0f,
                        Disabled = false,
                        DisabledUntil = 0f,
                    };
                    state.DisabledHandles[AllLimbGroups[i]] = new List<HandleRagdoll>();
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
                return;
            }

            foreach (KeyValuePair<int, CreatureState> entry in trackedCreatures)
            {
                RestoreCreatureState(entry.Value);
            }

            trackedCreatures.Clear();
            ELDMTelemetry.RecordTrackedCreatures(0);
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

            state.LocomotionClampApplied = false;
            state.LastAppliedSpeedMultiplier = 1f;
            state.NoStandModifierApplied = false;
            state.KnockoutActive = false;
            state.KnockoutUntil = 0f;
            state.FullBodyPinClampApplied = false;
            state.EyeClipOverrideApplied = false;
            state.ActiveEyeMode = EyeMode.None;
            state.PendingEyeRestore = false;
            state.PendingDeadRecovery = false;
            state.KnockoutSlowUntil = 0f;
            state.DeadRecoverySlowUntil = 0f;
            state.StandAttemptCount = 0;
            state.NextStandAttemptAt = 0f;

            for (int i = 0; i < AllLimbGroups.Length; i++)
            {
                RestorePinsAndHandlesForGroup(state, AllLimbGroups[i]);
            }
        }

        private static bool IsValidTargetCreature(Creature creature)
        {
            return creature != null && !creature.isPlayer && creature.state != Creature.State.Dead && creature.ragdoll != null;
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
                case RagdollPart.Type.LeftFoot:
                case RagdollPart.Type.RightLeg:
                case RagdollPart.Type.RightFoot:
                    group = LimbGroup.Legs;
                    return true;
                case RagdollPart.Type.LeftArm:
                case RagdollPart.Type.LeftHand:
                case RagdollPart.Type.RightArm:
                case RagdollPart.Type.RightHand:
                case RagdollPart.Type.LeftWing:
                case RagdollPart.Type.RightWing:
                    group = LimbGroup.Arms;
                    return true;
                case RagdollPart.Type.Tail:
                    group = LimbGroup.Legs;
                    return true;
            }

            string token = BuildPartToken(part);
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
            return group == LimbGroup.Legs
                ? RagdollPart.Type.LeftLeg
                : RagdollPart.Type.LeftArm;
        }

        private static RagdollPart.Type GroupToPinMask(LimbGroup group)
        {
            if (group == LimbGroup.Legs)
            {
                return RagdollPart.Type.LeftLeg | RagdollPart.Type.LeftFoot | RagdollPart.Type.RightLeg | RagdollPart.Type.RightFoot | RagdollPart.Type.Tail;
            }

            return RagdollPart.Type.LeftArm | RagdollPart.Type.LeftHand | RagdollPart.Type.RightArm | RagdollPart.Type.RightHand | RagdollPart.Type.LeftWing | RagdollPart.Type.RightWing;
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

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f;
        }

        private static string GroupName(LimbGroup group)
        {
            return group == LimbGroup.Legs ? "legs" : "arms";
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
