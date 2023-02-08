using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rock : MonoBehaviour
{
    public int Vida;
    public int MaxVida;
    public int id;
    public bool MatrixRock;
    public bool Net;
    public GameObject Pedra;
    public List<Transform> Position = new List<Transform>();

    void Start()
    {
        SlotScalerItem slt = FindObjectOfType<SlotScalerItem>();
        id = slt.IDTree + 1;
        slt.IDTree = id;
    }
    void Update()
    {
        //Dano enviado pela Internet
        /*if (Net && Vida > 0)
        {
            LastVida = Vida;
            Net = false;
        }

        // Voce Bateu
        if (!Net && LastVida != Vida && PhotonNetwork.IsConnected)
        {
            servidor.Server.RPC("QuebrarPedra", RpcTarget.Others, id, Vida);
        }
        LastVida = Vida;

        // Se foi voce que derrubou a Pedra
        if (Vida <= 0)
            destroy();*/
    }

    public void Damage(int Dano, bool IsMine=false)
    {
        bool Online = PhotonNetwork.IsConnected;
        Vida -= Dano;

        if (Online && IsMine)
            servidor.Server.RPC("QuebrarPedra", RpcTarget.Others, id, Dano);

        // Se foi voce que derrubou a Pedra
        if (Vida <= 0 && Online && IsMine)
            destroy();

        else if (Vida <= 0 && !Online)
            destroy();

    }

    public void destroy(bool destroi=false)
    {
        if (destroi)
            Destroy(this.gameObject);

        else
        {
            string Path;
            if (MatrixRock)
                Path = "Materiais/Prefarbs/Ambiente/";

            else
                Path = "Materiais/Prefarbs/Itens/";

            foreach (Transform item in Position)
            {
                if (PhotonNetwork.IsConnected)
                {
                    if (PhotonNetwork.IsMasterClient)
                        PhotonNetwork.InstantiateRoomObject(Path + Pedra.transform.name, item.transform.position, item.transform.rotation);

                    else
                        servidor.Server.RPC("Instantiate", RpcTarget.MasterClient, Path + Pedra.transform.name, item.transform.position, item.transform.rotation);
                }
                else
                    Instantiate(Pedra, item.transform.position, item.transform.rotation);
            }

            if (PhotonNetwork.IsConnected)
            {
                servidor.Server.RPC("QuebrarPedra", RpcTarget.AllBufferedViaServer, id, MaxVida);
            }
            else
                Destroy(this.gameObject);
        }
    }
}
