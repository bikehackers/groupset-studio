#!/usr/bin/env bash

dotnet restore
dotnet tool restore
dotnet paket install

cd ./external/components-db/components
dotnet paket install
cd -

dotnet paket generate-load-scripts
dotnet build
dotnet test

dotnet fsi ./generate-blob.fsx

yarn install --pure-lockfile

rm -rf ./out

NODE_ENV=production yarn webpack
