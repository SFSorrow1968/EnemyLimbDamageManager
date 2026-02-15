using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace StandaloneReactionAnimationMod.Core
{
    internal sealed partial class SRAMManager
    {
        public static SRAMManager Instance { get; } = new SRAMManager();

        private SRAMManager()
        {
        }

        public void Initialize()
        {
            ClearAllState();
        }

        public void Shutdown()
        {
            ClearAllState();
            RestoreAnimationDataPatches();
        }

        public void Update(float now)
        {
            if (!SRAMOptions.EnableMod)
            {
                if (trackedCreatures.Count > 0)
                {
                    ClearAllState();
                }

                RestoreAnimationDataPatches();
                return;
            }

            if (trackedCreatures.Count == 0)
            {
                return;
            }

            List<int> removeIds = null;
            foreach (KeyValuePair<int, CreatureState> pair in trackedCreatures)
            {
                CreatureState state = pair.Value;
                Creature creature = state.Creature;
                if (creature == null)
                {
                    if (removeIds == null)
                    {
                        removeIds = new List<int>();
                    }

                    removeIds.Add(pair.Key);
                    continue;
                }

                state.LastTouchedAt = now;
                MaybeApplyDeathAnimationDataPatch(state, "update");
                UpdateDeathAnimatorSpeedFallback(state, now);

                // If another system resurrects the creature, clear death-only visual overrides.
                if (state.DeathPresentationApplied && IsAlive(creature))
                {
                    ResetDeathPresentation(state, force: true);
                }
            }

            if (removeIds == null)
            {
                return;
            }

            for (int i = 0; i < removeIds.Count; i++)
            {
                RemoveCreatureState(removeIds[i]);
            }
        }

        public void OnCreatureSpawn(Creature creature)
        {
            if (!SRAMOptions.EnableMod || !IsValidTarget(creature))
            {
                return;
            }

            CreatureState state = EnsureState(creature);
            MaybeApplyDeathAnimationDataPatch(state, "spawn");
        }

        public void OnCreatureKill(Creature creature, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (!SRAMOptions.EnableMod || eventTime != EventTime.OnEnd || creature == null || creature.isPlayer)
            {
                return;
            }

            if (!IsDeadCreature(creature))
            {
                return;
            }

            CreatureState state = EnsureStateAny(creature);
            float now = Time.unscaledTime;
            state.LastTouchedAt = now;
            if (now < state.LastKillHandledAt + KillDedupWindowSeconds)
            {
                return;
            }

            state.LastKillHandledAt = now;
            MaybeApplyDeathAnimationDataPatch(state, "kill");
            StartDeathAnimatorSpeedFallback(state, now);
            SRAMLog.Diag(
                "death_speed_apply creature=" + SafeName(creature) +
                " clipSpeed=" + SRAMOptions.GetDeathAnimationSpeedMultiplier().ToString("0.00") +
                " animatorUntil=" + state.DeathAnimatorSpeedUntil.ToString("0.00"));

            RollDeathEyeProfile(state);
            ApplyDeathPresentation(state);

            SRAMLog.Info(
                "death_presentation_applied creature=" + SafeName(creature) +
                " profile=" + state.EyeProfile +
                " squintVar=" + (state.SquintVariationIndex + 1) + "/" + EyeVariationCount +
                " rollVar=" + (state.RollVariationIndex + 1) + "/" + EyeVariationCount,
                verboseOnly: true);
        }

        public void OnCreatureDespawn(Creature creature, EventTime eventTime)
        {
            if (eventTime != EventTime.OnStart || creature == null)
            {
                return;
            }

            RemoveCreatureState(creature.GetInstanceID());
        }

        public void OnLevelUnload(EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                ClearAllState();
            }
        }
    }
}
