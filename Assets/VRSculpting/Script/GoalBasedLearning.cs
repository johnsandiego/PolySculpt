/*
 * Author: John San Diego
 * Project Name: VRSculpting App
 * Date:3/16/2018
 * Brief: The goalbased learning takes multiple variables and over time the user earns experience by using the tools
 * 	Variables: Controllers inputs - trigger, TouchPad, GribButton, Mesh Transform, Paint
 *  goals list are: sculpting
 * 					scale
 * 					rotate
 * 					crease 
 * 					grab
 * 					symmetry
 * 					smooth
 * 					paint
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using VRTK;



namespace sandiegoJohn.VRsculpting{

	public class GoalBasedLearning : MonoBehaviour {

		float timeRequiredToReachGoal = 1f;
		public float goalTime = 1f;
		public GameObject[] MainTools;
		Stack<GameObject> ToolList = new Stack<GameObject>();
		public GameObject currentTool;
		public GameObject[] currentTools;
		public GameObject Star;
		public GameObject[] goals;
		public Slider[] expPointsliders;
		PaintMeshDeformer PMD;
		public VRTK_ControllerEvents _controller;
		int count = 1;
		public GameObject goalPanel;
		public GameObject CloseGoal, OpenGoal;
		public float ExpGlobalAmount = 25f;
		public bool[] toolsboolean;

		public Button skipButton;
		public Button RestartGoal;
		UINavigationSystem UINAV;

		public TypeOfExperience Exp;

		//holds all the different tools experience points
		public struct TypeOfExperience{
			public float sculptEXP ;
			public float scaleEXP;
			public float rotateEXP;
			public float creaseEXP;
			public float grabEXP;
			public float symmetryEXP;
			public float smoothEXP;
			public float paintEXP;

			public float RequiredsculptEXP;
			public float RequiredscaleEXP;
			public float RequiredrotateEXP;
			public float RequiredcreaseEXP;
			public float RequiredgrabEXP;
			public float RequiredsymmetryEXP;
			public float RequiredsmoothEXP;
			public float RequiredpaintEXP;

		}

		// Use this for initialization
		void Start () {
			for (int i = 0; i < MainTools.Length; i++) {
				ToolList.Push (MainTools [i]);
			}

			PMD = GameObject.FindGameObjectWithTag ("PMD").GetComponent<PaintMeshDeformer> ();
			UINAV = GameObject.FindGameObjectWithTag ("UINref").GetComponent<UINavigationSystem>();
			foreach (GameObject tl in MainTools) {
				tl.SetActive (false);
			}


			MainTools [0].SetActive (true);

			//goals = GameObject.FindGameObjectsWithTag ("goals");
			foreach (GameObject gl in goals) {
				gl.SetActive (false);
			}
			goals [0].SetActive (true);

			//required points to moved to next goal, then new tools get unlock
			Exp.RequiredsculptEXP = ExpGlobalAmount;
			Exp.RequiredscaleEXP = ExpGlobalAmount/2;
			Exp.RequiredrotateEXP = ExpGlobalAmount/2;
			Exp.RequiredcreaseEXP = ExpGlobalAmount/2;
			Exp.RequiredgrabEXP = ExpGlobalAmount/2;
			Exp.RequiredsymmetryEXP = ExpGlobalAmount/2;
			Exp.RequiredsmoothEXP = ExpGlobalAmount/2;
			Exp.RequiredpaintEXP = ExpGlobalAmount;

			foreach (Slider sli in expPointsliders) {
				sli.maxValue = ExpGlobalAmount;
			}

			for (int i = 0; i < toolsboolean.Length; i++) {
				toolsboolean [i] = false;
			}
		}
		
		// Update is called once per frame
		void FixedUpdate () {
			try{
				/*checks to see if the player is in one of the menu page
				 * then a assign the current experience point to slider UI.
				 * then check if the user has pressed the trigger and the pointer is pointing at the mesh
				 * if true, decrement timerequiredtoreachgoal over deltaTime
				 * Next, if Timerequiretoreachgoal is equal to zero
				 * increment tools experience, then reset the time. 
				 * 
				 * Then if tool experience reaches the required experience
				 * A new tool is activated, then a new goal is presented.
				 * 
				*/
				//Debug.Log("grip is pressed: " + PMD._control.gripClicked);
				//Controls goals 
				if(Input.GetKeyDown(KeyCode.T)){
					Exp.sculptEXP = ExpGlobalAmount;
				}else if(Input.GetKeyDown(KeyCode.Y)){
					Exp.scaleEXP = ExpGlobalAmount;
				}else if(Input.GetKeyDown(KeyCode.U)){
					Exp.rotateEXP = ExpGlobalAmount;
				}else if(Input.GetKeyDown(KeyCode.I)){
					Exp.creaseEXP = ExpGlobalAmount;
				}else if(Input.GetKeyDown(KeyCode.O)){
					Exp.grabEXP = ExpGlobalAmount;
				}else if(Input.GetKeyDown(KeyCode.P)){
					Exp.symmetryEXP = ExpGlobalAmount;
				}else if(Input.GetKeyDown(KeyCode.K)){
					Exp.smoothEXP = ExpGlobalAmount;
				}else if(Input.GetKeyDown(KeyCode.L)){
					Exp.paintEXP = ExpGlobalAmount;
				}

				if (PMD.currentTool == Tools.Sculpt) {
					

					if(PMD.isTriggerOn){
						//Debug.Log("name: " + PMD.hit.collider.name);
						timeRequiredToReachGoal -= Time.deltaTime;
						//Debug.Log("Time Required: "+ timeRequiredToReachGoal);
						if(timeRequiredToReachGoal < 0 && Exp.sculptEXP < Exp.RequiredsculptEXP/2){
							Exp.sculptEXP++;
							timeRequiredToReachGoal = goalTime;
						}

					}
					//Debug.Log(Exp.sculptEXP == Exp.RequiredsculptEXP);
					if (Exp.sculptEXP >= Exp.RequiredsculptEXP && toolsboolean[0] == false) {
						MainTools[count].SetActive(true);
						goals[count].SetActive(true);
						goals[count-1].SetActive(false);

						count++;
						toolsboolean[0] = true;
					}
					expPointsliders[0].value = (int)Exp.sculptEXP;
					SculptClickEvent();

				}else if(PMD.currentTool == Tools.Scale){
					expPointsliders[1].value = Exp.scaleEXP;
					if(PMD.touchPadIsOn){
						timeRequiredToReachGoal -= Time.deltaTime;
						if(timeRequiredToReachGoal < 0){
							Exp.scaleEXP++;
							timeRequiredToReachGoal = goalTime;

						}

						if (Exp.scaleEXP >= Exp.RequiredscaleEXP && toolsboolean[1] == false) {
							MainTools[count].SetActive(true);
							goals[count].SetActive(true);
							goals[count-1].SetActive(false);
							count++;
							toolsboolean[1] = true;
						}
					}

					expPointsliders[1].value = Exp.scaleEXP;
					ScaleClickEvent();

				}else if(PMD.currentTool == Tools.Rotate){
					expPointsliders[2].value = Exp.rotateEXP;
					if(PMD.touchPadIsOn){
						timeRequiredToReachGoal -= Time.deltaTime;
						if(timeRequiredToReachGoal < 0){
							Exp.rotateEXP++;
							timeRequiredToReachGoal = goalTime;
						}

						if (Exp.rotateEXP >= Exp.RequiredrotateEXP && toolsboolean[2] == false) {
							MainTools[count].SetActive(true);
							goals[count].SetActive(true);
							goals[count-1].SetActive(false);
							count++;
							toolsboolean[2] = true;
						}
					}
				}else if(PMD.currentTool == Tools.Crease){
					expPointsliders[3].value = Exp.creaseEXP;
					if(PMD.isTriggerOn || PMD.touchPadIsOn){
						timeRequiredToReachGoal -= Time.deltaTime;
						if(timeRequiredToReachGoal < 0){
							Exp.creaseEXP++;
							timeRequiredToReachGoal = goalTime;
						}

						if (Exp.creaseEXP >= Exp.RequiredcreaseEXP && toolsboolean[3] == false) {
							MainTools[count].SetActive(true);
							goals[count].SetActive(true);
							goals[count-1].SetActive(false);
							count++;
							toolsboolean[3] = true;
						}
					}
				}else if(PMD.currentTool == Tools.Grab){
					expPointsliders[4].value = Exp.grabEXP;
					if(PMD.ClayMesh == null){
						PMD.ClayMesh = GameObject.FindGameObjectWithTag("Mesh");
						PMD.MeshTransform = PMD.ClayMesh.GetComponent<GetMeshTransform>();
					}
					if(PMD.ClayMesh.GetComponent<VRTK_InteractableObject>().IsGrabbed()){
						//Debug.Log("GRIP is ON!!!");
						timeRequiredToReachGoal -= Time.deltaTime;
						//Debug.Log("timerequired: "+timeRequiredToReachGoal);
						if(timeRequiredToReachGoal < 0){
							Exp.grabEXP++;
							timeRequiredToReachGoal = goalTime;
						}

						if (Exp.grabEXP >= Exp.RequiredgrabEXP && toolsboolean[4] == false) {
							MainTools[count].SetActive(true);
							goals[count].SetActive(true);
							goals[count-1].SetActive(false);
							count++;
							toolsboolean[4] = true;
						}
					}
				}else if(PMD.currentTool == Tools.Symmetry){
					expPointsliders[5].value = Exp.symmetryEXP;
					if(PMD.touchPadIsOn){
						timeRequiredToReachGoal -= Time.deltaTime;
						if(timeRequiredToReachGoal < 0){
							Exp.symmetryEXP++;
							timeRequiredToReachGoal = goalTime;
						}

						if (Exp.symmetryEXP >= Exp.RequiredsymmetryEXP && toolsboolean[5] == false) {
							MainTools[count].SetActive(true);
							goals[count].SetActive(true);
							goals[count-1].SetActive(false);
							count++;
							toolsboolean[5] = true;
						}
					}
				}else if(PMD.currentTool == Tools.Smooth){
					expPointsliders[6].value = Exp.smoothEXP;
					if(PMD.touchPadIsOn){
						timeRequiredToReachGoal -= Time.deltaTime;
						if(timeRequiredToReachGoal < 0){
							Exp.smoothEXP++;
							timeRequiredToReachGoal = goalTime;
						}

						if (Exp.smoothEXP >= Exp.RequiredsmoothEXP && toolsboolean[6] == false) {
							MainTools[count].SetActive(true);
							goals[count].SetActive(true);
							goals[count-1].SetActive(false);
							count++;
							toolsboolean[6] = true;
							//Debug.Log("count: " + count);
						}
					}
				}else if(PMD.currentTool == Tools.Paint){
					expPointsliders[7].value = Exp.paintEXP;
					if(PMD.isTriggerOn){
						timeRequiredToReachGoal -= Time.deltaTime;
						if(timeRequiredToReachGoal < 0){
							Exp.paintEXP++;
							timeRequiredToReachGoal = goalTime;
						}

						if (Exp.paintEXP >= Exp.RequiredpaintEXP && toolsboolean[7] == false) {
							MainTools[7].SetActive(true);
							goals[7].SetActive(true);
							goals[6].SetActive(false);
							count++;
							toolsboolean[7] = true;
							//Debug.Log("count: " + count);

						}
					}
				}



				//Debug.Log("Sculpt EXP: " + Exp.sculptEXP);
