using RampantC20.Halo1;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rampancy.LightBridge
{
    public static class Data
    {
        public const string DirName      = "RampancyLightBridge";
        public const string ProfilesDir  = $"{DirName}/Profiles";
        public const string LogsDir      = $"{DirName}/Logs";
        public const string CompilersDir = $"{DirName}/Compilers";

        public static List<Profile> DefaultProfiles = new List<Profile>()
        {
            new Profile()
            {
                Name       = "Halo CE (MCC) - Trenchbroom",
                Game       = Game.HaloCEMCC,
                Editor     = OutPutEditor.TrenchBroom,
                Compiler   = Compiler.EricwQ2,
                ShaderTags = new string[] { "shader_environment", "shader_transparent_glass" }
            }
        };

        public static List<Profile> LoadProfiles()
        {
            Directory.CreateDirectory(ProfilesDir);
            Directory.CreateDirectory(CompilersDir);

            var files    = Directory.GetFiles(ProfilesDir, "*.json");
            var profiles = new List<Profile>(files.Length);
            foreach (var file in files)
            {
                try
                {
                    var profile = Profile.Load(file);
                    profiles.Add(profile);
                }
                catch (Exception)
                {
                    Log.Error("Error laoding profile {Path}", file);
                }
            }

            // Check if missing default profiles
            foreach (var defaults in DefaultProfiles)
            {
                if (profiles.FirstOrDefault(x => x.Name == defaults.Name) == null)
                {
                    Log.Information("Missing default profile ({ProfileName}), creating", defaults.Name);
                    profiles.Add(defaults);
                    defaults.Save(ProfilesDir);
                }
            }

            return profiles;
        }
    }

    public enum Game
    {
        HaloCE,
        HaloCEMCC
    }

    public enum OutPutEditor
    {
        TrenchBroom
    }

    public enum Compiler
    {
        None,
        EricwQ2
    }
}
