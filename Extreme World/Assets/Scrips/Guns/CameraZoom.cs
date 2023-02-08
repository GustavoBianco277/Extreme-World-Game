using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private int MinDistance = 10;
    [SerializeField] private float Speed  = 4;
    [SerializeField] private float TimeWait = 5;
    public Transform Alvo;
    private bool Parent;
    private float Timer;
    public Vector3 LastPosition;
    
    void Update()
    {
        if (Alvo != null)
        {
            Timer += Time.deltaTime;
            if (Timer >= TimeWait)
            {
                if (Timer < TimeWait*2)
                    CamZoom(true);
                else
                {
                    Alvo.parent.GetComponent<Outline>().enabled = false;
                    FindObjectOfType<Status>().PainelMorto.SetActive(true);

                    Alvo = null;
                    Timer= 0;
                }
            }
            else
                CamZoom();
        }
    }

    public void CamZoom(bool Revert = false)
    {
        transform.LookAt(Alvo);
        if (Revert)
        {
            Parent= false;
            transform.localPosition = Vector3.Lerp(transform.localPosition, LastPosition, Time.deltaTime * Speed);
            //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, Time.deltaTime * Speed);
        }
        else
        {
            Alvo.parent.GetComponent<Outline>().enabled = true;
            float Distance = Vector3.Distance(Alvo.transform.position, transform.position);

            if (Distance > MinDistance)
            {
                if (!Parent)
                {
                    Parent = true;
                    //Camera.main.transform.SetParent(transform.parent.parent);
                    //MouseLook.MouseEnable = false;
                }
                transform.Translate(Vector3.forward * Speed);

                RaycastHit hit;
                if (Physics.Linecast(Alvo.position, transform.position, out hit))
                    transform.position = hit.point + transform.forward* 0.2f;
            }
        }
    }
}
