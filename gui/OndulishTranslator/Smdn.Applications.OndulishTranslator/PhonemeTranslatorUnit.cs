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
using System.Text;

using Microsoft.VisualBasic;

namespace Smdn.Applications.OndulishTranslator {
  public class PhonemeTranslatorUnit : TranslatorUnitBase {
    public override StringBuilder Translate(StringBuilder input)
    {
      //var ret = new StringBuilder(KanaUtils.ConvertWideKatakanaToHiragana(input.ToString()));
      var ret = new StringBuilder(input.ToString());

      // 最優先
      ret.Replace("る", "どぅ");
      ret.Replace("む", "ゔ");

      // 母音
      ret.Replace("あ", "あ゛");
      ret.Replace("う", "る");
      ret.Replace("や", "ゃ");

      // 摩擦音
      ret.Replace("さ", "ざぁ");
      ret.Replace("す", "ず");
      ret.Replace("ぜ", "で");

      ret.Replace("は", "ゔぁ");
      ret.Replace("ひ", "びぃ");
      ret.Replace("ふ", "ゔ");
      ret.Replace("へ", "べ");
      ret.Replace("ほ", "ぼ");

      ret.Replace("ぶ", "む");

      ret.Replace("ぜ", "で");

      // 破裂音変換
      ret.Replace("く", "ぐ");
      ret.Replace("き", "く");

      ret.Replace("で", "でぃ");

      ret.Replace("た", "だ");
      ret.Replace("ち", "でぃ");
      ret.Replace("つ", "づ");
      ret.Replace("て", "で");
      ret.Replace("と", "どぅ");

      ret.Replace("ぴ", "ゔぃ");

      // 鼻音
      ret.Replace("に", "でぃ");
      ret.Replace("ぬ", "ず");
      ret.Replace("ね", "べ");
      ret.Replace("の", "ど");

      ret.Replace("ま", "ば");
      ret.Replace("み", "ゔぃ");
      ret.Replace("め", "べ");
      ret.Replace("も", "ぼ");

      ret.Replace("り", "でぃ");
      ret.Replace("れ", "り");
      ret.Replace("ろ", "ど");

      return ret;
    }
  }
}

