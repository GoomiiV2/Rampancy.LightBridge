using RampantC20;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Rampancy.LightBridge
{
    public class Quake2BSP
    {
        public Vector3[] Verts;
        public Edge[] Edges;
        public TexInfo[] TexInfos;
        public Face[] Faces;
        public Model[] Models;
        public int[] FaceEdges;
        public Plane[] Planes;

        public Quake2BSP(string path)
        {
            var data   = File.ReadAllBytes(path).AsSpan();
            var header = MemoryMarshal.Cast<byte, Header>(data)[0];

            Verts     = header.Vertices.GetAs<Vector3>(data).ToArray();
            Edges     = header.Edges.GetAs<Edge>(data).ToArray();
            TexInfos  = header.TexInfo.GetAs<TexInfo>(data).ToArray();
            Faces     = header.Faces.GetAs<Face>(data).ToArray();
            Models    = header.Models.GetAs<Model>(data).ToArray();
            FaceEdges = header.FaceEdgeTable.GetAs<int>(data).ToArray();
            Planes    = header.Planes.GetAs<Plane>(data).ToArray();
        }

        public GenericModel ToModel(MapPreProcessor.PreProcessResult mapInfo)
        {
            var model          = new GenericModel();
            model.VertPositons = new List<Vector3>(Verts.Length);

            // Meshes based on texture and props
            var matMeshes = new Dictionary<string, (List<uint> Indices, string TexName, uint Flags, uint Value)>();
            for (int i = 0; i < TexInfos.Length; i++)
            {
                var id = GetMatIdFromTexInfo(TexInfos[i]);
                matMeshes.TryAdd(id, (new List<uint>(), TexInfos[i].GetNameStr(), TexInfos[i].Flags, TexInfos[i].Value));
            }

            foreach (var face in Faces)
            {
                var texInfo = TexInfos[face.TexInfoIdx];
                var edge    = Math.Abs(FaceEdges[face.FirstEdgeIdx]);
                for (int i = 1; i < face.NumEdges - 1; i++)
                {
                    var edge1 = Math.Abs(FaceEdges[face.FirstEdgeIdx + i]);
                    var edge2 = Math.Abs(FaceEdges[face.FirstEdgeIdx + i + 1]);

                    var vert1 = Verts[FaceEdges[face.FirstEdgeIdx] < 0 ? Edges[edge].End : Edges[edge].Start];
                    var vert2 = Verts[FaceEdges[face.FirstEdgeIdx + i + 1] < 0 ? Edges[edge2].End : Edges[edge2].Start];
                    var vert3 = Verts[FaceEdges[face.FirstEdgeIdx + i] < 0 ? Edges[edge1].End : Edges[edge1].Start];

                    var uvs1 = GetUvs(mapInfo, texInfo, vert1);
                    var uvs2 = GetUvs(mapInfo, texInfo, vert2);
                    var uvs3 = GetUvs(mapInfo, texInfo, vert3);

                    var norm = Utils.GetTriNormal(vert1, vert2, vert3);

                    var indice1 = model.AddVert(vert1, norm, uvs1);
                    var indice2 = model.AddVert(vert2, norm, uvs2);
                    var indice3 = model.AddVert(vert3, norm, uvs3);

                    var matId = GetMatIdFromTexInfo(texInfo);
                    var matIndices = matMeshes[matId];
                    matIndices.Indices.Add(indice1);
                    matIndices.Indices.Add(indice2);
                    matIndices.Indices.Add(indice3);
                }
            }

            foreach (var matMesh in matMeshes.Values)
            {
                mapInfo.TexInfos.TryGetValue(matMesh.TexName, out var texPath);
                var mesh     = new GenricMesh();
                mesh.Start   = model.Indices.Count;
                mesh.Length  = matMesh.Indices.Count;
                mesh.Texture = texPath?.TexturePath ?? "";
                mesh.Flags   = matMesh.Flags;
                mesh.Value   = matMesh.Value;
                model.Indices.AddRange(matMesh.Indices);  

                model.Meshes.Add(mesh);
            }

            return model;
        }

        private Vector2 GetUvs(MapPreProcessor.PreProcessResult mapInfo, TexInfo texInfo, Vector3 vert)
        {
            if (mapInfo.TexInfos.TryGetValue(texInfo.GetNameStr(), out var texSize))
            {
                var u = (Vector3.Dot(vert, texInfo.UAxis) + texInfo.UOfset) / texSize.Width;
                var v = (Vector3.Dot(vert, texInfo.VAxis) + texInfo.VOfset) / texSize.Height;

                return new Vector2(1 - u, 1 - v);
            }
            else
            {
                var u = (Vector3.Dot(vert, texInfo.UAxis) + texInfo.UOfset) / 256;
                var v = (Vector3.Dot(vert, texInfo.VAxis) + texInfo.VOfset) / 256;
                return new Vector2((int)u, (int)v);
            }
        }

        private string GetMatIdFromTexInfo(TexInfo texInfo)
        {
            var id = $"{texInfo.GetNameStr()}{texInfo.Flags}{texInfo.Value}";
            return id;
        }

        public struct Header
        {
            public uint Magic;
            public uint Version;
            public LumpInfo Entities;
            public LumpInfo Planes;
            public LumpInfo Vertices;
            public LumpInfo Visibility;
            public LumpInfo Nodes;
            public LumpInfo TexInfo;
            public LumpInfo Faces;
            public LumpInfo Lightmaps;
            public LumpInfo Leaves;
            public LumpInfo LeafFaceTable;
            public LumpInfo LeafBrushTable;
            public LumpInfo Edges;
            public LumpInfo FaceEdgeTable;
            public LumpInfo Models;
            public LumpInfo Brushes;
            public LumpInfo BrushSides;
            public LumpInfo Pop;
            public LumpInfo Areas;
            public LumpInfo AreaPortals;
        }
        public struct LumpInfo
        {
            public int Offset;
            public int Length;
            public Span<byte> GetData(Span<byte> data)                => data.Slice(Offset, Length);
            public Span<T> GetAs<T>(Span<byte> data) where T : struct => MemoryMarshal.Cast<byte, T>(GetData(data));
        }
        public struct Edge
        {
            public short Start;
            public short End;
        }

        public unsafe struct TexInfo
        {
            public Vector3 UAxis;
            public float UOfset;
            public Vector3 VAxis;
            public float VOfset;
            public uint Flags;
            public uint Value;
            public fixed byte Name[32];
            public uint NextTexInfo;

            public string GetNameStr()
            {
                fixed (byte* namePtr = Name)
                {
                    return Encoding.UTF8.GetString(namePtr, 32).Trim('\0');
                }
            }
        }

        public struct Face
        {
            public ushort PlaneIdx;
            public ushort PlaneSideIdx;
            public uint FirstEdgeIdx;
            public ushort NumEdges;
            public ushort TexInfoIdx;
            public byte LightmapStyles1;
            public byte LightmapStyles2;
            public byte LightmapStyles3;
            public byte LightmapStyles4;
            public uint LightmapIdx;
        }

        public struct Model
        {
            public Vector3 Min;
            public Vector3 Max;
            public Vector3 Origin;
            public int HeadNodeIdx; 
            public int FirstFaceIdx; 
            public int NumFaces; 
        }

        public struct Plane
        {
            public Vector3 Normal;
            public float Distance;
            public uint PlaneType;
        }
    }
}
