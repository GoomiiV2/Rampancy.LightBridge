using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static RampantC20.AssetDb;

namespace Rampancy.LightBridge
{
    public class MapPreProcessor
    {
        public static PreProcessResult Preprocess(string path, Profile profile)
        {
            var result = new PreProcessResult()
            {
                InMapFilepath  = path,
                OutMapFilepath = $"{path}.processed.map",
                TexInfos       = new Dictionary<string, TexInfo>()
            };

            var sb = new StringBuilder();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                // this isn't a good way to do it but its a fast way :>
                var trimmedLine = line.Trim(' ');
                if (trimmedLine.StartsWith('('))
                {
                    var start   = trimmedLine.LastIndexOf(')') + 1;
                    var end     = trimmedLine.IndexOf('[');
                    var texName = trimmedLine.Substring(start, end - start).Trim(' ', '"');

                    if (Path.GetFileNameWithoutExtension(texName).ToLower() == "skip")
                    {
                        sb.AppendLine(line.Replace(texName, "skip"));
                    }
                    else if (Path.GetFileNameWithoutExtension(texName).ToLower() == "hintskip")
                    {
                        sb.AppendLine(line.Replace(texName, "hintskip"));
                    }
                    else
                    {
                        var texNameHash = $"{texName.GetHashCode()}";
                        if (!result.TexInfos.ContainsKey(texNameHash))
                        {
                            var texInfo = new TexInfo()
                            {
                                TexturePath = texName,
                                Width = 256,
                                Height = 256
                            };

                            var fullTexPath = Path.Combine(profile.OutputPath, "textures", $"{texName}.png");
                            if (File.Exists(fullTexPath))
                            {
                                using var image = System.Drawing.Image.FromFile(fullTexPath);
                                texInfo.Width = image.Width;
                                texInfo.Height = image.Height;
                            }

                            result.TexInfos.Add(texNameHash, texInfo);
                        }

                        sb.AppendLine(line.Replace(texName, $"{texNameHash}"));
                    }


                    /*
                    var faceDataStr   = trimmedLine.Substring(trimmedLine.LastIndexOf(']') + 2);
                    var faceDataSplit = faceDataStr.Split(' ');
                    if (faceDataSplit.Length == 6)
                    {
                        var surContentsFaceValue = faceDataSplit.AsSpan()[^3..];
                        Log.Information("{surContentsFaceValue}", surContentsFaceValue.ToArray());
                    }
                    */
                }
                else if(trimmedLine.StartsWith("\"classname\""))
                {
                    var className = trimmedLine.Split(' ')[1].Trim('"');
                    var replaceWith = className.ToLower() switch
                    {
                        "func_exact_portal" => "func_detail_fence",
                        "func_portal"       => "func_detail_wall",
                        "func_glass"        => "func_detail_wall",
                        _                   => null
                    };

                    if (replaceWith != null)
                    {
                        sb.AppendLine(line.Replace(className, replaceWith));
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            //File.Copy(result.InMapFilepath, result.OutMapFilepath, true);
            File.WriteAllText(result.OutMapFilepath, sb.ToString());

            return result;
        }

        public class TexInfo
        {
            public string TexturePath;
            public int Width;
            public int Height;
        }

        public class PreProcessResult
        {
            public string InMapFilepath;
            public string OutMapFilepath;
            public Dictionary<string, TexInfo> TexInfos;
        }
    }
}
