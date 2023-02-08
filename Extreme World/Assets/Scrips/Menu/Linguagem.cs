using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum InteragirEnum
{
	Entrar_Veiculo,
	Acessar_Metralhadora,
	Sair,
	Pegar_Item,
	
}

[System.Serializable]
public class IdiomaMenu
{
	public bool Using;
	// Home
	public TextMeshProUGUI BotaoUmJogador, BotaoMultiJogador, BotaoOpcoes, BotaoSair;
	// Room
	public TextMeshProUGUI CriarSala, Voltar, Nome, ProcurandoSalas, Conectando, NomeDaSala,
		Criar, Cancelar, MaximoDeJogadores, NameErro, NameSalaErro, MaxPlayerErro;
}

[System.Serializable]
public class IdiomaJogo
{
	public bool Using;
	public TextMeshProUGUI Retomar, Opcoes, VoltarAoMenu, Interagir;
}

public class Linguagem : MonoBehaviour 
{
	private MenuPause menu;
	public int Lingua;
	public enum Idioma {Portugues = 0, Ingles = 1}
	public Idioma idm = Idioma.Portugues;
	public TMP_Dropdown IdiomaDropDown;
	public IdiomaMenu Menu = new IdiomaMenu();
	public IdiomaJogo Jogo = new IdiomaJogo();

	[Header("Menu")]
	public TextMeshProUGUI Volume;
	public TextMeshProUGUI modoJanela, Resolucoes, IdiomaText, Qualidades, BotaoVoltar,
		BotaoSalvar, BotaoAudio, BotaoTela, BotaoGraficos,
		BotaoTecladoMouse, TxtMenu, TxtSensibilidade,
		TxtInverterMouse, TxtDistanciaDetalhe, TxtSombra; //TxtDesfoqueMovimento, TxtAdaptacaoOcular, TxtNevoa,
		//TxtOclusaoAmbiental, TxtProfundidadeCampo, TxtDistanciaArvores;

	// privates
	private Button BotaoLeftI, BotaoRightI;
    [HideInInspector] public List<string> NomesIdiomaIngles, NomesIdiomaPortugues = new List<string>();

	void Awake()
	{
		//Inter.Add(Idioma.Portugues, "oi");
		//print(Inter[Idioma.Portugues]);
		menu = FindObjectOfType<MenuPause>();
		
		BotaoLeftI = IdiomaDropDown.transform.Find("Left").GetComponent<Button>();
		BotaoRightI = IdiomaDropDown.transform.Find("Right").GetComponent<Button>();
		//
		NomesIdiomaIngles.Add("Portuguese"); NomesIdiomaIngles.Add("English");
		NomesIdiomaPortugues.Add("Português"); NomesIdiomaPortugues.Add("Inglês"); 
		
		BotaoLeftI.onClick = new Button.ButtonClickedEvent();
		BotaoRightI.onClick = new Button.ButtonClickedEvent();
		//
		BotaoLeftI.onClick.AddListener(() => Left(IdiomaDropDown));
		BotaoRightI.onClick.AddListener(() => Right(IdiomaDropDown));
		AnalizarIdioma(true);
	}

