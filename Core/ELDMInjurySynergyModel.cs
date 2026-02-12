using UnityEngine;

namespace EnemyLimbDamageManager.Core
{
    public struct ELDMConflictInput
    {
        public bool PendingDeadRecovery;
        public bool KnockoutActive;
        public bool HipDisabled;
        public bool TorsoParalyzed;
        public bool LegDisabled;
        public bool LegImmobilizationEnabled;
    }

    public struct ELDMConflictResolution
    {
        public bool PreventStand;
        public bool ForceDestabilize;
        public bool HardLockSpeed;
    }

    public static class ELDMInjurySynergyModel
    {
        public static ELDMConflictResolution ResolveConflict(ELDMConflictInput input)
        {
            bool hardLock = input.KnockoutActive;
            bool preventStand = hardLock || (input.LegDisabled && input.LegImmobilizationEnabled);

            return new ELDMConflictResolution
            {
                PreventStand = preventStand,
                ForceDestabilize = input.LegDisabled && input.LegImmobilizationEnabled,
                HardLockSpeed = hardLock,
            };
        }

        public static float ComputeTraumaDurationMultiplier(int disableCount, float increasePerDisableRatio, float maxBonusRatio)
        {
            int events = Mathf.Max(0, disableCount);
            if (events <= 0)
            {
                return 1f;
            }

            float increase = events * Mathf.Clamp01(increasePerDisableRatio);
            float maxIncrease = Mathf.Max(0f, maxBonusRatio);
            float clampedIncrease = Mathf.Min(increase, maxIncrease);
            return 1f + clampedIncrease;
        }

        public static bool ShouldBecomePermanentDisable(int disableCount, int permanentAfterCount)
        {
            if (permanentAfterCount <= 0)
            {
                return false;
            }

            return disableCount >= permanentAfterCount;
        }
    }
}
