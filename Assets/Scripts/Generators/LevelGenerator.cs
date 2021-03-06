﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelGenerator : MonoBehaviour {

    public int currentLevel;
    public bool SpawnShop;
    public bool SpawnBoss;

    public List<MapGenerator> levelConfigs;

    public MapGenerator CurrentConfig;

    void Start()
    {
        GameManager.GM.Rng = new DefaultRNG(System.DateTime.Now.GetHashCode());

        SceneManager.sceneLoaded += OnLevelLoad;

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            GenerateLvL();
        }
    }

    void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        GenerateLvL();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoad;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLevelLoad;
    }

    public MapGenerator CurrentLvlConfig
    {
        get { return levelConfigs[currentLevel]; }
    }

    public void GenerateLvL ()
    {
        Clear();

        CurrentLvlConfig.Generate(GameManager.GM.mapManager.map, GameManager.GM.Rng, GameManager.GM.entityMetaData, this);

        GameManager.GM.mapManager.cManager.UpdateChunks(GameManager.GM.mapManager.map, force: true);// true forces the whole map to be regenerated.

        if (currentLevel < levelConfigs.Count - 1)
        {
            currentLevel++;
        }
        
    }

    void Clear()
    {
        SpawnShop = false;
        SpawnBoss = false;

        GameManager.GM.mapManager.map.ClearData();

        if (MinSpawnConstraint.SpawnCounts != null)
        {
            MinSpawnConstraint.SpawnCounts.Clear();
        }

        GameManager.GM.InGameObjectManager.DestroyAllGameObjects();
    }
}
