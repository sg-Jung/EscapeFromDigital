using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class SlaveOutline : MonoBehaviour
{
    [SerializeField] Outline[] outlines;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            foreach (Outline s in outlines)
            {
                s.eraseRenderer = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            foreach (Outline s in outlines)
            {
                s.eraseRenderer = true;
            }
        }
    }
}
