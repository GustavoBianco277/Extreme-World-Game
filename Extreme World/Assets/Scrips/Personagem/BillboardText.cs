
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardText : MonoBehaviour
{
    public Transform camTransform;

    Quaternion originalRotation;

    void Awake()
    {
        originalRotation = transform.rotation;
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        transform.rotation = camTransform.rotation * originalRotation;
    }
}