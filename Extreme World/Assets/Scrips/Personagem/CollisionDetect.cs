using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    public AudioSource FootStepWater, FootStepGround;
    public Movimentacao Move;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4 && !Move.PodeNadar)
        {
            Move.Swim.Stop();
            Move.FootStep.Stop();
            Move.FootStep = FootStepWater;
            Move.FootStep.Play();
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == 4) 
        {
            Move.FootStep.Stop();
            Move.FootStep = FootStepGround;
        }
    }
}
