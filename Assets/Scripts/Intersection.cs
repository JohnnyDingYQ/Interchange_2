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
  public uint Id;
  [SerializeField]
  Dictionary<RoadLane, RoadLane> connections;

  public void Initialize(Game game)
  {
    connections = new();
    this.game = game;
  }

  public void AddConnection(RoadLane from, RoadLane to)
  {
    connections.Add(from, to);
  }

  public void EvaluateMesh()
  {
    Trim();
    RoadLane from = connections.Keys.First();
    RoadLane to = connections.Values.First();
    if (game.TryGetRoad(from.Road, out Road road0) && game.TryGetRoad(to.Road, out Road road1))
    {
      float3 lane0 = road0.GetLanePos(Side.End, 0);
      float3 lane1 = road1.GetLanePos(Side.Start, 0);

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

  // Remove connection if road no longer exists
  // If intersection is empty, delete itself
  public void Trim()
  {
    List<RoadLane> toRemove = new();
    foreach (RoadLane from in connections.Keys)
    {
      RoadLane to = connections[from];
      if (!game.TryGetRoad(from.Road, out _) || !game.TryGetRoad(to.Road, out _))
      {
        toRemove.Add(from);
      }
    }
    toRemove.ForEach(r => connections.Remove(r));
    if (connections.Count == 0)
    {
      game.RemoveIntersection(this);
      Destroy(gameObject);
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
}