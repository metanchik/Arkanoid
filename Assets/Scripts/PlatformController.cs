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

    private Dictionary<BlockSide, Vector3> normals = new Dictionary<BlockSide, Vector3>() {
        {BlockSide.Left, new Vector3(-0.866f, 0.5f, 0).normalized},
        {BlockSide.Top, new Vector3(0, 1f, 0).normalized},
        {BlockSide.Right, new Vector3(0.866f, 0.5f, 0).normalized},
    };

    private void Awake() {
        currentLength = mainLength;
        field = FindObjectOfType<FieldController>();
        height = transform.localScale.y;
        // currentLength = 7f;
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

    public Vector3 GetDirection(BlockSide side, Vector3 intersectionPoint, Vector3 startDirection) {
        switch (side) {
            case BlockSide.Left: {
                Vector3 normal = normals[BlockSide.Left];
                Vector3 outVector = Vector3.Reflect(startDirection, normal);
                
                return outVector.normalized;
            }
            case BlockSide.Top: {
                Vector3 normal = normals[BlockSide.Top];
                Vector3 outVector = Vector3.Reflect(startDirection, normal);
                if (Mathf.Abs(intersectionPoint.x - transform.position.x) < (currentLength * 0.4f))
                    return outVector;
                float coef = Mathf.Abs(intersectionPoint.x - transform.position.x - currentLength * 0.4f) * 0.1f;

                float sign = outVector.x < 0 ? -1 : 1;
                Vector3 targetVector = new Vector3(sign, 0f, 0f);

                Vector3 newDirection = Vector3.Lerp(outVector, targetVector, coef);
                
                return newDirection.normalized;
            }
            case BlockSide.Right: {
                Vector3 normal = normals[BlockSide.Right];
                Vector3 outVector = Vector3.Reflect(startDirection, normal);
                
                return outVector.normalized;
            }
        }

        return Vector3.up;
    }
}