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
            LeftLeg = 0,
            RightLeg = 1,
            LeftArm = 2,
            RightArm = 3,
        }

        private struct LimbState
        {
            public float AccumulatedDamage;
            public bool Disabled;
            public float DisabledUntil;
        }

        private sealed class CreatureState
        {
            public Creature Creature;
            public float LastTouchedTime;
            public bool LocomotionClampApplied;
            public bool NoStandModifierApplied;
            public readonly Dictionary<LimbGroup, LimbState> Limbs = new Dictionary<LimbGroup, LimbState>();
            public readonly Dictionary<LimbGroup, List<HandleRagdoll>> DisabledHandles = new Dictionary<LimbGroup, List<HandleRagdoll>>();
        }

        private const float StaleCreatureStateSeconds = 45f;

        private static readonly LimbGroup[] AllLimbGroups =
        {
            LimbGroup.LeftLeg,
            LimbGroup.RightLeg,
            LimbGroup.LeftArm,
            LimbGroup.RightArm,
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
            HandleDiagnosticsToggles();

            if (!ELDMModOptions.EnableMod)
            {
                if (trackedCreatures.Count > 0)
                {
                    ClearAllTrackedState();
                }
                return;
            }

            float now = Time.unscaledTime;
            ProcessRecoveries(now);
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
            if (hitPart == null || !TryMapPartToGroup(hitPart.type, out LimbGroup limbGroup))
            {
                return;
            }

            float trackedDamage = Mathf.Max(0f, collisionInstance.damageStruct.damage);
            if (trackedDamage < ELDMModOptions.GetMinimumTrackedHitDamage())
            {
                return;
            }

            float now = Time.unscaledTime;
            CreatureState state = EnsureCreatureState(creature);
            state.LastTouchedTime = now;

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
                DisableLimb(state, limbGroup, hitPart.type, now);
            }
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
                if (!IsCreatureActive(state.Creature))
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

                if (AnyLimbDisabled(state))
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

        private void DisableLimb(CreatureState state, LimbGroup group, RagdollPart.Type hitType, float now)
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

            if (IsLegGroup(group))
            {
                SetGroupPinsDisabled(state, group);
                UpdateLegMobilityEffects(state, forceFall: ELDMModOptions.FallFromLegInjury);
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
                " sourcePart=" + hitType);
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

            if (IsLegGroup(group))
            {
                UpdateLegMobilityEffects(state, forceFall: false);
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
                if (part == null || part.handles == null || (part.type & mask) == 0)
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

        private void UpdateLegMobilityEffects(CreatureState state, bool forceFall)
        {
            Creature creature = state.Creature;
            if (creature == null)
            {
                return;
            }

            bool legDisabled = IsLimbDisabled(state, LimbGroup.LeftLeg) || IsLimbDisabled(state, LimbGroup.RightLeg);
            if (legDisabled)
            {
                float moveMultiplier = GetLegSquirmMultiplier(state);
                Locomotion locomotion = creature.currentLocomotion != null ? creature.currentLocomotion : creature.locomotion;
                if (locomotion != null)
                {
                    locomotion.SetSpeedModifier(this, moveMultiplier, moveMultiplier, moveMultiplier, moveMultiplier, 1f, moveMultiplier);
                    state.LocomotionClampApplied = true;
                }

                if (creature.brain != null && !state.NoStandModifierApplied)
                {
                    creature.brain.AddNoStandUpModifier(this);
                    state.NoStandModifierApplied = true;
                }

                if (forceFall && creature.ragdoll != null && creature.ragdoll.state == Ragdoll.State.Standing)
                {
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized, true);
                }

                ApplyLegPinState(state);
                return;
            }

            RemoveLegMobilityEffects(state);
            if (ELDMModOptions.RecoveryRestoresPinForces)
            {
                RestorePinsAndHandlesForGroup(state, LimbGroup.LeftLeg);
                RestorePinsAndHandlesForGroup(state, LimbGroup.RightLeg);
            }
        }

        private void ApplyLegPinState(CreatureState state)
        {
            if (!ELDMModOptions.LegImmobilization)
            {
                return;
            }

            if (IsLimbDisabled(state, LimbGroup.LeftLeg))
            {
                SetGroupPinsDisabled(state, LimbGroup.LeftLeg);
            }
            else if (ELDMModOptions.RecoveryRestoresPinForces)
            {
                RestorePinsAndHandlesForGroup(state, LimbGroup.LeftLeg);
            }

            if (IsLimbDisabled(state, LimbGroup.RightLeg))
            {
                SetGroupPinsDisabled(state, LimbGroup.RightLeg);
            }
            else if (ELDMModOptions.RecoveryRestoresPinForces)
            {
                RestorePinsAndHandlesForGroup(state, LimbGroup.RightLeg);
            }
        }

        private void SetGroupPinsDisabled(CreatureState state, LimbGroup group)
        {
            if (state?.Creature == null || state.Creature.ragdoll == null)
            {
                return;
            }

            bool shouldDisablePins = IsLegGroup(group) ? ELDMModOptions.LegImmobilization : ELDMModOptions.ArmImmobilization;
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
                bool hadPinned = IsLegGroup(group) ? ELDMModOptions.LegImmobilization : ELDMModOptions.ArmImmobilization;
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

        private void RemoveLegMobilityEffects(CreatureState state)
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

            state.LocomotionClampApplied = false;
            state.NoStandModifierApplied = false;
        }

        private void HandleDiagnosticsToggles()
        {
            bool refreshed = false;

            if (ELDMModOptions.ResetTracking)
            {
                ELDMModOptions.ResetTracking = false;
                ClearAllTrackedState();
                ELDMTelemetry.ResetTrackingCounters();
                ELDMLog.Info("Tracking reset from diagnostics menu.");
                refreshed = true;
            }

            if (refreshed)
            {
                ModManager.RefreshModOptionsUI();
            }
        }

        private CreatureState EnsureCreatureState(Creature creature)
        {
            int creatureId = creature.GetInstanceID();
            if (!trackedCreatures.TryGetValue(creatureId, out CreatureState state))
            {
                state = new CreatureState
                {
                    Creature = creature,
                    LastTouchedTime = Time.unscaledTime,
                    LocomotionClampApplied = false,
                    NoStandModifierApplied = false,
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
            if (state == null)
            {
                return;
            }

            RemoveLegMobilityEffects(state);

            if (!ELDMModOptions.RecoveryRestoresPinForces)
            {
                return;
            }

            for (int i = 0; i < AllLimbGroups.Length; i++)
            {
                RestorePinsAndHandlesForGroup(state, AllLimbGroups[i]);
            }
        }

        private static bool IsValidTargetCreature(Creature creature)
        {
            return creature != null && !creature.isPlayer && creature.state != Creature.State.Dead && creature.ragdoll != null;
        }

        private static bool IsCreatureActive(Creature creature)
        {
            return creature != null && creature.state != Creature.State.Dead;
        }

        private static bool TryMapPartToGroup(RagdollPart.Type partType, out LimbGroup group)
        {
            switch (partType)
            {
                case RagdollPart.Type.LeftLeg:
                case RagdollPart.Type.LeftFoot:
                    group = LimbGroup.LeftLeg;
                    return true;
                case RagdollPart.Type.RightLeg:
                case RagdollPart.Type.RightFoot:
                    group = LimbGroup.RightLeg;
                    return true;
                case RagdollPart.Type.LeftArm:
                case RagdollPart.Type.LeftHand:
                    group = LimbGroup.LeftArm;
                    return true;
                case RagdollPart.Type.RightArm:
                case RagdollPart.Type.RightHand:
                    group = LimbGroup.RightArm;
                    return true;
                default:
                    group = LimbGroup.LeftLeg;
                    return false;
            }
        }

        private static RagdollPart.Type GroupToRepresentativePart(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.LeftLeg:
                    return RagdollPart.Type.LeftLeg;
                case LimbGroup.RightLeg:
                    return RagdollPart.Type.RightLeg;
                case LimbGroup.LeftArm:
                    return RagdollPart.Type.LeftArm;
                case LimbGroup.RightArm:
                    return RagdollPart.Type.RightArm;
                default:
                    return RagdollPart.Type.Torso;
            }
        }

        private static RagdollPart.Type GroupToPinMask(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.LeftLeg:
                    return RagdollPart.Type.LeftLeg | RagdollPart.Type.LeftFoot;
                case LimbGroup.RightLeg:
                    return RagdollPart.Type.RightLeg | RagdollPart.Type.RightFoot;
                case LimbGroup.LeftArm:
                    return RagdollPart.Type.LeftArm | RagdollPart.Type.LeftHand;
                case LimbGroup.RightArm:
                    return RagdollPart.Type.RightArm | RagdollPart.Type.RightHand;
                default:
                    return (RagdollPart.Type)0;
            }
        }

        private static bool IsLegGroup(LimbGroup group)
        {
            return group == LimbGroup.LeftLeg || group == LimbGroup.RightLeg;
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

        private static float GetLegSquirmMultiplier(CreatureState state)
        {
            bool leftDisabled = IsLimbDisabled(state, LimbGroup.LeftLeg);
            bool rightDisabled = IsLimbDisabled(state, LimbGroup.RightLeg);
            if (leftDisabled && rightDisabled)
            {
                float left = ELDMModOptions.GetLimbSquirmMultiplier(RagdollPart.Type.LeftLeg);
                float right = ELDMModOptions.GetLimbSquirmMultiplier(RagdollPart.Type.RightLeg);
                return Mathf.Clamp01((left + right) * 0.5f);
            }

            if (leftDisabled)
            {
                return ELDMModOptions.GetLimbSquirmMultiplier(RagdollPart.Type.LeftLeg);
            }

            if (rightDisabled)
            {
                return ELDMModOptions.GetLimbSquirmMultiplier(RagdollPart.Type.RightLeg);
            }

            return 1f;
        }

        private static string GroupName(LimbGroup group)
        {
            switch (group)
            {
                case LimbGroup.LeftLeg:
                    return "left_leg";
                case LimbGroup.RightLeg:
                    return "right_leg";
                case LimbGroup.LeftArm:
                    return "left_arm";
                case LimbGroup.RightArm:
                    return "right_arm";
                default:
                    return "unknown";
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
