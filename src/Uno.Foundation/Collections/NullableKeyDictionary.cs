#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace Uno.Foundation.Collections
{
	/// <summary>
	/// Represents a variant of dictionary that allows adding a value for a null key.
	/// </summary>
	/// <typeparam name="TKey">Key.</typeparam>
	/// <typeparam name="TValue">Value.</typeparam>
	internal class NullableKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
		  where TKey : class
	{
		private TValue _nullValue = default!;
		private bool _containsNullValue;
		private readonly Dictionary<TKey, TValue> _dictionary;

		public NullableKeyDictionary() =>
			_dictionary = new Dictionary<TKey, TValue>();

		public NullableKeyDictionary(IEqualityComparer<TKey> comparer) =>
			_dictionary = new Dictionary<TKey, TValue>(comparer);

		public bool ContainsKey(TKey key) => key == null ? _containsNullValue : _dictionary.ContainsKey(key);

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				_nullValue = value;
				_containsNullValue = true;
			}
			else
			{
				_dictionary[key] = value;
			}
		}

		public bool Remove(TKey key)
		{
			if (key != null)
			{
				return _dictionary.Remove(key);
			}

			if (_containsNullValue)
			{
				_nullValue = default!;
				_containsNullValue = false;
				return true;
			}

			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key != null)
			{
				return _dictionary.TryGetValue(key, out value!);
			}

			if (_containsNullValue)
			{
				value = _nullValue;
				return true;
			}
			else
			{
				value = default!;
				return false;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (key != null)
				{
					TValue ret;

					_dictionary.TryGetValue(key, out ret);

					return ret!;
				}
				else
				{
					return _nullValue!;
				}
			}
			set
			{
				if (key != null)
				{
					_dictionary[key] = value;
				}
				else
				{
					_nullValue = value;
					_containsNullValue = true;
				}
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				if (!_containsNullValue)
				{
					return _dictionary.Keys;
				}
				else
				{
					var keys = new List<TKey>(_dictionary.Keys);
					keys.Add(null!);
					return keys;
				}
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				if (!_containsNullValue)
				{
					return _dictionary.Values;
				}
				else
				{
					var values = new List<TValue>(_dictionary.Values);
					values.Add(_nullValue);
					return values;
				}
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (KeyValuePair<TKey, TValue> kvp in _dictionary)
			{
				yield return kvp;
			}

			if (_containsNullValue)
			{
				yield return new KeyValuePair<TKey, TValue>(null!, _nullValue);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			if (item.Key == null)
			{
				_nullValue = item.Value;
				_containsNullValue = true;
			}
			else
			{
				_dictionary.Add(item.Key, item.Value);
			}
		}

		public void Clear()
		{
			_dictionary.Clear();
			_nullValue = default!;
			_containsNullValue = false;
		}

		public bool Contains(KeyValuePair<TKey, TValue> item) =>
			TryGetValue(item.Key, out var val) ? Equals(item.Value, val) : false;

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			foreach(var pair in this)
			{
				array[arrayIndex] = pair;
				arrayIndex++;
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> item) => Contains(item) ? Remove(item.Key) : false;

		public int Count => _containsNullValue ? _dictionary.Count + 1 : _dictionary.Count;

		public bool IsReadOnly => false;
	}
}
