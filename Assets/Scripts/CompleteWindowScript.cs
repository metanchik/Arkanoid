using System;
using UnityEngine;
using UnityEngine.UI;

public class CompleteWindowScript : MonoBehaviour {
    public Button nextLevelButton;
    private void OnEnable() {

        if (GameController.currentLevelIndex >= LevelConfigs.GetLevelsCount() - 1) {
            nextLevelButton.gameObject.SetActive(false);
        }
    }
}