using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAVE : MonoBehaviour
{
    public static int Linguagem;
    public GameObject[] Datas;
    void Awake()
    {
        Datas = GameObject.FindGameObjectsWithTag("Data");
        if (Datas.Length >= 2)
        {
            Destroy(Datas[0]);
        }
        DontDestroyOnLoad(transform.gameObject);
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("Lingua"))
        {
            Linguagem = PlayerPrefs.GetInt("Lingua");
        }
        else 
        {
            PlayerPrefs.SetInt("Lingua",Linguagem);
        }
    }
}
