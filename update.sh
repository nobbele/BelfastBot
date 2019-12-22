#!/usr/bin/env bash

# Make sure process has had time to quit
sleep 5

find . -maxdepth 1 -not \( -name 'update-old.sh' -o -name '*.json' \)

cp $1/* .
