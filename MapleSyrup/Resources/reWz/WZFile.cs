// reWZ is copyright angelsl, 2011 to 2015 inclusive.
// 
// This file (WZFile.cs) is part of reWZ.
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
using System.Collections.Generic;
using System.Globalization;
using reWZ.WZProperties;
using UsefulThings;

namespace reWZ {
    /// <summary>A WZ file.</summary>
    public sealed class WZFile : IDisposable {
        internal readonly WZAES _aes;
        private readonly IBytePointerObject _bpo;
        internal readonly bool _encrypted;
        internal readonly WZReadSelection _flag;
        private readonly WZBinaryReader _r;
        internal readonly WZVariant _variant;
        internal uint _fstart;

        /// <summary>Creates and loads a WZ file from a path. The Stream created will be disposed when the WZ file is disposed.</summary>
        /// <param name="path"> The path where the WZ file is located. </param>
        /// <param name="variant"> The variant of this WZ file. </param>
        /// <param name="encrypted"> Whether the WZ file is encrypted outside a WZ image. </param>
        /// <param name="flag"> WZ parsing flags. </param>
        public unsafe WZFile(string path, WZVariant variant, bool encrypted, WZReadSelection flag = WZReadSelection.None) {
            _variant = variant;
            _encrypted = encrypted;
            _flag = flag;
            _aes = new WZAES(_variant);
            _bpo = new MemoryMappedFile(path);
            _r = new WZBinaryReader(_bpo.Pointer, _bpo.Length, _aes, 0);
            Parse();
        }

        /// <summary>The root directory of the WZ file.</summary>
        public WZDirectory MainDirectory { get; private set; }

        /// <summary>Disposer.</summary>
        ~WZFile() {
            Dispose(false);
        }

        /// <summary>Resolves a path in the form "/a/b/c/.././d/e/f/".</summary>
        /// <param name="path"> The path to resolve. </param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The path has an invalid node.</exception>
        public WZObject ResolvePath(string path) {
            return WZUtil.ResolvePath(MainDirectory, path);
        }

        private void Parse() {
            _r.Seek(0);
            if (_r.ReadInt32() != 0x31474B50) {
                WZUtil.Die("WZ file has invalid header; file does not have magic \"PKG1\".");
            }
            _r.Skip(8);
            _fstart = _r.ReadUInt32();
            GuessVersion();
            MainDirectory = new WZDirectory("", null, this, _r, _fstart + 2);
        }

        private void GuessVersion() {
            _r.Seek(_fstart);
            short ver = _r.ReadInt16();
            bool success;
            long offset = TryFindImageInDir(out success);
            if (success && GuessVersionWithImageOffsetAt(ver, offset)) {
                return;
            }

            for (ushort v = 0; v < ushort.MaxValue; v++) {
                uint vHash;
                if (!VersionHash(v, ver, out vHash)) {
                    continue;
                }
                _r.VersionHash = vHash;
                if (DepthFirstImageSearch(out offset)) {
                    break;
                }
            }

            if (!GuessVersionWithImageOffsetAt(ver, offset)) {
                WZUtil.Die("Unable to guess WZ version.");
            }
        }

        private bool DepthFirstImageSearch(out long offset) {
            bool success = false;
            offset = -1;
            int count = _r.ReadWZInt();
            for (int i = 0; i < count; i++) {
                byte type = _r.ReadByte();
                switch (type) {
                    case 1:
                        _r.Skip(10);
                        continue;
                    case 2:
                        int x = _r.ReadInt32();
                        type = _r.PeekFor(() => {
                            _r.Seek(x + _fstart);
                            return _r.ReadByte();
                        });
                        break;
                    case 3:
                    case 4:
                        _r.SkipWZString();
                        break;
                    default:
                        WZUtil.Die("Unknown object type in WzDirectory.");
                        break;
                }

                _r.ReadWZInt();
                _r.ReadWZInt();
                offset = _r.Position;
                if (type == 4) {
                    success = true;
                    break;
                }

                if (type == 3) {
                    try {
                        offset = _r.PeekFor(() => {
                            _r.Seek(_r.ReadWZOffset(_fstart));
                            long o;
                            success = DepthFirstImageSearch(out o);
                            return o;
                        });
                        break;
                    } catch {}
                }
                _r.Skip(4);
            }
            return success;
        }

        private long TryFindImageInDir(out bool success) {
            int count = _r.ReadWZInt();
            if (count == 0) {
                WZUtil.Die("WZ file has no entries!");
            }
            long offset = 0;
            offset = TryFindImageOffset(count, offset, out success);
            return offset;
        }

