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
  uint roadId, intersectionId;
  Dictionary<uint, Road> roads;
  Dictionary<uint, Intersection> intersections;

  void Start()
  {
    intersections = new();
    intersectionId = 1;
    roads = new();
    roadId = 1;
  }

  public void AddRoad(Road road)
  {
    Assert.Zero(road.Id);
    road.Id = roadId;
    roads.Add(roadId, road);
    roadId++;
  }

  public void RemoveRoad(Road road)
  {
    roads.Remove(road.Id);
  }

  public bool TryGetRoad(uint key, out Road road)
  {
    return roads.TryGetValue(key, out road);
  }

  public void AddIntersection(Intersection intersection)
  {
    Assert.Zero(intersection.Id);
    intersection.Id = intersectionId;
    intersections.Add(intersectionId, intersection);
    intersectionId++;
  }

  public void RemoveIntersection(Intersection intersection)
  {
    intersections.Remove(intersection.Id);
  }

  public bool TryGetIntersection(uint key, out Intersection intersection)
  {
    return intersections.TryGetValue(key, out intersection);
  }

  public void TrimAllIntersections()
  {
    intersections.Values.ToList().ForEach(i => i.Trim());
  }
}