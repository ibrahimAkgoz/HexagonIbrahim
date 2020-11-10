using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data 
{
    public static void SetInt(string key,int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    public static int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }
}
