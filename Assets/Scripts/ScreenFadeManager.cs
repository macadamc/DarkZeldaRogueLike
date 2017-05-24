using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFadeManager : MonoBehaviour {

    public CanvasGroup screenOverlay;

    public static ScreenFadeManager sfm;

	// Use this for initialization
	void Start () {

        sfm = this;

        SceneManager.sceneLoaded += FadeIn;
		
	}

    void OnDisable()
    {
        SceneManager.sceneLoaded -= FadeIn;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= FadeIn;
    }

    // Update is called once per frame
    void Update () {
		
	}

    void FadeIn(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(Fade(0f, 1f,0.5f));
    }

    public IEnumerator Fade(float alpha, float time)
    {
        while (screenOverlay.alpha != alpha)
        {
            if (screenOverlay.alpha < alpha)
            {
                screenOverlay.alpha += time * Time.deltaTime;
            }
            else
            {
                screenOverlay.alpha -= time * Time.deltaTime;
            }

            if (Mathf.Approximately(screenOverlay.alpha, alpha))
                screenOverlay.alpha = alpha;

            yield return new WaitForEndOfFrame();
        }

    }

    public IEnumerator Fade(float alpha, float time, float delay)
    {
        yield return new WaitForSeconds(delay);

        while (screenOverlay.alpha != alpha)
        {
            if (screenOverlay.alpha < alpha)
            {
                screenOverlay.alpha += time * Time.deltaTime;
            }
            else
            {
                screenOverlay.alpha -= time * Time.deltaTime;
            }

            if (Mathf.Approximately(screenOverlay.alpha, alpha))
                screenOverlay.alpha = alpha;

            yield return new WaitForEndOfFrame();
        }

    }
}
