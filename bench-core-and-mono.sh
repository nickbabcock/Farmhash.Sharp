#!/bin/bash -e

sudo -u $USER dotnet restore
sudo -u $USER dotnet restore -r net46

dotnet run -c Release -f netcoreapp1.1 -p \
    Farmhash.Sharp.Benchmarks/Farmhash.Sharp.Benchmarks.csproj

sudo -u $USER cp -r BenchmarkDotNet.Artifacts BenchmarkDotNet.Artifacts-temp
chown -R $USER.$USER BenchmarkDotNet.Artifacts-temp

export FrameworkPathOverride=/usr/lib/mono/4.5/
sudo -u $USER dotnet build -c Release -f net46 /property:DefineConstants=MONO \
    Farmhash.Sharp.Benchmarks/Farmhash.Sharp.Benchmarks.csproj

mono Farmhash.Sharp.Benchmarks/bin/Release/net46/Farmhash.Sharp.Benchmarks.exe
chown -R $USER.$USER BenchmarkDotNet.Artifacts
