#!/bin/bash

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