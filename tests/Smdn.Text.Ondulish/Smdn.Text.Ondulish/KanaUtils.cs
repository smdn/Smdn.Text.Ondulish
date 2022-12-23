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
  public void TestConvertWideHiraganaToKatakana(string input, string expected)
    => Assert.AreEqual(
      expected,
      KanaUtils.ConvertWideHiraganaToKatakana(input)
    );

  [Test]
  public void TestConvertWideHiraganaToKatakana_ArgumentNull()
    => Assert.Throws<ArgumentNullException>(() => KanaUtils.ConvertWideHiraganaToKatakana(input: null));

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
  public void TestConvertWideKatakanaToHiragana(string input, string expected)
    => Assert.AreEqual(
      expected,
      KanaUtils.ConvertWideKatakanaToHiragana(input)
    );

  [Test]
  public void TestConvertWideKatakanaToHiragana_ArgumentNull()
    => Assert.Throws<ArgumentNullException>(() => KanaUtils.ConvertWideKatakanaToHiragana(input: null));

  [TestCase("", "")]
  public void TestConvertWideKatakanaToNarrowKatakana(string input, string expected)
    => Assert.AreEqual(
      expected,
      KanaUtils.ConvertWideKatakanaToNarrowKatakana(input)
    );

  [Test]
  public void TestConvertWideKatakanaToNarrowKatakana_ArgumentNull()
    => Assert.Throws<ArgumentNullException>(() => KanaUtils.ConvertWideKatakanaToNarrowKatakana(input: null));
}
