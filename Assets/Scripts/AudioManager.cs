using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip startBgm;

    void Start()
    {
        PlayBGM(startBgm);
    }


    public void PlayBGM(AudioClip bgm)
    {
        if (bgmSource.clip == null || bgmSource.clip != bgm)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInOutMusic(bgm));
        }
    }
    public void PlaySFX(AudioClip sfx)
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
            sfxSource.clip = sfx;
            sfxSource.Play();
        }
    }
    public void PlaySFX(AudioClip sfx, AudioSource source)
    {
        source.Stop();
        source.clip = sfx;
        source.Play();
    }

    public IEnumerator FadeInOutMusic(AudioClip bgm)
    {
        while(bgmSource.volume > 0)
        {
            bgmSource.volume -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);
        bgmSource.clip = bgm;
        bgmSource.Play();

        while (bgmSource.volume < 1)
        {
            bgmSource.volume += Time.deltaTime;
            yield return null;
        }

        yield return null;

    }
}
