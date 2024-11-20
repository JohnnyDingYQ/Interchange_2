using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Build : MonoBehaviour
{
  [SerializeField]
  PlayerInputHandling playerInputHandling; Game game;
  [SerializeField]
  Road roadPrefab;
  int laneCount = 1;
  float3 PA, PB;
  bool assignedPA, assignedPB;

  public void BuildRoad()
  {
    if (!assignedPA)
    {
      PA = GetPointPos(playerInputHandling.MouseWorldPos);
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
    road.Initialize(game.GetNextRoadId(), curve, laneCount);
    assignedPA = false;
    assignedPB = false;


    static float3 GetPointPos(float2 mouseWorldPos)
    {
      return new(mouseWorldPos.x, 0, mouseWorldPos.y);
    }
  }
}