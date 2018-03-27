using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

namespace sandiegoJohn.VRsculpting{

	public class UINavigationSystem : MonoBehaviour {

		PaintMeshDeformer PMD;
		public GameObject ToolsPage;
		public GameObject[] ToolsArray;
		GameObject grab, symmetry;
		//used for scaling the mesh - sliderUI
		public Slider[] Scalexyz;
		public Slider[] Rotatexyz;
		public Slider CreaseStrength;
		//toggle to uniformly Scale the mesh
		public Toggle uniformScale;
		public Toggle Grab;

		public GameObject ToolButtons;
		public GameObject colorpicker;

		public AudioSource MenuSounds;

		public GameObject[] dividers;

		public VRTK_ControllerTooltips tooltip;
		float length;
		bool UsingGrab = false;
		VRTexturePainter VRTP;
		public bool isActive;

		void Awake(){
			ToolsArray = new GameObject[6];
			ToolsArray [0] = GameObject.FindGameObjectWithTag ("sculpt");
			ToolsArray [1] = GameObject.FindGameObjectWithTag ("scale");
			ToolsArray [2] = GameObject.FindGameObjectWithTag ("rotate");
			ToolsArray [3] = GameObject.FindGameObjectWithTag ("Crease");
			ToolsArray [4] = GameObject.FindGameObjectWithTag ("smooth");
			ToolsArray [5] = GameObject.FindGameObjectWithTag ("paint");
			grab = GameObject.FindGameObjectWithTag ("grab");
			symmetry = GameObject.FindGameObjectWithTag ("symmetry");
			CreaseStrength = ToolsArray [3].GetComponentInChildren<Slider> ();

			foreach (GameObject tools in ToolsArray) {
				tools.SetActive (false);
			}
			PMD = GameObject.FindGameObjectWithTag ("PMD").GetComponent<PaintMeshDeformer>();
			VRTP = GameObject.FindGameObjectWithTag ("TP").GetComponent<VRTexturePainter> ();

			tooltip = GameObject.FindGameObjectWithTag ("tooltip").GetComponent<VRTK_ControllerTooltips> ();
		}

		// Update is called once per frame
		void Update () {

		}
		public void GrabToggle(){
			if (grab.GetComponent<Toggle> ().isOn) {
				PMD.currentTool = Tools.Grab;
				MenuSounds.Play ();
				tooltip.UpdateText (VRTK_ControllerTooltips.TooltipButtons.GripTooltip, "Grip");
			} else if (!grab.GetComponent<Toggle> ().isOn) {
				MenuSounds.Play ();
				tooltip.UpdateText (VRTK_ControllerTooltips.TooltipButtons.GripTooltip, "");
			}
		}

		public void SymmetryToggle(){
			if (symmetry.GetComponent<Toggle> ().isOn) {
				PMD.currentTool = Tools.Symmetry;
				dividers [0].SetActive (true);
				dividers [1].SetActive (true);
				MenuSounds.Play ();
				//tooltip.UpdateText (VRTK_ControllerTooltips.TooltipButtons.GripTooltip, "Grip");
			} else if (!symmetry.GetComponent<Toggle> ().isOn) {
				dividers [0].SetActive (false);
				dividers [1].SetActive (false);
				MenuSounds.Play ();
				//tooltip.UpdateText (VRTK_ControllerTooltips.TooltipButtons.GripTooltip, "");
			}
		}

		public void assignedAllUI(){
			Scalexyz = ToolsArray[1].GetComponentsInChildren<Slider> ();
			Rotatexyz = ToolsArray [2].GetComponentsInChildren<Slider> ();
			uniformScale = ToolsArray [1].GetComponentInChildren<Toggle> ();
			CreaseStrength = ToolsArray [3].GetComponentInChildren<Slider> ();
			isActive = false;

		}

		public void SculptPage(){
			
			foreach (GameObject tools in ToolsArray) {
				tools.SetActive (false);
			}

			ToolButtons.SetActive (false);
			ToolsArray [0].SetActive (true);
			PMD.currentTool = Tools.Sculpt;
			VRTP.enabled = false;
			colorpicker.SetActive (false);
			isActive = true;
			MenuSounds.Play ();
		}
		public void ScalePage(){
			foreach (GameObject tools in ToolsArray) {
				tools.SetActive (false);
			}
			ToolButtons.SetActive (false);
			ToolsArray [1].SetActive (true);
			PMD.currentTool = Tools.Scale;
			VRTP.enabled = false;
			colorpicker.SetActive (false);
			isActive = false;
			MenuSounds.Play ();
		}
		public void RotatePage(){
			foreach (GameObject tools in ToolsArray) {
				tools.SetActive (false);
			}
			ToolButtons.SetActive (false);
			ToolsArray [2].SetActive (true);
			PMD.currentTool = Tools.Rotate;
			VRTP.enabled = false;
			colorpicker.SetActive (false);
			isActive = false;
			MenuSounds.Play ();
		}
		public void CreasePage(){
			foreach (GameObject tools in ToolsArray) {
				tools.SetActive (false);
			}
			ToolButtons.SetActive (false);
			ToolsArray [3].SetActive (true);
			PMD.currentTool = Tools.Crease;
			VRTP.enabled = false;
			colorpicker.SetActive (false);
			isActive = true;
			MenuSounds.Play ();
		}
		public void SmoothPage(){
			foreach (GameObject tools in ToolsArray) {
				tools.SetActive (false);
			}
			ToolButtons.SetActive (false);
			ToolsArray [4].SetActive (true);
			PMD.currentTool = Tools.Smooth;
			VRTP.enabled = false;
			colorpicker.SetActive (false);
			isActive = false;
			MenuSounds.Play ();
			//PMD.dm.assignUI ();
			//Debug.Log ("in smooth page");
		}

		public void PaintPage(){
			foreach (GameObject tools in ToolsArray) {
				tools.SetActive (false);
			}
			ToolButtons.SetActive (false);
			ToolsArray [5].SetActive (true);
			PMD.currentTool = Tools.Paint;
			colorpicker.SetActive (true);
			isActive = true;
			MenuSounds.Play ();
		}

	}
}