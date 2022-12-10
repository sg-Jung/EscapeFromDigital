using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBullet : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] SimpleShoot gun;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            audioSource.Play();
            Invoke(nameof(SetActiveFalseAndAddBullet), 0.5f);
        }
    }

    void SetActiveFalseAndAddBullet()
    {
        gun.AddBullet();
        transform.gameObject.SetActive(false);
    }

}
