using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LOAD : MonoBehaviour {
	public string cenaACarregar;
	public float TempoFixoSeg = 5;
	public enum TipoCarreg {Carregamento, TempoFixo};
	public TipoCarreg TipoDeCarregamento;
	public Image barraDeCarregamento;
	public TextMeshProUGUI TextoProgresso;
	private int progresso = 0;

	void Start () 
	{
		cenaACarregar = FindObjectOfType<servidor>().SceneName;
		switch (TipoDeCarregamento) 
		{
			case TipoCarreg.Carregamento:
				StartCoroutine (CenaDeCarregamento (cenaACarregar));
				break;
			case TipoCarreg.TempoFixo:
				StartCoroutine (TempoFixo (cenaACarregar));
				break;
		}
		//
		if (barraDeCarregamento != null) {
			barraDeCarregamento.type = Image.Type.Filled;
			barraDeCarregamento.fillMethod = Image.FillMethod.Horizontal;
			barraDeCarregamento.fillOrigin = (int)Image.OriginHorizontal.Left;
		}
	}

	IEnumerator CenaDeCarregamento(string cena)
	{
		AsyncOperation carregamento = SceneManager.LoadSceneAsync (cena);
		while (!carregamento.isDone) 
		{
			progresso = (int)(carregamento.progress * 100.0f);
			yield return null;
		}
	}

	IEnumerator TempoFixo(string cena){
		yield return new WaitForSeconds (TempoFixoSeg);
		SceneManager.LoadScene (cena);
	}

	void Update () 
	{
		Cursor.visible = false;
		switch (TipoDeCarregamento) 
		{
			case TipoCarreg.Carregamento:
				break;
			case TipoCarreg.TempoFixo:
				progresso = (int)(Mathf.Clamp((Time.time / TempoFixoSeg),0.0f,1.0f)* 100.0f);
				break;
		}
		if (TextoProgresso != null) 
			TextoProgresso.text = progresso + "%";

		if (barraDeCarregamento != null) 
			barraDeCarregamento.fillAmount = (progresso / 100.0f);
	}
}