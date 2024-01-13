# Rampancy.Lightbridge
A bridge from Trenchbroom to Halo

Rampancy.LightBridge is a tool to bridge the level creation workflow from editors like Trenchbroom to Halo.</br>
Related but not a replacement for my other Rampancy project.

# Features
* Halo 1 Support
* Trenchbroom support

## Halo 1: Setup
You will need to grab 3 things to get setup
* Rampancy.LightBridge from this repo
* TrenchBroom from here https://github.com/TrenchBroom/TrenchBroom/releases 
* Ericw's tools from here https://github.com/ericwa/ericw-tools/releases (the `ericw-tools-x-win64.zip` one)

Setup:
* Pick a folder to setup Rampancy.Lightbridge in, eg. ``Rampancy_Lightbridge`` under your Halo one HEK install
* Extract Rampancy.Lightbridge here and run it
* Click Ok on the popup telling you profile paths aren't set, we will be setting those now
* Click on the settings cog wheel in the top right
  * Set the path to your HEK Install. eg. ```C:\Steam\steamapps\common\Chelan_1```
  * Pick a folder for the output location, this can be anywhere but it will be where you will save your map sources and the exported Halo shaders will go
  * Click Save and then close the settings with the cog wheel
* Click the Sync button to setup the exported shaders into your work dir
* If all goes well in a few seconds you should see the log say ```[INFO] Finished scanning tags in ""```

* Now open the ericw tools zip you downloaded and copy the files from `ericw-tools-x-win64\bin` in there to `RampancyLightBridge/Compilers/ericw` folder next to where you extracted `Rampancy.LightBridge.exe` to.</br> You do only need the `qbsp.exe`, `embree3.dll`, `tbb.dll`, `tbbmalloc.dll` files in there.

* Next is setting up Trenchbroom
  * Extract Trenchbroom to some where you like
  * Copy the `RampancyHalo1` folder from the Rampancy.Lightbridge install to `games` under the Trenchbroom install
  * Start Trenchbroom, select `New Map` and for game select `RampancyHalo1`
  * Click `Run` > `Compile Map`, Click the `+` in the bottom left to add a new profile.
  * For `Working Directory` set it to where you extracted Rampancy.Lightbridge to, eg `C:\Steam\steamapps\common\Chelan_1\Rampancy_LightBridge`
  * Set `Tool Path` to `${WORK_DIR_PATH}/Rampancy.LightBridge.exe`
  * Set `Parameters` to `compile --profile "Halo CE (MCC) - Trenchbroom" --src "${MAP_DIR_PATH}\${MAP_FULL_NAME}" --structure --light 0.0001`
    * This will compile the map with settings from the `Halo CE (MCC) - Trenchbroom` profile
    * `--structure` will compile the structure with tool
    * `--light 0.0001` will use tool to light the map, the number at the end is the quality, negative numbers are previews

* Testing
  * Save the current map under the output folder you set in Rampancy. it should be like `maps\lb\test\models` with the name `test.map`
    * The structure under `maps` should mirror what you would have under `data` in the HEK install