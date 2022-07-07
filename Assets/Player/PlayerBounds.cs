using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(BoxCollider))]
public class PlayerBounds : MonoBehaviour
{
    private BoxCollider bounds;
    private Collider boundsCollider;

    private void Start()
    {
        boundsCollider = GetComponent<Collider>();
    }
    
    public bool InBoundary(Vector3 origin, Vector3 extends)
    {
        return extends == Vector3.zero ?
            boundsCollider.bounds.Contains(origin) : 
            boundsCollider.bounds.Contains(origin) && boundsCollider.bounds.Contains(origin + extends);
    }

    public bool InBoundary(Vector3 origin)
    {
        return InBoundary(origin, Vector3.zero);
    }
    
    public bool InBoundary((Vector3, Vector3) box)
    {
        return InBoundary(box.Item1, box.Item2);
    }

    /* returns a vector describing the closest distance to the boundary
    / Returns 0 if object is within the boundaries */
    public Vector3 DistToBoundary(Vector3 origin, Vector3 extends)
    {
        if (InBoundary(origin, extends))
            return Vector3.zero;

        Vector3 GetDeltaClampedVector(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(value.x - Mathf.Clamp(value.x, min.x, max.x),
                               value.y - Mathf.Clamp(value.y, min.y, max.y), 
                               value.z - Mathf.Clamp(value.z, min.z, max.z));
        }

        Bounds colliderBounds = boundsCollider.bounds;
        Vector3 minBounds = colliderBounds.min;
        Vector3 maxBounds = colliderBounds.max;
        
        Vector3 outDeltaVectorFirst = GetDeltaClampedVector(origin, minBounds, maxBounds);
        Vector3 outDeltaVectorSecond = GetDeltaClampedVector(origin + extends, minBounds, maxBounds);

        return outDeltaVectorFirst + outDeltaVectorSecond;
    }

    public Vector3 DistToBoundary((Vector3, Vector3) box)
    {
        return DistToBoundary(box.Item1, box.Item2);
    }
}
