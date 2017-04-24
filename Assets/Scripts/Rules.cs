using UnityEngine;
using System.Collections.Generic;

public delegate void RulesEventHandler(float amount);
public delegate void RulesStateEventHandler();
public static class Rules{
    public static GameManager GameManagerObject;
    public static GameControls GameControls;
    public static float InfusionCost = 100;
    static float score;
    public static float Score { get { return score; } }

    public static RulesEventHandler ScoreUpdated;
    public static RulesStateEventHandler GameStarted;

    public static void AddScore(float amount)
    {
        IncreaseScore(amount);
    }
    public static void RegisterProduction(LightMote mote)
    {
        mote.Produced += IncreaseScore;
    }

    public static void UnregisterProduction(LightMote mote)
    {
        mote.Produced -= IncreaseScore;
    }

    static void IncreaseScore(float amount)
    {
        amount *= 10;
        score += amount * amount;
        ScoreUpdated(score);
    }
}
