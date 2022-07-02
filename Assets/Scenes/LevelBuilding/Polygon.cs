using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    private const float VERY_LARGE_NUMBER = 10000F;

    public Vector2[] Vertices { get; private set; }

    public Polygon(params Vector2[] vertices)
    {
        if (vertices.Length < 3)
        {
            throw new ArgumentException("Polygon needs at least 3 vertices.");
        }
        Vertices = vertices;
    }

    public void DrawDebugOutLine(Transform transform, float yOffset, Color color, float duration, bool depthTest)
    {
        for (int i = 0; i < Vertices.Length; i++)
        {
            int j = nextVerticeIndex(i);
            Vector3 a = new(Vertices[i].x, yOffset, Vertices[i].y);
            Vector3 b = new(Vertices[j].x, yOffset, Vertices[j].y);
            Debug.DrawLine(transform.TransformPoint(a), transform.TransformPoint(b), color, duration, depthTest);
            //Debug.Log(transform.TransformPoint(a) + ", " + transform.TransformPoint(b));
        }
    }

    private int nextVerticeIndex(int i)
    {
        int j;
        if (i == Vertices.Length - 1)
        {
            j = 0;
        }
        else
        {
            j = i + 1;
        }

        return j;
    }

    public bool contains(Vector2 point)
    {
        Vector2 farAwayPoint = new Vector2(VERY_LARGE_NUMBER, point.y);
        int intersections = 0;

        for (int i = 0; i < Vertices.Length; i++)
        {
            int j = nextVerticeIndex(i);
            if (LineSegmentsIntersecting(point, farAwayPoint, Vertices[i], Vertices[j])){
                intersections++;
            }
        }

        return intersections % 2 == 1;
    }

    private static bool LineSegmentsIntersecting(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
    {
        return PointsOnDifferentSides(point1, point2, point3, point4) && PointsOnDifferentSides(point3, point4, point1, point2);
    }

    private static bool PointsOnDifferentSides(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
    {
        Vector2 lineVector = point2 - point1;

        Vector2 lineNormal = new Vector2(-lineVector.y, lineVector.x);

        float dot1 = Vector2.Dot(lineNormal, point3 - point1);
        float dot2 = Vector2.Dot(lineNormal, point4 - point1);

        return dot1 * dot2 < 0f;
    }
}
