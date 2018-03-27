using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using VRTK;
using System;
public class ToolPreviewHover : MonoBehaviour {

	VideoPlayer VD;
	public GameObject Video;
	public VRTK_UIPointer UIPointer;
	public VideoClip[] VideoClipArr;

	// Use this for initialization
	void Start () {
		UIPointer = GetComponent<VRTK_UIPointer> ();
		VD = Video.GetComponent<VideoPlayer> ();

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		try{
			if (UIPointer.hoveringElement.name == "Sculpt Button") {
				VD.clip = VideoClipArr[0];
				VD.Play();
			}else if (UIPointer.hoveringElement.name == "Scale Button") {
				VD.clip = VideoClipArr[1];
				VD.Play();
			}else if (UIPointer.hoveringElement.name == "Rotate Button") {
				VD.clip = VideoClipArr[2];
				VD.Play();
			}else if (UIPointer.hoveringElement.name == "Crease Button") {
				VD.clip = VideoClipArr[3];
				VD.Play();
			}else if (UIPointer.hoveringElement.name == "Grab Toggle") {
				VD.clip = VideoClipArr[4];
				VD.Play();
			}else if (UIPointer.hoveringElement.name == "Symmetry Toggle") {
				VD.clip = VideoClipArr[5];
				VD.Play();
			}else if (UIPointer.hoveringElement.name == "Smooth Button") {
				VD.clip = VideoClipArr[6];
				VD.Play();
			}else if (UIPointer.hoveringElement.name == "Paint Button") {
				VD.clip = VideoClipArr[7];
				VD.Play();
			}
			else {
				VD.Stop();
			}

		}catch(Exception ex){
			if (ex is NullReferenceException || ex is MissingComponentException || ex is UnassignedReferenceException || ex is MissingReferenceException) {
				return;
			}
			throw;
		}
	}
}
