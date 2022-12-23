// SPDX-FileCopyrightText: 2012 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

using System;
using System.Text;

namespace Smdn.Text.Ondulish;

public static class KanaUtils {
  private const char WideHiraganaStart = '\u3041';
  private const char WideHiraganaEnd = '\u3096';

  private const char WideKatakanaStart = '\u30a1';
  private const char WideKatakanaEnd = '\u30f6';

  private const int OffsetFromHiraganaToKatakana = WideKatakanaStart - WideHiraganaStart;

  private const char WideKatakanaExEnd = '\u30fa';

  private static readonly string[] WideToNarrowKatakanaMap = new[] {
    "ｧ", "ｱ", "ｨ", "ｲ", "ｩ", "ｳ", "ｪ", "ｴ", "ｫ", "ｵ", "ｶ", "ｶﾞ", "ｷ", "ｷﾞ", "ｸ",               // 30A1 - 30AF
    "ｸﾞ", "ｹ", "ｹﾞ", "ｺ", "ｺﾞ", "ｻ", "ｻﾞ", "ｼ", "ｼﾞ", "ｽ", "ｽﾞ", "ｾ", "ｾﾞ", "ｿ", "ｿﾞ", "ﾀ",    // 30B0 - 30BF
    "ﾀﾞ", "ﾁ", "ﾁﾞ", "ｯ", "ﾂ", "ﾂﾞ", "ﾃ", "ﾃﾞ", "ﾄ", "ﾄﾞ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ",       // 30C0 - 30CF
    "ﾊﾞ", "ﾊﾟ", "ﾋ", "ﾋﾞ", "ﾋﾟ", "ﾌ", "ﾌﾞ", "ﾌﾟ", "ﾍ", "ﾍﾞ", "ﾍﾟ", "ﾎ", "ﾎﾞ", "ﾎﾟ", "ﾏ", "ﾐ",  // 30D0 - 30DF
    "ﾑ", "ﾒ", "ﾓ", "ｬ", "ﾔ", "ｭ", "ﾕ", "ｮ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ヮ", "ﾜ",           // 30E0 - 30EF
    "ヰ", "ヱ", "ｦ", "ﾝ", "ｳﾞ", "ヵ", "ヶ", "ﾜﾞ", "ヸ", "ヹ", "ｦﾞ", // 30F0 - 30FA
  };

  public static string ConvertWideHiraganaToKatakana(string input)
  {
    if (input is null)
      throw new ArgumentNullException(nameof(input));
    if (input.Length == 0)
      return string.Empty;

#if SYSTEM_STRING_CREATE
    return string.Create(input.Length, input, static (chars, s) => {
      for (var index = 0; index < chars.Length; index++) {
        chars[index] = s[index] is >= WideHiraganaStart and <= WideHiraganaEnd
          ? (char)(s[index] + OffsetFromHiraganaToKatakana)
          : s[index];
      }
    });
#else
    var outputChars = new char[input.Length];

    for (var index = 0; index < input.Length; index++) {
      outputChars[index] = input[index] is >= WideHiraganaStart and <= WideHiraganaEnd
        ? (char)(input[index] + OffsetFromHiraganaToKatakana)
        : outputChars[index] = input[index];
    }

    return new string(outputChars);
#endif
  }

  public static string ConvertWideKatakanaToHiragana(string input)
  {
    if (input is null)
      throw new ArgumentNullException(nameof(input));
    if (input.Length == 0)
      return string.Empty;

#if SYSTEM_STRING_CREATE
    return string.Create(input.Length, input, static (chars, s) => {
      for (var index = 0; index < chars.Length; index++) {
        chars[index] = s[index] is >= WideKatakanaStart and <= WideKatakanaEnd
          ? (char)(s[index] - OffsetFromHiraganaToKatakana)
          : s[index];
      }
    });
#else
    var outputChars = new char[input.Length];

    for (var index = 0; index < input.Length; index++) {
      outputChars[index] = input[index] is >= WideKatakanaStart and <= WideKatakanaEnd
        ? (char)(input[index] - OffsetFromHiraganaToKatakana)
        : input[index];
    }

    return new string(outputChars);
#endif
  }

  public static string ConvertWideKatakanaToNarrowKatakana(string input)
  {
    if (input is null)
      throw new ArgumentNullException(nameof(input));
    if (input.Length == 0)
      return string.Empty;

    var output = new StringBuilder(input.Length * 2);

    for (var index = 0; index < input.Length; index++) {
      output.Append(
        input[index] switch {
          >= WideKatakanaStart and <= WideKatakanaExEnd => WideToNarrowKatakanaMap[input[index] - WideKatakanaStart],
          'ー' => 'ｰ',
          '゛' => 'ﾞ',
          '゜' => 'ﾟ',
          '？' => '?',
          '！' => '!',
          '、' => '､',
          '。' => '｡',
          '，' => ',',
          '．' => '.',
          _ => input[index],
        }
      );
    }

    return output.ToString();
  }
}
