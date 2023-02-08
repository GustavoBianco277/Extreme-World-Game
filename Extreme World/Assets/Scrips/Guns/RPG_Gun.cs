using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG_Gun : MonoBehaviour
{
    
    // a_  Ammo Values
    [Header("Ammo")]
    [SerializeField] private int Ammo = 10;
    [SerializeField] private int AmmoInPaint = 1;
    [SerializeField] private int MaxAmmoInPaint = 1;
    [HideInInspector] public Rocket CurrentProjectile;

    [Header("Configuration")]
    private bool Recharge;
    private bool IsScoped;
    [SerializeField] private float TimeToReload = 3;
    [SerializeField] private GameObject ShotParticle;
    [SerializeField] private GameObject RocketPrefab;

    [Header("Sounds")]
    public AudioSource ShotSound;
    public AudioSource ReloadSound;
    public AudioSource BulletSound;

    // Components
    private GunsControl GC;
    private PhotonView Net;
    private Animator Animator;
    private Movimentacao Player;

    [Header("Cross Hair")]
    private RectTransform Reticle;
    private float CurrentSize;
    [SerializeField] private int RestingSize = 50;
    [SerializeField] private int RunJumpSize = 70;
    [SerializeField] private int MaxSize = 100;
    [SerializeField] private float Speed = 2;

    public bool Active;
    void Start()
    {
        ShotSound.transform.SetParent(transform.parent);
        GC = FindObjectOfType<GunsControl>();
        Net = MouseLook.player.GetComponent<PhotonView>();
        Player = MouseLook.player.GetComponent<Movimentacao>();
        Animator = transform.GetComponentInParent<Animator>();
        Reticle = GC.Reticle;
        GC.Ammo.text = $"{AmmoInPaint}/{Ammo}";
        GC.Target.GetComponent<Animator>().SetBool("IdleRPG", true);
    }

    void Update()
    {
        if (Active)
        {
            CrossHair();
            if (Input.GetMouseButton(0) && AmmoInPaint > 0 && !Recharge)
            {
                Shot();
                StartCoroutine(ShotAnimation());
            }

            else if (Input.GetMouseButtonDown(1) && !Recharge)
            {
                IsScoped = !IsScoped;
                StartCoroutine(OnScope(IsScoped));
            }

            else if (Input.GetKeyDown("r") && Ammo > 0 && AmmoInPaint < MaxAmmoInPaint || AmmoInPaint == 0 && Ammo > 0)
            {
                if (!Recharge && MouseLook.MouseEnable && !ChatMsm.Opened)
                {
                    StartCoroutine(Reload());
                    if (IsScoped)
                    {
                        IsScoped = !IsScoped;
                        StartCoroutine(OnScope(IsScoped));
                    }
                }
            }
        }
    }

    private void Shot()
    {
        AmmoInPaint--;
        GC.Ammo.text = $"{AmmoInPaint}/{Ammo}";
        CurrentProjectile = transform.GetChild(0).GetComponent<Rocket>();

        if (PhotonNetwork.IsConnected)
            servidor.Server.RPC("ActiveRocket", RpcTarget.Others, transform.GetChild(0).GetComponent<PhotonView>().ViewID);

        CurrentProjectile.StartRocket();
        //Instantiate(ShotParticle, transform);
        if (PhotonNetwork.IsConnected)
            servidor.Server.RPC("Sound", RpcTarget.Others, Net.ViewID, "ShotSound", true, Vector3.zero);
        ShotSound.Play();
    }

    private IEnumerator ShotAnimation()
    {
        Animator.SetBool("ManualShot", true);
        GC.Target.GetComponent<Animator>().SetBool("ShotRPG", true);

        yield return new WaitForSeconds(0.5f);

        Animator.SetBool("ManualShot", false);
        GC.Target.GetComponent<Animator>().SetBool("ShotRPG", false);
    }

    private IEnumerator Reload()
    {
        Recharge = GC.Reload = true;
        yield return new WaitForSeconds(0.5f);
        
        if (PhotonNetwork.IsConnected)
            servidor.Server.RPC("Sound", RpcTarget.Others, Net.ViewID, "ReloadSound", true, Vector3.zero);

        if (!Status.Morreu)
            ReloadSound.Play();

        Animator.SetBool("Reload", true);
        GC.Target.GetComponent<Animator>().SetBool("Reload", true);

        yield return new WaitForSeconds(TimeToReload);

        int Need = MaxAmmoInPaint - AmmoInPaint;
        if (Ammo >= Need)
        {
            AmmoInPaint += Need;
            Ammo -= Need;
        }
        else
        {
            AmmoInPaint += Ammo;
            Ammo = 0;
        }

        AddRocket();
        Animator.SetBool("Reload", false);
        GC.Target.GetComponent<Animator>().SetBool("Reload", false);
        GC.Ammo.text = $"{AmmoInPaint}/{Ammo}";

        yield return new WaitForSeconds(0.4f);
        Recharge = GC.Reload =  false;
    }
    public void AddRocket(bool Net=false, int RocketViewID = -1)
    {
        GameObject G;
        if (PhotonNetwork.IsConnected)
        {
            if (!Net)
            {
                G = PhotonNetwork.Instantiate("Materiais/Prefarbs/Guns/RPG-Rocket", Vector3.zero, Quaternion.identity);
                servidor.Server.RPC("AddRemRocket", RpcTarget.Others, MouseLook.player.GetComponent<PhotonView>().ViewID, G.GetComponent<PhotonView>().ViewID);
            }
            else
            {
                G = PhotonView.Find(RocketViewID).gameObject;
                G.layer= 0;
            }

            G.transform.SetParent(transform);
            G.transform.localPosition = RocketPrefab.transform.localPosition;
            G.transform.localRotation = Quaternion.identity;
        }
        else
            G = Instantiate(RocketPrefab, transform);
        
        G.transform.SetSiblingIndex(0);
    }

    public IEnumerator OnScope(bool Active)
    {
        if (GC != null)
            GC.IsScoped = Active;

        Animator.SetBool("Scoped", Active);
        GC.Target.GetComponent<Animator>().SetBool("ScopedRPG", Active);

        if (Active)
            yield return new WaitForSeconds(.15f);

        FindObjectOfType<DerrubarArvores>().enabled = !Active;
        //FindObjectOfType<Status>().HideStatus(!Active);

        if (GunsControl.GunsMode)
            Reticle.gameObject.SetActive(!Active);
    }

    private void CrossHair()
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
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
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
