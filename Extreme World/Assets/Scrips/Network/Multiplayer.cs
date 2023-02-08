using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Multiplayer :  MonoBehaviourPunCallbacks 
{
	public GameObject Server;
	public TextMeshProUGUI MaxPlayer, RoomName, PlayerName;
	public GameObject ConteinerDeSala, CriadorDeSalas, PrefabDeSala, PainelMultiplayer, Canvas, MensagemDeProcurandoSalas, Conectando,
		NameError, SalaNameError, MaxPlayersError, SalaBackGround;
	public Button BotaoMultiplayer, DeCriarSala, Left, Right;
	public Scrollbar ScrollDasSalas;
	public TMP_InputField Name, NameRoom;
	[SerializeField]
	public List<RoomInfo> Salas = new List<RoomInfo>();
	private RectTransform Rt;
	private byte maxPlayer = 20;
	private bool ModNum, update, conected;
	private int myInt, TamConteiner;

	void Awake () 
	{
		Rt = ConteinerDeSala.GetComponent<RectTransform>();
	}
	void Start()
	{
		if (PlayerPrefs.HasKey("nome"))
		{
			Name.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("nome");
			PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("nome");
		}
	}

    void Update () 
	{
		TamConteiner = (int)Rt.parent.GetComponentInParent<RectTransform>().rect.height;
		
		if (maxPlayer > 2)
			Left.interactable = true;
		else
			Left.interactable = false;
		if (maxPlayer < 20)
			Right.interactable = true;
		else
			Right.interactable = false;

		if (PhotonNetwork.IsConnected) 
			ListarSalas ();
	}

	public void Nome()
	{
		print("("+Name.text.Trim()+")");
		if (Name.text.Trim() != string.Empty)
		{
			PhotonNetwork.LocalPlayer.NickName = Name.text.Trim();
			PlayerPrefs.SetString("nome", Name.text.Trim());
			print("gravei");
		}
		else
			Name.text = PhotonNetwork.LocalPlayer.NickName;

    }

    public override void OnConnected() 
	{
		ConteinerDeSala.SetActive (true);
		DeCriarSala.gameObject.SetActive (true);
		Conectando.SetActive(false);
		MensagemDeProcurandoSalas.SetActive(true);
	}

    public override void OnConnectedToMaster()
    {
		PhotonNetwork.JoinLobby();
		conected = true;
	}

	public void BotaoConectar ()
	{
		FindObjectOfType<MenuPause>().VisibleMenu(false);
		BotaoMultiplayer.gameObject.SetActive(false);

		PainelMultiplayer.SetActive(true);
		if (!PhotonNetwork.IsConnected) 
		{
			PhotonNetwork.ConnectUsingSettings();
			ListarSalas();
			update = true;
		}
		MensagemDeProcurandoSalas.SetActive(false);
	}
	int numAtualDeSalas = 0;
	int numAnteriorDeSalas = 0;
	
	void ListarSalas()
	{
		numAtualDeSalas = Salas.Count;
		if (numAtualDeSalas != numAnteriorDeSalas || update) 
		{
			for (int i = 0; i < Rt.childCount; i++)
			{
				Destroy(Rt.GetChild(i).gameObject);
			}
			if (numAtualDeSalas == 0 && conected) 
			{
				MensagemDeProcurandoSalas.SetActive(true);
			} 
			else 
			{
				insereSala();
			}
		}
		numAnteriorDeSalas = numAtualDeSalas;
	}

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
		Salas = roomList;
	}

    public void insereSala()
	{
		TextMeshProUGUI t;
		for (int i = 0; i < Salas.Count; i++)
		{
			RectTransform T = Instantiate(PrefabDeSala, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();
			T.SetParent(Rt);
			T.GetComponent<EntrarNaSala>().PlayerCount = Salas[i].PlayerCount;
			T.GetComponent<EntrarNaSala>().MaxPlayers = Salas[i].MaxPlayers;
			t = T.GetChild (0).GetComponentInChildren<TextMeshProUGUI> ();

			if (PlayerPrefs.GetInt("idioma") == 1)
				T.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Open";

			else
				T.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Entrar";

			t.text = Salas[i].Name;
			t = T.GetChild (1).GetComponentInChildren<TextMeshProUGUI> ();
			t.text = $"{Salas[i].PlayerCount} / {Salas[i].MaxPlayers}";
			
			SalaRect(T.gameObject);
            MensagemDeProcurandoSalas.SetActive(false);
        }
		AdapteConteiner(Salas.Count);
		ScrollDasSalas.value = 1;
	}

	public void AdapteConteiner(int numSalas)
	{
		int TotalSalas = numSalas;
		int SalaHeight = (int)SalaBackGround.GetComponent<RectTransform>().rect.height;
		int TamanhoDasSalas = TotalSalas * (SalaHeight + 10);
		int TamanhoDoPainel = (int)Rt.rect.height;
		if (update)
		{
			TamanhoDoPainel = TamConteiner;
			update = false;
		}
		if (TamanhoDoPainel <= TamanhoDasSalas)
		{
			Rt.sizeDelta = new Vector2(0, TamanhoDasSalas - TamConteiner);
		}
		else if (TamanhoDoPainel >= TamanhoDasSalas && TamanhoDoPainel >= TamConteiner)
		{
			if (TamanhoDasSalas <= TamConteiner)
			{
				Rt.sizeDelta = new Vector2(0, 0);
				ScrollDasSalas.size = 1;
			}
			else
				Rt.sizeDelta = new Vector2(0, TamanhoDasSalas - TamConteiner);
		}
	}

	public void Cancelar()
	{
		CriadorDeSalas.SetActive(false);
		DeCriarSala.gameObject.SetActive(true);
		SalaNameError.SetActive(false);
	}

	public void Voltar()
	{
		FindObjectOfType<MenuPause>().VisibleMenu(true);
		BotaoMultiplayer.gameObject.SetActive(true);

		if (PhotonNetwork.IsConnected)
			PhotonNetwork.Disconnect();

		PainelMultiplayer.SetActive(false);
		ConteinerDeSala.SetActive(false);
		DeCriarSala.gameObject.SetActive(false);
		NameError.SetActive(false);
		Conectando.SetActive(true);
		MensagemDeProcurandoSalas.SetActive(false);
		CriadorDeSalas.SetActive(false);
		conected = false;
	}

	public void left()
	{
		ModNum = true;
		int Int = int.Parse(MaxPlayer.text);
		myInt = Int;
		if (ModNum)
		{
			myInt -= 1;
			ModNum = false;
		}
		maxPlayer = (byte)myInt;
		MaxPlayer.text = maxPlayer.ToString();
	}

	public void right()
	{
		ModNum = true;
		int Int = int.Parse(MaxPlayer.text);
		myInt = Int;
		if (ModNum)
		{
			myInt += 1;
			ModNum = false;
		}
		maxPlayer = (byte)myInt;
		MaxPlayer.text = maxPlayer.ToString();
	}

	public void SalaRect(GameObject Sala)
	{
		int width = (int)SalaBackGround.GetComponent<RectTransform>().rect.width;
		int height = (int)SalaBackGround.GetComponent<RectTransform>().rect.height;
		Sala.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
	}

	public void criadorSala()
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = maxPlayer;
		roomOptions.IsVisible = false;
		if (RoomName.text == string.Empty)
		{
			SalaNameError.SetActive(true);
			SalaNameError.GetComponent<AnimateMessages>().reset = true;
			NameRoom.ActivateInputField();
		}
		else
		{
			string nomeSala = RoomName.text;
			if (SalaNameError.activeInHierarchy == true)
				SalaNameError.SetActive(false);
			PhotonNetwork.CreateRoom(nomeSala, roomOptions);
		}
	}
 
    public void Criar()
	{
		if (Name.text.Trim() != string.Empty)
		{
			if (PlayerPrefs.GetInt("idioma") == 1)
				NameRoom.GetComponent<TMP_InputField>().text = $"Room by {Name.GetComponent<TMP_InputField>().text}";
			else
				NameRoom.GetComponent<TMP_InputField>().text = $"Sala de {Name.GetComponent<TMP_InputField>().text}";
			CriadorDeSalas.SetActive(true);
			DeCriarSala.gameObject.SetActive(false);
			if (NameError.activeInHierarchy == true)
				NameError.SetActive(false);
		}
		else
		{
			NameError.SetActive(true);
			NameError.GetComponent<AnimateMessages>().reset = true;
			Name.ActivateInputField();
		}
	}
}
