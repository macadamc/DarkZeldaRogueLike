using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour {

    public GameProgression gameProgression;

    public string keyName;
    public string keyValue;

    public int progressionCount = 0;

    public void ResetFields()
    {
        keyName = null;
        keyValue = null;
        progressionCount = gameProgression.progressionKeyNames.Count;
    }

}
