#!/bin/sh
dotnet script 10_FETCH_JSON.csx
./20_PREPARE_TEMPLATES.sh
dotnet script 30_SCAFFOLD_DIRECTORIES.cs
./40_INVOKE_MAKE.sh