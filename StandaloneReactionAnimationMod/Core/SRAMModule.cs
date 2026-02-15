using System;
using ThunderRoad;
using UnityEngine;

namespace StandaloneReactionAnimationMod.Core
{
    public sealed class SRAMModule : ThunderScript
    {
        public override void ScriptEnable()
        {
            base.ScriptEnable();

            try
            {
                SRAMLog.Info("Standalone Reaction Animation v" + SRAMOptions.Version + " enabled.");
                SRAMManager.Instance.Initialize();
                Hooks.SRAMEventHooks.Subscribe();
            }
            catch (Exception ex)
            {
                SRAMLog.Error("ScriptEnable failed: " + ex.Message);
            }
        }

        public override void ScriptUpdate()
        {
            base.ScriptUpdate();

            try
            {
                SRAMManager.Instance.Update(Time.unscaledTime);
            }
            catch (Exception ex)
            {
                SRAMLog.Error("ScriptUpdate failed: " + ex.Message);
            }
        }

        public override void ScriptDisable()
        {
            try
            {
                Hooks.SRAMEventHooks.Unsubscribe();
                SRAMManager.Instance.Shutdown();
                SRAMLog.Info("Standalone Reaction Animation disabled.");
            }
            catch (Exception ex)
            {
                SRAMLog.Error("ScriptDisable failed: " + ex.Message);
            }

            base.ScriptDisable();
        }
    }
}
