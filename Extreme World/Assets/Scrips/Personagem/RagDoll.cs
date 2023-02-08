using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class RagDoll : MonoBehaviour
{
    public Collider[] PartsBory;
    public CapsuleCollider Bory;
    public Transform Pelvis;
    public LookAtConstraint Look;
    public float Force = 1;

    public void RagDollFunc(bool Active, bool IsMine=false)
    {
        Rigidbody[] Rds = Pelvis.GetComponentsInChildren<Rigidbody>();
        Vector3 Velocity = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().isKinematic = Active;
        GetComponent<Photon.Pun.PhotonTransformView>().enabled = !Active;
        Bory.enabled = !Active;
        Look.constraintActive = !Active;

        if (IsMine)
        {           
            if (Active)
            {
                transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                transform.GetChild(1).gameObject.layer = 0;
                transform.GetChild(2).gameObject.layer = 0;
                transform.GetChild(3).gameObject.layer = 0;
            }
            else
            {
                transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        else
        {
            GetComponent<Agachar>().Name.gameObject.SetActive(!Active);
        }

        foreach (Collider c in PartsBory)
        {
            //c.enabled = Active;
            c.isTrigger = !Active;
        }

        GetComponent<Animator>().enabled = !Active;
        foreach (Rigidbody Rg in Rds)
        {
            //Rg.AddExplosionForce(1000, new Vector3(-1, 0.5f, -1), 5, 0, ForceMode.Impulse);
            //Rg.AddForce(transform.forward * Force, ForceMode.Impulse);
            Rg.isKinematic = !Active;
            Rg.velocity = Velocity;
        }

        if (GetComponent<Movimentacao>().GunUsing != null)
        {
            Transform Gun = GetComponent<Movimentacao>().GunUsing.transform;

            Gun.GetComponent<Collider>().enabled = Active;
            Gun.GetComponent<Rigidbody>().isKinematic = !Active;
            Gun.transform.SetParent(transform.root.parent);
            /*if (!Active)
                FindObjectOfType<GunsControl>().RemGun();*/
        }
        
    }
    public void IgnoreRayCast()
    {
        foreach (Collider c in PartsBory)
        {
            c.gameObject.layer = 2;
        }
    }
}
