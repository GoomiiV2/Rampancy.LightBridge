using RampantC20;
using Serilog;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Rampancy.LightBridge
{
    public static class CommandLine
    {

        public static void Run(string[] args)
        {
            SetupLogging();
            Log.Information("{Name} Commandline v{Version}", Data.DirName, ThisAssembly.AssemblyInformationalVersion);
            Log.Information("Args: {Args}", args);

            var rootCmd = new RootCommand(Data.DirName);

            var convertCmd = CreateConvertCommand();
            rootCmd.AddCommand(convertCmd);

            var result = rootCmd.Invoke(args);
            Log.Information($"Result: {result}");
        }

        private static void SetupLogging()
        {
            var latestLogPath = Path.Combine(Data.LogsDir, "latest-commandline.txt");
            File.Delete(latestLogPath);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(Data.LogsDir, "log-commandline.txt"), rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm:ss} [{Category}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(latestLogPath)
                .WriteTo.Console()
                .CreateLogger();
        }

        private static Command CreateConvertCommand()
        {
            var srcOpt           = new Option<string>("--src");
            var profileOpt       = new Option<string>("--profile");
            var structureOpt     = new Option<bool>("--structure");
            var lightOpt         = new Option<float?>("--light");
            var cmd = new Command("compile", "Compile a file to a format that tool can consume")
            {
                srcOpt,
                profileOpt,
                structureOpt,
                lightOpt
            };

            cmd.SetHandler((src, profileName, structureOpt, lightOpt) =>
            {
                Log.Information($"{src}, {profileName}");
                var profile = LoadProfile(profileName);
                if (profile == null)
                    LogFatalError($"Couldn't find profile with name {profileName} at {Path.Combine(Environment.CurrentDirectory, Data.ProfilesDir)}");

                if (profile.Editor == OutPutEditor.TrenchBroom)
                {
                    var result = MapPreProcessor.Preprocess(src, profile);

                    if (profile.Compiler == Compiler.EricwQ2)
                        CompileWithEricwQ2(result.OutMapFilepath);

                    var bspPath = Path.ChangeExtension(result.OutMapFilepath, "bsp");
                    var bsp     = new Quake2BSP(bspPath);
                    var model   = bsp.ToModel(result);

                    var wingedMesh = new WingedMesh();
                    wingedMesh.FromModel(model);
                    var tJunctions = wingedMesh.FindTJunctions();
                    wingedMesh.FixTJunctions(tJunctions);
                    var newModel = wingedMesh.ToModel();
                    foreach (var mesh in newModel.Meshes)
                    {
                        mesh.Texture = model.Meshes[mesh.Id].Texture;
                        mesh.Flags   = model.Meshes[mesh.Id].Flags;
                    }

                    Log.Information("Model Verts before {VertsBefore} and after {VertsAfter}, had {NumTJunctions} T-Junctions.",
                        model.VertPositons.Count(), newModel.VertPositons.Count(), tJunctions.Count);

                    if (profile.Game is Game.HaloCE or Game.HaloCEMCC)
                    {
                        var relJmsPath = Path.ChangeExtension(GetDataRelPathForMap(result.OutMapFilepath, profile), "jms");
                        var jmsPath    = Path.Combine(profile.DataDir, "levels", relJmsPath);

                        Directory.CreateDirectory(Path.GetDirectoryName(jmsPath));
                        newModel.SaveJMS(jmsPath);

                        var unprocessedPath = Path.Combine(Path.GetDirectoryName(jmsPath), $"unprocessed_{Path.GetFileName(jmsPath)}");
                        model.SaveJMS(unprocessedPath);

                        var toolPath = Path.Combine(profile.HEKPath, "tool.exe");

                        if (structureOpt)
                        {
                            var pathForStructure = Path.GetDirectoryName(relJmsPath)[0..^7];
                            RunProcess(toolPath, $"structure levels/{pathForStructure} {Path.GetFileNameWithoutExtension(relJmsPath)}", profile.HEKPath, true);
                        }

                        if (lightOpt != null)
                        {
                            RunProcess(toolPath, $"lightmaps  {relJmsPath} {Path.GetFileNameWithoutExtension(relJmsPath)} {(lightOpt.Value < 0 ? "0" : "1")} {lightOpt.Value}", profile.HEKPath, true);
                        }
                    }

                    newModel.SaveObj(Path.ChangeExtension(result.OutMapFilepath, "obj"));
                }
            }, srcOpt, profileOpt, structureOpt, lightOpt);

            return cmd;
        }

        private static Profile LoadProfile(string name)
        {
            var profiles = Data.LoadProfiles();
            var profile  = profiles.FirstOrDefault(p => p.Name == name);

            return profile;
        }

        private static void LogFatalError(string error)
        {
            Log.Fatal(error);
            MessageBox.Show(error, "Error");
        }

        private static string GetDataRelPathForMap(string path, Profile profile)
        {
            var newPath = path.Replace(".processed.map", "")
                .Replace(profile.MapSrcDir, "").TrimStart('\\');

            return newPath;
        }

        private static void CompileWithEricwQ2(string mapFile) =>
            RunProcess($"{Data.CompilersDir}/ericw/qbsp.exe", $"-nopercent -nosubdivide -q2bsp -tjunc mwt \"{mapFile}\"");

        private static void RunProcess(string path, string args, string workingDir = null, bool showWindow = true)
        {
            Process process                          = new Process();
            process.StartInfo.FileName               = path;
            process.StartInfo.Arguments              = args;
            process.StartInfo.WorkingDirectory       = workingDir;
            process.StartInfo.WindowStyle            = showWindow ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
            process.StartInfo.RedirectStandardOutput = !showWindow;
            process.StartInfo.RedirectStandardError  = !showWindow;
            process.Start();

            if (!showWindow)
                Log.Information(process.StandardOutput.ReadToEnd());

            process.WaitForExit();
        }

        private static void LogProcessOutput(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                Log.Information(outLine.Data);
            }
        }
    }
}
