using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldPos {

    public int X;
    public int Y;

    public WorldPos()
    {
        X = 0;
        Y = 0;
    }

    public WorldPos (int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        WorldPos other = obj as WorldPos;
        if (other == null) { return false; }
        if (other.X == this.X && other.Y == this.Y) { return true; }
        else { return false; }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
