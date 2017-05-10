#!/bin/bash -e
# This script must be invoked as a sudoer

THE_USER=${SUDO_USER:=$USER}

cd "$(dirname "$0")"
cd Farmhash.Benchmarks
sudo -u $THE_USER make all

nice -n -20 ./benchmark-farmhash
nice -n -20 ./benchmark-farmhash-no-ha
