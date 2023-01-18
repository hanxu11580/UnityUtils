using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsOctreeDemo : MonoBehaviour
{
    BoundsOctree<GameObject> boundsTree;
    PointOctree<GameObject> pointTree;
    // Start is called before the first frame update
    void Start()
    {
        // Initial size (metres), initial centre position, minimum node size (metres), looseness
        boundsTree = new BoundsOctree<GameObject>(15, transform.position, 1, 1.25f);
        // Initial size (metres), initial centre position, minimum node size (metres)
        pointTree = new PointOctree<GameObject>(15, transform.position, 1);
    }

    void OnDrawGizmos() {
        if(boundsTree != null) {
            boundsTree.DrawAllBounds(); // Draw node boundaries
            boundsTree.DrawAllObjects(); // Draw object boundaries
            boundsTree.DrawCollisionChecks(); // Draw the last *numCollisionsToSave* collision check boundaries
        }

        if(pointTree != null) {
            pointTree.DrawAllBounds(); // Draw node boundaries
            pointTree.DrawAllObjects(); // Mark object positions
        }
    }
}
