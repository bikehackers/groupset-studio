#!/usr/bin/env bash

dotnet restore
dotnet tool restore
dotnet paket install
dotnet paket generate-load-scripts
dotnet build
dotnet test

dotnet fsi ./generate-blob.fsx

yarn install --pure-lockfile

rm -rf ./out

NODE_ENV=production yarn webpack
