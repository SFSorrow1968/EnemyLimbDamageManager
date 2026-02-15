using System;
using StandaloneReactionAnimationMod.Core;
using ThunderRoad;

namespace StandaloneReactionAnimationMod.Hooks
{
    internal sealed class SRAMEventHooks
    {
        private static SRAMEventHooks instance;

        private bool subscribed;
        private EventManager.CreatureSpawnedEvent onCreatureSpawnHandler;
        private EventManager.CreatureKillEvent onCreatureKillHandler;
        private EventManager.CreatureDespawnedEvent onCreatureDespawnHandler;
        private EventManager.LevelLoadEvent onLevelUnloadHandler;

        public static void Subscribe()
        {
            if (instance == null)
            {
                instance = new SRAMEventHooks();
            }

            instance.SubscribeInternal();
        }

        public static void Unsubscribe()
        {
            instance?.UnsubscribeInternal();
        }

        private void SubscribeInternal()
        {
            if (subscribed)
            {
                return;
            }

            try
            {
                onCreatureSpawnHandler = OnCreatureSpawn;
                onCreatureKillHandler = OnCreatureKill;
                onCreatureDespawnHandler = OnCreatureDespawn;
                onLevelUnloadHandler = OnLevelUnload;

                EventManager.onCreatureSpawn += onCreatureSpawnHandler;
                EventManager.onCreatureKill += onCreatureKillHandler;
                EventManager.onCreatureDespawn += onCreatureDespawnHandler;
                EventManager.onLevelUnload += onLevelUnloadHandler;

                subscribed = true;
                SRAMLog.Info("Event hooks subscribed.");
            }
            catch (Exception ex)
            {
                SRAMLog.Error("Failed to subscribe hooks: " + ex.Message);
            }
        }

        private void UnsubscribeInternal()
        {
            if (!subscribed)
            {
                return;
            }

            try
            {
                if (onCreatureSpawnHandler != null)
                {
                    EventManager.onCreatureSpawn -= onCreatureSpawnHandler;
                }
                if (onCreatureKillHandler != null)
                {
                    EventManager.onCreatureKill -= onCreatureKillHandler;
                }
                if (onCreatureDespawnHandler != null)
                {
                    EventManager.onCreatureDespawn -= onCreatureDespawnHandler;
                }
                if (onLevelUnloadHandler != null)
                {
                    EventManager.onLevelUnload -= onLevelUnloadHandler;
                }
            }
            catch (Exception ex)
            {
                SRAMLog.Warn("Failed to fully unsubscribe hooks: " + ex.Message);
            }

            onCreatureSpawnHandler = null;
            onCreatureKillHandler = null;
            onCreatureDespawnHandler = null;
            onLevelUnloadHandler = null;
            subscribed = false;
        }

        private static void OnCreatureSpawn(Creature creature)
        {
            SRAMManager.Instance.OnCreatureSpawn(creature);
        }

        private static void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            SRAMManager.Instance.OnCreatureKill(creature, collisionInstance, eventTime);
        }

        private static void OnCreatureDespawn(Creature creature, EventTime eventTime)
        {
            SRAMManager.Instance.OnCreatureDespawn(creature, eventTime);
        }

        private static void OnLevelUnload(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
        {
            SRAMManager.Instance.OnLevelUnload(eventTime);
        }
    }
}
