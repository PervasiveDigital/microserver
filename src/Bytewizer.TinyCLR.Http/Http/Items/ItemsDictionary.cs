﻿using System;
using System.Collections;

namespace Bytewizer.TinyCLR.Http
{
    /// <summary>
    /// An <see cref="ItemsDictionary"/> type for route values.
    /// </summary>
    public class ItemsDictionary : ICollection, IEnumerable
    {
        /// <summary>
        /// The array list used to store the pairs.
        /// </summary>
        private ArrayList _pairs;

        /// <summary>
        /// Initializes an empty uninitialized instance of the <see cref="ItemsDictionary" /> class.
        /// </summary>
        public static readonly ItemsDictionary Empty = new ItemsDictionary();

        /// <summary>
        /// Initializes a new empty uninitialized instance of class.
        /// </summary>
        public ItemsDictionary() { }

        /// <summary>
        /// Initializes a new, empty instance of the class.
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// </summary>
        public ItemsDictionary(int capacity)
        {
            _pairs = new ArrayList
            {
                Capacity = capacity
            };
        }

        /// <summary>
        ///  Initializes a new, empty instance of the class using the specified <see cref="ArrayList"/>.
        /// </summary>
        public ItemsDictionary(ArrayList pairs)
        {
            _pairs = pairs;
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found,
        /// attempting to get it returns null, and attempting to set it creates a new element
        /// using the specified key.
        /// </returns>
        public object this[object key]
        {
            get
            {
                if (_pairs == null)
                {
                    return null;
                }

                for (int i = 0; i < Count; i++)
                {
                    ItemsValue kvp = (ItemsValue)_pairs[i];
                    if (kvp.Key == key)
                    {
                        return kvp.Value;
                    }
                }
                return null;
            }
            set
            {
                if (_pairs == null)
                {
                    _pairs = new ArrayList();
                }

                for (int i = 0; i < Count; i++)
                {
                    ItemsValue kvp = (ItemsValue)_pairs[i];
                    if (kvp.Key == key)
                    {
                        kvp.Value = value;
                        return;
                    }
                }
                _pairs.Add(new ItemsValue(key, value));
            }
        }

        /// <summary>
        /// Gets the value associated with the specified index.
        /// </summary>
        /// <param name="index">The index whose value to get.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found,
        /// attempting to get it returns null, and attempting to set it creates a new element
        /// using the specified key.
        /// </returns>
        public ItemsValue this[int index]
        {
            get 
            { 
                return (ItemsValue)_pairs[index]; 
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the keys in the collection.
        /// </summary>
        public ICollection Keys
        {
            get
            {
                if (_pairs == null)
                {
                    return new ArrayList();
                }

                ArrayList list = new ArrayList();
                foreach (ItemsValue kvp in _pairs)
                {
                    list.Add(kvp.Key);
                }
                return list;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> containing the values in the collection.
        /// </summary>
        public ICollection Values
        {
            get
            {
                if (_pairs == null)
                {
                    return new ArrayList();
                }

                ArrayList list = new ArrayList();
                foreach (ItemsValue kvp in _pairs)
                {
                    list.Add(kvp.Value);
                }
                return list;
            }
        }

        /// <summary>
        /// Adds the specified element to the collection.
        /// </summary>
        /// <param name="key">The key to use as the key of the element to add.</param>
        /// <param name="value">The value of the rule to add.</param>
        public void Add(object key, object value)
        {
            if (_pairs == null)
            {
                _pairs = new ArrayList();
            }

            this[key] = value;
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (_pairs != null)
            {
                _pairs.Clear();
            }
        }

        /// <summary>
        /// Determines whether the collection contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the collection.</param>
        public bool Contains(object key)
        {
            if (_pairs == null)
            {
                return false;
            }

            return _pairs.Contains(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the
        /// key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if the object that implements collection contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Specified key is <c>null</c></exception>
        public bool TryGetValue(object key, out object value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var item = this[key];

            if (item == null)
            {
                value = default;
                return false;
            }

            value = item;
            return true;
        }

        /// <summary>
        /// Removes the element with the specified key from the collection.
        /// </summary>
        /// <param name="item">The <see cref="ItemsValue"/> to remove from the collection.</param>
        public void Remove(ItemsValue item)
        {
            if (_pairs != null)
            {
                _pairs.Remove(item.Key);
            }
        }

        /// <summary>
        /// Removes the element with the specified key from the collection.
        /// </summary>
        /// <param name="key">The key to remove from the collection.</param>
        public void Remove(object key)
        {
            if (_pairs != null)
            {
                for (int i = 0; i < Count; i++)
                {
                    ItemsValue nvp = (ItemsValue)_pairs[i];
                    if (nvp.Key == key)
                    {
                        _pairs.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        #region ICollection Members

        /// <summary>
        /// The one-dimensional array of type <see cref="ItemsValue"/> that is the destination of <see cref="ItemsValue"/> 
        /// objects copied from <see cref="ICollection"/>. The array must have zero-based indexing.
        /// </summary>
        /// <param name="array">The one-dimensional array of <see cref="ItemsValue"/> that is the destination of the elements copied from the collection. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
		public void CopyTo(ItemsValue[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (!(array is ItemsValue[]typedArray))
                throw new InvalidCastException(nameof(array));

            if (index < 0 || (typedArray.Length - index) < Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (_pairs != null)
            {
                foreach (string key in Keys)
                {
                    typedArray[index++] = new ItemsValue(key, this[key]);
                }
            }
        }

        /// <summary>
        /// The one-dimensional <see cref="Array"/> that is the destination of <see cref="ItemsValue"/> 
        /// objects copied from <see cref="ICollection"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from the collection. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            if (_pairs != null)
            {
                if (_pairs.Count > 0)
                {
                    _pairs.CopyTo(array, index);
                }
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                if (_pairs == null)
                {
                    return 0;
                }

                return _pairs.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                if (_pairs == null)
                {
                    return false;
                }

                return _pairs.IsFixedSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (_pairs == null)
                {
                    return false;
                }

                return _pairs.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                if (_pairs == null)
                {
                    return false;
                }

                return _pairs.IsSynchronized;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                if (_pairs == null)
                {
                    return new ArrayList().SyncRoot;
                }

                return _pairs.SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns <see cref="IEnumerator"/> that iterates through the collection.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return new ItemsEnumerator(this);
        }

        #endregion
    }
}