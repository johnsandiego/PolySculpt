//#define SUBDIVIDEVECTOR4 // used to subdivide meshes in 4D

using System.Collections.Generic;
using UnityEngine;

//using System.Linq; -- don't use Linq for performance

namespace sandiegoJohn.VRsculpting{
    // Vector used for position, normal and tangent of a Vertex
#if !SUBDIVIDEVECTOR4
    using Vector = Vector3;
#else
    using Vector = Vector4; // to subdivide 4D meshes
#endif

    /// <summary>
    /// Temporal Vertex object extracted from (inserted to) Mesh data arrays (positions, normals, uvs,..)
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Vertex: pos {posIndex}. {position}; normal {normal}")]
    public struct Vertex {
        public int posIndex;
        public Vector position;
        public Vector normal;
        public Vector tangent;
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
    }

    /// <summary>
    /// Content of Mesh data arrays
    /// </summary>
    public enum VertexContent {
        none    = 0x00,
        pos     = 0x01,
        normal  = 0x02,
        tangent = 0x04,
        uv0     = 0x08,
        uv1     = 0x10,
        uv2     = 0x20,
        uv3     = 0x40,
        full    = 0x7F,
    }
    public static class VertexContentMethods {
        public static bool HasPosition(this VertexContent c) { return (c & VertexContent.pos)     != 0; }
        public static bool HasNormal  (this VertexContent c) { return (c & VertexContent.normal)  != 0; }
        public static bool HasTangent (this VertexContent c) { return (c & VertexContent.tangent) != 0; }
        public static bool HasUV0     (this VertexContent c) { return (c & VertexContent.uv0)     != 0; }
        public static bool HasUV1     (this VertexContent c) { return (c & VertexContent.uv1)     != 0; }
        public static bool HasUV2     (this VertexContent c) { return (c & VertexContent.uv2)     != 0; }
        public static bool HasUV3     (this VertexContent c) { return (c & VertexContent.uv3)     != 0; }
    }

    public struct Submesh {
        public int[][] faces; // [face index] => [face vertex indices]
    }

    public struct Face {
        public int submesh;
        public int index; // face index
    }
    public struct Edge {
        public Face face;
        public int index; // edge index in a face (0..3)
        //
        public static Edge Invalid = new Edge { index = -1 };
        public bool IsValid() { return index != -1; }
    }

    public enum EdgeType { // see GetEdgeType(Edge e)
        boundary = -1, // the edge has no opposite neighbor
        back     = -2, // back edge - skip it to not count the edge twice
        solid    = 0,  // front solid edge: both edge vertices belong to both faces
        seam0    = 1,  // front seam edge: faces have different uv on this edge
        seam1    = 2,
    }

    /// <summary>
    /// Extended mesh. Allows to weld vertex positions keeping different uvs. So 'positions.Count' may be less than 'vertexCount'
    /// </summary>
    public class MeshX
    {
        // by position index
        public IList<Vector> positions  = null;
        // by vertex index
        public IList<int>    posIndices = null;
        public IList<Vector> normals    = null; // may be per position: if 'normalPerPosition'
        public IList<Vector> tangents   = null;
        public IList<Vector2> uvs0      = null;
        public IList<Vector2> uvs1      = null;
        public IList<Vector2> uvs2      = null;
        public IList<Vector2> uvs3      = null;
        //
        public VertexContent content    = VertexContent.none;
        public bool normalPerPosition   = false; // normal per position or per vertex

        public string name = null;

        public bool buildMode = false;

        // faces
        public Submesh[] submeshes  = null;

        public int vertexCount { get { return posIndices == null ? -1 : posIndices.Count; } }

        // helpers
        public bool helpersInited { get { return positionVertices != null; } }
        public  int[][]     positionVertices = null; // [position index] => [vertex indices]
        public  Edge[][]    vertexEdges      = null; // [vertex index] => [face edges started from the vertex]
        private Edge[][][]  neighbors        = null; // [submesh index][face index][face edge index] => neighbor edge

        //
        public MeshX() {}

        public void Trace() {
            Debug.Log("Positions:");
            for (int i = 0; i < positions.Count; ++i) {
                Debug.LogFormat("  {0,3}: {1}", i, positions[i].ToString("0.0000"));
            }
            Debug.Log("Vertices:");
            for (int v = 0; v < vertexCount; ++v) {
                Debug.LogFormat("  {0,3}: pos {1}", v, posIndices[v]);
            }
            Debug.Log("Faces:");
            for (int s = 0; s < submeshes.Length; ++s) {
                Debug.Log(" Submesh: " + s);
                for (int i = 0; i < submeshes[s].faces.Length; ++i) {
                    string l = string.Format("  {0,3}:", i);
                    foreach (int v in submeshes[s].faces[i]) l += " " + v;
                    Debug.Log(l);
                }
            }
        }

