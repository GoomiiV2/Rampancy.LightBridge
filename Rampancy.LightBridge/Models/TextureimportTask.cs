using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RampantC20.AssetDb;

namespace Rampancy.LightBridge
{
    public class TextureImportData
    {
        public TagInfo TextureTag;
        public string DestPath;
        public bool KeepAlpha;

        public TextureImportData(TagInfo textureTag, string destPath, bool keepAlpha)
        {
            TextureTag     = textureTag;
            DestPath       = destPath;
            KeepAlpha      = keepAlpha;
        }
    }
}
