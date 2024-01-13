using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonConverterAttribute = Newtonsoft.Json.JsonConverterAttribute;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace Rampancy.LightBridge
{
    public class Profile
    {
        public string Name;
        [JsonConverter(typeof(StringEnumConverter))]
        public Game Game;
        [JsonConverter(typeof(StringEnumConverter))]
        public OutPutEditor Editor;
        [JsonConverter(typeof(StringEnumConverter))]
        public Compiler Compiler;
        public string HEKPath;
        public string OutputPath;
        public string[] ShaderTags;

        [JsonIgnore] public string DataDir         => Path.Combine(HEKPath, "data");
        [JsonIgnore] public string TagsDir         => Path.Combine(HEKPath, "tags");
        [JsonIgnore] public string AssetDB         => Path.Combine(OutputPath, "AssetDB.json");
        [JsonIgnore] public string ImportedAssetDB => Path.Combine(OutputPath, "ImportedAssetDB.json");
        [JsonIgnore] public string MapSrcDir       => Path.Combine(OutputPath, "maps");

        public void Save(string path)
        {
            var fullPath = Path.Combine(path, $"{Name}.json");
            var json     = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(fullPath, json);

            Log.Information("Saved profile ({ProfileName}) at {Path}", Name, fullPath);
        }

        public static Profile Load(string path)
        {
            var jsonTxt = File.ReadAllText(path);
            var profile = JsonConvert.DeserializeObject<Profile>(jsonTxt);
            return profile;
        }

        public Profile Copy()
        {
            var newProfile = new Profile()
            {
                Name       = Name,
                Game       = Game,
                Editor     = Editor,
                Compiler   = Compiler,
                HEKPath    = HEKPath,
                OutputPath = OutputPath,
                ShaderTags = ShaderTags
            };

            return newProfile;
        }

        // Check if this profile is ok to use
        public ProfileValidState IsValid()
        {
            var state = ProfileValidState.Valid;

            if (string.IsNullOrEmpty(Name)) 
                state |= ProfileValidState.NoName;
            if (string.IsNullOrEmpty(HEKPath))
                state |= ProfileValidState.NoHEKPath;
            if (string.IsNullOrEmpty(OutputPath))
                state |= ProfileValidState.NoOutputPath;

            return state;
        }
    }

    [Flags]
    public enum ProfileValidState
    {
        Valid        = 0,
        NoName       = 1,
        NoHEKPath    = 2,
        NoOutputPath = 4
    }
}