        #region Build mode
        /// <summary>
        /// In Build mode we store Mesh data in lists (not arrays) allowing to add vertices
        /// </summary>
        public void StartBuilding() {
            if (content.HasPosition()) positions = new List<Vector>();
            if (content.HasNormal  ()) normals   = new List<Vector>();
            if (content.HasTangent ()) tangents  = new List<Vector>();
            if (content.HasUV0     ()) uvs0      = new List<Vector2>();
            if (content.HasUV1     ()) uvs1      = new List<Vector2>();
            if (content.HasUV2     ()) uvs2      = new List<Vector2>();
            if (content.HasUV3     ()) uvs3      = new List<Vector2>();
            posIndices = new List<int>();
            buildMode = true;
        }
        public void FinishBuilding() {
            if (content.HasPosition()) positions = ToArray(positions);
            if (content.HasNormal  ()) normals   = ToArray(normals);
            if (content.HasTangent ()) tangents  = ToArray(tangents);
            if (content.HasUV0     ()) uvs0      = ToArray(uvs0);
            if (content.HasUV1     ()) uvs1      = ToArray(uvs1);
            if (content.HasUV2     ()) uvs2      = ToArray(uvs2);
            if (content.HasUV3     ()) uvs3      = ToArray(uvs3);
            posIndices = ToArray(posIndices);
            buildMode = false;
        }
        private static T[] ToArray<T>(IList<T> source) {
            var array = new T[source.Count];
            source.CopyTo(array, 0);
            return array;
        }

        public int AddPosition(Vertex v) {
            Debug.Assert(buildMode, "AddPosition in buildMode only");
            positions.Add(v.position);
            if (content.HasNormal() && normalPerPosition) normals.Add(v.normal);
            return positions.Count - 1;
        }

        public int AddVertex(Vertex v, bool addPosition = false) {
            Debug.Assert(buildMode, "AddVertex in buildMode only");
            int pi = addPosition ? AddPosition(v) : v.posIndex;
            posIndices.Add(pi);
            if (content.HasNormal() && !normalPerPosition) normals.Add(v.normal);
            if (content.HasTangent()) tangents.Add(v.tangent);
            if (content.HasUV0()) uvs0.Add(v.uv0);
            if (content.HasUV1()) uvs1.Add(v.uv1);
            if (content.HasUV2()) uvs2.Add(v.uv2);
            if (content.HasUV3()) uvs3.Add(v.uv3);
            return posIndices.Count - 1;
        }
        #endregion Build mode

        // positions & vertices
        public void SetPosition(int pi, Vertex v) {
            positions[pi] = v.position;
            if (content.HasNormal() && normalPerPosition) normals[pi] = v.normal;
        }

        public void SetVertex(int vi, Vertex v, bool setPosition = false) {
            if (setPosition) SetPosition(v.posIndex, v);
            posIndices[vi] = v.posIndex;
            if (content.HasNormal() && !normalPerPosition) normals[vi] = v.normal;
            if (content.HasTangent()) tangents[vi] = v.tangent;
            if (content.HasUV0()) uvs0[vi] = v.uv0;
            if (content.HasUV1()) uvs1[vi] = v.uv1;
            if (content.HasUV2()) uvs2[vi] = v.uv2;
            if (content.HasUV3()) uvs3[vi] = v.uv3;
        }

        public Vertex GetPosition(int pi) { // position & normal (if per position) only
            Vertex v = new Vertex();
            v.position = positions[pi];
            if (content.HasNormal() && normalPerPosition) v.normal = normals[pi];
            return v;
        }

        public Vertex GetVertex(int vi, VertexContent mask = VertexContent.full) {
            VertexContent c = content & mask;
            int pi = posIndices[vi];
            Vertex v = new Vertex();
            v.posIndex = pi; // always set
            if (c.HasPosition()) v.position = positions[pi];
            if (c.HasNormal())   v.normal   = normals[normalPerPosition ? pi : vi];
            if (c.HasTangent())  v.tangent  = tangents[vi];
            if (c.HasUV0())      v.uv0      = uvs0[vi];
            if (c.HasUV1())      v.uv1      = uvs1[vi];
            if (c.HasUV2())      v.uv2      = uvs2[vi];
            if (c.HasUV3())      v.uv3      = uvs3[vi];
            return v;
        }

        public Vertex[] GetVertices(int[] indices) {
            Vertex[] vs = new Vertex[indices.Length];
            for (int i = 0; i < indices.Length; ++i) {
                vs[i] = GetVertex(indices[i]);
            }
            return vs;
        }

        public void MakeNormalPerPosition() {
            if (normalPerPosition) return;
            if (!content.HasNormal()) return;
            Vector[] newNormals = new Vector[positions.Count];
            for (int i = 0; i < positions.Count; ++i) {
                int[] vs = positionVertices[i];
                Vector[] ns = new Vector[vs.Length];
                for (int j = 0; j < vs.Length; ++j) {
                    ns[j] = normals[vs[j]];
                }
                newNormals[i] = AverageVectors(ns).normalized;
            }
            normals = newNormals;
            normalPerPosition = true;
        }

