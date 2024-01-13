using RampantC20.Halo1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RampantC20.Halo1
{
    public class ShaderTransparentGlassTag : ShaderTag
    {
        public TagRef BGTintMap;
        public TagRef ReflectionMap;
        public TagRef BumpMap;
        public TagRef DiffuseMap;
        public TagRef DiffuseDetialMap;
        public TagRef SpecularMap;
        public TagRef SpecularDetailMap;

        public override BinaryReader Read(AssetDb.TagInfo tagInfo)
        {
            var br = base.Read(tagInfo);

            br.BaseStream.Seek(62, SeekOrigin.Current);
            BGTintMap = new(br);

            br.BaseStream.Seek(56, SeekOrigin.Current);
            ReflectionMap = new(br);

            br.BaseStream.Seek(4, SeekOrigin.Current);
            BumpMap = new(br);

            br.BaseStream.Seek(136, SeekOrigin.Current);
            DiffuseMap = new(br);

            br.BaseStream.Seek(4, SeekOrigin.Current);
            DiffuseDetialMap = new(br);

            br.BaseStream.Seek(36, SeekOrigin.Current);
            SpecularMap = new(br);

            br.BaseStream.Seek(4, SeekOrigin.Current);
            SpecularDetailMap = new(br);

            // Now skip to the end of the tag file for the path strings
            br.BaseStream.Seek(544, SeekOrigin.Begin);

            if (BGTintMap.PathLen > 0)
                BGTintMap.ReadTagPath(br);

            if (ReflectionMap.PathLen > 0)
                ReflectionMap.ReadTagPath(br);

            if (BumpMap.PathLen > 0)
                BumpMap.ReadTagPath(br);

            if (DiffuseMap.PathLen > 0)
                DiffuseMap.ReadTagPath(br);

            if (DiffuseDetialMap.PathLen > 0)
                DiffuseDetialMap.ReadTagPath(br);

            if (SpecularMap.PathLen > 0)
                SpecularMap.ReadTagPath(br);

            if (SpecularDetailMap.PathLen > 0)
                SpecularDetailMap.ReadTagPath(br);

            return br;
        }
    }
}
