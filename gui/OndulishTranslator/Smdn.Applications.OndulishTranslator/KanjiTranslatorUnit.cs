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
using System.Text.RegularExpressions;

namespace Smdn.Applications.OndulishTranslator {
  public class KanjiTranslatorUnit : TranslatorUnitBase {
    public override StringBuilder Translate(StringBuilder input)
    {
      var ret = input.ToString();

      foreach (var map in kanjiMaps) {
        ret = map.Item1.Replace(ret, map.Item2);
      }

      return new StringBuilder(ret);
    }

    private static readonly Tuple<Regex, string>[] kanjiMaps = new[] {
      Tuple.Create(new Regex(@"\p{IsCJKUnifiedIdeographs}{6,}"), "ウェェェェェェェェェェェェェェェイ!!"),
      Tuple.Create(new Regex(@"\p{IsCJKUnifiedIdeographs}{5}"), "ダディャーナザァーン!!"),
      Tuple.Create(new Regex(@"\p{IsCJKUnifiedIdeographs}{4}"), "ドンドコドーン!!"),
      Tuple.Create(new Regex(@"\p{IsCJKUnifiedIdeographs}{3}"), "ウェイ!"),
      Tuple.Create(new Regex(@"\p{IsCJKUnifiedIdeographs}{2}"), "ウェッ!"),
      Tuple.Create(new Regex(@"\p{IsCJKUnifiedIdeographs}{1}"), "ヘァ!"),
    };
  }
}
