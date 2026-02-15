using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace StandaloneReactionAnimationMod.Core
{
    internal sealed partial class SRAMManager
    {
        private const float FrequentChanceFloor = 99f;
        private const float FrequentWeightFloor = 5f;

        private static readonly HashSet<string> KnownDeathAnimationAddresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Bas.Animation.Death.SimpleBackward",
            "Bas.Animation.Death.SimpleForward",
            "Bas.Animation.Death.SimpleLeft",
            "Bas.Animation.Death.SimpleRight",
            "Bas.Animation.Death_1hFront",
            "Bas.Animation.Death_2",
            "Bas.Animation.Death_back",
            "Bas.Animation.Death_Backward",
            "Bas.Animation.Death_Backward2",
            "Bas.Animation.Death_Backward3",
            "Bas.Animation.Death_BackwardBow",
            "Bas.Animation.Death_Forward",
            "Bas.Animation.Death_ForwardBow",
            "Bas.Animation.Death_KneelBackward",
            "Bas.Animation.Death_KneelForward",
            "Bas.Animation.Death_KneelForward2",
            "Bas.Animation.Death_KneelForward3",
            "Bas.Animation.Death_KneelMid",
            "Bas.Animation.Death_Left",
            "Bas.Animation.Death_Left2",
            "Bas.Animation.Death_Right_Bow",
            "Bas.Animation.Death_Right_Sword",
            "Bas.Animation.Death_RSword",
            "Bas.Animation.Death1",
            "Bas.Animation.Death2",
        };

        private readonly Dictionary<AnimationData.Clip, float> originalClipSpeed = new Dictionary<AnimationData.Clip, float>();
        private readonly Dictionary<AnimationData.Clip, float> originalClipChance = new Dictionary<AnimationData.Clip, float>();
        private readonly Dictionary<AnimationData.Clip, float> originalClipWeight = new Dictionary<AnimationData.Clip, float>();

        private void MaybeApplyDeathAnimationDataPatch(CreatureState state, string reason)
        {
            if (state?.Creature == null || state.Creature.brain == null || state.Creature.brain.instance == null)
            {
                return;
            }

            float targetSpeed = SRAMOptions.GetDeathAnimationSpeedMultiplier();
            bool speedChanged = !state.DeathDataSynced || Mathf.Abs(state.LastAppliedDeathSpeed - targetSpeed) > 0.001f;
            if (!speedChanged)
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
                if (!state.MissingDeathModuleLogged)
                {
                    SRAMLog.Warn(
                        "death_patch_skipped creature=" + SafeName(state.Creature) +
                        " reason=module_lookup_failed error=" + ex.Message);
                    state.MissingDeathModuleLogged = true;
                }
                return;
            }

            if (deathModule == null)
            {
                if (!state.MissingDeathModuleLogged)
                {
                    SRAMLog.Info(
                        "death_patch_skipped creature=" + SafeName(state.Creature) +
                        " reason=missing_brain_module_death",
                        verboseOnly: true);
                    state.MissingDeathModuleLogged = true;
                }
                return;
            }

            state.MissingDeathModuleLogged = false;

            int clipsTouched = 0;
            int clipsChanged = 0;
            int knownAddressHits = 0;
            PatchAnimationData(deathModule.defaultAnimationData, targetSpeed, ref clipsTouched, ref clipsChanged, ref knownAddressHits);
            PatchAnimationData(deathModule.stabAnimationData, targetSpeed, ref clipsTouched, ref clipsChanged, ref knownAddressHits);
            PatchAnimationData(deathModule.fireAnimationData, targetSpeed, ref clipsTouched, ref clipsChanged, ref knownAddressHits);
            PatchAnimationData(deathModule.lightningAnimationData, targetSpeed, ref clipsTouched, ref clipsChanged, ref knownAddressHits);

            state.DeathDataSynced = true;
            state.LastAppliedDeathSpeed = targetSpeed;

            SRAMLog.Info(
                "death_patch_applied creature=" + SafeName(state.Creature) +
                " speed=" + targetSpeed.ToString("0.00") +
                " clipsTouched=" + clipsTouched +
                " clipsChanged=" + clipsChanged +
                " knownAddressHits=" + knownAddressHits +
                " reason=" + reason,
                verboseOnly: true);
        }

        private void PatchAnimationData(
            AnimationData animationData,
            float speed,
            ref int clipsTouched,
            ref int clipsChanged,
            ref int knownAddressHits)
        {
            if (animationData?.animationClips == null)
            {
                return;
            }

            float clampedSpeed = Mathf.Clamp(speed, 0.05f, 2f);
            for (int i = 0; i < animationData.animationClips.Count; i++)
            {
                AnimationData.Clip clip = animationData.animationClips[i];
                if (clip == null)
                {
                    continue;
                }

                clipsTouched++;
                if (!originalClipSpeed.ContainsKey(clip))
                {
                    originalClipSpeed[clip] = clip.animationSpeed;
                }
                if (!originalClipChance.ContainsKey(clip))
                {
                    originalClipChance[clip] = clip.chance;
                }
                if (!originalClipWeight.ContainsKey(clip))
                {
                    originalClipWeight[clip] = clip.weight;
                }

                if (Mathf.Abs(clip.animationSpeed - clampedSpeed) > 0.0001f)
                {
                    clip.animationSpeed = clampedSpeed;
                    clipsChanged++;
                }

                if (!string.IsNullOrWhiteSpace(clip.address) && KnownDeathAnimationAddresses.Contains(clip.address))
                {
                    knownAddressHits++;
                }

                float desiredChance = Mathf.Max(clip.chance, FrequentChanceFloor);
                if (Mathf.Abs(clip.chance - desiredChance) > 0.0001f)
                {
                    clip.chance = desiredChance;
                    clipsChanged++;
                }

                float desiredWeight = Mathf.Max(clip.weight, FrequentWeightFloor);
                if (Mathf.Abs(clip.weight - desiredWeight) > 0.0001f)
                {
                    clip.weight = desiredWeight;
                    clipsChanged++;
                }
            }
        }

        private void RestoreAnimationDataPatches()
        {
            int restoredSpeed = 0;
            foreach (KeyValuePair<AnimationData.Clip, float> pair in originalClipSpeed)
            {
                AnimationData.Clip clip = pair.Key;
                if (clip == null)
                {
                    continue;
                }

                if (Mathf.Abs(clip.animationSpeed - pair.Value) <= 0.0001f)
                {
                    continue;
                }

                clip.animationSpeed = pair.Value;
                restoredSpeed++;
            }

            int restoredChance = 0;
            foreach (KeyValuePair<AnimationData.Clip, float> pair in originalClipChance)
            {
                AnimationData.Clip clip = pair.Key;
                if (clip == null)
                {
                    continue;
                }

                if (Mathf.Abs(clip.chance - pair.Value) <= 0.0001f)
                {
                    continue;
                }

                clip.chance = pair.Value;
                restoredChance++;
            }

            int restoredWeight = 0;
            foreach (KeyValuePair<AnimationData.Clip, float> pair in originalClipWeight)
            {
                AnimationData.Clip clip = pair.Key;
                if (clip == null)
                {
                    continue;
                }

                if (Mathf.Abs(clip.weight - pair.Value) <= 0.0001f)
                {
                    continue;
                }

                clip.weight = pair.Value;
                restoredWeight++;
            }

            originalClipSpeed.Clear();
            originalClipChance.Clear();
            originalClipWeight.Clear();

            foreach (CreatureState state in trackedCreatures.Values)
            {
                state.DeathDataSynced = false;
                state.LastAppliedDeathSpeed = 0f;
                state.MissingDeathModuleLogged = false;
            }

            if (restoredSpeed > 0 || restoredChance > 0 || restoredWeight > 0)
            {
                SRAMLog.Info(
                    "death_patch_restored speed=" + restoredSpeed +
                    " chance=" + restoredChance +
                    " weight=" + restoredWeight,
                    verboseOnly: true);
            }
        }
    }
}
