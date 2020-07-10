# OpenWithTerminal

OpenWithTerminal is a micro utility (just for windows 10 for now) to start very fast your favorite programs. Works better if you map to a an empty key as pause key.
When is launched will prompt the config.json options and you just need to press the indicated button to open the application.

First started will auto-generate the config.json template with some entries.


## Installation

Download (or clone), place folder where you like, open executable folder and run openWithTerminal.exe. They will auto-generate config.json so you can close and edit as you need it for the next start. If you prefer you can donwload source and build in visual studio (Newtonsoft.Json nuget is required).

If you want map .exe to your keyboard you can download an external keyboard utility.

## Build yourself
* Clone repository or donwload Program.cs (only file you need it)

```bash
 git clone https://github.com/erika9133/openWithTerminal
```
* Import class in new visual studio project
* Import Newtonsoft nuget
* Publish or build in a single file with command below (not recomended because of the size)

```bash
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
```

## Config explanation

```json
{
  "MapsKeys": [
    {
      "Key": "1",
      "Route": "notepad.exe",
      "Description": "Notepad"
    },
    {
      "Key": "2",
      "Route": "cmd.exe",
      "Description": "Command Prompt"
    },
    {
      "Key": "3",
      "Route": "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe",
      "Description": "Chrome"
    },
    {
      "Key": "4",
      "Route": "%UserProfile%\\fastnotes.txt",
      "Description": "Fast Notes (create a new file if doesn't exist)"
    }
  ],
  "Default": {
    "Route": "explorer.exe",
    "Description": "Windows explorer"
  },
  "CreateFileIfNotExist": true
}

```
This is the auto-generated file. You can map as keys as you want or deleted auto-gendered!
There are three different kinds of routes.
* Files who already exist in the system path (as notepad.exe example). 
* Files with absolute path (as chrome example)
* Files with an environment variable (as Fast notes example).

Please be carefully typing routes. Only one environment parameter can be used now.
Also, there is a default program that will be started if the program doesn't find the input key in the config file.

The parameter CreateFileIfNotExist will create the file if it doesn't be an .exe file (As Fast notes example) and doesn't exist.



## Keys supported
* Numbers 0-9
* F keys (F1-F12)
* Letters A-Z.
* Symbols don't work for now

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## Next steps...
I would like to fork into a system tray application.

## My custom keyboard app doesn't run cmd apps
In this case, you can create a macro (example below) to start "Windows run" (Win + R). I recommend renaming the folder and the .exe file to a simple letter like "o" and putting it directly in the root folder (usually "C:").

Remember to include a delay (100MS should be fine) next to open "Windows Run" to let some time to system to open windows run application.

<img src="https://github.com/erika9133/openWithTerminal/blob/master/gitResources/macro1.png" width="300">
<img src="https://github.com/erika9133/openWithTerminal/blob/master/gitResources/macro2.png" width="300">

## License
[MIT](https://choosealicense.com/licenses/mit/)
