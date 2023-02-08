using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FecharAviso : MonoBehaviour
{
    public void Sair()
    {
        try
        {
            GameObject.Find("Salas").active = false;
        }
        catch { }
        
        FindObjectOfType<MenuPause>().VisibleMenu(true);
        FindObjectOfType<Multiplayer>().BotaoMultiplayer.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
