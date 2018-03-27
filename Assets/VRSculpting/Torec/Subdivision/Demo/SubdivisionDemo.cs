using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace sandiegoJohn.VRsculpting{

#if UNITY_EDITOR
// Add "Update" button for SubdivisionDemo component

using UnityEditor;

//[CustomEditor(typeof(SubdivisionDemo))]
//public class SubdivisionDemoEditor : Editor {
//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();
//        if (GUILayout.Button("Update")) {
//            SubdivisionDemo demo = (SubdivisionDemo)target;
//            demo.SubdivideMeshes();
//        }
//    }
//}

#endif


	public class SubdivisionDemo : MonoBehaviour
	{
		public int subCount = 0;
	    public Material m_material;
	    public CatmullClark.Options.BoundaryInterpolation m_boundaryInterpolation;

		public MeshFilter ClayMesh;

		public void AssignMesh(){
			ClayMesh = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<MeshFilter> ();
		}
//	    void Reset()
//	    {
//	        SubdivideMeshes();
//	    }

	    public void SubdivideMeshes()
	    {
	        // delete previous children
	        var oldChildren = Enumerable.Range(0, this.transform.childCount)
	            .Select(i => this.transform.GetChild(i)).ToArray();
			foreach (var c in oldChildren) Object.Destroy(c.gameObject);

	        // create 4x4 array of subdivided objects

	            //Mesh cube = CreatePrimitiveCube(holeCount: holes);
	            //cube.name = string.Format("Cube_holes{0}", holes);

	           // cube.RecalculateNormals();


//				Mesh mesh = CatmullClark.Subdivide(ClayMesh.mesh, 1, new CatmullClark.Options {
//	                  boundaryInterpolation = m_boundaryInterpolation});
			if (subCount <= 3) {
				CatmullClark.Subdivide (ClayMesh.gameObject, 1, new CatmullClark.Options{ boundaryInterpolation = m_boundaryInterpolation });
				subCount++;

			}

				//ClayMesh.sharedMesh = mesh2;
//	            string name = string.Format("Cube_holes{0}_subdiv{1}", holes, iter);
//	            var obj = new GameObject(name);
//	            obj.transform.SetParent(this.transform);
//	            obj.transform.position = new Vector3(iter * 3f, holes * 3f, 0);
//	            obj.AddComponent<MeshFilter>().sharedMesh = mesh;
//	            obj.AddComponent<MeshRenderer>().material = m_material;
//	            
	        
	    }

	    private Mesh CreatePrimitiveCube(int holeCount = 0) {
	        // vertices
	        var vs = new List<Vector3>(8);
	        float[] cs = new[] { -1f, 1f };
	        foreach (float z in cs) {
	            foreach (float y in cs) {
	                foreach (float x in cs) {
	                    vs.Add(new Vector3(x,y,z));
	                }
	            }
	        }
	        // triangles
	        var ts = new List<int>(6*2*3);
	        int[] aa = new[] { 1,2,4, 2,4,1, 4,1,2 }; // three directions to get face vertices
	        for (int a = 0; a < 9; a += 3) {
	            int[] q = new[] { 0, aa[a], aa[a+1], aa[a]+aa[a+1] }; // aa{1,2,4} -> {0,1,2,3};  aa{2,4,1} -> {0,2,4,6};  aa{4,1,2} -> {0,4,1,5}
	            int s = aa[a + 2];
	            // holes
	            bool skipFrontFace = (holeCount > 1 && a == 3);
	            bool skipBackFace  = (holeCount > 0 && a == 3) || (holeCount > 2 && a == 0);
	            //
	            if (!skipFrontFace) ts.AddRange(new[] { 0,2,3, 0,3,1 }.Select(i => q[i]));
	            if (!skipBackFace)  ts.AddRange(new[] { 3,2,0, 3,0,1 }.Select(i => q[i] + s));
	        }
	        // mesh
	        Mesh m = new Mesh();
	        m.SetVertices(vs);
	        m.SetTriangles(ts, 0);
	        return m;
	    }
		

	}
}