        // neighbnors
        private void SetNeighbor(Edge e, Edge n) {
            neighbors[e.face.submesh][e.face.index][e.index] = n;
        }
        public Edge GetNeighbor(Edge e) {
            return neighbors[e.face.submesh][e.face.index][e.index];
        }

        public void InitHelpers() {
            // vertex edges
            vertexEdges = new Edge[vertexCount][];
            for (int v = 0; v < vertexCount; ++v) {
                var es = new List<Edge>();
                foreach (Face face in IterAllFaces()) {
                    int e = GetIndexInFace(GetFaceVertexIndices(face), v);
                    if (e != -1) {
                        es.Add(new Edge { face = face, index = e });
                    }
                }
                vertexEdges[v] = es.ToArray();
            }
            // position vertices
            positionVertices = new int[positions.Count][];
            for (int p = 0; p < positions.Count; ++p) {
                var vs = new List<int>();
                for (int v = 0; v < vertexCount; ++v) {
                    if (posIndices[v] == p) {
                        vs.Add(v);
                    }
                }
                positionVertices[p] = vs.ToArray();
            }
            // neighbors
            neighbors = new Edge[submeshes.Length][][];
            for (int s = 0; s < submeshes.Length; ++s) {
                int[][] faces = submeshes[s].faces;
                neighbors[s] = new Edge[faces.Length][];
                for (int f = 0; f < faces.Length; ++f) {
                    int[] es = faces[f];
                    neighbors[s][f] = new Edge[es.Length];
                    for (int e = 0; e < es.Length; ++e) {
                        neighbors[s][f][e] = Edge.Invalid;
                    }
                }
            }
            foreach (Edge e in IterAllEdges()) {
                if (GetNeighbor(e).IsValid()) continue; // already found
                Edge n = FindNeighbor(e);
                if (!n.IsValid()) continue; // e is boundary edge and has no neighbor
                SetNeighbor(e, n);
                SetNeighbor(n, e);
            }
        }

        #region triangles <-> (quad or triangle) faces
        // Convert: triangles -> faces
        private struct QuadVariant { // how two triangles (6 indices) compose a quad (4 indices)
            public int[] v0; // [2] 1st matching vertex indices (value 0..5)
            public int[] v1; // [2] 2nd matching vertex indices (value 0..5)
            public int[] vs; // [4] quad vertex indices         (value 0..5)
        }
        private static QuadVariant[] GetQuadVariants() {
            var qs = new QuadVariant[9];
            int[] tv = new[] { 0, 1, 2 };
            foreach (int i in tv) {
                foreach (int j in tv) {
                    qs[3*i+j] = new QuadVariant {
                        v0 = new[] { (i+1)%3, 3+(j+2)%3 },
                        v1 = new[] { (i+2)%3, 3+(j+1)%3 },
                        vs = new[] { (i+2)%3, (i+0)%3, (i+1)%3, 3+(j+0)%3 },
                    };
                }
            }
            return qs;
        }
        private static QuadVariant[] quadVariants = null;
        private static int[][] MakeQuadFaces(int[] ts) {
            // prepare quad variants once
            if (quadVariants == null) {
                quadVariants = GetQuadVariants();
            }
            // try to get quad faces
            var qs = new int[ts.Length / 6][];
            for (int i = 0; i < ts.Length; i += 6) {
                bool quadFound = false;
                foreach (QuadVariant q in quadVariants) {
                    if (ts[i + q.v0[0]] == ts[i + q.v0[1]] &&
                        ts[i + q.v1[0]] == ts[i + q.v1[1]])
                    {
                        qs[i / 6] = new[] {
                            ts[i + q.vs[0]],
                            ts[i + q.vs[1]],
                            ts[i + q.vs[2]],
                            ts[i + q.vs[3]],
                        };
                        quadFound = true;
                        break;
                    }
                }
                if (!quadFound) {
                    //if (i != 0) {
                    //    Debug.LogFormat("Non-quad found at {0}: {1} {2} {3} {4} {5} {6}", 
                    //        i / 6, ts[i], ts[i+1], ts[i+2], ts[i+3], ts[i+4], ts[i+5]);
                    //}
                    return null;
                }
            }
            return qs;
        }
        private static int[][] MakeTriangleFaces(int[] ts) {
            var fs = new int[ts.Length / 3][];
            for (int i = 0; i < ts.Length; i += 3) {
                fs[i / 3] = new[] {
                    ts[i + 0],
                    ts[i + 1],
                    ts[i + 2],
                };
            }
            return fs;
        }
        private static int[][] TrianglesToFaces(int[] triangles) {
            //!!! here might be some mode to mix quad (when possible) and triangle faces
            return 
                MakeQuadFaces(triangles) ?? 
                MakeTriangleFaces(triangles);
        }
        // Convert: faces -> triangles
        public static int[] FacesToTriangles(int[][] faces) {
            var ts = new List<int>();
            foreach (int[] f in faces) {
                for (int i = 2; i < f.Length; ++i) {
                    ts.AddRange(new[] { f[0], f[i-1], f[i] }); // e.g. {0,1,2}, {0,2,3} for quads
                }
            }
            return ts.ToArray();
        }
        #endregion triangles <-> faces

#if !SUBDIVIDEVECTOR4
        #region Mesh <-> MeshX
        private static VertexContent GetMeshVertexContent(Mesh mesh) {
            var c = VertexContent.pos;
            if (mesh.normals  != null && mesh.normals .Length == mesh.vertexCount) c |= VertexContent.normal;
            if (mesh.tangents != null && mesh.tangents.Length == mesh.vertexCount) c |= VertexContent.tangent;
            if (mesh.uv       != null && mesh.uv      .Length == mesh.vertexCount) c |= VertexContent.uv0;
            if (mesh.uv2      != null && mesh.uv2     .Length == mesh.vertexCount) c |= VertexContent.uv1;
            if (mesh.uv3      != null && mesh.uv3     .Length == mesh.vertexCount) c |= VertexContent.uv2;
            if (mesh.uv4      != null && mesh.uv4     .Length == mesh.vertexCount) c |= VertexContent.uv3;
            return c;
        }

