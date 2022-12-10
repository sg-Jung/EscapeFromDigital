using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject desc;

    public void ClickStartBtn()
    {
        SceneManager.LoadScene(1);
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
        SceneManager.LoadScene(1);
    }

    public void MainBtn()
    {
        SceneManager.LoadScene(0);
    }
}
