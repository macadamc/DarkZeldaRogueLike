using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReference : MonoBehaviour {

    public static GameObject player;


    void Awake()
    {
        if (player == null)
            player = GameObject.Find("Player");
    }

}
