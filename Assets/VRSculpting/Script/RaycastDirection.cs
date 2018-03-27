using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDirection : MonoBehaviour {
	public float range = 0.0f;
	public LayerMask mask = -1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 forward = transform.TransformDirection(Vector3.forward) * range;
		Debug.DrawRay(transform.position, forward, Color.green);

		RaycastHit hit;

		if (Physics.Raycast (transform.position, forward, out hit, range)) {
			Debug.Log (hit.collider.name);
		}
	}
}
