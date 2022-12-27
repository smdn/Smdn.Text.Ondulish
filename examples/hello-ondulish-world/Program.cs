// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using Smdn.Text.Ondulish;

using var t = new Translator();

// This code outputs as follows: "ｺﾝﾃﾞｨﾃﾞｨｳﾞｧ､ｵﾝﾄﾞｩﾙｺﾞﾄﾞｾｶｲ!"
Console.WriteLine(t.Translate("こんにちは、オンドゥル語の世界！"));

// This code outputs as follows: "ｵﾝﾄﾞｩﾙﾙﾗｷﾞｯﾀﾝﾃﾞｨｽｶｰ!?"
Console.WriteLine(t.Translate("本当に裏切ったんですか!?"));
