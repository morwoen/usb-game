using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
  [SerializeField]
  private float speed = 3f;

  [SerializeField]
  private GameObject directionIndicator;

  private bool isMoving = false;
  private float directionIndicatorDetectionAngle = 30f;

  public bool IsMoving {
    get { return isMoving; }
    private set {
      isMoving = value;
      directionIndicator.SetActive(!isMoving);
    }
  }

  Vector3 target1 = new Vector3(0.5f, -2.5f, 0f);
  Vector3 target2 = new Vector3(-7.5f, 3.5f, 0f);
  Vector3 target3 = new Vector3(9.5f, 3.5f, 0f);

  void Start() {
    Vector3 point1 = new Vector3(0.5f, -2.5f, 0f);
    Vector3 point2 = new Vector3(0.5f, 3.5f, 0f);
    Vector3 point3 = new Vector3(-0.5f, 3.5f, 0f);
    Vector3[] path = new Vector3[] {
      transform.position, point1, point2, point3
    };

    //Move(path);

  }

  private void Update() {
    RotateIndicatorTowardsMouse();

    Debug.DrawLine(transform.position, target1, Color.red);
    Debug.DrawLine(transform.position, target2, Color.red);
    Debug.DrawLine(transform.position, target3, Color.red);

    Vector3[] targets = new Vector3[] {
      target1, target2, target3
    };

    Vector3 minTarget = Vector3.zero;
    float minAngle = 360;
    foreach (var target in targets) {
      Vector3 direction = target - directionIndicator.transform.position;
      float angle = Vector3.Angle(directionIndicator.transform.localPosition, direction);
      if (angle < minAngle) {
        minAngle = angle;
        minTarget = target;
      }
    }

    if (minAngle <= directionIndicatorDetectionAngle) {
      Debug.DrawLine(transform.position, minTarget, Color.blue);

      if (Input.GetMouseButtonDown(0)) {
        Vector3[] path = new Vector3[] {
          minTarget
        };
        Move(path);
      }
    }
  }

  void RotateIndicatorTowardsMouse() {
    if (isMoving) return;

    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f;

    Vector3 playerToMouseDirection = mouseWorldPos - transform.position;
    Vector3 indicatorDirection = directionIndicator.transform.localPosition;

    float targetAngle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
    float currentAngle = Mathf.Atan2(indicatorDirection.y, indicatorDirection.x) * Mathf.Rad2Deg;

    directionIndicator.transform.RotateAround(transform.position, transform.forward, targetAngle - currentAngle);
  }

  void OnTweenComplete() {
    IsMoving = false;
  }

  void Move(Vector3[] path) {
    IsMoving = true;
    transform.DOPath(path, speed, gizmoColor: Color.red)
      .SetSpeedBased()
      .SetEase(Ease.Linear)
      .OnComplete(OnTweenComplete);
  }
}
