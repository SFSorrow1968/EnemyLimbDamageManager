using System;
using EnemyLimbDamageManager.Core;
using ThunderRoad;

namespace EnemyLimbDamageManager.Hooks
{
    public sealed class EventHooks
    {
        private static EventHooks instance;

        private bool subscribed;
        private EventManager.CreatureSpawnedEvent onCreatureSpawnHandler;
        private EventManager.CreatureHitEvent onCreatureHitHandler;
        private EventManager.CreatureDespawnedEvent onCreatureDespawnHandler;
        private EventManager.LevelLoadEvent onLevelUnloadHandler;

        public static void Subscribe()
        {
            if (instance == null)
            {
                instance = new EventHooks();
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
                onCreatureHitHandler = OnCreatureHit;
                onCreatureDespawnHandler = OnCreatureDespawn;
                onLevelUnloadHandler = OnLevelUnload;

                EventManager.onCreatureSpawn += onCreatureSpawnHandler;
                EventManager.onCreatureHit += onCreatureHitHandler;
                EventManager.onCreatureDespawn += onCreatureDespawnHandler;
                EventManager.onLevelUnload += onLevelUnloadHandler;

                subscribed = true;
                ELDMLog.Info("Event hooks subscribed.");
            }
            catch (Exception ex)
            {
                ELDMLog.Error("Failed to subscribe hooks: " + ex.Message);
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
                if (onCreatureHitHandler != null)
                {
                    EventManager.onCreatureHit -= onCreatureHitHandler;
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
                ELDMLog.Warn("Failed to fully unsubscribe hooks: " + ex.Message);
            }

            onCreatureSpawnHandler = null;
            onCreatureHitHandler = null;
            onCreatureDespawnHandler = null;
            onLevelUnloadHandler = null;
            subscribed = false;
        }

        private void OnCreatureSpawn(Creature creature)
        {
            EnemyLimbManager.Instance.OnCreatureSpawn(creature);
        }

        private void OnCreatureHit(Creature creature, CollisionInstance collisionInstance, EventTime eventTime)
        {
            EnemyLimbManager.Instance.OnCreatureHit(creature, collisionInstance, eventTime);
        }

        private void OnCreatureDespawn(Creature creature, EventTime eventTime)
        {
            EnemyLimbManager.Instance.OnCreatureDespawn(creature, eventTime);
        }

        private void OnLevelUnload(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
        {
            EnemyLimbManager.Instance.OnLevelUnload(levelData, mode, eventTime);
        }
    }
}
