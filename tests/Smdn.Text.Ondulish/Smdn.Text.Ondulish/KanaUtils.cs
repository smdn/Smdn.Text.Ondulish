using System;

using NUnit.Framework;

namespace Smdn.Applications.OndulishTranslator {
  [TestFixture]
  public class KanaUtilsTests {
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
    {
      Assert.AreEqual(
        expected,
        KanaUtils.ConvertWideHiraganaToKatakana(input)
      );
    }

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
    {
      Assert.AreEqual(
        expected,
        KanaUtils.ConvertWideKatakanaToHiragana(input)
      );
    }
  }
}