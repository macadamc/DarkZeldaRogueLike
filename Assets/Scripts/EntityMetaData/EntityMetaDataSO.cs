using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityMetaDataSO : ScriptableObject
{
    public List<EntityMetaData> passivesData;
    [HideInInspector]
    public List<string> passiveKeys;

    public List<EnemyMetaData> enemysData;
    [HideInInspector]
    public List<string> enemyKeys = new List<string>();

    public List<EnemyMetaData> GetEnemyByFirstLvl(int firstLvl)
    {
        IEnumerable<EnemyMetaData> query =
                    from entity in enemysData
                    where entity.firstLvl == firstLvl
                    select entity;

        return query.ToList<EnemyMetaData>();
    }
    public List<EnemyMetaData> GetEnemyByFirstLvlRange(int start, int end)
    {
        IEnumerable<EnemyMetaData> query =
                    from entity in enemysData
                    where entity.firstLvl >= start && entity.firstLvl <= end
                    select entity;

        return query.ToList<EnemyMetaData>();
    }

    public List<EntityMetaData> GetPassiveByFirstLvl(int firstLvl)
    {
        IEnumerable<EntityMetaData> query =
                    from entity in passivesData
                    where entity.firstLvl == firstLvl
                    select entity;

        return query.ToList<EntityMetaData>();
    }
    public List<EntityMetaData> GetPassiveByFirstLvlRange(int start, int end)
    {
        IEnumerable<EntityMetaData> query =
                    from entity in passivesData
                    where entity.firstLvl >= start && entity.firstLvl <= end
                    select entity;

        return query.ToList<EntityMetaData>();
    }
}