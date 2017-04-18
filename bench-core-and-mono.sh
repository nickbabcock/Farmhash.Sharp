#!/bin/bash -e

dotnet restore
dotnet restore -r net46

sudo dotnet run -c Release -f netcoreapp1.1 -p \
    Farmhash.Sharp.Benchmarks/Farmhash.Sharp.Benchmarks.csproj

cp -r BenchmarkDotNet.Artifacts BenchmarkDotNet.Artifacts-temp

export FrameworkPathOverride=/usr/lib/mono/4.5/
dotnet build -c Release -f net46 /property:DefineConstants=MONO \
    Farmhash.Sharp.Benchmarks/Farmhash.Sharp.Benchmarks.csproj

sudo mono /home/nick/projects/Farmhash.Sharp/Farmhash.Sharp.Benchmarks/bin/Release/net46/Farmhash.Sharp.Benchmarks.exe
