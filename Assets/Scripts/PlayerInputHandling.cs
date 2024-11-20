using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandling : MonoBehaviour
{
  [SerializeField]
  Build build;
  public float2 MouseWorldPos;
  InputAction buildRoad;

  void Start()
  {
    buildRoad = InputSystem.actions.FindAction("Build");
    buildRoad.canceled += BuildRoad;
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
}