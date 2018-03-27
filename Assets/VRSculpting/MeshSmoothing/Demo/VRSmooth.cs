using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace sandiegoJohn.VRsculpting{


	public class VRSmooth : MonoBehaviour {

		public Dropdown SmoothAlgorithmValue;
		public Slider Times;
		public Button ActivateSmooth;
		public MeshFilter ClayMesh;
		public GameObject smth;
		public RectTransform[] smoothpage;
		//public Slider iterationSlider;
		public PaintMeshDeformer PMD;
		Mesh mesh;
		bool hasMesh;

		[System.Serializable] 
		enum FilterType {
			Laplacian, HC
		};

		MeshFilter filter {
			get {
				if(_filter == null) {
					_filter = ClayMesh;
				}
				return _filter;
			}
		}

		MeshFilter _filter;

		[SerializeField, Range(0f, 1f)] float intensity = 0.0f;
		[SerializeField] FilterType type;
		[SerializeField, Range(0, 20)] int times = 3;
		[SerializeField, Range(0f, 1f)] float hcAlpha = 0.5f;
		[SerializeField, Range(0f, 1f)] float hcBeta = 0.5f;
		
		void Start () {

		}
		
		 void Update () {
			if (ClayMesh == null &&  hasMesh) {
				ClayMesh = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<MeshFilter> ();
			}
		}


		Mesh ApplyNormalNoise (Mesh mesh) {

			var vertices = mesh.vertices;
			var normals = mesh.normals;
			for(int i = 0, n = mesh.vertexCount; i < n; i++) {
				vertices[i] = vertices[i] + normals[i] * UnityEngine.Random.value * intensity;
			}
			mesh.vertices = vertices;

			return mesh;
		}

		//function for the button to use
		public void LaplacianSmooth(Mesh currentMesh){
			times = (int)Times.value;
			filter.mesh = MeshSmoothing.LaplacianFilter (currentMesh, times);
		}

		public void SmoothObject(){
			switch (SmoothAlgorithmValue.value) {
				case 0:
					LaplacianSmooth ();
					break;
				case 1:
					HumphreySmooth ();
					break;
				default:
					HumphreySmooth ();
					break;
			}
		}

		//function for the button to use
		public void LaplacianSmooth(){
			if (Times != null) {
				times = (int)Times.value;
			} else {
				times = 1;
			}
			filter.mesh = MeshSmoothing.LaplacianFilter (filter.mesh, times);
		}

		public void HumphreySmooth(){
			if (Times != null) {
				times = (int)Times.value;
			} else {
				times = 1;
			}
			filter.mesh = MeshSmoothing.HCFilter (filter.mesh, times, hcAlpha, hcBeta);
		}

		public void assignedUIObjects(){
				ClayMesh = null;
		
				ClayMesh = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<MeshFilter> ();
				//Debug.Log ("VR Smooth");
				//smoothpage = new RectTransform[3];
				//smth = GameObject.FindGameObjectWithTag ("smooth");
				mesh = ClayMesh.mesh;
			hasMesh = true;
		}

//		public void assignUI(){
//			Debug.Log ("butt");
//			try{
//				
//				smoothpage = smth.GetComponentsInChildren<RectTransform> ();
//				Debug.Log("smooth assigning ui");
//				SmoothAlgorithmValue = smoothpage [0].GetComponent<Dropdown>();
//				Times = smoothpage [1].GetComponent<Slider>();
//				ActivateSmooth = smoothpage [2].GetComponent<Button>();
//
//			}catch(Exception ex){
//				if (ex is NullReferenceException || ex is UnassignedReferenceException) {
//					return;
//				}
//			}
//		}

	}


}
