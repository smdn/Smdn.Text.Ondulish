// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using NUnit.Framework;

namespace Smdn.Text.Ondulish;

[TestFixture]
public class KanaUtilsTests {
  [TestCase("", "")]
  [TestCase("ABC", "ABC")]
  [TestCase("abc", "abc")]
  [TestCase("ＡＢＣ", "ＡＢＣ")]
  [TestCase("ａｂｃ", "ａｂｃ")]
  [TestCase("$%&", "$%&")]
  [TestCase("あいうえお", "アイウエオ")]
  [TestCase("アイウエオ", "アイウエオ")]
  [TestCase("わをん", "ワヲン")]
  [TestCase("日本語", "日本語")]
  [TestCase("\u3040\u3041\u3096\u3097", "\u3040\u30a1\u30f6\u3097")]
  public void ConvertWideHiraganaToKatakana(string input, string expected)
    => Assert.AreEqual(
      expected,
      KanaUtils.ConvertWideHiraganaToKatakana(input)
    );

  [Test]
  public void ConvertWideHiraganaToKatakana_ArgumentNull()
    => Assert.Throws<ArgumentNullException>(() => KanaUtils.ConvertWideHiraganaToKatakana(input: null!));

  [TestCase("", "")]
  [TestCase("ABC", "ABC")]
  [TestCase("abc", "abc")]
  [TestCase("ＡＢＣ", "ＡＢＣ")]
  [TestCase("ａｂｃ", "ａｂｃ")]
  [TestCase("$%&", "$%&")]
  [TestCase("アイウエオ", "あいうえお")]
  [TestCase("あいうえお", "あいうえお")]
  [TestCase("ワヲン", "わをん")]
  [TestCase("日本語", "日本語")]
  [TestCase("\u3040\u30a1\u30f6\u3097", "\u3040\u3041\u3096\u3097")]
  public void ConvertWideKatakanaToHiragana(string input, string expected)
    => Assert.AreEqual(
      expected,
      KanaUtils.ConvertWideKatakanaToHiragana(input)
    );

  [Test]
  public void ConvertWideKatakanaToHiragana_ArgumentNull()
    => Assert.Throws<ArgumentNullException>(() => KanaUtils.ConvertWideKatakanaToHiragana(input: null!));

