using EvolveGames;
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
    [SerializeField] float rotateSpeed = 8f;
    [SerializeField] float attackDamage = 10f;
    [SerializeField] PlayerController target;
    [Header("Sound")]
    [SerializeField] AudioClip idleClip;
    [SerializeField] AudioClip damagedClip;
    [SerializeField] AudioClip dieClip;
    [SerializeField] AudioClip attackClip;
    bool isStop = false;
    bool isDamaged = false;
    bool isDie = false;
    bool isAttack = false;
    bool onPlayer = false;

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
            agent.destination = target.transform.position;
            OnHitPlayer();
        }
    }


    void PlaySound(AudioClip clip)
    {
        audioSource.Stop(); // 기존에 재생중인 사운드를 정지하고
        audioSource.clip = clip; // 새로운 사운드 clip으로 교체 후
        audioSource.Play(); // 사운드 재생
    }

    public void OnDamaged(float damage)
    {
        if (isDie) return;

        enemyHP -= damage;
        SetAgentState(true);

        if(enemyHP > 0f)
        {
            anim.SetBool("isHit", true);
            isDamaged = true;
            StartCoroutine(nameof(OnDamageCor));
        }
        else if(enemyHP <= 0f)
        {
            anim.SetBool("isDie", true);
            isDie = true;
            StartCoroutine(nameof(OnDieCor));
        }
    }

    void OnHitPlayer()
    {
        if(agent.remainingDistance <= agent.stoppingDistance + 0.5)
        {
            RotateToPlayer();
            Attack();
        }
    }

    void RotateToPlayer()
    {
        Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        Vector3 from = new Vector3(agent.transform.position.x, 0, agent.transform.position.z);
        Quaternion rot = Quaternion.LookRotation(to - from);

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, rot, Time.deltaTime * rotateSpeed);
    }

    void Attack()
    {
        if (!onPlayer) return;

        isAttack = true;
        SetAgentState(true);

        anim.SetBool("isAttack", true);
        StartCoroutine(nameof(OnAttackCor));
    }

    IEnumerator OnAttackCor()
    {
        PlaySound(attackClip);

        yield return new WaitForSeconds(1f);

        if (target.isEnemyInTrigger && !isDamaged) target.PlayerOnDamaged(attackDamage);

        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isAttack", false);
        SetAgentState(false);
        isAttack = false;
    }

    IEnumerator OnDamageCor()
    {
        PlaySound(damagedClip);

        yield return new WaitForSeconds(1.2f);
        anim.SetBool("isHit", false);
        isDamaged = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController")) onPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController")) onPlayer = false;
    }
}
