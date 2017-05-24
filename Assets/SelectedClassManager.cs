using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectedClassManager : MonoBehaviour {

    public CharacterClass curClass;
    public GameObject selectedObjSpotLight;
    public Button StartGameButton;
    [HideInInspector]
    public static SelectedClassManager scm;
    Animator curAnim, lastAnim;
    public Text classNameText, classDescriptionText;
    public GameObject playerSpawnPoint;
    public GameObject[] destroyOnLevelLoad;
    public GameObject[] turnOnWhenLevelLoad;



    // Use this for initialization
    void Awake () {

        scm = this;

        StartGameButton.interactable = false;

        UpdateClassInfo();


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSelectedObject(Animator anim, CharacterClass charClass)
    {
        lastAnim = curAnim;
        curAnim = anim;
        if (lastAnim != null)
            lastAnim.SetBool("Selected", false);
        curAnim.SetBool("Selected", true);

        curClass = charClass;

        selectedObjSpotLight.transform.position = anim.gameObject.transform.position;

        StartGameButton.interactable = true;

        UpdateClassInfo();
    }

    public void UpdateClassInfo()
    {
        if(curClass == null)
        {
            classNameText.text = "Choose a character";
            classDescriptionText.text = "";
        }
        else
        {
            classNameText.text = curClass.className;
            classDescriptionText.text = curClass.description;
        }

    }

    public void StartGame()
    {

        StartCoroutine(StartGameCoroutine());


    }

    public IEnumerator StartGameCoroutine()
    {
        ScreenFadeManager.sfm.StartCoroutine(ScreenFadeManager.sfm.Fade(1f,1f));
        yield return new WaitForSeconds(1.5f);

        foreach(GameObject go in destroyOnLevelLoad)
        {
            Destroy(go);
        }

        foreach(GameObject go in turnOnWhenLevelLoad)
        {
            go.SetActive(true);
        }
        GameObject playerObj = Instantiate(curClass.playerPrefab, playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation);
        SceneManager.LoadSceneAsync("scene1", LoadSceneMode.Additive);

        foreach (GameObject go in destroyOnLevelLoad)
        {
            Destroy(go);
        }

    }

}