        public MeshX(Mesh mesh) {
            // set vertex content
            content = GetMeshVertexContent(mesh);

            // init positions
            var poses = new List<Vector>();
            posIndices = new int[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; ++i) {
                Vector pos = mesh.vertices[i];
                int posIndex = poses.IndexOf(pos); //!!! use threshold?
                if (posIndex == -1) {
                    poses.Add(pos);
                    posIndex = poses.Count - 1;
                }
                posIndices[i] = posIndex;
            }
            positions = poses.ToArray();

            // init vertex content
            if (content.HasNormal()) {
                normals = mesh.normals;
                normalPerPosition = false;
            }
            if (content.HasTangent()) {
                tangents = new Vector[mesh.vertexCount];
                for (int i = 0; i < mesh.vertexCount; ++i) tangents[i] = (Vector)mesh.tangents[i];
            }
            if (content.HasUV0()) uvs0 = mesh.uv;
            if (content.HasUV1()) uvs1 = mesh.uv2;
            if (content.HasUV2()) uvs2 = mesh.uv3;
            if (content.HasUV3()) uvs3 = mesh.uv4;

            // set faces
            int submeshCount = mesh.subMeshCount;
            submeshes = new Submesh[submeshCount];
            for (int s = 0; s < submeshCount; ++s) {
                int[][] faces = MeshX.TrianglesToFaces(mesh.GetTriangles(s));
                submeshes[s] = new Submesh { faces = faces };
            }

            // copy name
            this.name = mesh.name;
        }

        public Mesh ConvertToMesh() {
            Mesh m = new Mesh { name = this.name };

            // positions
            var vs = new List<Vector3>();
            foreach (int i in posIndices) {
                vs.Add(positions[i]);
            }
            m.SetVertices(vs);

            // normals
            if (content.HasNormal()) {
                List<Vector3> ns;
                if (normalPerPosition) {
                    ns = new List<Vector3>();
                    foreach (int i in posIndices) {
                        ns.Add(normals[i]);
                    }
                } else {
                    ns = new List<Vector3>(normals);
                }
                m.SetNormals(ns);
            }

            // tangents
            if (content.HasTangent()) {
                var ts = new List<Vector4>();
                foreach (Vector3 t in tangents) {
                    ts.Add(new Vector4(t.x, t.y, t.z, 1f));
                }
                m.SetTangents(ts);
            }

            // uvs
            if (content.HasUV0()) m.SetUVs(0, new List<Vector2>(uvs0));
            if (content.HasUV1()) m.SetUVs(1, new List<Vector2>(uvs1));
            if (content.HasUV2()) m.SetUVs(2, new List<Vector2>(uvs2));
            if (content.HasUV3()) m.SetUVs(3, new List<Vector2>(uvs3));

            // faces
            m.subMeshCount = submeshes.Length;
            for (int s = 0; s < submeshes.Length; ++s) {
                m.SetTriangles(FacesToTriangles(submeshes[s].faces), s);
            }

            return m;
        }
        #endregion Mesh <-> MeshX
#endif

        public static int GetIndexInFace(int[] face, int v) {
            return System.Array.IndexOf<int>(face, v);
        }

        public static int GetNextInFace(int[] face, int v, int shift = 1) {
            //  v. -> .
            //     f  v
            //   . <  .
            int i = GetIndexInFace(face, v);
            int l = face.Length;
            return face[(i + shift + l) % l];
        }

        public Edge GetNextInFace(Edge edge, int shift = 1) {
            int[] face = GetFaceVertexIndices(edge.face);
            int l = face.Length;
            return new Edge {
                face = edge.face,
                index = (edge.index + shift + l) % l
            };
        }

