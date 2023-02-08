using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor.SceneManagement;

public class ThisBuild : MonoBehaviour
{
    [SerializeField] private GameObject ObjetoSpawn;
    [SerializeField] private Material Green, Red;
    [SerializeField] private string NameTag, NameObjectBuild = "Fundacao(Clone)";
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private Vector3 MaxAngle = new Vector3(30, 30, 30);

    [HideInInspector] public Transform Outro;
    public bool StopDown;
    public bool Colidiu, AngleCheck = true, PodeConstruir = true;
    private Material currentMaterial;
    private Vector3 velocity;

    private SistemaConstrucao SisConstrucao;
    void Start()
    {
        SisConstrucao = FindObjectOfType<SistemaConstrucao>();
    }
    void Update()
    {
        VerificarAngle();
        if (Colidiu && Outro != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Outro.position, ref velocity, smoothTime);
            transform.rotation = Outro.rotation;
        }

        if (Input.GetMouseButtonDown(0) && PodeConstruir && AngleCheck && !MenuPause.MenuOpen && !ChatMsm.Opened && !Status.Morreu)
        {   
            if (PhotonNetwork.IsConnected)
            {
                //PhotonNetwork.InstantiateRoomObject($"Materiais/Prefarbs/Construcao/{ObjetoSpawn.name}", transform.position, transform.rotation);
                servidor.Server.RPC("Instantiate", RpcTarget.MasterClient, $"Materiais/Prefarbs/Construcao/{ObjetoSpawn.name}", transform.position, transform.rotation);
            }
            else
            {
                GameObject T = Instantiate(ObjetoSpawn);
                T.transform.position = transform.position;
                T.transform.rotation = transform.rotation;
            }

            Colidiu = false;
            SisConstrucao.PosXY[1] = 0f;
            SisConstrucao.PosXY[0] = 0f;

            if (Outro != null)
                Destroy(Outro.gameObject);

            if (SisConstrucao.OneBuild)
            {
                SisConstrucao.enabled = false;
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //print(other.name);
        if (other.name == NameObjectBuild || other.transform.root.name == "Ambiente")
        {
            PodeConstruir = false;
            TrocarMaterial(Red);
            print("desativei");
            //transform.GetComponent<Renderer>().material = Red;
            //material.color = Color.red;
        }

        else if (other.tag == NameTag && !Colidiu)
        {
            Colidiu = true;
            Outro = other.transform;
            print("Vc colidiu");
        }
        if (other.name != "Terrain")
        {
            StopDown = true;
        }
        else
            StopDown= false;
        
    }
    private void OnTriggerExit(Collider other)
    {
        print($"Saiu {other.name}");
        //if (other.name == "Fundacao(Clone)" || other.transform.root.name == "Ambiente")
        //{
            PodeConstruir = true;
            TrocarMaterial(Green);
            //transform.GetComponent<Renderer>().material = Green;
            //material.color = Color.green;
        //}
         
    }

    private void TrocarMaterial(Material material)
    {
        if (currentMaterial != material)
        {
            currentMaterial = material;
            Renderer[] Renderes = transform.GetComponentsInChildren<Renderer>();
            List<Material> Materials = new List<Material>();

            foreach (Renderer Render in Renderes)
            {
                for (int i = 0; i < Render.materials.Length; i++)
                {
                    Materials.Add(material);
                }
                Render.materials = Materials.ToArray();
                Materials.Clear();
            }
        }
    }
    private void VerificarAngle()
    {
        float x = transform.eulerAngles.x;
        float y = transform.eulerAngles.y;
        float z = transform.eulerAngles.z;

        if (x < 360f - MaxAngle.x && x > MaxAngle.x)
        {
            TrocarMaterial(Red);
            AngleCheck = false;
        }

        else if (y < 360f - MaxAngle.y && y > MaxAngle.y)
        {
            TrocarMaterial(Red);
            AngleCheck = false;
        }

        else if (z < 360f - MaxAngle.z && z > MaxAngle.z)
        {
            TrocarMaterial(Red);
            AngleCheck = false;
        }

        else if (PodeConstruir)
        {
            AngleCheck = true;
            TrocarMaterial(Green);
        }
    }
}
