using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
namespace sandiegoJohn.VRsculpting
{

    public class GetMeshTransform : MonoBehaviour
    {
        //Scaling a mesh using vive controller

        public float speed = 1f;
        public float offset = 0.0f;

        public GameObject scaleArmature;
        //GameObject scalePrefab;
        public Material InteractMat;
		public GameObject sphere;

        public GameObject controllerPos;
        public float range = 2f;

		//rotation
		PaintMeshDeformer PMD;
		//sensitivity of the rotation
		public  float _sensitivity;
		//starting mouse position
		private Vector3 _controllerReference;

		public Transform _controllerPos;
		//mouse position after it moved
		private Vector3 _controllerOffset;
		//this rotates the object by an amount
		private Vector3 _rotation;
		public bool triggerIsclicked = false;
		public bool triggerunclicked = false;

		public float x,y,z;
		public Slider[] sculpt;
		public Slider[] rotate;

		public Toggle UniformScale;

		public VRTK_InteractableObject scriptRef;

		public Toggle GrabToggle;

		public VRTK_UIPointer PointerRef;

		public UINavigationSystem UIref;

		bool CheckingReference;

        //test to random float to see if the sphere scales using the variable
        //create a grabbing mechanic. controller needs to grab the armature of the sphere. the distance the controller travels is what gets assigned to the 
        //sphere in realtimeSmooth
        // Use this for initialization

        void FixedUpdate()
        {
            
			if (CheckingReference) {
				if (sphere == null) {
					sphere = GameObject.FindGameObjectWithTag ("Mesh");

				}
				if (scriptRef == null) {
					scriptRef = sphere.GetComponent<VRTK_InteractableObject> ();
				}
				if (PointerRef == null) {
					PointerRef = GameObject.FindGameObjectWithTag ("controllerRight").GetComponent<VRTK_UIPointer> ();

				}
				if (UIref == null) {
					UIref = GameObject.FindGameObjectWithTag ("UINref").GetComponent<UINavigationSystem> ();
					sculpt = UIref.Scalexyz;
					rotate = UIref.Rotatexyz;
					UniformScale = UIref.uniformScale;
					GrabToggle = UIref.Grab;
				}
				if (controllerPos == null) {
					controllerPos = GameObject.FindGameObjectWithTag ("controllerRight");
				}
			}
        }

		public void AssignedMeshTrasformReference(){
			sphere = GameObject.FindGameObjectWithTag ("Mesh");
			_rotation = Vector3.zero;
			PMD = GameObject.FindGameObjectWithTag("PMD").GetComponent<PaintMeshDeformer>();
			_controllerPos = GameObject.FindGameObjectWithTag("controllerRight").GetComponent<Transform>();
			scriptRef = sphere.GetComponent<VRTK_InteractableObject> ();
			PointerRef = GameObject.FindGameObjectWithTag ("controllerRight").GetComponent<VRTK_UIPointer> ();
			UIref = GameObject.FindGameObjectWithTag ("UINref").GetComponent<UINavigationSystem> ();
			if (UIref == null) {
				UIref = GameObject.FindGameObjectWithTag ("UINref").GetComponent<UINavigationSystem> ();
			}
			controllerPos = GameObject.FindGameObjectWithTag ("controllerRight");
			sculpt = UIref.Scalexyz;
			rotate = UIref.Rotatexyz;
			UniformScale = UIref.uniformScale;
			GrabToggle = UIref.Grab;
			CheckingReference = true;

		}

        void detectObject()
        {
            //check if the controller is within range
            if (Vector3.Distance(controllerPos.transform.position, transform.position) < range)
            {
                InteractMat.color = Color.red;

            }
            else
            {
                InteractMat.color = Color.white;
            }
        }

		public void Scale(){
			if(!UniformScale.isOn){
				x = sculpt[0].value;
				y = sculpt[1].value;
				z = sculpt[2].value;

			}else if(UniformScale.isOn){
				//Debug.Log ("hoverName: " + PointerRef.hoveringElement.name);
				x = y = z = sculpt[0].value;
				x = y = z = sculpt[1].value;
				x = y = z = sculpt[2].value;
				y = x = z = sculpt[0].value;
				y = x = z = sculpt[1].value;
				y = x = z = sculpt[2].value;
				if (PointerRef.hoveringElement.name == "X Slider") {
					slideryzChange ();
				} 
				if (PointerRef.hoveringElement.name == "Y Slider") {
					sliderxzChange ();
				}
				if (PointerRef.hoveringElement.name == "Z Slider") {
					sliderxyChange ();
				}
			}
			sphere.transform.localScale = new Vector3 (x, y, z);
		}

		void sliderxyChange(){
			sculpt[0].value = z;
			sculpt[1].value = z;
		}

		void sliderxzChange(){
			sculpt[0].value = y;
			sculpt[2].value = y;
		}

		void slideryzChange(){
			sculpt[1].value = x;
			sculpt[2].value = x;
		}


		public void Rotate()
		{
			float x, y, z;

			x = rotate[0].value;
			y = rotate[1].value;
			z = rotate[2].value;

			sphere.transform.eulerAngles = new Vector3 (x, y, z);
			//Debug.Log("rotating");
//			if (PMD.isTriggerOn) // if rotating is true
//			{
//				
//				// offset
//				_controllerOffset = (controllerPos.transform.position - _controllerReference);  //store initial mouse position - the end mouse position
//
//				// apply rotation
//				_rotation.y = -(_controllerOffset.x + _controllerOffset.y) * _sensitivity;  //
//
//				_rotation.y = -(_controllerOffset.x) * _sensitivity;
//				_rotation.x = -(_controllerOffset.y) * _sensitivity;
//				// rotate
//				sphere.transform.Rotate(_rotation);
//				sphere.transform.eulerAngles += _rotation;
//
//				// store mouse
//				_controllerReference = controllerPos.transform.position;
//
//				Debug.Log("trigger is clicked");
//			}
	
		}

		public void Grab(){
			//Debug.Log ("in grab");
			if (GrabToggle.isOn) {
				scriptRef.enabled = true;
				//Debug.Log ("grab is toggled true");
			} else if (!GrabToggle.isOn) {
				scriptRef.enabled = false;
				//Debug.Log ("grab is toggled false");
			}
		}

		public float xVal(){

			return x;
		}
		public float yVal(){
			return y;
		}
		public float zVal(){
			return z;
		}


    }
}