        public IEnumerable<Face> IterAllFaces() {
            for (int s = 0; s < submeshes.Length; ++s) {
                int[][] faces = submeshes[s].faces;
                for (int f = 0; f < faces.Length; ++f) {
                    yield return new Face { submesh = s, index = f };
                }
            }
        }

        public IEnumerable<Edge> IterAllEdges() {
            foreach (Face face in IterAllFaces()) {
                int[] vs = GetFaceVertexIndices(face);
                for (int i = 0; i < vs.Length; ++i) {
                    yield return new Edge {
                        face = face,
                        index = i
                    };
                }
            }
        }

        public int[] GetFaceVertexIndices(Face f) {
            return submeshes[f.submesh].faces[f.index];
        }

        public int[] GetEdgeVertexIndices(Edge e) {
            int[] vs = GetFaceVertexIndices(e.face);
            int v0 = vs[e.index];
            int v1 = GetNextInFace(vs, v0);
            return new[] { v0, v1 };
        }

        private Edge FindNeighbor(Edge e) {
            int[] vs = GetEdgeVertexIndices(e);
            // edge "p0 -> p1"
            int p0 = posIndices[vs[0]];
            int p1 = posIndices[vs[1]];
            // find edge "p1 -> p0"
            foreach (int V1 in positionVertices[p1]) {
                foreach (Edge E in vertexEdges[V1]) {
                    int V0 = GetEdgeVertexIndices(E)[1];
                    int P0 = posIndices[V0];
                    if (P0 == p0) {
                        return E;
                    }
                }
            }
            return Edge.Invalid;
        }

        public EdgeType GetEdgeType(Edge e) {
            Debug.Assert(e.IsValid(), "GetEdgeType for invalid edge");
            Edge n = GetNeighbor(e);
            if (!n.IsValid()) return EdgeType.boundary;
            int[] E = GetEdgeVertexIndices(e);
            int[] N = GetEdgeVertexIndices(n);
            if (E[0] > N[0]) return EdgeType.back;
            EdgeType type = EdgeType.solid;
            if (E[0] != N[1]) type ^= EdgeType.seam0;
            if (E[1] != N[0]) type ^= EdgeType.seam1;
            return type;
        }


        public static Vector AverageVectors(Vector[] ps, float[] weights = null) {
            Vector p = new Vector();
            float ws = 0;
            for (int i = 0; i < ps.Length; ++i) {
                float w = weights != null ? weights[i] : 1.0f;
                p += w * ps[i];
                ws += w;
            }
            p /= ws;
            return p;
        }

        private static Vertex AverageVertices(Vertex[] vs, VertexContent c, float[] weights = null) {
            Vertex r = new Vertex();
            float ws = 0;
            for (int i = 0; i < vs.Length; ++i) {
                Vertex v = vs[i];
                float w = weights != null ? weights[i] : 1f;
                if (c.HasPosition()) r.position += w * v.position;
                if (c.HasNormal())   r.normal   += w * v.normal;
                if (c.HasTangent())  r.tangent  += w * v.tangent;
                if (c.HasUV0())      r.uv0      += w * v.uv0;
                if (c.HasUV1())      r.uv1      += w * v.uv1;
                if (c.HasUV2())      r.uv2      += w * v.uv2;
                if (c.HasUV3())      r.uv3      += w * v.uv3;
                ws += w;
            }
            if (c.HasPosition()) r.position /= ws;
            if (c.HasNormal())   r.normal.Normalize();
            if (c.HasTangent())  r.tangent.Normalize();
            if (c.HasUV0())      r.uv0 /= ws;
            if (c.HasUV1())      r.uv1 /= ws;
            if (c.HasUV2())      r.uv2 /= ws;
            if (c.HasUV3())      r.uv3 /= ws;
            r.posIndex = -1;
            return r;
        }

        public Vertex Average(Vertex[] vs, VertexContent mask = VertexContent.full, float[] weights = null) {
            return MeshX.AverageVertices(vs, content & mask, weights);
        }
    }
}

namespace sandiegoJohn.VRsculpting
{
    using VertexKey = System.UInt32;

    public static class CatmullClark {

        // based on:
        // http://www.rorydriscoll.com/2008/08/01/catmull-clark-subdivision-the-basics/
        // https://rosettacode.org/wiki/Catmull%E2%80%93Clark_subdivision_surface
        // https://graphics.pixar.com/opensubdiv/docs/subdivision_surfaces.html

