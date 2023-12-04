// reWZ is copyright angelsl, 2011 to 2015 inclusive.
//
// This file (WZAES.cs) is part of reWZ.
//
// reWZ is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// reWZ is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with reWZ. If not, see <http://www.gnu.org/licenses/>.
//
// Linking reWZ statically or dynamically with other modules
// is making a combined work based on reWZ. Thus, the terms and
// conditions of the GNU General Public License cover the whole combination.
//
// As a special exception, the copyright holders of reWZ give you
// permission to link reWZ with independent modules to produce an
// executable, regardless of the license terms of these independent modules,
// and to copy and distribute the resulting executable under terms of your
// choice, provided that you also meet, for each linked independent module,
// the terms and conditions of the license of that module. An independent
// module is a module which is not derived from or based on reWZ.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace reWZ {
    internal sealed class WZAES {
        private byte[] _asciiEncKey;
        private byte[] _asciiKey;
        private byte[] _unicodeEncKey;
        private byte[] _unicodeKey;
        private byte[] _wzKey;

        private readonly WZVariant _version;

        internal WZAES(WZVariant version) {
            _version = version;
            GenerateKeys(0x10000);
        }

        private void GenerateKeys(int length) {
            _wzKey = GetWZKey(_version, length);
            byte[] asciiKey = new byte[_wzKey.Length];
            byte[] unicodeKey = new byte[_wzKey.Length];
            byte[] asciiEncKey = new byte[_wzKey.Length];
            byte[] unicodeEncKey = new byte[_wzKey.Length];
            unchecked {
                byte mask = 0xAA;
                for (int i = 0; i < _wzKey.Length; ++i, ++mask) {
                    asciiKey[i] = mask;
                    asciiEncKey[i] = (byte) (_wzKey[i] ^ mask);
                }
                ushort umask = 0xAAAA;
                for (int i = 0; i < _wzKey.Length/2; i += 2, ++umask) {
                    unicodeKey[i] = (byte) (umask & 0xFF);
                    unicodeKey[i + 1] = (byte) ((umask & 0xFF00) >> 8);
                    unicodeEncKey[i] = (byte) (_wzKey[i] ^ unicodeKey[i]);
                    unicodeEncKey[i + 1] = (byte) (_wzKey[i + 1] ^ unicodeKey[i + 1]);
                }
            }
            _asciiKey = asciiKey;
            _unicodeKey = unicodeKey;
            _asciiEncKey = asciiEncKey;
            _unicodeEncKey = unicodeEncKey;
        }

        internal string DecryptASCIIString(byte[] asciiBytes, bool encrypted = true) {
            CheckKeyLength(asciiBytes.Length);
            return Encoding.ASCII.GetString(DecryptData(asciiBytes, encrypted ? _asciiEncKey : _asciiKey));
        }

        internal string DecryptUnicodeString(byte[] ushortChars, bool encrypted = true) {
            CheckKeyLength(ushortChars.Length);
            return Encoding.Unicode.GetString(DecryptData(ushortChars, encrypted ? _unicodeEncKey : _unicodeKey));
        }

        internal byte[] DecryptBytes(byte[] bytes) {
            CheckKeyLength(bytes.Length);
            return DecryptData(bytes, _wzKey);
        }

        private void CheckKeyLength(int length) {
            if (_wzKey.Length < length) {
                GenerateKeys(length);
            }
        }

        internal const uint OffsetKey = 0x581C3F6D;

        private static readonly byte[] AESKey = {
            0x13, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,
            0xB4, 0x00, 0x00, 0x00, 0x1B, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0x00, 0x52, 0x00,
            0x00, 0x00
        };

        private static readonly byte[] GMSIV = {
            0x4D, 0x23, 0xC7, 0x2B, 0x4D, 0x23, 0xC7, 0x2B, 0x4D, 0x23, 0xC7, 0x2B,
            0x4D, 0x23, 0xC7, 0x2B
        };

        private static readonly byte[] KMSIV = {
            0xB9, 0x7D, 0x63, 0xE9, 0xB9, 0x7D, 0x63, 0xE9, 0xB9, 0x7D, 0x63, 0xE9,
            0xB9, 0x7D, 0x63, 0xE9
        };

        private static byte[] GetWZKey(WZVariant version, int length) {
            length = (length & ~15) + ((length & 15) > 0 ? 16 : 0);
            switch ((int) version) {
                case 0:
                    return GenerateKey(KMSIV, AESKey, length);
                case 1:
                    return GenerateKey(GMSIV, AESKey, length);
                case 2:
                    return new byte[length];
                default:
                    throw new ArgumentException("Invalid WZ variant passed.", nameof(version));
            }
        }

        private static byte[] GenerateKey(byte[] iv, byte[] aesKey, int length) {
            using (MemoryStream memStream = new MemoryStream(length)) {
                using (AesManaged aem = new AesManaged {KeySize = 256, Key = aesKey, Mode = CipherMode.CBC, IV = iv}) {
                    using (
                        CryptoStream cStream = new CryptoStream(memStream, aem.CreateEncryptor(), CryptoStreamMode.Write)
                        ) {
                        cStream.Write(new byte[length], 0, length);
                        cStream.Flush();
                        return memStream.ToArray();
                    }
                }
            }
        }

        private static unsafe byte[] DecryptData(byte[] data, byte[] key) {
            if (data.Length > key.Length) {
                throw new InvalidOperationException("data.Length > key.Length; not supposed to happen, please report this to reWZ");
            }

            fixed (byte* c = data, k = key) {
                byte* d = c, l = k, e = d + data.Length;
                while (d < e) {
                    *d++ ^= *l++;
                }
            }

            return data;
        }
    }

    /// <summary>This enum is used to specify the WZ key to be used.</summary>
    public enum WZVariant {
        /// <summary>MapleStory SEA</summary>
        MSEA = 0,

        /// <summary>Korea MapleStory</summary>
        KMS = 0,

        /// <summary>Korea MapleStory (Tespia)</summary>
        KMST = 0,

        /// <summary>Japan MapleStory</summary>
        JMS = 0,

        /// <summary>Japan MapleStory (Tespia)</summary>
        JMST = 0,

        /// <summary>Europe MapleStory</summary>
        EMS = 0,

        /// <summary>Global MapleStory</summary>
        GMS = 1,

        /// <summary>Global MapleStory (Tespia)</summary>
        GMST = 1,

        /// <summary>Taiwan MapleStory</summary>
        TMS = 1,

        /// <summary>Brazil MapleStory</summary>
        BMS = 2,

        /// <summary>Classic MapleStory (Data.wz)</summary>
        Classic = 2
    }
}
