using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAI : MonoBehaviour
{
  [SerializeField]
  private float waitOnNode = 3f;
  [SerializeField]
  private float moveSpeed = 5f;

  private SpriteRenderer sprite;

  private Map.Node node;
  private Map.Node prevNode;
  public Map.Node Node
  {
    get { return node; }
    set
    {
      bool wasEmpty = node == null;
      prevNode = node;
      node = value;
      if (wasEmpty) {
        transform.position = node.position;
        MoveToNextNode();
      }
    }
  }

  private void OnEnable() {
    sprite = GetComponentInChildren<SpriteRenderer>();
  }

  void MoveToNextNode() {
    if (node.playerIsOnNode) {
      Debug.Log("DEATH");
    }

    List<Map.Node> targets = node.links.Where(n => {
      bool isPrevious = n == prevNode;
      bool isDoorLeadingToNewNode = n.Type == Map.Node.NodeType.Door;
      if (isDoorLeadingToNewNode) {
        Map.Node other = n.links.FirstOrDefault(doorNode => doorNode != node && doorNode.IsComputer);
        isDoorLeadingToNewNode = isDoorLeadingToNewNode && other != null && other != prevNode;
      }

      return (!isPrevious && n.IsComputer) || isDoorLeadingToNewNode;
    }).ToList();

    Map.Node target;
    if (targets.Count > 0) {
      target = targets[Random.Range(0, targets.Count)];
      if (target.Type == Map.Node.NodeType.Door) {
        target = target.links.First(doorNode => doorNode != node);
      }
    } else {
      target = prevNode;
    }

    // TODO: Generate path like the player ones
    Vector3[] path = new Vector3[] {
      target.position
    };

    Node = target;

    transform.DOPath(path, moveSpeed, gizmoColor: Color.red)
      .SetSpeedBased()
      .SetEase(Ease.Linear)
      .SetDelay(waitOnNode)
      .OnPlay(AfterWait)
      .OnComplete(MoveToNextNode);
  }

  void AfterWait() {
    sprite.enabled = node.isKnown;
  }
}