        // calculation of unique keys for faces and edges
        #region Vertex keys
        // Check limits of mesh data arrays for valid vertex keys
        private static readonly int keyBitsSubmeshes    = 6;
        private static readonly int keyBitsSubmeshFaces = 32 - keyBitsSubmeshes - 2 - 1;
        private static readonly int keyMaxSubmeshes    = 1 << keyBitsSubmeshes;
        private static readonly int keyMaxSubmeshFaces = 1 << keyBitsSubmeshFaces;
        //
        private static void CheckForVertexKeys(MeshX mesh) {
            Debug.AssertFormat(mesh.submeshes.Length <= keyMaxSubmeshes, 
                "Subdivision may not work correctly for submesh count ({0}) > {1}", mesh.submeshes.Length, keyMaxSubmeshes);
            foreach (Submesh s in mesh.submeshes) {
                Debug.AssertFormat(s.faces.Length <= keyMaxSubmeshFaces,
                    "Subdivision may not work correctly for submesh face count ({0}) > {1}", s.faces.Length, keyMaxSubmeshFaces);
            }
        }
        private static VertexKey GetFacePointKey(int si, int fi) {
            // Bit format: [face index *23][submesh index *6][1]
            VertexKey key = 0;
                                      key ^= (VertexKey)fi;
            key <<= keyBitsSubmeshes; key ^= (VertexKey)si;
            key <<= 1;                key |= 1; // lowest bit 1 for face point key
            return key;
        }
        private static VertexKey GetEdgePointKey(int si, int fi, int ei) {
            // Bit format: [face index *23][submesh index *6][face edge index *2][0]
            VertexKey key = 0;
                                      key ^= (VertexKey)fi;
            key <<= keyBitsSubmeshes; key ^= (VertexKey)si;
            key <<= 2;                key ^= (VertexKey)ei;
            key <<= 1;                key |= 0; // lowest bit 0 for edge point key
            return key;
        }
        private static VertexKey GetFacePointKey(Face f) {
            return GetFacePointKey(f.submesh, f.index);
        }
        private static VertexKey GetEdgePointKey(Edge e) { // Be sure it's not EdgeType.back
            return GetEdgePointKey(e.face.submesh, e.face.index, e.index);
        }

        #endregion Vertex keys

        private class MeshVertices { // Allows to add and get mesh vertices by keys
            public MeshX mesh;
            public Dictionary<VertexKey, int> vertexIndices = new Dictionary<VertexKey, int>();
            public struct VertexKeyPair {
                public Vertex vertex;
                public VertexKey key;
            }
            public void AddVertices(Vertex position, params VertexKeyPair[] vertexPairs) { // Add a single position and several corresponding vertices
                int pi = mesh.AddPosition(position);
                foreach (VertexKeyPair p in vertexPairs) {
                    Vertex v = p.vertex;
                    v.posIndex = pi;
                    int vi = mesh.AddVertex(v, addPosition: false);
                    vertexIndices[p.key] = vi;
                }
            }
            public Vertex GetVertex(VertexKey key, VertexContent mask = VertexContent.full) {
                return mesh.GetVertex(vertexIndices[key], mask);
            }
        }
        private static MeshVertices.VertexKeyPair Pair(Vertex v, VertexKey k) { // new VertexKeyPair shortcut
            return new MeshVertices.VertexKeyPair { vertex = v, key = k };
        }

        private static VertexContent maskPosition = VertexContent.pos | VertexContent.normal;
        private static VertexContent maskVertex = VertexContent.tangent | VertexContent.uv0 | VertexContent.uv1 | VertexContent.uv2 | VertexContent.uv3;

