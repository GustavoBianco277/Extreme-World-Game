using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Movimentacao : MonoBehaviour {

	public Animacoes animationController;
	private Agachar agachar;
	private Rigidbody rigid;
	private GunsControl GC;
	private float velocidade;
	public float MaxVelocityWalk = 2, MaxVelocityRun = 5, MaxVelocityCrouch = 1, MaxVelocitySwim = 1;
	public int ID;
	public float VelocidadeCorrendo, VelocidadeAndando, VelocidadeAgachado, VelocidadeNadando, ForcaPulo;
	public bool PodeNadar, PodePular, OffLine, Correndo, Pulo, Dirigindo, Enabled;
	public ConstantForce gravidade;
	private PhotonView Net;
	private ConfigTeclas Keys;
	public Transform Mao, HolderGun;
	public Status Status;
	public AudioSource FootStep, Swim;
	[HideInInspector] public GameObject GunUsing, LastGun;
	[HideInInspector] public Transform VehicleUsing;

	private void OnCollisionStay(Collision collision)
	{
		if (!Dirigindo)
		{
            PodePular = true;
			if (collision.gameObject.tag == "Agua")
			{
				PodeNadar = true;
			}
			/*else if (FootStep != FootStepGround)
			{
				FootStep = FootStepGround;
			}*/
		}
	}
	private void OnCollisionExit(Collision collision)
	{		
		PodeNadar = false;
		PodePular = false;
	}
	void Awake()
	{
		animationController = GetComponent<Animacoes> ();
		agachar = GetComponent<Agachar>();
		rigid = GetComponent<Rigidbody>();
		GC = FindObjectOfType<GunsControl>();
		Net = GetComponent<PhotonView>();
		Status = FindObjectOfType<Status>();
		FootStep = GetComponent<AudioSource>();
		Keys = FindObjectOfType<ConfigTeclas>();
	}
	void Update()
	{
		if (Net.IsMine && Enabled || OffLine && Enabled)
		{
			LimityVelocity();

			if (Input.GetKeyDown("c") && !Dirigindo && agachar.TempoAgachar)
            {
				agachar.EstaAgachado = !agachar.EstaAgachado;
				StartCoroutine(agachar.AgacharFunc(agachar.EstaAgachado));
            }
			// Andando
			if (!PodeNadar && !Dirigindo && !agachar.EstaAgachado)
			{
                if (Swim.isPlaying)
                    Swim.Stop();

                if (Input.GetKeyUp(Keys.Dict["MoveForward"].Key) || Input.GetKeyUp(Keys.Dict["MoveBack"].Key) || Input.GetKeyUp(Keys.Dict["MoveRight"].Key) || Input.GetKeyUp(Keys.Dict["MoveLeft"].Key) || Input.GetKeyUp("c") || Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
                {
                    //StartCoroutine(Drag(drag));
                    // Idle
                    animationController.PlayAnimation(AnimationStates.IDLE);

                    if (FootStep.isPlaying)
                        FootStep.Stop();

                    if (GC.GunSelected != null)
                    {
                        GC.transform.GetComponent<Animator>().SetBool("Walk", false);
                        GC.Target.GetComponent<Animator>().SetBool("Walk", false);
                    }

                    if (Correndo)
                    {
                        Correndo = false;
                        FootStep.pitch = 1.3f;
                        if (GC.GunSelected != null)
                        {
                            GC.Target.GetComponent<Animator>().SetBool("Run", false);
                            GC.transform.GetComponent<Animator>().SetBool("Run", false);
                        }
                    }
                }
                if (Input.anyKey)
				{
					if (Input.GetKey(Keys.Dict["MoveForward"].Key) && Input.GetKey(KeyCode.LeftShift) && !Status.semEstamina && !GC.Reload && !GC.IsScoped)
					{
                        if (!FootStep.isPlaying)
                            FootStep.Play();
                        Correndo = true;
						FootStep.pitch = 2;
						velocidade = VelocidadeCorrendo;
						animationController.PlayAnimation(AnimationStates.RUN);
						rigid.AddForce(transform.forward * velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);
						if (GC.GunSelected != null)
						{
							GC.Target.GetComponent<Animator>().SetBool("Run", true);
							GC.Target.GetComponent<Animator>().SetBool("Walk", false);
							GC.transform.GetComponent<Animator>().SetBool("Run", true);
							GC.transform.GetComponent<Animator>().SetBool("Walk", false);
						}
					}
					else if (Correndo)
					{
						Correndo = false;
						FootStep.pitch = 1.3f;
						if (GC.GunSelected != null)
						{
							GC.Target.GetComponent<Animator>().SetBool("Run", false);
							GC.transform.GetComponent<Animator>().SetBool("Run", false);
						}
					}
					else if (Input.GetKey(Keys.Dict["MoveForward"].Key))
					{
						// Walk Forward
						velocidade = VelocidadeAndando;
						animationController.PlayAnimation(AnimationStates.WALK);
						rigid.AddForce(transform.forward * velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);

						if (!FootStep.isPlaying)
							FootStep.Play();

						if (GC.GunSelected != null)
						{
							GC.transform.GetComponent<Animator>().SetBool("Walk", true);
							GC.Target.GetComponent<Animator>().SetBool("Walk", true);
						};
					}

					else if (Input.GetKey(Keys.Dict["MoveBack"].Key))
					{
						// Walk Back
						velocidade = VelocidadeAndando;
						animationController.PlayAnimation(AnimationStates.WALKBACK);
						rigid.AddForce(transform.forward * -velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);

						if (!FootStep.isPlaying)
							FootStep.Play();

						if (GC.GunSelected != null)
						{
							GC.transform.GetComponent<Animator>().SetBool("Walk", true);
							GC.Target.GetComponent<Animator>().SetBool("Walk", true);
						}
					}
					if (Input.GetKey(Keys.Dict["MoveLeft"].Key))
					{
						// Walk Left
						velocidade = VelocidadeAndando;
						animationController.PlayAnimation(AnimationStates.WALKLEFT);
						rigid.AddForce(transform.right * -velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);

						if (!FootStep.isPlaying)
							FootStep.Play();

						if (GC.GunSelected != null)
						{
							GC.transform.GetComponent<Animator>().SetBool("Walk", true);
							GC.Target.GetComponent<Animator>().SetBool("Walk", true);
						}
					}
					if (Input.GetKey(Keys.Dict["MoveRight"].Key))
					{
						// Walk Right
						velocidade = VelocidadeAndando;
						animationController.PlayAnimation(AnimationStates.WALKRIGHT);
						rigid.AddForce(transform.right * velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);

						if (!FootStep.isPlaying)
							FootStep.Play();

						if (GC.GunSelected != null)
						{
							GC.transform.GetComponent<Animator>().SetBool("Walk", true);
							GC.Target.GetComponent<Animator>().SetBool("Walk", true);
						}
					}

					if (Input.GetKeyDown(KeyCode.Space) && PodePular && !Pulo)
					{
						// Jump
						if (PodePular && !Pulo)
						{
							Pulo = true;
							animationController.PlayAnimation(AnimationStates.JUMP);
							rigid.AddForce(transform.up * ForcaPulo * rigid.mass, ForceMode.Impulse);
							StartCoroutine(Jump());
						}
					}
				}

				/*else if (Input.GetMouseButtonDown(0))
					animationController.PlayAnimation(AnimationStates.HITTOOL);*/

			}
			
			// Agachado
			else if (agachar.EstaAgachado && !Dirigindo && !PodeNadar)
			{
				if (Swim.isPlaying)
					Swim.Stop();

				velocidade = VelocidadeAgachado;
				if (Input.GetKey("w"))
				{
					// Forward
					animationController.PlayAnimation(AnimationStates.CROUCHFORWARD);
					rigid.AddForce(transform.forward * velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);
				}

				else if (Input.GetKey("a"))
				{
					// Back
					animationController.PlayAnimation(AnimationStates.CROUCHLEFT);
					rigid.AddForce(transform.right * -velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);
				}

				else if (Input.GetKey("d"))
				{
					// Right
					animationController.PlayAnimation(AnimationStates.CROUCHRIGHT);
					rigid.AddForce(transform.right * velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);
				}

				else if (Input.GetKey("s"))
				{
					//Left
					animationController.PlayAnimation(AnimationStates.CROUCHBACK);
					rigid.AddForce(transform.forward * -velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);
				}

				else
				{
                    if (FootStep.isPlaying)
                        FootStep.Stop();
                    animationController.PlayAnimation(AnimationStates.CROUCHIDLE);
				}
			}

			// Nadando
			else if (PodeNadar && !agachar.EstaAgachado && !Dirigindo)
			{
                if (FootStep.isPlaying)
                    FootStep.Stop();
                if (!Swim.isPlaying)
                    Swim.Play();

                velocidade = VelocidadeNadando;
				if (Input.GetKey("w"))
				{
                    
                    animationController.PlayAnimation(AnimationStates.SWIM);
					rigid.AddForce(transform.forward * velocidade * Time.deltaTime * rigid.mass, ForceMode.Force);
				}
				else
					animationController.PlayAnimation(AnimationStates.SWIMIDLE);
			}
		}
    }
	private IEnumerator Jump()
    {
		yield return new WaitForSeconds(1.5f);
		Pulo = false;
	}
	private IEnumerator Drag(float value = 1.5f)
    {
		if (PodePular)
			rigid.velocity = rigid.velocity - rigid.velocity / value;

		yield return new WaitForEndOfFrame();
		StartCoroutine(Drag(value));
	}
	private void LimityVelocity()
    {
		float Velocity;
		if (Correndo)
			Velocity = MaxVelocityRun;

		else if (agachar.EstaAgachado)
			Velocity = MaxVelocityCrouch;

		else if (PodeNadar)
			Velocity = MaxVelocitySwim;

		else
			Velocity = MaxVelocityWalk;

		if (rigid.velocity.x > Velocity)
		{
			rigid.velocity = new Vector3(Velocity, rigid.velocity.y, rigid.velocity.z);
		}
		if (rigid.velocity.z > Velocity)
		{
			rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, Velocity);
		}
		if (rigid.velocity.x < Velocity * -1)
		{
			rigid.velocity = new Vector3(Velocity * -1, rigid.velocity.y, rigid.velocity.z);
		}
		if (rigid.velocity.z < Velocity * -1)
		{
			rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, Velocity * -1);
		}
	}
}
