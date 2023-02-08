using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

//[RequireComponent(typeof(AudioSource))]
[System.Serializable]
public class Tools
{
	public string Name;
	public int id;
	public int Dano;
	public float TempoPorAtaque;
	public float DistanciaMin;
}
public class DerrubarArvores : MonoBehaviour 
{
	public Camera cameraPrincipal;
	public float DistanciaMinima = 3, DistanciaColetar = 5, TempoPorAtaque = 1;
	public int DanoCausadoAxe, DanoCausadoPickaxe;
	public Texture2D mira;
	public Image ShowDestroy;
	public AudioSource SoundAxe;
	public AudioSource[] BackPackSound;
	public List<AudioSource> SoundsPickaxe = new List<AudioSource>();
	public List<Tools> Ferramentas = new List<Tools>();

	// privates
	private float Distance;
	private bool PodeAtacar = true;
	private Inv inventario;
	private RaycastHit colisor;
	private Ray CentroDaTela;
	private Rock ScriptRock;
	private ARVORE ScriptArvore;
	private Chest SelecChest;
	private Linguagem Ling;
	[HideInInspector] public Animacoes Anim;
	void Start () 
	{
		cameraPrincipal = Camera.main;
		Cursor.visible = false;
		inventario = FindObjectOfType<Inv>();
		Ling = FindObjectOfType<Linguagem>();
	}
	void Update() 
	{
		// Cria o Raycast
		CentroDaTela = cameraPrincipal.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		bool colisao = Physics.Raycast(CentroDaTela, out colisor);

		if (colisor.transform != null)
			Distance = Vector3.Distance(transform.position, colisor.transform.position);

        // Desativa o ShowDestroy
        if (!colisao || colisor.transform.gameObject.tag != "Rock" || colisor.transform.gameObject.tag != "ARVORE" || Distance > DistanciaMinima && ShowDestroy.gameObject.active)
			ShowDestroy.gameObject.SetActive(false);

		if (!colisao || !colisor.transform.GetComponent<Chest>() || Distance > DistanciaMinima)
		{
			if (SelecChest != null)
			{
				SelecChest.Selected = false;
				SelecChest = null;
			}
		}

		if (colisao && !GunsControl.GunsMode)
		{
			// Se esta mais perto do que a distancia minima
			if (Distance <= DistanciaMinima)
			{
				// Se eu olhar para uma Pedra
				if (colisor.transform.tag == "Rock")
				{
					ScriptRock = colisor.transform.GetComponent<Rock>();
					if (ScriptRock.Vida > 0)
						ShowDestroy.gameObject.SetActive(true);

					float Fill = 100.0f / ScriptRock.MaxVida * ScriptRock.Vida;
					ShowDestroy.fillAmount = Fill / 100;

					if (ShowDestroy.fillAmount == 0)
						ShowDestroy.gameObject.SetActive(false);
				}

				// Se eu olhar para uma arvore
				else if (colisor.transform.tag == "ARVORE")
				{
					ScriptArvore = colisor.transform.GetComponent<ARVORE>();
					if (ScriptArvore.VIDA > 0)
						ShowDestroy.gameObject.SetActive(true);

					float Fill = 100.0f / ScriptArvore.MaxVida * ScriptArvore.VIDA;
					ShowDestroy.fillAmount = Fill / 100;

					if (ShowDestroy.fillAmount == 0)
						ShowDestroy.gameObject.SetActive(false);
				}

				// Se eu bater
				if (Input.GetMouseButtonDown(0) && PodeAtacar)
				{
					// Se eu bati numa arvore
					if (colisor.transform.tag == "ARVORE" && colisor.transform.GetComponent<ARVORE>().enabled)
						StartCoroutine(Hit(0));

					// se eu bati numa pedra
					else if (colisor.transform.tag == "Rock" && DanoCausadoPickaxe != 0)
						StartCoroutine(Hit(1));

					else if (colisor.transform.tag == "Animal")
					{
						StartCoroutine(Hit(Animal: colisor.transform));
					}
				}
			}
			if (Distance <= DistanciaColetar)
			{
				// Se eu olhar para um Coletavel
				/*if (colisor.transform.tag == "Coletavel")
					Ling.Interagir(InteragirEnum.Pegar_Item);*/

				if (colisor.transform.GetComponent<Chest>())
				{
					if (colisor.transform.GetComponent<Chest>() != SelecChest)
					{
						if (SelecChest != null)
							SelecChest.Selected = false;
						SelecChest = colisor.transform.GetComponent<Chest>();
						SelecChest.Selected = true;
					}
				}

				// coletar
				else if (Input.GetKeyDown("e") && colisor.transform.tag == "Coletavel")
				{
					bool Ok = inventario.addItem(colisor.transform.gameObject.GetComponent<itemObject>().id, 1);
					if (PhotonNetwork.IsConnected && Ok)
					{
						//servidor.Server.RPC("Destroy", RpcTarget.All, colisor.transform.GetComponent<PhotonView>().ViewID, 0.0f);
						if (PhotonNetwork.IsMasterClient)
							PhotonNetwork.Destroy(colisor.transform.GetComponent<PhotonView>());

						else
							servidor.Server.RPC("Destroy", RpcTarget.MasterClient, colisor.transform.GetComponent<PhotonView>().ViewID);

						BackPackSound[Random.Range(0, BackPackSound.Length)].Play();
					}
					else if (Ok)
					{
						Destroy(colisor.transform.gameObject);
						BackPackSound[Random.Range(0, BackPackSound.Length)].Play();
					}
					else
						print("Nao recolho");
				}

				else if (Input.GetKeyDown("e") && colisor.transform.GetComponent<CampFire>())
				{
					colisor.transform.GetComponent<CampFire>().Fire();
				}
			}

		}
	}
	private IEnumerator Hit(int Id = -1, Transform Animal = null)
	{
        PodeAtacar = false;
        Anim.PlayAnimation(AnimationStates.HITTOOL);
        yield return new WaitForSeconds(0.5f);

        if (Id == 0)
        {
            SoundAxe.Play();

            if (PhotonNetwork.IsConnected)
                ScriptArvore.Damage(DanoCausadoAxe, true);

            else
                ScriptArvore.Damage(DanoCausadoAxe);
        }

        else if (Id == 1)
        {
            int index = Random.Range(0, 2);
            SoundsPickaxe[index].Play();

            if (PhotonNetwork.IsConnected)
                ScriptRock.Damage(DanoCausadoPickaxe, true);

            else
                ScriptRock.Damage(DanoCausadoPickaxe);
        }

		else if (Animal != null)
		{
            Animal.GetComponent<Ia_Animal>().Hit(10);
        }

		yield return new WaitForSeconds(TempoPorAtaque - 0.5f);
        PodeAtacar = true;
    }
	void OnGUI ()
	{
		GUI.DrawTexture(new Rect(Screen.width / 2 - mira.width / 6, Screen.height / 2 - mira.height / 6, mira.width / 3, mira.height / 3), mira);
	}

	public void CheckID(int itemID)
	{
		bool Achei = false;
		foreach (Tools Tl in Ferramentas)
		{
			if (itemID == Tl.id)
			{
				if (Tl.Name.Contains("Pickaxe"))
				{
					DanoCausadoPickaxe = Tl.Dano;
					TempoPorAtaque = Tl.TempoPorAtaque;
					DistanciaMinima = Tl.DistanciaMin;
					Achei = true;
				}
				else if (Tl.Name.Contains("Axe"))
				{
					DanoCausadoAxe = Tl.Dano;
					TempoPorAtaque = Tl.TempoPorAtaque;
                    DistanciaMinima = Tl.DistanciaMin;
                    Achei = true;
				}
			}
		}
		if (!Achei)
        {
			DanoCausadoAxe = 4;
			DanoCausadoPickaxe = 0;
			DistanciaMinima = 3;
			TempoPorAtaque = 1.5f;
		}
	}
}