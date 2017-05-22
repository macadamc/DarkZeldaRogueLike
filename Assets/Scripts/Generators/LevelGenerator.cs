using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadyPixel.Astar;

public class LevelGenerator : MonoBehaviour {

    public List<ForestGeneratorSO> ForestConfigs;

    public MapGenerator CurrentConfig;

    void Start()
    {
        GameManager.GM.Rng = new DefaultRNG(System.DateTime.Now.GetHashCode());

        if (CurrentConfig != null)
        {
            GenerateLvL();
        }
        
    }

    public void GenerateLvL ()
    {
        GameManager.GM.mapManager.map.ClearData();

        if (MinSpawnConstraint.SpawnCounts != null)
        {
            MinSpawnConstraint.SpawnCounts.Clear();
        }
        
        GameManager.GM.InGameObjectManager.DestroyAllGameObjects();

        CurrentConfig.Init(GameManager.GM.mapManager, GameManager.GM.Rng, GameManager.GM.entityMetaData);
        CurrentConfig.Generate();

        GameManager.GM.mapManager.cManager.UpdateChunks(force: true);// true forces the whole map to be regenerated.
        GameManager.GM.GetComponent<Grid>().Start();
    }
}
