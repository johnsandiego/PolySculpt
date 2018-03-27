using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour {


	public GameObject cube;
	public float speed; 
	public GameObject[] arrCube;
	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	//loos
	void Update () {
		Movement ();
//		if (Input.GetKey (KeyCode.Space)) {
		//loop - for
		//foreach

//			for (int smallCube = 0; smallCube < 100; smallCube++) {
//				cube.transform.Translate(new Vector3(0,0,speed));
//				cube.transform.Translate(new Vector3(-speed,0,0));
//				cube.transform.Translate(new Vector3(0,0,-speed));
//				cube.transform.Translate(new Vector3(speed,0,0));
//				Debug.Log ("whats the problem im working");
//			}
//		}




	}

	//FUnction
	public void Movement(){
		if (Input.GetKey(KeyCode.W)) {
			cube.transform.Translate(new Vector3(0,0,speed*Time.deltaTime));

		}
		if (Input.GetKey (KeyCode.S)) {
			cube.transform.Translate(new Vector3(0,0,-speed*Time.deltaTime));
		}
		if (Input.GetKey (KeyCode.A)) {
			cube.transform.Translate(new Vector3(-speed*Time.deltaTime,0,0));

		}
		if (Input.GetKey (KeyCode.D)) {
			cube.transform.Translate(new Vector3(speed*Time.deltaTime,0,0));

		}
		if (Input.GetKey (KeyCode.Q)) {
			cube.transform.Translate(new Vector3(0,speed*Time.deltaTime,0));

		}
		if (Input.GetKey (KeyCode.E)) {
			cube.transform.Translate(new Vector3(0,-speed*Time.deltaTime,0));

		}
		if (Input.GetKey (KeyCode.R)) {
			cube.transform.rotation = new Quaternion (0, speed * Time.deltaTime, 0, 0);
		}
		if (Input.GetKey (KeyCode.T)) {
			cube.transform.Rotate (new Vector3 (0, -speed * Time.deltaTime * 10, 0));
		}
	}

	void Shoot(){
		
	}



}
