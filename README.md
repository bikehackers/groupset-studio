# groupset-studio

![Main](https://github.com/bikehackers/groupset-studio/workflows/Main/badge.svg)

Experimental groupset compatability checker driven by the components database.

```bash
# Restore Paket
dotnet tool restore

# Install Paket dependencies
dotnet paket install
dotnet paket generate-load-scripts

# Generate the data blob
dotnet fsi ./generate-blob.fsx

# Install NPM dependencies
yarn install

# Build the app
yarn webpack-dev-server
```
