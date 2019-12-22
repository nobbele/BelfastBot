#!/usr/bin/env bash

rm update-status

# Make sure process has had time to quit
sleep 5

find . -maxdepth 1 -not \( -name 'update-old.sh' -o -name '*.xml' \) -exec rm {} \;

cp $1/* .

echo 'DONE' > update-status