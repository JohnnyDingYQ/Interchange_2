using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public static class Utiliy
{
  public static Mesh CreateMesh(BezierCurve curve, float pointPerUnitLength, float halfWidth)
  {
    DistanceToInterpolation[] lut = new DistanceToInterpolation[10];
    CurveUtility.CalculateCurveLengths(curve, lut);
    float length = CurveUtility.CalculateLength(curve);
    int numPoints = (int)math.ceil(length * pointPerUnitLength);
    Mesh mesh = new();

    // Create vertices
    List<Vector3> vertices = new();
    for (int i = 0; i < numPoints; i++)
    {
      float t = CurveUtility.GetDistanceToInterpolation(lut, length * i / (numPoints - 1));
      float3 posOnCurve = CurveUtility.EvaluatePosition(curve, t);
      float3 normal = GetNormal(curve, t);
      vertices.Add(posOnCurve + halfWidth * normal);
      vertices.Add(posOnCurve - halfWidth * normal);
    }
    // Create triangles
    List<int> triangles = new();
    for (int i = 2; i < vertices.Count; i += 2)
    {
      int upperLeft = i, upperRight = i + 1;
      int lowerLeft = i - 2, lowerRight = i - 1;
      triangles.AddRange(new List<int>() { upperRight, upperLeft, lowerLeft });
      triangles.AddRange(new List<int>() { lowerLeft, lowerRight, upperRight });
    }

    // Create normals
    List<Vector3> normals = new();
    for (int i = 0; i < vertices.Count; i++)
      normals.Add(new(0, 1, 0));


    // Create uvs
    Vector2[] uvs = new Vector2[numPoints * 2];

    for (int i = 0; i < numPoints; i++)
    {
      float distanceOncurve = length * i / (numPoints - 1);
      uvs[i * 2] = new(0, distanceOncurve / (2 * halfWidth));
      uvs[i * 2 + 1] = new(1, distanceOncurve / (2 * halfWidth));
    }

    mesh.SetVertices(vertices);
    mesh.SetTriangles(triangles, 0);
    mesh.SetNormals(normals);
    mesh.SetUVs(0, uvs);
    return mesh;
  }

  public static float3 GetNormal(BezierCurve curve, float t)
  {
    float3 normal = math.cross(new float3(0, 1, 0), CurveUtility.EvaluateTangent(curve, t));
    normal.y = 0;
    return math.normalizesafe(normal);
  }

}