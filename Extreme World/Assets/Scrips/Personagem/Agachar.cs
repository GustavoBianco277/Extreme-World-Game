using UnityEngine;
using System.Collections;
using Photon.Pun;

public class Agachar : MonoBehaviour 
{
	public CapsuleCollider Collider;
	public Transform Name, HolderGun;
	public float AlturaAbaixado = 1.3f;
	public float AlturaLevantado = 1.9f;
	public bool EstaAgachado, TempoAgachar = true;
	public GameObject cam;
	public Vector3 camPos, HolderGunPos;
	private Vector3 CamLast, ColliderCenterLast, HolderGunLast;
	void Start () 
	{
		Collider = GetComponent<CapsuleCollider>();
		cam = GameObject.FindGameObjectWithTag("MainCamera");
		CamLast = cam.transform.localPosition;
		ColliderCenterLast = Collider.center;
        HolderGunLast = HolderGun.transform.localPosition;
	}

	public IEnumerator AgacharFunc(bool Active, float Timer=0, bool Agachei=false)
    {
		/*if (PhotonNetwork.IsConnected && !Agachei)
		{
			if (Active)
				servidor.Server.RPC("PositionGun", RpcTarget.OthersBuffered, GetComponent<PhotonView>().ViewID, Active);
			else
			{
				int[] ListId = { PhotonNetwork.LocalPlayer.ActorNumber };
				PhotonNetwork.RemoveBufferedRPCs(servidor.Server.ViewID, "PositionGun", ListId);
			}

			Agachei = true;
		}*/
		StartCoroutine(PositionGun(Active));

		TempoAgachar = false;
		if (Active)
		{
			Collider.height = AlturaAbaixado;
			Collider.center = new Vector3(0, 0.5f, 0);
			cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, camPos, Time.deltaTime * 3);
		}
		else
		{
			EstaAgachado = false;
			Collider.height = AlturaLevantado;
			Collider.center = ColliderCenterLast;
			cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, CamLast, Time.deltaTime * 3);
		}

		if (Timer <= 1) 
		{
			yield return new WaitForEndOfFrame();
			Timer += Time.deltaTime;
			StartCoroutine(AgacharFunc(Active, Timer, Agachei)); 
		}
        else
        {
			if (Active)
				cam.transform.localPosition = camPos;
			else
				cam.transform.localPosition = CamLast;
			TempoAgachar = true;
        }
	}
	
	public IEnumerator PositionGun(bool State, float time = 0)
	{
		Vector3 Pos = HolderGun.transform.localPosition;
		if (State)
            HolderGun.transform.localPosition = Vector3.Lerp(Pos, HolderGunPos, time);
		else
            HolderGun.transform.localPosition = Vector3.Lerp(Pos, HolderGunLast, time);

        if (time < 1)
		{
			yield return new WaitForEndOfFrame();
			StartCoroutine(PositionGun(State, time += Time.deltaTime * 2));
		}
		else
		{
			if (State)
                HolderGun.transform.localPosition = HolderGunPos;
			else
                HolderGun.transform.localPosition = HolderGunLast;

        }

	}
}