// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace Smdn.Text.Ondulish;

[TestFixture]
public class TranslatorTests {
  [Test]
  public void Ctor()
  {
    using var t = new Translator();

    Assert.IsNotNull(t);
    Assert.DoesNotThrow(t.Dispose);
  }

  [Test]
  public void Ctor_ArgumentNull()
  {
    Assert.Throws<ArgumentNullException>(() => new Translator(tagger: null!, shouldDisposeTagger: true));
    Assert.Throws<ArgumentNullException>(() => new Translator(tagger: null!, shouldDisposeTagger: false));
  }

  [Test]
  public void Dispose()
  {
    using var t = new Translator();

    Assert.DoesNotThrow(() => t.Translate("input", convertKatakanaToNarrow: true));
    Assert.DoesNotThrow(() => t.Translate("input", convertKatakanaToNarrow: false));
    Assert.DoesNotThrow(() => t.Translate("input", output: TextWriter.Null, convertKatakanaToNarrow: true));
    Assert.DoesNotThrow(() => t.Translate("input", output: TextWriter.Null, convertKatakanaToNarrow: false));

    Assert.DoesNotThrow(() => t.Dispose(), "dispose #1");
    Assert.DoesNotThrow(() => t.Dispose(), "dispose #2");

    Assert.Throws<ObjectDisposedException>(() => t.Translate("input", convertKatakanaToNarrow: true));
    Assert.Throws<ObjectDisposedException>(() => t.Translate("input", convertKatakanaToNarrow: false));
    Assert.Throws<ObjectDisposedException>(() => t.Translate("input", output: TextWriter.Null, convertKatakanaToNarrow: true));
    Assert.Throws<ObjectDisposedException>(() => t.Translate("input", output: TextWriter.Null, convertKatakanaToNarrow: false));
  }

  [Test]
  public void Dispose_DisposeComposedTagger([Values(true, false)] bool shouldDisposeTagger)
  {
    using var tagger = Translator.CreateTaggerForBundledDictionary();
    using var t = new Translator(
      tagger: tagger,
      shouldDisposeTagger: shouldDisposeTagger
    );

    Assert.DoesNotThrow(t.Dispose);

    if (shouldDisposeTagger) {
      // cannot test: this test code will cause crash
      // Assert.Throws<Exception>(() => tagger.parseToString("mecab"));
    }
    else {
      Assert.DoesNotThrow(() => tagger.parseToString("mecab"));
    }
  }

  [TestCase("", "")]
  [TestCase("オンドゥル", "オンドゥル")]
  [TestCase("変身", "ヘシン")]
  [TestCase("橘さん", "ダディャーナザァーン")]
  [TestCase("本当に裏切ったんですか", "オンドゥルルラギッタンディスカー")]
  [TestCase("決着を", "ケッチャコ")]
  [TestCase("俺は貴様をぶっころす", "オレァクサムヲムッコロス")]
  [TestCase("俺は貴様をぶっ殺す", "オレァクサムヲムッコロス")]
  [TestCase("俺は！貴様を！", "オレァ！クサムヲ！")]
  [TestCase("俺は！ 貴様を！", "オレァ！クサムヲ！")]
  [TestCase("俺は！  貴様を！", "オレァ！クサムヲ！")]
  [TestCase("俺は！　貴様を！", "オレァ！　クサムヲ！")]
  [TestCase("俺は！貴様か！", "オレァ！クサムカァ！")]
  [TestCase("いえ！何も！", "ヴェ！マリモ！")]
  [TestCase("いえ!何も!", "ヴェ!マリモ!")]
  [TestCase("いえ、何も!", "ヴェ、マリモ!")]
  [TestCase("いえ何も。", "ヴェマリモ。")]
  [TestCase("俺の体はボロボロだ", "オデノカラダハボドボドダ")]
  [TestCase("あいうえお", "ア゛イルエオ")]
  [TestCase("#$%&'", "#$%&'")]
  [TestCase(@""","", ""，"",", @"""，""，""，""，")]
  [TestCase(@"変身😆😄", @"ヘシン😆😄")]
  [TestCase(@"オンドゥル😆😄", @"オンドゥル😆😄")]
  public void Translate(string input, string expected)
  {
    using var t = new Translator();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: false)
    );
  }

  [Test]
  public void Translate_SingleLine()
  {
    using var t = new Translator();

    Assert.AreEqual(
      "オンドゥルルラギッタンディスカー",
      t.Translate("本当に裏切ったんですか", convertKatakanaToNarrow: false)
    );
  }

