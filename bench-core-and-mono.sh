#!/bin/bash -e
# This script must be invoked as a sudoer

THE_USER=${SUDO_USER:=$USER}

sudo -u $THE_USER dotnet restore
sudo -u $THE_USER dotnet restore -r net46

dotnet run -c Release -f netcoreapp2.1 -p \
    Farmhash.Sharp.Benchmarks/Farmhash.Sharp.Benchmarks.csproj

sudo -u $THE_USER cp -r BenchmarkDotNet.Artifacts BenchmarkDotNet.Artifacts-temp
chown -R $THE_USER.$THE_USER BenchmarkDotNet.Artifacts-temp

sudo -u $THE_USER FrameworkPathOverride=/usr/lib/mono/4.5-api/ \
    dotnet build -c Release -f net461 /property:DefineConstants=MONO \
    Farmhash.Sharp.Benchmarks/Farmhash.Sharp.Benchmarks.csproj

mono Farmhash.Sharp.Benchmarks/bin/Release/net46/Farmhash.Sharp.Benchmarks.exe
chown -R $THE_USER.$THE_USER BenchmarkDotNet.Artifacts
