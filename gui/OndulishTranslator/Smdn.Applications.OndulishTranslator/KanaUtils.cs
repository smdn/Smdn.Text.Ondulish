//
// Author:
//       smdn <smdn@smdn.jp>
//
// Copyright (c) 2012 smdn
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualBasic;

namespace Smdn.Applications.OndulishTranslator {
  public static class KanaUtils {
    /*
     * http://pgcafe.moo.jp/new/JAVA/index.php?dno=10&fno=2
     */
    private const char wideHiraganaStart = '\u3041';
    private const char wideHiraganaEnd = '\u3093';

    private const char wideKatakanaStart = '\u30a1';
    private const char wideKatakanaEnd = '\u30f3';

    private const int offsetFromHiraganaToKatakana = ((int)wideKatakanaStart - (int)wideHiraganaStart);

    private static readonly Dictionary<string, string> hoge = new Dictionary<string, string>() {
      // 2文字構成の濁点付き半角カナ
      // 必ずテーブルに先頭に置いてサーチ順を優先すること
      { "ｶﾞ", "ガ" }, { "ｷﾞ", "ギ" }, { "ｸﾞ", "グ" }, { "ｹﾞ", "ゲ" }, { "ｺﾞ", "ゴ" },
      { "ｻﾞ", "ザ" }, { "ｼﾞ", "ジ" }, { "ｽﾞ", "ズ" }, { "ｾﾞ", "ゼ" }, { "ｿﾞ", "ゾ" },
      { "ﾀﾞ", "ダ" }, { "ﾁﾞ", "ヂ" }, { "ﾂﾞ", "ヅ" }, { "ﾃﾞ", "デ" }, { "ﾄﾞ", "ド" },
      { "ﾊﾞ", "バ" }, { "ﾋﾞ", "ビ" }, { "ﾌﾞ", "ブ" }, { "ﾍﾞ", "ベ" }, { "ﾎﾞ", "ボ" },
      { "ﾊﾟ", "パ" }, { "ﾋﾟ", "ピ" }, { "ﾌﾟ", "プ" }, { "ﾍﾟ", "ペ" }, { "ﾎﾟ", "ポ" },
      { "ｳﾞ", "ヴ" },
      // 1文字構成の半角カナ
      { "ｱ", "ア" }, { "ｲ", "イ" }, { "ｳ", "ウ" }, { "ｴ", "エ" }, { "ｵ", "オ" },
      { "ｶ", "カ" }, { "ｷ", "キ" }, { "ｸ", "ク" }, { "ｹ", "ケ" }, { "ｺ", "コ" },
      { "ｻ", "サ" }, { "ｼ", "シ" }, { "ｽ", "ス" }, { "ｾ", "セ" }, { "ｿ", "ソ" },
      { "ﾀ", "タ" }, { "ﾁ", "チ" }, { "ﾂ", "ツ" }, { "ﾃ", "テ" }, { "ﾄ", "ト" },
      { "ﾅ", "ナ" }, { "ﾆ", "ニ" }, { "ﾇ", "ヌ" }, { "ﾈ", "ネ" }, { "ﾉ", "ノ" },
      { "ﾊ", "ハ" }, { "ﾋ", "ヒ" }, { "ﾌ", "フ" }, { "ﾍ", "ヘ" }, { "ﾎ", "ホ" },
      { "ﾏ", "マ" }, { "ﾐ", "ミ" }, { "ﾑ", "ム" }, { "ﾒ", "メ" }, { "ﾓ", "モ" },
      { "ﾔ", "ヤ" }, { "ﾕ", "ユ" }, { "ﾖ", "ヨ" },
      { "ﾗ", "ラ" }, { "ﾘ", "リ" }, { "ﾙ", "ル" }, { "ﾚ", "レ" }, { "ﾛ", "ロ" },
      { "ﾜ", "ワ" }, { "ｦ", "ヲ" }, { "ﾝ", "ン" },
      { "ｧ", "ァ" }, { "ｨ", "ィ" }, { "ｩ", "ゥ" }, { "ｪ", "ェ" }, { "ｫ", "ォ" },
      { "ｬ", "ャ" }, { "ｭ", "ュ" }, { "ｮ", "ョ" },
      { "ｯ", "ッ" },
      { "｡", "。" }, { "｢", "「" }, { "｣", "」" }, { "､", "、" }, { "･", "・" }, { "ｰ", "ー" },
    };
    public static string ConvertWideHiraganaToKatakana(string input)
    {
      if (Runtime.IsRunningOnNetFx) {
        return Strings.StrConv(input, VbStrConv.Katakana);
      }
      else {
        var inputChars = input.ToCharArray();
        var outputChars = new char[inputChars.Length];

        for (var index = 0; index < inputChars.Length; index++) {
          if (wideHiraganaStart <= inputChars[index] && inputChars[index] <= wideHiraganaEnd)
            outputChars[index] = (char)((int)inputChars[index] + offsetFromHiraganaToKatakana);
          else
            outputChars[index] = inputChars[index];
        }

        return new string(outputChars);
      }
    }

    public static string ConvertWideKatakanaToHiragana(string input)
    {
      if (Runtime.IsRunningOnNetFx) {
        return Strings.StrConv(input, VbStrConv.Hiragana);
      }
      else {
        var inputChars = input.ToCharArray();
        var outputChars = new char[inputChars.Length];

        for (var index = 0; index < inputChars.Length; index++) {
          if (wideKatakanaStart <= inputChars[index] && inputChars[index] <= wideKatakanaEnd)
            outputChars[index] = (char)((int)inputChars[index] - offsetFromHiraganaToKatakana);
          else
            outputChars[index] = inputChars[index];
        }

        return new string(outputChars);
      }
    }
  }
}

