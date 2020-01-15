using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfigs : MonoBehaviour
{
    private static List<string> levels = new List<string>() {
        "-/." +
        "-/." +
        "-/." +
        "r/."+
        "g/."+
        "b/."+
        "y/."+
        "r/."+
        "c/."+
        "m/."+
        "g/."+
        "r/."+
        "-/." +
        "-/." +
        "-/.",
        //-----------
        "-/." +
        "-/." +
        "-/." +
        "r2/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "-/." +
        "-/." +
        "-/.",
        //-----------
        "g2/." +
        "-/." +
        "b2/." +
        "r2/."+
        "-/."+
        "-/."+
        "c2/."+
        "m3/."+
        "-/."+
        "-/."+
        "r2/."+
        "y3/."+
        "-/." +
        "-/." +
        "-/.",
    };

    public static string GetLevelConfig(int index) {
        return levels[index];
    }

    public static int GetLevelsCount() {
        return levels.Count;
    }
}
