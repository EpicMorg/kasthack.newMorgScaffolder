#!/bin/sh

#   Q-A:
#        What does this script do?
#            It runs the full build: 
#               updates jsons
#               configures templates
#               scaffolds directories
#               builds containers
#        What software does this script depend on?
#            bash
#        How to run this script?
#            ./<script-name>
#        What was the last time this script was tested and confiremed to work?
#            2021-11-21
#        Who can help with this script?
#            @kasthack. Available on github / telegram / twitter or anything else. Google me.

dotnet script 10_FETCH_JSON.csx
./20_PREPARE_TEMPLATES.sh
dotnet script 30_SCAFFOLD_DIRECTORIES.cs
./40_INVOKE_MAKE.sh