// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
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

    Assert.That(stream, Is.Not.Null);
    Assert.That(stream.Length, Is.GreaterThan(0));
    Assert.That(stream.CanRead, Is.True);

    var reader = new StreamReader(stream, Encoding.UTF8);

    Assert.That(
      reader.ReadToEnd(),
      Does.Contain(",オンドゥルルラギッタンディスカー")
    );
  }

  [Test]
  public void OpenWordDictionaryStream()
  {
    using var stream = OndulishDictionaries.OpenWordDictionaryStream();

    Assert.That(stream, Is.Not.Null);
    Assert.That(stream.Length, Is.GreaterThan(0));
    Assert.That(stream.CanRead, Is.True);

    var reader = new StreamReader(stream, Encoding.UTF8);

    Assert.That(
      reader.ReadToEnd(),
      Does.Contain(",ダディャーナザァーン")
    );
  }
}
