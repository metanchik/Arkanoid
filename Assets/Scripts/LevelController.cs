using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class LevelController : MonoBehaviour {
    public int lives = 3;
    private float levelTime;
    private int rowsCount = 15;
    private int columnsCount = 6;
    private int currentBlockCount = 0;

    public Block[][] blocks;
    public Vector3 blockSize;
    public GameObject completeWindow;
    public GameObject failWindow;
    public PlatformController platform;
    public LivesPanelScript livesPanel;

    public int RowsCount => rowsCount;
    public int ColumnsCount => columnsCount;

    void Awake() {
        blocks = new Block[rowsCount][];
        string config = LevelConfigs.GetLevelConfig(GameController.currentLevelIndex);
        BuildBlocks(config);
        platform = FindObjectOfType<PlatformController>();
        livesPanel = FindObjectOfType<LivesPanelScript>();
    }

    private void BuildBlocks(string config) {
        string[] rows = config.Split(new char[] {'.'});
        for (int i = 0; i < rowsCount; i++) {
            List<string> cells = rows[i].Split(new char[] {'/'}).ToList();
            cells.RemoveAll(s => s.Equals(""));
            blocks[i] = new Block[columnsCount];
            if (cells.Count == 1) {
                if (!cells[0][0].Equals('-')) {
                    Color cellColor = GetColor(cells[0][0]);
                    int lives = 1;
                    if (cells[0].Length > 1) {
                        if (char.IsNumber(cells[0][1])) {
                            lives = int.Parse(cells[0][1].ToString());
                        }
                        else {
                            Debug.LogError("[Need] cells[0][1] is not number");
                        }

                        if (cells[0].Length == 3) {
                            Debug.LogError("[Need] cells[0] length is 3");
                        }
                    }

                    for (int j = 0; j < columnsCount; j++) {
                        Block block = new Block(cellColor, lives, i, j);
                        blocks[i][j] = block;
                        currentBlockCount += 1;
                    }
                }
            }
            else {
                Debug.LogError("[Need] cells.length > 1");
                for (int j = 0; j < cells.Count; j++) {
                }
            }
        }

        CalculateBlockCenters();
    }

    private Color GetColor(char colorConfig) {
        Color color;
        switch (colorConfig) {
            case 'r': {
                color = Color.red;
                break;
            }
            case 'g': {
                color = Color.green;
                break;
            }
            case 'b': {
                color = Color.blue;
                break;
            }
            case 'y': {
                color = Color.yellow;
                break;
            }
            case 'c': {
                color = Color.cyan;
                break;
            }
            case 'm': {
                color = Color.magenta;
                break;
            }
            default: {
                color = Color.black;
                break;
            }
        }

        return color;
    }

    public void CalculateBlockCenters() {
        GameObject field = GameObject.Find("Field");
        Vector3 fieldPos = field.transform.position;
        Vector3 fieldSize = field.transform.localScale;
        float leftGrid = fieldPos.x - fieldSize.x / 2 + fieldSize.x / 5;
        float topGrid = fieldPos.y + fieldSize.y / 2 - fieldSize.y / 6;

        float gridWeight = fieldSize.x * 3 / 5;
        float gridHeight = fieldSize.y / 4;

        float blockWidth = gridWeight / columnsCount;
        float blockHeight = gridHeight / rowsCount;

        float firstBlockCenterX = leftGrid + blockWidth / 2;
        float firstBlockCenterY = topGrid - blockHeight / 2;

        blockSize = new Vector3(blockWidth, blockHeight, 1f);

        for (int i = 0; i < rowsCount; i++) {
            for (int j = 0; j < columnsCount; j++) {
                if (blocks[i][j] != null) {
                    float centerX = firstBlockCenterX + blockWidth * j;
                    float centerY = firstBlockCenterY - blockHeight * i;

                    blocks[i][j].BuildBlock(new Vector3(centerX, centerY, -0.1f), blockSize, "Block_" + i + "_" + j);
                }
            }
        }
    }

    public void RemoveBlock(int i, int j) {
        blocks[i][j] = null;
        currentBlockCount -= 1;
        CheckLevelCompleted();
    }

    public void CheckLevelCompleted() {
        if (currentBlockCount == 0) {
            Debug.Log("Complete level!)");
            FindObjectsOfType<BallController>().ToList().ForEach(b => b.speed = 0);
            completeWindow.SetActive(true);
        }
    }

    public void LoseLife() {
        lives -= 1;
        livesPanel.Actualize();
        if (lives < 1) {
            failWindow.SetActive(true);
        }
    }
}