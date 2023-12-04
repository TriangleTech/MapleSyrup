// reWZ is copyright angelsl, 2011 to 2015 inclusive.
// 
// This file (UnDXT.cs) is part of reWZ.
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

namespace reWZ {
    // The following code is ported from libsquish <https://code.google.com/p/libsquish/>.
    /*
        Copyright (c) 2006 Simon Brown                          si@sjbrown.co.uk

        Permission is hereby granted, free of charge, to any person obtaining
        a copy of this software and associated documentation files (the 
        "Software"), to deal in the Software without restriction, including
        without limitation the rights to use, copy, modify, merge, publish,
        distribute, sublicense, and/or sell copies of the Software, and to 
        permit persons to whom the Software is furnished to do so, subject to 
        the following conditions:

        The above copyright notice and this permission notice shall be included
        in all copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
        OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
        MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
        IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
        CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
        TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
        SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    */

    internal static class UnDXT {
        //! Use DXT1 compression.
        internal const int kDxt1 = 1 << 0;
        //! Use DXT3 compression.
        internal const int kDxt3 = 1 << 1;
        //! Use DXT5 compression.
        internal const int kDxt5 = 1 << 2;

        private static int FixFlags(int flags) {
            // grab the flag bits
            int method = flags & (kDxt1 | kDxt3 | kDxt5);

            // set defaults
            if (method != kDxt3 && method != kDxt5) {
                method = kDxt1;
            }

            // done
            return method;
        }

        internal static unsafe void DecompressImage(byte* rgba, int width, int height, void* blocks, int flags) {
            // fix any bad flags
            flags = FixFlags(flags);

            // initialise the block input
            byte* sourceBlock = (byte*) blocks;
            int bytesPerBlock = (flags & kDxt1) != 0 ? 8 : 16;

            // loop over blocks
            for (int y = 0; y < height; y += 4) {
                for (int x = 0; x < width; x += 4) {
                    // decompress the block
                    byte[] targetRgba = new byte[4*16];

                    // write the decompressed pixels to the correct image locations
                    fixed (byte* sourcePixelA = targetRgba) {
                        Decompress(sourcePixelA, sourceBlock, flags);
                        byte* sourcePixel = sourcePixelA;
                        for (int py = 0; py < 4; ++py) {
                            for (int px = 0; px < 4; ++px) {
                                // get the target location
                                int sx = x + px;
                                int sy = y + py;
                                if (sx < width && sy < height) {
                                    byte* targetPixel = rgba + 4*(width*sy + sx);

                                    // copy the rgba value
                                    for (int i = 0; i < 4; ++i) {
                                        *targetPixel++ = *sourcePixel++;
                                    }
                                } else // skip this pixel as its outside the image
                                {
                                    sourcePixel += 4;
                                }
                            }
                        }
                    }
                    // advance
                    sourceBlock += bytesPerBlock;
                }
            }
        }

        private static unsafe void Decompress(byte* rgba, void* block, int flags) {
            // fix any bad flags
            flags = FixFlags(flags);

            // get the block locations
            void* colourBlock = block;
            void* alphaBock = block;
            if ((flags & (kDxt3 | kDxt5)) != 0) {
                colourBlock = (byte*) block + 8;
            }

            // decompress colour
            DecompressColour(rgba, colourBlock, (flags & kDxt1) != 0);

            // decompress alpha separately if necessary
            if ((flags & kDxt3) != 0) {
                DecompressAlphaDxt3(rgba, alphaBock);
            } else if ((flags & kDxt5) != 0) {
                DecompressAlphaDxt5(rgba, alphaBock);
            }
        }

        private static unsafe int Unpack565(byte* packed, byte* colour) {
            // build the packed value
            int value = packed[0] | (packed[1] << 8);

            // get the components in the stored range
            byte red = (byte) ((value >> 11) & 0x1f);
            byte green = (byte) ((value >> 5) & 0x3f);
            byte blue = (byte) (value & 0x1f);

            // scale up to 8 bits
            colour[0] = (byte) ((red << 3) | (red >> 2));
            colour[1] = (byte) ((green << 2) | (green >> 4));
            colour[2] = (byte) ((blue << 3) | (blue >> 2));
            colour[3] = 255;

            // return the value
            return value;
        }

