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
    private const char wideHiraganaStart = '\u3041';
    private const char wideHiraganaEnd = '\u3096';

    private const char wideKatakanaStart = '\u30a1';
    private const char wideKatakanaEnd = '\u30f6';

    private const int offsetFromHiraganaToKatakana = ((int)wideKatakanaStart - (int)wideHiraganaStart);

    private const char wideKatakanaExEnd = '\u30fa';

    private static readonly string[] wideToNarrowKatakanaMap = new[] {
      "ｧ", "ｱ", "ｨ", "ｲ", "ｩ", "ｳ", "ｪ", "ｴ", "ｫ", "ｵ", "ｶ", "ｶﾞ", "ｷ", "ｷﾞ", "ｸ",               // 30A1 - 30AF
      "ｸﾞ", "ｹ", "ｹﾞ", "ｺ", "ｺﾞ", "ｻ", "ｻﾞ", "ｼ", "ｼﾞ", "ｽ", "ｽﾞ", "ｾ", "ｾﾞ", "ｿ", "ｿﾞ", "ﾀ",    // 30B0 - 30BF
      "ﾀﾞ", "ﾁ", "ﾁﾞ", "ｯ", "ﾂ", "ﾂﾞ", "ﾃ", "ﾃﾞ", "ﾄ", "ﾄﾞ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ",       // 30C0 - 30CF
      "ﾊﾞ", "ﾊﾟ", "ﾋ", "ﾋﾞ", "ﾋﾟ", "ﾌ", "ﾌﾞ", "ﾌﾟ", "ﾍ", "ﾍﾞ", "ﾍﾟ", "ﾎ", "ﾎﾞ", "ﾎﾟ", "ﾏ", "ﾐ",  // 30D0 - 30DF
      "ﾑ", "ﾒ", "ﾓ", "ｬ", "ﾔ", "ｭ", "ﾕ", "ｮ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ヮ", "ﾜ",           // 30E0 - 30EF
      "ヰ", "ヱ", "ｦ", "ﾝ", "ｳﾞ", "ヵ", "ヶ", "ﾜﾞ", "ヸ", "ヹ", "ｦﾞ", // 30F0 - 30FA
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

    public static string ConvertWideKatakanaToNarrowKatakana(string input)
    {
      var inputChars = input.ToCharArray();
      var output = new StringBuilder();

      for (var index = 0; index < inputChars.Length; index++) {
        if (wideKatakanaStart <= inputChars[index] && inputChars[index] <= wideKatakanaExEnd)
          output.Append(wideToNarrowKatakanaMap[inputChars[index] - wideKatakanaStart]);
        else if (inputChars[index] == 'ー')
          output.Append('ｰ');
        else if (inputChars[index] == '、')
          output.Append('､');
        else if (inputChars[index] == '。')
          output.Append('｡');
        else if (inputChars[index] == '，')
          output.Append(',');
        else if (inputChars[index] == '．')
          output.Append('.');
        else
          output.Append(inputChars[index]);
      }

      return output.ToString();
    }
  }
}

