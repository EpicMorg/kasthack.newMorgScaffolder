#!/bin/bash
#   Q-A:
#        What does this script do?
#           It generates symlinks to shared makefiles / docker-composes
#        What software does this script depend on?
#            bash
#        How to run this script?
#            ./<script-name>
#        What was the last time this script was tested and confiremed to work?
#            2021-11-21
#        Who can help with this script?
#            @kasthack. Available on github / telegram / twitter or anything else. Google me.

for VERSION in `echo */`
do
    cd $VERSION
        ln -s ../../shared/Makefile
        ln -s ../../shared/docker-compose.yml
    cd ..
done
cd 8
rm docker-compose.yml
ln -s ../../shared/docker-compose-dual-jdk.yml docker-compose.yml