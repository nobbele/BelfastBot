#!/usr/bin/env bash

# Make sure process has had time to quit
sleep 5

find . -type f ! -name 'update-old.sh' -delete

cp $1/* .
