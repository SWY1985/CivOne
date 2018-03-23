#!/bin/bash
BIN="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd)"
mkdir -p ~/.local/share/CivOne
( cd ~/.local/share/CivOne && $BIN"/CivOne.SDL" "$@" )