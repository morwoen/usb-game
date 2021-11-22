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
  [SerializeField]
  private ParticleSystem enemyEffect;
  [SerializeField]
  private ParticleSystem[] movementEffects;
  [SerializeField]
  private TrailRenderer trail;

  private Tween movement;
  private bool moving = false;

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

  private void OnDisable() {
    movement?.Kill();
  }

  private void Update() {
    bool shouldShow = false;
    if (moving) {
      shouldShow = node?.isKnown == true;
    } else {
      shouldShow = prevNode?.isKnown == true;
    }

    trail.enabled = shouldShow;
    if (shouldShow) {
      if (enemyEffect.isStopped) {
        enemyEffect.Play();
      }

      if (moving) {
        foreach (ParticleSystem movementEffect in movementEffects) {
          if (movementEffect.isStopped) {
            movementEffect.Play();
          }
        }
      } else {
        foreach (ParticleSystem movementEffect in movementEffects) {
          if (movementEffect.isPlaying) {
            movementEffect.Stop();
          }
        }
      }
    } else {
      if (enemyEffect.isPlaying) {
        enemyEffect.Stop();
      }

      foreach (ParticleSystem movementEffect in movementEffects) {
        if (movementEffect.isPlaying) {
          movementEffect.Stop();
        }
      }
    }
  }

  void MoveToNextNode() {
    moving = false;

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
      .OnPlay(() => moving = true)
      .OnComplete(MoveToNextNode);
  }
}
