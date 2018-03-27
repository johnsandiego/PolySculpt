using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour {
	public Text sliderValueHolder;
	public Slider sliderValue;



	// Use this for initialization
	void Awake () {
		sliderValue = GetComponentInParent<Slider> ();
		sliderValueHolder = GetComponentInChildren<Text> ();

	}
	
	// FixedUpdate is called once per .05 sec
	void FixedUpdate () {
		StartCoroutine (UpdateValue ());
	}

	IEnumerator UpdateValue(){
		yield return new WaitForSeconds (1f);
		sliderValueHolder.text = sliderValue.value.ToString ();
	}
}