  private static System.Collections.IEnumerable YieldTestCases_ConvertWideKatakanaToNarrowKatakana()
  {
    yield return new object[] { string.Empty, string.Empty };

    // 30A1 - 30AF
    yield return new object[] { "ァ", "ｧ" };
    yield return new object[] { "ア", "ｱ" };
    yield return new object[] { "ィ", "ｨ" };
    yield return new object[] { "イ", "ｲ" };
    yield return new object[] { "ゥ", "ｩ" };
    yield return new object[] { "ウ", "ｳ" };
    yield return new object[] { "ェ", "ｪ" };
    yield return new object[] { "エ", "ｴ" };
    yield return new object[] { "ォ", "ｫ" };
    yield return new object[] { "オ", "ｵ" };
    yield return new object[] { "カ", "ｶ" };
    yield return new object[] { "ガ", "ｶﾞ" };
    yield return new object[] { "キ", "ｷ" };
    yield return new object[] { "ギ", "ｷﾞ" };
    yield return new object[] { "ク", "ｸ" };
    // 30B0 - 30BF
    yield return new object[] { "グ", "ｸﾞ" };
    yield return new object[] { "ケ", "ｹ" };
    yield return new object[] { "ゲ", "ｹﾞ" };
    yield return new object[] { "コ", "ｺ" };
    yield return new object[] { "ゴ", "ｺﾞ" };
    yield return new object[] { "サ", "ｻ" };
    yield return new object[] { "ザ", "ｻﾞ" };
    yield return new object[] { "シ", "ｼ" };
    yield return new object[] { "ジ", "ｼﾞ" };
    yield return new object[] { "ス", "ｽ" };
    yield return new object[] { "ズ", "ｽﾞ" };
    yield return new object[] { "セ", "ｾ" };
    yield return new object[] { "ゼ", "ｾﾞ" };
    yield return new object[] { "ソ", "ｿ" };
    yield return new object[] { "ゾ", "ｿﾞ" };
    yield return new object[] { "タ", "ﾀ" };
    // 30C0 - 30CF
    yield return new object[] { "ダ", "ﾀﾞ" };
    yield return new object[] { "チ", "ﾁ" };
    yield return new object[] { "ヂ", "ﾁﾞ" };
    yield return new object[] { "ッ", "ｯ" };
    yield return new object[] { "ツ", "ﾂ" };
    yield return new object[] { "ヅ", "ﾂﾞ" };
    yield return new object[] { "テ", "ﾃ" };
    yield return new object[] { "デ", "ﾃﾞ" };
    yield return new object[] { "ト", "ﾄ" };
    yield return new object[] { "ド", "ﾄﾞ" };
    yield return new object[] { "ナ", "ﾅ" };
    yield return new object[] { "ニ", "ﾆ" };
    yield return new object[] { "ヌ", "ﾇ" };
    yield return new object[] { "ネ", "ﾈ" };
    yield return new object[] { "ノ", "ﾉ" };
    yield return new object[] { "ハ", "ﾊ" };
    // 30D0 - 30DF
    yield return new object[] { "バ", "ﾊﾞ" };
    yield return new object[] { "パ", "ﾊﾟ" };
    yield return new object[] { "ヒ", "ﾋ" };
    yield return new object[] { "ビ", "ﾋﾞ" };
    yield return new object[] { "ピ", "ﾋﾟ" };
    yield return new object[] { "フ", "ﾌ" };
    yield return new object[] { "ブ", "ﾌﾞ" };
    yield return new object[] { "プ", "ﾌﾟ" };
    yield return new object[] { "ヘ", "ﾍ" };
    yield return new object[] { "ベ", "ﾍﾞ" };
    yield return new object[] { "ペ", "ﾍﾟ" };
    yield return new object[] { "ホ", "ﾎ" };
    yield return new object[] { "ボ", "ﾎﾞ" };
    yield return new object[] { "ポ", "ﾎﾟ" };
    yield return new object[] { "マ", "ﾏ" };
    yield return new object[] { "ミ", "ﾐ" };
    // 30E0 - 30EF
    yield return new object[] { "ム", "ﾑ" };
    yield return new object[] { "メ", "ﾒ" };
    yield return new object[] { "モ", "ﾓ" };
    yield return new object[] { "ャ", "ｬ" };
    yield return new object[] { "ヤ", "ﾔ" };
    yield return new object[] { "ュ", "ｭ" };
    yield return new object[] { "ユ", "ﾕ" };
    yield return new object[] { "ョ", "ｮ" };
    yield return new object[] { "ヨ", "ﾖ" };
    yield return new object[] { "ラ", "ﾗ" };
    yield return new object[] { "リ", "ﾘ" };
    yield return new object[] { "ル", "ﾙ" };
    yield return new object[] { "レ", "ﾚ" };
    yield return new object[] { "ロ", "ﾛ" };
    yield return new object[] { "ヮ", "ヮ" };
    yield return new object[] { "ワ", "ﾜ" };
    // 30F0 - 30FA
    yield return new object[] { "ヰ", "ヰ" };
    yield return new object[] { "ヱ", "ヱ" };
    yield return new object[] { "ヲ", "ｦ" };
    yield return new object[] { "ン", "ﾝ" };
    yield return new object[] { "ヴ", "ｳﾞ" };
    yield return new object[] { "ヵ", "ヵ" };
    yield return new object[] { "ヶ", "ヶ" };
    yield return new object[] { "ヷ", "ﾜﾞ" };
    yield return new object[] { "ヸ", "ヸ" };
    yield return new object[] { "ヹ", "ヹ" };
    yield return new object[] { "ヺ", "ｦﾞ" };

    yield return new object[] { "ABC", "ABC" };
    yield return new object[] { "abc", "abc" };
    yield return new object[] { "ＡＢＣ", "ＡＢＣ" };
    yield return new object[] { "ａｂｃ", "ａｂｃ" };
    yield return new object[] { "$%&", "$%&" };
    yield return new object[] { "あいうえお", "あいうえお" };
    yield return new object[] { "アイウエオ", "ｱｲｳｴｵ" };
    yield return new object[] { "ヷヸヹヺ", "ﾜﾞヸヹｦﾞ" };
  }

  [TestCaseSource(nameof(YieldTestCases_ConvertWideKatakanaToNarrowKatakana))]
  public void ConvertWideKatakanaToNarrowKatakana(string input, string expected)
    => Assert.AreEqual(
      expected,
      KanaUtils.ConvertWideKatakanaToNarrowKatakana(input)
    );

  [Test]
  public void ConvertWideKatakanaToNarrowKatakana_ArgumentNull()
    => Assert.Throws<ArgumentNullException>(() => KanaUtils.ConvertWideKatakanaToNarrowKatakana(input: null!));
}
