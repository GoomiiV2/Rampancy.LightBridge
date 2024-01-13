using Rampancy.LightBridge.Properties;
using RampantC20;
using RampantC20.Halo1;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Rampancy.ImportedAssetDb;
using static RampantC20.AssetDb;

namespace Rampancy.LightBridge
{
    public class Processor
    {
        private Profile Profile;
        private AssetDb AssetDb;
        private ImportedAssetDb ImportedAssetDb;

        private WorkQueue<TagInfo> ShaderQueue;
        private WorkQueue<TextureImportData> TextureQueue;

        public bool IsSyncing { get; private set; }
        public Action OnSyncingStarted;
        public Action OnSyncingDone;

        public Processor()
        {

        }

        public void SetProfile(Profile profile)
        {
            Profile = profile;

            ShaderQueue  = new(ProcessShader);
            TextureQueue = new(ProcessTexture);

            AssetDb = new AssetDb(profile.HEKPath);
            AssetDb.OnTagChanged += OnTagChanged;

            ImportedAssetDb = new ImportedAssetDb(profile.ImportedAssetDB);
            ImportedAssetDb.Load();
        }

        public void Sync(bool forceResync)
        {
            var sw = Stopwatch.StartNew();

            OnSyncingStarted?.Invoke();
            IsSyncing = true;
            AssetDb.CheckForChanges(Profile.AssetDB, forceResync);

            if (forceResync)
                ImportedAssetDb.Clear();

            var task = Task.Factory.StartNew(() =>
            {
                WaitTillAllTasksAreFinished();
                CopyDemoMap();
                CopyToolTextures();
                IsSyncing = false;
                OnSyncingDone?.Invoke();

                sw.Stop();
                Log.Information("{SyncType} took: {SyncDuration}", forceResync ? "Full Resync" : "Sync", sw.Elapsed);
            });
        }

        public int GetPendingJobCount()
        {
            int count = 0;
            count    += ShaderQueue.GetPendingItemsCount();
            count    += TextureQueue.GetPendingItemsCount();

            return count;
        } 

        private void WaitTillAllTasksAreFinished()
        {
            while (!ShaderQueue.IsIdle() || !TextureQueue.IsIdle())
            {
                Thread.Sleep(100);
            }
        }

        private void OnTagChanged(AssetDb.TagChangedType change, TagInfo tagInfo)
        {
            if (change is TagChangedType.Added or TagChangedType.Changed)
            {
                if (Profile.ShaderTags.Contains(tagInfo.TagType))
                {
                    ShaderQueue.AddItem(tagInfo);
                }
            }
            else if (change == TagChangedType.Deleted)
            {

            }
        }

        private void ProcessShader(TagInfo tagInfo)
        {
            if (ImportedAssetDb.IsImported(tagInfo.PathNoExt, tagInfo.TagType))
                return;

            Log.Information("Processing shader {ShaderName}", tagInfo.Name);
            if (Profile.Game == Game.HaloCEMCC || Profile.Game == Game.HaloCE)
                Halo1_ProcessShader(tagInfo);
        }

        private void ProcessTexture(TextureImportData taskInfo)
        {
            //if (ImportedAssetDb.IsImported(taskInfo.TextureTag.PathNoExt, taskInfo.TextureTag.TagType))
                //return;

            if (Profile.Game is Game.HaloCEMCC or Game.HaloCE)
                Halo1_ProcessTexture(taskInfo);
        }

        private void Halo1_ProcessShader(TagInfo tagInfo)
        {
            var importedAsset = new ImportedAsset(tagInfo.PathNoExt);
            var diffusePath = "";
            bool keepAlpha = false;
            if (tagInfo.TagType == "shader_environment")
            {
                var shader = new ShaderEnvironmentTag();
                shader.Read(tagInfo);
                diffusePath = shader.BaseMap.Path;
                keepAlpha = false; //((shader.ShaderFlags & 0x1) != 0); // Is alpha tested flag
            }
            else if (tagInfo.TagType == "shader_transparent_glass")
            {
                var shader = new ShaderTransparentGlassTag();
                shader.Read(tagInfo);
                diffusePath = shader.DiffuseMap.Path;
                //keepAlpha   = ((shader.ShaderFlags & 0x1) != 0); // Is alpha tested flag
            }

            if (!string.IsNullOrEmpty(diffusePath))
            {
                var baseMapTexTagInfo = AssetDb.FindTagByRelPath(diffusePath, "bitmap");
                if (baseMapTexTagInfo != null)
                {
                    var shaderPath = Path.Combine(Profile.OutputPath, "textures", $"{tagInfo.PathNoExt}.png");
                    TextureQueue.AddItem(new TextureImportData(baseMapTexTagInfo, shaderPath, keepAlpha));
                    importedAsset.AddRef(diffusePath, "bitmap");
                }

                //WriteQuake3Shader($"{shaderPath}.shader", tagInfo.PathNoExt, shader.BaseMap.Path);
            }

            ImportedAssetDb.Add(importedAsset, tagInfo.TagType);
        }

