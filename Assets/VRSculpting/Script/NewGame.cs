using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using System;

namespace sandiegoJohn.VRsculpting{

	public class NewGame : MonoBehaviour {
		
		[Tooltip("Holds the Clay prefab")]
		public GameObject clone;

		public Transform SpawnPosition;

		public PaintMeshDeformer PMD;

		public GetMeshTransform meshTrans;

		public UndoAndRedo undoNredo;

		public UINavigationSystem UIref;

		public VRSmooth dm;

		public bool newGamePressed;

		public AudioSource newgameSound;

		//SubdivisionDemo Sub;
		public VRTK_ControllerTooltips tooltip;


		// Use this for initialization
		void Start () {
			SpawnPosition = GameObject.FindGameObjectWithTag ("SpawnPosition").transform;
			PMD = GameObject.FindGameObjectWithTag ("PMD").GetComponent<PaintMeshDeformer>();
			undoNredo = GameObject.FindGameObjectWithTag ("SLUR").GetComponent<UndoAndRedo> ();
			UIref = GameObject.FindGameObjectWithTag ("UINref").GetComponent<UINavigationSystem> ();
			dm = GameObject.FindGameObjectWithTag ("SM").GetComponent<VRSmooth> ();
			tooltip = GameObject.FindGameObjectWithTag ("tooltip").GetComponent<VRTK_ControllerTooltips> ();

		}
		
		// Update is called once per frame
		void FixedUpdate () {
			try{
			if (newGamePressed) {
				if (meshTrans == null) {
					meshTrans = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<GetMeshTransform> ();
					meshTrans.AssignedMeshTrasformReference ();
				}
			}
			}catch(Exception ex){
				if (ex is NullReferenceException || ex is UnassignedReferenceException) {
					return;
				}
			}
		}

		void Reset(){
			
		}
		//instantiates a new mesh object prefab with all its scripts
		public void SpawnNewMesh(){
			//Debug.Log (clone);
			newgameSound.Play ();
			if (clone == null) {
				clone = (GameObject)Instantiate (Resources.Load ("TexturePainter-Instances/Sphere"), SpawnPosition);
				clone.GetComponent<GenerateSphere> ().GenerateMesh ();
				UIref.assignedAllUI ();
				PMD.AssignedAllReference ();
				meshTrans = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<GetMeshTransform> ();
				meshTrans.AssignedMeshTrasformReference ();
				undoNredo.assignedReference ();
				dm.assignedUIObjects ();
				//Debug.Log ("spawned");
				newGamePressed = true;
			} else {
				Destroy (clone);
				clone = (GameObject)Instantiate (Resources.Load ("TexturePainter-Instances/Sphere"), SpawnPosition);
				clone.GetComponent<GenerateSphere> ().GenerateMesh ();
				UIref.assignedAllUI ();
				PMD.AssignedAllReference ();
				meshTrans = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<GetMeshTransform> ();
				meshTrans.AssignedMeshTrasformReference ();
				undoNredo.assignedReference ();
				dm.assignedUIObjects ();
				//Debug.Log ("Object already instantiated");
				newGamePressed = true;
			}
		}

	}

}