        private static unsafe void DecompressColour(byte* rgba, void* block, bool isDxt1) {
            // get the block bytes
            byte* bytes = (byte*) block;

            // unpack the endpoints
            byte[] codesA = new byte[16];
            byte[] indicesA = new byte[16];

            fixed (byte* codes = codesA) {
                fixed (byte* indices = indicesA) {
                    int a = Unpack565(bytes, codes);
                    int b = Unpack565(bytes + 2, codes + 4);

                    // generate the midpoints
                    for (int i = 0; i < 3; ++i) {
                        int c = codes[i];
                        int d = codes[4 + i];

                        if (isDxt1 && a <= b) {
                            codes[8 + i] = (byte) ((c + d)/2);
                            codes[12 + i] = 0;
                        } else {
                            codes[8 + i] = (byte) ((2*c + d)/3);
                            codes[12 + i] = (byte) ((c + 2*d)/3);
                        }
                    }

                    // fill in alpha for the intermediate values
                    codes[8 + 3] = 255;
                    codes[12 + 3] = (byte) (isDxt1 && a <= b ? 0 : 255);

                    // unpack the indices
                    for (int i = 0; i < 4; ++i) {
                        byte* ind = indices + 4*i;
                        byte packed = bytes[4 + i];

                        ind[0] = (byte) (packed & 0x3);
                        ind[1] = (byte) ((packed >> 2) & 0x3);
                        ind[2] = (byte) ((packed >> 4) & 0x3);
                        ind[3] = (byte) ((packed >> 6) & 0x3);
                    }

                    // store out the colours
                    for (int i = 0; i < 16; ++i) {
                        byte offset = (byte) (4*indices[i]);
                        for (int j = 0; j < 3; ++j) {
                            rgba[4*i + 2 - j] = codes[offset + j];
                        }
                        rgba[4*i + 3] = codes[offset + 3];
                    }
                }
            }
        }

        private static unsafe void DecompressAlphaDxt3(byte* rgba, void* block) {
            byte* bytes = (byte*) block;

            // unpack the alpha values pairwise
            for (int i = 0; i < 8; ++i) {
                // quantise down to 4 bits
                byte quant = bytes[i];

                // unpack the values
                byte lo = (byte) (quant & 0x0f);
                byte hi = (byte) (quant & 0xf0);

                // convert back up to bytes
                rgba[8*i + 3] = (byte) (lo | (lo << 4));
                rgba[8*i + 7] = (byte) (hi | (hi >> 4));
            }
        }

        private static unsafe void DecompressAlphaDxt5(byte* rgba, void* block) {
            // get the two alpha values
            byte* bytes = (byte*) block;
            int alpha0 = bytes[0];
            int alpha1 = bytes[1];

            // compare the values to build the codebook
            byte[] codes = new byte[8];
            codes[0] = (byte) alpha0;
            codes[1] = (byte) alpha1;
            if (alpha0 <= alpha1) {
                // use 5-alpha codebook
                for (int i = 1; i < 5; ++i) {
                    codes[1 + i] = (byte) (((5 - i)*alpha0 + i*alpha1)/5);
                }
                codes[6] = 0;
                codes[7] = 255;
            } else {
                // use 7-alpha codebook
                for (int i = 1; i < 7; ++i) {
                    codes[1 + i] = (byte) (((7 - i)*alpha0 + i*alpha1)/7);
                }
            }

            // decode the indices
            byte[] indices = new byte[16];
            byte* src = bytes + 2;
            fixed (byte* destP = indices) {
                byte* dest = destP;
                for (int i = 0; i < 2; ++i) {
                    // grab 3 bytes
                    int value = 0;
                    for (int j = 0; j < 3; ++j) {
                        int @byte = *src++;
                        value |= @byte << 8*j;
                    }

                    // unpack 8 3-bit values from it
                    for (int j = 0; j < 8; ++j) {
                        int index = (value >> 3*j) & 0x7;
                        *dest++ = (byte) index;
                    }
                }
            }
            // write out the indexed codebook values
            for (int i = 0; i < 16; ++i) {
                rgba[4*i + 3] = codes[indices[i]];
            }
        }
    }
}
