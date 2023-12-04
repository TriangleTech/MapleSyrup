// UsefulThings is copyright angelsl, 2011 to 2015 inclusive.
// 
// This file (PointerStream.cs) is part of UsefulThings.
// 
// UsefulThings is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// UsefulThings is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with UsefulThings. If not, see <http://www.gnu.org/licenses/>.
// 
// Linking UsefulThings statically or dynamically with other modules
// is making a combined work based on UsefulThings. Thus, the terms and
// conditions of the GNU General Public License cover the whole combination.
// 
// As a special exception, the copyright holders of UsefulThings give you
// permission to link UsefulThings with independent modules to produce an
// executable, regardless of the license terms of these independent modules,
// and to copy and distribute the resulting executable under terms of your
// choice, provided that you also meet, for each linked independent module,
// the terms and conditions of the license of that module. An independent
// module is a module which is not derived from or based on UsefulThings.

using System.Runtime.CompilerServices;

namespace UsefulThings {
    internal unsafe class PointerStream : IBytePointerObject {
        protected readonly byte* _start;
        protected readonly byte* _end;
        protected readonly long _length;
        protected byte* _cur;

        public PointerStream(byte* start, long length) {
            _start = start;
            _cur = start;
            _end = start + length;
            _length = length;
        }

        public long Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _length; }
        }

        public byte* Pointer {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _cur; }
        }

        public long Position {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return _cur - _start; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] set { _cur = _start + value; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(byte[] buffer, long offset, long count) {
            ByteMarshal.CopyTo(_cur, buffer, offset, count);
            _cur += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(byte[] buffer) {
            Read(buffer, 0, buffer.LongLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() {
            return *_cur++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBoolean() {
            bool ret = *((bool*) _cur);
            _cur += sizeof(bool);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte() {
            sbyte ret = *((sbyte*) _cur);
            _cur += sizeof(sbyte);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar() {
            char ret = *((char*) _cur);
            _cur += sizeof(char);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16() {
            short ret = *((short*) _cur);
            _cur += sizeof(short);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16() {
            ushort ret = *((ushort*) _cur);
            _cur += sizeof(ushort);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32() {
            int ret = *((int*) _cur);
            _cur += sizeof(int);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32() {
            uint ret = *((uint*) _cur);
            _cur += sizeof(uint);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64() {
            long ret = *((long*) _cur);
            _cur += sizeof(long);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64() {
            ulong ret = *((ulong*) _cur);
            _cur += sizeof(ulong);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle() {
            float ret = *((float*) _cur);
            _cur += sizeof(float);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble() {
            double ret = *((double*) _cur);
            _cur += sizeof(double);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal ReadDecimal() {
            decimal ret = *((decimal*) _cur);
            _cur += sizeof(decimal);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadBytes(long count) {
            byte[] ret = new byte[count];
            ByteMarshal.CopyTo(_cur, ret, 0, count);
            _cur += count;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] buffer, long offset, long count) {
            ByteMarshal.CopyTo(buffer, offset, _cur, count);
            _cur += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] buffer) {
            Write(buffer, 0, buffer.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(bool value) {
            * ((bool*) _cur) = value;
            _cur += sizeof (bool);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value) {
            *_cur = value;
            _cur += sizeof(byte);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte value) {
            *((sbyte*)_cur) = value;
            _cur += sizeof(sbyte);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char value) {
            *((char*) _cur) = value;
            _cur += sizeof(char);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(double value) {
            *((double*) _cur) = value;
            _cur += sizeof(double);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(decimal value) {
            *((decimal*) _cur) = value;
            _cur += sizeof(decimal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value) {
            *((short*) _cur) = value;
            _cur += sizeof(short);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort value) {
            *((ushort*) _cur) = value;
            _cur += sizeof(ushort);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value) {
            *((int*) _cur) = value;
            _cur += sizeof(int);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value) {
            *((uint*) _cur) = value;
            _cur += sizeof(uint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value) {
            *((long*) _cur) = value;
            _cur += sizeof(long);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value) {
            *((ulong*) _cur) = value;
            _cur += sizeof(ulong);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(float value) {
            *((float*) _cur) = value;
            _cur += sizeof(float);
        }

        public void Dispose() {}
    }
}