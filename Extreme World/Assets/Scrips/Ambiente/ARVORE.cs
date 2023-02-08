using UnityEngine;
using Photon.Pun;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class ARVORE : MonoBehaviour 
{
	public int id;
	public int TempoDeQueda = 8;
	public int VIDA = 100;
	public int MaxVida;
	public int massa = 250;
	public int forca = 75;
	public bool Net;
	private Rigidbody corpoRigido;
	private float cronometro;
	private bool comecarContagem;
	private int LastVida;
	public GameObject Madeiras, Gravetos;
	public GameObject[] localMadeiras;
	public int maxMadeiras = 5;
	public int maxGraveto = 10;

	void Start () 
	{
		SlotScalerItem slt = FindObjectOfType<SlotScalerItem>();
		id = slt.IDTree + 1;
		slt.IDTree = id;
		corpoRigido = GetComponent <Rigidbody> ();
		corpoRigido.useGravity = true;
		corpoRigido.isKinematic = true;
		cronometro = 0;
		comecarContagem = false;
		corpoRigido.mass = massa;
		LastVida = VIDA;
	}

	void Update () 
	{
		//Dano enviado pela Internet
		/*if (Net && VIDA > 0)
        {
			LastVida = VIDA;
			Net = false;
        }
		// Voce Bateu
		if (!Net && LastVida != VIDA && PhotonNetwork.IsConnected)
		{
			servidor.Server.RPC("DerrubarArvore", RpcTarget.Others, id, VIDA);
		}
		LastVida = VIDA;

		// Se foi voce que derrubou a Arvore
		if (VIDA <= 0) 
		{
			if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
			{
				int ForceX = Random.Range(-20, 40);
				int ForceY = Random.Range(-20, 40);
				corpoRigido.AddForce(ForceX * forca, 0, ForceY * forca);
            }
			corpoRigido.isKinematic = false;
			comecarContagem = true;
		}
		if (comecarContagem) 
		{
			cronometro += Time.deltaTime;
		}
		if (cronometro >= TempoDeQueda) 
		{
			cronometro = 0;
			GameObject Obj;
			if (maxMadeiras > localMadeiras.Length)
			{
				maxMadeiras = localMadeiras.Length;
			}
			for (int x = 0; x < maxMadeiras; x++)
			{
				if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
				{
					Obj = PhotonNetwork.InstantiateRoomObject("Materiais/Prefarbs/Itens/Wood", localMadeiras[x].transform.position, transform.rotation);
					//Obj.GetComponent<Rigidbody>().isKinematic = false;
				}
				else if (!PhotonNetwork.IsConnected)
				{
					Obj = Instantiate(Madeiras, localMadeiras[x].transform.position, transform.rotation);
					//Obj.GetComponent<Rigidbody>().isKinematic = false;
				}
			}
			for (int y = 0; y < maxGraveto; y++)
			{
					
				if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
				{
					Obj = PhotonNetwork.InstantiateRoomObject("Materiais/Prefarbs/Itens/Stick", localMadeiras[localMadeiras.Length - 1].transform.position, Quaternion.identity);
					//Obj.GetComponent<Rigidbody>().isKinematic = false;
				}
				else if (!PhotonNetwork.IsConnected)
				{
					Obj = Instantiate(Gravetos, localMadeiras[localMadeiras.Length - 1].transform.position, Quaternion.identity);
					//Obj.GetComponent<Rigidbody>().isKinematic = false;
				}
			}
			if (!GetComponent<GrowthTree>())
			{
				if (Random.Range(0, 2) == 2)
				{
					Obj = Instantiate(this.gameObject, localMadeiras[localMadeiras.Length - 1].transform.position, Quaternion.identity);
					Obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
					Obj.GetComponent<ARVORE>().VIDA = MaxVida;
					Obj.GetComponent<ARVORE>().enabled = false;
					Obj.GetComponent<Rigidbody>().isKinematic = false;
					Obj.GetComponent<Rigidbody>().mass = 10;
					Destroy(Obj.GetComponent<GrowthTree>());
				}
			}
			if (PhotonNetwork.IsConnected)
			{
				if (PhotonNetwork.IsMasterClient)
					PhotonNetwork.Destroy(GetComponent<PhotonView>());
				else
					servidor.Server.RPC("Destroy", RpcTarget.MasterClient, GetComponent<PhotonView>().ViewID);
			}
			else
				Destroy(gameObject);
		}*/
	}

	public void Damage(int Dano, bool IsMine=false)
	{
		bool Online = PhotonNetwork.IsConnected;
		VIDA -= Dano;

        if (Online && IsMine)
        {
            servidor.Server.RPC("DerrubarArvore", RpcTarget.Others, GetComponent<PhotonView>().ViewID, Dano);
			print("mandei");
        }

		if (VIDA <= 0 && Online && IsMine)
		{
			GetComponent<OwnerTransfer>().TrocarDono();
			StartCoroutine(CairArvore());
		}

		else if (VIDA <= 0 && Online && !IsMine)
		{
			corpoRigido.isKinematic = false;
		}

		else if (VIDA <= 0 && !Online)
			StartCoroutine(CairArvore());
    }

	public IEnumerator CairArvore(float cronometro=0.0f)
	{
        int ForceX = Random.Range(-20, 40);
        int ForceY = Random.Range(-20, 40);
        corpoRigido.AddForce(ForceX * forca, 0, ForceY * forca);

        corpoRigido.isKinematic = false;
		cronometro += Time.fixedDeltaTime;

		if (cronometro >= TempoDeQueda)
		{
			if (maxMadeiras > localMadeiras.Length)
				maxMadeiras = localMadeiras.Length;

			// Instancia Madeiras
			for (int x = 0; x < maxMadeiras; x++)
			{
				if (PhotonNetwork.IsConnected)
				{
					if (PhotonNetwork.IsMasterClient)
						PhotonNetwork.InstantiateRoomObject("Materiais/Prefarbs/Itens/Wood", localMadeiras[x].transform.position, transform.rotation);
					else
						servidor.Server.RPC("Instantiate", RpcTarget.MasterClient, "Materiais/Prefarbs/Itens/Wood", localMadeiras[x].transform.position, transform.rotation);

					//Obj.GetComponent<Rigidbody>().isKinematic = false;
				}
				else
				{
					GameObject g = Instantiate(Madeiras, localMadeiras[x].transform.position, transform.rotation);
					//Obj.GetComponent<Rigidbody>().isKinematic = false;
				}
			}

			// Instancia Gravetos
			for (int y = 0; y < maxGraveto; y++)
			{

				if (PhotonNetwork.IsConnected)
				{
					if (PhotonNetwork.IsMasterClient)
						PhotonNetwork.InstantiateRoomObject("Materiais/Prefarbs/Itens/Stick", localMadeiras[localMadeiras.Length - 1].transform.position, Quaternion.identity);
					else
						servidor.Server.RPC("Instantiate", RpcTarget.MasterClient, "Materiais/Prefarbs/Itens/Stick", localMadeiras[localMadeiras.Length - 1].transform.position, Quaternion.identity);
					//Obj.GetComponent<Rigidbody>().isKinematic = false;
				}
				else
				{
					Instantiate(Gravetos, localMadeiras[localMadeiras.Length - 1].transform.position, Quaternion.identity);
					//Obj.GetComponent<Rigidbody>().isKinematic = false;
				}
			}
			print("Instanciei tudo");
			// Se a Arvore crece
			if (!GetComponent<GrowthTree>())
			{
				GameObject Obj;
				if (Random.Range(0, 2) == 2)
				{
					Obj = Instantiate(this.gameObject, localMadeiras[localMadeiras.Length - 1].transform.position, Quaternion.identity);
					Obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
					Obj.GetComponent<ARVORE>().VIDA = MaxVida;
					Obj.GetComponent<ARVORE>().enabled = false;
					Obj.GetComponent<Rigidbody>().isKinematic = false;
					Obj.GetComponent<Rigidbody>().mass = 10;
					Destroy(Obj.GetComponent<GrowthTree>());
				}
			}

			// Destroi a arvore
			if (PhotonNetwork.IsConnected)
			{
				//if (PhotonNetwork.IsMasterClient)
					PhotonNetwork.Destroy(GetComponent<PhotonView>());
				//else
					//servidor.Server.RPC("Destroy", RpcTarget.MasterClient, GetComponent<PhotonView>().ViewID);
			}
			else
				Destroy(gameObject);
		}

		else
		{
			yield return new WaitForFixedUpdate();
			StartCoroutine(CairArvore(cronometro));
		}
    }
}