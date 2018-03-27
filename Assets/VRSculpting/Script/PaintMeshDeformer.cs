using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using System.Linq;
using VRTK;
using System;
/*
 * Author: John San Diego
 * Modified from Unity Procedural Primitive scripts
 * Controllers Sculpting, UI
 */
public enum Tools {None, Sculpt, Scale, Rotate, Crease, Grab, Symmetry, Smooth, Paint };
namespace sandiegoJohn.VRsculpting{

	public class PaintMeshDeformer : MonoBehaviour {

		[Tooltip("Modifiable Mesh prefab")]
		public GameObject ClayMesh;

		//SteamVR tracked controller
		//private SteamVR_TrackedController _controller;
		[Tooltip("Gets controller inputs")]
		public VRTK_ControllerEvents _control;
		[Tooltip("Raypoint Point location")]
		public VRTK_Pointer point;
		[Tooltip("Tooltip VRTK script")]
		public VRTK_ControllerTooltips tooltip;

		[Tooltip("Controller Reference")]
		//Gameobject for the right controller
		public GameObject rightController;

		[Tooltip("VRSmooth Reference")]
		//uses the DEMO script
		public VRSmooth dm;

		[Tooltip("Radius of sculpt tool")]
		//Mesh list of required variables.
		public float radius;
		[Tooltip("Strength of sculpt push and pull")]
		public float pull;
		[Tooltip("x*x+y*y+z*z")]
		public Vector3 sqrMagnitude;
		[Tooltip("distance from other vertices")]
		public float distance;
		[Tooltip("Inverse of controller x position")]
		public Vector3 inverse;

		[Tooltip("Chooses the paint color")]
	    //public float fallOff;
		//UI chooser
	    public Dropdown PaintChooser;
		[Tooltip("Dropdown for choosing add or subtract")]
	    public Dropdown AddorSubtract;
		[Tooltip("strength level of the pull and push or mesh deformation")]
	    public Slider strengthLevel;
		[Tooltip("Radius Slider")]
		public Slider radiusLevel;
		[Tooltip("Mesh Symmetry Toggle")]
		public Toggle MeshSymmetryToggle;

		[Tooltip("RaycastHit variable")]
		public RaycastHit hit;
		[Tooltip("Raypoint reference")]
		public Transform Raypoint;

		//enum for choosing type of scult algorithm
		public enum FallOff{
			Gauss, 
			Linear, 
			Needle
		};

		[Tooltip("enum reference to which tool is being used")]
		//enum to check which tool is being used
		public Tools currentTool;
		[Tooltip("Mesh which are changed by sculpt. Needs to be changed")]
		private MeshFilter unappliedMesh;
		[Tooltip("Holds the clay meshfilter")]
		public MeshFilter filter;


		[Tooltip("controller trigger boolean on or off")]
		public bool isTriggerOn = false; //is trigger pressed boolean
		[Tooltip("Checks if smooth is currently being used")]
		public bool isInSmoothPage = false;

		public bool touchPadIsOn = false;

		public bool gripIsOn = false;

		[Tooltip("default sculpt algorithm")]
	    public FallOff fallOff = FallOff.Gauss;
		[Tooltip("fall off value")]
	    float _fallOff = 0.0f;

		[Tooltip("Gets the tranform of the clay mesh")]
		//rotate , scale, move script
		public GetMeshTransform MeshTransform;
		[Tooltip("reference for Vrpaint")]
		//UINavigation
		public VRTexturePainter texpaint;
		bool tooltipUIIsActive = false;
		bool tooltipMESHsActive = false;

		UINavigationSystem UIN;

		void Awake(){
			tooltip = GameObject.FindGameObjectWithTag ("tooltip").GetComponent<VRTK_ControllerTooltips> ();
			//tooltip.touchpadText = "Touchpad";
			UIN = GameObject.FindGameObjectWithTag("UINref").GetComponent<UINavigationSystem>();
		}


