// SPDX-FileCopyrightText: 2012 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using MeCab;

using MeCabConsts = MeCab.MeCab;

namespace Smdn.Text.Ondulish;

public partial class Translator : IDisposable {
  private const string MeCabDeploymentDirectory = "mecab";

  private Tagger? tagger;
  private readonly bool shouldDisposeTagger;

  private void ThrowIfDisposed()
  {
    if (tagger is null)
      throw new ObjectDisposedException(GetType().FullName);
  }

  public static Tagger CreateTaggerForBundledDictionary()
  {
    var assemblyDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    var mecabDeploymentDirectoryPath = string.IsNullOrEmpty(assemblyDirectory)
      ? MeCabDeploymentDirectory // fallback: use relative path from current directory
      : System.IO.Path.Join(assemblyDirectory, MeCabDeploymentDirectory);

    var pathToMeCabResourceFile = System.IO.Path.Join(mecabDeploymentDirectoryPath, "null.mecabrc");
    var pathToMeCabDictionaryDirectory = System.IO.Path.Join(mecabDeploymentDirectoryPath, "dic", "ipadic");

    var taggerArgs = $"-r {pathToMeCabResourceFile} -d {pathToMeCabDictionaryDirectory}";

    return new Tagger(taggerArgs);
  }

  public Translator()
    : this(
      tagger: CreateTaggerForBundledDictionary(),
      shouldDisposeTagger: true
    )
  {
  }

  public Translator(
    Tagger tagger,
    bool shouldDisposeTagger
  )
  {
    if (tagger is null)
      throw new ArgumentNullException(nameof(tagger));

    this.tagger = tagger;
    this.shouldDisposeTagger = shouldDisposeTagger;

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

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (shouldDisposeTagger && tagger is not null)
      tagger.Dispose();

    tagger = null;
  }

  public string Translate(
    string input,
    bool convertKatakanaToNarrow = true
  )
  {
    if (input is null)
      throw new ArgumentNullException(nameof(input));

    ThrowIfDisposed();

    if (input.Length == 0)
      return string.Empty;

    var sb = new StringBuilder(input.Length * 2);

    Translate(
      input: new StringReader(input),
      output: new StringWriter(sb),
      convertKatakanaToNarrow: convertKatakanaToNarrow
    );

    return sb.ToString();
  }

  public void Translate(
    string input,
    TextWriter output,
    bool convertKatakanaToNarrow
  )
    => Translate(
      input: new StringReader(input ?? throw new ArgumentNullException(nameof(input))),
      output: output ?? throw new ArgumentNullException(nameof(output)),
      convertKatakanaToNarrow: convertKatakanaToNarrow
    );

  public void Translate(
    TextReader input,
    TextWriter output,
    bool convertKatakanaToNarrow
  )
  {
    if (input is null)
      throw new ArgumentNullException(nameof(input));
    if (output is null)
      throw new ArgumentNullException(nameof(output));

    ThrowIfDisposed();

    var firstLine = true;

    for (var line = input.ReadLine(); line is not null; line = input.ReadLine()) {
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
}
