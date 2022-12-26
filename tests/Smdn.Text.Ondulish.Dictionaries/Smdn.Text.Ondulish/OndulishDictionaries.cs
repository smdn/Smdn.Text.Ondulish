// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Smdn.Text.Ondulish;

[TestFixture]
public class OndulishDictionariesTests {
  [Test]
  public void OpenPhraseDictionaryStream()
  {
    using var stream = OndulishDictionaries.OpenPhraseDictionaryStream();

    Assert.IsNotNull(stream);
    Assert.Greater(stream.Length, 0);
    Assert.IsTrue(stream.CanRead);

    var reader = new StreamReader(stream, Encoding.UTF8);

    StringAssert.Contains(
      ",オンドゥルルラギッタンディスカー",
      reader.ReadToEnd()
    );
  }

  [Test]
  public void OpenWordDictionaryStream()
  {
    using var stream = OndulishDictionaries.OpenWordDictionaryStream();

    Assert.IsNotNull(stream);
    Assert.Greater(stream.Length, 0);
    Assert.IsTrue(stream.CanRead);

    var reader = new StreamReader(stream, Encoding.UTF8);

    StringAssert.Contains(
      ",ダディャーナザァーン",
      reader.ReadToEnd()
    );
  }
}
