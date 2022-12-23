# SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
# SPDX-License-Identifier: MIT

.DEFAULT_GOAL := all

THIS_FILE_DIR := $(abspath $(dir $(lastword $(MAKEFILE_LIST))))/
CXXFLAGS_COMMON = -fPIC -m64 -O3
MECAB_SOURCE_REPO := https://github.com/taku910/mecab.git
MECAB_SOURCE_ROOT_DIR := $(THIS_FILE_DIR)mecab/
MECAB_SOURCE_MECAB_DIR := $(MECAB_SOURCE_ROOT_DIR)mecab/
MECAB_SOURCE_MECAB_IPADIC_DIR := $(MECAB_SOURCE_ROOT_DIR)mecab-ipadic/
MECAB_SWIG_INTERFACE_FILE := $(MECAB_SOURCE_ROOT_DIR)mecab/swig/MeCab.i
LIBMECAB_STATIC_FILE := $(MECAB_SOURCE_MECAB_DIR)src/.libs/libmecab.a
MECAB_BINDINGS_NAMESPACE := MeCab
MECAB_BINDINGS_DLLIMPORTNAME := mecab
MECAB_BINDINGS_BASEDIR := $(THIS_FILE_DIR)mecab-bindings/
MECAB_BINDINGS_WRAPPER_SRC := $(MECAB_BINDINGS_BASEDIR)MeCab_wrapper.cxx
MECAB_BINDINGS_WRAPPER_OBJ := $(MECAB_BINDINGS_WRAPPER_SRC:%.cxx=%.o)
MECAB_BINDINGS_SOURCES_OUTPUT_DIR := $(MECAB_BINDINGS_BASEDIR)src/
MECAB_BINDINGS_NATIVE_LIBRARY_OUTPUT_BASEDIR := $(MECAB_BINDINGS_BASEDIR)runtimes/
MECAB_BINDINGS_NATIVE_LIBRARY_OUTPUT_PATH_UBUNTU_22_04_X64 := $(MECAB_BINDINGS_NATIVE_LIBRARY_OUTPUT_BASEDIR)ubuntu.22.04-x64/native/lib$(MECAB_BINDINGS_DLLIMPORTNAME).so

mecab-src: $(MECAB_SOURCE_ROOT_DIR)

$(MECAB_SOURCE_ROOT_DIR):
# TODO: specify commit hash or branch
	git clone \
	  $(MECAB_SOURCE_REPO) \
	  --depth 1 \
	  $(MECAB_SOURCE_ROOT_DIR)

	cd $(MECAB_SOURCE_MECAB_DIR) && ./configure \
	  --enable-shared \
	  --enable-static \
	  --with-charset=utf8 \
	  --with-pic

libmecab-static: $(LIBMECAB_STATIC_FILE)

$(LIBMECAB_STATIC_FILE): $(MECAB_SOURCE_ROOT_DIR)
	cd $(MECAB_SOURCE_MECAB_DIR) && ./configure \
	  --enable-shared \
	  --enable-static \
	  --with-pic \
	  --with-charset=utf8 \
	  --enable-utf8-only

	$(MAKE) \
	  --directory $(MECAB_SOURCE_MECAB_DIR)src/ \
	  CXXFLAGS="$(CXXFLAGS_COMMON)" \
	  libmecab.la

mecab-bindings: $(MECAB_BINDINGS_NATIVE_LIBRARY_OUTPUT_PATH_UBUNTU_22_04_X64)

$(MECAB_BINDINGS_NATIVE_LIBRARY_OUTPUT_PATH_UBUNTU_22_04_X64): $(LIBMECAB_STATIC_FILE)
	mkdir -p $(MECAB_BINDINGS_SOURCES_OUTPUT_DIR)

	swig -c++ -csharp \
	  -o $(MECAB_BINDINGS_WRAPPER_SRC) \
	  -namespace $(MECAB_BINDINGS_NAMESPACE) \
	  -dllimport $(MECAB_BINDINGS_DLLIMPORTNAME) \
	  -outdir $(MECAB_BINDINGS_SOURCES_OUTPUT_DIR) \
	  $(MECAB_SWIG_INTERFACE_FILE)

	mkdir -p $(dir $@)

	g++ $(CXXFLAGS_COMMON) -c $(MECAB_BINDINGS_WRAPPER_SRC) -I$(MECAB_SOURCE_MECAB_DIR)src/ -o $(MECAB_BINDINGS_WRAPPER_OBJ)
	g++ $(CXXFLAGS_COMMON) -shared -s $(MECAB_BINDINGS_WRAPPER_OBJ) $(LIBMECAB_STATIC_FILE) -o $@

mecab-ipadic: $(MECAB_SOURCE_ROOT_DIR)
	$(MAKE) --directory $(MECAB_SOURCE_MECAB_DIR) CXXFLAGS="$(CXXFLAGS_COMMON)" mecab-config
	$(MAKE) --directory $(MECAB_SOURCE_MECAB_DIR)src/ CXXFLAGS="$(CXXFLAGS_COMMON)" mecab-dict-index

	cd $(MECAB_SOURCE_MECAB_IPADIC_DIR) && ./configure \
	  --with-charset=utf8 \
	  --with-mecab-config=$(MECAB_SOURCE_MECAB_DIR)mecab-config

	$(MAKE) --directory $(MECAB_SOURCE_MECAB_IPADIC_DIR) \
	  MECAB_CONFIG="$(MECAB_SOURCE_MECAB_DIR)mecab-config" \
	  MECAB_DICT_INDEX="$(MECAB_SOURCE_MECAB_DIR)src/mecab-dict-index" \
	  mecab_dict_index="$(MECAB_SOURCE_MECAB_DIR)src/mecab-dict-index"

clean-mecab:
	$(MAKE) --directory $(MECAB_SOURCE_MECAB_DIR)src/ clean

clean-mecab-ipadic:
	$(MAKE) --directory $(MECAB_SOURCE_MECAB_IPADIC_DIR) clean

clean-mecab-bindings:
	rm -rf $(MECAB_BINDINGS_SOURCES_OUTPUT_DIR)

clean: clean-mecab clean-mecab-ipadic clean-mecab-bindings

all: mecab-bindings mecab-ipadic

.PHONY: mecab-src
.PHONY: libmecab-static
.PHONY: mecab-bindings
.PHONY: mecab-ipadic
.PHONY: clean-mecab
.PHONY: clean-mecab-ipadic
.PHONY: clean-mecab-bindings
.PHONY: clean
.PHONY: all
