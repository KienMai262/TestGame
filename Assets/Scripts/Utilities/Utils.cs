using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    public static List<NormalItem.eNormalType> weightedList = new List<NormalItem.eNormalType>();
    private static bool isInitialized = false;
    public static Dictionary<NormalItem.eNormalType, int> debugCounter = new Dictionary<NormalItem.eNormalType, int>();
    public static List<NormalItem.eNormalType> filteredList = new List<NormalItem.eNormalType>();
    public static Dictionary<NormalItem.eNormalType, int> weights = new Dictionary<NormalItem.eNormalType, int>
    {
        { NormalItem.eNormalType.TYPE_ONE, 27 },
        { NormalItem.eNormalType.TYPE_TWO, 27 },
        { NormalItem.eNormalType.TYPE_THREE, 24 },
        { NormalItem.eNormalType.TYPE_FOUR, 24 },
        { NormalItem.eNormalType.TYPE_FIVE, 24 },
        { NormalItem.eNormalType.TYPE_SIX, 24 },
        { NormalItem.eNormalType.TYPE_SEVEN, 24 }
    };

    private static void InitializeWeightedList()
    {
        if (isInitialized) return;

        foreach (var kvp in weights)
        {
            weightedList.AddRange(Enumerable.Repeat(kvp.Key, kvp.Value));
            debugCounter[kvp.Key] = 0;
        }

        weightedList = weightedList.OrderBy(x => URandom.value).ToList();

        isInitialized = true;
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept(NormalItem.eNormalType[] types)
    {
        InitializeWeightedList();

        List<NormalItem.eNormalType> filteredList = weightedList.Where(t => !types.Contains(t)).ToList();

        if (filteredList.Count == 0)
            throw new Exception("Không còn loại hợp lệ để chọn!");

        NormalItem.eNormalType selected = new NormalItem.eNormalType();
        int index = 0;
        do
        {
            index = URandom.Range(0, filteredList.Count);
            selected = filteredList[index];
        } while (debugCounter[selected] >= weights[selected]);
        debugCounter[selected]++;

        return selected;
    }
    public static void ResetUtils()
    {
        weightedList.Clear();
        debugCounter.Clear();
        isInitialized = false;
        InitializeWeightedList();
        Debug.Log("Utils đã được reset! WeightedList count: " + weightedList.Count);
    }
}
