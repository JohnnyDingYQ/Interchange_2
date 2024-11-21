using UnityEngine;

[CreateAssetMenu(fileName = "RoadSettings", menuName = "Scriptable Objects/RoadSettings")]
public class RoadSettings : ScriptableObject
{
  public float LaneWidth;
  public float PointPerUnitLength;
}