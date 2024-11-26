using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Build : MonoBehaviour
{
  [SerializeField]
  PlayerInputHandling playerInputHandling;
  [SerializeField]
  Game game;
  [SerializeField]
  Road roadPrefab;
  [SerializeField]
  Intersection intersectionPrefab;
  [SerializeField]
  RoadSettings roadSettings;
  public int LaneCount;
  float3 PA, PB;
  bool assignedPA, assignedPB;
  public SnapPoint HoveredSnap;
  [SerializeField]
  SnapPoint savedSnap;
  public Road HoveredRoad;

  void Start()
  {
    LaneCount = 1;
  }

  void Update()
  {
  }

  public void BuildRoad()
  {
    // Handle first click
    if (!assignedPA)
    {
      PA = GetPA();
      assignedPA = true;
      return;
    }

    // Handle second click
    if (!assignedPB)
    {
      PB = GetPB();
      assignedPB = true;
      return;
    }

    // Handle third click
    BezierCurve curve = new(PA, PB, GetPointPos(playerInputHandling.MouseWorldPos));
    Road road = Instantiate(roadPrefab);
    game.AddRoad(road);
    road.Initialize(curve, LaneCount, this);

    if (savedSnap != null && game.TryGetRoad(savedSnap.RoadId, out Road roadToConnect))
    {
      List<KeyValuePair<RoadLane, RoadLane>> newConnections = new();
      if (LaneCount == 1)
      {
        newConnections.Add(new(new(roadToConnect.Id, savedSnap.LaneIndex), new(road.Id, 0)));
      }
      if (LaneCount == 2)
      {
        newConnections.Add(new(new(roadToConnect.Id, savedSnap.LaneIndex), new(road.Id, 0)));
        newConnections.Add(new(new(roadToConnect.Id, savedSnap.LaneIndex + 1), new(road.Id, 1)));
      }
      if (LaneCount == 3)
      {
        newConnections.Add(new(new(roadToConnect.Id, savedSnap.LaneIndex - 1), new(road.Id, 0)));
        newConnections.Add(new(new(roadToConnect.Id, savedSnap.LaneIndex), new(road.Id, 1)));
        newConnections.Add(new(new(roadToConnect.Id, savedSnap.LaneIndex + 1), new(road.Id, 2)));
      }

      // create and evaluate intersection
      foreach (KeyValuePair<RoadLane, RoadLane> connection in newConnections)
      {
        RoadLane from = connection.Key;
        RoadLane to = connection.Value;
        game.AddConnection(from, to);
        Intersection intersection = Instantiate(intersectionPrefab, roadToConnect.transform);
        intersection.Initialize(from, to, game);
        intersection.EvaluateMesh();
      }
      DestroyImmediate(savedSnap.gameObject);
    }
    assignedPA = false;
    assignedPB = false;
    savedSnap = null;


    static float3 GetPointPos(float2 mouseWorldPos)
    {
      return new(mouseWorldPos.x, 0, mouseWorldPos.y);
    }

    float3 GetPA()
    {
      if (HoveredSnap != null && game.TryGetRoad(HoveredSnap.RoadId, out Road hoveredRoad))
      {
        savedSnap = HoveredSnap;
        float3 tangent = math.normalizesafe(CurveUtility.EvaluateTangent(hoveredRoad.Curve, 1));
        float3 pos = hoveredRoad.GetLanePos(Side.End, savedSnap.LaneIndex);
        if (LaneCount == 2)
        {
          pos = math.lerp(hoveredRoad.GetLanePos(Side.End, savedSnap.LaneIndex), hoveredRoad.GetLanePos(Side.End, savedSnap.LaneIndex + 1), 0.5f);
        }
        return pos + tangent * roadSettings.IntersectionSeparation;
      }
      return GetPointPos(playerInputHandling.MouseWorldPos);
    }

    float3 GetPB()
    {
      if (savedSnap != null && game.TryGetRoad(savedSnap.RoadId, out Road hoveredRoad))
      {
        float3 b = math.normalizesafe(CurveUtility.EvaluateTangent(hoveredRoad.Curve, 1));
        float3 a = GetPointPos(playerInputHandling.MouseWorldPos) - PA;
        return math.project(a, b) + PA;
      }
      return GetPointPos(playerInputHandling.MouseWorldPos);
    }
  }

  public void RemoveRoad()
  {
    if (HoveredRoad == null)
      return;
    game.RemoveRoad(HoveredRoad);
    Destroy(HoveredRoad.gameObject);
    game.TrimAllIntersections();
  }

  public void CreateOddSnapPoints()
  {
    foreach (Road road in game.Roads.Values)
    {
      road.CreateOddSnapPoints();
    }
  }

  public void CreateEvenSnapPoints()
  {
    foreach (Road road in game.Roads.Values)
    {
      road.CreateEvenSnapPoints();
    }
  }
}