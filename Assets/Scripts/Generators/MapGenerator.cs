using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerator : ScriptableObject {

    public abstract void Generate();
    public abstract void Init(TileMapManager mManager, DefaultRNG rng, EntityMetaDataSO EntityData);
}
