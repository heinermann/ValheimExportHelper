## V2.0.0
* Made into a standalone application for AssetRipper 3.2.0.
* Fix version capture for Valheim 0.217.5.

## V1.5.0
* Fixed issues with corrupted WAVs.
* Fixed several issues with Valheim patch 0.214.2. It is still worse than previous versions, but loadable.

## V1.4.0
* Updated to work with AssetRipper 0.3.0.0
* Updated to use .NET 7
* Fixed an issue with Valheim 0.212.9

## V1.3.0
* Updated to work with AssetRipper 0.2.4.2
* Various fixes for Valheim 0.211.8 (PlayFab)

## V1.2.0
* Updated to work with AssetRipper 0.2.3.0
* Fixed various shader errors and issues. Unity will no longer corrupt them.
* Fixed a bug that prevented usage of "Dll Export Without Renaming".
* Turn off motion blur by default, so you can see the scenes properly.
* Rename the project directory from ExportedProject to "Valheim \<version string\> - YYYY-MM-DD"
* Cut down on Unity project dependencies slightly.

## V1.1.0
* Updated to work with AssetRipper 0.2.2.0
* Includes AssetBundling scripts
* Includes HLSL open source shaders
* Includes a blank scene for easier prefab viewing
* Include AssetBundleBrowser directly in the package list
* Fix for the Valheim cursor texture
* Fix for corrupted wav file (`Water_BoilLoop.wav`)
