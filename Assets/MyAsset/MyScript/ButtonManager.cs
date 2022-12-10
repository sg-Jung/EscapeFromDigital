using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject desc;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ClickDescBtn();
        }
    }

    public void ClickStartBtn()
    {
        if (Time.timeScale != 1) Time.timeScale = 1f;
        SceneManager.LoadScene(1);
        Destroy(QuestController.Instance);
    }

    public void ClickDescBtn()
    {
        if(desc.activeSelf) desc.SetActive(false);
        else desc.SetActive(true);
    }

    public void ClickExitBtn()
    {
        Application.Quit();
    }

    public void RestartBtn()
    {
        if (Time.timeScale != 1) Time.timeScale = 1f;
        SceneManager.LoadScene(1);
        Destroy(QuestController.Instance);
    }

    public void MainBtn()
    {
        if (Time.timeScale != 1) Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        Destroy(QuestController.Instance);
    }
}
