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
  int laneCount;
  float3 PA, PB;
  bool assignedPA, assignedPB;
  public SnapPoint SelectedSnap;
  SnapPoint savedSnap;

  void Start()
  {
    laneCount = 1;
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
    road.Initialize(game.GetNextRoadId(), curve, laneCount, this);
    game.AddRoad(road);

    if (savedSnap != null && game.TryGetRoad(savedSnap.RoadId, out Road roadToConnect))
    {
      Intersection intersection = Instantiate(intersectionPrefab);
      intersection.Initialize(game);
      intersection.AddConnection(new(roadToConnect.Id, 0), new(road.Id, 0));
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
      if (SelectedSnap != null && game.TryGetRoad(SelectedSnap.RoadId, out Road hoveredRoad))
      {
        savedSnap = SelectedSnap;
        float3 tangent = math.normalizesafe(CurveUtility.EvaluateTangent(hoveredRoad.Curve, 1));
        return tangent * roadSettings.IntersectionSeparation + hoveredRoad.GetLanePos(Side.End, 0);
      }
      return GetPointPos(playerInputHandling.MouseWorldPos);
    }

    float3 GetPB()
    {
      if (savedSnap != null && game.TryGetRoad(savedSnap.RoadId, out Road hoveredRoad))
      {
        float3 origin = hoveredRoad.GetLanePos(Side.End, 0);
        float3 b = math.normalizesafe(CurveUtility.EvaluateTangent(hoveredRoad.Curve, 1));
        float3 a = GetPointPos(playerInputHandling.MouseWorldPos) - origin;
        return math.dot(a, b) / math.lengthsq(b) * b + origin;
      }
      return GetPointPos(playerInputHandling.MouseWorldPos);

    }
  }
}