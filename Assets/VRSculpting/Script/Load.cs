using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace sandiegoJohn.VRsculpting{

	public class Load : MonoBehaviour {

		string fileName = "Assets/SerializedMesh.data";
		MeshFilter ClayMesh;
		public Texture tex;
		public Material mat;
		Mesh inputMesh;
		public AudioSource LoadSound;
		// Use this for initialization
		void Start () {
			

		}

		void FixedUpdate(){
	//		if (Input.GetKeyDown (KeyCode.N)) {
	//			//tex = Resources.Load ("ClayTexture") as Texture;
	//			LoadIt ();
	//		}

		}



		public Mesh LoadObject(Mesh input){
			if (ClayMesh == null) {
				ClayMesh = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<MeshFilter> ();
			}
			mat.mainTexture = null;

			if (fileName != null) {
				input = MeshSerializer.ReadMeshFromFile (fileName);
			}

			return input;
		}

		public void LoadIt(){
			Mesh mesh = LoadObject (inputMesh);
			if (!mesh) {
				Debug.Log ("failed to load mesh");
				return;
			}
			Debug.Log ("Mesh loaded");
			//GetComponent<MeshRenderer> ().material.mainTexture = tex;
			LoadSound.Play();
			ClayMesh.GetComponent<MeshCollider>().sharedMesh = mesh;
			ClayMesh.mesh = mesh;
			mat.mainTexture = tex;


		}
	}

}