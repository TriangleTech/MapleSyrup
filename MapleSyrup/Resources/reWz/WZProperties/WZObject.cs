// reWZ is copyright angelsl, 2011 to 2015 inclusive.
// 
// This file (WZObject.cs) is part of reWZ.
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace reWZ.WZProperties {
    /// <summary>An object in a WZ file.</summary>
    public abstract class WZObject : IEnumerable<WZObject> {
        private readonly ChildCollection _backing;
        private readonly bool _canContainChildren;

        internal WZObject(string name, WZObject parent, WZFile container, bool children, WZObjectType type) {
            Name = string.Intern(name);
            Parent = parent;
            File = container;
            _canContainChildren = children;
            if (_canContainChildren) {
                _backing = new ChildCollection();
            }
            Type = type;
        }

        /// <summary>The name of the WZ object.</summary>
        public string Name { get; }

        /// <summary>The parent of this WZ object, or <code>null</code> if this is the main WZ directory.</summary>
        public WZObject Parent { get; }

        /// <summary>The absolute path to this object.</summary>
        public string Path => ConstructPath();

        /// <summary>The WZ file containing this object.</summary>
        public WZFile File { get; }

        /// <summary>The type of this object.</summary>
        public WZObjectType Type { get; }

        /// <summary>Returns the child with the name <paramref name="childName" /> .</summary>
        /// <param name="childName"> The name of the child to return. </param>
        /// <returns> The retrieved child. </returns>
        public virtual WZObject this[string childName] {
            get {
                if (!_canContainChildren) {
                    throw new NotSupportedException("This WZObject cannot contain children.");
                }
                return _backing[childName];
            }
        }

        /// <summary>Returns the number of children this property contains.</summary>
        public virtual int ChildCount => _canContainChildren ? _backing.Count : 0;

        /// <summary>Checks if this property has a child with name <paramref name="name" /> .</summary>
        /// <param name="name"> The name of the child to locate. </param>
        /// <returns> true if this property has such a child, false otherwise or if this property cannot contain children. </returns>
        public virtual bool HasChild(string name) {
            return _canContainChildren && _backing.Contains(name);
        }

        /// <summary>Tries to cast this to a <see cref="WZProperty{T}" /> and returns its value, or throws an exception if the cast
        ///     is invalid.</summary>
        /// <typeparam name="T"> The type of the value to return. </typeparam>
        /// <exception cref="System.InvalidCastException">This WZ object is not a
        ///     <see cref="WZProperty{T}" />
        ///     .</exception>
        /// <returns> The value enclosed by this WZ property. </returns>
        public T ValueOrDie<T>() {
            return ((WZProperty<T>) this).Value;
        }

        /// <summary>Tries to cast this to a <see cref="WZProperty{T}" /> and returns its value, or returns a default value if the
        ///     cast is invalid.</summary>
        /// <param name="default"> The value to return if the cast is unsuccessful. </param>
        /// <typeparam name="T"> The type of the value to return. </typeparam>
        /// <returns> The value enclosed by this WZ property, or the default value. </returns>
        public T ValueOrDefault<T>(T @default) {
            WZProperty<T> ret = this as WZProperty<T>;
            return ret != null ? ret.Value : @default;
        }

        /// <summary>Resolves a path in the form "/a/b/c/.././d/e/f/".</summary>
        /// <param name="path"> The path to resolve. </param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The path has an invalid node.</exception>
        /// <returns> The object located at the path. </returns>
        public WZObject ResolvePath(string path) {
            return
                (path.StartsWith("/") ? path.Substring(1) : path).Split('/')
                    .Where(node => node != ".")
                    .Aggregate(this,
                        (current, node) =>
                            node == ".." ? current.Parent : current[node]);
        }

        internal void Add(WZObject o) {
            _backing.Add(o);
        }

        private string ConstructPath() {
            StringBuilder s = new StringBuilder(Name);
            WZObject p = this;
            while ((p = p.Parent) != null) {
                s.Insert(0, "/").Insert(0, p.Name);
            }
            return string.Intern(s.ToString());
        }

        #region Nested type: ChildCollection

        private class ChildCollection : KeyedCollection<string, WZObject> {
            internal ChildCollection() : base(null, 4) {}

            protected override string GetKeyForItem(WZObject item) {
                return item.Name;
            }
        }

        #endregion

        #region IEnumerable<WZObject> Members

        /// <summary>Returns an enumerator that iterates through the children in this property.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the children
        ///     in this property.</returns>
        public virtual IEnumerator<WZObject> GetEnumerator() {
            return _canContainChildren ? _backing.GetEnumerator() : Enumerable.Empty<WZObject>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    public enum WZObjectType {
        Directory = 0,
        Image = 1,
        Null = 2,
        UInt16 = 3,
        Int32 = 4,
        Int64 = 14,
        Single = 5,
        Double = 6,
        String = 7,
        Point = 8,
        UOL = 9,
        Audio = 10,
        Canvas = 11,
        SubProperty = 12,
        Convex = 13
    }
}