	    // Update is called once per frame
	    void FixedUpdate () {
			try{
				RaycastHit tooltipRayCast;
				//tooltip to show the player where the buttons are located on the controller
				if(Physics.Raycast(Raypoint.position, Raypoint.forward, out tooltipRayCast, 100f)){
					if(tooltipRayCast.collider.tag == "UI" && !tooltipUIIsActive){
						//tooltip.touchpadText = "Touchpad";
						tooltip.UpdateText(VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, "Press Touchpad");
						tooltip.UpdateText(VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, "");

						if(tooltip == null){
							tooltip = GameObject.FindGameObjectWithTag ("tooltip").GetComponent<VRTK_ControllerTooltips> ();
						}
						//tooltip.triggerText = "";
						tooltipUIIsActive = true;
						tooltipMESHsActive = false;
						//tooltip.touchpadText = "Touchpad";
						//Debug.Log("tag is UI = " + tooltip.touchpadText);

					}else if(tooltipRayCast.collider.tag == "Mesh" && !tooltipMESHsActive && UIN.isActive){
						
						tooltip.UpdateText(VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, "Press Trigger");
						tooltip.UpdateText(VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, "");

						if(tooltip == null){
							tooltip = GameObject.FindGameObjectWithTag ("tooltip").GetComponent<VRTK_ControllerTooltips> ();
						}
						//tooltip.touchpadText = "";
						tooltipMESHsActive = true;
						tooltipUIIsActive = false;
						//tooltip.triggerText = "Trigger";
						//Debug.Log("tag is UI = " + tooltip.triggerText);
						//Debug.Log("tag is Mesh");
					}else if(tooltipRayCast.collider.tag != "UI"){
						tooltip.UpdateText(VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, "");
						tooltipUIIsActive = false;
					}
				}

				//manages which tool is being used
				//using an enum
				switch (currentTool) {

					//if sculpt is pressed, this finds the collider with a mesh tag. and 
					//gets the meshfilter so it can be modified.
					case Tools.Sculpt:
	                    try
	                    {	
							hit = point.pointerRenderer.GetDestinationHit();
	                        if (hit.transform.gameObject.tag != "Non-Modifiable")
	                          {
								//Debug.Log(hit.collider.name);
	                                filter = hit.collider.GetComponent<MeshFilter>();
	                                if (filter)
	                                {
	                                    if (filter != unappliedMesh)
	                                    {
	                                        ApplyMeshCollider();
	                                        unappliedMesh = filter;
	                                    }
	                                    //Mesh Symmetry. simple but it works
	                                    Vector3 relativePoint = filter.transform.InverseTransformPoint(hit.point);
	                                    Vector3 inversePoint = relativePoint;
	                                    inversePoint.x = -relativePoint.x;
	                                    if (isTriggerOn)
										{	
											//Debug.Log("is trigger on? " + isTriggerOn);
	                                        DeformMesh(filter.mesh, relativePoint, pull * Time.deltaTime, radius);
											if (MeshSymmetryToggle.isOn)
	                                        {
	                                            DeformMesh(filter.mesh, inversePoint, pull * Time.deltaTime, radius);
	                                        }
	                                    }
	                                }
	                           }
	                    }
	                    catch (System.NullReferenceException) { }
						break;
				case Tools.Scale:
					try{
						if (MeshTransform == null) {
							MeshTransform = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<GetMeshTransform> ();
						}
		                    //Debug.Log("Scaling");
							MeshTransform.Scale();

						}catch(Exception ex){
							if (ex is NullReferenceException || ex is UnassignedReferenceException) {
								return;
							}
						}
						break;
					case Tools.Rotate:
					try{
						if (MeshTransform == null) {
							MeshTransform = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<GetMeshTransform> ();
						}
	                    //Debug.Log("Rotating");
						MeshTransform.Rotate();
					}catch(Exception ex){
						if (ex is NullReferenceException || ex is UnassignedReferenceException) {
							return;
						}
					}
						break;
					case Tools.Crease:

						try
						{

						if (UIN == null) {
							UIN = GameObject.FindGameObjectWithTag ("UINref").GetComponent<UINavigationSystem> ();
						}
						AddorSubtract.value = 1;
						pull = -UIN.CreaseStrength.value;
						radius = .03f;

							
							hit = point.pointerRenderer.GetDestinationHit();
								if (hit.transform.gameObject.tag != "Non-Modifiable")
								{
									filter = hit.collider.GetComponent<MeshFilter>();
									if (filter)
									{
										if (filter != unappliedMesh)
										{
											ApplyMeshCollider();
											unappliedMesh = filter;
										}
										//Mesh Symmetry. simple but it works
										Vector3 relativePoint = filter.transform.InverseTransformPoint(hit.point);
										Vector3 inversePoint = relativePoint;
										inversePoint.x = -relativePoint.x;
										if (isTriggerOn)
										{
											//Debug.Log("trigger on");
											DeformMesh(filter.mesh, relativePoint, pull * Time.deltaTime, radius);
											//filter.gameObject.GetComponent<MeshCollider> ().sharedMesh = filter.mesh;
											if (MeshSymmetryToggle.isOn)
											{
												radius = .03f;
												//Debug.Log(pull + " pull value");
												DeformMesh(filter.mesh, inversePoint, pull * Time.deltaTime, radius);
												//filter.gameObject.GetComponent<MeshCollider> ().sharedMesh = filter.mesh;
											}
										}

								}
							}

							
						}
						catch (System.NullReferenceException) { }

	                    break; //get out
					case Tools.Grab:
						try{
							if (MeshTransform == null) {
								MeshTransform = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<GetMeshTransform> ();
							}
							//if (GrabToggle.isOn) {
								MeshTransform.Grab ();	

								//Debug.Log("Moving");
							}catch(Exception ex){
								if (ex is NullReferenceException || ex is UnassignedReferenceException) {
									return;
								}
							}

						break;
					case Tools.Symmetry:
						try{
							//MeshSymmetryToggle.isOn = true;
						}catch(Exception ex){
							if (ex is NullReferenceException || ex is UnassignedReferenceException) {
								return;
							}
						}
						break;
					case Tools.Smooth:
						//Debug.Log("Moving");

						try{
							isInSmoothPage = true;
							if (dm == null) {
								dm.ClayMesh = ClayMesh.GetComponent<MeshFilter>();
								dm = GameObject.FindGameObjectWithTag ("Mesh").GetComponent<VRSmooth> ();
							}
						}catch(Exception ex){
							if (ex is NullReferenceException || ex is UnassignedReferenceException) {
								return;
							}
						}

						break;
					case Tools.Paint:
						//Debug.Log("paint ON");
						try{
							if (texpaint == null) {
								texpaint = GameObject.FindGameObjectWithTag ("TP").GetComponent<VRTexturePainter> ();

							}
							texpaint.enabled = true;
						}catch(Exception ex){
							if (ex is NullReferenceException || ex is UnassignedReferenceException) {
								return;
							}
						}
						break;
				}
			}catch(Exception ex){
				if (ex is NullReferenceException || ex is UnassignedReferenceException) {
					return;
				}
			}

		}
        //coroutine for calling the smooth function
		IEnumerator Smooth(){
			dm.HumphreySmooth ();
			yield return null;
		}

