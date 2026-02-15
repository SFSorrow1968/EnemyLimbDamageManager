using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace StandaloneReactionAnimationMod.Core
{
    internal sealed partial class SRAMManager
    {
        private enum DeathEyeProfile
        {
            Closed = 0,
            Squint = 1,
            Rolled = 2,
            RolledSquint = 3,
        }

        private sealed class CreatureState
        {
            public Creature Creature;
            public float LastTouchedAt;
            public float LastKillHandledAt;

            public bool DeathPresentationApplied;

            public bool EyeClipOverrideApplied;
            public bool EyeClipAutoWasActive;
            public bool RolledEyeClipApplied;
            public DeathEyeProfile EyeProfile;
            public int SquintVariationIndex;
            public int RollVariationIndex;
            public float SquintCloseAmount;
            public float RollCloseAmount;

            public bool JawCaptured;
            public Quaternion JawOriginalLocalRotation;

            public bool DeathDataSynced;
            public bool MissingDeathModuleLogged;
            public float LastAppliedDeathSpeed;

            public bool DeathAnimatorSpeedOverrideApplied;
            public float DeathAnimatorOriginalSpeed;
            public float DeathAnimatorSpeedUntil;
        }

        private const float ClosedChance = 0.20f;
        private const float KillDedupWindowSeconds = 0.25f;
        private const float SubtleMouthOpenFactor = 0.14f;
        private const int EyeVariationCount = 20;

        // Squint never goes below 40% closed, but can exceed 60% closed.
        private static readonly float[] SquintCloseVariations = BuildVariations(0.40f, 0.85f, EyeVariationCount);
        private static readonly float[] RollCloseVariations = BuildVariations(0.10f, 0.70f, EyeVariationCount);

        private readonly Dictionary<int, CreatureState> trackedCreatures = new Dictionary<int, CreatureState>();

        private CreatureState EnsureState(Creature creature)
        {
            return EnsureStateAny(creature);
        }

        private CreatureState EnsureStateAny(Creature creature)
        {
            if (creature == null)
            {
                return null;
            }

            int id = creature.GetInstanceID();
            CreatureState state;
            if (!trackedCreatures.TryGetValue(id, out state))
            {
                state = new CreatureState
                {
                    Creature = creature,
                    LastTouchedAt = Time.unscaledTime,
                    SquintCloseAmount = SquintCloseVariations[0],
                    RollCloseAmount = RollCloseVariations[0],
                    EyeProfile = DeathEyeProfile.Rolled,
                };
                trackedCreatures[id] = state;
            }
            else
            {
                state.Creature = creature;
                state.LastTouchedAt = Time.unscaledTime;
            }

            return state;
        }

        private void RemoveCreatureState(int creatureId)
        {
            CreatureState state;
            if (!trackedCreatures.TryGetValue(creatureId, out state))
            {
                return;
            }

            ResetDeathPresentation(state, force: true);
            ClearDeathAnimatorSpeedOverride(state);
            trackedCreatures.Remove(creatureId);
        }

        private void ClearAllState()
        {
            if (trackedCreatures.Count == 0)
            {
                return;
            }

            List<int> ids = new List<int>(trackedCreatures.Keys);
            for (int i = 0; i < ids.Count; i++)
            {
                RemoveCreatureState(ids[i]);
            }
        }

        private static bool IsValidTarget(Creature creature)
        {
            return creature != null && !creature.isPlayer;
        }

        private static bool IsDeadCreature(Creature creature)
        {
            return creature != null && creature.state == Creature.State.Dead;
        }

        private static bool IsAlive(Creature creature)
        {
            return creature != null && creature.state != Creature.State.Dead;
        }

        private static float[] BuildVariations(float minInclusive, float maxInclusive, int count)
        {
            float[] result = new float[count];
            if (count <= 1)
            {
                result[0] = minInclusive;
                return result;
            }

            float range = maxInclusive - minInclusive;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)(count - 1);
                result[i] = minInclusive + (range * t);
            }

            return result;
        }
    }
}
