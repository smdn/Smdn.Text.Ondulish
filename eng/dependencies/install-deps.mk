# SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
# SPDX-License-Identifier: MIT
#
# This script installs the dependent apt packages to compile and run
# MeCab shared library, especially on GitHub Actions' runner image.
#
install-buildtime-deps-ubuntu.22.04.stamp:
	sudo apt-get install -y \
	  autotools-dev
	touch $@

install-buildtime-deps-ubuntu.22.04: install-buildtime-deps-ubuntu.22.04.stamp

.PHONY: install-buildtime-deps-ubuntu.22.04
