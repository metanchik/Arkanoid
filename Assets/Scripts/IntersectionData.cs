using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionData {
   public Block block;
   public Dictionary<BlockSide, List<Vector3>> sides;
   public Vector3 intersectionPoint;

   public IntersectionData() {
   }

   public IntersectionData(Block block, Dictionary<BlockSide, List<Vector3>> sides, Vector3 intersectionPoint) {
      this.block = block;
      this.sides = sides;
      this.intersectionPoint = intersectionPoint;
   }
}
