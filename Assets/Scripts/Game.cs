using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.Splines;
using NUnit.Framework;

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
    roadId = 1;
  }

  public uint GetNextRoadId()
  {
    return roadId++;
  }

  public void AddRoad(Road road)
  {
    Assert.NotZero(road.Id);
    roads.Add(road.Id, road);
  }

  public bool TryGetRoad(uint key, out Road road)
  {
    return roads.TryGetValue(key, out road);
  }
}