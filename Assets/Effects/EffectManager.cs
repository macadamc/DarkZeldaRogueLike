using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

    void Start()
    {
        Entity p = GameObject.Find("Player").GetComponent<Entity>();

        new Slow(10, .75f, p);
        new Poison(2, 10, p);
    }
}
