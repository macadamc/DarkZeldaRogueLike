using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModifiers
{
    public float damage;
    public float maxSpeed;
    public float attackDelay;

    public StatModifiers ()
    {
        damage = 0;

        maxSpeed = 0;
        attackDelay = 0;
    }
}
