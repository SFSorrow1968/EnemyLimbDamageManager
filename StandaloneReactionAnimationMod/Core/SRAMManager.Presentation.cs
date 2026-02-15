using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace StandaloneReactionAnimationMod.Core
{
    internal sealed partial class SRAMManager
    {
        private void ApplyDeathPresentation(CreatureState state)
        {
            if (state?.Creature == null)
            {
                return;
            }

            ApplyEyeClipLock(state, true);
            ApplyEyeProfile(state);
            ApplyMouthOpenFactor(state, SubtleMouthOpenFactor);
            state.DeathPresentationApplied = true;
        }

        private void ApplyEyeProfile(CreatureState state)
        {
            Creature creature = state?.Creature;
            if (creature == null)
            {
                return;
            }

            DeathEyeProfile profile = state.EyeProfile;
            if (profile == DeathEyeProfile.Closed)
            {
                state.RolledEyeClipApplied = false;
                DisableAllEyeClips(creature);
                SetEyesClosedAmount(creature, 1f);
                return;
            }

            bool useSquint = profile == DeathEyeProfile.Squint || profile == DeathEyeProfile.RolledSquint;
            bool useRoll = profile == DeathEyeProfile.Rolled || profile == DeathEyeProfile.RolledSquint;

            float squintClose = Mathf.Clamp(state.SquintCloseAmount, 0.08f, 0.99f);
            float rollClose = Mathf.Clamp(state.RollCloseAmount, 0.08f, 0.99f);

            if (useRoll && !state.RolledEyeClipApplied)
            {
                DisableAllEyeClips(creature);
                state.RolledEyeClipApplied = TryPlayRolledEyeClip(creature);
            }
            else if (!useRoll)
            {
                state.RolledEyeClipApplied = false;
                DisableAllEyeClips(creature);
            }

            float targetClose = 0f;
            if (useSquint)
            {
                targetClose = squintClose;
            }
            if (useRoll)
            {
                targetClose = Mathf.Max(targetClose, rollClose);
            }

            SetEyesClosedAmount(creature, Mathf.Clamp(targetClose <= 0f ? 0.35f : targetClose, 0.08f, 0.99f));
        }

        private void RollDeathEyeProfile(CreatureState state)
        {
            if (state == null)
            {
                return;
            }

            state.SquintVariationIndex = UnityEngine.Random.Range(0, EyeVariationCount);
            state.RollVariationIndex = UnityEngine.Random.Range(0, EyeVariationCount);
            state.SquintCloseAmount = SquintCloseVariations[state.SquintVariationIndex];
            state.RollCloseAmount = RollCloseVariations[state.RollVariationIndex];
            state.RolledEyeClipApplied = false;

            if (UnityEngine.Random.value < ClosedChance)
            {
                state.EyeProfile = DeathEyeProfile.Closed;
                return;
            }

            float roll = UnityEngine.Random.value;
            if (roll < 0.34f)
            {
                state.EyeProfile = DeathEyeProfile.Squint;
            }
            else if (roll < 0.67f)
            {
                state.EyeProfile = DeathEyeProfile.Rolled;
            }
            else
            {
                state.EyeProfile = DeathEyeProfile.RolledSquint;
            }
        }

        private void ApplyMouthOpenFactor(CreatureState state, float factor)
        {
            Creature creature = state?.Creature;
            if (creature == null || creature.jaw == null)
            {
                return;
            }

            if (!state.JawCaptured)
            {
                state.JawOriginalLocalRotation = creature.jaw.localRotation;
                state.JawCaptured = true;
            }

            Vector3 jawMax = creature.jawMaxRotation;
            if (jawMax.sqrMagnitude < 0.01f)
            {
                jawMax = new Vector3(-18f, 0f, 0f);
            }

            float clamped = Mathf.Clamp01(factor);
            creature.jaw.localRotation = state.JawOriginalLocalRotation * Quaternion.Euler(jawMax * clamped);
        }

        private void RestoreMouthPresentation(CreatureState state)
        {
            if (state?.Creature == null || state.Creature.jaw == null || !state.JawCaptured)
            {
                return;
            }

            state.Creature.jaw.localRotation = state.JawOriginalLocalRotation;
        }

        private void ResetDeathPresentation(CreatureState state, bool force)
        {
            if (state == null || (!state.DeathPresentationApplied && !force))
            {
                return;
            }

            RestoreEyePresentation(state);
            RestoreMouthPresentation(state);
            state.DeathPresentationApplied = false;
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

        private static void RestoreEyePresentation(CreatureState state)
        {
            if (state?.Creature == null)
            {
                return;
            }

            ApplyEyeClipLock(state, false);
            DisableAllEyeClips(state.Creature);
            SetEyesClosedAmount(state.Creature, 0f);
            state.RolledEyeClipApplied = false;
        }
    }
}
