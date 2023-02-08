using UnityEngine;
using System.Collections;

public class CamDirect : MonoBehaviour {
	private GameObject emptObj;
	private bool ready;
	private Vector3 mousScreen;
	private VectDirect vectDirect=new VectDirect();
	
	void Start () {
		emptObj=new GameObject("ControlObj");
	}
	
	void Update () {
		mousScreen=new Vector3(Input.mousePosition.x-(Screen.width/2F),Input.mousePosition.y-(Screen.height/2F),0F);
		StartCoroutine ("mouseLook");
	}
	IEnumerator mouseLook(){
		ready=true;
		emptObj.transform.forward=vectDirect.CalcVect(mousScreen,1.3F);
		transform.localEulerAngles=emptObj.transform.eulerAngles;
		yield return new WaitForEndOfFrame();
		ready=false;
	}
}
