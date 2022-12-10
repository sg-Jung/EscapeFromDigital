using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessQuest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController")) QuestController.Instance.GameEnd();
    }

}
