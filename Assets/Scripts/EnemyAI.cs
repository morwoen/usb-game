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

  private Tween movement;

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

  private void OnDisable() {
    movement?.Kill();
  }

  void MoveToNextNode() {
    if (node.playerIsOnNode) {
      TransitionManager.Lose();
      return;
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

    Vector3[] path = Utils.GeneratePath(Node, target).ToArray();

    Node = target;

    movement = transform.DOPath(path, moveSpeed, gizmoColor: Color.red)
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