		public static MeshX Subdivide(MeshX mesh, Options options) 
        {
            if (!mesh.helpersInited) mesh.InitHelpers();
            if (!mesh.normalPerPosition) mesh.MakeNormalPerPosition();

            MeshX newMesh = new MeshX {
                name = mesh.name + "/s",
                content = mesh.content,
                normalPerPosition = true,
            };
            newMesh.StartBuilding();

            CheckForVertexKeys(mesh);
            var newVertices = new MeshVertices { mesh = newMesh };
            var edgeMidPositions = new Dictionary<VertexKey, Vertex>(); // position & normal


            // reserve indices for new control-points: they keep indices and we need no special keys for them.
            // later for control-points we set position & normal only: tangent & uv stay the same.
            for (int pi = 0; pi < mesh.positions.Count; ++pi) {
                newMesh.AddPosition(default(Vertex));
            }
            for (int vi = 0; vi < mesh.vertexCount; ++vi) {
                Vertex v = mesh.GetVertex(vi, maskVertex);
                newMesh.AddVertex(v, addPosition: false);
            }

            // add face-points
            // "for each face, a face point is created which is the average of all the points of the face."
            foreach (Face f in mesh.IterAllFaces()) {
                Vertex[] vs = mesh.GetVertices(mesh.GetFaceVertexIndices(f));

                Vertex v = mesh.Average(vs);

                VertexKey keyF = GetFacePointKey(f);
                newVertices.AddVertices(v, Pair(v, keyF));
            }

            // add edge-points
            foreach (Edge e in mesh.IterAllEdges()) {
                EdgeType type = mesh.GetEdgeType(e);
                if (type == EdgeType.back) continue;

                if (type == EdgeType.boundary) {
                    // "for the edges that are on the border of a hole, the edge point is just the middle of the edge."

                    Vertex midE = mesh.Average(
                        mesh.GetVertices(mesh.GetEdgeVertexIndices(e))
                    );

                    VertexKey keyE = GetEdgePointKey(e);

                    edgeMidPositions[keyE] = midE;

                    newVertices.AddVertices(midE, Pair(midE, keyE));

                } else if (type == EdgeType.solid) {
                    // "for each edge, an edge point is created which is the average between the center of the edge 
                    //  and the center of the segment made with the face points of the two adjacent faces."

                    Edge n = mesh.GetNeighbor(e);

                    Vertex midE = mesh.Average(
                        mesh.GetVertices(mesh.GetEdgeVertexIndices(e))
                    );

                    VertexKey keyE = GetEdgePointKey(e);

                    edgeMidPositions[keyE] = midE;

                    Vertex v = mesh.Average(
                        new[] {
                            midE,
                            newVertices.GetVertex(GetFacePointKey(e.face)),
                            newVertices.GetVertex(GetFacePointKey(n.face)),
                        },
                        weights: new[] { 2f, 1f, 1f }
                    );

                    newVertices.AddVertices(v, Pair(v, keyE));

                } else { // seam edge

                    Edge n = mesh.GetNeighbor(e);

                    Vertex midE = mesh.Average(
                        mesh.GetVertices(mesh.GetEdgeVertexIndices(e))
                    );
                    Vertex midN = mesh.Average(
                        mesh.GetVertices(mesh.GetEdgeVertexIndices(n)), 
                        maskVertex // pos & normal already got in midE
                    );

                    VertexKey keyE = GetEdgePointKey(e);
                    VertexKey keyN = GetEdgePointKey(n);

                    edgeMidPositions[keyE] = midE;

                    Vertex p = mesh.Average( // pos & normal only
                        new[] {
                            midE,
                            newVertices.GetVertex(GetFacePointKey(e.face), maskPosition),
                            newVertices.GetVertex(GetFacePointKey(n.face), maskPosition),
                        },
                        maskPosition,
                        new[] { 2f, 1f, 1f }
                    );

                    newVertices.AddVertices(p, Pair(midE, keyE), Pair(midN, keyN));
                }
            }

            // move control-points
            for (int pi = 0; pi < mesh.positions.Count; ++pi) {

                // count edges
                var edges      = new List<Edge>(); // edges outcoming from the position
                var boundaries = new List<Edge>();
                var front      = new List<Edge>();
                foreach (int vi in mesh.positionVertices[pi]) {
                    foreach (Edge e in mesh.vertexEdges[vi]) {
                        edges.Add(e);
                        foreach (Edge edge in new[] { e, mesh.GetNextInFace(e, -1) }) {
                            EdgeType type = mesh.GetEdgeType(edge);
                            if (type == EdgeType.boundary) {
                                boundaries.Add(edge);
                            } else if (type != EdgeType.back) {
                                front.Add(edge);
                            }
                        }
                    }
                }
                Debug.AssertFormat(boundaries.Count > 0 || (edges.Count == front.Count),
                    "Counting edges error: boundaries: {0}, edges {1}, front: {2}", boundaries.Count, edges.Count, front.Count);

                Vertex controlPoint;

                if (boundaries.Count > 0) {
                    bool isCorner = edges.Count == 1;
                    if (options.boundaryInterpolation == Options.BoundaryInterpolation.fixBoundaries ||
                        options.boundaryInterpolation == Options.BoundaryInterpolation.fixCorners && isCorner) {
                        controlPoint = mesh.GetPosition(pi); // keep same position
                    } else {
                        // "for the vertex points that are on the border of a hole, the new coordinates are calculated as follows:
                        //   1. in all the edges the point belongs to, only take in account the middles of the edges that are on the border of the hole
                        //   2. calculate the average between these points (on the hole boundary) and the old coordinates (also on the hole boundary)."
                        Vertex[] vs = new Vertex[boundaries.Count + 1];
                        vs[0] = mesh.GetPosition(pi);
                        for (int e = 0; e < boundaries.Count; ++e) {
                            vs[e + 1] = edgeMidPositions[GetEdgePointKey(boundaries[e])];
                        }
                        controlPoint = mesh.Average(vs, maskPosition);
                    }
                } else {
                    // "for each vertex point, its coordinates are updated from (new_coords):
                    //   the old coordinates (P),
                    //   the average of the face points of the faces the point belongs to (F),
                    //   the average of the centers of edges the point belongs to (R),
                    //   how many faces a point belongs to (n), then use this formula: 
                    //   (F + 2R + (n-3)P) / n"

                    // edge-midpoints
                    Vertex[] ms = new Vertex[front.Count];
                    for (int e = 0; e < front.Count; ++e) {
                        ms[e] = edgeMidPositions[GetEdgePointKey(front[e])];
                    }
                    Vertex edgeMidAverage = mesh.Average(ms, maskPosition);
                    // face-points
                    Vertex[] fs = new Vertex[edges.Count];
                    for (int e = 0; e < edges.Count; ++e) {
                        fs[e] = newVertices.GetVertex(GetFacePointKey(edges[e].face), maskPosition);
                    }
                    Vertex faceAverage = mesh.Average(fs, maskPosition);
                    // new control-point
                    controlPoint = mesh.Average(
                        new[] {
                            faceAverage,
                            edgeMidAverage,
                            mesh.GetPosition(pi)
                        },
                        maskPosition,
                        new[] { 1f, 2f, edges.Count - 3f }
                    );
                }

                // set moved control-point position to reserved index
                newMesh.SetPosition(pi, controlPoint);
            }

            // add 4 new quads per face
            //           eis[i]
            //   fis[i].----.----.fis[i+1]
            //         |         |
            // eis[i-1].  ci.    .
            //         |         |
            //         .____.____.
            newMesh.submeshes = new Submesh[mesh.submeshes.Length];
            for (int si = 0; si < mesh.submeshes.Length; ++si) {
                int[][] faces = mesh.submeshes[si].faces;
                // get new face count
                int faceCount = 0;
                foreach (int[] face in faces) faceCount += face.Length;
                newMesh.submeshes[si].faces = new int[faceCount][];
                // fill faces
                int faceIndex = 0;
                for (int fi = 0; fi < faces.Length; ++fi) {
                    int[] fis = faces[fi];
                    int edgeCount = fis.Length; // 3 or 4
                    //
                    Face f = new Face { submesh = si, index = fi };
                    int ci = newVertices.vertexIndices[GetFacePointKey(f)]; // face-point index
                    //
                    int[] eis = new int[edgeCount]; // edge-point indices
                    for (int i = 0; i < edgeCount; ++i) {
                        Edge e = new Edge { face = f, index = i };
                        // get neighbor for solid back edges
                        if (mesh.GetEdgeType(e) == EdgeType.back) { //!!! here should be some EdgeType.backOfSeam or EdgeType.backOfSolid
                            Edge n = mesh.GetNeighbor(e);
                            if (mesh.GetEdgeType(n) == EdgeType.solid) {
                                e = n;
                            }
                        }
                        eis[i] = newVertices.vertexIndices[GetEdgePointKey(e)];
                    }
                    //
                    for (int i = 0; i < edgeCount; ++i) {
                        int[] q = new int[4]; // new faces are always quads
                        int s = edgeCount == 4 ? i : 0; // shift indices (+ i) to keep quad orientation
                        q[(0 + s) % 4] = fis[i];
                        q[(1 + s) % 4] = eis[i];
                        q[(2 + s) % 4] = ci;
                        q[(3 + s) % 4] = eis[(i - 1 + edgeCount) % edgeCount];
                        newMesh.submeshes[si].faces[faceIndex++] = q;
                    }
                }
            }

            newMesh.FinishBuilding();

            return newMesh;
        }
        
