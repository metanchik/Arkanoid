using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void NextLevel() {
        GameController.currentLevelIndex += 1;
        // SceneManager.LoadScene("Level");
        Debug.Log("Load next level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
