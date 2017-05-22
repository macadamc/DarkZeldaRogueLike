using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraSizeToCamera : MonoBehaviour {

    public Camera targetCam;
    public Camera thisCam;

	// Update is called once per frame
	void Update () {

        thisCam.orthographicSize = targetCam.orthographicSize;
		
	}
}
