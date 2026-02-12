using System;
using System.IO;
using System.Reflection;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Audio;

namespace EnemyLimbDamageManager.Core
{
    internal sealed class ELDMSoundManager
    {
        private const float DefaultVolume = 2.80f;
        private const float SpatialBlend3D = 1.0f;
        private const float MaxDistance = 55f;
        private const string ManagedAudioObjectPrefix = "UIO_Audio";

        [Serializable]
        private class SoundCatalog
        {
            public string[] bluntLight = Array.Empty<string>();
            public string[] bluntHeavy = Array.Empty<string>();
            public string[] slashSword = Array.Empty<string>();
            public string[] slashAxe = Array.Empty<string>();
            public string[] pierce = Array.Empty<string>();
        }

        private static readonly string[] BluntLightFiles =
        {
            "BoneBreakHeavy1.wav",
            "BoneBreakHeavy2.wav",
            "BoneBreakHeavy3.wav",
            "BoneBreakHeavy4.wav",
            "BoneBreakHeavy5.wav",
        };

        private static readonly string[] BluntHeavyFiles =
        {
            "BoneBreakHeavy1.wav",
            "BoneBreakHeavy2.wav",
            "BoneBreakHeavy3.wav",
            "BoneBreakHeavy4.wav",
            "BoneBreakHeavy5.wav",
        };

        private static readonly string[] SlashSwordFiles =
        {
            "SwordSlice1.wav",
            "SwordSlice2.wav",
        };

        private static readonly string[] SlashAxeFiles =
        {
            "AxeSlice1.wav",
        };

        private static readonly string[] PierceFiles =
        {
            "Pierce.wav",
        };

        private static readonly string[] AxeKeywords =
        {
            "AXE",
            "HATCHET",
            "CLEAVER",
            "TOMAHAWK",
            "BARDICHE",
            "HALBERD",
            "POLEAXE",
            "BATTLEAXE",
        };

        public static ELDMSoundManager Instance { get; } = new ELDMSoundManager();

        private AudioClip[] bluntLightClips;
        private AudioClip[] bluntHeavyClips;
        private AudioClip[] slashSwordClips;
        private AudioClip[] slashAxeClips;
        private AudioClip[] pierceClips;
        private bool initialized;

        private ELDMSoundManager()
        {
        }

        public void Initialize()
        {
            string modRoot = TryGetModRoot();
            SoundCatalog soundCatalog = LoadSoundCatalog(modRoot);
            string soundsDir = ResolveSoundsDirectory(modRoot);
            if (string.IsNullOrEmpty(soundsDir))
            {
                ELDMLog.Warn("sound_init_fail reason=sounds_dir_not_found");
                bluntLightClips = Array.Empty<AudioClip>();
                bluntHeavyClips = Array.Empty<AudioClip>();
                slashSwordClips = Array.Empty<AudioClip>();
                slashAxeClips = Array.Empty<AudioClip>();
                pierceClips = Array.Empty<AudioClip>();
                initialized = true;
                return;
            }

            ELDMLog.Info("sound_init loading from " + soundsDir);

            string[] bluntLightFiles = ResolveFileList(soundCatalog?.bluntLight, BluntLightFiles);
            string[] bluntHeavyFiles = ResolveFileList(soundCatalog?.bluntHeavy, BluntHeavyFiles);
            string[] slashSwordFiles = ResolveFileList(soundCatalog?.slashSword, SlashSwordFiles);
            string[] slashAxeFiles = ResolveFileList(soundCatalog?.slashAxe, SlashAxeFiles);
            string[] pierceFiles = ResolveFileList(soundCatalog?.pierce, PierceFiles);

            bluntLightClips = LoadClipArray(soundsDir, bluntLightFiles, "blunt_light");
            bluntHeavyClips = LoadClipArray(soundsDir, bluntHeavyFiles, "blunt_heavy");
            slashSwordClips = LoadClipArray(soundsDir, slashSwordFiles, "slash_sword");
            slashAxeClips = LoadClipArray(soundsDir, slashAxeFiles, "slash_axe");
            pierceClips = LoadClipArray(soundsDir, pierceFiles, "pierce");

            initialized = true;
            ELDMLog.Info(
                "sound_init_complete" +
                " bluntLight=" + bluntLightClips.Length +
                " bluntHeavy=" + bluntHeavyClips.Length +
                " slashSword=" + slashSwordClips.Length +
                " slashAxe=" + slashAxeClips.Length +
                " pierce=" + pierceClips.Length);
        }

