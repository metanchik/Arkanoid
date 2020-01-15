using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlatformController : MonoBehaviour {
    private float mainLength = 1.9f;
    private float currentLength;
    private float platformSpeed = 0.5f;
    private FieldController field;
    private float height;

    private Dictionary<PlatformNormals, List<Vector3>> normals = new Dictionary<PlatformNormals, List<Vector3>>() {
        {PlatformNormals.Left, new List<Vector3>() {new Vector3(-0.866f, 0.5f, 0).normalized}},
        {PlatformNormals.LeftCorner, new List<Vector3>() {new Vector3(-0.7f, 0.7f, 0).normalized}}, {
            PlatformNormals.Top, new List<Vector3>() {
                new Vector3(-0.5f, 0.866f, 0).normalized,
                new Vector3(-0.259f, 0.966f, 0).normalized,
                new Vector3(0, 1f, 0).normalized,
                new Vector3(0.259f, 0.966f, 0).normalized,
                new Vector3(0.5f, 0.866f, 0).normalized,
            }
        },
        {PlatformNormals.RightCorner, new List<Vector3>() {new Vector3(0.7f, 0.7f, 0).normalized}},
        {PlatformNormals.Right, new List<Vector3>() {new Vector3(0.866f, 0.5f, 0).normalized}},
    };

    private void Awake() {
        currentLength = mainLength;
        field = FindObjectOfType<FieldController>();
        height = transform.localScale.y;
        currentLength = 7f;
        transform.localScale = new Vector3(currentLength, height, 1);
    }

    private void Update() {
        Vector3 newPosition = transform.position + new Vector3(Input.GetAxis("Mouse X") * platformSpeed, 0, 0);

        if (newPosition.x - currentLength / 2 < field.left) {
            newPosition.x = field.left + currentLength / 2;
        }

        if (newPosition.x + currentLength / 2 > field.right) {
            newPosition.x = field.right - currentLength / 2;
        }

        transform.position = newPosition;
    }

    public Dictionary<BlockSide, List<Vector3>> GetSidesForCheckingIntersect(float ballRadius) {
        Dictionary<BlockSide, List<Vector3>> sides = new Dictionary<BlockSide, List<Vector3>>();

        float left = transform.position.x - currentLength / 2;
        float top = transform.position.y + height / 2;
        float right = transform.position.x + currentLength / 2;
        float bottom = transform.position.y - height / 2;

        sides.Add(BlockSide.Left,
            new List<Vector3>()
                {new Vector3(left - ballRadius, bottom, 0f), new Vector3(left - ballRadius, top + ballRadius, 0f)});
        sides.Add(BlockSide.Top,
            new List<Vector3>() {
                new Vector3(left - ballRadius, top + ballRadius, 0f),
                new Vector3(right + ballRadius, top + ballRadius, 0f)
            });
        sides.Add(BlockSide.Right,
            new List<Vector3>()
                {new Vector3(right + ballRadius, bottom, 0f), new Vector3(right + ballRadius, top + ballRadius, 0f)});

        return sides;
    }

    public Vector3 GetDirection(BlockSide side, Vector3 intersectionPoint) {
        float left = transform.position.x - currentLength / 2;
        float top = transform.position.y + height / 2;
        float right = transform.position.x + currentLength / 2;

        switch (side) {
            case BlockSide.Left: {
                if (intersectionPoint.y <= top)
                    return normals[PlatformNormals.Left][0];
                return normals[PlatformNormals.LeftCorner][0];
            }
            case BlockSide.Top: {
                if (intersectionPoint.x <= left)
                    return normals[PlatformNormals.LeftCorner][0];
                if (intersectionPoint.x >= right)
                    return normals[PlatformNormals.RightCorner][0];
                float deltaTop = currentLength / normals[PlatformNormals.Top].Count;
                if (intersectionPoint.x <= left + deltaTop)
                    return normals[PlatformNormals.Top][0];
                if (intersectionPoint.x <= left + deltaTop * 2)
                    return normals[PlatformNormals.Top][1];
                if (intersectionPoint.x <= left + deltaTop * 3)
                    return normals[PlatformNormals.Top][2];
                if (intersectionPoint.x <= left + deltaTop * 4)
                    return normals[PlatformNormals.Top][3];
                return normals[PlatformNormals.Top][4];
            }
            case BlockSide.Right: {
                if (intersectionPoint.y <= top)
                    return normals[PlatformNormals.Right][0];
                return normals[PlatformNormals.RightCorner][0];
            }
        }

        return Vector3.up;
    }
}

enum PlatformNormals {
    Left,
    LeftCorner,
    Top,
    RightCorner,
    Right
}