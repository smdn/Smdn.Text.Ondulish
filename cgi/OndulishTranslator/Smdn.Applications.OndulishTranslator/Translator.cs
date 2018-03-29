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
using System.Collections.Generic;
using System.Text;

using Smdn.Formats.Csv;

using MeCab;
using MeCabConsts = MeCab.MeCab;

namespace Smdn.Applications.OndulishTranslator {
  public class Translator : IDisposable {
    public Translator(string taggerArgs, string dictionaryPath)
    {
      tagger = new Tagger(taggerArgs);

      if (tagger == null)
        throw new DllNotFoundException("can't create tagger");

      wordDictionary = LoadDictionary(dictionaryPath);
    }

    private static readonly char[] dictionaryPunctuationChars = new[] {'！', '？', '!', '?', '、', '。'};

    private static SortedList<string, string> LoadDictionary(string dictionaryPath)
    {
      var dictionary = new SortedList<string, string>(new WordDictionaryComparer());

      using (var reader = new CsvReader(dictionaryPath, Encoding.UTF8)) {
        for (;;) {
          var entries = reader.ReadLine();

          if (entries == null) {
            break;
          }
          else if (3 <= entries.Length) {
            entries[0] = entries[0].Trim();

            if (entries[0].StartsWith("#", StringComparison.Ordinal))
              continue; // comment line

            entries[1] = entries[1].Trim().RemoveChars(dictionaryPunctuationChars);

            dictionary[KanaUtils.ConvertWideHiraganaToKatakana(entries[1])] = entries[2].Trim();
          }
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
      {
        if (x.Length == y.Length)
          return StringComparer.Ordinal.Compare(x, y);
        else
          return y.Length - x.Length;
      }
    }

    public void Dispose()
    {
      if (tagger != null) {
        tagger.Dispose();
        tagger = null;
      }
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

      for (;;) {
        var line = reader.ReadLine();

        if (line == null)
          break;

        if (line.Trim().Length == 0) {
          output.WriteLine(line);
          continue;
        }

        line = ConvertToKatakana(line);

        var fragments = ConvertWords(line);

        fragments = ConvertPhoneme(fragments);

        var result = JoinFragments(fragments);

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
      var inputBytes = Encoding.UTF8.GetBytes(input);
      var inputOffset = 0;

      for (var node = tagger.parseToNode(input); node != null; node = node.next) {
        if (node.stat == MeCabConsts.MECAB_BOS_NODE || node.stat == MeCabConsts.MECAB_EOS_NODE)
          continue;

#if false
        Console.WriteLine("feature: {0}", node.feature);
#endif

        var featureEntries = node.feature.Split(featureSplitter);

        if (8 <= featureEntries.Length)
          ret.Append(featureEntries[7]);
        else
          // XXX
          ret.Append(KanaUtils.ConvertWideHiraganaToKatakana(Encoding.UTF8.GetString(inputBytes, inputOffset, node.length)));

        inputOffset += node.length;
      }

#if false
      Console.WriteLine("{0} {1}", input, ret);
#endif

      return ret.ToString();
    }

    struct TextFragment {
      public readonly string SourceText;
      public readonly string ConvertedText;

      public TextFragment(string sourceText, string convertedText)
      {
        this.SourceText = sourceText;
        this.ConvertedText = convertedText;
      }
    }

    private List<TextFragment> ConvertWords(string input)
    {
      var fragments = new List<TextFragment>();
      var offset = 0;

      for (;;) {
        var posCandidate = int.MaxValue;
        KeyValuePair<string, string> candidate;

        foreach (var entry in wordDictionary) {
          var pos = input.IndexOf(entry.Key, offset, StringComparison.Ordinal);

          if (0 <= pos && pos < posCandidate) {
            posCandidate = pos;
            candidate = entry;
          }
        }

        if (posCandidate == int.MaxValue)
          // nothing to convert
          break;

        var preFragment = input.Substring(offset, posCandidate - offset);

        fragments.Add(new TextFragment(preFragment, null));

        fragments.Add(new TextFragment(candidate.Key, candidate.Value));

        offset = posCandidate + candidate.Key.Length;

        var postFragment = input.Substring(offset);

        fragments.Add(new TextFragment(postFragment, null));
      }

      if (fragments.Count == 0)
        // if no dictionary entry found
        fragments.Add(new TextFragment(input, null));

      return fragments;
    }

    private List<TextFragment> ConvertPhoneme(List<TextFragment> inputFragments)
    {
      for (var i = 0; i < inputFragments.Count; i++) {
        if (inputFragments[i].ConvertedText != null)
          continue;

        inputFragments[i] = new TextFragment(inputFragments[i].SourceText, ConvertPhoneme(inputFragments[i].SourceText));
      }

      return inputFragments;
    }

    private static string ConvertPhoneme(string input)
    {
      var ret = new StringBuilder(input);

      // 最優先
      ret.Replace("ル", "ドゥ");
      ret.Replace("ム", "ヴ");

      // 母音
      ret.Replace("ア", "ア゛");
      ret.Replace("ウ", "ル");
      ret.Replace("ヤ", "ャ");

      // 摩擦音
      ret.Replace("サ", "ザァ");
      ret.Replace("ス", "ズ");
      ret.Replace("ゼ", "デ");

      ret.Replace("ハ", "ヴァ");
      ret.Replace("ヒ", "ビィ");
      ret.Replace("フ", "ヴ");
      ret.Replace("ヘ", "ベ");
      ret.Replace("ホ", "ボ");

      ret.Replace("ブ", "ム");

      ret.Replace("ゼ", "デ");

      // 破裂音
      ret.Replace("ク", "グ");
      ret.Replace("キ", "ク");

      ret.Replace("デ", "ディ");

      ret.Replace("タ", "ダ");
      ret.Replace("チ", "ディ");
      ret.Replace("ツ", "ヅ");
      ret.Replace("テ", "デ");
      ret.Replace("ト", "ドゥ");

      ret.Replace("ピ", "ヴィ");

      // 鼻音
      ret.Replace("ニ", "ディ");
      ret.Replace("ヌ", "ズ");
      ret.Replace("ネ", "ベ");
      ret.Replace("ノ", "ド");

      ret.Replace("マ", "バ");
      ret.Replace("ミ", "ヴィ");
      ret.Replace("メ", "ベ");
      ret.Replace("モ", "ボ");

      ret.Replace("リ", "ディ");
      ret.Replace("レ", "リ");
      ret.Replace("ロ", "ド");

      return ret.ToString();
    }

    private static string JoinFragments(IEnumerable<TextFragment> fragments)
    {
      var ret = new StringBuilder();

      foreach (var fragment in fragments) {
        ret.Append(fragment.ConvertedText);
      }

      return ret.ToString();
    }

    private Tagger tagger;
    private SortedList<string, string> wordDictionary;
  }
}

