using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Block {
    public Vector3 center;
    public int lives = 1;
    public Color color;
    public GameObject block;

    private int i;
    private int j;

    private List<Vector3> leftSide;
    private List<Vector3> topSide;
    private List<Vector3> rightSide;
    private List<Vector3> bottomSide;

    private Dictionary<BlockSide, List<Vector3>> sides = new Dictionary<BlockSide, List<Vector3>>();

    public Block(Color color, int i, int j) {
        this.i = i;
        this.j = j;
        this.color = color;
    }

    public Block(Color color, int lives, int i , int j) : this(color, i, j) {
        SetLives(lives);
    }

    public void SetLives(int lives) {
        if (lives > 0) {
            this.lives = lives;
        }
    }

    public void SetBlockTransform(Vector3 center, Vector3 size) {
        this.center = center;
        block.transform.position = center;
        block.transform.localScale = size;

        float left = center.x - size.x / 2;
        float right = center.x + size.x / 2;
        float top = center.y + size.y / 2;
        float bottom = center.y - size.y / 2;
        
        leftSide = new List<Vector3>(){new Vector3(left, bottom, 0f), new Vector3(left, top, 0f), Vector3.left};
        topSide = new List<Vector3>(){new Vector3(left, top, 0f), new Vector3(right, top, 0f), Vector3.up};
        rightSide = new List<Vector3>(){new Vector3(right, bottom, 0f), new Vector3(right, top, 0f), Vector3.right};
        bottomSide = new List<Vector3>(){new Vector3(left, bottom, 0f), new Vector3(right, bottom, 0f), Vector3.down};
        
        sides.Add(BlockSide.Left, leftSide);
        sides.Add(BlockSide.Top, topSide);
        sides.Add(BlockSide.Right, rightSide);
        sides.Add(BlockSide.Bottom, bottomSide);
    }

    public void BuildBlock(Vector3 center, Vector3 scale, string name) {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/block");
        block = GameObject.Instantiate(prefab);
        SetBlockTransform(center, scale);
        block.name = name;
        SpriteRenderer sprite = block.GetComponent<SpriteRenderer>();
        sprite.color = color;
        if (lives > 1) {
            sprite.color = Color.white;
        }
    }

    public List<Vector3> GetSide(BlockSide side) {
        return sides[side];
    }

    public void Hit() {
        lives -= 1;
        if (lives == 0) {
            GameObject.Destroy(block);
            GameObject.FindObjectOfType<LevelController>().RemoveBlock(i, j);
        }
        else {
            if (lives == 1) {
                block.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }
}

public enum BlockSide {
    Left,
    Top,
    Right,
    Bottom
}
