using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadyPixel.Astar;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour {

    public List<ForestGeneratorSO> ForestConfigs;

    public MapGenerator CurrentConfig;

    void Start()
    {
        GameManager.GM.Rng = new DefaultRNG(System.DateTime.Now.GetHashCode());

        SceneManager.sceneLoaded += OnLevelLoad;

    }

    void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        if (CurrentConfig != null)
        {
            GenerateLvL();
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoad;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLevelLoad;
    }

    public void GenerateLvL ()
    {
        GameManager.GM.mapManager.map.ClearData();

        if (MinSpawnConstraint.SpawnCounts != null)
        {
            MinSpawnConstraint.SpawnCounts.Clear();
        }
        
        GameManager.GM.InGameObjectManager.DestroyAllGameObjects();

        CurrentConfig.Generate(GameManager.GM.mapManager.map, GameManager.GM.Rng, GameManager.GM.entityMetaData);

        GameManager.GM.mapManager.cManager.UpdateChunks(GameManager.GM.mapManager.map, force: true);// true forces the whole map to be regenerated.
        GameManager.GM.GetComponent<Grid>().Start();
    }
}
