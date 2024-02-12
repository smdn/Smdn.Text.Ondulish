// SPDX-FileCopyrightText: 2012 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Smdn.Formats.Csv;

namespace Smdn.Text.Ondulish;

#pragma warning disable IDE0040
partial class Translator {
#pragma warning restore IDE0040

  public IReadOnlyDictionary<string, string> PhraseDictionary { get; }
  public IReadOnlyDictionary<string, string> WordDictionary { get; }

  private sealed class WordDictionaryComparer : IComparer<string> {
    public int Compare(string? x, string? y)
    {
      x ??= string.Empty;
      y ??= string.Empty;

      return x.Length == y.Length
        ? StringComparer.Ordinal.Compare(x, y)
        : y.Length - x.Length;
    }
  }

  private static readonly char[] DictionaryPunctuationChars = new[] { '！', '？', '!', '?', '、', '。' };

  private static SortedList<string, string> LoadDictionary(Stream stream)
  {
    var dictionary = new SortedList<string, string>(new WordDictionaryComparer());

    using var reader = new CsvReader(stream, Encoding.UTF8);

    foreach (var entries in reader.ReadRecords()) {
      if (entries.Count < 3)
        continue;

      var entry = entries[0].Trim();

      if (entry.StartsWith('#'))
        continue; // comment line

      var key = entries[1].Trim().RemoveChars(DictionaryPunctuationChars);

      dictionary[KanaUtils.ConvertWideHiraganaToKatakana(key).ToLowerInvariant()] = entries[2].Trim().ToLowerInvariant();
    }

    return dictionary;
  }

  private static readonly IReadOnlyDictionary<string, string> PhonemeDictionary =
    new ReadOnlyOrderedDictionary<string, string>(
      new[] {
        // 最優先
        ("ル", "ドゥ"),
        ("ム", "ヴ"),
        ("ボー", "ポッ"),
        ("ドー", "ドゥー"),
        ("スナ", "スダ"),
        ("スルナ", "ドゥルダ"),
        ("スル", "ドゥル"),
        ("デモ", "デロ"),
        ("ンヤ", "ッニャ"),
        ("ネイ", "ニッ"),
        ("ネエ", "ニェ"),
        ("デス", "ディス"),
        ("ウラ", "ルラ"),
        ("トオ", "ドーゥ"),
        ("いじゃ", "チョナ"),
        ("とは", "トヴァ"),

        // 母音
        ("ア", "ア゛"),
        ("ウ", "ル"),
        ("ヤ", "ャ"),

        // 摩擦音
        ("サ", "ザァ"),
        ("ス", "ズ"),
        ("ゼ", "デ"),

        ("ハ", "ヴァ"),
        ("ヒ", "ビィ"),
        ("フ", "ヴ"),
        ("ヘ", "ベ"),
        ("ホ", "ボ"),

        ("ブ", "ム"),

        ("ゼ", "デ"),

        // 破裂音
        ("ク", "グ"),
        ("キ", "ク"),

        ("タ", "ダ"),
        ("チ", "ディ"),
        ("ツ", "ヅ"),
        ("テ", "デ"),
        ("ト", "ドゥ"),

        ("ピ", "ヴィ"),

        // 鼻音
        ("ニ", "ディ"),
        ("ヌ", "ズ"),
        ("ネ", "ベ"),
        ("ノ", "ド"),

        ("マ", "バ"),
        ("ミ", "ヴィ"),
        ("メ", "ベ"),
        ("モ", "ボ"),

        // 流音
        ("リ", "ディ"),
        ("レ", "リ"),
        ("ロ", "ド"),
      }
    );
}
