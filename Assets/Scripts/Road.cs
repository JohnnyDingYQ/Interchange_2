using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer))]
public class Road : MonoBehaviour
{
  public uint Id;
  public int LaneCount;
  [SerializeField]
  RoadSettings roadSettings;
  public BezierCurve Curve;
  Build build;

  // Monobehaviour functions
  void Start()
  {
  }

  void OnMouseOver()
  {
    build.HoveredRoad = Id;
  }
  void OnMouseExit()
  {
    // build.HoveredRoad = 0;
  }

  // Other functions
  public void Initialize(uint id, BezierCurve curve, int laneCount, Build build)
  {
    Id = id;
    LaneCount = laneCount;
    this.Curve = curve;
    this.build = build;
    MeshFilter meshFilter = GetComponent<MeshFilter>();
    Mesh mesh = Utiliy.CreateMesh(curve, roadSettings.PointPerUnitLength, roadSettings.LaneWidth * LaneCount / 2);
    meshFilter.sharedMesh = mesh;
    GetComponent<MeshCollider>().sharedMesh = mesh;
  }

  public float3 GetLanePos(Side side, int laneIndex)
  {
    Assert.True(laneIndex < LaneCount);
    float t = side == Side.Start ? 0 : 1;
    float3 normal = GetNormal(t);
    float3 center = CurveUtility.EvaluatePosition(Curve, t);
    float3 leftMost = center + roadSettings.LaneWidth * LaneCount / 2 * normal;
    // return leftMost + (0.5f + laneIndex) * roadSettings.LaneWidth;
    return center;
  }

  float3 GetNormal(float t)
  {
    float3 normal = math.cross(new float3(0, 1, 0), CurveUtility.EvaluateTangent(Curve, t));
    normal.y = 0;
    return math.normalizesafe(normal);
  }

  Mesh CreateMesh()
  {
    DistanceToInterpolation[] lut = new DistanceToInterpolation[10];
    CurveUtility.CalculateCurveLengths(Curve, lut);
    float length = CurveUtility.CalculateLength(Curve);
    int numPoints = (int)math.ceil(length * roadSettings.PointPerUnitLength);
    Mesh mesh = new();

    // Create vertices
    List<Vector3> vertices = new();
    for (int i = 0; i < numPoints; i++)
    {
      float t = CurveUtility.GetDistanceToInterpolation(lut, length * i / (numPoints - 1));
      float3 posOnCurve = CurveUtility.EvaluatePosition(Curve, t);
      float3 normal = GetNormal(t);
      vertices.Add(posOnCurve + roadSettings.LaneWidth * LaneCount / 2 * normal);
      vertices.Add(posOnCurve - roadSettings.LaneWidth * LaneCount / 2 * normal);
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
    float width = LaneCount * roadSettings.LaneWidth;
    for (int i = 0; i < numPoints; i++)
    {
      float distanceOncurve = length * i / (numPoints - 1);
      uvs[i * 2] = new(0, distanceOncurve / width);
      uvs[i * 2 + 1] = new(1, distanceOncurve / width);
    }

    mesh.SetVertices(vertices);
    mesh.SetTriangles(triangles, 0);
    mesh.SetNormals(normals);
    mesh.SetUVs(0, uvs);
    return mesh;
  }
}