        public static MeshX Subdivide(MeshX mesh, int iterations, Options options = default(Options)) {
            string name = mesh.name;
            // subdivide
            for (int i = 0; i < iterations; ++i) {
                mesh = Subdivide(mesh, options);
            }
            // rename
            if (iterations > 0) {
                mesh.name = name + "/s" + iterations;
            }
            return mesh;
        }

        public struct Options {
            // like https://graphics.pixar.com/opensubdiv/docs/subdivision_surfaces.html#boundary-interpolation-rules
            public enum BoundaryInterpolation {
                normal, // default
                fixCorners,
                fixBoundaries,
            }
            public BoundaryInterpolation boundaryInterpolation;
        }

#if !SUBDIVIDEVECTOR4
        public static Mesh Subdivide(Mesh mesh, int iterations, Options options = default(Options)) {
            var m = new MeshX(mesh);
            //m.Trace();
            m = Subdivide(m, iterations, options);
            //Debug.Log("----->"); m.Trace();
            return m.ConvertToMesh();
        }

        public static void Subdivide(GameObject obj, int iterations, Options options = default(Options)) {
            if (iterations <= 0) return;

            MeshFilter mf = CheckMeshFilter(obj);

            Mesh originalMesh = mf.sharedMesh;

            //Mesh mesh = Object.Instantiate<Mesh>(originalMesh); // no need to copy: Subdivide doesn't change the Mesh
            Mesh mesh = originalMesh;

            mesh = Subdivide(mesh, iterations, options);

            mf.sharedMesh = mesh;
        }

        public static MeshFilter CheckMeshFilter(GameObject obj)
        {
            if (obj == null) throw new System.NullReferenceException("No GameObject specified");

            var mf = obj.GetComponent<MeshFilter>();
            if (mf == null) throw new System.Exception("No MeshFilter found for " + obj.name);

            if (mf.sharedMesh == null) throw new System.Exception("No mesh set to " + obj.name);

            return mf;
        }
#endif
    }

    [System.Obsolete("MeshData.Subdivide has been deprecated. Use Torec.CatmullClark.Subdivide instead", true)]
    public class MeshData {
        public static MeshData Subdivide(MeshData meshData, int iterations) { return meshData; } // dummy
    }
}

