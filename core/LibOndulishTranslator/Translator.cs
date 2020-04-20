//
// Author:
//       smdn <smdn@smdn.jp>
//
// Copyright (c) 2012 smdn
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if !(NETSTANDARD2_1)
using Smdn.Collections;
#endif
using Smdn.Formats.Csv;
using Smdn.IO;

using MeCab;
using MeCabConsts = MeCab.MeCab;

namespace Smdn.Applications.OndulishTranslator {
  public class Translator : IDisposable {
    public IReadOnlyDictionary<string, string> PhraseDictionary { get; }
    public IReadOnlyDictionary<string, string> WordDictionary { get; }

    public Translator(string taggerArgs, string dictionaryDirectory)
    {
      tagger = new Tagger(taggerArgs);

      if (tagger == null)
        throw new DllNotFoundException("can't create tagger");

      PhraseDictionary = LoadDictionary(System.IO.Path.Combine(dictionaryDirectory, "dictionary-phrases.csv"));
      WordDictionary = LoadDictionary(System.IO.Path.Combine(dictionaryDirectory, "dictionary-words.csv"));
    }

    private static readonly char[] dictionaryPunctuationChars = new[] {'！', '？', '!', '?', '、', '。'};

    private static SortedList<string, string> LoadDictionary(string dictionaryPath)
    {
      var dictionary = new SortedList<string, string>(new WordDictionaryComparer());

      using (var reader = new CsvReader(dictionaryPath, Encoding.UTF8)) {
        for (;;) {
          var entries = reader.ReadLine();

          if (entries == null)
            break;
          if (entries.Length < 3)
            continue;

          entries[0] = entries[0].Trim();

          if (entries[0].StartsWith('#'))
            continue; // comment line

          entries[1] = entries[1].Trim().RemoveChars(dictionaryPunctuationChars);

          dictionary[KanaUtils.ConvertWideHiraganaToKatakana(entries[1])] = entries[2].Trim();
        }
      }

#if false
      foreach (var e in dictionary) {
        Console.WriteLine("{0} => {1}", e.Key, e.Value);
      }
#endif

      return dictionary;
    }

    private class WordDictionaryComparer : IComparer<string> {
      public int Compare(string x, string y)
        => x.Length == y.Length
          ? StringComparer.Ordinal.Compare(x, y)
          : y.Length - x.Length;
    }

    public void Dispose()
    {
      tagger?.Dispose();
      tagger = null;
    }

    public string Translate(string input, bool convertKatakanaToNarrow)
    {
      var sb = new StringBuilder(input.Length * 2);
      var sw = new StringWriter(sb);

      Translate(input, convertKatakanaToNarrow, sw);

      return sb.ToString();
    }

    public void Translate(string input, bool convertKatakanaToNarrow, TextWriter output)
    {
      var reader = new StringReader(input);

      foreach (var line in reader.ReadLines()) {
        if (string.IsNullOrWhiteSpace(line)) {
          output.WriteLine(line);
          continue;
        }

        var fragments =
          ConvertWithDictionary(
            ConvertToKatakana(line),
            PhraseDictionary
          )
          .SelectMany(f =>
            f.ConvertedText == null
              ? ConvertWithDictionary(f.SourceText, WordDictionary)
              : Enumerable.Repeat(f, 1)
          )
          .SelectMany(f =>
            f.ConvertedText == null
              ? ConvertWithDictionary(f.SourceText, phonemeDictionary)
              : Enumerable.Repeat(f, 1)
          )
          .Select(f =>
            new TextFragment(
              f.SourceText,
              f.ConvertedText ?? KanaUtils.ConvertWideHiraganaToKatakana(f.SourceText) // redundant?
            )
          );

        var result = string.Concat(fragments.Select(fragment => fragment.ConvertedText));

        if (convertKatakanaToNarrow)
          result = KanaUtils.ConvertWideKatakanaToNarrowKatakana(result);

        output.WriteLine(result);
      }

      output.Flush();
    }

    private static readonly char[] featureSplitter = new[] {','};

    private string ConvertToKatakana(string input)
    {
      input = input.Replace(",", "，"); // XXX: feature splitter

      var ret = new StringBuilder(input.Length * 2);

      for (var node = tagger.parseToNode(input); node != null; node = node.next) {
        if (node.stat == MeCabConsts.MECAB_BOS_NODE || node.stat == MeCabConsts.MECAB_EOS_NODE)
          continue;

#if false
        Console.WriteLine("feature: {0}", node.feature);
#endif

        var featureEntries = node.feature.Split(featureSplitter);

        if (8 <= featureEntries.Length) {
          switch (featureEntries[6]) {
            case "ぶっ殺す": ret.Append("ブッコロス"); break; // ipadic says 'ぶっとばす'
            default: ret.Append(featureEntries[7]); break;
          }
        }
        else {
          ret.Append(node.surface);
        }
      }

#if false
      Console.WriteLine("{0} {1}", input, ret);
#endif

      return ret.ToString();
    }

    readonly struct TextFragment {
      public readonly string SourceText;
      public readonly string ConvertedText;

      public TextFragment(string sourceText, string convertedText)
      {
        this.SourceText = sourceText;
        this.ConvertedText = convertedText;
      }
    }

    private static bool FindMostLeftAndLongestCandidate(
      string input,
      int startIndex,
      IReadOnlyDictionary<string, string> dictionary,
      out int position,
      out KeyValuePair<string, string> candidate
    )
    {
      position = int.MaxValue;
      candidate = default;

      foreach (var entry in dictionary) {
        var pos = input.IndexOf(entry.Key, startIndex, StringComparison.Ordinal);

        if (0 <= pos && pos < position) {
          position = pos;
          candidate = entry;
        }
      }

      return position != int.MaxValue;
    }

    private static IEnumerable<TextFragment> ConvertWithDictionary(
      string input,
      IReadOnlyDictionary<string, string> dictionary
    )
    {
      var offset = 0;

      while (FindMostLeftAndLongestCandidate(input, offset, dictionary, out var position, out var candidate)) {
        if (offset < position)
          yield return new TextFragment(input.Substring(offset, position - offset), null);

        yield return new TextFragment(candidate.Key, candidate.Value);

        offset = position + candidate.Key.Length;
      }

      yield return new TextFragment(input.Substring(offset), null);
    }

    private class ReadOnlyOrderedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
      private readonly IReadOnlyList<KeyValuePair<TKey, TValue>> dictionary;

      public TValue this[TKey key] => throw new NotImplementedException();
      public IEnumerable<TKey> Keys => throw new NotImplementedException();
      public IEnumerable<TValue> Values => throw new NotImplementedException();
      public int Count => dictionary.Count;

      public ReadOnlyOrderedDictionary(IEnumerable<(TKey key, TValue value)> dictionary)
        : this(
          (dictionary ?? throw new ArgumentNullException(nameof(dictionary)))
          .Select(pair => KeyValuePair.Create(pair.key, pair.value))
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

    private readonly IReadOnlyDictionary<string, string> phonemeDictionary = new ReadOnlyOrderedDictionary<string, string>(new[] {
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
    });

    private Tagger tagger;
  }
}

