using System;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRNG
{
    System.Random _Rng;

    public DefaultRNG(int seed)
    {
        SetSeed(seed);
    }

    public void SetSeed (int seed)
    {
        _Rng = new System.Random(seed);
    }

    public int Next(int Max)
    {
        return _Rng.Next(Max);
    }

    public int Next(int Min, int Max)
    {
        return _Rng.Next(Min, Max);
    }

    public double NextDouble()
    {
        return _Rng.NextDouble();
    }

    public float NextFloat()
    {
        return (float)_Rng.NextDouble();
    }

    public float Rand()
    {
        return (float)Math.Round((_Rng.NextDouble() * 2f) - 1f, 2);
    }

    public Vector2 PointInCircle(float CenterX, float CenterY, float Radius)
    {
        var angle = _Rng.NextDouble() * Math.PI * 2;
        var radius = Math.Sqrt(_Rng.NextDouble()) * Radius;
        var x = CenterX + radius * Math.Cos(angle);
        var y = CenterY + radius * Math.Sin(angle);

        return new Vector2((float)x, (float)y);
    }

    public Vector2 PointOnCircle(float radius)
    {
        float angle = (float)_Rng.NextDouble() * Mathf.PI * 2;

        return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }

    public T WeightedChoice<T>(IList<T> choices, List<int> weights, int totalWeight)
    {
        int randomNumber = _Rng.Next(0, totalWeight);

        T selectedChoice = default(T);

        for (int index = 0; index < weights.Count;index++)
        {
            if (randomNumber < weights[index])
            {
                selectedChoice = choices[index];
                break;
            }

            randomNumber = randomNumber - weights[index];
        }

        return selectedChoice;
    }

    public T WeightedChoice<T>(IList<T> choices, List<int> weights)
    {
        int totalWeight = 0;

        // Calculate the Total weight;
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        return WeightedChoice(choices, weights, totalWeight);
    }
}
