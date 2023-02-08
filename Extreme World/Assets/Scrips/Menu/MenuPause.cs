using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;

[System.Serializable]
public class PartMenu
{
	public bool Using;
	//public GameObject Servidor;
	public Button BotaoUmJogador, BotaoOpcoes, BotaoSair;
	[HideInInspector] public bool MouseBool;
}
[System.Serializable]
public class PartJogo
{
	public bool Using, Actived = true;
	public GameObject Player;
	public Transform[] Spawns;
	public Terrain Terreno;
	public Button BotaoRetornarAoJogo, BotaoOpcoes, BotaoVoltarAoMenu;
	public Status Morto;
	public MouseLook mouse;
	public DerrubarArvores Mira2;
	public Movimentacao controlador;
	[HideInInspector] public SelecionaSlot SelectSlot;
    [HideInInspector] public GunsControl GC;
    [HideInInspector] public bool menuParte1Ativo, menuParte2Ativo;
}
public class MenuPause : MonoBehaviour 
{
	public PartMenu PartMenu = new PartMenu();
	public PartJogo PartJogo = new PartJogo();
	//
	public string NomeCena = "Carregamento";
	public Slider BarraVolume, Sensibilidade, DistanciaDetalhe, DistanciaArvores;
	public GameObject PainelMenu, PainelTela,PainelGraficos,PainelTecladoMouse,PainelAudio;
	public Toggle CaixaModoJanela, AntiAliasing, InverterMouse, DesfoqueMovimento, AdaptacaoOcular, Nevoa, OclusaoAmbiental, ProfundidadeCampo;
	public TMP_Dropdown Resolucoes, Qualidades, Sombras;
	public Button BotaoVoltar, BotaoSalvar, BotaoTela, BotaoAudio, BotaoGrafico, BotaoTecladoMouse;
	private Button BotaoLeftQ, BotaoRightQ, BotaoLeftR, BotaoRightR, BotaoRightS, BotaoLeftS;
	public TextMeshProUGUI PorcentagemVolume, PorcentagemSensitivy, TxtNome, TxtDetelheDistancia, TxtDistanciaArvores;
	private float VOLUME, DetalheDistancia, DistanciaArvoresF;
	private int qualidadeGrafica, modoJanelaAtivo, AntiAliasingInt, InverterMouseInt, resolucaoSalveIndex, DesfoqueMovimentoInt, SombraInt,
		AdaptacaoOcularInt, NevoaInt, OclusaoAmbientalInt, ProfundidadeCampoInt;
	private bool telaCheiaAtivada;
	public List<Resolution> resolucoesSuportadas = new List<Resolution>();
	public Linguagem lingua;
	[HideInInspector]
	public bool AntiAliasingBool, DesfoqueMovimentoBool, AdaptacaoOcularBool, NevoaBool, OclusaoAmbientalBool, ProfundidadeCampoBool;
	public static bool MenuOpen;

    public List<string> NomesQualyIngles, NomesSombraIngles, NomesSombraPortugues;//, NomesIdiomaIngles, NomesIdiomaPortugues;

	/*[RuntimeInitializeOnLoadMethod]
	static void OnRuntimeMethodLoad()
	{
		print("oi");
        //Instantiate(PartMenu.Servidor);
    }*/
	void Awake()
	{
		MenuOpen = false;
        if (!PlayerPrefs.HasKey("Init"))
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetInt("Init", 1);
		}
		//resolucoesSuportadas = Screen.resolutions;
        
		foreach (Resolution resolutionin in Screen.resolutions)
		{
			if (resolutionin.refreshRate == Screen.currentResolution.refreshRate)
				resolucoesSuportadas.Add(resolutionin);
			
		}
        //
        BotaoLeftQ = AddButtons(Qualidades, "Left");
		BotaoRightQ = AddButtons(Qualidades, "Right");
		BotaoLeftR = AddButtons(Resolucoes, "Left");
		BotaoRightR = AddButtons(Resolucoes, "Right");
		BotaoLeftS = AddButtons(Sombras, "Left");
		BotaoRightS = AddButtons(Sombras, "Right");