  [Test]
  public void Translate_MultipleLines()
  {
    const string input = @"本当に裏切ったんですか

本当に裏切ったんですか";

    var expected = @"オンドゥルルラギッタンディスカー

オンドゥルルラギッタンディスカー".Replace("\r", string.Empty).Replace("\n", Environment.NewLine);

    using var t = new Translator();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: false)
    );
  }

  [TestCase("オンドゥル", "ｵﾝﾄﾞｩﾙ")]
  [TestCase("変身", "ﾍｼﾝ")]
  [TestCase("めかぶ", "ﾍﾞｶﾑ")]
  public void Translate_ConvertKatakanaToNarrowDefaultValue(string input, string expected)
  {
    using var t = new Translator();

    Assert.AreEqual(
      expected,
      t.Translate(input)
    );
  }

  [Test]
  public void Translate_InputNull([Values(true, false)] bool convertKatakanaToNarrow)
  {
    using var t = new Translator();

    Assert.Throws<ArgumentNullException>(
      () => t.Translate(input: null!, convertKatakanaToNarrow: convertKatakanaToNarrow)
    );
  }

  [TestCase("", "")]
  [TestCase("オンドゥル", "オンドゥル")]
  [TestCase("変身", "ヘシン")]
  public void Translate_ToTextWriter(string input, string expectedOutput)
  {
    using var t = new Translator();

    var sb = new StringBuilder();
    var writer = new StringWriter(sb);

    Assert.DoesNotThrow(() => t.Translate(
      input: input,
      output: writer,
      convertKatakanaToNarrow: false)
    );
    Assert.AreEqual(expectedOutput, sb.ToString());
  }

  [TestCase("", "")]
  [TestCase("オンドゥル\n\n変身\n", "オンドゥル\n\nヘシン")] // final newline should be trimmed
  public void Translate_ToTextWriter_FromTextReader(string input, string expectedOutput)
  {
    using var t = new Translator();

    var sb = new StringBuilder();
    var writer = new StringWriter(sb);

    Assert.DoesNotThrow(
      () => t.Translate(
        input: new StringReader(input),
        output: writer,
        convertKatakanaToNarrow: false
      )
    );

    Assert.AreEqual(expectedOutput, sb.ToString());
  }

  [Test]
  public void Translate_ToTextWriter_TextReaderInputNull([Values(true, false)] bool convertKatakanaToNarrow)
  {
    using var t = new Translator();

    Assert.Throws<ArgumentNullException>(
      () => t.Translate(
        input: (TextReader)null!,
        output: TextWriter.Null,
        convertKatakanaToNarrow: convertKatakanaToNarrow
      )
    );
  }

  [Test]
  public void Translate_ToTextWriter_StringInputNull([Values(true, false)] bool convertKatakanaToNarrow)
  {
    using var t = new Translator();

    Assert.Throws<ArgumentNullException>(
      () => t.Translate(
        input: (string)null!,
        output: TextWriter.Null,
        convertKatakanaToNarrow: convertKatakanaToNarrow
      )
    );
  }

  [Test]
  public void Translate_ToTextWriter_OutputNull([Values(true, false)] bool convertKatakanaToNarrow)
  {
    using var t = new Translator();

    Assert.Throws<ArgumentNullException>(
      () => t.Translate(
        input: string.Empty,
        output: null!,
        convertKatakanaToNarrow: convertKatakanaToNarrow
      )
    );
  }

  [TestCase("オンドゥル", "ｵﾝﾄﾞｩﾙ")]
  [TestCase("変身", "ﾍｼﾝ")]
  [TestCase("橘さん", "ﾀﾞﾃﾞｨｬｰﾅｻﾞｧｰﾝ")]
  [TestCase("俺は貴様をぶっころす", "ｵﾚｧｸｻﾑｦﾑｯｺﾛｽ")]
  [TestCase(@"変身😆😄", @"ﾍｼﾝ😆😄")]
  [TestCase("あいするな", "ｱﾞｲﾄﾞｩﾙﾀﾞ")]
  public void Translate_ToNarrowKatakana(string input, string expected)
  {
    using var t = new Translator();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: true)
    );
  }

  [TestCase("GOT TO BE STRONG", "辛味噌")]
  [TestCase("Got To Be Strong", "辛味噌")]
  [TestCase("got to be strong", "辛味噌")]
  [TestCase("got to be strong!", "辛味噌!")]
  [TestCase("7020km", "南西20キロ")]
  [TestCase("7020キロ", "南西20キロ")]
  public void Translate_FromNonJapanesePhrase(string input, string expected)
  {
    using var t = new Translator();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: false)
    );
  }

  [TestCase("相手は俺だ", "アンギョン和田")] // be translated terms with kanji chars
  [TestCase("貴様、相手は俺だ", "クサム、アンギョン和田")]
  public void Translate_SpecialCase(string input, string expected)
  {
    using var t = new Translator();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: false)
    );
  }

  [TestCase("めかぶ", "ベカム")]
  [TestCase("かてる", "カデドゥ")]
  [TestCase("あいするな", "ア゛イドゥルダ")]
  [TestCase("あいする", "ア゛イドゥル")]
  [TestCase("あいすな", "ア゛イスダ")]
  [TestCase("あいす", "ア゛イズ")]
  [TestCase("あいでも", "ア゛イデロ")]
  [TestCase("あいで", "ア゛イデ")]
  [TestCase("あいに", "ア゛イディ")]
  [TestCase("あいる", "ア゛イドゥ")]
  [TestCase("ぼーる", "ポッドゥ")]
  [TestCase("ばーる", "バードゥ")]
  [TestCase("おんどぅる", "オンドゥル")]
  [TestCase("おんどぅ", "オンドゥ")]
  public void Translate_Phoneme(string input, string expected)
  {
    using var t = new Translator();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: false)
    );
  }

  [Test]
  public void Translate_DictionaryTerm_Words()
  {
    using var t = new Translator();

    foreach (var pair in t.WordDictionary) {
      const string inputPrepend = "あ";
      const string outputPrepend = "ア゛";
      const string inputAppend = "う";
      const string outputAppend = "ル";

      Assert.AreEqual(
        outputPrepend + pair.Value + outputAppend,
        t.Translate(inputPrepend + pair.Key + inputAppend, convertKatakanaToNarrow: false)
      );
    }
  }

  [Test]
  public void Translate_DictionaryTerm_Phrases()
  {
    using var t = new Translator();

    foreach (var pair in t.PhraseDictionary) {
      const string inputPrepend = "あ";
      const string outputPrepend = "ア゛";
      const string inputAppend = "う";
      const string outputAppend = "ル";

      Assert.AreEqual(
        outputPrepend + pair.Value + outputAppend,
        t.Translate(inputPrepend + pair.Key + inputAppend, convertKatakanaToNarrow: false)
      );
    }
  }
}
