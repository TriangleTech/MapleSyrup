// reWZ is copyright angelsl, 2011 to 2015 inclusive.
// 
// This file (WZBinaryReader.cs) is part of reWZ.
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
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using UsefulThings;

namespace reWZ {
    internal sealed class WZBinaryReader : PointerStream {
        private readonly WZAES _aes;

        internal unsafe WZBinaryReader(byte* start, long size, WZAES aes, uint versionHash) : base(start, size) {
            _aes = aes;
            VersionHash = versionHash;
        }

        internal uint VersionHash { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Seek(long pos) {
            Position = pos;
        }

        internal unsafe WZBinaryReader Clone() {
            WZBinaryReader ret = new WZBinaryReader(_start, _end - _start, _aes, VersionHash);
            ret._cur = _cur;
            return ret;
        }

        /// <summary>Advances the position within the backing stream by <paramref name="count" /> .</summary>
        /// <param name="count"> The amount of bytes to skip. </param>
        internal unsafe void Skip(long count) {
            _cur += count;
        }

        /// <summary>Executes a delegate of type <see cref="System.Action" /> , then sets the position of the backing stream back
        ///     to the original value.</summary>
        /// <param name="result"> The delegate to execute. </param>
        internal unsafe void PeekFor(Action result) {
            byte* orig = _cur;
            try {
                result();
            } finally {
                _cur = orig;
            }
        }

        /// <summary>Executes a delegate of type <see cref="System.Func{TResult}" /> , then sets the position of the backing stream
        ///     back to the original value.</summary>
        /// <typeparam name="T"> The return type of the delegate. </typeparam>
        /// <param name="result"> The delegate to execute. </param>
        /// <returns> The object returned by the delegate. </returns>
        internal unsafe T PeekFor<T>(Func<T> result) {
            byte* orig = _cur;
            try {
                return result();
            } finally {
                _cur = orig;
            }
        }

        /// <summary>Reads a string encoded in WZ format.</summary>
        /// <param name="encrypted"> Whether the string is encrypted. </param>
        /// <returns> The read string. </returns>
        internal string ReadWZString(bool encrypted = true) {
            if (Position + 1 > Length) {
                throw new WZException("WZ string offset out of bounds");
            }
            int length = ReadSByte();
            if (length == 0) {
                return "";
            }
            if (length > 0) {
                length = length == 127 ? ReadInt32WithinBounds() : length;
                if (length == 0) {
                    return "";
                }
                if (Position + length*2 > Length) {
                    throw new WZException("Not enough bytes to read WZ string");
                }
                byte[] rbytes = ReadBytes(length*2);
                return _aes.DecryptUnicodeString(rbytes, encrypted);
            } // !(length >= 0), i think we can assume length < 0, but the compiler can't seem to see that
            length = length == -128 ? ReadInt32WithinBounds() : -length;
            if (Position + length > Length) {
                throw new WZException("Not enough bytes to read WZ string");
            }
            return length == 0 ? "" : _aes.DecryptASCIIString(ReadBytes(length), encrypted);
        }

        /// <summary>Reads a string encoded in WZ format at a specific offset, then returns the backing stream's position to its
        ///     original value.</summary>
        /// <param name="offset"> The offset where the string is located. </param>
        /// <param name="encrypted"> Whether the string is encrypted. </param>
        /// <returns> The read string. </returns>
        private string ReadWZStringAtOffset(long offset, bool encrypted = true) {
            return PeekFor(() => {
                Position = offset;
                return ReadWZString(encrypted);
            });
        }

        /// <summary>Reads a raw and unencrypted ASCII string.</summary>
        /// <param name="length"> The length of the string. </param>
        /// <returns> The read string. </returns>
        internal string ReadASCIIString(int length) => Encoding.ASCII.GetString(ReadBytes(length));

        internal string ReadWZStringBlock(bool encrypted) {
            switch (ReadByte()) {
                case 0:
                case 0x73:
                    return ReadWZString(encrypted);
                case 1:
                case 0x1B:
                    return ReadWZStringAtOffset(ReadInt32WithinBounds(), encrypted);
                default:
                    return WZUtil.Die<string>("Unknown string type in string block!");
            }
        }

        internal void SkipWZStringBlock() {
            switch (ReadByte()) {
                case 0:
                case 0x73:
                    SkipWZString();
                    return;
                case 1:
                case 0x1B:
                    Skip(4);
                    return;
                default:
                    WZUtil.Die("Unknown string type in string block!");
                    return;
            }
        }

        internal void SkipWZString() {
            int length = ReadSByte();
            Skip(length >= 0 ? (length == 127 ? ReadInt32() : length)*2 : length == -128 ? ReadInt32() : -length);
        }

        /// <summary>Reads a WZ-compressed 32-bit integer.</summary>
        /// <returns> The read integer. </returns>
        internal int ReadWZInt() {
            sbyte s = ReadSByte();
            return s == -128 ? ReadInt32() : s;
        }

        /// <summary>Reads a WZ-compressed 64-bit integer.</summary>
        /// <returns> The read integer. </returns>
        internal long ReadWZLong() {
            sbyte s = ReadSByte();
            return s == -128 ? ReadInt64() : s;
        }

        private int ReadInt32WithinBounds() {
            if (Position + 4 > Length) {
                throw new WZException("Not enough bytes to read int32");
            }
            return ReadInt32();
        }

        internal uint ReadWZOffset(uint fstart) {
            unchecked {
                uint ret = (((uint) Position - fstart) ^ 0xFFFFFFFF)*VersionHash - WZAES.OffsetKey;
                return (((ret << (int) ret) | (ret >> (int) (32 - ret))) ^ ReadUInt32()) + fstart*2;
            }
        }

        internal Guid[] ReadGuidArray() {
            int length = ReadByte();
            Guid[] ret = new Guid[length];
            for (int i = 0; i < length; ++i) {
                ret[i] = new Guid(ReadBytes(16));
            }
            return ret;
        }

        internal static byte[] Inflate(Stream @in) {
            long length = 512*1024;
            try {
                length = Math.Max(@in.Length, length);
            } catch {}
            byte[] dec = new byte[length];
            using (DeflateStream dStr = new DeflateStream(@in, CompressionMode.Decompress)) {
                using (MemoryStream @out = new MemoryStream(dec.Length*2)) {
                    int len;
                    while ((len = dStr.Read(dec, 0, dec.Length)) > 0) {
                        @out.Write(dec, 0, len);
                    }
                    return @out.ToArray();
                }
            }
        }
    }
}