        private long TryFindImageOffset(int count, long offset, out bool success) {
            success = false;
            for (int i = 0; i < count; i++) {
                byte type = _r.ReadByte();
                switch (type) {
                    case 1:
                        _r.Skip(10);
                        continue;
                    case 2:
                        int x = _r.ReadInt32();
                        type = _r.PeekFor(() => {
                            _r.Seek(x + _fstart);
                            return _r.ReadByte();
                        });
                        break;
                    case 3:
                    case 4:
                        _r.SkipWZString();
                        break;
                    default:
                        WZUtil.Die("Unknown object type in WzDirectory.");
                        break;
                }

                _r.ReadWZInt();
                _r.ReadWZInt();
                offset = _r.Position;
                _r.Skip(4);
                if (type != 4) {
                    continue;
                }

                success = true;
                break;
            }
            return offset;
        }

        private bool GuessVersionWithImageOffsetAt(short ver, long offset) {
            bool success = false;
            for (ushort v = 0; v < ushort.MaxValue; v++) {
                uint vHash;
                if (!VersionHash(v, ver, out vHash)) {
                    continue;
                }
                _r.Seek(offset);
                _r.VersionHash = vHash;
                try {
                    uint off = _r.ReadWZOffset(_fstart);
                    if (off + 1 > _r.Length) {
                        continue;
                    }
                    _r.Seek(off);
                    if (_r.PeekFor(() => _r.ReadWZStringBlock(true)) != "Property" &&
                        _r.PeekFor(() => _r.ReadWZStringBlock(false)) != "Property") {
                        continue;
                    }
                    success = true;
                    break;
                } catch {
                    success = false;
                }
            }
            return success;
        }

        internal unsafe WZBinaryReader GetSubstream(long offset, long length) {
            return new WZBinaryReader(_bpo.Pointer + offset, length, _aes, _r.VersionHash);
        }

        internal static bool VersionHash(ushort v, short sV, out uint vHash) {
            vHash = 0;
            foreach (char c in v.ToString(CultureInfo.InvariantCulture)) {
                vHash = 32*vHash + c + 1;
            }
            return (0xFF
                    ^ ((vHash >> 24) & 0xFF)
                    ^ ((vHash >> 16) & 0xFF)
                    ^ ((vHash >> 8) & 0xFF)
                    ^ (vHash & 0xFF)) == sV;
        }

        #region IDisposable Members

        /// <summary>Disposes this WZ file.</summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                _bpo.Dispose();
            }
            MainDirectory = null;
        }

        #endregion
    }

    /// <summary>WZ reading flags.</summary>
    [Flags]
    public enum WZReadSelection : byte {
        /// <summary>No flags are enabled, that is, lazy loading of properties and WZ images is enabled.</summary>
        None = 0,

        /// <summary>Set this flag to disable lazy loading of string properties.</summary>
        EagerParseStrings = 1,

        /// <summary>Set this flag to disable lazy loading of Audio properties.</summary>
        EagerParseAudio = 2,

        /// <summary>Set this flag to disable lazy loading of canvas properties.</summary>
        EagerParseCanvas = 4,

        /// <summary>Set this flag to completely disable loading of canvas properties.</summary>
        NeverParseCanvas = 8,

        /// <summary>Set this flag to disable lazy loading of string, Audio and canvas properties.</summary>
        EagerParseAll = EagerParseCanvas | EagerParseAudio | EagerParseStrings,

        /// <summary>Set this flag to disable lazy loading of WZ images.</summary>
        EagerParseImage = 16
    }

    internal static class WZUtil {
        internal static T Die<T>(string cause) {
            throw new WZException(cause);
        }

        internal static void Die(string cause) {
            throw new WZException(cause);
        }

        internal static WZObject ResolvePath(WZObject start, string path) {
            try {
                WZObject result = start;
                string[] nodes = (path.StartsWith("/") ? path.Substring(1) : path).Split('/');
                foreach (string node in nodes) {
                    switch (node) {
                        case ".":
                            continue;
                        case "..":
                            result = result.Parent;
                            continue;
                    }
                    WZUOLProperty uolNode = result as WZUOLProperty;
                    if (uolNode != null) {
                        result = uolNode.FinalTarget;
                    }
                    result = result[node];
                }
                return result;
            } catch (KeyNotFoundException) {
                return null;
            }
        }
    }
}
