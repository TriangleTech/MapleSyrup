// reWZ is copyright angelsl, 2011 to 2015 inclusive.
// 
// This file (WZCanvasProperty.cs) is part of reWZ.
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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rectangle = System.Drawing.Rectangle;

namespace reWZ.WZProperties {
    /// <summary>A bitmap property, containing an image, and children.</summary>
    public sealed class WZCanvasProperty : WZDelayedProperty<Bitmap>, IDisposable {
        private long _afterChildren;

        internal WZCanvasProperty(string name, WZObject parent, WZBinaryReader br, WZImage container)
            : base(name, parent, container, br, true, WZObjectType.Canvas) {}

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            _value?.Dispose();
            _value = null;
            _parsed = false;
        }

        internal override bool Parse(WZBinaryReader br, bool initial, out Bitmap result) {
            bool skip = (File._flag & WZReadSelection.NeverParseCanvas) == WZReadSelection.NeverParseCanvas,
                eager = (File._flag & WZReadSelection.EagerParseCanvas) == WZReadSelection.EagerParseCanvas;
            if (initial) {
                if (skip && eager) {
                    result = null;
                    return WZUtil.Die<bool>("Both NeverParseCanvas and EagerParseCanvas are set.");
                }
                br.Skip(1);
                if (br.ReadByte() == 1) {
                    br.Skip(2);
                    List<WZObject> l = WZExtendedParser.ParsePropertyList(br, this, Image, Image._encrypted);
                    if (ChildCount == 0) {
                        l.ForEach(Add);
                    }
                }
                _afterChildren = br.Position;
            } else {
                br.Position = _afterChildren;
            }
            int width = br.ReadWZInt(); // width
            int height = br.ReadWZInt(); // height
            int format1 = br.ReadWZInt(); // format 1
            int scale = br.ReadByte(); // format 2
            br.Skip(4);
            int blockLen = br.ReadInt32();
            if ((initial || skip) && !eager) {
                br.Skip(blockLen); // block Len & png data
                result = null;
                return skip;
            } else {
                br.Skip(1);
                byte[] header = br.ReadBytes(2);
                // FLG bit 5 indicates the presence of a preset dictionary
                // seems like MS doesn't use that?
                if ((header[1] & 0x20) != 0) {
                    Debug.WriteLine("Warning; zlib with preset dictionary");
                    result = null;
                    br.Skip(blockLen - 3);
                    return true;
                }
                // zlib header: CMF (byte), FLG (byte)
                byte[] pngData = br.ReadBytes(blockLen - 3);
                result = ParsePNG(width, height, format1, scale,
                    // CMF least significant bits 0 to 3 are compression method. only 8 is valid
                    (header[0] & 0xF) != 8 ||
                    // CMF << 8 | FLG i.e. header read as a big-endian short is a multiple of 31
                    (header[0] << 8 | header[1])%31 != 0
                        ? DecryptPNG(pngData)
                        : pngData);
                return true;
            }
        }

        private byte[] DecryptPNG(byte[] @in) {
            using (MemoryStream sIn = new MemoryStream(@in, false)) {
                using (BinaryReader sBr = new BinaryReader(sIn)) {
                    using (MemoryStream sOut = new MemoryStream(@in.Length)) {
                        while (sIn.Position < sIn.Length) {
                            int blockLen = sBr.ReadInt32();
                            sOut.Write(File._aes.DecryptBytes(sBr.ReadBytes(blockLen)), 0, blockLen);
                        }
                        return sOut.ToArray();
                    }
                }
            }
        }

        private static unsafe Bitmap ParsePNG(int width, int height, int format1, int scale, byte[] data) {
            byte[] dec;
            using (MemoryStream @in = new MemoryStream(data, 0, data.Length)) {
                dec = WZBinaryReader.Inflate(@in);
            }
            int decLen = dec.Length;
            width >>= scale;
            height >>= scale;
            switch (format1) {
                case 0x001: {
                    if (decLen != width*height*2) {
                        Debug.WriteLine("Warning; dec.Length != 2wh; ARGB4444");
                    }
                    Bitmap ret = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    BitmapData bd = ret.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb);
                    try {
                        fixed (byte* t = dec) {
                            byte* u = t, s = (byte*) bd.Scan0;
                            for (int i = 0; i < decLen; i++) {
                                *s++ = (byte) ((*u & 0x0F)*0x11);
                                *s++ = (byte) (((*u++ & 0xF0) >> 4)*0x11);
                            }
                        }
                    } finally {
                        ret.UnlockBits(bd);
                    }
                    return ret;
                }
                case 0x002:
                    if (decLen != width*height*4) {
                        Debug.WriteLine("Warning; dec.Length != 4wh; 32BPP");
                    }
                    return BitmapFromBytes(width, height, PixelFormat.Format32bppArgb, dec);
                case 0x201:
                    if (decLen != width*height*2) {
                        Debug.WriteLine("Warning; dec.Length != 2wh; 16BPP");
                    }
                    return BitmapFromBytes(width, height, PixelFormat.Format16bppRgb565, dec);
                case 0x402: {
                    Bitmap ret = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    BitmapData bd = ret.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb);
                    try {
                        fixed (byte* decP = dec) {
                            UnDXT.DecompressImage((byte*) bd.Scan0, width, height, decP, UnDXT.kDxt3);
                        }
                    } finally {
                        ret.UnlockBits(bd);
                    }
                    return ret;
                }
                default:
                    Debug.WriteLine("Unknown bitmap type format1:{0} scale:{1}", format1, scale);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Bitmap BitmapFromBytes(int width, int height, PixelFormat format, byte[] data) {
            Bitmap ret = new Bitmap(width, height, format);
            BitmapData bd = ret.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, format);
            try {
                Marshal.Copy(data, 0, bd.Scan0, Math.Min(data.Length, height*bd.Stride));
            } finally {
                ret.UnlockBits(bd);
            }
            return ret;
        }
    }
}
