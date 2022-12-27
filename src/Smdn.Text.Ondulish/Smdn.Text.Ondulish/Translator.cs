// SPDX-FileCopyrightText: 2012 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MeCab;

using Smdn.Formats.Csv;

using MeCabConsts = MeCab.MeCab;

namespace Smdn.Text.Ondulish;

public class Translator : IDisposable {
  private const string MeCabDeploymentDirectory = "mecab";

  public IReadOnlyDictionary<string, string> PhraseDictionary { get; }
  public IReadOnlyDictionary<string, string> WordDictionary { get; }

  private Tagger? tagger;

  private void ThrowIfDisposed()
  {
    if (tagger is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  private static string GetMeCabDefaultTaggerArgs(string baseDirectory)
  {
    var mecabDeploymentDirectoryPath = string.IsNullOrEmpty(baseDirectory)
      ? MeCabDeploymentDirectory // fallback: use relative path from current directory
      : System.IO.Path.Join(baseDirectory, MeCabDeploymentDirectory);

    var pathToMeCabResourceFile = System.IO.Path.Join(mecabDeploymentDirectoryPath, "null.mecabrc");
    var pathToMeCabDictionaryDirectory = System.IO.Path.Join(mecabDeploymentDirectoryPath, "dic", "ipadic");

    return $"-r {pathToMeCabResourceFile} -d {pathToMeCabDictionaryDirectory}";
  }

  private static string? GetProcessDirectory()
#if SYSTEM_ENVIRONMENT_PROCESSPATH
    => System.IO.Path.GetDirectoryName(Environment.ProcessPath);
#else
    => System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#endif

  public Translator()
    : this(processDirectory: GetProcessDirectory() ?? string.Empty /* fallback: use relative path from current directory */)
  {
  }

  public Translator(string processDirectory)
    : this(
      taggerArgs: GetMeCabDefaultTaggerArgs(processDirectory ?? throw new ArgumentNullException(nameof(processDirectory))),
      dictionaryDirectory: processDirectory
    )
  {
  }

  public Translator(string taggerArgs, string dictionaryDirectory)
  {
    if (taggerArgs is null)
      throw new ArgumentNullException(nameof(taggerArgs));
    if (dictionaryDirectory is null)
      throw new ArgumentNullException(nameof(dictionaryDirectory));

    tagger = new Tagger(taggerArgs);

    // load Ondulish dictionaries from assembly Smdn.Text.Ondulish.Dictionaries
    try {
      using var stream = OndulishDictionaries.OpenPhraseDictionaryStream();

      PhraseDictionary = LoadDictionary(stream);
    }
    catch {
      // ignore exceptions
      PhraseDictionary = CreateEmptyDictionary();
    }

    try {
      using var stream = OndulishDictionaries.OpenWordDictionaryStream();

      WordDictionary = LoadDictionary(stream);
    }
    catch {
      // ignore exceptions
      WordDictionary = CreateEmptyDictionary();
    }

    static IReadOnlyDictionary<string, string> CreateEmptyDictionary()
      => Enumerable.Empty<(string Key, string Value)>().ToDictionary(static pair => pair.Key, static pair => pair.Value);
  }

  private static readonly char[] dictionaryPunctuationChars = new[] { '！', '？', '!', '?', '、', '。' };

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

      var key = entries[1].Trim().RemoveChars(dictionaryPunctuationChars);

      dictionary[KanaUtils.ConvertWideHiraganaToKatakana(key)] = entries[2].Trim();
    }

#if false
    foreach (var e in dictionary) {
      Console.WriteLine("{0} => {1}", e.Key, e.Value);
    }
#endif

    return dictionary;
  }

  private class WordDictionaryComparer : IComparer<string> {
    public int Compare(string? x, string? y)
    {
      x ??= string.Empty;
      y ??= string.Empty;

      return x.Length == y.Length
        ? StringComparer.Ordinal.Compare(x, y)
        : y.Length - x.Length;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    tagger?.Dispose();
    tagger = null;
  }

  public string Translate(string input, bool convertKatakanaToNarrow = true)
  {
    if (input is null)
      throw new ArgumentNullException(nameof(input));

    ThrowIfDisposed();

    if (input.Length == 0)
      return string.Empty;

    var sb = new StringBuilder(input.Length * 2);
    var sw = new StringWriter(sb);

    Translate(input, convertKatakanaToNarrow, sw);

    return sb.ToString();
  }

  public void Translate(string input, bool convertKatakanaToNarrow, TextWriter output)
  {
    if (input is null)
      throw new ArgumentNullException(nameof(input));
    if (output is null)
      throw new ArgumentNullException(nameof(output));

    ThrowIfDisposed();

    if (input.Length == 0)
      return;

    var reader = new StringReader(input);
    var firstLine = true;

    for (var line = reader.ReadLine(); line is not null; line = reader.ReadLine()) {
      if (firstLine)
        firstLine = false;
      else
        output.WriteLine();

      if (string.IsNullOrWhiteSpace(line)) {
        output.Write(line);
        continue;
      }

      var fragments =
        ConvertWithDictionary(
          ConvertToKatakana(line),
          PhraseDictionary
        )
        .SelectMany(f =>
          f.ConvertedText is null
            ? ConvertWithDictionary(f.SourceText, WordDictionary)
            : Enumerable.Repeat(f, 1)
        )
        .SelectMany(f =>
          f.ConvertedText is null
            ? ConvertWithDictionary(f.SourceText, phonemeDictionary)
            : Enumerable.Repeat(f, 1)
        )
        .Select(static f =>
          new TextFragment(
            f.SourceText,
            f.ConvertedText ?? KanaUtils.ConvertWideHiraganaToKatakana(f.SourceText) // redundant?
          )
        );

      if (convertKatakanaToNarrow) {
        fragments = fragments.Select(static f =>
          new TextFragment(
            f.SourceText,
            f.ConvertedText is null
              ? null
              : KanaUtils.ConvertWideKatakanaToNarrowKatakana(f.ConvertedText)
          )
        );
      }

      foreach (var fragment in fragments) {
        output.Write(fragment.ConvertedText);
      }
    }

    output.Flush();
  }

  private static readonly char[] featureSplitter = new[] { ',' };

  private string ConvertToKatakana(string input)
  {
    input = input.Replace(",", "，"); // XXX: feature splitter

    var ret = new StringBuilder(input.Length * 2);

    for (var node = tagger!.parseToNode(input); node != null; node = node.next) {
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

  private readonly struct TextFragment {
    public readonly string SourceText;
    public readonly string? ConvertedText;

    public TextFragment(string sourceText, string? convertedText)
    {
      SourceText = sourceText;
      ConvertedText = convertedText;
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
}
