using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class servidor : MonoBehaviourPunCallbacks
{
	public string SceneName = "Game",TextNameRoom;
	public GameObject Aviso;
	public ARVORE Arvore;
	public Rock Pedra;
	[HideInInspector]
	public bool Stance = true, Entrei, Spawnei, PlayerStatsBool;
	public static PhotonView Server;
	private Transform[] Spawns;
	private GameObject Player;
	[HideInInspector]public SelecionaSlot Sl;
	private void Awake()
	{
		
		if (!PlayerPrefs.HasKey("Exitnet"))
			PlayerPrefs.SetInt("Exitnet", 0);

		DontDestroyOnLoad(this.gameObject);
		Server = GetComponent<PhotonView>();
	}
	void Update()
	{
		/*if (SceneManager.GetActiveScene().name == "MENU" && PlayerPrefs.GetInt("Exitnet") == 1)
		{
			Instantiate(Aviso, FindObjectOfType<MenuPause>().transform);
			PlayerPrefs.SetInt("Exitnet", 0);
		}*/

		if (PlayerStatsBool)
        {
			Server.RPC("PlayerStats", RpcTarget.All, PhotonNetwork.GetPing(), Player.GetComponent<PhotonView>().ViewID, PhotonNetwork.NickName);
        }

		if (PhotonNetwork.IsConnected && SceneManager.GetActiveScene().name == SceneName && Stance)
		{
			Stance = false;
			if (Entrei)
			{
				PhotonNetwork.LeaveLobby();
			}
		}
		if (PhotonNetwork.InRoom && SceneManager.GetActiveScene().name == SceneName && !Spawnei)
		{
			if (PhotonNetwork.IsMasterClient)
				PhotonNetwork.CurrentRoom.IsVisible = true;

			if (Spawns == null)
				Spawns = FindObjectOfType<MenuPause>().PartJogo.Spawns;

			Player = PhotonNetwork.Instantiate("Materiais/Prefarbs/Players/HomemTeste", Spawns[0].transform.position, Quaternion.identity, 0);
			Player.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			Player.GetComponent<Agachar>().Name.gameObject.SetActive(false);
            Player.GetComponent<Rigidbody>().isKinematic = false;
			Player.GetComponent<RagDoll>().IgnoreRayCast();
            FindObjectOfType<GunsControl>().ArmLeft = Player.transform.GetChild(1);
            FindObjectOfType<GunsControl>().ArmRight = Player.transform.GetChild(2);
            Spawnei = true;

			// Mensagem do chat
			ChatMsm.Server.RPC("NewMensage", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " Entrou Na Sala", true, string.Empty);
			// Seta o nome
			Server.RPC("Nome", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, Player.GetComponent<PhotonView>().ViewID);

			//Player.transform.Find("Nome").GetComponent<TextMeshPro>().text = Nome;
			MouseLook.player = Player.transform;
			ChatMsm chat = FindObjectOfType<ChatMsm>();
            chat.controller = Player.GetComponent<Movimentacao>();
		}
	}

	public override void OnCreatedRoom()
	{
		SceneName = "Game";
		SceneManager.LoadScene("Carregamento");
	}

    public override void OnLeftLobby()
    {
		PhotonNetwork.JoinRoom(TextNameRoom);
	}

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
		print("Falha ao criar a sala" + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
		print("Falha ao entrar na sala  "+ message);
    }

    public override void OnLeftRoom()
    {
		PhotonNetwork.Disconnect();
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
        
		FindObjectOfType<ChatMsm>().NewMensage(otherPlayer.NickName + " Saiu Da Sala", true);
    }

	public override void OnDisconnected(DisconnectCause cause)
    {
		print("desconectado " + cause);
        
        if (cause != DisconnectCause.DisconnectByClientLogic)
        {
            if (SceneManager.GetActiveScene().name != "MENU")
            {
                SceneName = "MENU";
                SceneManager.LoadScene("Carregamento");
                PlayerPrefs.SetInt("Exitnet", 1);
            }

            Instantiate(Aviso, FindObjectOfType<MenuPause>().transform);
        }	
    }

	[PunRPC]
	public void Nome(string Nome, int ID)
	{
		PhotonView.Find(ID).GetComponent<Agachar>().Name.GetComponent<TextMeshPro>().text = Nome;
	}

	[PunRPC]
	public void PositionGun(int ViewID, bool State)
	{
		StartCoroutine(PhotonView.Find(ViewID).GetComponent<Agachar>().PositionGun(State));
	}

    [PunRPC]
	public void DerrubarArvore(int ID, int Dano)
    {
		/*if (Arvore == null || Arvore.id != ID)
		{
			ARVORE[] Arvores = FindObjectsOfType<ARVORE>();
			foreach (ARVORE arvore in Arvores)
			{
				if (arvore.id == ID)
				{
					Arvore = arvore;
					break;
				}
			}
        }*/
		ARVORE Arvore = PhotonView.Find(ID).GetComponent<ARVORE>();

		//Arvore.Net = true;
		Arvore.Damage(Dano);        
    }

	[PunRPC]
	public void QuebrarPedra(int ID, int Dano)
	{
		if (Pedra == null || Pedra.id != ID)
		{
			Rock[] Rocks = FindObjectsOfType<Rock>();
			foreach (Rock rock in Rocks)
			{
				if (rock.id == ID)
				{
					Pedra = rock;
					break;
				}
			}
		}
		if (Pedra.MaxVida == Dano)
			Pedra.destroy(true);

		else
			Pedra.Damage(Dano);
	}

	[PunRPC]
    public void DestroyObj(int ViewID, float Time)
    {
		Destroy(PhotonView.Find(ViewID).gameObject, Time);
    }

	private IEnumerator Timer(float timer, RPG_Gun GunUsing)
	{
		yield return new WaitForSeconds(timer);
		GunUsing.ShotSound.Stop();
	}

    [PunRPC]
	public void Sound(int ViewID, string NameChild, bool Active, Vector3 Position)
    {
		AudioSource Obj;
        Transform Gun = PhotonView.Find(ViewID).GetComponent<Movimentacao>().GunUsing.transform;

		if (Gun.Find(NameChild))
		{
			Obj = Gun.Find(NameChild).GetComponent<AudioSource>();
			if (Position != Vector3.zero)
				Obj.transform.position = Position;
		}
		else
		{
			if (Gun.GetComponent<Gun>())
				Obj = Gun.GetComponent<Gun>().ShotSound;
			else
				Obj = Gun.GetComponent<RPG_Gun>().ShotSound;
		}

        if (Active)
			Obj.Play();

		else
			Obj.Stop();
	}
	[PunRPC]
	public void StartParticle(int ViewID)
    {
		PhotonView.Find(ViewID).transform.GetComponent<Movimentacao>().GunUsing.GetComponent<Gun>().StartParticle();		
    }

	[PunRPC]
	public void AddGun(int ViewID, string NameChild, int Index)
    {
		GunsControl Gun = FindObjectOfType<GunsControl>();
		Transform Player = PhotonView.Find(ViewID).transform;
		Transform Local = Player.GetComponent<Movimentacao>().HolderGun.GetChild(0);
		GameObject GunPrefab = Instantiate(Gun.Guns[Index], Local);

		Player.GetComponent<Movimentacao>().GunUsing = GunPrefab;
		GunPrefab.layer = 0;

		if (GunPrefab.GetComponent<RPG_Gun>())
		{
			GunPrefab.transform.Find("RPG-7").gameObject.layer= 0;
		}

		IkMove IK = PhotonView.Find(ViewID).GetComponent<IkMove>();
		IK.RightHand = GunPrefab.transform.Find("Right Hand");
        IK.LeftHand = GunPrefab.transform.Find("Left Hand");
    }

	[PunRPC]
	public void RemGun(int ViewID)
    {
		Movimentacao Holder = PhotonView.Find(ViewID).GetComponent<Movimentacao>();
		if (Holder.LastGun)
			Destroy(Holder.LastGun);
		Holder.LastGun = Holder.GunUsing;
        Holder.GunUsing= null;
		Holder.LastGun.SetActive(false);
    }

    [PunRPC]
    public void GunDamage(int Damage, string Name, string LocationShot, int ViewID)
    {
        if (LocationShot == "mixamorig:Head")
            Damage *= 2;
		PhotonView View = PhotonView.Find(ViewID);

        
		if (!View.IsMine)
		{
            FindObjectOfType<Status>().Hit(Damage, Name, ViewID);
            FindObjectOfType<DetectShot>().Alvo = View.transform;
		}
		else
            FindObjectOfType<Status>().Hit(Damage, Name);
    }

    [PunRPC]
    public void DestroyRocket(int ViewID, int RocketViewID, Vector3 Point, bool ThisProjectile)
    {
		Movimentacao Holder = PhotonView.Find(ViewID).GetComponent<Movimentacao>();
		RPG_Gun GunUsing;
		Transform Rocket = PhotonView.Find(RocketViewID).transform;
		print(Rocket);

        if (Holder.GunUsing.GetComponent<RPG_Gun>())
			GunUsing = Holder.GunUsing.GetComponent<RPG_Gun>();
		else
		{
			GunUsing = Holder.LastGun.GetComponent<RPG_Gun>();
			GunUsing.BulletSound.transform.SetParent(GunUsing.transform.parent);
			Destroy(GunUsing.BulletSound.gameObject, 5);
		}
        
        //Rigidbody Rd = GunUsing.transform.GetChild(0).GetComponent<Rigidbody>();
        //Rocket Rocket = GunUsing.transform.GetChild(0).GetComponent<Rocket>();
        //Rd.AddExplosionForce(Rocket.ForceExplosion, transform.position, Rocket.Radius, 1, ForceMode.Impulse);
        GunUsing.BulletSound.transform.position = Point;
        //Destroy(GunUsing.transform.GetChild(0).gameObject, 0.4f);
		print("destroy");
        if (ThisProjectile)
        {
            GunUsing.ShotSound.volume = 0.3f;
            StartCoroutine(Timer(0.3f, GunUsing));
        }
        GunUsing.BulletSound.Play();
        //GunUsing.CurrentProjectile.transform.GetComponent<MeshRenderer>().enabled = false;
		Rocket.GetComponent<MeshRenderer>().enabled = false;

		print(Rocket.GetComponent<Rocket>().Prefab.name);
		ParticleSystem Particle = Rocket.GetComponent<Rocket>().Prefab.GetComponent<ParticleSystem>();
		//var Module = Particle.main;
		//Module.duration = Particle.time;
        Destroy(Particle.gameObject);
    }

    [PunRPC]
    public void ActiveRocket(int ViewID)
    {
        PhotonView.Find(ViewID).GetComponent<Rocket>().StartRocket(true);
    }

    [PunRPC]
    public void AddRemRocket(int ViewID, int RocketViewID)
    {
        //Gun G = PhotonView.Find(ViewID).GetComponent<Movimentacao>().GunUsing;
        PhotonView.Find(ViewID).GetComponent<Movimentacao>().GunUsing.GetComponent<RPG_Gun>().AddRocket(true, RocketViewID);
    }

    [PunRPC]
	public void ItemNetWork(int IdChest, int IdItem, int Count, int Numero, bool ADD,bool CountItem=false)
    {
		Chest Bau = PhotonView.Find(IdChest).GetComponent<Chest>();

        if (ADD)
        {
            if (Bau.Opened)
                FindObjectOfType<Inv>().addItem(IdItem, Count, false, true, Numero);
            else
            {
                if (CountItem)
                    Bau.ADDSave(IdItem, Count, Numero, true);
                else
                    Bau.ADDSave(IdItem, Count, Numero);
            }
        }
        else
        {
            if (Bau.Opened)
                FindObjectOfType<Inv>().remItem(IdItem, Count, false, true, Numero);
            else
                Bau.REMSave(Numero);
        }
    }

	[PunRPC]
	public void ChestNetwork(int id, bool Stats)
	{
		Chest Bau = PhotonView.Find(id).GetComponent<Chest>();
        if (Stats)
            Bau.Open(true);
        else
            Bau.Close(true);

    }
	[PunRPC]
	public void ItemMao(int ViewId, int id, Vector3 Position, Quaternion Rotation)
    {	print("Entrou");
		if (Sl == null)
		{
			Sl = FindObjectOfType<SelecionaSlot>();
			Sl.SelecionaView(ViewId, id, Position, Rotation);
			print("Foii");
		}
    }

	[PunRPC]
	public void GunsMode(bool State) 
	{
		FindObjectOfType<GunsControl>().GunsModeActive(State);
		print("ativar Gunsmode");
	}

    [PunRPC]
	public void ActivePlayerStats(bool Active)
    {
		PlayerStatsBool = Active;
		if (Active == false)
		{
			if (FindObjectOfType<ListPlayersConected>().ListPlayerStats.active)
				Server.RPC("ActivePlayerStats", RpcTarget.All, true);
		}
    }

	[PunRPC]
	public void PlayerStats(int Ping, int Id, string Name)
    {
		ListPlayersConected ListPL = FindObjectOfType<ListPlayersConected>();
		if (ListPL.ListPlayerStats.active)
		{
			ListPL.PlayersStatus(Ping, Id, Name);
		}
    }

	[PunRPC]
	public void Instantiate(string PrefabName, Vector3 Position, Quaternion Rotation)
	{
        PhotonNetwork.InstantiateRoomObject(PrefabName, Position, Rotation);
    }

	[PunRPC]
	public void Destroy(int ViewID)
	{
		PhotonNetwork.Destroy(PhotonView.Find(ViewID));
	}

	[PunRPC]
	public void IAMoving(Vector3 Destino, int ViewID, string Animation, bool SomenteAnimation)
    {
        /*foreach(Ia_Animal Ia in FindObjectsOfType<Ia_Animal>())
        {
			if (Ia.Id == Id)
            {
				if (!SomenteAnimation)
					Ia.Agent.SetDestination(Destino);
				Ia.SetAnimation(Animation);
            }
        }*/
        Ia_Animal Ia = PhotonView.Find(ViewID).GetComponent<Ia_Animal>();
        if (!SomenteAnimation)
            Ia.Agent.SetDestination(Destino);
    }

	[PunRPC]
	public void AnimaLook(int ViewID, int Speed, bool look)
    {
		/*foreach (Ia_Animal Ia in FindObjectsOfType<Ia_Animal>())
		{
			if (Ia.Id == Id)
			{
				Ia.Look = look;
				Ia.Agent.speed = Speed;
				if (!look)
				{
					Ia.StopCoroutine(Ia.Following());
					Ia.PlayerFollow = null;
				}
			}
		}*/
		Ia_Animal Ia = PhotonView.Find(ViewID).GetComponent<Ia_Animal>();

        Ia.Look = look;
        Ia.Agent.speed = Speed;
        if (!look)
        {
            Ia.StopCoroutine(Ia.Following());
            Ia.PlayerFollow = null;
        }
    }

	[PunRPC]
	public void FollowPlayer(int IdPlayer, int IdIA, string Animation)
    {
		/*foreach (Ia_Animal Ia in FindObjectsOfType<Ia_Animal>())
		{
			if (Ia.Id == IdIA)
			{
				Ia.PlayerFollow = PhotonView.Find(IdPlayer).transform;
				Ia.StartCoroutine(Ia.Following());
				Ia.SetAnimation(Animation);
			}
		}*/
		Ia_Animal Ia = PhotonView.Find(IdIA).GetComponent<Ia_Animal>();

        Ia.PlayerFollow = PhotonView.Find(IdPlayer).transform;
        Ia.StartCoroutine(Ia.Following());
        Ia.SetAnimation(Animation);
    }

	[PunRPC]
	public void DamageAnimal(int ViewID, int Damage)
	{
		PhotonView.Find(ViewID).GetComponent<Ia_Animal>().Hit(Damage);
	}

	[PunRPC]
	public void SetPositionIA(int ViewID, Vector3 PositionAtual, Vector3 Destino)
	{
		Transform IA = PhotonView.Find(ViewID).transform;

		IA.position = PositionAtual;
        IA.GetComponent<Ia_Animal>().Agent.SetDestination(Destino);
	}

	[PunRPC]
	public void DamagePart(int ViewID, string Name, int Dano)
	{
		print("Atualizar dano: " + Name);
        Helicopter_Controller Heli = PhotonView.Find(ViewID).GetComponent<Helicopter_Controller>();
		Heli.UpdateParts(Name, Dano);

	}

	[PunRPC]
	public void Morte(int ViewID, bool State)
	{
		PhotonView.Find(ViewID).GetComponent<RagDoll>().RagDollFunc(State);
	}

}