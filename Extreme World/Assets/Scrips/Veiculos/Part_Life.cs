using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Part_Life : MonoBehaviour
{
    private Master_Part_Life Master;
    private float MaxLife;
    public float Life = 100;
    [Range(0,100)]public float Porcentagem = 100;
    public bool DropPart = true, DanoCritico;
    public GameObject Smoke;
    public Slider SliderLife;
    public Image Fill;
    public List<Part_Life> Conjunto;

    public void Start()
    {
        MaxLife = Life;
        Master = transform.root.GetComponent<Master_Part_Life>();

        if (SliderLife != null)
        {
            float value = SliderLife.value = 1 / MaxLife * Life;
            Fill.color = new Color(1 - value, value, 0);
        }
    }

    public void Death(int Dano, int ViewID = -1)
    {
        print("Chegou o dano");
        Life -= Dano;

        if (SliderLife != null)
        {
            SliderColor();
        }

        float damage = Dano * (Porcentagem / 100);
        Master.Death(damage);

        if (Photon.Pun.PhotonNetwork.IsConnected && ViewID != -1)
            servidor.Server.RPC("DamagePart", Photon.Pun.RpcTarget.Others, ViewID, gameObject.name, Dano);

        if (Life <= MaxLife / 2 && Smoke != null)
        {
            GameObject G = Instantiate(Smoke, transform);
            G.transform.SetParent(transform.parent);
            print("smoke");
        }

        if (Life <= 0)
        {
            if (DropPart)
                gameObject.AddComponent<Rigidbody>();
            else if (DanoCritico)
                transform.root.GetComponent<Helicopter_Controller>().VerificaParts(DanoCritico: true);
            else
                transform.root.GetComponent<Helicopter_Controller>().VerificaParts(Cair: true);

            Destroy(this);
        }
    }
    private void SliderColor()
    {
        float value;
        if (Conjunto.Count != 0)
        {
            float life = Life, maxLife = MaxLife;
            foreach (Part_Life P in Conjunto)
            {
                life += P.Life;
                maxLife += P.MaxLife;
            }
            value = 1 / maxLife * life;
        }
        else
            value = 1 / MaxLife * Life;

        SliderLife.value = value;

        if (value > 0.5f)
            Fill.color = new Color((1 - value) * 2, 1, 0);
        else
            Fill.color = new Color(1, value * 2, 0);
        if (value <= 0.01f)
            Fill.enabled = false;
    }
}