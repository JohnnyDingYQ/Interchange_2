using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.Splines;

public class Game : MonoBehaviour
{
  [SerializeField]
  GameSettings gameSettings;
  [SerializeField]
  uint roadId, intersectionsId;
  Dictionary<uint, Road> roads;
  Dictionary<uint, Intersection> intersections;

  void Start()
  {
    roads = new();
  }

  public uint GetNextRoadId()
  {
    return roadId++;
  }
}