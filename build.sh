#!/usr/bin/env bash

dotnet tool restore

cd ./external/components-db/components
dotnet paket install
cd -

dotnet paket install
dotnet paket generate-load-scripts
dotnet build
dotnet test

dotnet fsi ./generate-blob.fsx

yarn install --pure-lockfile

rm -rf ./out

NODE_ENV=production yarn webpack
