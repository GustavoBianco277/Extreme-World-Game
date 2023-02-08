using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Gun : MonoBehaviour
{
    [Header("Gun Configuraçoes")]
    public int damage;
    public float renge;
    public float ScopedFOV = 15;
    private float NormalFOV = 70;
    private float ZoomFOV;
    private MouseLook mouse;
    private Camera Camera;
    private Animator Animator;
    private GunsControl GC;
    private PhotonView Net;
    private Movimentacao Player;

    public float fireRate;
    public float WaitToFireRate;
    public ParticleSystem ShotParticle;
    public GameObject ImpactGround;
    public bool Automatic = true, Rocke;
    public GameObject RocketPrefab;
    private bool hond = false;
    public bool Scoped;
    [HideInInspector] public bool Offline;
    private bool Shot;
    [Space]
    [Header("Ammo")]
    public int maxAmmoInPaint;
    public int ammoInPaint;
    public int ammo;
    public int timeToRecharge;
    private bool rechargeb = false, ShootGun = true;
    [Space]
    [Header("Audio")]
    public AudioSource ReloadSound;
    public AudioSource ShotSound;
    public AudioSource BulletSound;
    [Space]
    [Header("Canvas")]
    private TMPro.TextMeshProUGUI ammotxt;
    private GameObject ScopeOverlay;
    public bool IsScoped;
    [Space]
    [Header("Crosshair properties")]
    private RectTransform Reticle;
    private float CurrentSize;
    public float RestingSize;
    public float RunJumpSize;
    public float Speed = 1;
    public float MaxSize;

    void Start()
    {
        ShotSound.transform.SetParent(this.transform.parent);
        
        mouse = FindObjectOfType<MouseLook>();
        Animator = transform.GetComponentInParent<Animator>();
        Camera = Camera.main;
        GC = FindObjectOfType<GunsControl>();
        Reticle = GC.Reticle;
        ammotxt = GC.Ammo;
        ScopeOverlay = GC.ScopeOverlay;
        NormalFOV = Camera.fieldOfView;
        ammotxt.text = ammoInPaint + "/" + ammo;
        ZoomFOV = NormalFOV / ScopedFOV;
    }
    void Update()
    {
        if (Offline)
        {
            if (Net == null || Player == null)
            {
                Net = MouseLook.player.GetComponent<PhotonView>();
                Player = MouseLook.player.GetComponent<Movimentacao>();
            }

            CrossHair();
            // Automatic Gun
            if (Automatic)
            {
                if (Input.GetMouseButton(0) && MouseLook.MouseEnable && !ChatMsm.Opened)
                    hond = true;

                else
                    hond = false;

                if (hond)
                {
                    if (IsScoped)
                    {
                        Animator.SetBool("ScopedShot", true);
                        Animator.SetBool("Shot", false);
                        //
                        GC.Target.GetComponent<Animator>().SetBool("ScopedShot", true);
                        GC.Target.GetComponent<Animator>().SetBool("Shot", false);
                    }
                    else
                    {
                        Animator.SetBool("Shot", true);
                        Animator.SetBool("ScopedShot", false);
                        //
                        GC.Target.GetComponent<Animator>().SetBool("Shot", true);
                        GC.Target.GetComponent<Animator>().SetBool("ScopedShot", false);
                    }
                    WaitToFireRate += 1;
                }
                if (hond == false || ammoInPaint == 0)
                {
                    Animator.SetBool("Shot", false);
                    Animator.SetBool("ScopedShot", false);
                    //
                    GC.Target.GetComponent<Animator>().SetBool("Shot", false);
                    GC.Target.GetComponent<Animator>().SetBool("ScopedShot", false);
                }

                if (WaitToFireRate * Time.deltaTime > fireRate && ammoInPaint >= 1 && !rechargeb)
                {
                    Shoot();
                }
            }
            // Manual Gun
            else
            {
                if (Input.GetMouseButtonDown(0) && ammoInPaint > 0 && !rechargeb && ShootGun && MouseLook.MouseEnable && !ChatMsm.Opened)
                {
                    Shoot();
                    StartCoroutine(ShotAnimation());
                }
            }

            if (Input.GetMouseButtonDown(1) && !rechargeb)
            {
                IsScoped = !IsScoped;
                StartCoroutine(OnScope(IsScoped));
            }

            // Rechargeb
            if (Input.GetKeyDown("r") && ammoInPaint != maxAmmoInPaint && ammo != 0 && !rechargeb && MouseLook.MouseEnable && !ChatMsm.Opened 
                || ammoInPaint == 0 && ammo != 0 && !rechargeb && MouseLook.MouseEnable && !ChatMsm.Opened && !Status.Morreu)
            {
                StartCoroutine(Reload());

                if (IsScoped)
                {
                    IsScoped = !IsScoped;
                    StartCoroutine(OnScope(IsScoped));
                }
            }
            if (Scoped && IsScoped && Input.mouseScrollDelta.y != 0)
            {
                float Scrol = Input.mouseScrollDelta.y;
                if (Scrol > 0)
                {
                    Camera.fieldOfView = ScopedFOV /3;
                    SensitivyFOV(3);
                }
                else
                {
                    Camera.fieldOfView = ScopedFOV;
                    SensitivyFOV();
                }
            }
        }
    }
    
    void Shoot()
    {
        if (!Shot)
            StartCoroutine(CrossHairTimeShot());
        
        WaitToFireRate = 0;
        ammoInPaint -= 1;

        /*if (Rocket)
        {
            if (PhotonNetwork.IsConnected)
                servidor.Server.RPC("ActiveRocket", RpcTarget.Others, Net.ViewID);
            
            transform.GetChild(0).GetComponent<Rocket>().StartRocket();
        }*/

        
        //{
            if (PhotonNetwork.IsConnected)
                servidor.Server.RPC("Sound", RpcTarget.Others, Net.ViewID, "ShotSound", true, Vector3.zero);

            ShotSound.Play();

            if (PhotonNetwork.IsConnected)
                servidor.Server.RPC("StartParticle", RpcTarget.Others, Net.ViewID);

            if (!IsScoped)
                StartParticle();

            RaycastHit hit;
            Ray CentroTela;

            if (IsScoped)
                CentroTela = Camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            else
            {
                Vector2 Point = Random.insideUnitCircle * ((CurrentSize - 30) / 2);
                CentroTela = Camera.ScreenPointToRay(new Vector3(Point.x + Screen.width / 2, Point.y + Screen.height / 2, 0));
            }

            if (Physics.Raycast(CentroTela, out hit, renge))
            {
                //ObjectDestroy ob = hit.transform.GetComponent<ObjectDestroy>();
                //if (ob != null)
                //    ob.takeDamage(damage);
                if (PhotonNetwork.IsConnected)
                {
                    if (hit.transform.root.GetComponent<Impact_Material>())
                    {
                        string Name = hit.transform.root.GetComponent<Impact_Material>().Material.name;
                        PhotonNetwork.Instantiate($"Materiais/Particles/{Name}", hit.point, Quaternion.LookRotation(hit.normal));
                    }
                    else
                        PhotonNetwork.Instantiate("Materiais/Prefarbs/Guns/Fire", hit.point, Quaternion.LookRotation(hit.normal));
                    servidor.Server.RPC("Sound", RpcTarget.Others, Net.ViewID, "BulletSound", true, hit.point);
                }

                else
                {
                    if (hit.transform.root.GetComponent<Impact_Material>())
                        Instantiate(hit.transform.root.GetComponent<Impact_Material>().Material, hit.point, Quaternion.LookRotation(hit.normal));
                    else
                        Instantiate(ImpactGround, hit.point, Quaternion.LookRotation(hit.normal));
                    BulletSound.transform.position = hit.point;
                    BulletSound.Play();
                }

                if (hit.collider.transform.GetComponent<Part_Life>())
                {
                    if (PhotonNetwork.IsConnected)
                        hit.collider.transform.GetComponent<Part_Life>().Death(damage, hit.transform.GetComponent<PhotonView>().ViewID);

                    else
                        hit.collider.transform.GetComponent<Part_Life>().Death(damage);
                }

                else if (hit.collider.transform.GetComponent<Rigidbody>())
                {
                    hit.collider.transform.GetComponent<Rigidbody>().AddForce(Quaternion.LookRotation(hit.normal).ToEulerAngles() * 1000 * Time.deltaTime, ForceMode.Force);
                }

                if (hit.transform.root.tag == "Player" && PhotonNetwork.IsConnected)
                {
                    servidor.Server.RPC("GunDamage", hit.transform.root.GetComponent<PhotonView>().Owner, damage, PhotonNetwork.LocalPlayer.NickName, hit.collider.transform.name, MouseLook.player.GetComponent<PhotonView>().ViewID);
                    //BulletSound.Play();
                }

                else if (hit.collider.transform.GetComponent<Ia_Animal>())
                    hit.collider.transform.GetComponent<Ia_Animal>().Hit(damage, Net);

                //Destroy(Instantiate(bulledHole, hit.point, Quaternion.LookRotation(hit.normal)), 9999.0f);
            //}
        }
        ammotxt.text = ammoInPaint + "/" + ammo;
        StartCoroutine(FireTime());
    }

    private IEnumerator FireTime()
    {
        ShootGun = false;
        yield return new WaitForSeconds(fireRate);
        ShootGun = true;
    }

    public IEnumerator OnScope(bool Active)
    {
        if (GC != null)
            GC.IsScoped = Active;


        Animator.SetBool("Scoped", Active);
        GC.Target.GetComponent<Animator>().SetBool("Scoped", Active);

        if (Active)
            yield return new WaitForSeconds(.15f);

        FindObjectOfType<DerrubarArvores>().enabled = !Active;
        //FindObjectOfType<Status>().HideStatus(!Active);

        if (GunsControl.GunsMode)
            Reticle.gameObject.SetActive(!Active);

        if (Scoped)
        {
            ScopeOverlay.SetActive(Active);
            //GunCamera.SetActive(!Active);
            GetComponent<MeshRenderer>().enabled = !Active;

            GC.ArmLeft.gameObject.SetActive(!Active);
            GC.ArmRight.gameObject.SetActive(!Active);

            if (Active)
            {
                Camera.fieldOfView = ScopedFOV;
                SensitivyFOV();
            }
            else
            {
                Camera.fieldOfView = NormalFOV;
                mouse.sensitivityX = PlayerPrefs.GetFloat("sensibilidade");
                mouse.sensitivityY = PlayerPrefs.GetFloat("sensibilidade");
            }
        }
    }
    public void SensitivyFOV(float Mult = 1)
    {
        mouse.sensitivityX = PlayerPrefs.GetFloat("sensibilidade") / (ZoomFOV * Mult);
        mouse.sensitivityY = PlayerPrefs.GetFloat("sensibilidade") / (ZoomFOV * Mult);
    }

    private IEnumerator ShotAnimation()
    {
        Animator.SetBool("ManualShot", true);
        GC.Target.GetComponent<Animator>().SetBool("ManualShot", true);

        yield return new WaitForSeconds(0.5f);

        Animator.SetBool("ManualShot", false);
        GC.Target.GetComponent<Animator>().SetBool("ManualShot", false);
    }

    private IEnumerator Reload()
    {
        if (PhotonNetwork.IsConnected)
            servidor.Server.RPC("Sound", RpcTarget.Others, Net.ViewID, "ReloadSound", true, Vector3.zero);

        ReloadSound.Play();

        rechargeb = GC.Reload = true;
        Animator.SetBool("Reload", true);
        GC.Target.GetComponent<Animator>().SetBool("Reload", true);

        yield return new WaitForSeconds(timeToRecharge);

        int Need = maxAmmoInPaint - ammoInPaint;
        if (ammo >= Need)
        {
            ammo -= Need;
            ammoInPaint += Need;
        }
        else
        {
            ammoInPaint += ammo;
            ammo = 0;
        }
        WaitToFireRate = 0;
        Animator.SetBool("Reload", false);
        GC.Target.GetComponent<Animator>().SetBool("Reload", false);
        ammotxt.text = ammoInPaint + "/" + ammo;

        /*if (Rocket)
        {
            if (PhotonNetwork.IsConnected)
                servidor.Server.RPC("AddRemRocket", RpcTarget.Others, MouseLook.player.GetComponent<PhotonView>().ViewID);
            AddRocket();
        }*/

        yield return new WaitForSeconds(0.4f);
        rechargeb = GC.Reload = false;
    }
    
    
    public void StartParticle()
    {
        ShotParticle.Play();
    }

    private IEnumerator CrossHairTimeShot()
    {
        Shot = true;
        yield return new WaitForSeconds(2.0f / Speed);
        Shot = false;
    }

    public void CrossHair()
    {
        if (RunOrJump)
            CurrentSize = Mathf.Lerp(CurrentSize, RunJumpSize, Time.deltaTime * Speed);

        else if (Ismoving)
            CurrentSize = Mathf.Lerp(CurrentSize, MaxSize, Time.deltaTime * Speed);

        else
            CurrentSize = Mathf.Lerp(CurrentSize, RestingSize, Time.deltaTime * Speed);

        Reticle.sizeDelta = new Vector2(CurrentSize, CurrentSize);
    }

    bool Ismoving
    {
        get
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Shot)
                return true;
            else 
                return false;
        }
    }

    bool RunOrJump
    {
        get
        {
            if (Player.Correndo || Player.Pulo)
                return true;
            else
                return false;
        }
    }

    private void OnDestroy()
    {
        Animator.SetBool("Shot", false);
        Animator.SetBool("ScopedShot", false);   
        ShotSound.GetComponent<DestroyAfterTimeParticle>().enabled = true;
    }
}
