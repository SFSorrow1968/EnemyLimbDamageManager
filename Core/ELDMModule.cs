using System;
using EnemyLimbDamageManager.Configuration;
using ThunderRoad;
using UnityEngine;

namespace EnemyLimbDamageManager.Core
{
    public class ELDMModule : ThunderScript
    {
        public override void ScriptEnable()
        {
            base.ScriptEnable();

            try
            {
                ELDMLog.Info("Ultimate Injury Overhaul (UIO) v" + ELDMModOptions.VERSION + " enabled.");
                ELDMTelemetry.Initialize();
                ELDMSoundManager.Instance.Initialize();
                ELDMModOptionSync.Instance.Initialize();
                EnemyLimbManager.Instance.Initialize();
                Hooks.EventHooks.Subscribe();
            }
            catch (Exception ex)
            {
                ELDMLog.Error("ScriptEnable failed: " + ex.Message);
            }
        }

        public override void ScriptUpdate()
        {
            base.ScriptUpdate();

            try
            {
                ELDMModOptionSync.Instance.Update();
                EnemyLimbManager.Instance.Update();
                ELDMTelemetry.Update(Time.unscaledTime);
            }
            catch (Exception ex)
            {
                ELDMLog.Error("ScriptUpdate failed: " + ex.Message);
            }
        }

        public override void ScriptDisable()
        {
            try
            {
                Hooks.EventHooks.Unsubscribe();
                EnemyLimbManager.Instance.Shutdown();
                ELDMModOptionSync.Instance.Shutdown();
                ELDMSoundManager.Instance.Shutdown();
                ELDMTelemetry.Shutdown();
                ELDMLog.Info("Ultimate Injury Overhaul (UIO) disabled.");
            }
            catch (Exception ex)
            {
                ELDMLog.Error("ScriptDisable failed: " + ex.Message);
            }

            base.ScriptDisable();
        }
    }
}
