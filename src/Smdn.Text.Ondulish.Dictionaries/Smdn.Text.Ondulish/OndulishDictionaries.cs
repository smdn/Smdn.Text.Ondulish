// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System.IO;
using System.Reflection;

namespace Smdn.Text.Ondulish;

public static class OndulishDictionaries {
  private static Stream? OpenDictionary(string name)
    => Assembly.GetExecutingAssembly().GetManifestResourceStream(name);

  public static Stream OpenPhraseDictionaryStream() => OpenDictionary(ManifestResourceNames.OndulishDictionaryPhrases) ?? Stream.Null;
  public static Stream OpenWordDictionaryStream() => OpenDictionary(ManifestResourceNames.OndulishDictionaryWords) ?? Stream.Null;
}
