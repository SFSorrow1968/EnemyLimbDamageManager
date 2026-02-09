using EnemyLimbDamageManager.Configuration;
using NUnit.Framework;

namespace EnemyLimbDamageManager.Tests
{
    [TestFixture]
    public class ELDMModOptionsTests
    {
        [SetUp]
        public void SetUp()
        {
            ELDMModOptions.EnableMod = true;
            ELDMModOptions.PresetDamageModel = ELDMModOptions.PresetDamageDefault;
            ELDMModOptions.PresetLimbSlowModel = ELDMModOptions.PresetLimbSlowDefault;
            ELDMModOptions.PresetLastStandModel = ELDMModOptions.PresetLastStandDefault;
            ELDMModOptions.PresetLastStandSlowModel = ELDMModOptions.PresetLastStandSlowDefault;
            ELDMModOptions.EnableBasicLogging = true;
            ELDMModOptions.EnableDiagnosticsLogging = false;
            ELDMModOptions.EnableVerboseLogging = false;
        }

        [Test]
        public void Version_IsSemverLike()
        {
            Assert.That(ELDMModOptions.VERSION, Is.Not.Null.And.Not.Empty);
            Assert.That(ELDMModOptions.VERSION, Does.Match("^\\d+\\.\\d+\\.\\d+"));
        }

        [Test]
        public void HardcodedHitTrackingThreshold_IsPointOne()
        {
            Assert.That(ELDMModOptions.GetMinimumTrackedHitDamage(), Is.EqualTo(0.10f));
        }

        [Test]
        public void BooleanLogging_DefaultsAreSensible()
        {
            Assert.That(ELDMModOptions.EnableBasicLogging, Is.True);
            Assert.That(ELDMModOptions.EnableDiagnosticsLogging, Is.False);
            Assert.That(ELDMModOptions.EnableVerboseLogging, Is.False);
        }

        [Test]
        public void PresetNormalization_MapsAliases()
        {
            Assert.That(ELDMModOptions.NormalizeDamagePreset("brutal"), Is.EqualTo(ELDMModOptions.PresetDamageBrutal));
            Assert.That(ELDMModOptions.NormalizeDamagePreset("Extreme"), Is.EqualTo(ELDMModOptions.PresetDamageExtreme));
            Assert.That(ELDMModOptions.NormalizeLimbSlowPreset("high"), Is.EqualTo(ELDMModOptions.PresetLimbSlowHigh));
            Assert.That(ELDMModOptions.NormalizeLastStandPreset("off"), Is.EqualTo("OffLegacy"));
            Assert.That(ELDMModOptions.NormalizeLastStandSlowPreset("heavy"), Is.EqualTo(ELDMModOptions.PresetLastStandSlowHeavy));
        }

        [Test]
        public void PresetSelectionHash_ChangesWhenPresetSelectionChanges()
        {
            string previous = ELDMModOptions.PresetDamageModel;
            try
            {
                int before = ELDMModOptions.GetPresetSelectionHash();
                ELDMModOptions.PresetDamageModel = ELDMModOptions.PresetDamageSevere;
                int after = ELDMModOptions.GetPresetSelectionHash();
                Assert.That(after, Is.Not.EqualTo(before));
            }
            finally
            {
                ELDMModOptions.PresetDamageModel = previous;
            }
        }

        [Test]
        public void DeadRecoveryChance_HalvesPerSuccessfulRecovery()
        {
            ELDMModOptions.DeadRevivalChancePercent = 100f;
            Assert.That(ELDMModOptions.GetEffectiveDeadRecoveryChanceRatio(0), Is.EqualTo(1f).Within(0.0001f));
            Assert.That(ELDMModOptions.GetEffectiveDeadRecoveryChanceRatio(1), Is.EqualTo(0.5f).Within(0.0001f));
            Assert.That(ELDMModOptions.GetEffectiveDeadRecoveryChanceRatio(2), Is.EqualTo(0.25f).Within(0.0001f));
        }
    }
}
