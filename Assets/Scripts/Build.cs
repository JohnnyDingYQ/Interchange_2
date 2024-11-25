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
      // add connection to game
      RoadLane from = new(roadToConnect.Id, savedSnap.LaneIndex);
      RoadLane to = new(road.Id, 0);
      game.AddConnection(from, to);

      // create and evaluate intersection
      Intersection intersection = Instantiate(intersectionPrefab, roadToConnect.transform);
      intersection.Initialize(from, to, game);
      intersection.EvaluateMesh();
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
        return tangent * roadSettings.IntersectionSeparation + hoveredRoad.GetLanePos(Side.End, savedSnap.LaneIndex);
      }
      return GetPointPos(playerInputHandling.MouseWorldPos);
    }

    float3 GetPB()
    {
      if (savedSnap != null && game.TryGetRoad(savedSnap.RoadId, out Road hoveredRoad))
      {
        float3 origin = hoveredRoad.GetLanePos(Side.End, savedSnap.LaneIndex);
        float3 b = math.normalizesafe(CurveUtility.EvaluateTangent(hoveredRoad.Curve, 1));
        float3 a = GetPointPos(playerInputHandling.MouseWorldPos) - origin;
        return math.project(a, b) + origin;
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
      DeleteSnapPoints(road);

    }

    static void DeleteSnapPoints(Road road)
    {
      foreach (Transform child in road.transform)
      {
        if (child.GetComponent<SnapPoint>() != null)
        {
          Destroy(child.gameObject);
        }
      }
    }
  }
}