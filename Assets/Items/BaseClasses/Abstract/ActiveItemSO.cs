﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItemSO : ItemSO {

    public GameObject HeldObj;

    public int baseDamage;
    public float AttackDelay;
    public float knockBack;
    public bool strafe;

    [Range(0f, 1f)]
    public float attackingMoveSpeed;

}
