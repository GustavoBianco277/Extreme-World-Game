using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public GameObject FirePrefab;
    public void Fire()
    {
        Instantiate(FirePrefab, this.transform);
    }
}
