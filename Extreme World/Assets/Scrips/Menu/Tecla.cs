using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tecla : MonoBehaviour
{
    public KeyCode Key;
    public TMP_InputField Input;
    public TMP_Text text;
    public AudioSource Som;
    private Tecla LastKey;

    void Start()
    {
        if (PlayerPrefs.HasKey(transform.name))
        {
            Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(transform.name));
            Input.text = Key.ToString();
        }
        else
        {
            PlayerPrefs.SetString(transform.name, Key.ToString());
            Input.text = Key.ToString();
        }

       // Som = FindObjectOfType<servidor>().transform.GetComponent<AudioSource>();
    }
   
    public void Select()
    {
        Input.ActivateInputField();
        Input.text = "";
    }
    
    public void Charge()
    {
        if (Input.text != "")
        {
            bool NewKey = true;
            foreach (Tecla T in FindObjectsOfType<Tecla>())
            {
                if (T.Key.ToString() == Input.text && T != this)
                {
                    NewKey = false;
                    if (LastKey)
                        LastKey.text.color = Color.white;

                    T.text.color = Color.red;
                    LastKey = T;
                    Input.text = "";
                    //Som.Play();
                    StartCoroutine(T.ErrorAnim());
                    break;
                }
            }

            if (NewKey)
            {
                try
                {
                    Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), Input.text);
                    EventSystem.current.SetSelectedGameObject(null);
                    PlayerPrefs.SetString(transform.name, Key.ToString());

                    if (LastKey)
                    {
                        LastKey.text.color = Color.white;
                        LastKey = null;
                    }
                }
                catch
                {
                    Input.text = "";
                }
            }
        }
    }

    public void EditEnd()
    {
        if (Input.text == "")
            Input.text = Key.ToString();
    }
    public IEnumerator ErrorAnim()
    {
        RectTransform Rect = Input.GetComponent<RectTransform>();
        float width = Rect.rect.width / 20;
        float height = Rect.rect.height / 20;
        Rect.sizeDelta = new Vector2(Rect.rect.width + width, Rect.rect.height + height);

        yield return new WaitForSecondsRealtime(0.5f);

        Rect.sizeDelta = new Vector2(Rect.rect.width + width, Rect.rect.height + height);

    }
}
