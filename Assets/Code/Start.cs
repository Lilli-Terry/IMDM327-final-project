using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public void Chonk()
    {
        SceneManager.LoadScene("CHONK");
        Time.timeScale = 1f;
        Pause.isPaused = false;
    }

    public void Minecraft()
    {
        SceneManager.LoadScene("Minecraft");
        Time.timeScale = 1f;
        Pause.isPaused = false;
    }

    public void Real()
    {
        SceneManager.LoadScene("Real");
        Time.timeScale = 1f;
        Pause.isPaused = false;
    }

    public void main()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
