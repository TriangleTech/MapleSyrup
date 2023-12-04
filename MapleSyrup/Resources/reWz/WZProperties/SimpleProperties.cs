// reWZ is copyright angelsl, 2011 to 2015 inclusive.
// 
// This file (SimpleProperties.cs) is part of reWZ.
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

using System.Collections.Generic;
using System.Drawing;
using Point = System.Drawing.Point;

namespace reWZ.WZProperties {
    /// <summary>A struct used to represent nothing.</summary>
    public struct WZNothing {}

    /// <summary>Null.</summary>
    public sealed class WZNullProperty : WZProperty<WZNothing> {
        internal WZNullProperty(string name, WZObject parent, WZImage container)
            : base(name, parent, default(WZNothing), container, false, WZObjectType.Null) {}
    }

    /// <summary>An unsigned 16-bit integer property.</summary>
    public sealed class WZUInt16Property : WZProperty<ushort> {
        internal WZUInt16Property(string name, WZObject parent, WZBinaryReader reader, WZImage container)
            : base(name, parent, reader.ReadUInt16(), container, false, WZObjectType.UInt16) {}
    }

    /// <summary>A compressed signed 32-bit integer property.</summary>
    public sealed class WZInt32Property : WZProperty<int> {
        internal WZInt32Property(string name, WZObject parent, WZBinaryReader reader, WZImage container)
            : base(name, parent, reader.ReadWZInt(), container, false, WZObjectType.Int32) {}
    }

    /// <summary>A compressed signed 64-bit integer property.</summary>
    public sealed class WZInt64Property : WZProperty<long> {
        internal WZInt64Property(string name, WZObject parent, WZBinaryReader reader, WZImage container)
            : base(name, parent, reader.ReadWZLong(), container, false, WZObjectType.Int64) {}
    }

    /// <summary>A floating point number with single precision property.</summary>
    public sealed class WZSingleProperty : WZProperty<float> {
        internal WZSingleProperty(string name, WZObject parent, WZBinaryReader reader, WZImage container)
            : base(name, parent, ReadSingle(reader), container, false, WZObjectType.Single) {}

        private static float ReadSingle(WZBinaryReader reader) {
            byte t = reader.ReadByte();
            return t == 0x80
                ? reader.ReadSingle()
                : (t == 0 ? 0f : WZUtil.Die<float>("Unknown byte while reading WZSingleProperty."));
        }
    }

    /// <summary>A floating point number with double precision property.</summary>
    public sealed class WZDoubleProperty : WZProperty<double> {
        internal WZDoubleProperty(string name, WZObject parent, WZBinaryReader reader, WZImage container)
            : base(name, parent, reader.ReadDouble(), container, false, WZObjectType.Double) {}
    }

    /// <summary>A string property.</summary>
    public sealed class WZStringProperty : WZDelayedProperty<string> {
        internal WZStringProperty(string name, WZObject parent, WZBinaryReader reader, WZImage container)
            : base(name, parent, container, reader, false, WZObjectType.String) {}

        internal override bool Parse(WZBinaryReader r, bool initial, out string result) {
            if (!initial || (File._flag & WZReadSelection.EagerParseStrings) == WZReadSelection.EagerParseStrings) {
                result = string.Intern(r.ReadWZStringBlock(Image._encrypted));
                return true;
            } else {
                r.SkipWZStringBlock();
                result = null;
                return false;
            }
        }
    }

    /// <summary>A point property, containing an X and Y value pair.</summary>
    public sealed class WZPointProperty : WZProperty<Point> {
        internal WZPointProperty(string name, WZObject parent, WZBinaryReader wzbr, WZImage container)
            : base(name, parent, new Point(wzbr.ReadWZInt(), wzbr.ReadWZInt()), container, false, WZObjectType.Point) {}
    }

    /// <summary>A link property, used to link to other properties in the WZ file.</summary>
    public sealed class WZUOLProperty : WZProperty<string> {
        private WZObject _finalTarget;
        private bool _resolved;

        internal WZUOLProperty(string name, WZObject parent, WZBinaryReader reader, WZImage container)
            : base(
                name, parent, string.Intern(reader.ReadWZStringBlock(container._encrypted)), container, false,
                WZObjectType.UOL) {}

        /// <summary>Returns the <see cref="WZObject" /> that is directly pointed to by this UOL.</summary>
        public WZObject Target => WZUtil.ResolvePath(Parent, Value);

        public WZObject FinalTarget {
            get {
                // No need lock here, it's okay to repeat work
                if (_resolved) {
                    return _finalTarget;
                }
                _finalTarget = ResolveFully();
                _resolved = true;
                return _finalTarget;
            }
        }

        private WZObject ResolveFully() {
            HashSet<WZObject> traversed = new HashSet<WZObject> {this};
            WZObject ret = this;
            while (ret is WZUOLProperty) {
                ret = ((WZUOLProperty) ret).Target;
                if (ret == null || traversed.Contains(ret)) {
                    return null;
                }
                traversed.Add(ret);
            }
            return ret;
        }
    }
}
