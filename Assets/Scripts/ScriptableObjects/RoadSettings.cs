using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class RoadSettings : ScriptableObject
{
  float laneWidth;
  float pointPerUnitLength;
}