[![GitHub license](https://img.shields.io/github/license/smdn/Smdn.Text.Ondulish)](https://github.com/smdn/Smdn.Text.Ondulish/blob/main/LICENSE.txt)
[![tests/main](https://img.shields.io/github/actions/workflow/status/smdn/Smdn.Text.Ondulish/test.yml?branch=main&label=tests%2Fmain)](https://github.com/smdn/Smdn.Text.Ondulish/actions/workflows/test.yml)
[![CodeQL](https://github.com/smdn/Smdn.Text.Ondulish/actions/workflows/codeql-analysis.yml/badge.svg?branch=main)](https://github.com/smdn/Smdn.Text.Ondulish/actions/workflows/codeql-analysis.yml)

# Smdn.Text.Ondulish
This repository contains the following two .NET assemblies/NuGet packages.

|Assembly|Package|
| --- | --- |
|[Smdn.Text.Ondulish](src/Smdn.Text.Ondulish/)|[![NuGet Smdn.Text.Ondulish](https://img.shields.io/nuget/v/Smdn.Text.Ondulish.svg)](https://www.nuget.org/packages/Smdn.Text.Ondulish/)|
|[Smdn.Text.Ondulish.Dictionaries](src/Smdn.Text.Ondulish.Dictionaries/)|[![NuGet Smdn.Text.Ondulish.Dictionaries](https://img.shields.io/nuget/v/Smdn.Text.Ondulish.Dictionaries.svg)](https://www.nuget.org/packages/Smdn.Text.Ondulish.Dictionaries/)|

----

**[Smdn.Text.Ondulish](src/Smdn.Text.Ondulish/)** is a text conversion library that provides translation features from Japanese to [Ondulish](https://kamenrider.fandom.com/wiki/Kazuma_Kenzaki#Memes) (オンドゥル語, Ondul-go, Ondul language).

**[Smdn.Text.Ondulish](src/Smdn.Text.Ondulish/)** は日本語から[オンドゥル語](https://ja.wikipedia.org/wiki/%E4%BB%AE%E9%9D%A2%E3%83%A9%E3%82%A4%E3%83%80%E3%83%BC%E5%89%A3#%E3%82%AA%E3%83%B3%E3%83%89%E3%82%A5%E3%83%AB%E8%AA%9E)への変換機能を提供する.NETライブラリです。

This library requires [MeCab](https://taku910.github.io/mecab/) for the text conversion. The NuGet package `Smdn.Text.Ondulish` includes and provides MeCab bindings for .NET. Supported platforms of the bundled wrapper library are described by the property `RuntimeIdentifiers`' [RID](https://learn.microsoft.com/dotnet/core/rid-catalog). See the file [Smdn.Text.Ondulish.csproj](src/Smdn.Text.Ondulish/Smdn.Text.Ondulish.csproj).

For other platforms, it is required to build and deploy the wrapper library for the bindings by yourself for now. See [build instructions](./doc/build-mecab-wrapper-library.md) for detail.

このライブラリでは、テキスト変換に[MeCab](https://taku910.github.io/mecab/)を使用しています。　NuGetパッケージ`Smdn.Text.Ondulish`には、.NET用MeCabバインディングも同梱していますが、[Smdn.Text.Ondulish.csproj](src/Smdn.Text.Ondulish/Smdn.Text.Ondulish.csproj)の`RuntimeIdentifiers`に記載されているプラットフォームのみをサポートしています。

それ以外のプラットフォームでは、MeCabバインディング用のラッパーライブラリを個別にビルドして配置する必要があります。　詳しくは[ビルド手順](./doc/build-mecab-wrapper-library.md)を参照してください。

----

**[Smdn.Text.Ondulish.Dictionaries](src/Smdn.Text.Ondulish.Dictionaries/)** is a library provides dictionaries for conversion to Ondulish words and phrases.

**[Smdn.Text.Ondulish.Dictionaries](src/Smdn.Text.Ondulish.Dictionaries/)** はオンドゥル語の単語とフレーズへの変換を行う辞書を提供するライブラリです。

These dictionaries are provided as `<EmbeddedResource>`s in the assembly, and are automatically loaded from `Smdn.Text.Ondulish`.

## Usage
First, add `<PackageReference>`s to the project file.

```xml
  <ItemGroup>
    <PackageReference Include="Smdn.Text.Ondulish" Version="4.0.*" />
    <PackageReference Include="Smdn.Text.Ondulish.Dictionaries" Version="4.0.*" />
  </ItemGroup>
```

Then write the code like below:

```cs
using Smdn.Text.Ondulish;

using var t = new Translator();

Console.WriteLine(t.Translate("本当に裏切ったんですか!?"));
```

This code outputs as follows:

```txt
ｵﾝﾄﾞｩﾙﾙﾗｷﾞｯﾀﾝﾃﾞｨｽｶｰ!?
```

See the projects in the [examples](./examples/) directory for more usages.

## Troubleshooting at run time
### Warning `NETSDK1206` / Simplified RID issues (.NET 8.0)
Starting with .NET 8.0, Runtime Identifiers (RID) such as `ubuntu.22.04-x64` have been simplified and it is recommended to use `linux-x64` instead.

Some older versions of `Smdn.Text.Ondulish` packages do not contain native binaries targeting `linux-x64`.

This may result in native binaries not being deployed and `DllNotFoundException` being thrown at runtime. Also, a warning `NETSDK1206` is displayed at build time.

```
$ dotnet build
/usr/share/dotnet/sdk/8.0.101/Sdks/Microsoft.NET.Sdk/targets/Microsoft.NET.Sdk.targets(284,5): warning NETSDK1206: Found version-specific or distribution-specific runtime identifier(s): ubuntu.22.04-x64. Affected libraries: Smdn.Text.Ondulish. In .NET 8.0 and higher, assets for version-specific and distribution-specific runtime identifiers will not be found by default. See https://aka.ms/dotnet/rid-usage for details. [/home/runner/work/OndulishTranslator/OndulishTranslator.csproj::TargetFramework=net8.0]

$ dotnet run
System.TypeInitializationException: The type initializer for 'MeCab.MeCabPINVOKE' threw an exception.
  ---> System.TypeInitializationException: The type initializer for 'SWIGExceptionHelper' threw an exception.
  ---> System.DllNotFoundException: Unable to load shared library 'mecab' or one of its dependencies. In order to help diagnose loading problems, consider using a tool like strace. If you're using glibc, consider setting the LD_DEBUG environment variable:
```

To avoid this problem, use the version of `Smdn.Text.Ondulish` that ships with `linux-x64` native binaries, or apply the workaround given in [smdn/Smdn.LibHighlightSharp issue #141](https://github.com/smdn/Smdn.LibHighlightSharp/issues/141).



### `DllImport` issues
If the exception `TypeInitializationException` or `DllNotFoundException` occurs like below, the [MeCab SWIG binding library](https://github.com/taku910/mecab/tree/master/mecab/swig) may not have been deployed correctly or is not available on the deployed platform.

```txt
Unhandled exception. System.TypeInitializationException: The type initializer for 'MeCab.MeCabPINVOKE' threw an exception.
 ---> System.TypeInitializationException: The type initializer for 'SWIGExceptionHelper' threw an exception.
 ---> System.DllNotFoundException: Unable to load shared library 'mecab' or one of its dependencies. In order to help diagnose loading problems, consider setting the LD_DEBUG environment variable: libmecab: cannot open shared object file: No such file or directory
```

If you have copied or moved the output files generated by `dotnet build` or `dotnet publish` to another location, make sure that the wrapper library (`.dll`/`.so`/`.dylib`) is correctly deployed also and its dependencies are installed on your system.

One of the following wrapper library files must be located in the same directory as the executables or in the `runtimes/<RID>/native/` directory.

- `mecab.dll` (Windows)
- `libmecab.so` (Linux)
- `libmecab.dylib` (macOS)

# Documents

## API list
The APIs exposed by each package are listed in [doc/api-list/](doc/api-list/).



# For contributers
Contributions are welcome!

It is very welcome to accept contributions to improve and update the [word](src/Smdn.Text.Ondulish.Dictionaries/dictionary-words.csv) and [phrase](src/Smdn.Text.Ondulish.Dictionaries/dictionary-phrases.csv) dictionary. Please propose these changes by creating a [Issue](/../../issues/new?labels=dictionary-improvements&template=10_dictionary-improvements.yml&title=[辞書改善]%3A+) or [Pull Request](/../../pulls/).

[単語](src/Smdn.Text.Ondulish.Dictionaries/dictionary-words.csv)と[フレーズ](src/Smdn.Text.Ondulish.Dictionaries/dictionary-phrases.csv)の辞書については、更新・改善に関する変更を積極的に受け付けています。　[Issue](/../../issues/new?labels=dictionary-improvements&template=10_dictionary-improvements.yml&title=[辞書改善]%3A+)または[プルリクエスト](/../../pulls/)にてご提案ください。

Note that though, adding library functionality or expanding target platforms is not a major goal for now, so changes like these are not considered high priority task.

In any case, if you have any suggestions, please let me know via [issues](/../../issues/).

# Notice
This project uses the following components. See [ThirdPartyNotices.md](./ThirdPartyNotices.md) for detail.

## MeCab/MeCab IPA dictionary
The NuGet package [Smdn.Text.Ondulish](src/Smdn.Text.Ondulish/) bundles the [MeCab SWIG bindings](https://github.com/taku910/mecab/tree/master/mecab/swig) and the [MeCab IPA dictionary](https://github.com/taku910/mecab/tree/master/mecab-ipadic).
