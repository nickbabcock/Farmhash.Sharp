#!/bin/bash -e

cd "$(dirname "$0")"
cd Farmhash.Benchmarks
make all

sudo nice -n -20 ./benchmark-farmhash
sudo nice -n -20 ./benchmark-farmhash-no-ha
