#!/bin/sh

basedir=$(cd $(dirname $0) && pwd)
wrapperdir=$basedir/../../core/MeCab/
installdir=$basedir/mecab

package_mecab=mecab-0.994
package_mecab_ipadic=mecab-ipadic-2.7.0-20070801

# make mecab
cd $basedir

if [ ! -d $package_mecab ]; then
  tar -xvf ${package_mecab}.tar.*
  cd $package_mecab
  ./configure --enable-shared --enable-static --with-charset=utf8 --prefix=$installdir
else
  cd $package_mecab
fi

make
make install

# make mecab wrapper
cd swig

swig -csharp -o MeCab_wrap.cxx -namespace MeCab -dllimport libmecab -c++ MeCab.i
g++ -fPIC -O2 -c MeCab_wrap.cxx -I../src/
g++ -shared -s MeCab_wrap.o ../src/.libs/libmecab.a -o libmecab.so
mcs /t:library /out:MeCab.dll *.cs

# copy mecab wrapper
mkdir -p $wrapperdir
cp -t $wrapperdir MeCab.dll libmecab.so



# make mecab dictionary
export DYLD_LIBRARY_PATH=$installdir/lib:$DYLD_LIBRARY_PATH
export LD_LIBRARY_PATH=$installdir/lib:$LD_LIBRARY_PATH
export C_INCLUDE_PATH=$installdir/include
PATH=$installdir/bin:$PATH

cd $basedir

if [ ! -d $package_mecab_ipadic ]; then
  tar -xvf ${package_mecab_ipadic}.tar.*
  cd $package_mecab_ipadic
  ./configure --with-charset=utf8 --prefix=$installdir
else
  cd $package_mecab_ipadic
fi

make
make install

# copy mecab dictionary
mkdir -p $wrapperdir/dic/ipadic
cp -t $wrapperdir/dic/ipadic matrix.bin char.bin sys.dic unk.dic left-id.def right-id.def rewrite.def pos-id.def dicrc


