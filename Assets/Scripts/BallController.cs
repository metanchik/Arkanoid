using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BallController : MonoBehaviour {
    public float mainSpeed = 5;
    public float speed;
    private Vector3 currentDirection;
    private List<List<Vector3>> borders;
    private LevelController level;
    public float ballRadius;
    private bool isLaunched = false;


    private void Awake() {
        speed = 10;
        borders = new List<List<Vector3>>();
        SetBorders();
        level = FindObjectOfType<LevelController>();
        ballRadius = GameObject.Find("Ball").transform.localScale.x / 2;
    }

    private void SetBorders() {
        FieldController field = FindObjectOfType<FieldController>();
        ballRadius = GameObject.Find("Ball").transform.localScale.x / 2;

        float left = field.left + ballRadius;
        float top = field.top - ballRadius;
        float right = field.right - ballRadius;
        float bottom = field.bottom + ballRadius;

        borders.Add(new List<Vector3>() {new Vector3(left, bottom, 0), new Vector3(left, top, 0f), Vector3.right});
        borders.Add(new List<Vector3>() {new Vector3(left, top, 0f), new Vector3(right, top, 0f), Vector3.down});
        borders.Add(new List<Vector3>() {new Vector3(right, bottom, 0f), new Vector3(right, top, 0f), Vector3.left});
        borders.Add(new List<Vector3>() {new Vector3(left, bottom, 0f), new Vector3(right, bottom, 0f), Vector3.up});
    }

    void Start() {
        currentDirection = (Vector3.left + Vector3.up).normalized;
    }

    void Update() {
        if (speed > 0) {
            Vector3 newPosition = GetNewPosition(transform.position, Time.deltaTime);

            transform.position = newPosition;
        }
    }

    private void FixedUpdate() {
        if (!isLaunched) {
        }
    }

    private Vector3 GetNewPosition(Vector3 startPosition, float deltaTime) {
        Vector3 newPosition = Vector3.Lerp(startPosition, startPosition + speed * currentDirection,
            deltaTime);
        newPosition.z = -0.1f;

        Vector3 intersection;
        Vector3 normal;
        if (CheckPlatformIntersect(startPosition, newPosition, out intersection, out normal)) {
            intersection.z = -0.1f;
            if (intersection != startPosition) {
                currentDirection = Vector3.Reflect(intersection - startPosition, normal).normalized;

                if (intersection != newPosition) {
                    Vector3 b1i = newPosition - intersection;
                    Vector3 ball = newPosition - startPosition;
                    float dt = b1i.magnitude * deltaTime / ball.magnitude;

                    newPosition = GetNewPosition(intersection, dt);
                }
            }
        }

        IntersectionData intersectionData;
        if (CheckBlocksIntersect(startPosition, newPosition, out intersectionData)) {
            intersectionData.intersectionPoint.z = -0.1f;
            if (intersectionData.intersectionPoint != startPosition) {
                normal = intersectionData.sides.First().Value[2];
                currentDirection = Vector3.Reflect(intersectionData.intersectionPoint - startPosition, normal)
                    .normalized;
                if (intersection != newPosition) {
                    Vector3 b1i = newPosition - intersectionData.intersectionPoint;
                    Vector3 ball = newPosition - startPosition;
                    float dt = b1i.magnitude * deltaTime / ball.magnitude;

                    intersectionData.block.Hit();
                    newPosition = GetNewPosition(intersectionData.intersectionPoint, dt);
                }
            }
        }
        else {
            borders.ForEach(b => {
                if (LineSegmentsIntersect(b[0], b[1], startPosition, newPosition, out intersection)) {
                    intersection.z = -0.1f;
                    if (intersection != startPosition) {
                        normal = b[2];
                        currentDirection = Vector3.Reflect(intersection - startPosition, normal).normalized;
                        if (intersection != newPosition) {
                            Vector3 b1i = newPosition - intersection;
                            Vector3 ball = newPosition - startPosition;
                            float dt = b1i.magnitude * deltaTime / ball.magnitude;
                            newPosition = GetNewPosition(intersection, dt);
                        }
                    }
                }
            });
        }

        return newPosition;
    }

    private bool LineSegmentsIntersect(Vector3 point1, Vector3 point2, Vector3 ball1, Vector3 ball2,
        out Vector3 intersection) {
        intersection = new Vector3();
        point1.z = point2.z = ball1.z = ball2.z = 0f;

        Vector3 line = point2 - point1;
        Vector3 ball = ball2 - ball1;
        float lxb = Vector3.Cross(line, ball).z;

        float t = Vector3.Cross((ball1 - point1), ball).z / lxb;
        float u = Vector3.Cross((ball1 - point1), line).z / lxb;

        if (Mathf.Abs(lxb) > 0 && (0 <= t && t <= 1) && (0 <= u && u <= 1)) {
            intersection = point1 + t * line;
            return true;
        }

        return false;
    }

    private bool CheckBlocksIntersect(Vector3 ball1, Vector3 ball2, out IntersectionData intersectionRes) {
        intersectionRes = new IntersectionData();
        List<BlockSide> sidesForCheckingNames = GetBlockSideNamesForChecking();
        List<IntersectionData> intersections = new List<IntersectionData>();
        for (int i = 0; i < level.RowsCount; i++) {
            for (int j = 0; j < level.ColumnsCount; j++) {
                Block block = level.blocks[i][j];
                if (block != null) {
                    Dictionary<BlockSide, List<Vector3>> sides =
                        GetActualBlockSidesForChecking(sidesForCheckingNames, level.blocks, i, j);
                    if (sides.Count > 0) {
                        IntersectionData intersectionData = null;
                        Vector3 intersection;
                        foreach (var pair in sides) {
                            List<Vector3> side = pair.Value;
                            if (LineSegmentsIntersect(side[0], side[1], ball1, ball2, out intersection)) {
                                if (intersectionData == null)
                                    intersectionData = new IntersectionData(block,
                                        new Dictionary<BlockSide, List<Vector3>>() {
                                            {pair.Key, side}
                                        }, intersection);
                                else {
                                    intersectionData.sides.Add(pair.Key, side);
                                }
                            }
                        }

                        if (intersectionData != null) {
                            intersections.Add(intersectionData);
                        }
                    }
                }
            }
        }

        if (intersections.Count > 0) {
            intersectionRes = intersections[0];

            float minDistance = float.MaxValue;

            foreach (var id in intersections) {
                float sqrtMag = (id.intersectionPoint - ball1).sqrMagnitude;
                if (sqrtMag < minDistance) {
                    minDistance = sqrtMag;
                    intersectionRes = id;
                }
            }

            //todo: добавить проверку на угол

            return true;
        }

        return false;
    }

    private List<BlockSide> GetBlockSideNamesForChecking() {
        List<BlockSide> sides = new List<BlockSide>();

        if (currentDirection.x < 0)
            sides.Add(BlockSide.Right);
        else
            sides.Add(BlockSide.Left);
        if (currentDirection.y > 0)
            sides.Add(BlockSide.Bottom);
        else
            sides.Add(BlockSide.Top);

        return sides;
    }

    private Dictionary<BlockSide, List<Vector3>> GetActualBlockSidesForChecking(List<BlockSide> sidesNames,
        Block[][] blocks, int i, int j) {
        Dictionary<BlockSide, List<Vector3>> blockSides = new Dictionary<BlockSide, List<Vector3>>();
        sidesNames.ForEach(sn => {
            switch (sn) {
                case BlockSide.Left: {
                    if (j == 0 || blocks[i][j - 1] == null) {
                        List<Vector3> side = new List<Vector3>(blocks[i][j].GetSide(sn));
                        side[0] += new Vector3(-ballRadius, -ballRadius, 0f);
                        side[1] += new Vector3(-ballRadius, ballRadius, 0f);
                        blockSides.Add(sn, side);
                    }

                    break;
                }
                case BlockSide.Top: {
                    if (i == 0 || blocks[i - 1][j] == null) {
                        List<Vector3> side = new List<Vector3>(blocks[i][j].GetSide(sn));
                        side[0] += new Vector3(-ballRadius, ballRadius, 0f);
                        side[1] += new Vector3(ballRadius, ballRadius, 0f);
                        blockSides.Add(sn, side);
                    }

                    break;
                }
                case BlockSide.Right: {
                    if (j == level.ColumnsCount - 1 || blocks[i][j + 1] == null) {
                        List<Vector3> side = new List<Vector3>(blocks[i][j].GetSide(sn));
                        side[0] += new Vector3(ballRadius, -ballRadius, 0f);
                        side[1] += new Vector3(ballRadius, ballRadius, 0f);
                        blockSides.Add(sn, side);
                    }

                    break;
                }
                default: {
                    if (i == level.RowsCount - 1 || blocks[i + 1][j] == null) {
                        List<Vector3> side = new List<Vector3>(blocks[i][j].GetSide(sn));
                        side[0] += new Vector3(-ballRadius, -ballRadius, 0f);
                        side[1] += new Vector3(ballRadius, -ballRadius, 0f);
                        blockSides.Add(sn, side);
                    }

                    break;
                }
            }
        });
        return blockSides;
    }

    public bool CheckPlatformIntersect(Vector3 ball1, Vector3 ball2, out Vector3 intersection, out Vector3 direction) {
        Dictionary<BlockSide, List<Vector3>> sides = new Dictionary<BlockSide, List<Vector3>>(level.platform.GetSidesForCheckingIntersect(ballRadius));
        Dictionary<BlockSide, List<Vector3>> intersections = new Dictionary<BlockSide, List<Vector3>>();
        direction = new Vector3();
        intersection = new Vector3();
        
        foreach (var pair in sides) {
            List<Vector3> side = new List<Vector3>(pair.Value);
            if (LineSegmentsIntersect(side[0], side[1], ball1, ball2, out intersection)) {
                side = side.Prepend(intersection).ToList();
                intersections.Add(pair.Key, side);
            }
        }

        if (intersections.Count > 0) {
            float minDistance = float.MaxValue;
            KeyValuePair<BlockSide, List<Vector3>> intersectionRes;

            foreach (var id in intersections) {
                float sqrtMag = (id.Value[0] - ball1).sqrMagnitude;
                if (sqrtMag < minDistance) {
                    minDistance = sqrtMag;
                    intersectionRes = id;
                    intersection = id.Value[0];
                }
            }

            direction = level.platform.GetDirection(intersectionRes.Key, intersection);

            //todo: добавить проверку на угол
            return true;
        }

        return false;
    }
}