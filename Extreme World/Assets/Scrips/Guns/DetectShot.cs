using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectShot : MonoBehaviour
{
    public RectTransform Detectshot;
    public Transform Alvo;
    public float VisibleTime = 0.5f;
    private float Timer;

    void Update()
    {
        if (Alvo != null)
        {
            Detectshot.gameObject.SetActive(true);
            Timer += Time.deltaTime;

            if (Timer < VisibleTime)
                Look(Alvo);
            else
            {
                Alvo = null;
                Detectshot.gameObject.SetActive(false);
                Timer = 0;
            }
        }
    }
    private void Look(Transform Alvo)
    {
        transform.LookAt(Alvo);
        Detectshot.localEulerAngles = new Vector3(0, -180, transform.localRotation.eulerAngles.y);
    }
}
