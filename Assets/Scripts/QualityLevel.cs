using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityLevel : MonoBehaviour {

    public int qualityLevel;
    SpriteRenderer sprite;

	// Use this for initialization
	void Start () {

        sprite = GetComponent<SpriteRenderer>();
        CheckLevel();
	}

    public void CheckLevel()
    {
        if (sprite == null)
            return;

        if (qualityLevel > GameManager.GM.optionsManager.qualityMode)
            sprite.enabled = false;
        else
            sprite.enabled = true;
    }
}
