using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OpenDoor : MonoBehaviourPunCallbacks
{
    public VehicleShoting Shot;
    public bool Piloting;
    public KeyCode Entrar = KeyCode.F;
    public Transform PilotSeatLoc, PassagerSeatLoc;
    
    public void Open(OpenVehicle Vehicle)
    {
        if (Input.GetKeyUp(Entrar) && !ChatMsm.Opened && !MenuPause.MenuOpen)
        {
            StartCoroutine(Vehicle.EntrarVeiculo());
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        Seat Passager = PassagerSeatLoc.GetComponent<Seat>();
        Seat Pilot = PilotSeatLoc.GetComponent<Seat>();

        if (Pilot.ActorNumber == otherPlayer.ActorNumber)
        {
            Pilot.SentFull = false;
            Pilot.ActorNumber = -1;
        }

        else if (Passager.ActorNumber == otherPlayer.ActorNumber)
        {
            Passager.SentFull = false;
            Passager.ActorNumber = -1;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            Helicopter_Controller Heli = GetComponent<Helicopter_Controller>();
            GetComponent<PhotonView>().RPC("ForceHelicopter", newPlayer, Heli.RotationMotor, Heli.ForceUpDown);
        }
    }

    [PunRPC]
    public void ForceHelicopter(float Force, float ForceUpDown)
    {
        GetComponent<Helicopter_Controller>().RotationMotor = Force;
        GetComponent<Helicopter_Controller>().ForceUpDown = ForceUpDown;
    }

    [PunRPC]
    public void SeatHelicopter(bool PilotSeat, bool Sair, int id, int ActorNum)
    {
        GameObject PlayerPrefab = PhotonView.Find(id).gameObject;
        IkMove IK = PlayerPrefab.GetComponent<IkMove>();
        OpenVehicle OV;

        Transform Seat;
        if (PilotSeat)
	    {
            Seat = PilotSeatLoc;
	        OV = transform.GetChild(0).GetComponent<OpenVehicle>();
	    }
        else
	    {
            Seat = PassagerSeatLoc;
    	    OV = transform.GetChild(1).GetComponent<OpenVehicle>();
	    }

        if (Sair)
        {
            Seat.GetComponent<Seat>().SentFull = false;
            Seat.GetComponent<Seat>().Target = null;
            Seat.GetComponent<Seat>().ActorNumber = -1;
            PlayerPrefab.GetComponent<PhotonTransformView>().enabled = true;
            PlayerPrefab.GetComponent<CapsuleCollider>().enabled = true;
            PlayerPrefab.GetComponent<Movimentacao>().VehicleUsing = null;

            IK.RightHand = null;
            IK.LeftHand = null;
            IK.RightFoot = null;
            IK.LeftFoot = null;
        }
        else
        {
            Seat.GetComponent<Seat>().SentFull = true;
            Seat.GetComponent<Seat>().Target = PlayerPrefab.transform;
            Seat.GetComponent<Seat>().ActorNumber = ActorNum;
            PlayerPrefab.GetComponent<PhotonTransformView>().enabled = false;
            PlayerPrefab.GetComponent<CapsuleCollider>().enabled = false;
            PlayerPrefab.GetComponent<Movimentacao>().VehicleUsing = transform;

            if (PhotonNetwork.LocalPlayer.ActorNumber != ActorNum && PilotSeat)
            {
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<ConstantForce>().enabled = false;
            }

            IK.RightHand = OV.RightHand;
            IK.LeftHand = OV.LeftHand;
            IK.RightFoot = OV.RightFoot;
            IK.LeftFoot = OV.LeftFoot;
        }
    }

    [PunRPC]
    public void UpdateCockpit(int ViewID, Vector2 hMove, bool Active)
    {
        Transform Heli = PhotonView.Find(ViewID).transform;
        if (Heli == MouseLook.player.GetComponent<Movimentacao>().VehicleUsing)
            Heli.GetComponent<Helicopter_Controller>().UpdateCockpit(hMove, Active);
    }

    [PunRPC]
    public void Explosion(int ViewID)
    {
        PhotonView.Find(ViewID).GetComponent<Helicopter_Controller>().ExplosaoVoid();
    }

    [PunRPC]
    public void StartMachineGun(int ViewID, bool Active)
    {
        VehicleShoting Shot = PhotonView.Find(ViewID).GetComponent<OpenDoor>().Shot;
        Shot.GunActive = Active;
        if (!Active)
            Shot.StartShoting(false);
    }
}
