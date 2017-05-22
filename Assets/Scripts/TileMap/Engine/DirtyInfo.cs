using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DirtyInfo
{
    public MapChunk chunk;
    public string layerName;
    public string subLayerName;

    public DirtyInfo(MapChunk Chunk, string LayerName)
    {
        chunk = Chunk;
        layerName = LayerName;
    }

    public override bool Equals(object Obj)
    {
        DirtyInfo other = (DirtyInfo)Obj;
        return chunk.pos == other.chunk.pos && layerName == other.layerName;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
