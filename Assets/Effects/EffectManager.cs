using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

    void Start()
    {
        //Poison p = new Poison(3, 20, GameObject.Find("Player").GetComponent<Entity>());
        new Slow(4, .75f, GameObject.Find("Player").GetComponent<Entity>());
    }
}
