using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour {

    [Range(0, 1)]
    public int qualityMode;

    [Range(0,1)]
    public float masterVolume;

    [Range(0.1f, 1)]
    public float controlsTransparency;

    public int muteSfx;
    public int muteBgm;

    public GameObject optionsMenu;
    public CanvasGroup touchControlCanvasGroup;

    public AudioMixer audioMix;

    public Slider qualitySlider;
    public Slider controlTransparencySlider;
    public Slider masterVolumeSlider;
    public Toggle muteSfxToggle;
    public Toggle muteBgmToggle;

    // Use this for initialization
    void Start () {

        //checks to see if option exists. if it doesn't, they have not been created yet.
        if(PlayerPrefs.HasKey("qualityMode") == false)
        {
            CreateOptions();
        }
        else
        {
            LoadOptions();
        }
    }
	
    public void LoadOptions()
    {
        qualityMode = PlayerPrefs.GetInt("qualityMode");
        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        controlsTransparency = PlayerPrefs.GetFloat("controlsTransparency");
        muteBgm = PlayerPrefs.GetInt("muteBgm");
        muteSfx = PlayerPrefs.GetInt("muteSfx");

        qualitySlider.value = qualityMode;
        masterVolumeSlider.value = masterVolume;
        controlTransparencySlider.value = controlsTransparency;

        muteSfxToggle.isOn = (muteSfx == 0) ? false : true;
        muteBgmToggle.isOn = (muteBgm == 0) ? false : true;
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetInt("qualityMode", qualityMode);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("controlsTransparency",controlsTransparency);
        PlayerPrefs.SetInt("muteBgm", muteBgm);
        PlayerPrefs.SetInt("muteSfx", muteSfx);
        PlayerPrefs.Save();
        UpdateQualityObjects();

    }

    public void CreateOptions()
    {
        PlayerPrefs.SetInt("qualityMode", 1);
        PlayerPrefs.SetFloat("masterVolume", 1);
        PlayerPrefs.SetFloat("controlsTransparency", 0.5f);
        PlayerPrefs.SetInt("muteBgm", 0);
        PlayerPrefs.SetInt("muteSfx", 0);
        PlayerPrefs.Save();
        qualitySlider.value = qualityMode;
    }

    public void UpdateQualityObjects()
    {
        QualityLevel[] qualityObjs = GameObject.FindObjectsOfType<QualityLevel>();
        foreach(QualityLevel obj in qualityObjs)
        {
            obj.CheckLevel();
        }
    }

    public void UpdateQuality()
    {
        qualityMode = (int)qualitySlider.value;
        SaveOptions();
    }

    public void UpdateVolume()
    {
        masterVolume = masterVolumeSlider.value;
        SaveOptions();
        audioMix.SetFloat("masterVolume", masterVolume);
    }

    public void UpdateTransparency()
    {
        controlsTransparency = controlTransparencySlider.value;
        SaveOptions();
    }

    public void Hide()
    {
        optionsMenu.gameObject.SetActive(false);
    }

    public void Show()
    {
        optionsMenu.gameObject.SetActive(true);
    }

    public void UpdateSfx(bool state)
    {
        muteSfx = (state == false) ? 0 : 1;
        PlayerPrefs.SetInt("muteSfx", muteSfx);
        muteSfxToggle.isOn = state;
        SaveOptions();
    }
    public void UpdateBgm(bool state)
    {
        muteBgm = (state == false) ? 0 : 1;
        PlayerPrefs.SetInt("muteBgm", muteBgm);
        muteBgmToggle.isOn = state;
        SaveOptions();
    }

    // Update is called once per frame
    void Update () {

        touchControlCanvasGroup.alpha = controlsTransparency;

        audioMix.SetFloat("masterVolume", masterVolume);

        if (muteBgm == 1)
            audioMix.SetFloat("bgmVolume", -100f);
        else
            audioMix.SetFloat("bgmVolume", -15);

        if (muteSfx == 1)
            audioMix.SetFloat("sfxVolume", -100f);
        else
            audioMix.SetFloat("sfxVolume", -10);

    }
}
