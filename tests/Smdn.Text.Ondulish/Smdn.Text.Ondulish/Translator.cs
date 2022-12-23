// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Reflection;

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
  public void TestTranslate(string input, string expected)
  {
    using (var t = Create()) {
      Assert.AreEqual(
        expected,
        t.Translate(input, convertKatakanaToNarrow: false).TrimEnd()
      );
    }
  }

  [TestCase("オンドゥル", "ｵﾝﾄﾞｩﾙ")]
  [TestCase("変身", "ﾍｼﾝ")]
  [TestCase("橘さん", "ﾀﾞﾃﾞｨｬｰﾅｻﾞｧｰﾝ")]
  [TestCase("俺は貴様をぶっころす", "ｵﾚｧｸｻﾑｦﾑｯｺﾛｽ")]
  [TestCase(@"変身😆😄", @"ﾍｼﾝ😆😄")]
  [TestCase("あいするな", "ｱﾞｲﾄﾞｩﾙﾀﾞ")]
  public void TestTranslateToNarrowKatakana(string input, string expected)
  {
    using (var t = Create()) {
      Assert.AreEqual(
        expected,
        t.Translate(input, convertKatakanaToNarrow: true).TrimEnd()
      );
    }
  }

  [TestCase("相手は俺だ", "アンギョン和田")] // be translated terms with kanji chars
  [TestCase("貴様、相手は俺だ", "チサマ、アンギョン和田")]
  public void TestTranslate_SpecialCase(string input, string expected)
  {
    using (var t = Create()) {
      Assert.AreEqual(
        expected,
        t.Translate(input, convertKatakanaToNarrow: false).TrimEnd()
      );
    }
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
  public void TestTranslatePhoneme(string input, string expected)
  {
    using (var t = Create()) {
      Assert.AreEqual(
        expected,
        t.Translate(input, convertKatakanaToNarrow: false).TrimEnd()
      );
    }
  }

  [Test]
  public void TestTranslateDictionaryTerm_Words()
  {
    using (var t = Create()) {
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
  }

  [Test]
  public void TestTranslateDictionaryTerm_Phrases()
  {
    using (var t = Create()) {
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
}
