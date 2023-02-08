using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimateMessages : MonoBehaviour
{
    private TextMeshProUGUI Texto;
    private string txt;
    private float timer;
    public int cont;
    [HideInInspector]public bool reset;
    public bool Error, Wait;
    public int repeticoes;
    public float velocity = 1;
    public AudioSource ButtonSound;
    private void Start()
    {
        Texto = GetComponent<TextMeshProUGUI>();
        txt = Texto.text;
        if (Wait)
        {
            StartCoroutine(Waiting());
        }

    }
    void Update()
    {
        if (!Wait)
        {
            if (reset)
            {
                cont = 0;
                timer = 0;
                reset = false;
                ButtonSound.Play();
            }
            timer += Time.deltaTime;
            if (Error)
            {
                if (timer >= velocity * 2)
                {
                    timer = 0;
                    cont += 1;
                }
                if (cont <= repeticoes && timer <= velocity)
                {
                    Texto.color = Color.clear;
                }
                else if (cont <= repeticoes && timer >= velocity)
                {
                    Texto.color = Color.red;
                }
            }
            else
            {
                if (timer >= 1)
                {
                    Texto.text = Texto.text + ".";
                    timer = 0;
                    cont += 1;
                }
                if (cont == 4)
                {
                    txt = Texto.text.Replace("....", "");
                    Texto.text = txt;
                    cont = 0;
                }
            }
        }
    }

    public IEnumerator Waiting(float Cont = 1.0f, bool Revert = true)
    {
        
        if (Cont >= 1)
            Revert = true;

        else if (Cont <= 0.2)
            Revert = false;

        if (Revert)
        {
            Cont -= Time.fixedDeltaTime /3;
            Texto.color = new Color(1, 1, 1, Cont);
        }

        else
        {
            Texto.color = new Color(1, 1, 1, Cont);
            Cont += Time.fixedDeltaTime /3;
        }

        yield return new WaitForFixedUpdate();
        StartCoroutine(Waiting(Cont, Revert));
    }
    private void OnDisable()
    {
        StopCoroutine(Waiting());
    }
}
