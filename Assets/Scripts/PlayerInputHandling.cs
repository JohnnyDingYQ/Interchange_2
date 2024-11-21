using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandling : MonoBehaviour
{
  [SerializeField]
  Build build;
  public float2 MouseWorldPos;
  InputAction buildRoad, removeRoad;

  void Start()
  {
    buildRoad = InputSystem.actions.FindAction("Build");
    removeRoad = InputSystem.actions.FindAction("Remove");
    buildRoad.canceled += BuildRoad;
    removeRoad.canceled += RemoveRoad;
  }

  void Update()
  {
    MouseWorldPos = GetMouseworldPos();

    static float2 GetMouseworldPos()
    {
      float2 result = 0;
      float3 temp = Camera.main.ScreenToWorldPoint(new(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
      result.x = temp.x;
      result.y = temp.z;
      return result;
    }
  }
  void BuildRoad(InputAction.CallbackContext context)
  {
    build.BuildRoad();
  }

  void RemoveRoad(InputAction.CallbackContext context)
  {
    build.RemoveRoad();
  }
}