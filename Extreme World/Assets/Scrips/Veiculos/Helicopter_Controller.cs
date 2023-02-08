using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class Parts_Helicopter
{
    public GameObject Audio;
    public GameObject[] PartsGameObject;
}

public class Helicopter_Controller : MonoBehaviour
{
    private Rigidbody Helicopter;
    private ConstantForce Bory;
    [Header("Components")]
    public Transform Alavanca, acelerador;
    public HeliRotorController HeliceTop;
    public HeliRotorController HeliceTail;
    public AudioSource HelicopterSound, ExplosaoSound;
    public ParticleSystem Explosao;
    public VehicleShoting shoting;
    public Parts_Helicopter Parts = new Parts_Helicopter();
    public int ForceExplosao;
    [Header("Configuration")]
    public float VelocityUpDown = 3.7f;
    public float ForceForward = 2000;
    public float ForceTilt = 500;
    public float ForceTurn = 1000;
    public float ForceUpDown = 0;
    public float RotationMotor = 0;
    public float ForwardTilt = 20;
    public float TurnTilt = 30;
    public float turnTiltForcePercent = 0.93f;
    public float velocity;

    public bool NoChao, Controller;
    [Header("Keys")]
    public KeyCode Forward = KeyCode.W;
    public KeyCode Back = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Up = KeyCode.LeftShift;
    public KeyCode Down = KeyCode.Space;
    public KeyCode TurnLeft = KeyCode.Q;
    public KeyCode TurnRight = KeyCode.E;
    //      Privates
    public Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    public Vector3 ForceF, ForceT;
    private bool Turning, Caindo, Cai, Mandar, Rodopiar;
    private float lastForce, force;
    private PhotonView View;

    [HideInInspector] public Seat SeatPlayer;

    void Start()
    {
        ForceUpDown = 0;
        Helicopter = GetComponent<Rigidbody>();
        Bory = GetComponent<ConstantForce>();
        ForceF = new Vector3(0, (10 + (0.061f * (ForceForward / 10)) * ForwardTilt * turnTiltForcePercent) * Helicopter.mass / 10, 0);
        ForceT = new Vector3(0, (10 + (0.061f * (ForceTilt / 10)) * ForwardTilt * turnTiltForcePercent) * Helicopter.mass / 5.65f, 0);
        NoChao = true;
        Bory.force = Vector3.zero;
        View = GetComponent<PhotonView>();
    }
    void Update()
    {
        if (!Caindo)
            velocity = Helicopter.velocity.x;
        if (Rodopiar)
            Helicopter.AddTorque(0, ForceTurn * 5 * Helicopter.mass * Time.deltaTime, 0);

        // sai do chao
        if (ForceUpDown > 10 * Helicopter.mass && Controller)
            NoChao = false;

        // start o som
        if (ForceUpDown > 0 && !HelicopterSound.isPlaying)
            HelicopterSound.Play();

        // storp o som
        else if (RotationMotor <= 0 && HelicopterSound.isPlaying)
            HelicopterSound.Stop();

        if (!NoChao && Controller)
            RotationMotor = Mathf.Clamp(RotationMotor, 35, 50);

        else if (NoChao && ForceUpDown <= 0 & Controller)
        {
            if (RotationMotor < 0.01f)
                RotationMotor = 0;
            if (RotationMotor > 0)
                RotationMotor = Mathf.Lerp(RotationMotor, 0, Time.deltaTime);
        }

        HeliceTop.RotarSpeed = RotationMotor * 80;
        HeliceTail.RotarSpeed = RotationMotor * 40;
        HelicopterSound.pitch = Mathf.Clamp(RotationMotor / 40, 0, 1.2f);

        if (Controller && PhotonNetwork.IsConnected && RotationMotor != lastForce)
        {
            lastForce = RotationMotor;
            View.RPC("ForceHelicopter", RpcTarget.Others, RotationMotor, ForceUpDown);
        }
        if (!Turning)
            LiftProcess();

        if (PhotonNetwork.IsConnected && View.IsMine || !PhotonNetwork.IsConnected)
            TurnMoveProcess();

        else if (!Turning && PhotonNetwork.IsConnected && MouseLook.Veiculo)
            Estabelizador();

        // VerificaParts
        //VerificaParts();

        if (Caindo)
        {
            if (Helicopter.velocity.y < velocity)
                velocity = Helicopter.velocity.y;
        }
    }

