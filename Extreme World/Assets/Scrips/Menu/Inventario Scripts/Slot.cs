using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Slot : MonoBehaviour {

	public int Id = -1;
	public int count = 0, countFake = 0;
	public int Numero;
	public bool selected = false, ItemFake = false;
	public float Tamanho;
	private void Update()
	{
		Tamanho = transform.GetComponent<RectTransform>().rect.width;
		selected = RectTransformUtility.RectangleContainsScreenPoint (GetComponent<RectTransform> (), Input.mousePosition);
	}

}