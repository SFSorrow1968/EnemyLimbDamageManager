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
            ELDMModOptions.PresetDamageModel = ELDMModOptions.PresetDamageDefaultPlus;
            ELDMModOptions.PresetRecoveryModel = ELDMModOptions.PresetRecoveryDefault;
            ELDMModOptions.PresetSquirmModel = ELDMModOptions.PresetSquirmTight;
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
            Assert.That(ELDMModOptions.NormalizeDamagePreset("Extreme"), Is.EqualTo(ELDMModOptions.PresetDamageSevered));
            Assert.That(ELDMModOptions.NormalizeRecoveryPreset("off"), Is.EqualTo(ELDMModOptions.PresetRecoveryDisabled));
            Assert.That(ELDMModOptions.NormalizeSquirmPreset("wild"), Is.EqualTo(ELDMModOptions.PresetSquirmWild));
        }

        [Test]
        public void PresetSelectionHash_ChangesWhenPresetSelectionChanges()
        {
            string previous = ELDMModOptions.PresetDamageModel;
            try
            {
                int before = ELDMModOptions.GetPresetSelectionHash();
                ELDMModOptions.PresetDamageModel = ELDMModOptions.PresetDamageTactical;
                int after = ELDMModOptions.GetPresetSelectionHash();
                Assert.That(after, Is.Not.EqualTo(before));
            }
            finally
            {
                ELDMModOptions.PresetDamageModel = previous;
            }
        }
    }
}
