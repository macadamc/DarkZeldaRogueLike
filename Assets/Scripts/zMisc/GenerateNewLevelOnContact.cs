using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerateNewLevelOnContact : MonoBehaviour {


	void Awake () {
        GetComponent<Collider2D>().isTrigger = true;
	}
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.GM.levelGenerator.GenerateLvL();
        }
    }
}
