#!/bin/bash

#   Q-A:
#        What does this script do?
#            Runs make for the generated directories
#        What software does this script depend on?
#            bash
#            make
#        How to run this script?
#            ./<script-name>
#        What was the last time this script was tested and confiremed to work?
#            2021-11-21
#        Who can help with this script?
#            @kasthack. Available on github / telegram / twitter or anything else. Google me.

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