		if (PartJogo.Using)
		{
			PartJogo.SelectSlot = FindObjectOfType<SelecionaSlot>();
			PartJogo.GC = FindObjectOfType<GunsControl>();
		}
		else if (PartMenu.Using)
			Cursor.visible = true;
	}

	void Start () 
	{
		if (PartJogo.Using)
		{
			if (!PhotonNetwork.IsConnected)
			{
				PartJogo.Player.transform.position = PartJogo.Spawns[0].transform.position;
                PartJogo.Player.transform.rotation = PartJogo.Spawns[0].transform.rotation;

				//Spwan Player
                GameObject Pl = Instantiate(PartJogo.Player);
				Pl.GetComponent<Movimentacao>().OffLine = true;
				Pl.GetComponent<Rigidbody>().isKinematic = false;
				Pl.GetComponent<Agachar>().Name.gameObject.SetActive(false);
				Pl.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
				Pl.GetComponent<RagDoll>().IgnoreRayCast();
				FindObjectOfType<GunsControl>().ArmLeft = Pl.transform.GetChild(1);
                FindObjectOfType<GunsControl>().ArmRight = Pl.transform.GetChild(2);

                MouseLook.player = Pl.transform;
				PartJogo.controlador = Pl.GetComponent<Movimentacao>();
			}

			DesabilitarPaineis();
			PainelAudio.SetActive(true);
			Opcoes(false, false);
		}
		ChecarResolucoes ();
		int Lingua = lingua.Lingua;

		if (PlayerPrefs.HasKey("idioma"))
			Lingua = PlayerPrefs.GetInt("idioma");

		if (Lingua == 0)
		{
			AjustarQualidades("portugues");
			Qualidades.captionText.text = "Baixo";
			ChecarSombras("portugues");
			Sombras.captionText.text = "Normal";
		}

		else
		{
			AjustarQualidades("ingles");
			Qualidades.captionText.text = "Low";
			ChecarSombras("ingles");
			Sombras.captionText.text = "Normal";
		}

		if (PlayerPrefs.HasKey ("RESOLUCAO")) 
		{
			int numResoluc = PlayerPrefs.GetInt ("RESOLUCAO");
			if (resolucoesSuportadas.Count <= numResoluc)
				PlayerPrefs.DeleteKey ("RESOLUCAO");
		}

		//=============== SAVES===========//
		if (PlayerPrefs.HasKey ("VOLUME")) 
		{
			VOLUME = PlayerPrefs.GetFloat ("VOLUME");
			BarraVolume.value = VOLUME;
		}

		else 
		{
			PlayerPrefs.SetFloat ("VOLUME", 100);
			BarraVolume.value = 100;
		}

		//=========Sensibilidade========//
		if (PlayerPrefs.HasKey("sensibilidade"))
		{
			Sensibilidade.value = PlayerPrefs.GetFloat("sensibilidade") *20;
			if (PartJogo.Using)
			{
				PartJogo.mouse.sensitivityX = PlayerPrefs.GetFloat("sensibilidade");
				PartJogo.mouse.sensitivityY = PlayerPrefs.GetFloat("sensibilidade");
			}
		}

		else
		{
			Sensibilidade.value = 50f;
			PlayerPrefs.SetFloat("sensibilidade", Sensibilidade.value /20);
		}

		if (PlayerPrefs.HasKey("distanciadetalhe"))
		{
			DetalheDistancia = PlayerPrefs.GetFloat("distanciadetalhe");
			DistanciaDetalhe.value = DetalheDistancia;
			if (PartJogo.Using)
				PartJogo.Terreno.detailObjectDistance = DetalheDistancia;
		}

		else
		{
			DetalheDistancia = 250f;
			DistanciaDetalhe.value = DetalheDistancia;
			PlayerPrefs.SetFloat("distanciadetalhe", DetalheDistancia);
		}

		if (PlayerPrefs.HasKey("distanciaarvores"))
		{
			DistanciaArvoresF = PlayerPrefs.GetFloat("distanciaarvores");
			DistanciaArvores.value = DistanciaArvoresF;
			if (PartJogo.Using)
				PartJogo.Terreno.treeDistance = DistanciaArvoresF;
		}

		else
		{
			DistanciaArvoresF = 3000f;
			DistanciaArvores.value = DistanciaArvoresF;
			PlayerPrefs.SetFloat("distanciaarvores", DistanciaArvoresF);
		}

		//=============MODO JANELA===========//
		CriarToggle("modoJanela", modoJanelaAtivo, Screen.fullScreen, CaixaModoJanela, false);

		//=========Profundidade Campo========//
		CriarToggle("profundidadecampo", ProfundidadeCampoInt, ProfundidadeCampoBool, ProfundidadeCampo);

		//=========Oclusao Ambiental========//
		CriarToggle("oclusaoambiental", OclusaoAmbientalInt, OclusaoAmbientalBool, OclusaoAmbiental);

		//=========Nevoa========//
		CriarToggle("nevoa", NevoaInt, NevoaBool, Nevoa);

		//=========Adaptaçao Ocular========//
		CriarToggle("adaptacaoocular", AdaptacaoOcularInt, AdaptacaoOcularBool, AdaptacaoOcular);

		//=========Anti Aliasing========//
		CriarToggle("antialiasing", AntiAliasingInt, AntiAliasingBool, AntiAliasing);

		//=========Inverter Mouse========//
		if (PartJogo.Using)
			CriarToggle("invertermouse", InverterMouseInt, PartJogo.mouse.InverterMouse, InverterMouse, false);

		else if (PartMenu.Using)
			CriarToggle("invertermouse", InverterMouseInt, PartMenu.MouseBool, InverterMouse, false);

		//========DesfoqueMovimnto========//
		CriarToggle("desfoquemovimento", DesfoqueMovimentoInt, DesfoqueMovimentoBool, DesfoqueMovimento);

		if (CaixaModoJanela.isOn == true)
		{
			modoJanelaAtivo = 1;
			telaCheiaAtivada = false;
		}

		else
		{
			modoJanelaAtivo = 0;
			telaCheiaAtivada = true;
		}

		//========RESOLUCOES========//
		if (PlayerPrefs.HasKey ("RESOLUCAO")) 
		{
			resolucaoSalveIndex = PlayerPrefs.GetInt ("RESOLUCAO");
			Screen.SetResolution(resolucoesSuportadas[resolucaoSalveIndex].width,resolucoesSuportadas[resolucaoSalveIndex].height,telaCheiaAtivada, Screen.currentResolution.refreshRate);
			Resolucoes.value = resolucaoSalveIndex;
		} 

		else 
		{
			resolucaoSalveIndex = resolucoesSuportadas.Count-1;
			Screen.SetResolution(resolucoesSuportadas[resolucaoSalveIndex].width,resolucoesSuportadas[resolucaoSalveIndex].height,telaCheiaAtivada);
			PlayerPrefs.SetInt ("RESOLUCAO", resolucaoSalveIndex);
			Resolucoes.value = resolucaoSalveIndex;
		}

		//=========QUALIDADES=========//
		if (PlayerPrefs.HasKey ("qualidadeGrafica")) 
		{
			qualidadeGrafica = PlayerPrefs.GetInt ("qualidadeGrafica");
			QualitySettings.SetQualityLevel(qualidadeGrafica);
			Qualidades.value = qualidadeGrafica;
		}

		else 
		{
			QualitySettings.SetQualityLevel((QualitySettings.names.Length-1));
			qualidadeGrafica = 3;
			PlayerPrefs.SetInt ("qualidadeGrafica", qualidadeGrafica);
			Qualidades.value = qualidadeGrafica;
		}

		//==========SOMBRAS==========//
		Sombra();
		Time.timeScale = 1;
		AudioListener.volume = VOLUME / 100;
		BarraVolume.minValue = 0;
		BarraVolume.maxValue = 100;

		if (PartJogo.Using)
			PartJogo.menuParte1Ativo = PartJogo.menuParte2Ativo = false;

		// =========SETAR BOTOES==========//
		if (PartJogo.Using)
		{
			PartJogo.BotaoVoltarAoMenu.onClick = new Button.ButtonClickedEvent();
			PartJogo.BotaoOpcoes.onClick = new Button.ButtonClickedEvent();
			PartJogo.BotaoRetornarAoJogo.onClick = new Button.ButtonClickedEvent();
		}

		else if (PartMenu.Using)
        {
			PartMenu.BotaoUmJogador.onClick = new Button.ButtonClickedEvent();
			PartMenu.BotaoOpcoes.onClick = new Button.ButtonClickedEvent();
			PartMenu.BotaoSair.onClick = new Button.ButtonClickedEvent();
		}

		BotaoVoltar.onClick = new Button.ButtonClickedEvent();
		BotaoSalvar.onClick = new Button.ButtonClickedEvent();
		BotaoAudio.onClick = new Button.ButtonClickedEvent();
		BotaoGrafico.onClick = new Button.ButtonClickedEvent();
		BotaoTela.onClick = new Button.ButtonClickedEvent();
		BotaoTecladoMouse.onClick = new Button.ButtonClickedEvent();
		BotaoLeftQ.onClick = new Button.ButtonClickedEvent();
		BotaoRightQ.onClick = new Button.ButtonClickedEvent();
		BotaoLeftR.onClick = new Button.ButtonClickedEvent();
		BotaoRightR.onClick = new Button.ButtonClickedEvent();

		//
		if (PartJogo.Using)
		{
			PartJogo.BotaoVoltarAoMenu.onClick.AddListener(() => VoltarAoMenu());
			PartJogo.BotaoOpcoes.onClick.AddListener(() => Opcoes(false, true));
			PartJogo.BotaoRetornarAoJogo.onClick.AddListener(() => Opcoes(false, false));
			PartJogo.BotaoRetornarAoJogo.onClick.AddListener(() => Retornar());
			BotaoVoltar.onClick.AddListener(() => Opcoes(true, false));
		}

		else if (PartMenu.Using)
		{
			PartMenu.BotaoUmJogador.onClick.AddListener(() => UmJogador());
			PartMenu.BotaoOpcoes.onClick.AddListener(() => Opcoes(false,false));
			PartMenu.BotaoSair.onClick.AddListener(() => Sair());
			BotaoVoltar.onClick.AddListener(() => Voltar());
		}

		//
		BotaoSalvar.onClick.AddListener(() => SalvarPreferencias());
		BotaoAudio.onClick.AddListener(() => Audio());
		BotaoGrafico.onClick.AddListener(() => Graficos());
		BotaoTela.onClick.AddListener(() => Tela());
		BotaoTecladoMouse.onClick.AddListener(() => TecladoMouse());
		BotaoLeftQ.onClick.AddListener(() => Left(Qualidades));
		BotaoRightQ.onClick.AddListener(() => Right(Qualidades));
		BotaoLeftS.onClick.AddListener(() => Left(Sombras));
		BotaoRightS.onClick.AddListener(() => Right(Sombras));
		BotaoLeftR.onClick.AddListener(() => Left(Resolucoes));
		BotaoRightR.onClick.AddListener(() => Right(Resolucoes));
	}
	void Update()
	{
		if (PartJogo.Using)
		{
			if (PartJogo.controlador == null && MouseLook.player != null)
			{
				PartJogo.controlador = MouseLook.player.GetComponent<Movimentacao>();
				return;
			}

			if (PartJogo.Mira2.Anim == null && MouseLook.player != null)
				PartJogo.Mira2.Anim = MouseLook.player.GetComponent<Animacoes>();

			if (Input.GetKeyDown(KeyCode.Escape) && PartJogo.Actived)
			{
				Cursor.lockState = CursorLockMode.None;

				// Abre o menu pause
				if (PartJogo.menuParte1Ativo == false && PartJogo.menuParte2Ativo == false)
				{
					PartJogo.menuParte1Ativo = true;
					PartJogo.menuParte2Ativo = false;
					Opcoes(true, false);

					if (!PhotonNetwork.IsConnected)
						Time.timeScale = 0;

					MenuOpenFun(true);
					//AudioListener.volume = 0;
				}
			}
		}

		if (MenuOpen)
		{
			TxtDistanciaArvores.text = (int)DistanciaArvores.value + "";
			TxtDetelheDistancia.text = (int)DistanciaDetalhe.value + "";
			PorcentagemVolume.text = (int)BarraVolume.value + "%";
			PorcentagemSensitivy.text = (int)Sensibilidade.value + "%";
		}
	}
	 //=========VOIDS DE CHECAGEM==========//
	private void ChecarResolucoes()
	{
		//Resolution[] resolucoesSuportadas = Screen.resolutions;
		Resolucoes.options.Clear ();

		for (int y = 0; y < resolucoesSuportadas.Count; y++)
		{
			Resolucoes.options.Add(new TMP_Dropdown.OptionData() { text = resolucoesSuportadas[y].width + "x" + resolucoesSuportadas[y].height });
		}

		Resolucoes.captionText.text = "1280x720";
	}

	public void ChecarSombras(string Idioma)
	{
		Sombras.options.Clear();

		for (int x = 0; x < NomesSombraIngles.Count; x++)
		{
			if (Idioma == "ingles")
			{
				Sombras.options.Add(new TMP_Dropdown.OptionData() { text = NomesSombraIngles[x] });
				Sombras.captionText.text = NomesSombraIngles[Sombras.value];
			}

			else
			{
				Sombras.options.Add(new TMP_Dropdown.OptionData() { text = NomesSombraPortugues[x] });
				Sombras.captionText.text = NomesSombraPortugues[Sombras.value];
			}
		}
	}

	public void AjustarQualidades(string Idioma)
	{
		string[] nomes = QualitySettings.names;
		Qualidades.options.Clear ();

		for(int y = 0; y < nomes.Length; y++)
		{
			if (Idioma == "ingles")
			{
				Qualidades.options.Add(new TMP_Dropdown.OptionData() { text = NomesQualyIngles[y] });
				Qualidades.captionText.text = NomesQualyIngles[Qualidades.value];
			}

			else
			{
				Qualidades.options.Add(new TMP_Dropdown.OptionData() { text = nomes[y] });
				Qualidades.captionText.text = nomes[Qualidades.value];
			}
		}
	}

	private void Opcoes(bool ativarOP, bool ativarOP2)
	{
		if (PartMenu.Using)
		{
			PainelMenu.SetActive(true);
			MenuOpen = true;
		}

		DesabilitarPaineis();
		PainelAudio.SetActive(true);
		BotaoAudio.interactable = false;
		TxtNome.text = BotaoAudio.GetComponentInChildren<TextMeshProUGUI>().text;
		//
		if (PartJogo.Using)
		{
			PartJogo.BotaoVoltarAoMenu.gameObject.SetActive(ativarOP);
			PartJogo.BotaoOpcoes.gameObject.SetActive(ativarOP);
			PartJogo.BotaoRetornarAoJogo.gameObject.SetActive(ativarOP);
			//
			if (ativarOP && !ativarOP2)
			{
				PartJogo.menuParte1Ativo = true;
				PartJogo.menuParte2Ativo = false;
				PainelMenu.SetActive(false);
				Cursor.visible = true;
			}

			else if (!ativarOP && ativarOP2)
			{
				PartJogo.menuParte1Ativo = false;
				PartJogo.menuParte2Ativo = true;
				PainelMenu.SetActive(true);
				Cursor.visible = true;
			}

			else if (!ativarOP && !ativarOP2)
			{
				PartJogo.menuParte1Ativo = false;
				PartJogo.menuParte2Ativo = false;
				PainelMenu.SetActive(false);
				Time.timeScale = 1;
				AudioListener.volume = VOLUME / 100;
			}
		}
	}

	//=========VOIDS DE SALVAMENTO==========//
	private void SalvarPreferencias()
	{
		if (ProfundidadeCampo.isOn)
		{
			ProfundidadeCampoInt = 1;
			ProfundidadeCampoBool = true;
		}

		else
		{
			ProfundidadeCampoInt = 0;
			ProfundidadeCampoBool = false;
		}

		if (OclusaoAmbiental.isOn)
		{
			OclusaoAmbientalInt = 1;
			OclusaoAmbientalBool = true;
		}

		else
		{
			OclusaoAmbientalInt = 0;
			OclusaoAmbientalBool = false;
		}

		if (Nevoa.isOn)
		{
			NevoaInt = 1;
			NevoaBool = true;
		}

		else
		{
			NevoaInt = 0;
			NevoaBool = false;
		}

		if (AdaptacaoOcular.isOn)
		{
			AdaptacaoOcularInt = 1;
			AdaptacaoOcularBool = true;
		}

		else
		{
			AdaptacaoOcularInt = 0;
			AdaptacaoOcularBool = false;
		}

		if (DesfoqueMovimento.isOn)
		{
			DesfoqueMovimentoInt = 1;
			DesfoqueMovimentoBool = true;
		}

		else
		{
			DesfoqueMovimentoInt = 0;
			DesfoqueMovimentoBool = false;
		}

		if (InverterMouse.isOn) 
		{
			InverterMouseInt = 1;

			if (PartJogo.Using)
				PartJogo.mouse.InverterMouse = true;

			else if (PartMenu.Using)
				PartMenu.MouseBool = true;
		}

		else 
		{
			InverterMouseInt = 0;

			if (PartJogo.Using)
				PartJogo.mouse.InverterMouse = false;

			else if (PartMenu.Using)
				PartMenu.MouseBool = true;
		}

		if (AntiAliasing.isOn)
		{
			AntiAliasingInt = 1;
			AntiAliasingBool = true;
		}

		else
		{
			AntiAliasingInt = 0;
			AntiAliasingBool = false;
		}

		if (CaixaModoJanela.isOn == true) 
		{
			modoJanelaAtivo = 1;
			telaCheiaAtivada = false;
		}

		else 
		{
			modoJanelaAtivo = 0;
			telaCheiaAtivada = true;
		}

		if (Sensibilidade.value < 10)
			Sensibilidade.value = 10f;

		DetalheDistancia = DistanciaDetalhe.value;
		DistanciaArvoresF = DistanciaArvores.value;
		//
		PlayerPrefs.SetFloat ("VOLUME", BarraVolume.value);
		PlayerPrefs.SetInt ("qualidadeGrafica", Qualidades.value);
		PlayerPrefs.SetInt ("sombra", Sombras.value);
		PlayerPrefs.SetInt ("modoJanela", modoJanelaAtivo);
		PlayerPrefs.SetInt ("profundidadecampo", ProfundidadeCampoInt);
		PlayerPrefs.SetInt ("oclusaoambiental", OclusaoAmbientalInt);
		PlayerPrefs.SetInt ("nevoa", NevoaInt);
		PlayerPrefs.SetInt ("adaptacaoocular", AdaptacaoOcularInt);
		PlayerPrefs.SetInt ("antialiasing", AntiAliasingInt);
		PlayerPrefs.SetInt ("desfoquemovimento", DesfoqueMovimentoInt);
		PlayerPrefs.SetInt ("invertermouse", InverterMouseInt);
		PlayerPrefs.SetInt ("RESOLUCAO", Resolucoes.value);
		PlayerPrefs.SetFloat ("sensibilidade", Sensibilidade.value/20);
		PlayerPrefs.SetFloat ("distanciadetalhe", DetalheDistancia);
		PlayerPrefs.SetFloat ("distanciaarvores", DistanciaArvoresF);
		//
		resolucaoSalveIndex = Resolucoes.value;
		VOLUME = PlayerPrefs.GetFloat("VOLUME");

		if (PartJogo.Using)
		{
			if (PartJogo.GC.IsScoped)
				PartJogo.GC.GunSelected.GetComponent<Gun>().SensitivyFOV();			

			else
			{
				PartJogo.mouse.sensitivityX = PlayerPrefs.GetFloat("sensibilidade");
				PartJogo.mouse.sensitivityY = PlayerPrefs.GetFloat("sensibilidade");
			}

            PartJogo.Terreno.detailObjectDistance = DetalheDistancia;
			PartJogo.Terreno.treeDistance = DistanciaArvoresF;
		}

		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qualidadeGrafica"));
		Sombra();
		Screen.SetResolution(resolucoesSuportadas[resolucaoSalveIndex].width, resolucoesSuportadas[resolucaoSalveIndex].height, telaCheiaAtivada);

		if (PhotonNetwork.IsConnected) 
			FindObjectOfType<ChatMsm>().ClearChat();
	}
	private void DesabilitarPaineis()
	{
		PainelAudio.SetActive(false);
		PainelGraficos.SetActive(false);
		PainelTela.SetActive(false);
		PainelTecladoMouse.SetActive(false);
		//
		BotaoTela.interactable = true;
		BotaoAudio.interactable = true;
		BotaoGrafico.interactable = true;
		BotaoTecladoMouse.interactable = true;
	}
	//===========VOIDS NORMAIS=========//
	private void UmJogador()
	{
		FindObjectOfType<servidor>().SceneName = "Game";
		SceneManager.LoadScene(NomeCena);
	}
	private void Voltar()
	{
		PainelMenu.SetActive(false);
		MenuOpen = false;
	}
	private void Sair()
	{
		Application.Quit();
	}
	private void VoltarAoMenu()
	{
		FindObjectOfType<servidor>().SceneName = "MENU";
		SceneManager.LoadScene (NomeCena);

		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.LeaveRoom();
		}
	}
	private void Retornar()
	{
		if (!Status.Morreu)
			MenuOpenFun(false);
		else
			MenuOpen = false;
    }

    private void Tela()
	{
		DesabilitarPaineis();
		PainelTela.SetActive(true);
		BotaoTela.interactable = false;
		TxtNome.text = BotaoTela.GetComponentInChildren<TextMeshProUGUI>().text;
	}

	private void Graficos()
	{
		DesabilitarPaineis();
		PainelGraficos.SetActive(true);
		BotaoGrafico.interactable = false;
		TxtNome.text = BotaoGrafico.GetComponentInChildren<TextMeshProUGUI>().text;
	}

	private void Audio()
	{
		DesabilitarPaineis();
		PainelAudio.SetActive(true);
		BotaoAudio.interactable = false;
		TxtNome.text = BotaoAudio.GetComponentInChildren<TextMeshProUGUI>().text;
	}

	private void TecladoMouse()
	{
		DesabilitarPaineis();
		PainelTecladoMouse.SetActive(true);
		BotaoTecladoMouse.interactable = false;
		TxtNome.text = BotaoTecladoMouse.GetComponentInChildren<TextMeshProUGUI>().text;
	}

	public void Left(TMP_Dropdown DropDown)
	{
			int valueAtual = DropDown.value;
			DropDown.value -= 1;
			if (DropDown.value == valueAtual)
				DropDown.value = DropDown.options.Count - 1;
	}

	public void Right(TMP_Dropdown DropDown)
	{
		int valueAtual = DropDown.value;
		DropDown.value += 1;
		if (DropDown.value == valueAtual)
			DropDown.value = 0;
	}

	private Button AddButtons(TMP_Dropdown DropDown, string BotaoName)
    {
		Button Botao = DropDown.transform.Find(BotaoName).GetComponent<Button>();
		return Botao;
	}

	private void Sombra()
	{
		if (PlayerPrefs.HasKey("sombra"))
		{
			SombraInt = PlayerPrefs.GetInt("sombra");

			if (SombraInt == 0)
			{
				QualitySettings.shadows = ShadowQuality.Disable;
				Sombras.value = SombraInt;
			}

			else if (SombraInt == 1)
			{
				QualitySettings.shadows = ShadowQuality.HardOnly;
				Sombras.value = SombraInt;
			}

			else
			{
				QualitySettings.shadows = ShadowQuality.All;
				Sombras.value = SombraInt;
			}
		}
		else
		{
			QualitySettings.shadows = ShadowQuality.HardOnly;
			SombraInt = 1;
			PlayerPrefs.SetInt("sombra", SombraInt);
			Sombras.value = SombraInt;
		}
	}

	private void CriarToggle(string Key,int ObjectInt,bool ObjectBool,Toggle ObjectToggle, bool Ativo=true)
	{
		if (PlayerPrefs.HasKey(Key))
		{
			ObjectInt = PlayerPrefs.GetInt(Key);

			if (ObjectInt == 1)
				ObjectToggle.isOn = true;

			else
				ObjectToggle.isOn = false;
		}

		else
		{
			if (Ativo)
			{
				ObjectInt = 1;
				PlayerPrefs.SetInt(Key, ObjectInt);
				ObjectToggle.isOn = true;
			}

			else
			{
				ObjectInt = 0;
				PlayerPrefs.SetInt(Key, ObjectInt);
				ObjectToggle.isOn = false;
			}
		}
	}

	public void VisibleMenu(bool Active)
    {
		PartMenu.BotaoUmJogador.gameObject.SetActive(Active);
		PartMenu.BotaoOpcoes.gameObject.SetActive(Active);
		PartMenu.BotaoSair.gameObject.SetActive(Active);
	}

	public void MenuOpenFun(bool Active)
    {
		GunsControl GC = FindObjectOfType<GunsControl>();
		bool Scope = GC.IsScoped;
		MenuOpen = Active;

		if (Active)
		{
            lingua.Jogo.Interagir.gameObject.SetActive(false);
            PartJogo.Mira2.ShowDestroy.gameObject.SetActive(false);
		}

		if (GC.GunSelected != null)
		{
			if (GC.GunSelected.GetComponent<Gun>())
				GC.GunSelected.GetComponent<Gun>().enabled = !Active;
			else 
				GC.GunSelected.GetComponent<RPG_Gun>().enabled = !Active;
		}

		Cursor.visible = Active;

		if (!PartJogo.controlador.Dirigindo)
			PartJogo.controlador.enabled = !Active;
		else
			PartJogo.controlador.VehicleUsing.GetComponent<Helicopter_Controller>().enabled = !Active;

		if (!Scope)
			PartJogo.Mira2.enabled = !Active;

        PartJogo.SelectSlot.enabled = !Active;
        MouseLook.MouseEnable = !Active;
		print("foi aqui");
	}
} 