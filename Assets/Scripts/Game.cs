using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.Splines;
using NUnit.Framework;
using System.Linq;

public class Game : MonoBehaviour
{
  [SerializeField]
  GameSettings gameSettings;
  [SerializeField]
  uint roadId;
  public Dictionary<uint, Road> Roads;
  public Dictionary<RoadLane, RoadLane> Connections;

  void Start()
  {
    Connections = new();
    Roads = new();
    roadId = 1;
  }

  public void AddRoad(Road road)
  {
    Assert.Zero(road.Id);
    road.Id = roadId;
    Roads.Add(roadId, road);
    roadId++;
  }

  public void RemoveRoad(Road road)
  {
    Roads.Remove(road.Id);
  }

  public bool TryGetRoad(uint key, out Road road)
  {
    return Roads.TryGetValue(key, out road);
  }


  public void AddConnection(RoadLane from, RoadLane to)
  {
    Connections.Add(from, to);
  }

  public void TrimAllIntersections()
  {
    // Intersections.Values.ToList().ForEach(i => i.Trim());
  }
}