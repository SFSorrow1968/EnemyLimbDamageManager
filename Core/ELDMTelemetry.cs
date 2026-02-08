using System;
using System.Collections.Generic;
using EnemyLimbDamageManager.Configuration;
using UnityEngine;

namespace EnemyLimbDamageManager.Core
{
    internal static class ELDMTelemetry
    {
        private const float HitLogIntervalSeconds = 0.25f;
        private const float SummaryIntervalSeconds = 30f;

        private static readonly Dictionary<string, float> hitLogGate = new Dictionary<string, float>();

        private static bool initialized;
        private static string runId = "none";
        private static float sessionStartTime;
        private static float nextSummaryTime;
        private static int summaryCount;

        private static int intervalHits;
        private static int intervalDisables;
        private static int intervalRecovers;
        private static int intervalTrackedCreatures;

        private static int totalHits;
        private static int totalDisables;
        private static int totalRecovers;
        private static int peakTrackedCreatures;

        public static void Initialize()
        {
            hitLogGate.Clear();
            initialized = true;
            runId = Guid.NewGuid().ToString("N").Substring(0, 8);
            sessionStartTime = Time.unscaledTime;
            summaryCount = 0;
            nextSummaryTime = sessionStartTime + SummaryIntervalSeconds;
            ResetIntervalCounters();
            ResetTotalCounters();

            ELDMLog.Diag(
                "diag evt=session_start run=" + runId +
                " presetHash=" + ELDMModOptions.GetPresetSelectionHash() +
                " sourceHash=" + ELDMModOptions.GetSourceOfTruthHash());
        }

        public static void Shutdown()
        {
            if (!initialized)
            {
                hitLogGate.Clear();
                return;
            }

            EmitSummary(force: true);
            EmitSessionTotals();
            ELDMLog.Diag(
                "diag evt=session_end run=" + runId +
                " uptimeSec=" + Mathf.Max(0f, Time.unscaledTime - sessionStartTime).ToString("F1") +
                " summaryCount=" + summaryCount);

            hitLogGate.Clear();
            initialized = false;
            nextSummaryTime = 0f;
        }

        public static void ResetTrackingCounters()
        {
            ResetIntervalCounters();
            ResetTotalCounters();
            summaryCount = 0;
            sessionStartTime = Time.unscaledTime;
            nextSummaryTime = sessionStartTime + SummaryIntervalSeconds;
        }

        public static bool ShouldLogHit(int creatureId, string limbGroup, float now)
        {
            string key = creatureId + ":" + (limbGroup ?? "unknown");
            if (hitLogGate.TryGetValue(key, out float nextAllowed) && now < nextAllowed)
            {
                return false;
            }

            hitLogGate[key] = now + HitLogIntervalSeconds;
            return true;
        }

        public static void RecordTrackedCreatures(int trackedCreatures)
        {
            if (!initialized)
            {
                return;
            }

            intervalTrackedCreatures = trackedCreatures;
            if (trackedCreatures > peakTrackedCreatures)
            {
                peakTrackedCreatures = trackedCreatures;
            }
        }

        public static void RecordHit()
        {
            if (!initialized)
            {
                return;
            }

            intervalHits++;
            totalHits++;
        }

        public static void RecordDisable()
        {
            if (!initialized)
            {
                return;
            }

            intervalDisables++;
            totalDisables++;
        }

        public static void RecordRecover()
        {
            if (!initialized)
            {
                return;
            }

            intervalRecovers++;
            totalRecovers++;
        }

        public static void Update(float now)
        {
            if (!initialized || now < nextSummaryTime)
            {
                return;
            }

            EmitSummary(force: false);
            nextSummaryTime = now + SummaryIntervalSeconds;
        }

        private static void EmitSummary(bool force)
        {
            if (!force && intervalHits == 0 && intervalDisables == 0 && intervalRecovers == 0)
            {
                return;
            }

            summaryCount++;
            ELDMLog.Diag(
                "diag evt=summary run=" + runId +
                " intervalSec=" + SummaryIntervalSeconds.ToString("F0") +
                " hits=" + intervalHits +
                " disables=" + intervalDisables +
                " recovers=" + intervalRecovers +
                " trackedCreatures=" + intervalTrackedCreatures +
                " peakTracked=" + peakTrackedCreatures);

            ResetIntervalCounters();
        }

        private static void EmitSessionTotals()
        {
            float uptime = Mathf.Max(0f, Time.unscaledTime - sessionStartTime);
            ELDMLog.Diag(
                "diag evt=session_totals run=" + runId +
                " uptimeSec=" + uptime.ToString("F1") +
                " summaryCount=" + summaryCount +
                " hits=" + totalHits +
                " disables=" + totalDisables +
                " recovers=" + totalRecovers +
                " peakTracked=" + peakTrackedCreatures);
        }

        private static void ResetIntervalCounters()
        {
            intervalHits = 0;
            intervalDisables = 0;
            intervalRecovers = 0;
            intervalTrackedCreatures = 0;
        }

        private static void ResetTotalCounters()
        {
            totalHits = 0;
            totalDisables = 0;
            totalRecovers = 0;
            peakTrackedCreatures = 0;
        }
    }
}
