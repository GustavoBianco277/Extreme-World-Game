using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleShoting : MonoBehaviour
{
    [Header("Gun Configuration")]
    private bool Shoting, Recharge, Send;
    private float WaitToFireRate;
    public bool Active, GunActive, CanShoting;
    [SerializeField] private int Damage = 200;
    [SerializeField] private float Range = 500;
    [SerializeField] private float FireRate = 0.1f;
    [SerializeField] private float TimeToStart = 0.3f;

    [Header("Ammo")]
    [SerializeField] private int Ammo = 1000;
    [SerializeField] private int AmmoInPant = 100;
    [SerializeField] private int MaxAmmoInPaint = 100;
    [SerializeField] private float TimeToReload = 4;

    [Header("Camera Configuration")]
    [SerializeField] private Vector2 MinMaxCamY = new Vector2(-40, 10);
    [SerializeField] private Vector2 MinMaxCamX = new Vector2(-30, 40);
    private Transform LastParentCam;
    private Vector3 LastPosCam;
    private Vector2 LastCamX;
    private Vector2 LastCamY;

    [Header("Crosshair properties")]
    [SerializeField] private float MaxSize = 70;
    [SerializeField] private float StableSize = 50;
    [SerializeField] private float Speed = 3;
    private float CurrentSize;
    private RectTransform Reticle;

    [Header("Components")]
    private MouseLook Mouse;
    private GunsControl GC;
    private Camera Cam;
    [SerializeField] private GameObject ImpactGround;
    [SerializeField] private ParticleSystem[] AmmoParticle;
    [SerializeField] private AudioSource GunSound, ReloadSound, BulletSound, EndGunShot;
    [SerializeField] private PhotonView Net;

    void Start()
    {
        Cam = Camera.main;
        Mouse = FindObjectOfType<MouseLook>();
        GC = FindObjectOfType<GunsControl>();
        Reticle = GC.Reticle;
        GC.Ammo.text = $"{AmmoInPant}/{Ammo}";
    }

    void Update()
    {
        if (MouseLook.Veiculo && Input.GetKeyDown("c") && !ChatMsm.Opened && !MenuPause.MenuOpen && GunsControl.GunsMode 
            || !GunsControl.GunsMode && Active)
        {
            CamMachineGun();
        }

        if (Active)
        {
            Reticle.gameObject.SetActive(true);
            CrossHair();
            if (Input.GetMouseButton(0) && AmmoInPant > 0 && !Recharge && MouseLook.MouseEnable && !ChatMsm.Opened)
            {
                StartShoting(true);
                if (PhotonNetwork.IsConnected && !Send)
                {
                    Net.RPC("StartMachineGun", RpcTarget.Others, Net.ViewID, true);
                    Send = true;
                }
            }

            else if (Shoting)
            {
                StartShoting(false);
                if (PhotonNetwork.IsConnected && Send)
                {
                    Net.RPC("StartMachineGun", RpcTarget.Others, Net.ViewID, false);
                    Send = false;
                }
            }

            if (Shoting && WaitToFireRate * Time.deltaTime > FireRate && CanShoting)
            {
                Shot();
            }
        }

        if (AmmoInPant == 0 && Ammo > 0 && !Recharge || Input.GetKeyDown("r") && AmmoInPant != MaxAmmoInPaint && Ammo > 0 && !Recharge )
        {
            if (!ChatMsm.Opened && MouseLook.MouseEnable)
                StartCoroutine(Reload());

        }

        if (GunActive)
        {
            StartShoting(true);
            if (CanShoting && !AmmoParticle[0].isPlaying)
            {
                AmmoParticle[0].Play();
                AmmoParticle[1].Play();
            }
        }
    }

    private void Shot()
    {
        WaitToFireRate = 0;
        AmmoInPant--;
        GC.Ammo.text = $"{AmmoInPant}/{Ammo}";

        if (!AmmoParticle[0].isPlaying)
        {
            AmmoParticle[0].Play();
            AmmoParticle[1].Play();
        }

        RaycastHit Hit;
        Vector2 Point = Random.insideUnitCircle * ((CurrentSize - 30) / 2);
        Ray Center = Cam.ScreenPointToRay(new Vector3(Point.x + Screen.width /2, Point.y + Screen.height/2, 0));

        if (Physics.Raycast(Center, out Hit, Range))
        {
            if (PhotonNetwork.IsConnected)
            {
                if (Hit.transform.root.GetComponent<Impact_Material>())
                {
                    string Name = Hit.transform.root.GetComponent<Impact_Material>().Material.name;
                    PhotonNetwork.Instantiate($"Materiais/Particles/{Name}", Hit.point, Quaternion.LookRotation(Hit.normal));
                }
                else
                    PhotonNetwork.Instantiate("Materiais/Prefarbs/Guns/Fire", Hit.point, Quaternion.LookRotation(Hit.normal));
            }

            else
            {
                if (Hit.transform.root.GetComponent<Impact_Material>())
                    Instantiate(Hit.transform.root.GetComponent<Impact_Material>().Material, Hit.point, Quaternion.LookRotation(Hit.normal));
                else
                    Instantiate(ImpactGround, Hit.point, Quaternion.LookRotation(Hit.normal));
            }

            if (Hit.collider.transform.GetComponent<Part_Life>())
            {
                if (PhotonNetwork.IsConnected)
                    Hit.collider.transform.GetComponent<Part_Life>().Death(Damage, Hit.transform.GetComponent<PhotonView>().ViewID);

                else
                    Hit.collider.transform.GetComponent<Part_Life>().Death(Damage);
            }

            else if (Hit.collider.transform.GetComponent<Rigidbody>())
            {
                Hit.collider.transform.GetComponent<Rigidbody>().AddForce(Quaternion.LookRotation(Hit.normal).ToEulerAngles() * 1000 * Time.deltaTime, ForceMode.Force);
            }

            if (Hit.transform.root.tag == "Player" && PhotonNetwork.IsConnected)
            {
                servidor.Server.RPC("GunDamage", Hit.transform.root.GetComponent<PhotonView>().Owner, Damage, PhotonNetwork.LocalPlayer.NickName, Hit.collider.transform.name, MouseLook.player.GetComponent<PhotonView>().ViewID);
                //BulletSound.Play();
            }

            else if (Hit.collider.transform.GetComponent<Ia_Animal>())
                Hit.collider.transform.GetComponent<Ia_Animal>().Hit(Damage, Net);
        }
    }

    private IEnumerator Reload()
    {
        Recharge = true;
        CanShoting= false;
        ReloadSound.Play();
        yield return new WaitForSeconds(TimeToReload);

        int Need = MaxAmmoInPaint - AmmoInPant;
        if (Ammo > Need)
        {
            Ammo -= Need;
            AmmoInPant += Need;
        }
        else
        {
            AmmoInPant += Ammo;
            Ammo = 0;
        }
        GC.Ammo.text = $"{AmmoInPant}/{Ammo}";
        Recharge = false;
        ReloadSound.Stop();
    }

    private IEnumerator Timer()
    {
        CanShoting = false;
        yield return new WaitForSeconds(TimeToStart);
        CanShoting= true;
    }

    public void ExitCam()
    {
        Cam.transform.SetParent(LastParentCam);
        Cam.transform.localPosition = LastPosCam;
        Mouse.minMaxX = LastCamX;
        Mouse.minMaxY = LastCamY;
        Reticle.gameObject.SetActive(false);
        print(LastParentCam);
    }

    public void CamMachineGun()
    {
        Active = !Active;

        if (Active)
        {
            LastPosCam = Cam.transform.localPosition;
            LastParentCam = Cam.transform.parent;
            Cam.transform.SetParent(this.transform);
            Cam.transform.localPosition = Vector3.zero;
            LastCamX = Mouse.minMaxX;
            LastCamY = Mouse.minMaxY;
            print(LastPosCam);
            Mouse.minMaxY = MinMaxCamY;
            Mouse.minMaxX = MinMaxCamX;
        }
        else
            ExitCam();
    }

    public void StartShoting(bool Active)
    {
        Shoting = Active;
        if (Shoting)
        {
            WaitToFireRate++;
            if (!CanShoting)
                StartCoroutine(Timer());
            
            if (!GunSound.isPlaying)
                GunSound.Play();
        }
        else
        {
            if (GunSound.isPlaying)
                GunSound.Stop();
            
            EndGunShot.Play();
            CanShoting = false;
        }

    }

    private void CrossHair()
    {
        if (Shoting)
            CurrentSize = Mathf.Lerp(CurrentSize, MaxSize, Time.deltaTime * Speed);
        else
            CurrentSize = Mathf.Lerp(CurrentSize, StableSize, Time.deltaTime * Speed * 2);

        Reticle.sizeDelta = new Vector2(CurrentSize, CurrentSize);
    }
}
