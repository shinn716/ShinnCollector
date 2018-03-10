﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShFadeAndDead : MonoBehaviour {

	[SerializeField]
	private bool _FadeOut;
	public bool FadeOut{
		get{ return _FadeOut; }
		set{ _FadeOut = value;}
	}

	public GameObject[] fadeObjs;
	public float FadeSpeed = 1;
	float lerpvalue=1;

	void Start(){
		lerpvalue = fadeObjs [0].GetComponent<Renderer>().material.color.a;
	}

	void FixedUpdate () {
		
		if(_FadeOut){
			if (lerpvalue < 0 + .01f) {

				lerpvalue = 0f;
				_FadeOut = false;
				for (int i = 0; i < fadeObjs.Length; i++) {
					fadeObjs [i].GetComponent<Renderer>().material.color = new Color (1, 1, 1, lerpvalue);
					Destroy (fadeObjs [i]);
				}

			} else
				lerpvalue = Mathf.Lerp (lerpvalue, 0, Time.fixedDeltaTime * FadeSpeed);


			for(int i=0; i<fadeObjs.Length; i++)
				fadeObjs [i].GetComponent<Renderer>().material.color = new Color (1,1,1, lerpvalue);
			

		}
	
	}
}
