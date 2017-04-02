using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Util
{
    //The MIT License (MIT)

    //Copyright (c) 2013 Clinton Brennan

    //Permission is hereby granted, free of charge, to any person obtaining a copy
    //of this software and associated documentation files (the "Software"), to deal
    //in the Software without restriction, including without limitation the rights
    //to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    //copies of the Software, and to permit persons to whom the Software is
    //furnished to do so, subject to the following conditions:

    //The above copyright notice and this permission notice shall be included in
    //all copies or substantial portions of the Software.

    //THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    //IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    //FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    //AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    //LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    //OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    //THE SOFTWARE.
    using System;
    using System.Collections.Generic;
    using System.Collections;

    namespace JsonApiConverter.Util
    {
        public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
        {
            private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> mDictionary;
            private LinkedList<KeyValuePair<TKey, TValue>> mLinkedList;

            private ValueCollection valueCollection;
            private KeyCollection keyCollection;

            #region Constructors
            public OrderedDictionary()
            {
                mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
                mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
                valueCollection = new ValueCollection(this);
                keyCollection = new KeyCollection(this);
            }

            public OrderedDictionary(int capacity)
            {
                mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity);
                mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
                valueCollection = new ValueCollection(this);
                keyCollection = new KeyCollection(this);
            }

            public OrderedDictionary(IEqualityComparer<TKey> comparer)
            {
                mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(comparer);
                mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
                valueCollection = new ValueCollection(this);
                keyCollection = new KeyCollection(this);
            }
            #endregion Constructors

            public void Add(TKey key, TValue value)
            {
                LinkedListNode<KeyValuePair<TKey, TValue>> lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
                mDictionary.Add(key, lln);
                mLinkedList.AddLast(lln);
            }

            #region IDictionary Generic
            public bool ContainsKey(TKey key)
            {
                return mDictionary.ContainsKey(key);
            }

            public ICollection<TKey> Keys
            {
                get { return keyCollection; }
            }

            public bool Remove(TKey key)
            {
                LinkedListNode<KeyValuePair<TKey, TValue>> lln;
                bool found = mDictionary.TryGetValue(key, out lln);
                if (!found) { return false; }
                mDictionary.Remove(key);
                mLinkedList.Remove(lln);
                return true;
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                LinkedListNode<KeyValuePair<TKey, TValue>> lln;
                bool found = mDictionary.TryGetValue(key, out lln);
                if (!found)
                {
                    value = default(TValue);
                    return false;
                }
                value = lln.Value.Value;
                return true;
            }

            public ICollection<TValue> Values
            {
                get { return valueCollection; }
            }

            public TValue this[TKey key]
            {
                get
                {
                    return mDictionary[key].Value.Value;
                }
                set
                {
                    LinkedListNode<KeyValuePair<TKey, TValue>> lln;
                    if (mDictionary.ContainsKey(key))
                    {
                        lln = mDictionary[key];
                        lln.Value = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    else
                    {
                        lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
                        mLinkedList.AddLast(lln);
                        mDictionary.Add(key, lln);
                    }
                }
            }

            public void Clear()
            {
                mDictionary.Clear();
                mLinkedList.Clear();
            }

            public int Count
            {
                get { return mLinkedList.Count; }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerable<KeyValuePair<TKey, TValue>> MutateFriendlyEnumerable()
            {
                var current = this.mLinkedList.First;
                while(current != null)
                {
                    yield return current.Value;
                    current = current.Next;
                }
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return MutateFriendlyEnumerable().GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion IDictionary Generic

            #region Explicit ICollection Generic
            void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            {
                LinkedListNode<KeyValuePair<TKey, TValue>> lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(item);
                mDictionary.Add(item.Key, lln);
                mLinkedList.AddLast(lln);
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            {
                return mDictionary.ContainsKey(item.Key);
            }

            void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                mLinkedList.CopyTo(array, arrayIndex);
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
            {
                get { return false; }
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            {
                return Remove(item.Key);
            }
            #endregion Explicit ICollection Generic

            #region Explicit IEnumerable Generic
            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion Explicit IEnumerable Generic

            public sealed class KeyCollection : ICollection<TKey>
            {
                OrderedDictionary<TKey, TValue> parent;

                internal KeyCollection(OrderedDictionary<TKey, TValue> parent)
                {
                    this.parent = parent;
                }

                public int Count => parent.Count;

                public bool IsReadOnly => true;

                public void CopyTo(TKey[] array, int arrayIndex)
                {
                    parent.mLinkedList.Select(x=>x.Key).ToList().CopyTo(array, arrayIndex);
                }

                public void Add(TKey item)
                {
                    throw new NotImplementedException();
                }

                public void Clear()
                {
                    throw new NotImplementedException();
                }

                public bool Contains(TKey item)
                {
                    return parent.ContainsKey(item);
                }

                public bool Remove(TKey item)
                {
                    throw new NotImplementedException();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return ((IEnumerable<TKey>)this).GetEnumerator();
                }

                IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
                {
                    return parent.mLinkedList.Select(x=>x.Key).GetEnumerator();
                }
            }

            public sealed class ValueCollection : ICollection<TValue>
            {
                OrderedDictionary<TKey, TValue> parent;

                internal ValueCollection(OrderedDictionary<TKey, TValue> parent)
                {
                    this.parent = parent;
                }

                public int Count => parent.Count;

                public bool IsReadOnly => true;

                public void CopyTo(TValue[] array, int arrayIndex)
                {
                    parent.mLinkedList.Select(x => x.Value).ToList().CopyTo(array, arrayIndex);
                }

                public void Add(TValue item)
                {
                    throw new NotImplementedException();
                }

                public void Clear()
                {
                    throw new NotImplementedException();
                }

                public bool Contains(TValue item)
                {
                    return parent.mLinkedList.Any(x => x.Equals(item));
                }

                public bool Remove(TValue item)
                {
                    throw new NotImplementedException();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return ((IEnumerable<TValue>)this).GetEnumerator();
                }

                IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
                {
                    return parent.mLinkedList.Select(x => x.Value).GetEnumerator();
                }
            }
        }
    }
}
