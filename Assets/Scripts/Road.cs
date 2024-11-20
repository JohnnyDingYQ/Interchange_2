using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Road : MonoBehaviour
{
  [SerializeField]
  uint id;
  [SerializeField]
  int laneCount;
  [SerializeField]
  GameSettings gameSettings;
  BezierCurve curve;

  void Start()
  {
  }

  public void Initialize(uint id, BezierCurve curve, int laneCount)
  {
    this.id = id;
    this.laneCount = laneCount;
    this.curve = curve;
    MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
    meshFilter.sharedMesh = CreateMesh();

    Mesh CreateMesh()
    {
      DistanceToInterpolation[] lut = new DistanceToInterpolation[10];
      CurveUtility.CalculateCurveLengths(curve, lut);
      float length = CurveUtility.CalculateLength(curve);
      int resolution = 10;
      Mesh mesh = new();

      // Create vertices
      List<Vector3> vertices = new();
      for (int i = 0; i < resolution; i++)
      {
        float t = CurveUtility.GetDistanceToInterpolation(lut, length * i / (resolution - 1));
        float3 posOnCurve = CurveUtility.EvaluatePosition(curve, t);
        float3 normal = math.normalizesafe(math.cross(new float3(0, 1, 0), CurveUtility.EvaluateTangent(curve, t)));
        vertices.Add(posOnCurve + normal * gameSettings.laneWidth * laneCount);
        vertices.Add(posOnCurve - normal * gameSettings.laneWidth * laneCount);
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

      mesh.SetVertices(vertices);
      mesh.SetTriangles(triangles, 0);
      mesh.SetNormals(normals);
      return mesh;
    }
  }
}