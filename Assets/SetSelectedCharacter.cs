using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSelectedCharacter : MonoBehaviour {

    public GameObject objectToAnimate;
    public CharacterClass charClass;
    Animator anim;

	// Use this for initialization
	void Start () {
        anim = objectToAnimate.GetComponent<Animator>();

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSelected()
    {
        SelectedClassManager.scm.SetSelectedObject(anim, charClass);
    }

}
