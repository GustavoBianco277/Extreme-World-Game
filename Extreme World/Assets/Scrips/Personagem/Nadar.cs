using UnityEngine;
using System.Collections;
public class Nadar : MonoBehaviour {
	public float VelocidadeNadando = 1;
	public float AlturaControllerNadando = 0.9f;
	public CharacterController controller;
	private ConfigAnimation Nadando;

	private CharacterController characterController;
	public float EscalaDoObjeto,VelocidadeAtual;
	public bool EstaNadando,EstaCorrendo;
	void Start () {
		controller = GetComponent<CharacterController>();
        Nadando = GetComponent<ConfigAnimation>();
        characterController = GetComponent<CharacterController> ();
		EscalaDoObjeto = characterController.height;
	}
	void FixedUpdate (){
		if (Input.GetKey ("w") && Nadando.VaiNadar == true) {
			EstaNadando = true;
			EscalaDoObjeto = 2f;
			controller.center = new Vector3 (0, AlturaControllerNadando, 0);
			VelocidadeAtual = VelocidadeNadando;
		} else if (EstaCorrendo == false) {
			EstaNadando = false;
            EscalaDoObjeto = 2;
			controller.center = new Vector3 (0, 1, 0);
		}

		characterController.height = Mathf.Lerp (characterController.height, EscalaDoObjeto, 3 * Time.deltaTime);
	}
}