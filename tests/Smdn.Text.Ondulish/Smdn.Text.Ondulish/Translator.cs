// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace Smdn.Text.Ondulish;

[TestFixture]
public class TranslatorTests {
  private static Translator Create()
  {
    var codeBaseDir = TestContext.CurrentContext.TestDirectory;
    var taggerArg = "-r " + Path.Combine(codeBaseDir, "mecabrc");

    return new Translator(taggerArg, dictionaryDirectory: codeBaseDir);
  }

  [Test]
  public void Ctor()
  {
    using var t = Create();

    Assert.IsNotNull(t);
    Assert.DoesNotThrow(t.Dispose);
  }

  [Test]
  public void Ctor_ArgumentNull()
  {
    Assert.Throws<ArgumentNullException>(() => new Translator(taggerArgs: null!, dictionaryDirectory: null!));
    Assert.Throws<ArgumentNullException>(() => new Translator(taggerArgs: null!, dictionaryDirectory: string.Empty));
    Assert.Throws<ArgumentNullException>(() => new Translator(taggerArgs: string.Empty, dictionaryDirectory: null!));
  }

  [Test]
  public void Dispose()
  {
    using var t = Create();

    Assert.DoesNotThrow(() => t.Translate("input", convertKatakanaToNarrow: true));
    Assert.DoesNotThrow(() => t.Translate("input", convertKatakanaToNarrow: false));
    Assert.DoesNotThrow(() => t.Translate("input", convertKatakanaToNarrow: true, output: TextWriter.Null));
    Assert.DoesNotThrow(() => t.Translate("input", convertKatakanaToNarrow: false, output: TextWriter.Null));

    Assert.DoesNotThrow(() => t.Dispose(), "dispose #1");
    Assert.DoesNotThrow(() => t.Dispose(), "dispose #2");

    Assert.Throws<ObjectDisposedException>(() => t.Translate("input", convertKatakanaToNarrow: true));
    Assert.Throws<ObjectDisposedException>(() => t.Translate("input", convertKatakanaToNarrow: false));
    Assert.Throws<ObjectDisposedException>(() => t.Translate("input", convertKatakanaToNarrow: true, output: TextWriter.Null));
    Assert.Throws<ObjectDisposedException>(() => t.Translate("input", convertKatakanaToNarrow: false, output: TextWriter.Null));
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
  [TestCase("俺は！貴様か！", "オレァ！クサムカァ！")]
  [TestCase("俺の体はボロボロだ", "オデノカラダハボドボドダ")]
  [TestCase("あいうえお", "ア゛イルエオ")]
  [TestCase("#$%&'", "#$%&'")]
  [TestCase(@""","", ""，"",", @"""，""，""，""，")]
  [TestCase(@"変身😆😄", @"ヘシン😆😄")]
  [TestCase(@"オンドゥル😆😄", @"オンドゥル😆😄")]
  public void Translate(string input, string expected)
  {
    using var t = Create();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: false).TrimEnd()
    );
  }

  [Test]
  public void Translate_InputNull([Values(true, false)] bool convertKatakanaToNarrow)
  {
    using var t = Create();

    Assert.Throws<ArgumentNullException>(() => t.Translate(input: null!, convertKatakanaToNarrow: convertKatakanaToNarrow));
  }

  [TestCase("", "")]
  [TestCase("オンドゥル", "オンドゥル")]
  [TestCase("変身", "ヘシン")]
  public void Translate_ToTextWriter(string input, string expectedOutput)
  {
    using var t = Create();

    var sb = new StringBuilder();
    var writer = new StringWriter(sb);

    Assert.DoesNotThrow(() => t.Translate(input: input, convertKatakanaToNarrow: false, output: writer));
    Assert.AreEqual(expectedOutput, sb.ToString().TrimEnd());
  }

  [Test]
  public void Translate_ToTextWriter_InputNull([Values(true, false)] bool convertKatakanaToNarrow)
  {
    using var t = Create();

    Assert.Throws<ArgumentNullException>(() => t.Translate(input: null!, convertKatakanaToNarrow: convertKatakanaToNarrow, output: TextWriter.Null));
  }

  [Test]
  public void Translate_ToTextWriter_OutputNull([Values(true, false)] bool convertKatakanaToNarrow)
  {
    using var t = Create();

    Assert.Throws<ArgumentNullException>(() => t.Translate(input: string.Empty, convertKatakanaToNarrow: convertKatakanaToNarrow, output: null!));
  }

  [TestCase("オンドゥル", "ｵﾝﾄﾞｩﾙ")]
  [TestCase("変身", "ﾍｼﾝ")]
  [TestCase("橘さん", "ﾀﾞﾃﾞｨｬｰﾅｻﾞｧｰﾝ")]
  [TestCase("俺は貴様をぶっころす", "ｵﾚｧｸｻﾑｦﾑｯｺﾛｽ")]
  [TestCase(@"変身😆😄", @"ﾍｼﾝ😆😄")]
  [TestCase("あいするな", "ｱﾞｲﾄﾞｩﾙﾀﾞ")]
  public void Translate_ToNarrowKatakana(string input, string expected)
  {
    using var t = Create();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: true).TrimEnd()
    );
  }

  [TestCase("相手は俺だ", "アンギョン和田")] // be translated terms with kanji chars
  [TestCase("貴様、相手は俺だ", "チサマ、アンギョン和田")]
  public void Translate_SpecialCase(string input, string expected)
  {
    using var t = Create();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: false).TrimEnd()
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
    using var t = Create();

    Assert.AreEqual(
      expected,
      t.Translate(input, convertKatakanaToNarrow: false).TrimEnd()
    );
  }

  [Test]
  public void Translate_DictionaryTerm_Words()
  {
    using var t = Create();

    foreach (var pair in t.WordDictionary) {
      const string inputPrepend = "あ";
      const string outputPrepend = "ア゛";
      const string inputAppend = "う";
      const string outputAppend = "ル";

      Assert.AreEqual(
        outputPrepend + pair.Value + outputAppend,
        t.Translate(inputPrepend + pair.Key + inputAppend, convertKatakanaToNarrow: false).TrimEnd()
      );
    }
  }

  [Test]
  public void Translate_DictionaryTerm_Phrases()
  {
    using var t = Create();

    foreach (var pair in t.PhraseDictionary) {
      const string inputPrepend = "あ";
      const string outputPrepend = "ア゛";
      const string inputAppend = "う";
      const string outputAppend = "ル";

      Assert.AreEqual(
        outputPrepend + pair.Value + outputAppend,
        t.Translate(inputPrepend + pair.Key + inputAppend, convertKatakanaToNarrow: false).TrimEnd()
      );
    }
  }
}
