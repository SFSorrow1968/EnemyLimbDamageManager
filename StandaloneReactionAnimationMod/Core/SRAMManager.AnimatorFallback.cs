using UnityEngine;

namespace StandaloneReactionAnimationMod.Core
{
    internal sealed partial class SRAMManager
    {
        private void StartDeathAnimatorSpeedFallback(CreatureState state, float now)
        {
            if (state == null)
            {
                return;
            }

            float speed = SRAMOptions.GetDeathAnimationSpeedMultiplier();
            state.DeathAnimatorSpeedUntil = now + ComputeDeathAnimationSpeedWindowSeconds(speed);
            ApplyDeathAnimatorSpeedOverride(state, speed);
        }

        private void UpdateDeathAnimatorSpeedFallback(CreatureState state, float now)
        {
            if (state?.Creature == null)
            {
                return;
            }

            if (IsDeadCreature(state.Creature) && now < state.DeathAnimatorSpeedUntil)
            {
                ApplyDeathAnimatorSpeedOverride(state, SRAMOptions.GetDeathAnimationSpeedMultiplier());
                return;
            }

            ClearDeathAnimatorSpeedOverride(state);
        }

        private static void ApplyDeathAnimatorSpeedOverride(CreatureState state, float speed)
        {
            if (state?.Creature == null)
            {
                return;
            }

            if (state.Creature.animator == null)
            {
                SRAMLog.Diag(
                    "death_animator_speed_skipped creature=" + SafeName(state.Creature) +
                    " reason=missing_animator",
                    verboseOnly: true);
                return;
            }

            Animator animator = state.Creature.animator;
            if (!state.DeathAnimatorSpeedOverrideApplied)
            {
                state.DeathAnimatorOriginalSpeed = animator.speed;
                state.DeathAnimatorSpeedOverrideApplied = true;
            }

            float clamped = Mathf.Clamp(speed, 0.05f, 2f);
            if (Mathf.Abs(animator.speed - clamped) > 0.0001f)
            {
                animator.speed = clamped;
            }
        }

        private static void ClearDeathAnimatorSpeedOverride(CreatureState state)
        {
            if (state == null || !state.DeathAnimatorSpeedOverrideApplied)
            {
                return;
            }

            if (state.Creature?.animator != null)
            {
                float restoreSpeed = state.DeathAnimatorOriginalSpeed > 0f ? state.DeathAnimatorOriginalSpeed : 1f;
                state.Creature.animator.speed = restoreSpeed;
            }

            state.DeathAnimatorOriginalSpeed = 1f;
            state.DeathAnimatorSpeedOverrideApplied = false;
            state.DeathAnimatorSpeedUntil = 0f;
        }

        private static float ComputeDeathAnimationSpeedWindowSeconds(float deathAnimSpeed)
        {
            float clampedSpeed = Mathf.Max(0.05f, deathAnimSpeed);
            return Mathf.Clamp((3.6f / clampedSpeed) + 0.35f, 1.5f, 12f);
        }
    }
}