        public void Shutdown()
        {
            bluntLightClips = null;
            bluntHeavyClips = null;
            slashSwordClips = null;
            slashAxeClips = null;
            pierceClips = null;
            initialized = false;
        }

        public float PlayDisableSound(Vector3 position, DamageType damageType, int tier, CollisionInstance collisionInstance)
        {
            if (!initialized)
            {
                return 0f;
            }

            AudioClip primaryClip = SelectClip(damageType, tier, collisionInstance);
            AudioClip crackClip = SelectBluntClip(tier >= 2 ? 2 : 1);
            float maxDuration = 0f;

            if (primaryClip == null && damageType != DamageType.Blunt)
            {
                primaryClip = crackClip;
            }

            if (primaryClip == null)
            {
                ELDMLog.Warn("sfx_disable_skip reason=no_clip damageType=" + damageType + " tier=" + tier);
                return 0f;
            }

            float primaryVolume = damageType == DamageType.Blunt
                ? (tier >= 2 ? 4.20f : 3.90f)
                : 2.60f;
            maxDuration = Mathf.Max(maxDuration, PlayClipAtPosition(primaryClip, position, primaryVolume));

            bool shouldLayerBoneCrack = damageType != DamageType.Blunt || tier >= 2;
            if (shouldLayerBoneCrack && crackClip != null && crackClip != primaryClip)
            {
                float crackPitch = UnityEngine.Random.Range(0.92f, 1.08f);
                float crackVolume = damageType == DamageType.Blunt ? 7.20f : 7.50f;
                maxDuration = Mathf.Max(maxDuration, PlayClipAtPosition(crackClip, position, crackVolume, crackPitch));
            }

            ELDMLog.Info(
                "sfx_disable_play clip=" + primaryClip.name +
                " damageType=" + damageType +
                " tier=" + tier +
                " pos=(" + position.x.ToString("0.0") + "," + position.y.ToString("0.0") + "," + position.z.ToString("0.0") + ")");
            return maxDuration;
        }

        private AudioClip SelectClip(DamageType damageType, int tier, CollisionInstance collisionInstance)
        {
            switch (damageType)
            {
                case DamageType.Blunt:
                    return SelectBluntClip(tier);

                case DamageType.Slash:
                    return SelectSlashClip(collisionInstance);

                case DamageType.Pierce:
                    return PickRandom(pierceClips);

                default:
                    return SelectBluntClip(tier);
            }
        }

        private AudioClip SelectBluntClip(int tier)
        {
            AudioClip heavyClip = PickRandom(bluntHeavyClips);
            if (heavyClip != null) return heavyClip;
            return PickRandom(bluntLightClips);
        }

        private AudioClip SelectSlashClip(CollisionInstance collisionInstance)
        {
            if (IsAxeWeapon(collisionInstance))
            {
                AudioClip axeClip = PickRandom(slashAxeClips);
                if (axeClip != null) return axeClip;
            }

            return PickRandom(slashSwordClips);
        }

