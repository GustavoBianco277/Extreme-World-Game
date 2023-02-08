using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class ChestSave
{
    public int ID;
    public int Cont;
    public int NumeroSlot;
}
public class Chest : MonoBehaviourPunCallbacks
{
    private Inv inv;
    private SlotScalerItem SLT;
    private Vector3 Vec;
    [HideInInspector] public PhotonView View;
    public AudioSource OpenAudio;
    public AudioSource CloseAudio;
    public KeyCode OpenClose = KeyCode.E;
    public bool Opened, OpenedNet, Selected;
    public List<ChestSave> Saves = new List<ChestSave>();
    [HideInInspector]
    public List<Slot> Slots = new List<Slot>();

    public void Start()
    {
        SLT = FindObjectOfType<SlotScalerItem>();
        inv = FindObjectOfType<Inv>();
        View = GetComponent<PhotonView>();

        //yield return new WaitForSeconds(0.5f);

        foreach (Slot slot in FindObjectsOfType<Slot>())
        {
            if (slot.Numero >= 55)
                Slots.Add(slot);
        }
        
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            View.RPC("SolicitarItens", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    void Update()
    {
        Vec = transform.GetChild(0).localEulerAngles;
        // Abre
        if (Opened || OpenedNet)
            transform.GetChild(0).localEulerAngles = Vector3.Lerp(Vec, Vector3.zero, Time.deltaTime * 3);

        // Fecha
        else if (!Opened || !OpenedNet)
            transform.GetChild(0).localEulerAngles = Vector3.Lerp(Vec, new Vector3(20, 0, 0), Time.deltaTime * 5);

        if (Input.GetKeyDown(OpenClose) && !Opened && Selected)
        {
            Open();
        }

        else if (Input.GetKeyDown(OpenClose) && Opened)
        {
            Close();
            print("foi aqui");
        }
    }
    public void Open(bool Net = false)
    {
        print("abri");
        if (PhotonNetwork.IsConnected && !Net)
            servidor.Server.RPC("ChestNetwork", RpcTarget.Others, View.ViewID, true);

        if (Net)
            OpenedNet = true;

        else
        {
            inv.Clear(55, 99);

            // add todos os itens
            foreach (ChestSave item in Saves)
            {
                inv.addItem(item.ID, item.Cont, false, true, item.NumeroSlot);
            }

            SLT.OpenInventario(true);
            print("aki");
            Opened = true;
        }
        OpenAudio.Play();
    }

    public void Close(bool Net = false)
    {
        print("fechei");
        if (PhotonNetwork.IsConnected && !Net)
            servidor.Server.RPC("ChestNetwork", RpcTarget.Others, View.ViewID, false);

        if (Net)
            OpenedNet = false;
        else
        {
            SLT.ChestAberto = false;
            SLT.CloseInventario();
            Saves.Clear();

            foreach (Slot slot in Slots)
            {
                if (slot.Id != -1)
                    ADDSave(slot.Id, slot.count, slot.Numero);
            }

            Opened = false;
            print("ali");
        }
        CloseAudio.Play();
    }
    public void ADDSave(int Id, int Count, int Numero, bool CountItem = false)
    {
        if (CountItem)
        {
            foreach (ChestSave save in Saves)
            {
                if (save.NumeroSlot == Numero)
                    save.Cont = Count;
            }
        }
        else
        {
            ChestSave item = new ChestSave();
            item.ID = Id;
            item.Cont = Count;
            item.NumeroSlot = Numero;
            Saves.Add(item);
        }
    }
    public void REMSave(int Numero)
    {
        for (int i = 0; i < Saves.Count; i++)
        {
            if (Saves[i].NumeroSlot == Numero)
                Saves.RemoveAt(i);
        }
    }

    [PunRPC]
    public void AddItens(int ActorNum, int ItemID, int ItemCount, int ItemNumSlot)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == ActorNum)
        {
            //inv.addItem(ItemID, ItemCount, false, true, ItemNumSlot);
            ChestSave item = new ChestSave();
            item.ID = ItemID;
            item.Cont = ItemCount;
            item.NumeroSlot = ItemNumSlot;
            Saves.Add(item);
        }
    }
    [PunRPC]
    public void SolicitarItens(int ActorNum)
    {
        foreach (ChestSave item in Saves)
        {
            View.RPC("AddItens", RpcTarget.Others, ActorNum, item.ID, item.Cont, item.NumeroSlot);
        }
    }
}
