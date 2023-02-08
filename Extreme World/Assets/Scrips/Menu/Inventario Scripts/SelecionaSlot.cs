using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SelecionaSlot : MonoBehaviour
{
    public KeyCode Drop = KeyCode.Q;
    public float PosX;
    public int Numero;
    public int itemID = -1;
    public Slot slotSelected;
    public GameObject itemMao;
    public List<GameObject> ItensPrefabs;
    private Slot[] slots;
    private List<Slot> HotBarSlot = new List<Slot>();
    private int LastCont;
    private Transform Mao;
    private DerrubarArvores Check;
    private List<string> Keys = new List<string>() {"1", "2", "3", "4", "5", "6", "7", "8", "9"};
    void Start()
    {
        Check = FindObjectOfType<DerrubarArvores>();
        slots = FindObjectsOfType<Slot>();
        foreach (Slot slot in slots)
        {
            if (slot.Numero < 10 && slot.Numero > 0)
                HotBarSlot.Add(slot);
        }
        Seleciona(1);
    }
    void Update()
    {
        string x = Input.inputString;
        if (Mao == null)
        {
            if (MouseLook.player != null)
                Mao = MouseLook.player.gameObject.GetComponent<Movimentacao>().Mao;
            return;
        }

        // Dropa Itens
        if (Input.GetKeyDown(Drop))
        {
            DropItens();
        }

        // Troca o Slot com o scrol do mouse
        if (Input.mouseScrollDelta.y != 0)
            Seleciona(Numero - (int)Input.mouseScrollDelta.y);
        
        // Troca o Slot com o teclado numerio (1,9)
        else if (x != "" && Keys.Contains(x))
            Seleciona(int.Parse(x));

        // Atualiza o Slot se o Cont mudar
        else
        {
            if (slotSelected.count >= 1 && LastCont == 0 && slotSelected.count != LastCont || slotSelected.count == 0 && LastCont == 1 && slotSelected.count != LastCont)
                Seleciona(Numero);
        }
    }
    public void Seleciona(int x)
    {
        foreach (Slot slot in HotBarSlot)
        {
            if (slot.Numero == x)
            {
                slotSelected = slot;
                Numero = x;
                PosX = slot.transform.localPosition.x;
                transform.GetComponent<RectTransform>().localPosition = new Vector3(PosX, 0, 0);
                LastCont = slotSelected.count;
                if (slot.count >= 1)
                {
                    bool Completed = false;
                    foreach (GameObject item in ItensPrefabs)
                    {
                        int id = item.GetComponent<itemObject>().id;
                        // slot.transform.GetChild(0).GetComponent<item>().Id

                        if (id == slot.Id)
                        {
                            if (itemMao == null || id != itemMao.GetComponent<itemObject>().id)
                            {
                                itemID = item.GetComponent<itemObject>().id;
                                
                                    if (PhotonNetwork.IsConnected)
                                    {
                                        if (itemMao != null)
                                            PhotonNetwork.Destroy(itemMao.GetComponent<PhotonView>());
                                        itemMao = PhotonNetwork.Instantiate($"Materiais/Prefarbs/Itens/{item.name}", item.transform.position, item.transform.rotation);
                                    }
                                    else
                                    {
                                        if (itemMao != null)
                                            Destroy(itemMao);
                                        itemMao = Instantiate(item);
                                    }
                                
                                itemMao.transform.SetParent(Mao.transform);
                                Destroy(itemMao.GetComponent<BoxCollider>());
                                Destroy(itemMao.GetComponent<PhotonRigidbodyView>());
                                Destroy(itemMao.GetComponent<Rigidbody>());
                                Destroy(itemMao.GetComponent<Outline>());

                                itemMao.transform.localPosition = item.transform.localPosition;
                                itemMao.transform.localRotation = item.transform.localRotation;
                                if (PhotonNetwork.IsConnected)
                                {
                                    DestroyItemView();
                                    int ViewId = MouseLook.player.GetComponent<PhotonView>().ViewID;
                                    int itemViewID = itemMao.GetComponent<PhotonView>().ViewID;
                                    servidor.Server.RPC("ItemMao", RpcTarget.OthersBuffered, ViewId, itemViewID, itemMao.transform.localPosition, itemMao.transform.localRotation);
                                }
                                Completed = true;
                                break;
                            }
                        }
                    }
                    if (!Completed)
                    {
                        if (itemMao != null)
                        {
                            itemID = -1;
                            if (PhotonNetwork.IsConnected)
                            {
                                PhotonNetwork.Destroy(itemMao.GetComponent<PhotonView>());
                                DestroyItemView();
                            }
                            else
                                Destroy(itemMao);
                        }
                    }
                }
                else
                {
                    if (itemMao != null)
                    {
                        itemID = -1;
                        if (PhotonNetwork.IsConnected)
                        {
                            PhotonNetwork.Destroy(itemMao.GetComponent<PhotonView>());
                            DestroyItemView();
                        }
                        else
                            Destroy(itemMao);
                    }
                }
                break;
            }
        }
        Check.CheckID(itemID);
        
    }
    public void SelecionaView (int viewId, int id, Vector3 Position, Quaternion Rotation)
    {
        Transform mao = PhotonView.Find(viewId).GetComponent<Movimentacao>().Mao;
        /*if (id == -1)
        {
            if (mao.childCount >= 1)
                Destroy(mao.GetChild(0).gameObject);
        }*/
        //else
        
        {
            GameObject itemMao = PhotonView.Find(id).gameObject;
            itemMao.transform.SetParent(mao);

            Destroy(itemMao.GetComponent<BoxCollider>());
            if (itemMao.GetComponent<PhotonRigidbodyView>())
                Destroy(itemMao.GetComponent<PhotonRigidbodyView>());
            Destroy(itemMao.GetComponent<Rigidbody>());
            Destroy(itemMao.GetComponent<Outline>());
            itemMao.transform.localPosition = Position;
            itemMao.transform.localRotation = Rotation;

            /*foreach (GameObject item in ItensPrefabs)
            {
                if (item.GetComponent<itemObject>().id == id)
                {
                    if (mao.childCount >= 1)
                        Destroy(mao.GetChild(0).gameObject);

                    itemMao = Instantiate(item);
                    itemMao.transform.SetParent(mao);
                    Destroy(itemMao.GetComponent<BoxCollider>());
                    if (itemMao.GetComponent<PhotonRigidbodyView>())
                        Destroy(itemMao.GetComponent<PhotonRigidbodyView>());
                    Destroy(itemMao.GetComponent<Rigidbody>());
                    Destroy(itemMao.GetComponent<Outline>());

                    itemMao.transform.localPosition = item.transform.localPosition;
                    itemMao.transform.localRotation = item.transform.localRotation;
                    break;
                }
            }*/
        }
    }
    public void DestroyItemView()
    {
        int[] ActorList = { PhotonNetwork.LocalPlayer.ActorNumber };
        PhotonNetwork.RemoveBufferedRPCs(servidor.Server.ViewID, "ItemMao", ActorList);
    }

    public void DropItens()
    {
        if (slotSelected.count > 0)
        {
            slotSelected.count--;

            // Procura o item e dropa ele
                GameObject item = slotSelected.transform.GetChild(0).GetComponent<item>().ItemObject;
                if (item.GetComponent<itemObject>().id == slotSelected.Id)
                {
                    if (PhotonNetwork.IsConnected)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            GameObject G = PhotonNetwork.InstantiateRoomObject($"Materiais/Prefarbs/Itens/{item.name}", Mao.position, Quaternion.identity);
                            G.transform.SetParent(MouseLook.player.parent);
                        }
                        else
                            servidor.Server.RPC("Instantiate", RpcTarget.MasterClient, $"Materiais/Prefarbs/Itens/{item.name}", Mao.position, Quaternion.identity);
                    }
                    else
                    {
                        GameObject I = Instantiate(item);
                        I.transform.position = Mao.position;
                    }
                    slotSelected.transform.GetChild(0).GetComponent<item>().update = true;
                }

            // Destroi o item caso ele acabe
            if (slotSelected.count == 0)
            {
                slotSelected.Id = -1;
                Destroy(slotSelected.transform.GetChild(0).gameObject);
            }
        }
    }
}
