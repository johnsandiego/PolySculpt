using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace sandiegoJohn.VRsculpting{
	
	public class UndoAndRedo : MonoBehaviour {

		Stack<Vector3[]> UndoStack = new Stack<Vector3[]>(); //a vector3 array stack
		Stack<Vector3[]> RedoStack = new Stack<Vector3[]>();
		PaintMeshDeformer PMD;
		public float SaveTimer = 1f;

		public AudioSource UndoAndRedoSound;

		public Mesh CurrentMesh; 

		// Use this for initialization
		void Start () {
			PMD = GameObject.FindGameObjectWithTag ("PMD").GetComponent<PaintMeshDeformer> ();

		}
		
		// Update is called once per frame
		void FixedUpdate () {


			//push the current mesh to stack once the mousebutton is pressed up
//			if (UndoStack == null) {
//				return;
//			}

			if (PMD.isTriggerOn) {
				SaveTimer -= Time.deltaTime;

				if (SaveTimer < 0.01f) {
					StartCoroutine(PushToStack ());
					SaveTimer = 1f;
				}


			}
			//if keyboard key p is pressed pop the current mesh from the stack
			if (Input.GetKeyDown (KeyCode.P)) {
				UndoPaint ();
			}

			if (Input.GetKeyDown (KeyCode.O)) {
				RedoPaint ();
			}


		}

		public void assignedReference(){
			CurrentMesh = GameObject.FindGameObjectWithTag("Mesh").GetComponent<MeshFilter> ().mesh;
			UndoStack.Push (CurrentMesh.vertices);
		}
		//Push the current mesh vertices in the stack
		IEnumerator PushToStack(){
			yield return new WaitForSeconds (1f);
			UndoStack.Push (CurrentMesh.vertices);
			yield return new WaitForSeconds (1f);
		}
		//checks to make sure their is something in the stack if not return
		//otherwise, pop the top of the stack and assign value to current mesh vertices
		//recalculate normals and bounds.
		public void UndoPaint(){
			
			if (UndoStack.Count != 0) {
				//RedoStack.Push (UndoStack.Peek ());
				UndoAndRedoSound.Play();
				CurrentMesh.vertices = UndoStack.Pop ();
				RedoStack.Push (CurrentMesh.vertices);
				CurrentMesh.RecalculateNormals ();
				CurrentMesh.RecalculateBounds ();
			} else
				return;

			//Debug.Log ("stack count: " + UndoStack.Count);
		}

		public void RedoPaint(){
			if (RedoStack.Count != 0) {
				UndoAndRedoSound.Play();
				CurrentMesh.vertices = RedoStack.Pop ();
				UndoStack.Push(CurrentMesh.vertices);
				CurrentMesh.RecalculateNormals ();
				CurrentMesh.RecalculateBounds ();
			} else
				return;
		}
	}
}
