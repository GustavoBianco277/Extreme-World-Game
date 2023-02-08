using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigTeclas : MonoBehaviour
{
    public IDictionary<string, Tecla> Dict = new Dictionary<string, Tecla>();

    void Start()
    {
        foreach(Tecla T in FindObjectsOfType<Tecla>())
        {
            Dict.Add(T.transform.name, T);
        }
    }
}
