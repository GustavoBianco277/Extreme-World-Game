using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public class Interagir : MonoBehaviour
{
    private RaycastHit Colisor;
    private Ray CentroDaTela;
    private Camera cam;
    private Linguagem Ling;
    [SerializeField] private int DistanciaMinima;
    void Start()
    {
        cam = Camera.main;
        Ling = FindObjectOfType<Linguagem>();
    }

    void Update()
    {
        CentroDaTela = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        bool colisao = Physics.Raycast(CentroDaTela, out Colisor);
        bool Active = false;

        if (MouseLook.Veiculo && GunsControl.GunsMode && !MouseLook.VehicleUsing.GetComponent<Helicopter_Controller>().shoting.Active && !Status.Morreu)
        {
            Ling.Interagir(InteragirEnum.Acessar_Metralhadora);
            Active= true;
        }

        else if (colisao)
        {
            if (Vector3.Distance(cam.transform.parent.transform.position, Colisor.transform.position) <= DistanciaMinima)
            {
                Transform coll = Colisor.collider.transform;
                if (coll.tag == "Veiculo" && !MouseLook.Veiculo)
                {
                    Ling.Interagir(InteragirEnum.Entrar_Veiculo);
                    coll.parent.GetComponent<OpenDoor>().Open(coll.GetComponent<OpenVehicle>());
                    Active = true;
                }
                else if (Colisor.transform.tag == "Coletavel")
                {
                    Ling.Interagir(InteragirEnum.Pegar_Item);
                    Active= true;
                }
            }
        }
        if (!Active)
            Ling.Jogo.Interagir.gameObject.SetActive(false);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Outline>() && other.tag != "Player")
            other.GetComponent<Outline>().enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Outline>() && other.tag != "Player")
            other.GetComponent<Outline>().enabled = false;
    }
}
