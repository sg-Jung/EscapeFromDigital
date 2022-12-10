using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QusetController : MonoBehaviour
{
    [SerializeField] GameObject mainUI;
    [SerializeField] GameObject endBack;
    [SerializeField] GameObject endText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            mainUI.SetActive(false);
            StartCoroutine(nameof(FadeIn));
            Invoke(nameof(StartFadeOut), 2f);
        }
    }

    IEnumerator FadeIn()
    {
        endBack.SetActive(true);
        Image backImg = endBack.GetComponent<Image>();

        float time = 0f;
        Color color = backImg.color;
        color.a = 0f;

        while (time <= 4f)
        {
            time += Time.deltaTime;
            color.a += Time.deltaTime / 1f;
            backImg.color = color;

            yield return null;
        }
        
    }

    void StartFadeOut()
    {
        StartCoroutine(nameof(FadeOutText));
    }

    IEnumerator FadeOutText()
    {
        endText.SetActive(true);

        float time = 0f;
        Color color = endText.GetComponent<Text>().color;
        color.a = 0f;

        while (time <= 4f)
        {
            time += Time.deltaTime;
            color.a += Time.deltaTime / 1f;
            endText.GetComponent<Text>().color = color;

            yield return null;
        }
    }
}