    private void LiftProcess()
    {
        if (Input.GetKeyUp(Up) || Input.GetKeyUp(Down) && Controller)
        {
            if (!NoChao)
            {
                Bory.force = new Vector3(0, 10 * Helicopter.mass, 0);
                ForceUpDown = 10 * Helicopter.mass;
                print("aqui");
            }
        }

        else if (Input.GetKey(Up) && Controller)
        {
            if (ForceUpDown <= (10 + VelocityUpDown) * Helicopter.mass)
            {
                if (ForceUpDown < 10 * Helicopter.mass && !NoChao)
                {
                    ForceUpDown += Time.deltaTime * Helicopter.mass * 4;
                    RotationMotor += Time.deltaTime * VelocityUpDown;
                }
                else
                {
                    ForceUpDown += Time.deltaTime * Helicopter.mass;
                    RotationMotor += Time.deltaTime * VelocityUpDown;
                }
            }

            ForceUpDown = Mathf.Clamp(ForceUpDown, 0, (10 + VelocityUpDown) * Helicopter.mass);
            Bory.force = new Vector3(0, ForceUpDown, 0);
        }

        else if (Input.GetKey(Down) && Controller)
        {
            if (ForceUpDown >= (10 - VelocityUpDown) * Helicopter.mass || NoChao)
            {

                if (ForceUpDown > 10 * Helicopter.mass)
                {
                    ForceUpDown -= Time.deltaTime * Helicopter.mass * 4;
                    RotationMotor -= Time.deltaTime * VelocityUpDown;
                }
                else if (ForceUpDown > 0)
                {
                    ForceUpDown -= Time.deltaTime * Helicopter.mass;
                    RotationMotor -= Time.deltaTime * VelocityUpDown;
                }
            }

            if (!NoChao)
                ForceUpDown = Mathf.Clamp(ForceUpDown, (10 - VelocityUpDown) * Helicopter.mass, (10 + VelocityUpDown) * Helicopter.mass);
            Bory.force = new Vector3(0, ForceUpDown, 0);
        }
    }

    private void TurnMoveProcess()
    {
        
        // Lerping
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTilt, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTilt, Time.deltaTime);

