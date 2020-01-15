using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour {
    public float left;
    public float top;
    public float right;
    public float bottom;
    public Vector3 fieldCenter;
    public Vector3 fieldSize;
    
    private void Awake() {
        fieldCenter = transform.position;
        fieldSize = transform.localScale;
        
        left = fieldCenter.x - fieldSize.x / 2;
        top = fieldCenter.y + fieldSize.y / 2;
        right = fieldCenter.x + fieldSize.x / 2;
        bottom = fieldCenter.y - fieldSize.y / 2;
    }
}