        private static bool IsAxeWeapon(CollisionInstance collisionInstance)
        {
            if (collisionInstance == null)
            {
                return false;
            }

            string itemToken = ExtractItemToken(collisionInstance);
            if (string.IsNullOrEmpty(itemToken))
            {
                return false;
            }

            for (int i = 0; i < AxeKeywords.Length; i++)
            {
                if (itemToken.IndexOf(AxeKeywords[i], StringComparison.Ordinal) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static string ExtractItemToken(CollisionInstance collisionInstance)
        {
            Damager damager = collisionInstance?.damageStruct.damager;
            if (damager == null)
            {
                return null;
            }

            Item item = null;
            try
            {
                if (damager.transform != null)
                {
                    item = damager.transform.GetComponentInParent<Item>();
                }
            }
            catch
            {
            }

            if (item == null)
            {
                return null;
            }

            string token = string.Empty;

            if (item.data != null && !string.IsNullOrWhiteSpace(item.data.id))
            {
                token = item.data.id;
            }

            if (!string.IsNullOrWhiteSpace(item.name))
            {
                token += "|" + item.name;
            }

            return token.Length > 0 ? token.ToUpperInvariant() : null;
        }

        private static AudioClip PickRandom(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
            {
                return null;
            }

            if (clips.Length == 1)
            {
                return clips[0];
            }

            return clips[UnityEngine.Random.Range(0, clips.Length)];
        }

        private static float PlayClipAtPosition(
            AudioClip clip,
            Vector3 position,
            float volumeMultiplier = 1f,
            float pitch = 1f)
        {
            if (clip == null)
            {
                return 0f;
            }

            GameObject tempGo = new GameObject(ManagedAudioObjectPrefix);
            tempGo.transform.position = position;

            AudioSource source = tempGo.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = DefaultVolume * Mathf.Clamp(volumeMultiplier, 0.10f, 5f);
            source.pitch = Mathf.Clamp(pitch, 0.35f, 2f);
            source.spatialBlend = SpatialBlend3D;
            source.minDistance = 1f;
            source.maxDistance = MaxDistance * 1.15f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.dopplerLevel = 0f;
            source.playOnAwake = false;
            source.loop = false;

            try
            {
                AudioMixerGroup mixerGroup = ThunderRoadSettings.GetAudioMixerGroup(AudioMixerName.Effect);
                if (mixerGroup != null)
                {
                    source.outputAudioMixerGroup = mixerGroup;
                }
            }
            catch
            {
            }

            source.Play();
            float duration = Mathf.Max(0.01f, clip.length / Mathf.Max(0.01f, Mathf.Abs(source.pitch)));
            UnityEngine.Object.Destroy(tempGo, duration + 0.5f);
            return duration;
        }

        private static SoundCatalog LoadSoundCatalog(string modRoot)
        {
            if (string.IsNullOrWhiteSpace(modRoot))
            {
                ELDMLog.Warn("sound_catalog_skip reason=no_mod_root");
                return null;
            }

            string catalogPath = Path.Combine(modRoot, "Configuration", "SoundCatalog.json");
            if (!File.Exists(catalogPath))
            {
                ELDMLog.Warn("sound_catalog_skip reason=file_missing path=" + catalogPath);
                return null;
            }

            try
            {
                string json = File.ReadAllText(catalogPath);
                SoundCatalog catalog = UnityEngine.JsonUtility.FromJson<SoundCatalog>(json);
                if (catalog == null)
                {
                    ELDMLog.Warn("sound_catalog_parse_fail reason=deserialize_null path=" + catalogPath);
                    return null;
                }

                ELDMLog.Info("sound_catalog_loaded path=" + catalogPath);
                return catalog;
            }
            catch (Exception ex)
            {
                ELDMLog.Warn("sound_catalog_parse_fail reason=error path=" + catalogPath + " error=" + ex.Message);
                return null;
            }
        }

        private static string TryGetModRoot()
        {
            try
            {
                if (ModManager.TryGetModData(Assembly.GetExecutingAssembly(), out ModManager.ModData modData))
                {
                    if (!string.IsNullOrWhiteSpace(modData?.fullPath))
                    {
                        return modData.fullPath;
                    }

                    if (!string.IsNullOrWhiteSpace(modData?.folderName))
                    {
                        return FileManager.GetFullPath(FileManager.Type.JSONCatalog, FileManager.Source.Mods, modData.folderName);
                    }
                }
            }
            catch (Exception ex)
            {
                ELDMLog.Warn("sound_modroot_fail reason=error error=" + ex.Message);
            }

            return null;
        }

        private static string[] ResolveFileList(string[] catalogFiles, string[] fallback)
        {
            string[] normalized = NormalizeFileList(catalogFiles);
            if (normalized.Length > 0)
            {
                return normalized;
            }

            return fallback ?? Array.Empty<string>();
        }

        private static string[] NormalizeFileList(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return Array.Empty<string>();
            }

            string[] buffer = new string[files.Length];
            int count = 0;

            for (int i = 0; i < files.Length; i++)
            {
                string entry = files[i];
                if (string.IsNullOrWhiteSpace(entry))
                {
                    continue;
                }

                string trimmed = entry.Trim();
                if (trimmed.Length == 0)
                {
                    continue;
                }

                buffer[count++] = trimmed;
            }

            if (count == 0)
            {
                return Array.Empty<string>();
            }

            if (count == buffer.Length)
            {
                return buffer;
            }

            string[] compact = new string[count];
            Array.Copy(buffer, compact, count);
            return compact;
        }

        private static AudioClip[] LoadClipArray(string soundsDir, string[] fileNames, string category)
        {
            AudioClip[] loaded = new AudioClip[fileNames.Length];
            int validCount = 0;

            for (int i = 0; i < fileNames.Length; i++)
            {
                string filePath = Path.Combine(soundsDir, fileNames[i]);
                AudioClip clip = WavLoader.LoadFromFile(filePath, Path.GetFileNameWithoutExtension(fileNames[i]));
                if (clip != null)
                {
                    loaded[validCount++] = clip;
                }
                else
                {
                    ELDMLog.Warn("sound_load_fail category=" + category + " file=" + fileNames[i]);
                }
            }

            if (validCount == fileNames.Length)
            {
                return loaded;
            }

            if (validCount == 0)
            {
                ELDMLog.Warn("sound_load_fail category=" + category + " reason=no_valid_clips");
                return Array.Empty<AudioClip>();
            }

            AudioClip[] compacted = new AudioClip[validCount];
            Array.Copy(loaded, compacted, validCount);
            return compacted;
        }

        private static string ResolveSoundsDirectory(string modRoot)
        {
            try
            {
                string root = modRoot;
                if (string.IsNullOrWhiteSpace(root))
                {
                    root = TryGetModRoot();
                }

                if (!string.IsNullOrWhiteSpace(root))
                {
                    string modSounds = Path.Combine(root, "Sounds");
                    if (Directory.Exists(modSounds))
                    {
                        return modSounds;
                    }
                }

                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                if (string.IsNullOrEmpty(assemblyLocation))
                {
                    ELDMLog.Warn("sound_resolve reason=no_assembly_location");
                    return null;
                }

                string assemblyDir = Path.GetDirectoryName(assemblyLocation);
                if (string.IsNullOrEmpty(assemblyDir))
                {
                    return null;
                }

                string soundsDir = Path.Combine(assemblyDir, "Sounds");
                if (Directory.Exists(soundsDir))
                {
                    return soundsDir;
                }

                string parentDir = Path.GetDirectoryName(assemblyDir);
                if (!string.IsNullOrEmpty(parentDir))
                {
                    soundsDir = Path.Combine(parentDir, "Sounds");
                    if (Directory.Exists(soundsDir))
                    {
                        return soundsDir;
                    }
                }

                ELDMLog.Warn("sound_resolve reason=no_sounds_dir tried=" + Path.Combine(assemblyDir, "Sounds"));
            }
            catch (Exception ex)
            {
                ELDMLog.Warn("sound_resolve reason=error error=" + ex.Message);
            }

            return null;
        }
    }
}
