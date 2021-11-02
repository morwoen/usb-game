using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
  // Start is called before the first frame update
  void Start() {
    Vector3 point1 = new Vector3(0.5f, -2.5f, 0f);
    Vector3 point2 = new Vector3(0.5f, 3.5f, 0f);
    Vector3 point3 = new Vector3(-0.5f, 3.5f, 0f);
    Vector3[] path = new Vector3[] {
      transform.position, point1, point2, point3
    };
    transform.DOPath(path, 3, gizmoColor: Color.red).SetSpeedBased().SetEase(Ease.Linear);
  }

  // Update is called once per frame
  void Update() {

  }
}
