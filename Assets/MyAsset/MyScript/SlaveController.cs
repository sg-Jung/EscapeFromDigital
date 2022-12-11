using cakeslice;
using EvolveGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class SlaveController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    [SerializeField] Transform target;
    [SerializeField] PlayerController pc;
    [SerializeField] bool onPlayer = false;
    public GameObject portal;
    public GameObject pressFImg;
    public Text mission;
    bool playerSaveHer = false;
    bool isStop = false;
    bool isWalk = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (onPlayer && !playerSaveHer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                mission.color = Color.yellow;
                mission.text = "출구를 찾아 탈출하라";

                if (pressFImg.activeSelf) pressFImg.SetActive(false);

                playerSaveHer = true;
                pc.isSlaveSafe = true;
                portal.SetActive(true);
                anim.SetBool("isRun", true);
            }
        }

        if (playerSaveHer)
        {
            agent.destination = target.position;

            if (agent.remainingDistance < agent.stoppingDistance + 0.5)
            {
                anim.SetBool("isRun", false);
                SetAgentState(true);
            }
            else
            {
                anim.SetBool("isRun", true);
                SetAgentState(false);
            }
        }
    }

    void SetAgentState(bool flag)
    {
        isStop = flag;
        agent.isStopped = flag;

        agent.updatePosition = !flag;
        agent.updateRotation = !flag;

        if (flag) agent.velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !playerSaveHer)
        {
            pressFImg.SetActive(true);
            onPlayer = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController") && !playerSaveHer)
        {
            onPlayer = false;
            pressFImg.SetActive(false);
        }
    }
}