	public void Portugues(bool start=false)
	{
		idm = Idioma.Portugues;
		PlayerPrefs.SetInt("idioma", 0);
		menu.TxtNome.text = "Tela";

		//       DropDown
		IdiomaDropDown.options.Clear();
		foreach (string nome in NomesIdiomaPortugues)
        {
			IdiomaDropDown.options.Add(new TMP_Dropdown.OptionData() { text = nome });
			IdiomaDropDown.captionText.text = NomesIdiomaPortugues[IdiomaDropDown.value];
        }

		if (!start)
		{
			menu.AjustarQualidades("portugues");
			menu.ChecarSombras("portugues");
		}
		
		if (Menu.Using)
		{
			//        Menu Principal
			Menu.BotaoUmJogador.text = "Um Jogador";
			Menu.BotaoMultiJogador.text = "MultiJogador";
			Menu.BotaoOpcoes.text = "Opções";
			Menu.BotaoSair.text = "Sair";
			//	            Salas
			Menu.CriarSala.text = "Criar Sala";
			Menu.Voltar.text = "Voltar";
			Menu.Nome.text = "NomePlayer";
			Menu.ProcurandoSalas.text = "Procurando Salas ";
			Menu.Conectando.text = "Conectando ";
			Menu.NameErro.text = "Escolha um nome para seu personagem !";
			Menu.NameSalaErro.text = "Escolha um nome para sua sala !";
			Menu.MaxPlayerErro.text = "A Sala esta cheia !";
			//         Criador de Salas
			Menu.NomeDaSala.text = "NomePlayer Da Sala";
			Menu.Criar.text = "Criar";
			Menu.Cancelar.text = "Cancelar";
			Menu.MaximoDeJogadores.text = "Maximo De Jogadores";
		}

		if (Jogo.Using)
        {
			Jogo.Retomar.text = "Retomar";
			Jogo.Opcoes.text = "Opções";
			Jogo.VoltarAoMenu.text = "Volta ao Menu";
		}

		//        Menu De Configurações
		Volume.text = "Som";
		modoJanela.text = "Modo Janela";
		Resolucoes.text = "Resoluções";
		IdiomaText.text = "Idioma";
		Qualidades.text = "Graficos";
		BotaoVoltar.text = "Voltar";
		BotaoSalvar.text = "Salvar";
		BotaoAudio.text = "Audio";
		BotaoGraficos.text = "Gráficos";
		BotaoTela.text = "Tela";
		BotaoTecladoMouse.text = "Teclado / Mouse";
		TxtMenu.text = "Opções";
		TxtSensibilidade.text = "Sensibilidade Do Mouse";
		TxtInverterMouse.text = "Inverter Mouse";
		TxtSombra.text = "Sombra";
		TxtDistanciaDetalhe.text = "Distancia Dos Detalhes";
		/*TxtAdaptacaoOcular.text = "Adaptação Ocular";
		TxtDesfoqueMovimento.text = "Desfoque De Movimento";
		TxtNevoa.text = "Névoa";
		TxtOclusaoAmbiental.text = "Oclusão Ambiental";
		TxtProfundidadeCampo.text = "Profundidade De Campo";
		TxtDistanciaArvores.text = "Distancia Das Árvores";*/
	}

