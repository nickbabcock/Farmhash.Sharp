#!/bin/bash -e

mlr --icsv --ocsv --rs lf put '$OutputBits = "32"' HashBenchmark32-report.csv > benchmarks.csv
mlr --icsv --ocsv --rs lf put '$OutputBits = "64"' HashBenchmark64-report.csv | \
    xsv cat rows - benchmarks.csv | \
    sponge benchmarks.csv

mlr --icsv --ocsv --rs lf put '$OutputBits = "32"' ../BenchmarkDotNet.Artifacts/results/HashBenchmark32-report.csv | \
    xsv cat rows - benchmarks.csv | \
    sponge benchmarks.csv
mlr --icsv --ocsv --rs lf put '$OutputBits = "64"' ../BenchmarkDotNet.Artifacts/results/HashBenchmark64-report.csv | \
    xsv cat rows - benchmarks.csv | \
    sponge benchmarks.csv

mlr --icsv --ocsv put '$OutputBits = "32"' net46-results/HashBenchmark32-report.csv | \
    xsv cat rows - benchmarks.csv | \
    sponge benchmarks.csv
mlr --icsv --ocsv put '$OutputBits = "64"' net46-results/HashBenchmark64-report.csv | \
    xsv cat rows - benchmarks.csv | \
    sponge benchmarks.csv
