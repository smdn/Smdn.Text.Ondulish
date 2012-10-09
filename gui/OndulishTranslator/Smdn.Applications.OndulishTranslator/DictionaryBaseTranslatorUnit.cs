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
using System.IO;
using System.Text;

using Smdn.Formats;

namespace Smdn.Applications.OndulishTranslator {
  public class DictionaryBaseTranslatorUnit : TranslatorUnitBase {
    public DictionaryBaseTranslatorUnit(string dictionaryPath)
    {
      try {
        dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        using (var reader = new CsvReader(dictionaryPath)) {
          for (;;) {
            var entries = reader.ReadLine();

            if (entries == null)
              break;
            else if (2 <= entries.Length)
              dictionary[entries[0].Trim()] = entries[1].Trim();
          }
        }
      }
      catch (IOException ex) {
        dictionary = null;
      }
    }

    public override StringBuilder Translate(StringBuilder input)
    {
      if (dictionary == null)
        return input;

      foreach (var pair in dictionary) {
        input.Replace(pair.Key, pair.Value);
      }

      return input;
    }

    private readonly Dictionary<string, string> dictionary;
  }
}

