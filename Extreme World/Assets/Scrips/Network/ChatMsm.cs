using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatMsm : MonoBehaviour
{
    public InputField Chat_InputField;
    public GameObject Mensagem;
    public bool Online;
    public static bool Opened;
    public static PhotonView Server;
    [Header("Key")]
    public KeyCode AbrirChat = KeyCode.T;
    private MenuPause menu;
    public Movimentacao controller;
    private DerrubarArvores mira;
    private SlotScalerItem inventory;
    private SelecionaSlot Select;

    void Start()
    {
        Opened = false;
        Chat_InputField.gameObject.SetActive(false);
        Server = GetComponent<PhotonView>();
        menu = FindObjectOfType<MenuPause>();
        mira = FindObjectOfType<DerrubarArvores>();
        inventory = FindObjectOfType<SlotScalerItem>();
        Select = FindObjectOfType<SelecionaSlot>();

        if (PhotonNetwork.IsConnected)
            Online = true;
        else
            gameObject.SetActive(false);
    }
    void Update()
    {
        if (Online)
        {
            if (transform.GetChild(0).transform.childCount > 15)
                Destroy(transform.GetChild(0).transform.GetChild(0).gameObject);

            if (Input.GetKeyDown(AbrirChat) && !MenuPause.MenuOpen)
            {
                if (!MouseLook.Veiculo)
                    controller.enabled = false;
                else
                    controller.VehicleUsing.GetComponent<Helicopter_Controller>().enabled = false;

                Chat_InputField.gameObject.SetActive(true);
                //menu.PartJogo.Actived = false;
                MouseLook.MouseEnable = false;
                mira.enabled = false;
                inventory.enabled = false;
                Select.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Opened = true;
                Chat_InputField.ActivateInputField();

                if (transform.GetChild(0).gameObject.active == false)
                    transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
    [PunRPC]
    public void NewMensage(string msm, bool color, string Nome = "Indefinido")
    {
        if (color)
        {
            Mensagem.GetComponent<Text>().color = Color.yellow;
            Mensagem.GetComponent<Text>().text = msm;
        }
        else
        {
            Mensagem.GetComponent<Text>().color = Color.white;
            Mensagem.GetComponent<Text>().text = $"{Nome}: {msm}";
        }

        GameObject MsM = Instantiate(Mensagem);
        MsM.transform.SetParent(transform.GetChild(0).transform);

        if (transform.GetChild(0).gameObject.active == false)
            transform.GetChild(0).gameObject.SetActive(true);
    }
    public void EnviarMsM (Text msm)
    {
        GameObject Obj;
        string mensagem = msm.text.Trim();

        if (mensagem.Trim().Contains("/Helicopter"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Obj = PhotonNetwork.InstantiateSceneObject("Materiais/Prefarbs/Veiculos/HelicopterEW", menu.PartJogo.Spawns[0].position, Quaternion.identity, 0);
                Obj.GetComponent<Rigidbody>().useGravity = true;

                Server.RPC("NewMensage", RpcTarget.All, $"Helicoptero instanciado por {PlayerPrefs.GetString("nome")}", true, string.Empty);
            }
            else
                servidor.Server.RPC("Instantiate", RpcTarget.MasterClient, "Materiais/Prefarbs/Veiculos/HelicopterEW", menu.PartJogo.Spawns[0].position, Quaternion.identity);
        }

        else if (mensagem.Contains("/Gunsmode true"))
        {
            servidor.Server.RPC("GunsMode", RpcTarget.AllBufferedViaServer, true);
            Server.RPC("NewMensage", RpcTarget.All, "Modo de jogo alterado para GunsMode", true, string.Empty);
        }

        else if (mensagem.Contains("/Gunsmode false"))
        {
            PhotonNetwork.RemoveBufferedRPCs(servidor.Server.ViewID, "GunsMode");
            servidor.Server.RPC("GunsMode", RpcTarget.All, false);
            Server.RPC("NewMensage", RpcTarget.All, "Modo de jogo alterado para SurvivalMode", true, string.Empty);
        }

        else if (mensagem != string.Empty)
            Server.RPC("NewMensage", RpcTarget.All, mensagem, false, PlayerPrefs.GetString("nome"));

        if (!MouseLook.Veiculo)
            controller.enabled = true;
        else
            controller.VehicleUsing.GetComponent<Helicopter_Controller>().enabled = true;

        Chat_InputField.text = string.Empty;
        Chat_InputField.gameObject.SetActive(false);
        menu.PartJogo.Actived = true;
        MouseLook.MouseEnable = true;
        mira.enabled = true;
        inventory.enabled = true;
        Select.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Opened = false;

        StartCoroutine(Timer(15));
    }
    private IEnumerator Timer(float Time)
    {
        yield return new WaitForSeconds(Time);
        transform.GetChild(0).gameObject.SetActive(false);
    }
    
    public void ClearChat()
    {
        for(int x = 0; x < transform.GetChild(0).childCount; x++)
        {
            Destroy(transform.GetChild(0).GetChild(x).gameObject);
        }
    }
}
