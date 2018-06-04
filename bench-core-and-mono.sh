#!/bin/bash -e

dotnet run -c Release -f netcoreapp2.1 -p \
    Farmhash.Sharp.Benchmarks/Farmhash.Sharp.Benchmarks.csproj

cp -r BenchmarkDotNet.Artifacts BenchmarkDotNet.Artifacts-temp

FrameworkPathOverride=/usr/lib/mono/4.5-api/ \
    dotnet build -c Release -f net461 /property:DefineConstants=MONO \
    Farmhash.Sharp.Benchmarks/Farmhash.Sharp.Benchmarks.csproj

mono Farmhash.Sharp.Benchmarks/bin/Release/net46/Farmhash.Sharp.Benchmarks.exe
