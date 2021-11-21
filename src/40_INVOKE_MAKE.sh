#!/bin/bash
cd atlassian/output

for PRODUCT in `echo */`
do
    cd $PRODUCT
    for MAJOR_VERSION in `echo */`
    do
        cd $MAJOR_VERSION
        for VERSION in `echo */`
        do
            cd $VERSION
            echo "Invoking make at `pwd`"
            make
            cd ..
        done
        cd ..
    done
    cd ..
done