	public void Ingles(bool start=false)
	{
		idm = Idioma.Ingles;
		PlayerPrefs.SetInt("idioma", 1);
		//       DropDown
		IdiomaDropDown.options.Clear();
		foreach (string nome in NomesIdiomaIngles)
		{
			IdiomaDropDown.options.Add(new TMP_Dropdown.OptionData() { text = nome });
			IdiomaDropDown.value = 1;
			IdiomaDropDown.captionText.text = NomesIdiomaIngles[IdiomaDropDown.value];
		}

		if (Menu.Using)
		{
			menu.TxtNome.text = "Screen";
			if (!start)
			{
				menu.AjustarQualidades("ingles");
				menu.ChecarSombras("ingles");
			}

			//        Menu Principal
			Menu.BotaoUmJogador.text = "SinglePlayer";
			Menu.BotaoMultiJogador.text = "MultiPlayer";
			Menu.BotaoOpcoes.text = "Options";
			Menu.BotaoSair.text = "Exit";
			//	            Salas
			Menu.CriarSala.text = "Create Room";
			Menu.Voltar.text = "Back";
			Menu.Nome.text = "Name";
			Menu.ProcurandoSalas.text = "Searching Rooms ";
			Menu.Conectando.text = "Connecting ";
			Menu.NameErro.text = "Choose a name for your character !";
			Menu.NameSalaErro.text = "Choose a name for your room !";
			Menu.MaxPlayerErro.text = "The room is full !";
			//         Criador de Salas
			Menu.NomeDaSala.text = "Room Name";
			Menu.Criar.text = "Create";
			Menu.Cancelar.text = "Cancel";
			Menu.MaximoDeJogadores.text = "Maximum Players";
		}

		if (Jogo.Using)
		{
			Jogo.Retomar.text = "Resume";
			Jogo.Opcoes.text = "Options";
			Jogo.VoltarAoMenu.text = "Back To Menu";
		}
		//       Menu De Configurações
		Volume.text = "Sound";
		modoJanela.text = "Windowed";
		Resolucoes.text = "Resolutions";
		IdiomaText.text = "Language";
		Qualidades.text = "Graphics";
		BotaoVoltar.text = "Back";
		BotaoSalvar.text = "Save";
		BotaoAudio.text = "Audio";
		BotaoGraficos.text = "Graphics";
		BotaoTela.text = "Screen";
		BotaoTecladoMouse.text = "Keyboard / Mouse";
		TxtMenu.text = "Options";
		TxtSensibilidade.text = "Mouse Sensitivity";
		TxtInverterMouse.text = "Reverse Mouse";
		TxtSombra.text = "Shadown";
		TxtDistanciaDetalhe.text = "Detail Distance";
		/*TxtAdaptacaoOcular.text = "Eye Adaptation";
		TxtDesfoqueMovimento.text = "Motion Blur";
		TxtNevoa.text = "Fog";
		TxtOclusaoAmbiental.text = "Ambient Occlusion";
		TxtProfundidadeCampo.text = "Depth Of Fild";
		TxtDistanciaArvores.text = "Tree Distance";*/
	}

	private void AnalizarIdioma(bool Start=false)
    {
		if (PlayerPrefs.HasKey("idioma"))
			Lingua = PlayerPrefs.GetInt("idioma");
		else
			PlayerPrefs.SetInt("idioma", Lingua);
		if (Lingua == 0)
			Portugues(Start);
		else
			Ingles(Start);
	}

	private void Left(TMP_Dropdown DropDown)
	{
		int valueAtual = DropDown.value;
		DropDown.value -= 1;
		if (DropDown.value == valueAtual)
			DropDown.value = DropDown.options.Count - 1;
		PlayerPrefs.SetInt("idioma", IdiomaDropDown.value);
		AnalizarIdioma();
	}

	private void Right(TMP_Dropdown DropDown)
	{
		int valueAtual = DropDown.value;
		DropDown.value++;
		if (DropDown.value == valueAtual)
			DropDown.value = 0;
		PlayerPrefs.SetInt("idioma", IdiomaDropDown.value);
		AnalizarIdioma();
	}

	public void Interagir(InteragirEnum InteragirStats)
	{
		Jogo.Interagir.gameObject.SetActive(true);
		switch (InteragirStats) 
		{
			case InteragirEnum.Pegar_Item:
				{
					if (idm == Idioma.Portugues)
					{
                        Jogo.Interagir.text = "Pressione [E] para Pegar";
                    }
					else
					{
                        Jogo.Interagir.text = "Press [E] to pick up";
                    }
				}
				break;
			case InteragirEnum.Entrar_Veiculo:
				{
                    if (idm == Idioma.Portugues)
                    {
                        Jogo.Interagir.text = "Pressione [F] para Entrar";
                    }
                    else
                    {
                        Jogo.Interagir.text = "Press [F] to Enter";
                    }
                }
				break;
			case InteragirEnum.Sair:
				{
					if (idm == Idioma.Portugues)
						Jogo.Interagir.text = "Pressiono [F] para Sair";
					else
                        Jogo.Interagir.text = "Press [F] to Exit";
                }
				break;
			case InteragirEnum.Acessar_Metralhadora:
				{
					if (idm == Idioma.Portugues)
                        Jogo.Interagir.text = "Pressiono [C] para acessar a Metralhadora ";
					else
                        Jogo.Interagir.text = "Press [C] to access Machine Gun";

                }
				break;
        }
	}
}
