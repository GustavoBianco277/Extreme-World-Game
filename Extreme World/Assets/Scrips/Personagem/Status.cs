using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Status : MonoBehaviour 
{
    // GameObject PainelMorto;
	public GameObject PainelMorto;
	public AudioSource[] HitSound;
	private GameObject Jogador;
	private EfeitoEscurecer Efeito;
	private GunsControl GC;
	private MenuPause Menu;
    //private float UltimaPosicaoEmY,DistanciaDeQueda;
    [Range(1,15)]
	public float AlturaQueda = 4,DanoPorMetro = 5 ;
	public Image BarraVida, BarraEstamina, BarraFome, BarraSede;
	public Slider BarraVidaGunsMode, BarraEstaminaGunsMode;
	[Range(20,5000)]
	public float VidaCheia = 100, EstaminaCheia = 100, FomeCheia = 100, SedeCheia = 100,velocidadeEstamina = 250;
	//[HideInInspector]
	public float VidaAtual, EstaminaAtual, FomeAtual, SedeAtual;
	public bool semEstamina;
	public static bool Morreu;
	[HideInInspector]
	public float cronometroFome,cronometroSede,velocidadeCaminhando,velocidadeCorrendo;
	void Start ()
	{
		Morreu = false;
		Efeito = FindObjectOfType<EfeitoEscurecer>();
		VidaAtual = VidaCheia;
		EstaminaAtual = EstaminaCheia;
		FomeAtual = FomeCheia;
		SedeAtual = SedeCheia;
		GC = FindObjectOfType<GunsControl>();
		Menu = FindObjectOfType<MenuPause>();
    }
	void Update ()
	{
		if (Jogador == null)
		{
			if (MouseLook.player != null)
				Jogador = MouseLook.player.gameObject;
			return;
		}

		else if (velocidadeCaminhando == 0) 
		{
			velocidadeCaminhando = Jogador.GetComponent<Movimentacao>().VelocidadeAndando;
			velocidadeCorrendo = Jogador.GetComponent<Movimentacao>().VelocidadeCorrendo;
		}
		SistemaDeQueda ();
		SistemaDeVida ();
		SistemaDeEstamina ();
		SistemaDeFome ();
		SistemaDeSede ();
		AplicarBarras ();
	}
	void SistemaDeQueda()
	{
		/*if (UltimaPosicaoEmY > Jogador.transform.position.y) {
			DistanciaDeQueda += UltimaPosicaoEmY-Jogador.transform.position.y;
		}
		UltimaPosicaoEmY = Jogador.transform.position.y;
		if (DistanciaDeQueda >= AlturaQueda) {
			VidaAtual = VidaAtual - DanoPorMetro*DistanciaDeQueda;
			DistanciaDeQueda = 0;
			UltimaPosicaoEmY = 0;
		}
		if (DistanciaDeQueda < AlturaQueda) {
			DistanciaDeQueda = 0;
			UltimaPosicaoEmY = 0;
		}*/
	}
	void SistemaDeFome()
	{
		FomeAtual -= Time.deltaTime /2;
		if (FomeAtual >= FomeCheia) 
			FomeAtual = FomeCheia;
		if (FomeAtual <= 0) 
		{
			FomeAtual = 0;
			cronometroFome += Time.deltaTime;
			if (cronometroFome >= 3) 
			{
				Hit(VidaCheia * 0.0015f);
				EstaminaAtual -= (EstaminaCheia * 0.01f);
				cronometroFome = 0;
			}
		}
		else
			cronometroFome = 0;
	}
	void SistemaDeSede()
	{
		SedeAtual -= Time.deltaTime / 1.5f;
		if (SedeAtual >= SedeCheia) 
			SedeAtual = SedeCheia;

		if (SedeAtual <= 0) 
		{
			SedeAtual = 0;
			cronometroSede += Time.deltaTime;
			if (cronometroSede >= 3) 
			{
				Hit(VidaCheia * 0.002f);
				EstaminaAtual -= (EstaminaCheia * 0.01f);
				cronometroSede = 0;
			}
		}
		else 
			cronometroSede = 0;
	}
	void SistemaDeEstamina()
	{
		float multEuler = ((1/EstaminaCheia) * EstaminaAtual)*((1/FomeCheia) * FomeAtual);
		if (EstaminaAtual >= EstaminaCheia)
			EstaminaAtual = EstaminaCheia;
		else
			EstaminaAtual += Time.deltaTime * (velocidadeEstamina / 40) * Mathf.Pow(2.718f, multEuler);
		if (EstaminaAtual <= 0) 
		{
			EstaminaAtual = 0;
			Jogador.GetComponent<Movimentacao>().VelocidadeCorrendo = velocidadeCaminhando;
			semEstamina = true;
		}
		if (semEstamina == true && EstaminaAtual >= (EstaminaCheia * 0.15f)) 
		{
			Jogador.GetComponent<Movimentacao>().VelocidadeCorrendo = velocidadeCorrendo;
			semEstamina = false;
		}
		if (Jogador.GetComponent<Movimentacao>().Correndo && semEstamina == false)
			EstaminaAtual -= Time.deltaTime * (velocidadeEstamina / 15) * Mathf.Pow(2.718f, multEuler);
	}
	void SistemaDeVida()
	{
		if (VidaAtual >= VidaCheia)
			VidaAtual = VidaCheia;
	}
	void AplicarBarras()
	{
        if (GunsControl.GunsMode)
        {
            BarraVidaGunsMode.value = ((1 / VidaCheia) * VidaAtual);
            BarraEstaminaGunsMode.value = ((1 / EstaminaCheia) * EstaminaAtual);
        }
        else
        {
            BarraVida.fillAmount = ((1 / VidaCheia) * VidaAtual);
            BarraEstamina.fillAmount = ((1 / EstaminaCheia) * EstaminaAtual);
            BarraFome.fillAmount = ((1 / FomeCheia) * FomeAtual);
            BarraSede.fillAmount = ((1 / SedeCheia) * SedeAtual);
        }
    }
    	
	public void Respawn ()
	{
		if (PhotonNetwork.IsConnected)
			servidor.Server.RPC("Morte", RpcTarget.Others, Jogador.GetComponent<PhotonView>().ViewID, false);

		Morreu = false;
        Camera.main.transform.localEulerAngles = Vector3.zero;
        FindObjectOfType<SlotScalerItem>().ControlComponent(true);
		VidaAtual = VidaCheia;
		FomeAtual = FomeCheia;
		SedeAtual = SedeCheia;
		EstaminaAtual = EstaminaCheia;
		PainelMorto.SetActive(false);
        Jogador.transform.GetComponent<RagDoll>().RagDollFunc(false, true);

        int Num = Random.RandomRange(0, Menu.PartJogo.Spawns.Length);
        Jogador.transform.position = Menu.PartJogo.Spawns[Num].position;
        Jogador.transform.rotation = Menu.PartJogo.Spawns[Num].rotation;

        Jogador.transform.GetComponent<Movimentacao>().animationController.PlayAnimation(AnimationStates.IDLE);
		Jogador.transform.GetComponent<Animator>().SetLayerWeight(1, 0);

        GC.RemGun();
		if (GunsControl.GunsMode)
			StartCoroutine(GC.AddGun(0));
	}
	public void Hit(float Dano, string Name = "", int ViewID = -1)
    {
		
		VidaAtual -= Dano;
		int Ran = Random.Range(0, HitSound.Length);

		if (!HitSound[Ran].isPlaying && !PainelMorto.active)
			HitSound[Ran].Play();

		// ====== MOrreu =========
		if (VidaAtual <= 0)
		{
			VidaAtual = 0;
			if (!Morreu)
			{
				Morreu = true;
				if (GC.IsScoped)
					StartCoroutine(GC.GunSelected.GetComponent<Gun>().OnScope(false));

                IkMove IK = MouseLook.player.GetComponent<IkMove>();
                IK.RightHand = null;
                IK.LeftHand = null;

				GC.Reticle.gameObject.SetActive(false);
				FindObjectOfType<SlotScalerItem>().ControlComponent(false, true);
                StartCoroutine(Efeito.Efeito2(PainelMorto, /*MaxAlpha: 0.995f*/MaxAlpha:0.1f, Morte: true));
				FindObjectOfType<Movimentacao>().FootStep.Stop();

				if (PhotonNetwork.IsConnected)
				{
                    servidor.Server.RPC("Morte", RpcTarget.Others, Jogador.GetComponent<PhotonView>().ViewID, true);
					if (Name != "")
						ChatMsm.Server.RPC("NewMensage", RpcTarget.All, $"{Name} Matou {PhotonNetwork.LocalPlayer.NickName}", true, Name);

					if (ViewID != -1)
					{
						Transform Alvo = PhotonView.Find(ViewID).transform.Find("Alvo");
						if (Alvo.gameObject != Jogador)
							FindObjectOfType<CameraZoom>().Alvo = Alvo;
					}
					else
						PainelMorto.SetActive(true);
                }
				else
                    PainelMorto.SetActive(true);
                Jogador.transform.GetComponent<RagDoll>().RagDollFunc(true, true);
			}
		}
	}
	public void HideStatus(bool Active)
    {
		BarraVida.transform.parent.gameObject.SetActive(Active);
		BarraSede.transform.parent.gameObject.SetActive(Active);
		BarraFome.transform.parent.gameObject.SetActive(Active);
		BarraEstamina.transform.parent.gameObject.SetActive(Active);
	}
}