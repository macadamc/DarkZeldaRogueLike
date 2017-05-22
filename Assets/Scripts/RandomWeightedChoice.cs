using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomWeightedChoice : MonoBehaviour {

    private static DefaultRNG rng;

    public List<Choice<int>> choices;

    void Awake ()
    {
        rng = new DefaultRNG(DateTime.Now.GetHashCode());

        if (choices == null) { choices = new List<Choice<int>>(); }
    }

    void Start()
    {
        if (choices.Count == 0) { return; }

        StartCoroutine("RandomChoice");
    }

    IEnumerator RandomChoice()
    {
        WaitForSeconds Wait = new WaitForSeconds(1f);
        List<int> weights = new List<int>();
        int totalweight;
        // total the weigth
        while(true)
        {
            weights.Clear();
            totalweight = 0;
            foreach (Choice<int> c in choices)
            {
                totalweight += c.Weight;
                weights.Add(c.Weight);
            }

            Debug.Log(rng.WeightedChoice(choices, weights, totalweight).Name);
            yield return Wait;
        }

    }
}

[Serializable]
public class Choice<T>
{
    public string Name = string.Empty;
    public int Weight = 0;
    public T Value;

    public Choice(string n, int w, T v)
    {
        this.Name = n;
        this.Weight = w;
        this.Value = v;
    }
}
