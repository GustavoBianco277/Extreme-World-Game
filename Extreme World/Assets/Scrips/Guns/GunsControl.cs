using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GunsControl : MonoBehaviour
{
    private Animator animator, AnimatorPlayer;
    private SelecionaSlot Select;
    private SlotScalerItem SLT;
    [SerializeField] private GameObject HotBar;
    public static bool GunsMode;
    public TMPro.TextMeshProUGUI Ammo;
    public RectTransform Reticle;
    public GameObject ScopeOverlay;
    public GameObject GunCamera;
    public Transform Target, ArmLeft, ArmRight;
    public bool IsScoped, Reload;
    public GameObject GunSelected;
    public GameObject[] Guns;

    void Start()
    {
        Select = FindObjectOfType<SelecionaSlot>();
        SLT = FindObjectOfType<SlotScalerItem>();
        print(GunsMode);
        GunsMode= false;
    }
    void Update()
    {
        if (Target == null)
        {
            if (MouseLook.player != null)
            {
                Target = MouseLook.player.GetComponent<Movimentacao>().HolderGun.GetChild(0);
                animator = Target.GetComponent<Animator>();
            }
            return;
        }

        if (Input.GetKeyDown("g") && Application.isEditor)
            GunsModeActive(!GunsMode);

        if (GunSelected != null)
            Target.parent.rotation = Camera.main.transform.rotation;

        if (Input.GetKeyDown("1") && GunsMode)
	        StartCoroutine(AddGun(0));

        else if (Input.GetKeyDown("2") && GunsMode)
	        StartCoroutine(AddGun(1));

        else if (Input.GetKeyDown("3") && GunsMode)
            StartCoroutine(AddGun(2));

    }
    public IEnumerator AddGun(int IdexGun)
    {
        if (!MouseLook.Veiculo && !Status.Morreu)
        {
            if (GunSelected == null || GunSelected.GetComponent<Guns_ID>().ID != Guns[IdexGun].GetComponent<Guns_ID>().ID)
            {
                if (AnimatorPlayer == null)
                    AnimatorPlayer = MouseLook.player.GetComponent<Animator>();

                MouseLook.player.GetComponent<Animacoes>().Desable();
                AnimatorPlayer.SetBool("InGun", true);

                // Troca Gun
                animator.SetBool("TrocaGun", true);
                GetComponent<Animator>().SetBool("TrocaGun", true);
                StopAnimation();

                yield return new WaitForSeconds(.15f);

                GameObject Gun;
                if (PhotonNetwork.IsConnected)
                {
                    if (GunSelected != null)
                    {
                        int[] PlayersID = { PhotonNetwork.LocalPlayer.ActorNumber };
                        PhotonNetwork.RemoveBufferedRPCs(servidor.Server.ViewID, "AddGun", PlayersID);
                        servidor.Server.RPC("RemGun", RpcTarget.Others, MouseLook.player.GetComponent<PhotonView>().ViewID);
                    }
                    servidor.Server.RPC("AddGun", RpcTarget.OthersBuffered, MouseLook.player.GetComponent<PhotonView>().ViewID, "HolderGun", IdexGun);
                }
                Reticle.gameObject.SetActive(true);

                if (IsScoped)
                {
                    if (GunSelected.GetComponent<Gun>())
                        StartCoroutine(GunSelected.GetComponent<Gun>().OnScope(!IsScoped));
                    else
                        StartCoroutine(GunSelected.GetComponent<RPG_Gun>().OnScope(!IsScoped));
                }

                if (GunSelected != null)
                    Destroy(GunSelected);

                Gun = Instantiate(Guns[IdexGun], transform);
                MouseLook.player.GetComponent<Movimentacao>().GunUsing = Gun;
                GunSelected = Gun;

                if (Gun.GetComponent<Gun>())
                    Gun.GetComponent<Gun>().Offline = true;
                else if (Gun.GetComponent<RPG_Gun>())
                {
                    Gun.GetComponent<RPG_Gun>().Active = true;
                    Gun.GetComponent<RPG_Gun>().AddRocket();
                }

                animator.SetBool("TrocaGun", false);
                GetComponent<Animator>().SetBool("TrocaGun", false);

                IkMove IK = MouseLook.player.GetComponent<IkMove>();
                IK.RightHand = GunSelected.transform.Find("Right Hand");
                IK.LeftHand = GunSelected.transform.Find("Left Hand");

                yield return new WaitForSeconds(.15f);

                AnimatorPlayer.SetLayerWeight(1, 1);

                MouseLook.player.transform.GetChild(1).gameObject.layer = 8;
                MouseLook.player.transform.GetChild(2).gameObject.layer = 8;
                MouseLook.player.transform.GetChild(3).gameObject.layer = 8;
                Reload = false;
            }
        }
    }
    private void StopAnimation()
    {
        animator.SetBool("Scoped", false);
        GetComponent<Animator>().SetBool("Scoped", false);
        animator.SetBool("ScopedShot", false);
        GetComponent<Animator>().SetBool("ScopedShot", false);
        animator.SetBool("Shot", false);
        GetComponent<Animator>().SetBool("Shot", false);
        animator.SetBool("ManualShot", false);
        GetComponent<Animator>().SetBool("ManualShot", false);
        animator.SetBool("Walk", false);
        GetComponent<Animator>().SetBool("Walk", false);
        animator.SetBool("Run", false);
        GetComponent<Animator>().SetBool("Run", false);
        animator.SetBool("Reload", false);
        GetComponent<Animator>().SetBool("Reload", false);
    }
    public void RemGun() 
    {
        if (GunSelected != null)
        {
            if (IsScoped)
                StartCoroutine(GunSelected.GetComponent<Gun>().OnScope(!IsScoped));

            if (PhotonNetwork.IsConnected)
            {
                int[] PlayersID = { PhotonNetwork.LocalPlayer.ActorNumber };
                PhotonNetwork.RemoveBufferedRPCs(servidor.Server.ViewID, "AddGun", PlayersID);
                servidor.Server.RPC("RemGun", RpcTarget.Others, MouseLook.player.GetComponent<PhotonView>().ViewID);
            }

            AnimatorPlayer.SetBool("InGun", false);
            AnimatorPlayer.SetLayerWeight(1, 0);

            MouseLook.player.transform.GetChild(1).gameObject.layer = 0;
            MouseLook.player.transform.GetChild(2).gameObject.layer = 0;
            MouseLook.player.transform.GetChild(3).gameObject.layer = 0;
            Reticle.gameObject.SetActive(false);

            if (GunSelected.GetComponent<RPG_Gun>())
            {
                GunSelected.SetActive(false);
                Destroy(GunSelected, 15);
                IkMove IK = MouseLook.player.GetComponent<IkMove>();
                IK.RightHand = null;
                IK.LeftHand = null;
            }
            else
                Destroy(GunSelected);
            GunSelected = null;
        }
    }
    public void GunsModeActive(bool State)
    {
        if (Target == null)
            StartCoroutine(WaitPerson());

        else
        {
            GunsMode = State;

            if (Select.itemMao != null)
            {
                if (PhotonNetwork.IsConnected)
                    Select.DestroyItemView();
                Destroy(Select.itemMao);
            }
            if (SLT.InvAberto)
                SLT.CloseInventario();

            Ammo.transform.parent.gameObject.SetActive(State);
            FindObjectOfType<Status>().HideStatus(!State);
            HotBar.SetActive(!State);

            if (State)
                StartCoroutine(AddGun(0));
            else
                RemGun();
        }
    }

    private IEnumerator WaitPerson()
    {
        if (Target != null)
            GunsModeActive(true);
        else
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(WaitPerson());
        }
    }
}