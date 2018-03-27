using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GetSliderValue : MonoBehaviour {
	public Text sliderValueHolder;
	public Slider sliderValue;

	public bool experienceIsActive;
	float oneHundred = 4f;
	float exp;

	// Use this for initialization
	void Start () {
		sliderValue = GetComponent<Slider> ();
		sliderValueHolder = GetComponentInChildren<Text> ();

	}
	
	// FixedUpdate is called once per .04 sec
	void FixedUpdate () {
		//sliderValueHolder.text = exp;

		if (experienceIsActive) {
			exp = sliderValue.value;
			sliderValueHolder.text = (exp * oneHundred).ToString ();
		} else {
			sliderValueHolder.text = sliderValue.value.ToString();
		}
	}
}
