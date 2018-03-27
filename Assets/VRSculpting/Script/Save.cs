using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sandiegoJohn.VRsculpting{

	public class Save : MonoBehaviour {
		public  AudioSource save;
		string fileName = "SerializedMesh.data";
		bool saveTangents = false;
		Mesh inputMesh;


		// Use this for initialization
		void Start () {
			
		}

		void Update(){
			if (Input.GetKeyDown (KeyCode.M)) {
				SaveIt ();
			}
		}

		public void SaveIt(){
			if (inputMesh == null) {
				inputMesh = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<MeshFilter> ().mesh;
			}
			if (inputMesh != null) {
				save.Play ();
				var fullFileName = Application.dataPath + "/" + fileName;
				MeshSerializer.WriteMeshToFile (inputMesh, fullFileName, saveTangents);
				Debug.Log ("saved " + name + " mesh to " + fullFileName);
			}
		}
			

	}
}