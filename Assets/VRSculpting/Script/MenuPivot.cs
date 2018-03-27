using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPivot : MonoBehaviour {

	public Transform PivotPoint;
	public Button CurrentButton;
	public float axis = 0f;
	public Scrollbar mainBar;
	// Use this for initialization
	void Start () {
		//transform.RotateAround (PivotPoint.position, Vector3.up, 90f);
	}
	
	// Update is called once per frame
	void Update () {


		//if (Input.GetKey (KeyCode.RightArrow)) {

			transform.RotateAround (PivotPoint.position, new Vector3(0 ,axis ,0) ,  mainBar.value * 90f);
			transform.rotation = Quaternion.identity;

		//}
	}
}
