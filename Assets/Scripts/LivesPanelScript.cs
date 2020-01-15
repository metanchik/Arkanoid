using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LivesPanelScript : MonoBehaviour {
    private LevelController level;
    private List<GameObject> icons;

    private void Start() {
        level = FindObjectOfType<LevelController>();
        icons = new List<GameObject>();
        foreach (Transform t in transform) {
            icons.Add(t.gameObject);
        } 
        Actualize();
    }

    public void Actualize() {
        int lives = level.lives;
        for (int i = 0; i < icons.Count; i++) {
            if (i < lives) 
                icons[i].SetActive(true);
            else 
                icons[i].SetActive(false);
        }
    }
}
