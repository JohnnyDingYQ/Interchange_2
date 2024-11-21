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
  int laneCount = 1;
  float3 PA, PB;
  bool assignedPA, assignedPB, connectionMode;
  public uint HoveredRoad;

  void Update()
  {
  }

  public void BuildRoad()
  {
    if (!assignedPA)
    {
      if (HoveredRoad != 0 && game.TryGetRoad(HoveredRoad, out Road hovered))
      {
        connectionMode = true;
        PA = math.normalizesafe(CurveUtility.EvaluateTangent(hovered.Curve, 1)) * roadSettings.IntersectionSeparation;
      }
      else
      {
        PA = GetPointPos(playerInputHandling.MouseWorldPos);
      }
      assignedPA = true;
      return;
    }
    if (!assignedPB)
    {
      PB = GetPointPos(playerInputHandling.MouseWorldPos);
      assignedPB = true;
      return;
    }
    BezierCurve curve = new(PA, PB, GetPointPos(playerInputHandling.MouseWorldPos));
    Road road = Instantiate(roadPrefab);
    road.Initialize(game.GetNextRoadId(), curve, laneCount, this);
    game.AddRoad(road);

    if (HoveredRoad != 0)
    {
      Intersection intersection = Instantiate(intersectionPrefab);
      intersection.Initialize(game);
      intersection.AddConnection(new(HoveredRoad, 0), new(road.Id, 0));
      intersection.EvaluateMesh();
    }
    assignedPA = false;
    assignedPB = false;


    static float3 GetPointPos(float2 mouseWorldPos)
    {
      return new(mouseWorldPos.x, 0, mouseWorldPos.y);
    }
  }
}