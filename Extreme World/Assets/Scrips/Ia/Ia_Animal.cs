using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class Ia_Animal : MonoBehaviourPunCallbacks
{
    public NavMeshAgent Agent;
    public Animator Animator;
    public Transform PositionGroup;
    [HideInInspector]public Transform PlayerFollow;
    public int Vida = 50;
    public int Dano = 20;
    public float intensy = 2;
    public float AttackTime = 1;
    public int SpeedWalk = 3;
    public int SpeedRun = 10;
    public float Radius = 50;
    public int MinWaitIdle = 5;
    public int MaxWaitIdle = 15;
    public int MinDistance = 10;
    public int MaxDistance = 50;
    public bool Pacifico, Parei, Look;
    public int Id;

    private float WaitTime, time, timerAtaque;
    private bool Wait, AnimalLook, PodeAtacar;
    private string enviei;
    private Transform Player;
    private Status status;
    void Start()
    {
        //Id = FindObjectOfType<SlotScalerItem>().IdViewAnimal + 1;
        //FindObjectOfType<SlotScalerItem>().IdViewAnimal = Id;

        status = FindObjectOfType<Status>();
        // verifica se o Player esta proximo do Agent
        StartCoroutine(DistanceAnimal());
        Agent.speed = SpeedWalk;
        StartCoroutine(SetDestino(0.5f));
        if (PhotonNetwork.IsConnected) 
            Id = GetComponent<PhotonView>().ViewID;
    }
   
    void Update()
    {
        if (Agent.isStopped)
            print("Parei");
        // Acha o Player
        if (MouseLook.player != null)
            Player = MouseLook.player;

        // Timer de espera
        /*time += Time.deltaTime;
        if (time < WaitTime)
            Wait = true;

        else if (Wait)
        {
            Vector3 Destino;
            Wait = false;
            if (PositionGroup == null)
                Destino = RandomPositionAI(Radius, transform.position);
            else
                Destino = RandomPositionAI(Radius, PositionGroup.transform.position);
            
            Agent.SetDestination(Destino);
            SetAnimation("InWalk");
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                servidor.Server.RPC("IAMoving", RpcTarget.Others, Destino, Id, "InWalk", false);
            }
        }*/

        // Quando o Player esta proximo demais do Agente
        if (AnimalLook)
        {
            string anim;
            if (Parei && !Pacifico)
            {
                anim = "InIdle";
                // Faz o Agente olhar para o alvo
                RotationIA(Player);
                /*if (PhotonNetwork.IsConnected)
                {
                    servidor.Server.RPC("AnimalRotation", RpcTarget.Others, Id, transform.rotation);
                }*/
            }
            else
                anim = "InRun";
            SetAnimation(anim);

            if (Pacifico)
            {
                //Agent.SetDestination(RandomPositionAI(Radius, transform.position, true));
            }

            else
            {
                /*if (PhotonNetwork.IsConnected && anim != enviei)
                {
                    enviei = anim;
                    servidor.Server.RPC("FollowPlayer", RpcTarget.Others, Player.GetComponent<PhotonView>().ViewID, Id, anim);
                }*/
                Agent.SetDestination(Player.transform.position);
            }
        }

        // Verifica se o Agente parou
        Vector3 vel = Agent.velocity;
        if (vel == new Vector3(0.0f, 0.0f, 0.0f) && !Wait)
        {
            StartCoroutine(Stoped(vel));
        }

        Attack();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            servidor.Server.RPC("SetPositionIA", newPlayer, Id, transform.position, Agent.destination);
        }
    }
    private void Attack()
    {
        timerAtaque += Time.deltaTime;
        if (PodeAtacar && timerAtaque >= AttackTime)
        {
            if (Parei)
            {
                SetAnimation("InAttack");
                /*if (PhotonNetwork.IsConnected)
                {
                    servidor.Server.RPC("IAMoving", RpcTarget.Others, Vector3.zero, Id, "InAttack", true);
                }*/
            }
            else
            {
                SetAnimation("InWalkAttack");
                /*if (PhotonNetwork.IsConnected)
                {
                    servidor.Server.RPC("IAMoving", RpcTarget.Others, Vector3.zero, Id, "InWalkAttack", true);
                }*/
            }
            
            status.Hit(Dano);
            timerAtaque = 0;
        }
    }
    public void RotationIA(Transform Player)
    {
        Vector3 Direcao = new Vector3(Player.position.x, transform.position.y, Player.position.z) - transform.position;
        Quaternion rotacao = Quaternion.LookRotation(Direcao);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotacao, 3 * Time.deltaTime);
    }
    public Vector3 RandomPositionAI (float Radius, Vector3 Target, bool Fugir=false)
    {
        Vector3 RandomDirection;
        if (Fugir)
            RandomDirection = Random.onUnitSphere * Radius;
        else
            RandomDirection = Random.insideUnitSphere * Radius;
        RandomDirection += Target;
        NavMeshHit hit;
        Vector3 FinalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(RandomDirection, out hit, Radius, 1))
        {
            FinalPosition = hit.position;
        }
        return FinalPosition;
    }
    private IEnumerator Stoped(Vector3 vel)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            yield return new WaitForSeconds(.15f);
            if (vel == Agent.velocity)
            {
                Parei = true;
                if (!PodeAtacar && !AnimalLook)
                {
                    WaitTime = Random.RandomRange(MinWaitIdle, MaxWaitIdle);
                    SetAnimation("InIdle");

                   /* if (PhotonNetwork.IsConnected && !Look)
                        servidor.Server.RPC("IAMoving", RpcTarget.Others, Vector3.zero, Id, "InIdle", true);*/

                    StartCoroutine(SetDestino(WaitTime));
                }
            }
            else
                Parei = false;
        }
    }
    private IEnumerator DistanceAnimal()
    {
        if (Player != null)
        {
            float distance = Vector3.Distance(transform.position, Player.transform.position);
            // Agent viu você
            if (distance < MinDistance && !Look)
            {
                AnimalLook = true;
                Wait = false;
                Agent.speed = SpeedRun;
                if (PhotonNetwork.IsConnected)
                {
                    servidor.Server.RPC("AnimaLook", RpcTarget.All, Id, SpeedRun, true);
                    servidor.Server.RPC("FollowPlayer", RpcTarget.Others, Player.GetComponent<PhotonView>().ViewID, Id, "InRun");
                }
            }

            // Agent quer atacar você
            if (distance <= 3 && AnimalLook && !Pacifico)
                PodeAtacar = true;
            else
                PodeAtacar = false;

            // Agent desiste de você
            if (distance > MaxDistance && AnimalLook)
            {
                AnimalLook = false;
                Agent.speed = SpeedWalk;

                WaitTime = Random.RandomRange(MinWaitIdle, MaxWaitIdle);
                SetAnimation("InIdle");
                if (PhotonNetwork.IsConnected)
                {
                    servidor.Server.RPC("AnimaLook", RpcTarget.All, Id, SpeedWalk, false);
                   // servidor.Server.RPC("IAMoving", RpcTarget.Others, Vector3.zero, Id, "InIdle", true);
                }
                StartCoroutine(SetDestino(WaitTime));
            }
        }

        yield return new WaitForSeconds(0.1f);
        yield return DistanceAnimal();
    }
    private IEnumerator SetDestino(float WaitTime)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            Wait = true;

            yield return new WaitForSeconds(WaitTime);

            Vector3 Destino;
            Wait = false;

            if (PositionGroup == null)
                Destino = RandomPositionAI(Radius, transform.position);
            else
                Destino = RandomPositionAI(Radius, PositionGroup.transform.position);

            Agent.SetDestination(Destino);
            SetAnimation("InWalk");

            if (PhotonNetwork.IsConnected)
                servidor.Server.RPC("IAMoving", RpcTarget.Others, Destino, Id, "InWalk", false);
        }
    }
    public IEnumerator Following()
    {
        Agent.SetDestination(PlayerFollow.position);

        if (Parei)
            RotationIA(PlayerFollow);

        yield return new WaitForSeconds(0.05f);
        yield return Following();
    }
    public void Hit(int Dano, bool Net = false)
    {
        Vida -= Dano;
        if (PhotonNetwork.IsConnected && Net)
        {
            servidor.Server.RPC("DamageAnimal", RpcTarget.Others, GetComponent<PhotonView>().ViewID, Dano);

            if (Vida <= 0 && PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(gameObject);
            
        }
        else if (Vida <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetAnimation(string Animation)
    {
        Animator.SetBool("InIdle", false);
        Animator.SetBool("InWalk", false);
        Animator.SetBool("InWalkBack", false);
        Animator.SetBool("InTurnLeft", false);
        Animator.SetBool("InTurnRight", false);
        Animator.SetBool("InAttack", false);
        Animator.SetBool("InWalkAttack", false);
        Animator.SetBool("InRun", false);

        Animator.SetBool(Animation, true);
    }
}