        private void Halo1_ProcessTexture(TextureImportData taskInfo)
        {
            ImportedAssetDb.Add(taskInfo.TextureTag.PathNoExt, taskInfo.TextureTag.TagType);

            var noAlpha = !taskInfo.KeepAlpha;
            var texData = Utils.GetColorPlateFromBitMap(taskInfo.TextureTag).Value;
            using (var bitmap = BitmapFromRGBAArray(texData.width, texData.height, texData.pixels, noAlpha))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(taskInfo.DestPath));
                try
                {
                    var bitmap2 = new Bitmap(bitmap);
                    bitmap2.Save(taskInfo.DestPath, ImageFormat.Png);
                    Log.Information("Saved texture {BitmapPath}", taskInfo.TextureTag.PathNoExt);
                }
                catch (Exception ex)
                {
                    Log.Information("Error saving texture {BitmapPath}", taskInfo.TextureTag.PathNoExt);
                }
            }
        }

        private unsafe Bitmap BitmapFromRGBAArray(int width, int height, byte[] values, bool noAlpha = false)
        {
            Bitmap bmp        = new Bitmap(width, height);
            BitmapData bmData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            IntPtr scan0      = bmData.Scan0;
            int stride        = bmData.Stride;
            int nWidth        = bmp.Width;
            int nHeight       = bmp.Height;

            int idx = 0;
            for (int y = 0; y < nHeight; y++)
            {
                //define the pointers inside the first loop for parallelizing
                byte* p = (byte*)scan0.ToPointer();
                p += y * stride;

                for (int x = 0; x < nWidth; x++)
                {
                    p[0] = values[idx++];// R component.
                    p[1] = values[idx++];// G component.
                    p[2] = values[idx++];// B component.
                    p[3] = noAlpha ? (byte)255 : values[idx++];// Alpha component.
                    p += 4;

                    if (noAlpha)
                        idx++;
                }
            }

            bmp.UnlockBits(bmData);
            return bmp;
        }

        private void WriteQuake3Shader(string path, string shaderPath, string diffuseTex)
        {
            var shaderTemplate = @$"{shaderPath}
{{
    qer_editorimage {diffuseTex}.png
}}";

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, shaderTemplate);
        }

        private void CopyDemoMap()
        {
            var basePath = Path.Combine(Profile.OutputPath, "maps");

            if (Profile.Editor == OutPutEditor.TrenchBroom && Profile.Game is Game.HaloCEMCC or Game.HaloCE)
            {
                SaveEmbedResourceToDisk("Rampancy.LightBridge.Assets.Trenchbroom.Halo1.maps.LB.Demo.models.demo.map",
                            Path.Combine(basePath, "LB/Demo/models/demo.map"));
            }
        }

        // Copy some tool textures used in the editors only that might not be with the gmaes
        private void CopyToolTextures()
        {
            if (Profile.Game is Game.HaloCE or Game.HaloCEMCC)
            {
                CopyHalo1ToolTextures();
            }
        }

        private void CopyHalo1ToolTextures()
        {
            var basePath     = Path.Combine(Profile.OutputPath, "textures", "tooltex");
            var skyEmbedPath = "Rampancy.LightBridge.Assets.tooltex.Sky.png";
            SaveEmbedResourceToDisk(skyEmbedPath, Path.Combine(basePath, "+sky.png"));
            SaveEmbedResourceToDisk(skyEmbedPath, Path.Combine(basePath, "+sky0.png"));
            SaveEmbedResourceToDisk(skyEmbedPath, Path.Combine(basePath, "+sky1.png"));
            SaveEmbedResourceToDisk(skyEmbedPath, Path.Combine(basePath, "+sky2.png"));
            SaveEmbedResourceToDisk(skyEmbedPath, Path.Combine(basePath, "+sky3.png"));

            SaveEmbedResourceToDisk("Rampancy.LightBridge.Assets.tooltex.Portal.png", Path.Combine(basePath, "+portal.png"));
            SaveEmbedResourceToDisk("Rampancy.LightBridge.Assets.tooltex.Exact Portal.png", Path.Combine(basePath, "+exactportal.png"));
            SaveEmbedResourceToDisk("Rampancy.LightBridge.Assets.tooltex.Weather Poly.png", Path.Combine(basePath, "+weatherpoly.png"));
            SaveEmbedResourceToDisk("Rampancy.LightBridge.Assets.tooltex.Sound.png", Path.Combine(basePath, "+sound.png"));

            SaveEmbedResourceToDisk("Rampancy.LightBridge.Assets.tooltex.Skip.png", Path.Combine(basePath, "skip.png"));
            SaveEmbedResourceToDisk("Rampancy.LightBridge.Assets.tooltex.Skip.png", Path.Combine(basePath, "hintskip.png"));
        }

        private void SaveEmbedResourceToDisk(string embedPath, string diskPath)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(embedPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(diskPath));
                using (var file = new FileStream(diskPath, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }
    }
}
