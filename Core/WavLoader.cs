using System;
using System.IO;
using UnityEngine;

namespace EnemyLimbDamageManager.Core
{
    /// <summary>
    /// Minimal WAV file parser that converts raw WAV bytes into a Unity AudioClip.
    /// Supports standard PCM (8-bit, 16-bit, 24-bit, 32-bit) uncompressed WAV files.
    /// </summary>
    internal static class WavLoader
    {
        public static AudioClip LoadFromFile(string filePath, string clipName)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                ELDMLog.Warn("wav_load_fail reason=file_not_found path=" + (filePath ?? "null"));
                return null;
            }

            byte[] data;
            try
            {
                data = File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                ELDMLog.Warn("wav_load_fail reason=read_error path=" + filePath + " error=" + ex.Message);
                return null;
            }

            return ParseWav(data, clipName);
        }

        private static AudioClip ParseWav(byte[] data, string clipName)
        {
            if (data == null || data.Length < 44)
            {
                ELDMLog.Warn("wav_parse_fail reason=too_small clip=" + clipName);
                return null;
            }

            // Validate RIFF header
            if (data[0] != 'R' || data[1] != 'I' || data[2] != 'F' || data[3] != 'F')
            {
                ELDMLog.Warn("wav_parse_fail reason=not_riff clip=" + clipName);
                return null;
            }

            // Validate WAVE format
            if (data[8] != 'W' || data[9] != 'A' || data[10] != 'V' || data[11] != 'E')
            {
                ELDMLog.Warn("wav_parse_fail reason=not_wave clip=" + clipName);
                return null;
            }

            // Find fmt chunk
            int fmtOffset = FindChunk(data, "fmt ", 12);
            if (fmtOffset < 0)
            {
                ELDMLog.Warn("wav_parse_fail reason=no_fmt_chunk clip=" + clipName);
                return null;
            }

            int fmtDataStart = fmtOffset + 8;
            ushort audioFormat = ReadUInt16(data, fmtDataStart);
            if (audioFormat != 1) // PCM only
            {
                ELDMLog.Warn("wav_parse_fail reason=not_pcm format=" + audioFormat + " clip=" + clipName);
                return null;
            }

            ushort channels = ReadUInt16(data, fmtDataStart + 2);
            int sampleRate = ReadInt32(data, fmtDataStart + 4);
            ushort bitsPerSample = ReadUInt16(data, fmtDataStart + 14);

            if (channels < 1 || channels > 8 || sampleRate < 8000 || sampleRate > 192000)
            {
                ELDMLog.Warn("wav_parse_fail reason=invalid_format channels=" + channels + " sampleRate=" + sampleRate + " clip=" + clipName);
                return null;
            }

            // Find data chunk
            int dataOffset = FindChunk(data, "data", 12);
            if (dataOffset < 0)
            {
                ELDMLog.Warn("wav_parse_fail reason=no_data_chunk clip=" + clipName);
                return null;
            }

            int dataSize = ReadInt32(data, dataOffset + 4);
            int dataStart = dataOffset + 8;

            if (dataStart + dataSize > data.Length)
            {
                dataSize = data.Length - dataStart;
            }

            if (dataSize <= 0)
            {
                ELDMLog.Warn("wav_parse_fail reason=empty_data clip=" + clipName);
                return null;
            }

            int bytesPerSample = bitsPerSample / 8;
            if (bytesPerSample < 1 || bytesPerSample > 4)
            {
                ELDMLog.Warn("wav_parse_fail reason=unsupported_bit_depth bits=" + bitsPerSample + " clip=" + clipName);
                return null;
            }

            int totalSamples = dataSize / bytesPerSample;
            int samplesPerChannel = totalSamples / channels;

            if (samplesPerChannel <= 0)
            {
                ELDMLog.Warn("wav_parse_fail reason=no_samples clip=" + clipName);
                return null;
            }

            float[] floatData = ConvertToFloat(data, dataStart, totalSamples, bytesPerSample);
            if (floatData == null)
            {
                ELDMLog.Warn("wav_parse_fail reason=conversion_failed clip=" + clipName);
                return null;
            }

            AudioClip clip = AudioClip.Create(clipName, samplesPerChannel, channels, sampleRate, false);
            if (!clip.SetData(floatData, 0))
            {
                ELDMLog.Warn("wav_parse_fail reason=setdata_failed clip=" + clipName);
                return null;
            }

            return clip;
        }

        private static float[] ConvertToFloat(byte[] data, int offset, int totalSamples, int bytesPerSample)
        {
            float[] result = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                int sampleOffset = offset + (i * bytesPerSample);
                if (sampleOffset + bytesPerSample > data.Length)
                {
                    break;
                }

                switch (bytesPerSample)
                {
                    case 1: // 8-bit unsigned
                        result[i] = (data[sampleOffset] - 128) / 128f;
                        break;
                    case 2: // 16-bit signed
                        short s16 = (short)(data[sampleOffset] | (data[sampleOffset + 1] << 8));
                        result[i] = s16 / 32768f;
                        break;
                    case 3: // 24-bit signed
                        int s24 = data[sampleOffset] | (data[sampleOffset + 1] << 8) | (data[sampleOffset + 2] << 16);
                        if ((s24 & 0x800000) != 0) s24 |= unchecked((int)0xFF000000);
                        result[i] = s24 / 8388608f;
                        break;
                    case 4: // 32-bit signed
                        int s32 = data[sampleOffset] | (data[sampleOffset + 1] << 8) | (data[sampleOffset + 2] << 16) | (data[sampleOffset + 3] << 24);
                        result[i] = s32 / 2147483648f;
                        break;
                }
            }

            return result;
        }

        private static int FindChunk(byte[] data, string chunkId, int startOffset)
        {
            if (chunkId.Length != 4) return -1;

            byte c0 = (byte)chunkId[0];
            byte c1 = (byte)chunkId[1];
            byte c2 = (byte)chunkId[2];
            byte c3 = (byte)chunkId[3];

            int pos = startOffset;
            while (pos + 8 <= data.Length)
            {
                if (data[pos] == c0 && data[pos + 1] == c1 && data[pos + 2] == c2 && data[pos + 3] == c3)
                {
                    return pos;
                }

                int chunkSize = ReadInt32(data, pos + 4);
                if (chunkSize < 0) break;
                pos += 8 + chunkSize;

                // Chunks are word-aligned
                if (pos % 2 != 0) pos++;
            }

            return -1;
        }

        private static ushort ReadUInt16(byte[] data, int offset)
        {
            return (ushort)(data[offset] | (data[offset + 1] << 8));
        }

        private static int ReadInt32(byte[] data, int offset)
        {
            return data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24);
        }
    }
}
