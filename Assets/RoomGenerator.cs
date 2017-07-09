using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.Helper;

[CreateAssetMenu]
public class RoomGenerator : ScriptableObject {

    public int weight;
    public SizeConstraint sizeConstraint;
    public SpawnRangeConstraint spawnRangeConstraint;

    public virtual bool isValid(Circle c, LayoutGenerator layout)
    {
        return 
            (sizeConstraint.useSizeConstraint ? sizeConstraint.IsValid(c, layout) : true) &&
            (spawnRangeConstraint.useSpawnRangeConstraint ? spawnRangeConstraint.IsValid(c, layout) : true);
    }

    public virtual void Generate ()
    {

    }
}

public class EmptyRoom : RoomGenerator
{
}
public class TreasureRoom : RoomGenerator
{
    public override void Generate()
    {
    }
}
