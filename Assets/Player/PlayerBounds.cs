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
}
