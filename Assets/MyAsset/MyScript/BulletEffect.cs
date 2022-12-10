using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    public Outline outline;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController")) outline.eraseRenderer = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController")) outline.eraseRenderer = true;
    }
}
