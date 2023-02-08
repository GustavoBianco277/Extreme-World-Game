using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    // Components
     public GameObject Prefab;
    private RPG_Gun GunUsing;
    private bool Start;

    [SerializeField] private AudioSource BulletSound;
    [SerializeField] private GameObject Particle_Rocket, Impact;
    [SerializeField] private int Force = 80;
    [SerializeField] private int Damage = 300;
    [SerializeField] private int ForceExplosion = 1000;
    [SerializeField] private float Radius = 10;

    public void Awake()
    {
        //GunUsing = transform.parent.parent.GetComponent<RPG_Gun>();
        //Prefab = Instantiate(Particle_Rocket, transform.parent);
        //GunUsing.ShotSound.Play();

        //if (!Net)
        //{
        //transform.parent.SetParent(transform.root.parent);
        
        //Prefab.transform.SetParent(transform.parent);
        /*GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<DestroyAfterTimeParticle>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(transform.right * Force, ForceMode.Impulse);*/
        //Start = true;
        //}
        //else
        //{
            //gameObject.SetActive(false);
            //Destroy(gameObject);
        //}
    }
    public void StartRocket(bool Net = false)
    {
        if (Net)
            Prefab = Instantiate(Particle_Rocket, transform.parent.parent.parent.GetChild(1));
        else
            Prefab = Instantiate(Particle_Rocket, transform.parent);
        GunUsing = transform.parent.GetComponent<RPG_Gun>();
        //GunUsing.CurrentProjectile = this;
        GunUsing.ShotSound.volume = 1;
        GunUsing.ShotSound.Play();
        
        print(Prefab.name);
        transform.SetParent(transform.root.parent);
        Prefab.transform.SetParent(transform.parent);
        //print(transform.root.parent);

        if (!Net)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(transform.right * Force, ForceMode.Impulse);
            GetComponent<CapsuleCollider>().enabled = true;
            GetComponent<DestroyAfterTimeParticle>().enabled = true;
            
            Start = true;
        }
        else
        {
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Start)
        {
            //transform.GetChild(0).gameObject.AddComponent<SphereCollider>().isTrigger = true;
            GetComponent<CapsuleCollider>().isTrigger = true;
            GetComponent<CapsuleCollider>().radius = 1.8f;
            GetComponent<Rigidbody>().isKinematic = true;

            bool ThisProjectile = false;
            GameObject Pref;
            if (GunUsing.CurrentProjectile == this)
                ThisProjectile = true;

            if (PhotonNetwork.IsConnected)
            {
                Pref = PhotonNetwork.Instantiate($"Materiais/Particles/{Impact.name}", transform.position, Quaternion.identity);
                servidor.Server.RPC("DestroyRocket", RpcTarget.Others, MouseLook.player.GetComponent<PhotonView>().ViewID, GetComponent<PhotonView>().ViewID, transform.position, ThisProjectile);
            }
            else
            {
                Pref = Instantiate(Impact, transform.parent);
            }
            Destroy(Prefab);
            Pref.transform.position = transform.position;

            GetComponent<Rigidbody>().AddExplosionForce(ForceExplosion, transform.position, Radius, 2f, ForceMode.Impulse);
            //GunUsing.BulletSound.transform.position = transform.position;
            if (ThisProjectile)
            {
                GunUsing.ShotSound.volume = 0.3f;
                StartCoroutine(Timer(0.3f, GunUsing));
            }
            //GunUsing.BulletSound.Play();
            BulletSound.Play();
            Start = false;
            GetComponent<MeshRenderer>().enabled = false;
            
            if (PhotonNetwork.IsConnected)
                Invoke("DestroyGO", 2);  
            else
                Destroy(gameObject, 2);   
        }
    }
    private void DestroyGO()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    private IEnumerator Timer(float timer, RPG_Gun GunUsing)
    {
        yield return new WaitForSeconds(timer);
        GunUsing.ShotSound.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name!= "Target")
        {
            if (other.transform.GetComponent<Part_Life>())
            {
                if (PhotonNetwork.IsConnected)
                {
                    print("acertei");
                    other.transform.GetComponent<Part_Life>().Death(Damage, other.transform.root.GetComponent<PhotonView>().ViewID);
                }

                else
                    other.transform.GetComponent<Part_Life>().Death(Damage);
            }

            if (other.transform.root.tag == "Player" && PhotonNetwork.IsConnected)
            {
                if (other.transform.root.GetComponent<Movimentacao>().VehicleUsing == null)
                    servidor.Server.RPC("GunDamage", other.transform.root.GetComponent<PhotonView>().Owner, Damage, PhotonNetwork.LocalPlayer.NickName, other.GetComponent<Collider>().transform.name, MouseLook.player.GetComponent<PhotonView>().ViewID);
            }
            else if (other.transform.root.tag == "Player")
                FindObjectOfType<Status>().Hit(Damage, "");

            Destroy(GetComponent<CapsuleCollider>(),0.2f);
        }
    }

    /*private void OnParticleCollision(GameObject other)
    {
        print(other.tag);
        transform.SetParent(transform.parent.parent);
        GameObject Pref;
        if (PhotonNetwork.IsConnected)
        {
            Pref = PhotonNetwork.Instantiate($"Materiais/Particles/{Impact.name}", transform.position, Quaternion.identity);
            servidor.Server.RPC("DestroyRocket", RpcTarget.Others, MouseLook.player.GetComponent<PhotonView>().ViewID, transform.position);
        }
        else
        {
            Pref = Instantiate(Impact, transform.parent);
        }
        
        Pref.transform.position = transform.position;

        if (other.GetComponent<Part_Life>())
        {
            print("entri");
            if (PhotonNetwork.IsConnected)
                other.GetComponent<Part_Life>().Death(Damage, other.GetComponent<PhotonView>().ViewID);

            else
                other.GetComponent<Part_Life>().Death(Damage);
        }

        if (other.tag == "Player" && PhotonNetwork.IsConnected)
        {
            servidor.Server.RPC("GunDamage", other.GetComponent<PhotonView>().Owner, Damage, PhotonNetwork.LocalPlayer.NickName, other.transform.name, MouseLook.player.GetComponent<PhotonView>().ViewID);
        }
        //GetComponent<Rigidbody>().AddExplosionForce(ForceExplosion, transform.position, Radius, 2f, ForceMode.Impulse);
        GunUsing.BulletSound.transform.position = transform.position;
        GunUsing.ShotSound.volume = 0.3f;
        StartCoroutine(Timer(0.3f, GunUsing));
        GunUsing.BulletSound.Play();
        Start = false;
        Destroy(gameObject, 0.4f);
        //Destroy(gameObject, 0.4f);
    }*/
}
