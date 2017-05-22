using UnityEngine;

public class TileMapManager : MonoBehaviour {

    public sMap map;
    [HideInInspector]
    public ChunkManager cManager;

	// Use this for initialization
	public void Awake ()
    {
        map.cManager = cManager;
        cManager.map = map;

        if (cManager.mapChunksGameObject == null)
        {
            map.ClearData();
            cManager.chunkPointers.Clear();
            cManager.mapChunks.Clear();
            cManager.mapChunksGameObject = new GameObject("MapChunks");
            cManager.mapChunksGameObject.transform.position = new Vector3(-map.width / 2, -map.height / 2, 0);
            cManager.SpawnChunks();
        }
    }

    void LateUpdate ()
    {
        cManager.UpdateChunks();
    }
	
}