        transform.localRotation = Quaternion.Euler(hTilt.y, transform.localEulerAngles.y, -hTilt.x);
        Alavanca.localRotation = Quaternion.Euler(hMove.y *5 -5, Alavanca.localEulerAngles.y, -hMove.x *5);

        
        // Turn W S D A

        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (hMove.y > 0)
            tempY = -Time.fixedDeltaTime;
        else if (hMove.y < 0)
            tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > 0)
            tempX = -Time.fixedDeltaTime;
        else if (hMove.x < 0)
            tempX = Time.fixedDeltaTime;

        if (Caindo) 
        {
            Turning = true;
            tempY = Time.fixedDeltaTime;
        }

        float x;
        if (hMove.y < 0)
            x = 0;
        else
            x = hMove.y;

        // Turn Q & E

        if (Input.GetKey(TurnRight) && !NoChao && Controller)
            Helicopter.AddTorque(0, ForceTurn * Helicopter.mass * Time.deltaTime, 0);

        else if (Input.GetKey(TurnLeft) && !NoChao && Controller)
            Helicopter.AddTorque(0, -ForceTurn * Helicopter.mass * Time.deltaTime, 0);

        // Inputs
        if (Input.GetKeyUp(Forward) || Input.GetKeyUp(Back) || Input.GetKeyUp(Left) || Input.GetKeyUp(Right))
        {
            if (Controller)
            {
                Bory.force = new Vector3(0, 10 * Helicopter.mass, 0);
                Turning = false;
            }
        }

        else if (Input.GetKey(Forward) && !NoChao && Controller)
        {
            Turning = true;
            tempY = Time.fixedDeltaTime;
            Helicopter.AddForce(transform.forward * ForceForward * Helicopter.mass * x * Time.deltaTime, ForceMode.Force);

            if (Input.GetKey(Up))
                Bory.force = Vector3.Lerp(Bory.force, new Vector3(0, ForceF.y + VelocityUpDown * Helicopter.mass, 0), Time.deltaTime);

            else if (Input.GetKey(Down))
                Bory.force = Vector3.Lerp(Bory.force, new Vector3(0, ForceF.y - VelocityUpDown * Helicopter.mass, 0), Time.deltaTime);

            else
                Bory.force = Vector3.Lerp(Bory.force, ForceF, Time.deltaTime * x);
        }
        else if (Input.GetKey(Back) && !NoChao && Controller)
        {
            Turning = true;
            tempY = -Time.fixedDeltaTime;
            Helicopter.AddForce(transform.forward * -ForceTilt * Helicopter.mass * Mathf.Abs(hMove.y) * Time.deltaTime, ForceMode.Force);

            if (Input.GetKey(Up))
                Bory.force = Vector3.Lerp(Bory.force, new Vector3(0, ForceT.y + VelocityUpDown * Helicopter.mass, 0), Time.deltaTime);

            else if (Input.GetKey(Down))
                Bory.force = Vector3.Lerp(Bory.force, new Vector3(0, ForceT.y - VelocityUpDown * Helicopter.mass, 0), Time.deltaTime);

            else
                Bory.force = Vector3.Lerp(Bory.force, ForceT, Time.deltaTime * Mathf.Abs(hMove.y));
        }
        else if (Input.GetKey(Left) && !NoChao && Controller)
        {
            Turning = true;
            tempX = -Time.fixedDeltaTime;
            Helicopter.AddForce(transform.right * -ForceTilt * Helicopter.mass * Mathf.Abs(hMove.x) * Time.deltaTime, ForceMode.Force);

            if (Input.GetKey(Up))
                Bory.force = Vector3.Lerp(Bory.force, new Vector3(0, ForceT.y + VelocityUpDown * Helicopter.mass, 0), Time.deltaTime);

            else if (Input.GetKey(Down))
                Bory.force = Vector3.Lerp(Bory.force, new Vector3(0, ForceT.y - VelocityUpDown * Helicopter.mass, 0), Time.deltaTime);

            else
                Bory.force = Vector3.Lerp(Bory.force, ForceT, Time.deltaTime * Mathf.Abs(hMove.y));
        }
        else if (Input.GetKey(Right) && !NoChao && Controller)
        {
            Turning = true;
            tempX = Time.fixedDeltaTime;
            Helicopter.AddForce(transform.right * ForceTilt * Helicopter.mass * Mathf.Abs(hMove.x) * Time.deltaTime, ForceMode.Force);

            if (Input.GetKey(Up))
                Bory.force = Vector3.Lerp(Bory.force, new Vector3(0, ForceT.y + VelocityUpDown * Helicopter.mass, 0), Time.deltaTime);

            else if (Input.GetKey(Down))
                Bory.force = Vector3.Lerp(Bory.force, new Vector3(0, ForceT.y - VelocityUpDown * Helicopter.mass, 0), Time.deltaTime);

            else
                Bory.force = Vector3.Lerp(Bory.force, ForceT, Time.deltaTime) * Mathf.Abs(hMove.y);
        }
            
        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);

        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);

        if (Turning && PhotonNetwork.IsConnected && View.IsMine)
        {
            View.RPC("UpdateCockpit", RpcTarget.Others, View.ViewID, hMove, true);
            Mandar = true;
        }

        else if (Mandar)
        {
            View.RPC("UpdateCockpit", RpcTarget.Others, View.ViewID, hMove, false);
            Mandar = false;
        }
    }

    public void UpdateParts(string Name, int Dano)
    {
        foreach( GameObject Part in Parts.PartsGameObject)
        {
            if (Part.name == Name)
                Part.GetComponent<Part_Life>().Death(Dano);
        }
    }
    public void ExplosaoVoid()
    {
        print("destruir");
        if (PhotonNetwork.IsConnected && Controller)
        {
            View.RPC("Explosion", RpcTarget.Others, View.ViewID);
        }
        Caindo = false;
        Cai = true;
        HelicopterSound.Stop();
        ExplosaoSound.Play();
        Instantiate(Explosao, transform.position, Quaternion.identity);
        //
        foreach (GameObject P in Parts.PartsGameObject) 
        {
            Rigidbody Rb;
            if (P.GetComponent<Rigidbody>())
                Rb = P.GetComponent<Rigidbody>();
            else
                Rb = P.AddComponent<Rigidbody>();

            P.transform.SetParent(transform.parent);
            Rb.AddExplosionForce(ForceExplosao, transform.position, 10, 1f, ForceMode.Impulse);
            Destroy(P, 10);
        }
        Parts.Audio.transform.SetParent(transform.parent);

        if (MouseLook.player.GetComponent<Movimentacao>().VehicleUsing == this.transform)
        {
            Status S = FindObjectOfType<Status>();
            S.Hit(S.VidaAtual);
        }

        if (SeatPlayer != null)

            SeatPlayer.transform.parent.GetComponent<OpenVehicle>().ExitCam();

        if (PhotonNetwork.IsConnected && View.IsMine)
            PhotonNetwork.Destroy(View);

        else if (!PhotonNetwork.IsConnected)
            Destroy(transform.gameObject);
    }

    public void VerificaParts(bool Cair = false, bool DanoCritico = false)
    {
        if (Cair && !NoChao)
        {
            Helicopter.useGravity= true;
            Bory.enabled = false;
            if (!Cai)
                Caindo = true;
            Parts.PartsGameObject[0].transform.SetParent(transform.parent);
            RotationMotor = Mathf.Lerp(RotationMotor, 35, Time.deltaTime / 2);
            Helicopter.drag = 0;
            Controller = false;
            NoChao = true;
            force = Mathf.Lerp(Bory.force.y, 0, Time.deltaTime * 3);
            Bory.force = new Vector3(0, force, 0);
        }
        else if (DanoCritico && !NoChao)
        {
            print("rodopiar");
            Rodopiar = true;
        }

        /*if (Parts.PartsGameObject[0].GetComponent<Rigidbody>() && !NoChao)
        {
            if (!Cai)
                Caindo = true;
            Parts.PartsGameObject[0].transform.SetParent(transform.parent);
            RotationMotor = Mathf.Lerp(RotationMotor, 35, Time.deltaTime / 2);
            Helicopter.drag = 0;
            Controller = false;
            NoChao = true;
            force = Mathf.Lerp(Bory.force.y, 0, Time.deltaTime * 2);
            Bory.force = new Vector3(0, force, 0);
        }*/
    }

    public void UpdateCockpit(Vector2 hMove, bool Turn)
    {
        Alavanca.localRotation = Quaternion.Euler(hMove.y * 5 - 5, Alavanca.localEulerAngles.y, -hMove.x * 5);
        Turning = Turn;
    }

    public void Estabelizador()
    {
        print("estabilizando");
        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (hMove.y > 0)
            tempY = -Time.fixedDeltaTime;
        else if (hMove.y < 0)
            tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > 0)
            tempX = -Time.fixedDeltaTime;
        else if (hMove.x < 0)
            tempX = Time.fixedDeltaTime;

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);

        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);

        Alavanca.localRotation = Quaternion.Euler(hMove.y * 5 - 5, Alavanca.localEulerAngles.y, -hMove.x * 5);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].thisCollider.name == "Base")
            GetComponent<Helicopter_Controller>().NoChao = true;

        if (Caindo && collision.transform.name != "Top_Rotor" && collision.transform.name != "Tail_Rotor" && velocity < -20) 
            ExplosaoVoid();
    }
}