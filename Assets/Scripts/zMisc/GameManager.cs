﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ShadyPixel.SaveLoad;

public class GameManager : MonoBehaviour {

    public static GameManager GM;
    public ProgressionManager progressionManager;
    public PlayerReference player;

    public ItemPanel itemPanel;

    public GameObject deathScreen;

    [HideInInspector]
    public TileMapManager mapManager;
    [HideInInspector]
    public GameObject InGameObjects;
    public InGameObjectManager InGameObjectManager;
    public DefaultRNG Rng;
    public EntityMetaDataSO entityMetaData;
    //public EnemyMetaDataSO enemyMetadata;
    public ItemsMetaData itemsMetaData;
    [HideInInspector]
    public OptionsManager optionsManager;
    [HideInInspector]
    public PauseManager pauseManager;
    [HideInInspector]
    public AudioManager audioManager;

    public LevelGenerator levelGenerator;


    public void Awake()
    {
        if (GM == null)
            GM = this;
        else
        if (GM != this)
            Destroy(gameObject);

        progressionManager = GetComponent<ProgressionManager>();
        player = GetComponent<PlayerReference>();

        DontDestroyOnLoad(gameObject);

        mapManager = GetComponent<TileMapManager>();
        optionsManager = GetComponent<OptionsManager>();
        pauseManager = GetComponent<PauseManager>();
        levelGenerator = GetComponent<LevelGenerator>();
        audioManager = GetComponent<AudioManager>();

        InGameObjects = GameObject.Find("InGameObjects");
        InGameObjectManager = InGameObjects.GetComponent<InGameObjectManager>();
        if (InGameObjects == null || InGameObjectManager == null)
        {
            throw new Exception("No object Named InGameObjects in the scene...");
        }

    }

    void Start()
    {
        SaveLoadManager.slm.LoadGame();
    }

    public void PlayerDeath()
    {
        deathScreen.SetActive(true);
        AchievementManager.am.DebugPrint();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

}
