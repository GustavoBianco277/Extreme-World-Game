using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EfeitoEscurecer : MonoBehaviour
{
    public GameObject PainelMorte, PainelHelicopter;
    private bool Start = true;
   // public bool Efeito, Texts, Reset, Veiculo;
    //public float TempoDeEspera, MaxAlpha = 1, Velocity = 1;
   
    public GameObject TextMeshPro, Respawn;
    
    void Update()
    {
        if (Input.anyKeyDown && Start)
        {
            StartCoroutine(Esclarecer());
            Start = false;
            MouseLook.player.GetComponent<Movimentacao>().Enabled = true;
            FindObjectOfType<DerrubarArvores>().enabled = true;
        }
    }
    public IEnumerator Efeito2(GameObject Painel, float Wait=0, float MaxAlpha=1, float Velocity=1, bool Reverse=false, bool Text=false, bool Morte=false)
    {
        Color color;
        if (Text)
            color = Painel.GetComponent<TMPro.TextMeshProUGUI>().color;
        else 
            color = Painel.GetComponent<Image>().color;

        if (Reverse)
        {
            if (Text)
                Painel.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime * Velocity);
            else
                Painel.GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime * Velocity);
        }
        else
        {
            if (Text)
                color = Painel.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, color.a + Time.deltaTime * Velocity);
            else
            {
                float value = color.a + Time.deltaTime * Velocity;
                value = Mathf.Clamp(value, 0, MaxAlpha);
                Painel.GetComponent<Image>().color = new Color(color.r, color.g, color.b, value);
            }
        }
        
        yield return new WaitForEndOfFrame();

        if (color.a < MaxAlpha && !Reverse)
        {
            if (Text)
                StartCoroutine(Efeito2(Painel, Wait, Text: true));
            else
                StartCoroutine(Efeito2(Painel, Wait, MaxAlpha, Velocity, Morte: Morte));
        }

        else if (color.a > 0 && Reverse)
        {
            if (Text)
                StartCoroutine(Efeito2(Painel, Reverse: true, Text: true));
            else
                StartCoroutine(Efeito2(Painel, Velocity: Velocity, Reverse: true));
        }

        else
        {
            if (Wait != 0)
            {
                yield return new WaitForSeconds(Wait);
                StartCoroutine(Efeito2(Painel, Velocity: Velocity, Reverse: true));
            }
            else if (Morte)
            {
                print("morri");
                StartCoroutine(Efeito2(TextMeshPro, Text: true));

                yield return new WaitForSeconds(1);
                Respawn.SetActive(true);
            }
        }
    }
    public IEnumerator Esclarecer(float Cont = 1.0f)
    {
        Cont -= Time.fixedDeltaTime /4;
        PainelHelicopter.GetComponent<Image>().color = new Color (0, 0, 0, Cont);

        if (Cont >= 0.0f)
        {
            PainelHelicopter.transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForFixedUpdate();
            StartCoroutine(Esclarecer(Cont));
        }
    }
}
