using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnimationStates
{
	IDLE,
	WALK,
	WALKBACK,
	WALKLEFT,
	WALKRIGHT,
	CROUCHIDLE,
	CROUCHFORWARD,
	CROUCHBACK,
	CROUCHRIGHT,
	CROUCHLEFT,
	SWIMIDLE,
	SWIM,
	RUN,
	HIT,
	HITTOOL,
	JUMP,
	PILOTHELICOPTER,
	IDDLESIT
}
public class Animacoes : MonoBehaviour {

	public Animator Animator;
	private float value;
	private bool Timer;
	private void Start()
	{
		Animator = GetComponent<Animator>();
	}
    private void Update()
    {
		SuaveLayer();
	}
    public void PlayAnimation(AnimationStates StateAnimation)
	{
		switch (StateAnimation) 
		{
		case AnimationStates.IDLE: 
			{
				StopAnimation ();
				Animator.SetBool ("InIdle", true);
			}
			break;
		case AnimationStates.WALK: 
			{
				StopAnimation ();
				Animator.SetBool ("InWalk", true);
			}
			break;
		case AnimationStates.WALKBACK: 
			{
				StopAnimation ();
				Animator.SetBool ("InWalkBack", true);
			}
			break;
		case AnimationStates.SWIM: 
			{
				StopAnimation ();
				Animator.SetBool ("InSwim", true);
			}
			break;
		case AnimationStates.SWIMIDLE: 
			{
				StopAnimation ();
				Animator.SetBool ("InSwimIdle", true);
			}
			break;
		case AnimationStates.WALKLEFT: 
			{
				StopAnimation ();
				Animator.SetBool ("InWalkLeft", true);
			}
			break;
		case AnimationStates.WALKRIGHT: 
			{
				StopAnimation ();
				Animator.SetBool ("InWalkRight", true);
			}
			break;
		case AnimationStates.RUN: 
			{
				StopAnimation ();
				Animator.SetBool ("InRun", true);
			}
			break;
		case AnimationStates.HIT:
			{
				StopAnimation();
				Animator.SetBool("InHit", true);
			}
			break;
		case AnimationStates.HITTOOL:
			{
				//StopAnimation();
				Animator.SetBool("InHitTool", true);
				//Break = false;
				//Timer = 0;
				Animator.SetLayerWeight(2 , 1);
				StartCoroutine(AnimationTimer());
				break;
			}
        case AnimationStates.CROUCHIDLE:
            {
                StopAnimation();
                Animator.SetBool("InCrouchIdle", true);
            }
            break;
        case AnimationStates.CROUCHFORWARD:
			{
				StopAnimation();
				Animator.SetBool("InCrouchForward", true);
			}
			break;
		case AnimationStates.CROUCHBACK:
			{
				StopAnimation();
				Animator.SetBool("InCrouchBack", true);
			}
			break;
		case AnimationStates.CROUCHRIGHT:
			{
				StopAnimation();
				Animator.SetBool("InCrouchRight", true);
			}
			break;
		case AnimationStates.CROUCHLEFT:
			{
				StopAnimation();
				Animator.SetBool("InCrouchLeft", true);
			}
			break;
		case AnimationStates.JUMP:
			{
				StopAnimation();
				Animator.SetBool("InJump", true);
			}
			break;
		case AnimationStates.PILOTHELICOPTER:
            {
				StopAnimation();
				Animator.SetBool("InPilotHelicopter", true);
            }
			break;
		case AnimationStates.IDDLESIT:
            {
				StopAnimation();
				Animator.SetBool("InSitIddle", true);
            }
			break;
		}	
	}
	void StopAnimation()
	{
		Animator.SetBool ("InIdle", false);
		Animator.SetBool ("InWalk", false);
		Animator.SetBool ("InWalkBack", false);
		Animator.SetBool ("InWalkLeft", false);
		Animator.SetBool ("InWalkRight", false);
		Animator.SetBool ("InCrouchIdle", false);
		Animator.SetBool ("InCrouchForward", false);
		Animator.SetBool ("InCrouchBack", false);
		Animator.SetBool ("InCrouchRight", false);
		Animator.SetBool ("InCrouchLeft", false);
		Animator.SetBool ("InSwim", false);
		Animator.SetBool ("InSwimIdle", false);
		Animator.SetBool ("InRun", false);
		Animator.SetBool ("InHit", false);
		//Animator.SetBool ("InHitTool", false);
		Animator.SetBool ("InJump", false);
		Animator.SetBool ("InPilotHelicopter", false);
		Animator.SetBool ("InSitIddle", false);
	}
	private IEnumerator AnimationTimer()
    {
		Timer = false;
		value = 1;
		yield return new WaitForSeconds(1);

		if (!Timer)
		{
			Animator.SetBool("InHitTool", false);
			print("desativei");
			Timer = true;
		}
	}
	private void SuaveLayer()
    {
		if (Timer && FindObjectOfType<GunsControl>().GunSelected == null)
		{
			value = Mathf.Lerp(value, 0.0f, Time.fixedDeltaTime * 2);
			if (value <= 0.1f)
			{
				value = 0;
				Timer = false;
			}
			Animator.SetLayerWeight(2, value);
		}
	}
	public void Desable()
    {
		//Animator.SetBool("InHitTool", false);
		Timer = false;
		value = 1;
    }
}
