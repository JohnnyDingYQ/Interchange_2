using System;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
  [SerializeField]
  float minHeight, maxHeight, panMultiplier, zoomMultiplier, driftDecayExponentMultiplier, spinMuliplier;
  [SerializeField]
  AnimationCurve zoomSpeed, panSpeed;
  [SerializeField]
  Vector3 cameraVelocity;
  InputAction moveCamera, spinCamera;
  public static Quaternion Quaternion { get => Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0); }

  void Start()
  {
    moveCamera = InputSystem.actions.FindAction("MoveCamera");
    spinCamera = InputSystem.actions.FindAction("SpinCamera");
  }

  void Update()
  {
    Camera.main.transform.Rotate(0, spinCamera.ReadValue<float>() * Time.deltaTime * spinMuliplier, 0, Space.World);
    SetCameraVelocity(moveCamera.ReadValue<Vector3>());
    ApplyCameraVelocity();
    cameraVelocity *= math.pow(math.E, -Time.deltaTime * driftDecayExponentMultiplier);
    ClampToBounds();
  }

  void SetCameraVelocity(Vector3 cameraOffset)
  {
    float cameraHeight = Camera.main.transform.position.y;
    float heightRatio = cameraHeight / (maxHeight - minHeight);
    cameraOffset.x *= panMultiplier * panSpeed.Evaluate(heightRatio);
    cameraOffset.z *= panMultiplier * panSpeed.Evaluate(heightRatio);
    cameraOffset.y *= zoomMultiplier * zoomSpeed.Evaluate(heightRatio);

    ApplyZoom();
    ApplyPan();

    void ApplyZoom()
    {
      if (cameraOffset.y == 0)
        return;
      Vector3 v = Camera.main.transform.position - Camera.main.ScreenToWorldPoint(new (Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
      float t = (cameraHeight + cameraOffset.y - Camera.main.transform.position.y) / v.y;
      cameraVelocity.y = (t * v).y;
    }

    void ApplyPan()
    {
      if (cameraOffset.x != 0)
        cameraVelocity.x = cameraOffset.x;
      if (cameraOffset.z != 0)
        cameraVelocity.z = cameraOffset.z;
    }
  }

  void ApplyCameraVelocity()
  {
    Camera.main.transform.position += Quaternion * cameraVelocity * Time.deltaTime;
  }

  void ClampToBounds()
  {
    float3 cameraPos = Camera.main.transform.position;
    cameraPos.y = Math.Clamp(cameraPos.y, minHeight, maxHeight);
    Camera.main.transform.position = cameraPos;
  }
}