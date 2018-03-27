using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace sandiegoJohn.VRsculpting{

	public class BackButton : MonoBehaviour {

		public GameObject ToolPanel;
		public GameObject Sculpt, Scale, Rotate, Pull, Grab, Smooth, Paint;
		public GameObject UINRef;
		public UINavigationSystem Navref;
		public GameObject ColorPicker;



		// Use this for initialization
		void Start () {
			ToolPanel = GameObject.FindGameObjectWithTag ("toolpanel");
				UINRef = GameObject.FindGameObjectWithTag ("UINref");
		}
		
		// Update is called once per frame
		void Update () {
			Navref = UINRef.GetComponent<UINavigationSystem> ();
		}

		public void BackToMenu(){
				
				//Debug.Log ("back is pressed");
			foreach (GameObject element in Navref.ToolsArray) {
				element.SetActive(false);
				}
			Navref.MenuSounds.Play ();
			ToolPanel.SetActive (true);
			ColorPicker.SetActive (false);

		}

	}
}