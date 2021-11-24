using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuNode : MonoBehaviour
{
  public bool isRoot;
  public bool isHidden;
  public Map.Node node;
  public MenuPlayerController player;

  [SerializeField]
  private UnityEvent onArrival;

  private void Update() {
    if (node != null) {
      node.SetPosition(new Vector3(transform.position.x, transform.position.y));
      if (node.playerIsOnNode && !player.IsMoving) {
        player.transform.position = node.position;
        if (!isRoot) {
          onArrival.Invoke();
        }
      }
    }
  }
}
