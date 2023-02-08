using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthTree : MonoBehaviour
{
    [Range(1, 6)] public int estagio = 1;
    public float TempoACadaEstagioMin;
    private float Timer;
    private bool Mudou;
    private ARVORE Arvore;
    private int Madeira = 0, Graveto = 0, vida = 0;
    void Start()
    {
        this.transform.localScale = new Vector3(0.08f, 0.09f, 0.08f);
        Arvore = GetComponent<ARVORE>();
        Madeira = Arvore.maxMadeiras;
        Graveto = Arvore.maxGraveto;
        vida = Arvore.VIDA;
        Arvore.maxMadeiras = 0;
        Arvore.maxGraveto = 0;
        Arvore.VIDA = vida / 5;

    }

    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= TempoACadaEstagioMin * 60)
        {
            estagio += 1;
            Mudou = true;
            Timer = 0;
        }
        if (Mudou)
        {
            Mudou = false;
            switch (estagio)
            {
                case 2:
                    this.transform.localScale = new Vector3(0.15f, 0.2f, 0.15f);
                    Arvore.maxMadeiras = 0;
                    Arvore.maxGraveto = 1;
                    Arvore.VIDA = vida / 4;
                    break;
                case 3:
                    this.transform.localScale = new Vector3(0.25f, 0.4f, 0.25f);
                    Arvore.maxGraveto = 2;
                    Arvore.maxMadeiras = 1;
                    Arvore.VIDA = vida / 3;
                    break;
                case 4:
                    this.transform.localScale = new Vector3(0.5f, 0.6f, 0.5f);
                    if (Madeira >= 3)
                        Arvore.maxMadeiras = Madeira / 2;
                    else
                        Arvore.maxMadeiras = 2;
                    Arvore.maxGraveto = 3;
                    Arvore.VIDA = vida / 2;
                    break;
                case 5:
                    this.transform.localScale = new Vector3(0.65f, 0.8f, 0.65f);
                    Arvore.maxMadeiras = Madeira;
                    Arvore.maxGraveto = Graveto / 2;
                    Arvore.VIDA = vida;
                    break;
                case 6:
                    this.transform.localScale = new Vector3(1, 1, 1);
                    Arvore.maxGraveto = Graveto;
                    Arvore.maxMadeiras = Madeira;
                    Destroy(this);
                    break;
            }
        }
    }
}
