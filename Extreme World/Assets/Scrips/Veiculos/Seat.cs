using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public Transform Target;
    public bool SentFull, PilotSent;
    public int ActorNumber = -1;

    void Update()
    {
        if (Target != null)
        {
            Target.position = transform.position;
            Target.rotation = transform.rotation;
            SentFull = true;
        }        
    }
}
