# ValheimExportHelper
AssetRipper plugin to make it easy to export Valheim to a Unity project.

## Usage
1. Download ValheimExportHelper and one of the supported AssetRipper releases.
2. Extract the DLLs to your `AssetRipper/Plugins` directory (or create a `Plugins` directory if you don't have one).
3. Launch AssetRipper and proceed to load and extract Valheim as normal.

When Valheim is loaded into AssetRipper, the console window should show the Valheim logo. When everything is extracted it should also show some additional steps being done in the console.

## Developing
1. Get the target AssetRipper version's source code.
2. Compile all of the AssetRipper modules.
3. Add dependency references for `AssetRipper.Fundamentals`, `AssetRipperCommon`, `AssetRipperCore`, and `AssetRipperLibrary`.
4. Compile.
