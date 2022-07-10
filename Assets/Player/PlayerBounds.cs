using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerBounds : MonoBehaviour
{
    [SerializeField]
    private Vector3 boundaries;
    
    private Bounds colliderBounds;

    private void Start()
    {
        colliderBounds.size = boundaries;
    }
    
    public bool InBoundary(Vector3 origin, Vector3 extends)
    {
        return extends == Vector3.zero ?
            colliderBounds.Contains(origin) : 
            colliderBounds.Contains(origin) && colliderBounds.Contains(origin + extends);
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

        Bounds bounds = colliderBounds;
        Vector3 minBounds = bounds.min;
        Vector3 maxBounds = bounds.max;
        
        Vector3 outDeltaVectorFirst = GetDeltaClampedVector(origin, minBounds, maxBounds);
        Vector3 outDeltaVectorSecond = GetDeltaClampedVector(origin + extends, minBounds, maxBounds);

        return outDeltaVectorFirst + outDeltaVectorSecond;
    }

    public Vector3 DistToBoundary((Vector3, Vector3) box)
    {
        return DistToBoundary(box.Item1, box.Item2);
    }
}
