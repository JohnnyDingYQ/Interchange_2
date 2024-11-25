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
  [SerializeField]
  SnapPoint snapPointPrefab;
  public BezierCurve Curve;
  Build build;

  // Monobehaviour functions
  void Start()
  {
  }

  void OnMouseOver()
  {
    build.HoveredRoad = this;
  }

  void OnMouseExit()
  {
    build.HoveredRoad = null;
  }

  // Other functions
  public void Initialize(BezierCurve curve, int laneCount, Build build)
  {
    LaneCount = laneCount;
    Curve = curve;
    this.build = build;
    MeshFilter meshFilter = GetComponent<MeshFilter>();
    Mesh mesh = Utiliy.CreateMesh(curve, roadSettings.PointPerUnitLength, roadSettings.LaneWidth * LaneCount / 2);
    meshFilter.sharedMesh = mesh;
    GetComponent<MeshCollider>().sharedMesh = mesh;
    CreateSnapPoints();
    gameObject.name = Id.ToString();
  }

  public float3 GetLanePos(Side side, int laneIndex)
  {
    Assert.True(laneIndex < LaneCount);
    float t = side == Side.Start ? 0 : 1;
    float3 normal = Utiliy.Get2DNormal(Curve, t);
    float3 center = CurveUtility.EvaluatePosition(Curve, t);
    float3 leftMost = center + roadSettings.LaneWidth * LaneCount / 2 * normal;
    // Debug.DrawLine(center, leftMost, Color.cyan, 100);
    return leftMost - (0.5f + laneIndex) * roadSettings.LaneWidth * normal;
  }

  void CreateSnapPoints()
  {
    for (int i = 0; i < LaneCount; i++)
    {
      float3 pos = GetLanePos(Side.End, i);
      float3 tangent = math.normalizesafe(CurveUtility.EvaluateTangent(Curve, 1));
      pos += tangent * roadSettings.IntersectionSeparation / 2;
      SnapPoint snapPoint = Instantiate(snapPointPrefab, transform);

      // set transfrom of created snap points
      snapPoint.transform.position = pos;
      snapPoint.transform.localScale = new float3(roadSettings.LaneWidth / 2, 1, roadSettings.IntersectionSeparation) / 10;
      snapPoint.transform.rotation = Quaternion.LookRotation(tangent);
      snapPoint.Initialize(Id, i, build);
    }
  }
}