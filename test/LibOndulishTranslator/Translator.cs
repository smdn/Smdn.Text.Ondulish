using System;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace Smdn.Applications.OndulishTranslator {
  [TestFixture]
  public class TranslatorTests {
    private static Translator Create()
    {
      var codeBaseDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
      var taggerArg = "-r " + Path.Combine(codeBaseDir, "mecabrc");

      return new Translator(taggerArg, dictionaryDirectory: codeBaseDir);
    }

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
    public void TestTranslate(string input, string expected)
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
}