using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    [SerializeField] float damage = 20f;
    [SerializeField] CameraShake cs;
    [SerializeField] int maxAmmo = 30;
    [SerializeField] int curAmmo = 15;
    [SerializeField] TMP_Text ammoText;

    [Header("Audio")]
    AudioSource audioSource;
    [SerializeField] AudioClip shotClip;
    [SerializeField] AudioClip reloadClip;

    Vector3 centerPos;
    float attackDistance = 20f;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        centerPos = Camera.main.ViewportPointToRay(Vector2.one * 0.5f).direction.normalized;
        audioSource = GetComponent<AudioSource>();
        ammoText.text = curAmmo.ToString();
    }

    void Update()
    {
        //If you want a different input, change it here
        if (Input.GetButtonDown("Fire1"))
        {
            if(curAmmo <= 0)
            {
                PlaySound(reloadClip);
                return;
            }
            else
            {
                if(audioSource.clip != shotClip) audioSource.clip = shotClip;
            }
            gunAnimator.SetTrigger("Fire");
        }
    }

    void PlaySound(AudioClip clip)
    {
        audioSource.Stop(); // 기존에 재생중인 사운드를 정지하고
        audioSource.clip = clip; // 새로운 사운드 clip으로 교체 후
        audioSource.Play(); // 사운드 재생
    }

    //This function creates the bullet behavior
    void Shoot()
    {
        curAmmo--;
        curAmmo = Mathf.Clamp(curAmmo, 0, maxAmmo);
        ammoText.text = curAmmo.ToString();

        if (muzzleFlashPrefab)
        {
            StartCoroutine(nameof(OnMuzzleEffect));
        }
        StartCoroutine(cs.Shake(0.1f, 0.2f)); // 총기 반동

        if (!bulletPrefab)
        { return; }

        PlaySound(shotClip);

        TwoStepRayCast();
    }

    void TwoStepRayCast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // 화면의 중앙 좌표 (Aim 기준으로 Raycast 연산)
        ray = Camera.main.ViewportPointToRay(Vector2.one * 0.5f); // mainCamera로 보는 화면 기준 왼쪽 아래 좌표가 (0, 0), 오른쪽 위 좌표가 (1, 1)이므로 화면의 중앙 좌표는 (0.5, 0.5)이다.

        // 공격 사거리(attackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            targetPoint = hit.point;
        }
        // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * attackDistance; // ray.origin: 시작점, ray.direction: 방향
        }

        // ***
        // 첫 번째 Raycast 연산으로 얻어진 targetPoint를 목표지점으로 설정하고, 총구를 시작지점으로 하여 Raycast 연산 수행
        // ***

        Vector3 attackDirection = (targetPoint - barrelLocation.position).normalized;

        if (Physics.Raycast(barrelLocation.position, attackDirection, out hit, attackDistance))
        {
            if(hit.transform.gameObject.CompareTag("Enemy"))
                hit.transform.gameObject.GetComponent<EnemyController>().OnDamaged(damage);
        }

        GameObject tempCasing = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);

        tempCasing.GetComponent<Rigidbody>().AddForce(attackDirection * shotPower);
        Destroy(tempCasing, destroyTimer);
    }

    IEnumerator OnMuzzleEffect()
    {
        muzzleFlashPrefab.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        muzzleFlashPrefab.SetActive(false);
    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        centerPos = Camera.main.ScreenPointToRay(Input.mousePosition).direction;

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

    public void AddBullet()
    {
        curAmmo += 7;
        curAmmo = Mathf.Clamp(curAmmo, 0, maxAmmo);
        ammoText.text = curAmmo.ToString();
    }
}
