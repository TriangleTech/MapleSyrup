// reWZ is copyright angelsl, 2011 to 2015 inclusive.
// 
// This file (WZAudioProperty.cs) is part of reWZ.
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
using System.Diagnostics;

namespace reWZ.WZProperties {
    /// <summary>A sound property.</summary>
    public sealed class WZAudioProperty : WZDelayedProperty<byte[]>, IDisposable {
        private static readonly Guid WaveFormatExGuid = new Guid("05589f81-c356-11ce-bf01-00aa0055595a");
        private static readonly Guid NoHeaderGuid = new Guid("00000000-0000-0000-0000-000000000000");
        private byte[] _header;

        internal WZAudioProperty(string name, WZObject parent, WZBinaryReader reader, WZImage container)
            : base(name, parent, container, reader, false, WZObjectType.Audio) {}

        /// <summary>The WAVEFORMATEX header, if present.</summary>
        public byte[] Header {
            get {
                if (!_parsed) {
                    CheckParsed();
                }
                return _header;
            }
        }

        public int Duration { get; private set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() {
            _value = null;
            _header = null;
            _parsed = false;
        }

        internal override bool Parse(WZBinaryReader r, bool initial, out byte[] result) {
            r.Skip(1);
            int blockLen = r.ReadWZInt(); // sound data length
            Duration = r.ReadWZInt(); // sound duration
            r.Skip(1 + 16 + 16 + 2); // Byte, Major type GUID, Sub type GUID, byte, byte

            Guid fmt = new Guid(r.ReadBytes(16));
            if (fmt == WaveFormatExGuid) {
                if (initial) {
                    r.Skip(r.ReadWZInt());
                } else {
                    _header = r.ReadBytes(r.ReadWZInt());
                    if (_header.Length != 18 + GetCbSize(_header)) {
                        _header = null; // TODO FIXME figure out what those gibberish headers are
                    }
                    // But in any case they don't affect our uses

                    //    File._aes.DecryptBytesAsciiKey(_header);
                    //if (_header.Length != 18 + GetCbSize(_header))
                    //    Debug.WriteLine("Failed to parse WAVEFORMATEX header at node {0}", Path);
                    //throw new WZException($"Failed to parse WAVEFORMATEX header at node {Path}");
                }
            } else if (fmt != NoHeaderGuid) {
                Debug.WriteLine("New format guid {0} @ {1}", fmt, Path);
            }

            if (!initial || (File._flag & WZReadSelection.EagerParseAudio) == WZReadSelection.EagerParseAudio) {
                result = r.ReadBytes(blockLen);
                return true;
            } else {
                r.Skip(blockLen);
                result = null;
                return false;
            }
        }

        private static int GetCbSize(byte[] header) {
            return header[16] | (header[17] << 8);
        }
    }
}
