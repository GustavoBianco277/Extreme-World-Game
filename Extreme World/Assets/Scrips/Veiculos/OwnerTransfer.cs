using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OwnerTransfer : MonoBehaviourPun
{
    public void TrocarDono()
    {
        base.photonView.RequestOwnership();
    }
}
