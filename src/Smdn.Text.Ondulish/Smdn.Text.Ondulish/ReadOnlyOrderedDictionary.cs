// SPDX-FileCopyrightText: 2012 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Smdn.Text.Ondulish;

internal sealed class ReadOnlyOrderedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
  private readonly IReadOnlyList<KeyValuePair<TKey, TValue>> dictionary;

  public TValue this[TKey key] => throw new NotImplementedException();
  public IEnumerable<TKey> Keys => throw new NotImplementedException();
  public IEnumerable<TValue> Values => throw new NotImplementedException();
  public int Count => dictionary.Count;

  public ReadOnlyOrderedDictionary(IEnumerable<(TKey Key, TValue Value)> dictionary)
    : this(
      (dictionary ?? throw new ArgumentNullException(nameof(dictionary)))
      .Select(static pair => new KeyValuePair<TKey, TValue>(pair.Key, pair.Value))
      .ToList()
    )
  { }

  public ReadOnlyOrderedDictionary(IReadOnlyList<KeyValuePair<TKey, TValue>> dictionary)
  {
    this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
  }

  public bool ContainsKey(TKey key)
    => throw new NotImplementedException();

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    => dictionary.GetEnumerator();

  public bool TryGetValue(TKey key, out TValue value)
    => throw new NotImplementedException();

  IEnumerator IEnumerable.GetEnumerator()
    => dictionary.GetEnumerator();
}
