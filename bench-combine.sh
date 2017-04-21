#!/bin/bash -e

xsv cat rows core-HashBenchmark32-report.csv core-HashBenchmark64-report.csv \
             mono-HashBenchmark32-report.csv mono-HashBenchmark64-report.csv \
             clr-HashBenchmark32-report.csv clr-HashBenchmark64-report.csv > benchmarks.csv