		public void SmoothOnce(){
			try{
				dm.HumphreySmooth ();
			}catch(Exception ex){
				if (ex is NullReferenceException || ex is UnassignedReferenceException) {
					return;
				}
			}

		}


        //when a button on the vive controller is pressed it = runs a function
        //adds the function to a list
		void OnEnable(){

            _control = rightController.GetComponent<VRTK_ControllerEvents>();
            _control.TriggerClicked += _control_TriggerClicked;
			_control.TriggerUnclicked += _control_TriggerUnclicked;
			_control.TouchpadPressed += _control_TouchpadPressed;
			_control.GripClicked += _control_GripClicked;

		}




        //when button is unpressed removes the function to the list
        void OnDisable(){
            _control.TriggerClicked -= _control_TriggerClicked;
			_control.TriggerUnclicked -= _control_TriggerUnclicked;
			_control.GripClicked -= _control_GripUnClicked;
			_control.TouchpadPressed -= _control_TouchpadUnPressed;

        }


		void _control_GripClicked (object sender, ControllerInteractionEventArgs e)
		{
			gripIsOn = true;
			Debug.Log ("grip is on!!!");
		}

		void _control_GripUnClicked (object sender, ControllerInteractionEventArgs e)
		{
			gripIsOn = false;
		}



		void _control_TouchpadPressed (object sender, ControllerInteractionEventArgs e)
		{
			if (currentTool == Tools.Sculpt ||currentTool == Tools.Scale || currentTool == Tools.Rotate || currentTool == Tools.Crease 
				|| currentTool == Tools.Grab || currentTool == Tools.Symmetry || currentTool == Tools.Smooth || currentTool == Tools.Paint) {
				touchPadIsOn = true;
			}
		}

