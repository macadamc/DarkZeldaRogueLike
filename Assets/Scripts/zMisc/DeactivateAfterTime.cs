using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour {

    public float time;

	void Update () {

        time -= Time.deltaTime;

        if(time <= 0)
        {
            Deactivate();
        }
		
	}

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
