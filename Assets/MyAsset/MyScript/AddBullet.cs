using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBullet : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] SimpleShoot gun;
    bool isEnter = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && !isEnter)
        {
            isEnter = true;
            audioSource.Play();
            gun.AddBullet();
            Debug.Log("enter add");

            Invoke(nameof(SetActiveFalseAndAddBullet), 0.5f);
        }
    }

    void SetActiveFalseAndAddBullet()
    {
        transform.gameObject.SetActive(false);
    }

}
