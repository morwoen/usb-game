using UnityEngine;
using Cinemachine;

public class CameraManager : CinemachineExtension
{
  [SerializeField]
  private float sizeOfCamera = 40;
  [SerializeField]
  private float maxViewUnderground = 3;

  private float orthSize;

  public float SizeOfCamera {
    get { return sizeOfCamera; }
  }

  private void Start() {
    var cam = GetComponent<CinemachineVirtualCamera>();
    orthSize = sizeOfCamera * Screen.height / Screen.width * 0.5f;
    cam.m_Lens.OrthographicSize = orthSize;
  }

  protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
    if (stage == CinemachineCore.Stage.Finalize) {
      var pos = state.RawPosition;

      // Fixed on X
      pos.x = 0;
      // Limit the Y value
      pos.y = Mathf.Clamp(pos.y, orthSize - maxViewUnderground, float.MaxValue);

      state.RawPosition = pos;
    }
  }
}
