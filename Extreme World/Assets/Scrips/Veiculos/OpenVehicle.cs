using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OpenVehicle : MonoBehaviourPun
{
    [HideInInspector] public bool Opened;//, Sair, Entrar;
    [HideInInspector] public AudioSource SomPortas;
    public Vector3 CamPosition = new Vector3(0, 2.2f, 0.8f);
    public Vector3 LastCamPosition;
    public KeyCode EntrarKey = KeyCode.F;

    private GameObject Player;
    private MouseLook Mouse;
    private Camera cam;
    private PhotonView View;
    private GunsControl GC;

    public Seat seat;
    public VehicleShoting Shoting;
    public GameObject Status;
    public Animacoes AnimationController;
    public Transform RightHand, LeftHand, RightFoot, LeftFoot;

    void Awake()
    {
        Mouse = FindObjectOfType<MouseLook>();
    }

    void Start()
    {
        //Entrar = Sair = false;
        cam = Camera.main;
        SomPortas = GetComponent<AudioSource>();
        View = transform.parent.GetComponent<PhotonView>();
        GC = FindObjectOfType<GunsControl>();
    }

    void Update()
    {
        if (Player == null)
        {
            if (MouseLook.player != null)
            {
                Player = MouseLook.player.gameObject;
                AnimationController = Player.GetComponent<Animacoes>();
            }
            return;
        }
        
        if (Input.GetKeyDown(EntrarKey) && Opened && !ChatMsm.Opened && !MenuPause.MenuOpen)
        {
            StartCoroutine(SairVeiculo());
        }
    }
    public IEnumerator EntrarVeiculo()
    {
        Helicopter_Controller HeliControl = transform.parent.GetComponent<Helicopter_Controller>();

        if (seat.SentFull)
            print("Assento cheio !");
        else
        {
            SomPortas.Play();
            EfeitoEscurecer Effect = FindObjectOfType<EfeitoEscurecer>();

            StartCoroutine(Effect.Efeito2(Effect.PainelHelicopter, 1, Velocity: 2));
            FindObjectOfType<GunsControl>().RemGun();

            yield return new WaitForSeconds(1);

            Status.SetActive(true);
            if (seat.PilotSent)
            {
                HeliControl.Controller = true;
                transform.parent.GetComponent<OwnerTransfer>().TrocarDono();
                transform.parent.GetComponent<Rigidbody>().useGravity = true;
                transform.parent.GetComponent<ConstantForce>().enabled = true;

                AnimationController.PlayAnimation(AnimationStates.PILOTHELICOPTER);

                if (PhotonNetwork.IsConnected)
                    View.RPC("SeatHelicopter", RpcTarget.OthersBuffered, true, false, Player.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                AnimationController.PlayAnimation(AnimationStates.IDDLESIT);

                if (PhotonNetwork.IsConnected)
                    View.RPC("SeatHelicopter", RpcTarget.OthersBuffered, false, false, Player.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            Shoting.enabled = true;
            HeliControl.SeatPlayer = seat;
            Player.GetComponent<Movimentacao>().Dirigindo = true;
            Player.GetComponent<Movimentacao>().VehicleUsing = transform.parent;
            MouseLook.VehicleUsing= transform.parent;
            MouseLook.Veiculo = true;
            Player.transform.GetComponent<CapsuleCollider>().enabled = false;
            seat.Target = Player.transform;
            LastCamPosition = cam.transform.localPosition;
            Player.transform.eulerAngles = Vector3.zero;
            Player.transform.GetComponent<Rigidbody>().useGravity = false;
            GC.Reticle.gameObject.SetActive(false);

            cam.transform.SetParent(seat.transform);
            cam.transform.localScale = Vector3.one;
            cam.transform.localPosition = CamPosition;

            IkMove IK = MouseLook.player.GetComponent<IkMove>();
            IK.RightHand = RightHand;
            IK.LeftHand = LeftHand;
            IK.RightFoot = RightFoot;
            IK.LeftFoot = LeftFoot;

            Opened = true;
        }
    }
    public IEnumerator SairVeiculo()
    {
        SomPortas.Play();
        EfeitoEscurecer Effect = FindObjectOfType<EfeitoEscurecer>();
        StartCoroutine(Effect.Efeito2(Effect.PainelHelicopter, 1, Velocity: 2));

        yield return new WaitForSeconds(1);
        Status.SetActive(false);

        if (seat.SentFull)
            seat.SentFull = false;

        if (seat.PilotSent)
        {
            transform.parent.GetComponent<Helicopter_Controller>().Controller = false;
            transform.parent.GetComponent<Rigidbody>().useGravity = false;
            transform.parent.GetComponent<ConstantForce>().enabled = false;

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.RemoveBufferedRPCs(View.ViewID, "SeatHelicopter");
                View.RPC("SeatHelicopter", RpcTarget.Others, true, true, Player.GetComponent<PhotonView>().ViewID, 0);
            }
        }
        else
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.RemoveBufferedRPCs(View.ViewID, "SeatHelicopter");
                View.RPC("SeatHelicopter", RpcTarget.Others, false, true, Player.GetComponent<PhotonView>().ViewID, 0);
            }
        }

        Opened = false;
        //MouseLook.player = Player.transform;
        Shoting.enabled = false;
        Player.transform.position = transform.position;
        Player.transform.GetComponent<CapsuleCollider>().enabled = true;
        Player.GetComponent<Movimentacao>().Dirigindo = false;
        Player.GetComponent<Movimentacao>().VehicleUsing = null;
        MouseLook.VehicleUsing= null;
        transform.parent.GetComponent<Helicopter_Controller>().SeatPlayer = null;
        seat.Target = null;
        ExitCam();

        if (GunsControl.GunsMode)
            GC.Reticle.gameObject.SetActive(true);
        IkMove IK = MouseLook.player.GetComponent<IkMove>();
        IK.RightHand = null;
        IK.LeftHand = null;
        IK.RightFoot = null;
        IK.LeftFoot = null;
    }
    public void ExitCam()
    {
        if (Shoting.Active)
            Shoting.CamMachineGun();

        AnimationController.PlayAnimation(AnimationStates.IDLE);
        MouseLook.Veiculo = false;
        Player.transform.GetComponent<Rigidbody>().useGravity = true;
        Player.GetComponent<Movimentacao>().Dirigindo = false;

        cam.transform.SetParent(Mouse.transform);
        cam.transform.localEulerAngles = Vector3.zero;
        cam.transform.localScale = Vector3.one;
        cam.transform.localPosition = LastCamPosition; 
    }
}
