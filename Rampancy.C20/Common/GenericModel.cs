using RampantC20.Halo1;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RampantC20
{
    public class GenericModel
    {
        public string Name;

        public List<Vector3> VertPositons = new ();
        public List<Vector3> VertNormlas  = new ();
        public List<Vector2> VertUvs      = new ();
        public List<uint> Indices         = new ();
        public List<GenricMesh> Meshes    = new ();

        public uint AddVert(Vector3 pos, Vector3? normal = null, Vector2? uv = null)
        {
            VertPositons.Add(pos);
            VertNormlas.Add(normal ?? Vector3.One);
            VertUvs.Add(uv ?? Vector2.Zero);

            return (uint)VertPositons.Count - 1;
        }

        public void SaveObj(string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("o World");

            sb.AppendLine();
            sb.AppendLine("# Positons");
            foreach (var pos in VertPositons)
                sb.AppendLine($"v {pos.X} {pos.Y} {pos.Z}");

            sb.AppendLine();
            sb.AppendLine("# Normals");
            foreach (var norm in VertNormlas)
                sb.AppendLine($"vn {norm.X} {norm.Y} {norm.Z}");

            sb.AppendLine();
            sb.AppendLine("# Uvs");
            foreach (var uv in VertUvs)
                sb.AppendLine($"vt {uv.X} {uv.Y}");

            sb.AppendLine();
            sb.AppendLine("# Faces");
            foreach (var mesh in Meshes)
            {
                sb.AppendLine($"# {mesh.Texture} Flags: {mesh.Flags} Value: {mesh.Value}");
                sb.AppendLine($"g {mesh.Texture}");
                sb.AppendLine($"usemtl {mesh.Id}");
                for (var i = 0; i < mesh.Length; i += 3)
                {
                    sb.AppendLine($"f {Indices[mesh.Start + i] + 1} {Indices[mesh.Start + (i + 1)] + 1} {Indices[mesh.Start + (i + 2)] + 1}");
                }
            }

            File.WriteAllText(path, sb.ToString());
        }

        public void SaveJMS(string path)
        {
            var jmsModel = new JMS();
            jmsModel.Nodes.Add(new JMS.Node
            {
                Name       = "frame",
                ChildIdx   = -1,
                SiblingIdx = -1,
                Rotation   = Quaternion.Identity,
                Position   = new Vector3(0, 0, 0)
            });

            jmsModel.Regions.Add(new JMS.Region
            {
                Name = "unnamed"
            });

            foreach (var mesh in Meshes)
            {
                jmsModel.Materials.Add(new JMS.Material
                {
                    Name = Path.GetFileName(mesh.Texture).TrimEnd('"') + Utils.Halo1MatFlagsToString(mesh.Flags),
                    Path = "<none>"
                });
            }

            bool hasUvs     = VertUvs.Count > 0;
            bool hasNormals = VertNormlas.Count > 0;
            float scale     = 1f;
            for (var i = 0; i < VertPositons.Count; i++)
            {
                var vert = new JMS.Vert
                {
                    Position   = VertPositons[i] * scale,
                    UV         = hasUvs ? VertUvs[i] * new Vector2(-1, 1) : Vector2.Zero,
                    Normal     = hasNormals ? new Vector3(VertNormlas[i].X, VertNormlas[i].Z, VertNormlas[i].Y) : Vector3.One,
                    NodeIdx    = 0,
                    NodeWeight = 0f
                };

                jmsModel.Verts.Add(vert);
            }

            for (var i = 0; i < Meshes.Count; i++)
            {
                var mesh = Meshes[i];
                for (var idx = 0; idx < mesh.Length; idx += 3)
                {
                    var tri = new JMS.Tri
                    {
                        RegionIdx   = 0,
                        MaterialIdx = i,
                        VertIdx0    = (int)Indices[mesh.Start + idx],
                        VertIdx1    = (int)Indices[mesh.Start + idx + 1],
                        VertIdx2    = (int)Indices[mesh.Start + idx + 2]
                    };

                    jmsModel.Tris.Add(tri);
                }
            }

            jmsModel.Save(path);
        }
    }

    public class GenricMesh
    {
        public int Id;
        public int Start;
        public int Length;
        public string Texture;
        public uint Flags;
        public float Value;
    }
}
