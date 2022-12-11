using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    private static QuestController instance = null;
    public static QuestController Instance
    {
        get
        {
            if (instance == null)
                return null;

            return instance;
        }
    }

    public GameObject mainUI;
    public GameObject endBack;
    public GameObject endText;
    public GameObject btn;
    GameObject player;

    bool isPause = false;
    public bool gameEnd = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        player = GameObject.FindGameObjectWithTag("GameController");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClickESC();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            GameEnd();
        }
    }

    void ClickESC()
    {
        if (!isPause)
        {
            endBack.SetActive(true);
            btn.SetActive(true);
            isPause = true;
            player.GetComponent<AudioSource>().Stop();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
        }
        else
        {
            endBack.SetActive(false);
            btn.SetActive(false);
            isPause = false;
            player.GetComponent<AudioSource>().Play();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
        }
    }

    public void GameEnd()
    {
        gameEnd = true;

        player.GetComponent<AudioSource>().Stop();
        mainUI.SetActive(false);
        StartCoroutine(nameof(FadeIn));
        Invoke(nameof(StartFadeOut), 2f);
    }

    public IEnumerator FadeIn()
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        float time = 0f;
        Color color = endText.GetComponent<Text>().color;
        color.a = 0f;

        while (time <= 2f)
        {
            time += Time.deltaTime;
            color.a += Time.deltaTime / 1f;
            endText.GetComponent<Text>().color = color;

            yield return null;
        }

        btn.SetActive(true);
    }
}
