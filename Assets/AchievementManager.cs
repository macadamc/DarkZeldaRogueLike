using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadyPixel.SaveLoad;

public class AchievementManager : MonoBehaviour {

    public Dictionary<string, int> monsterKillsLocal = new Dictionary<string, int>();
    public Dictionary<string, int> monsterKills = new Dictionary<string, int>();
    public static AchievementManager am;



	// Use this for initialization
	void Awake () {

        am = this;
		
	}

    public void OnObjDeath(string name)
    {
        if (monsterKillsLocal.ContainsKey(name))
            monsterKillsLocal[name]++;
        else
            monsterKillsLocal.Add(name, 1);
    }

    public void AddStats()
    {
        foreach (KeyValuePair<string, int> monsterkill in monsterKillsLocal)
        {
            if (monsterKills.ContainsKey(monsterkill.Key))
                monsterKills[monsterkill.Key] += monsterkill.Value;
            else
                monsterKills.Add(monsterkill.Key, monsterkill.Value);
        }

        foreach (KeyValuePair<string, int> monsterkill in monsterKills)
        {
            Debug.Log("total "+monsterkill.Key + " kills: " + monsterkill.Value);
        }

        SaveLoadManager.slm.SaveGame();
    }

    public void DebugPrint()
    {
        foreach (KeyValuePair<string, int> monsterkill in monsterKillsLocal)
        {
            Debug.Log(monsterkill.Key + ": " + monsterkill.Value);
        }
        AddStats();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
