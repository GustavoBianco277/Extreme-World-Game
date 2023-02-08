using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigAnimation : MonoBehaviour {


	private Agachar agachar;
	private Nadar Nadar;
	public bool VaiNadar;
	void Start () {
		agachar = GetComponent<Agachar> ();  
		Nadar  = GetComponent<Nadar> (); 
	}
	void Update () {
		if (VaiNadar == true) {
			Nadar.enabled = true;
			agachar.enabled = false;
		} else {
			Nadar.enabled = false;
			agachar.enabled = true;
		}
	}
}
