using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {

    public static bool gamePaused; 
    public GameObject pauseScreen;

    public void TogglePause()
    {
        if (!gamePaused)
            Pause();
        else
            UnPause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        gamePaused = true;
        pauseScreen.SetActive(true);
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        gamePaused = false;
        pauseScreen.SetActive(false);
    }

    public IEnumerator HitPause(float duration)
    {
        gamePaused = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        gamePaused = false;

    }

}
