using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

    public GameObject obj;

    [HideInInspector]
    public float objOffset;
    public float baseHeight;

    public float gravity;
    public float yVel;
    [Range(0f,1f)]
    public float dampening = 0.5f;
    public float stopCutoffVel = 0.01f;


    void Update()
    {
        //shadow.transform.position = transform.position + new Vector3(0f, shadowOffset);

        if (PauseManager.gamePaused)
            return;

        if (objOffset+yVel > baseHeight)
        {
            yVel -= Time.deltaTime * gravity;
            objOffset += yVel;
        }
        else
        {
            yVel = -yVel * dampening;

            if (yVel < stopCutoffVel)
            {
                objOffset = baseHeight;
            }
            else
            {
                objOffset = yVel;
            }
        }


        obj.transform.position = transform.position + new Vector3(0f, baseHeight+objOffset);
    }



}
