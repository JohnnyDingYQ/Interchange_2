using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Intersection : MonoBehaviour
{
  Game game;
  [SerializeField]
  RoadSettings roadSettings;
  public RoadLane from;
  public RoadLane to;
  public void Initialize(RoadLane from, RoadLane to, Game game)
  {
    this.from = from;
    this.to = to;
    this.game = game;
  }

  public void EvaluateMesh()
  {
    if (game.TryGetRoad(from.Road, out Road road0) && game.TryGetRoad(to.Road, out Road road1))
    {
      float3 lane0 = road0.GetLanePos(Side.End, from.Lane);
      float3 lane1 = road1.GetLanePos(Side.Start, to.Lane);

      BezierCurve curve = new(lane0, (lane0 + lane1) / 2, lane1);
      Mesh mesh = Utiliy.CreateMesh(curve, roadSettings.PointPerUnitLength, roadSettings.LaneWidth / 2);
      GetComponent<MeshFilter>().sharedMesh = mesh;
      Debug.Log("should be successful");
    }
    else
    {
      Debug.Log("roads not found");
    }
  }

  bool FindIntersection(float2 p1, float2 d1, float2 p2, float2 d2, out float2 intersection)
  {
    static float Cross(float2 a, float2 b)
    {
      return a.x * b.y - a.y * b.x;
    }

    float det = Cross(d1, d2);

    // Check if lines are parallel (determinant is close to zero)
    if (math.abs(det) < 1e-6f)
    {
      // Lines are parallel or coincident; no unique intersection
      intersection = 0;
      return false;
    }
    else
    {
      float2 deltaP = p2 - p1;
      float t1 = Cross(deltaP, d2) / det;
      intersection = p1 + t1 * d1;
      return true;
    }

  }
}

public struct RoadLane
{
  public uint Road;
  public int Lane;

  public RoadLane(uint road, int lane)
  {
    Road = road;
    Lane = lane;
  }

  public override readonly int GetHashCode()
  {
    return HashCode.Combine(Road.GetHashCode(), Lane.GetHashCode());
  }
}