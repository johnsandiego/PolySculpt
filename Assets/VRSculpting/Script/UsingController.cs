using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingController : MonoBehaviour {

	private SteamVR_TrackedController _controller;

	void OnEnable(){
		_controller = GetComponent<SteamVR_TrackedController> ();
		_controller.TriggerClicked += _controller_TriggerClicked;
	}

	void OnDisable(){
		_controller.TriggerClicked -= _controller_TriggerClicked;

	}

	void _controller_TriggerClicked (object sender, ClickedEventArgs e)
	{
		//Debug.Log ("trigger is Pressed");

	}
		
}