		void _control_TouchpadUnPressed (object sender, ControllerInteractionEventArgs e)
		{
			
				touchPadIsOn = false;
			
		}
		//when trigger of the right controller is unclicked
		void _control_TriggerUnclicked (object sender, ControllerInteractionEventArgs e)
		{
			try {
				if(currentTool == Tools.Sculpt){
					filter.gameObject.GetComponent<MeshCollider> ().sharedMesh = filter.mesh;
				}
			} catch (Exception ex) {
				if (ex is NullReferenceException || ex is MissingComponentException || ex is UnassignedReferenceException) {
					return;
				}
				throw;
			}

			isTriggerOn = false;
			if (currentTool == Tools.Rotate) {
				MeshTransform.triggerIsclicked = false;
				MeshTransform.triggerunclicked = true;
			}

//			if (currentTool == Tools.Crease) {
//				dm.Times.value = 1;
//				dm.HumphreySmooth ();
//			}
		}

        //function to run when pad of controller is pressed
        void _control_TriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            //Debug.Log("trigger click");
			try{
				if (filter.tag == "Mesh") {
					isTriggerOn = true;
				}
			}catch(Exception ex){
				if (ex is NullReferenceException || ex is MissingComponentException || ex is UnassignedReferenceException || ex is MissingReferenceException) {
					return;
				}
				throw;
			}

        }
			

        //function which calculates the amount of pull in terms of the radius
		public float LinearFallOff(float dis, float inRadius){
			return Mathf.Clamp01 (1.0f - dis / inRadius);
		}

		public float GaussFallOff(float diss, float inRadius){
			return Mathf.Clamp01(Mathf.Pow(360.0f, -Mathf.Pow(diss/inRadius,2.5f)-0.01f));
		}

		public float NeedleFalloff(float dist, float inRadius){
			return -(dist*dist)/(inRadius*inRadius)+1.0f;
		}

		public float PinchFalloff(float dist, float inRadius){
			return dist / inRadius + 1.0f;
		}



	    //deforms mesh by using the radius and power
		public void DeformMesh(Mesh mesh, Vector3 position, float power, float inRadius){
			//Mesh meshShared = filter.sharedMesh;
			Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			float sqrRadius = inRadius * inRadius;

			Vector3 averageNormal = Vector3.zero;
			for (int i = 0; i < vertices.Length; i++) {
				float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
	            //Debug.Log(sqrMagnitude);
				if (sqrMagnitude > sqrRadius)
					continue;

				distance = Mathf.Sqrt (sqrMagnitude);
				float fallOff = LinearFallOff (distance, inRadius);
				averageNormal += fallOff * normals [i];

			}
			averageNormal = averageNormal.normalized;

			for (int j = 0; j < vertices.Length; j++) {
				float sqrMagnitude = (vertices [j] - position).sqrMagnitude;



				if (sqrMagnitude > sqrRadius)
					continue;

				distance = Mathf.Sqrt (sqrMagnitude);

				switch (PaintChooser.value) {
					
				case 0:
					_fallOff = GaussFallOff (distance, inRadius);
					break;
				case 1:
					_fallOff = NeedleFalloff (distance, inRadius);
	                break;
	            case 2:
	                _fallOff = LinearFallOff(distance, inRadius);
	                break;
				default:
					_fallOff = LinearFallOff (distance, inRadius);
	                break;
				
				}
	            switch (AddorSubtract.value)
	            {
					case 0:
						pull = strengthLevel.value;
						radius = radiusLevel.value;
	                    break;
	                case 1:
	                    pull = -(strengthLevel.value);
						radius = radiusLevel.value;
	                    break;
	                default:
	                    pull = 2.0f;
	                    break;
	            }

	            vertices [j] += averageNormal * _fallOff * power;

			}


			mesh.vertices = vertices;
			mesh.RecalculateNormals ();
			mesh.RecalculateBounds ();
				
		}

		void ApplyMeshCollider(){
			if (unappliedMesh && unappliedMesh.GetComponent<MeshFilter> ()) {
				unappliedMesh.GetComponent<MeshFilter> ().mesh = unappliedMesh.mesh;		
			}
			unappliedMesh = null;
		}

		public void AssignedAllReference(){
			ClayMesh = null;
			ClayMesh = GameObject.FindGameObjectWithTag ("Mesh");
			
			texpaint = GameObject.FindGameObjectWithTag ("TP").GetComponent<VRTexturePainter> ();
			MeshTransform = ClayMesh.GetComponent<GetMeshTransform> ();
			dm = GameObject.FindGameObjectWithTag("SM").GetComponent<VRSmooth>();
			filter = ClayMesh.GetComponent<MeshFilter> ();
			//currentTool = Tools.Sculpt;
			point = rightController.GetComponent<VRTK_Pointer>();
		}


	}
}
