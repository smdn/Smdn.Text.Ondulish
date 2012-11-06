#!/bin/sh

basedir=$(cd $(dirname $0) && pwd)
wrapperdir=$basedir/../../core/MeCab/

# make mecab
cd $basedir

if [ -d ./mecab-* ] ; then
  tar -xvf ./mecab-*.tar.*
fi
cd mecab-*

./configure --enable-shared --enable-static --with-charset=utf8 --prefix=/usr/local/mecab
make

# make mecab wrapper
cd swig

swig -csharp -o MeCab_wrap.cxx -namespace MeCab -dllimport libmecab -c++ MeCab.i
g++ -shared -s MeCab_wrap.o ../src/libmecab.a -o libmecab.so
mcs /t:library /out:MeCab.dll *.cs

# copy mecab wrapper
mkdir -p $wrapperdir
cp -t $wrapperdir MeCab.dll libmecab.so



# make mecab dictionary
cd $basedir

if [ -d ./mecab-ipadic-* ] ; then
  tar -xvf ./mecab-ipadic-*.tar.*
fi
cd mecab-ipadic-*

./configure --with-charset=utf8 --prefix=/usr/local/mecab
make

# copy mecab dictionary
mkdir -p $wrapperdir/dic/ipadic
cp -t $wrapperdir/dic/ipadic matrix.bin char.bin sys.dic unk.dic left-id.def right-id.def rewrite.def pos-id.def dicrc


