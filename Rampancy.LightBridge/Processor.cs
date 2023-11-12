using RampantC20;
using RampantC20.Halo1;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
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
            if (tagInfo.TagType == "shader_environment")
            {
                var shader = new ShaderEnvironmentTag();
                shader.Read(tagInfo);

                if (!string.IsNullOrEmpty(shader.BaseMap.Path))
                {
                    var baseMapTexTagInfo = AssetDb.FindTagByRelPath(shader.BaseMap.Path, "bitmap");
                    if (baseMapTexTagInfo != null)
                    {
                        var shaderPath = Path.Combine(Profile.OutputPath, "textures", $"{tagInfo.PathNoExt}.png");
                        TextureQueue.AddItem(new TextureImportData(baseMapTexTagInfo, shaderPath));
                        importedAsset.AddRef(shader.BaseMap.Path, "bitmap");
                    }

                    //WriteQuake3Shader($"{shaderPath}.shader", tagInfo.PathNoExt, shader.BaseMap.Path);
                }
            }

            ImportedAssetDb.Add(importedAsset, tagInfo.TagType);
        }

        private void Halo1_ProcessTexture(TextureImportData taskInfo)
        {
            ImportedAssetDb.Add(taskInfo.TextureTag.PathNoExt, taskInfo.TextureTag.TagType);

            var texData = Utils.GetColorPlateFromBitMap(taskInfo.TextureTag).Value;
            using (var bitmap = BitmapFromRGBAArray(texData.width, texData.height, texData.pixels))
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

        private unsafe Bitmap BitmapFromRGBAArray(int width, int height, byte[] values)
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
                    p[3] = values[idx++];// Alpha component.
                    p += 4;
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
    }
}
