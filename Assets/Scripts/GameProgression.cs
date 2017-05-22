using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameProgression {

    public List<string> progressionKeyNames = new List<string>();
    public List<string> progressionValues = new List<string>();

    public string this[string key]
    {
        get
        {
            string value = null;
            if(progressionKeyNames.Contains(key))
            {
                value = progressionValues[progressionKeyNames.IndexOf(key)];
            }
            return value;
        }
        set
        {
            if(value == null)
            {
                RemoveKeyValuePair(key);
            }
            if(progressionKeyNames.Contains(key))
            {
                progressionValues[progressionKeyNames.IndexOf(key)] = value;
            }
            else
            {
                AddKeyValuePair(key, value);
            }
        }
    }
    public void AddKeyValuePair(string key, string value)
    {
        progressionKeyNames.Add(key);
        progressionValues.Add(value);
    }

    public void RemoveKeyValuePair(string key)
    {
        progressionValues.RemoveAt(progressionKeyNames.IndexOf(key));
        progressionKeyNames.Remove(key);
    }

    public void ClearAll()
    {
        progressionKeyNames.Clear();
        progressionValues.Clear();
    }

}


