using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoBuild : MonoBehaviour
{
    public string Name = "Fundacao(Clone)";
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == Name)
        {
            Destroy(this.gameObject);
        }
    }
}