//				Debug.Log("Scale EXP: " + Exp.scaleEXP);
//				Debug.Log("Rotate EXP: " + Exp.rotateEXP);
//				Debug.Log("Sculpt EXP: " + Exp.creaseEXP);
//				Debug.Log("Sculpt EXP: " + Exp.grabEXP);
//				Debug.Log("Sculpt EXP: " + Exp.symmetryEXP);
//				Debug.Log("Sculpt EXP: " + Exp.smoothEXP);
//				Debug.Log("Sculpt EXP: " + Exp.paintEXP);


			}catch(Exception ex){
				if (ex is NullReferenceException || ex is MissingComponentException) {
					return;
				}
				throw;
			}

		}
		public void SculptClickEvent(){
			if (EventSystem.current.IsPointerOverGameObject () && PMD.touchPadIsOn) {
				Exp.sculptEXP += .010f;
			}
		}

		public void SculptSliderValueChange(){
			
				Exp.sculptEXP += .010f;
		}
		public void ScaleSliderValueChange(){
			Exp.scaleEXP += .010f;
		}
		public void RotateSliderValueChange(){
			Exp.rotateEXP += .010f;
		}
		public void CreaseSliderValueChange(){
			Exp.creaseEXP += .010f;
		}
		public void SmoothSliderValueChange(){
			Exp.smoothEXP += .010f;
		}

		public void ScaleClickEvent(){
			if (EventSystem.current.IsPointerOverGameObject () && PMD.touchPadIsOn) {
				Debug.Log ("SculptOpacitySlider is USED!!!!!");
				Exp.scaleEXP += .010f;
			}
		}
		public void RotateClickEvent(){
			if (EventSystem.current.IsPointerOverGameObject () && PMD.touchPadIsOn) {
				Debug.Log ("SculptOpacitySlider is USED!!!!!");
				Exp.rotateEXP += .010f;
			}
		}
		public void CreaseClickEvent(){
			if (EventSystem.current.IsPointerOverGameObject () && PMD.touchPadIsOn) {
				Debug.Log ("SculptOpacitySlider is USED!!!!!");
				Exp.creaseEXP += .010f;
			}
		}
		public void SmoothClickEvent(){
			if (EventSystem.current.IsPointerOverGameObject () && PMD.touchPadIsOn) {
				Debug.Log ("SculptOpacitySlider is USED!!!!!");
				Exp.smoothEXP += .010f;
			}
		}

		public void SkipGoals(){
			Exp.sculptEXP = ExpGlobalAmount;
			Exp.scaleEXP = ExpGlobalAmount;
			Exp.rotateEXP = ExpGlobalAmount;
			Exp.creaseEXP = ExpGlobalAmount;
			Exp.grabEXP = ExpGlobalAmount;
			Exp.symmetryEXP = ExpGlobalAmount;
			Exp.smoothEXP = ExpGlobalAmount;
			Exp.paintEXP = ExpGlobalAmount;
			goalPanel.SetActive (false);
			CloseGoal.SetActive (false);
			OpenGoal.SetActive (false);

			for (int k = 0; k < MainTools.Length; k++) {
				MainTools [k].SetActive (true);
			}
			RestartGoal.gameObject.SetActive (true);
			skipButton.gameObject.SetActive (false);
		}

		public void RestartGL(){
			Exp.sculptEXP = 0f;
			Exp.scaleEXP = 0f;
			Exp.rotateEXP = 0f;
			Exp.creaseEXP = 0f;
			Exp.grabEXP = 0f;
			Exp.symmetryEXP = 0f;
			Exp.smoothEXP = 0f;
			Exp.paintEXP = 0f;

			goalPanel.SetActive (true);
			CloseGoal.SetActive (true);
			OpenGoal.SetActive (true);

			for (int k = 0; k < MainTools.Length; k++) {
				MainTools [k].SetActive (false);
			}
			MainTools [0].SetActive (true);

			timeRequiredToReachGoal = 1f;

			RestartGoal.gameObject.SetActive (false);
			skipButton.gameObject.SetActive (true);
		}

		public bool GoalReached(float timeRequired){
			if (timeRequired < 1f) {
				return true;
			} else
				return false;
		}
		public void turnOffGoals(){
			goalPanel.SetActive (false);
		}
		public void turnOnGoals(){
			goalPanel.SetActive (true);
		}
		public void resetExperiencePoints(){
			Exp.sculptEXP = 0;
			Exp.scaleEXP = 0;
			Exp.rotateEXP = 0;
			Exp.creaseEXP = 0;
			Exp.grabEXP = 0;
			Exp.symmetryEXP = 0;
			Exp.smoothEXP = 0;
			Exp.paintEXP = 0;
		}
	}

}