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

    Assert.That(t, Is.Not.Null);
    Assert.That(t.Dispose, Throws.Nothing);
  }

  [Test]
  public void Ctor_ArgumentNull()
  {
    Assert.That(() => new Translator(tagger: null!, shouldDisposeTagger: true), Throws.ArgumentNullException);
    Assert.That(() => new Translator(tagger: null!, shouldDisposeTagger: false), Throws.ArgumentNullException);
  }

  [Test]
  public void Dispose()
  {
    using var t = new Translator();

    Assert.That(
      () => t.Translate("input", convertKatakanaToNarrow: true),
      Throws.Nothing
    );
    Assert.That(
      () => t.Translate("input", convertKatakanaToNarrow: false),
      Throws.Nothing
    );
    Assert.That(
      () => t.Translate("input", output: TextWriter.Null, convertKatakanaToNarrow: true),
      Throws.Nothing
    );
    Assert.That(
      () => t.Translate("input", output: TextWriter.Null, convertKatakanaToNarrow: false),
      Throws.Nothing
    );

    Assert.That(t.Dispose, Throws.Nothing, "dispose #1");
    Assert.That(t.Dispose, Throws.Nothing, "dispose #2");

    Assert.That(() => t.Translate("input", convertKatakanaToNarrow: true), Throws.TypeOf<ObjectDisposedException>());
    Assert.That(() => t.Translate("input", convertKatakanaToNarrow: false), Throws.TypeOf<ObjectDisposedException>());
    Assert.That(() => t.Translate("input", output: TextWriter.Null, convertKatakanaToNarrow: true), Throws.TypeOf<ObjectDisposedException>());
    Assert.That(() => t.Translate("input", output: TextWriter.Null, convertKatakanaToNarrow: false), Throws.TypeOf<ObjectDisposedException>());
  }

  [Test]
  public void Dispose_DisposeComposedTagger([Values] bool shouldDisposeTagger)
  {
    using var tagger = Translator.CreateTaggerForBundledDictionary();
    using var t = new Translator(
      tagger: tagger,
      shouldDisposeTagger: shouldDisposeTagger
    );

    Assert.That(t.Dispose, Throws.Nothing);

    if (shouldDisposeTagger) {
      // cannot test: this test code will cause crash
      // Assert.That(() => tagger.parseToString("mecab"), Throws.Exception);
    }
    else {
      Assert.That(() => tagger.parseToString("mecab"), Throws.Nothing);
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

    Assert.That(
      t.Translate(input, convertKatakanaToNarrow: false),
      Is.EqualTo(expected)
    );
  }

  [Test]
  public void Translate_SingleLine()
  {
    using var t = new Translator();

    Assert.That(
      t.Translate("本当に裏切ったんですか", convertKatakanaToNarrow: false),
      Is.EqualTo("オンドゥルルラギッタンディスカー")
    );
  }

  [Test]
  public void Translate_MultipleLines()
  {
    const string Input = @"本当に裏切ったんですか

本当に裏切ったんですか";

    var expected = @"オンドゥルルラギッタンディスカー

オンドゥルルラギッタンディスカー".Replace("\r", string.Empty).Replace("\n", Environment.NewLine);

    using var t = new Translator();

    Assert.That(
      t.Translate(Input, convertKatakanaToNarrow: false),
      Is.EqualTo(expected)
    );
  }

  [TestCase("オンドゥル", "ｵﾝﾄﾞｩﾙ")]
  [TestCase("変身", "ﾍｼﾝ")]
  [TestCase("めかぶ", "ﾍﾞｶﾑ")]
  public void Translate_ConvertKatakanaToNarrowDefaultValue(string input, string expected)
  {
    using var t = new Translator();

    Assert.That(
      t.Translate(input),
      Is.EqualTo(expected)
    );
  }

  [Test]
  public void Translate_InputNull([Values] bool convertKatakanaToNarrow)
  {
    using var t = new Translator();

    Assert.That(
      () => t.Translate(input: null!, convertKatakanaToNarrow: convertKatakanaToNarrow),
      Throws.ArgumentNullException
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

    Assert.That(
      () => t.Translate(
        input: input,
        output: writer,
        convertKatakanaToNarrow: false
      ),
      Throws.Nothing
    );
    Assert.That(sb.ToString(), Is.EqualTo(expectedOutput));
  }

  [TestCase("", "")]
  [TestCase("オンドゥル\n\n変身\n", "オンドゥル\n\nヘシン")] // final newline should be trimmed
  public void Translate_ToTextWriter_FromTextReader(string input, string expectedOutput)
  {
    using var t = new Translator();

    var sb = new StringBuilder();
    var writer = new StringWriter(sb);

    Assert.That(
      () => t.Translate(
        input: new StringReader(input),
        output: writer,
        convertKatakanaToNarrow: false
      ),
      Throws.Nothing
    );

    Assert.That(sb.ToString(), Is.EqualTo(expectedOutput));
  }

  [Test]
  public void Translate_ToTextWriter_TextReaderInputNull([Values] bool convertKatakanaToNarrow)
  {
    using var t = new Translator();

    Assert.That(
      () => t.Translate(
        input: (TextReader)null!,
        output: TextWriter.Null,
        convertKatakanaToNarrow: convertKatakanaToNarrow
      ),
      Throws.ArgumentNullException
    );
  }

  [Test]
  public void Translate_ToTextWriter_StringInputNull([Values] bool convertKatakanaToNarrow)
  {
    using var t = new Translator();

    Assert.That(
      () => t.Translate(
        input: (string)null!,
        output: TextWriter.Null,
        convertKatakanaToNarrow: convertKatakanaToNarrow
      ),
      Throws.ArgumentNullException
    );
  }

  [Test]
  public void Translate_ToTextWriter_OutputNull([Values] bool convertKatakanaToNarrow)
  {
    using var t = new Translator();

    Assert.That(
      () => t.Translate(
        input: string.Empty,
        output: null!,
        convertKatakanaToNarrow: convertKatakanaToNarrow
      ),
      Throws.ArgumentNullException
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

    Assert.That(
      t.Translate(input, convertKatakanaToNarrow: true),
      Is.EqualTo(expected)
    );
  }

  [TestCase("GOT TO BE STRONG", "辛味噌")]
  [TestCase("Got To Be Strong", "辛味噌")]
  [TestCase("got to be strong", "辛味噌")]
  [TestCase("got to be strong!", "辛味噌!")]
  [TestCase("7020km", "南西20キロ")]
  [TestCase("7020キロ", "南西20キロ")]
  [TestCase("(OwO)", "(OwO)")] // will not be translated, to be returned as it is
  public void Translate_FromNonJapanesePhrase(string input, string expected)
  {
    using var t = new Translator();

    Assert.That(
      t.Translate(input, convertKatakanaToNarrow: false),
      Is.EqualTo(expected)
    );
  }

  [TestCase("相手は俺だ", "アンギョン和田")] // be translated terms with kanji chars
  [TestCase("貴様、相手は俺だ", "クサム、アンギョン和田")]
  public void Translate_SpecialCase(string input, string expected)
  {
    using var t = new Translator();

    Assert.That(
      t.Translate(input, convertKatakanaToNarrow: false),
      Is.EqualTo(expected)
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

    Assert.That(
      t.Translate(input, convertKatakanaToNarrow: false),
      Is.EqualTo(expected)
    );
  }

  [Test]
  public void Translate_DictionaryTerm_Words()
  {
    using var t = new Translator();

    foreach (var pair in t.WordDictionary) {
      const string InputPrepend = "あ";
      const string OutputPrepend = "ア゛";
      const string InputAppend = "う";
      const string OutputAppend = "ル";

      Assert.That(
        t.Translate(InputPrepend + pair.Key + InputAppend, convertKatakanaToNarrow: false),
        Is.EqualTo(OutputPrepend + pair.Value + OutputAppend)
      );
    }
  }

  [Test]
  public void Translate_DictionaryTerm_Phrases()
  {
    using var t = new Translator();

    foreach (var pair in t.PhraseDictionary) {
      const string InputPrepend = "あ";
      const string OutputPrepend = "ア゛";
      const string InputAppend = "う";
      const string OutputAppend = "ル";

      Assert.That(
        t.Translate(InputPrepend + pair.Key + InputAppend, convertKatakanaToNarrow: false),
        Is.EqualTo(OutputPrepend + pair.Value + OutputAppend)
      );
    }
  }
}
