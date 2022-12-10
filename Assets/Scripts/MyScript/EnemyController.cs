using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    CharacterController cc;
    AudioSource audioSource;
    [SerializeField] float enemyHP = 100f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform target;
    [Header("Sound")]
    [SerializeField] AudioClip idleClip;
    [SerializeField] AudioClip damagedClip;
    [SerializeField] AudioClip dieClip;
    bool isStop = false;
    bool isDie = false;

    void Start()
    {
        anim = GetComponent<Animator>();    
        agent = GetComponent<NavMeshAgent>();
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

    }

    void FixedUpdate()
    {
        if (!isStop && !isDie && enemyHP > 0)
        {
            agent.destination = target.position;
        }
    }


    void PlaySound(AudioClip clip)
    {
        audioSource.Stop(); // ������ ������� ���带 �����ϰ�
        audioSource.clip = clip; // ���ο� ���� clip���� ��ü ��
        audioSource.Play(); // ���� ���
    }

    public void OnDamaged(float damage)
    {
        if (isDie) return;

        enemyHP -= damage;
        SetAgentState(true);

        if(enemyHP > 0f)
        {
            anim.SetBool("isHit", true);
            StartCoroutine(nameof(OnDamageCor));
        }
        else if(enemyHP <= 0f)
        {
            anim.SetBool("isDie", true);
            isDie = true;
            StartCoroutine(nameof(OnDieCor));
        }
    }

    IEnumerator OnDamageCor()
    {
        PlaySound(damagedClip);

        yield return new WaitForSeconds(1.2f);
        anim.SetBool("isHit", false);
        SetAgentState(false);
    }

    IEnumerator OnDieCor()
    {
        PlaySound(dieClip);
        audioSource.loop = false;
        agent.ResetPath();

        yield return new WaitForSeconds(8f);
        Destroy(transform.gameObject);
    }

    void SetAgentState(bool flag)
    {
        isStop = flag;
        agent.isStopped = flag;

        agent.updatePosition = !flag;
        agent.updateRotation = !flag;

        if (flag) agent.velocity = Vector3.zero;
        else PlaySound(idleClip);
    }
}
