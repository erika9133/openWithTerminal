using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace openWithTerminal
{
    public class ConfigElement
    {
        public string Route;
    }

    public class ConfigElementMapped : ConfigElement
    {
        public string Key;
        public string Description;
    }

    public class ConfigFile
    {
        public ConfigElementMapped[] MapsKeys;
        public ConfigElement Default;
        public bool CreateFileIfNotExist;
    }

    internal class Program
    {
        static void Main()
        {
            const string filepath = @"CONFIG.json";

            if (!File.Exists(filepath))
            {
                CreateNewConfigFile(filepath);
            }

            ConfigFile fileLoaded = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(filepath));

            Console.WriteLine("OpenWithTerminal - Press a key from the list to open a program. ESC to exit");

            GenerateOptions(fileLoaded);

            ConsoleKeyInfo input = Console.ReadKey(); //Stop and wait for user input

            if (input.Key == ConsoleKey.Escape || fileLoaded.MapsKeys.Length <= 0) return;

            foreach (var element in fileLoaded.MapsKeys)
            {
                if (element.Key != input.Key.ToString()) continue;

                try
                {
                    //To avoid path problems change current directory first so firt split the route
                    string[] split = element.Route.Split(Path.DirectorySeparatorChar);

                    //Case it used a environment variable...
                    if (split.Any(x => x.Contains('%')))
                    {
                        //Select environment variables
                        string[] envVars = split.Where(x => x.Contains('%')).ToArray();

                        //Check possible errors
                        if (envVars.Length > 1) throw new Exception("Error. Only work with one environment variable");
                        if (string.IsNullOrWhiteSpace(envVars[0])) throw new Exception("Error parsing environment variable");

                        //Save path without fileTarget
                        string pathWithoutTarget = envVars[0].Replace("%", string.Empty);

                        //Move current path to target path
                        Environment.CurrentDirectory = Environment.GetEnvironmentVariable(pathWithoutTarget);

                        //Replace route with fileTarget because we are in parent directory
                        element.Route = split.Last();
                    }

                    //Check possible errors
                    if (string.IsNullOrWhiteSpace(element.Route)) throw new Exception("Route path" + element.Route + "is not support");

                    //Create if is not a program and not exist
                    if (fileLoaded.CreateFileIfNotExist &&
                        !element.Route.EndsWith(".exe") &&
                        !File.Exists(element.Route))
                    {
                        File.Open(element.Route, FileMode.CreateNew).Close();
                    }

                    //And finally can start the process
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = element.Route,
                            RedirectStandardOutput = false,
                            RedirectStandardError = false,
                            CreateNoWindow = true,
                            RedirectStandardInput = false,
                            UseShellExecute = true
                        }
                    };

                    process.Start();
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        private static void CreateNewConfigFile(string filepath)
        {
            ConfigFile newConfigFile = new ConfigFile()
            {
                MapsKeys = new[]
                {
                    new ConfigElementMapped() {Key = "1", Route = "notepad.exe", Description = "Notepad"},
                    new ConfigElementMapped() {Key = "2", Route = "cmd.exe", Description = "Command Prompt"},
                    new ConfigElementMapped() {Key = "3", Route = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe", Description = "Chrome"},
                    new ConfigElementMapped() {Key = "4", Route = "%UserProfile%\\fastnotes.txt", Description = "Fast Notes (create a new file if doesn't exist)"}
                },
                Default = new ConfigElement() { Route = "explorer.exe"},
                CreateFileIfNotExist = true
            };

            try
            {
                string serializeConfig = JsonConvert.SerializeObject(newConfigFile, Formatting.Indented);

                using var writer = new StreamWriter(filepath, true);
                writer.WriteLine(serializeConfig);
                writer.Close();
                Console.WriteLine("NEW CONFIG GENERATED SUCCESSFULLY. PLEASE CHECK AND CONFIGURE WITH YOUR OWN PREFERENCES IN: {0} ", filepath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void GenerateOptions(ConfigFile fileLoaded)
        {
            if (fileLoaded == null ||
                fileLoaded.MapsKeys == null ||
                fileLoaded.Default == null)
            {
                const string message = "Please check config file. If problem continue you can delete and will be auto-generated a new one.";
                Console.WriteLine(message);
                throw new Exception(message);
            }
            foreach (var element in fileLoaded.MapsKeys)
            {
                Console.WriteLine(element.Key + " - " + element.Description);
                //Numeric values (0-9) needed to be parsed to "D(0-9)" format
                if (int.TryParse(element.Key, out int keyNumberParsed))
                {
                    if (keyNumberParsed < 0 || keyNumberParsed > 9)
                    {
                        const string message = "Number error. Only 0-9 keys are supported";
                        Console.WriteLine(message);
                        throw new Exception(message);
                    }
                    element.Key = "D" + keyNumberParsed;
                }

            }
        }
    }
}
