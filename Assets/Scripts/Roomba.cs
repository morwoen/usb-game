using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roomba : MonoBehaviour
{
  private Map.Node node;
  private PlayerController player;

  private bool movingRight = true;

  private float speed = 3;
  private float withPlayerSpeed = 10;

  private void Update() {
    node?.SetPosition(transform.position);
    if (node.playerIsOnNode) {
      if (!player.IsMoving) {
        player.transform.position = transform.position;

        // NOTE: Do not change this to move through the rigid body
        // It is too funny how it bumps into walls
        // This is now a feature!
        if (Input.GetKey(KeyCode.A)) {
          transform.Translate(Vector3.left * Time.deltaTime * withPlayerSpeed);
        } else if (Input.GetKey(KeyCode.D)) {
          transform.Translate(Vector3.right * Time.deltaTime * withPlayerSpeed);
        }
      }
    } else {
      if (movingRight) {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
      } else {
        transform.Translate(Vector3.left * Time.deltaTime * speed);
      }
    }
  }

  private void OnEnable() {
    node = new Map.Node(Map.Node.NodeType.Roomba);
    player = FindObjectOfType<PlayerController>();
  }

  private void OnTriggerEnter2D(Collider2D collider) {
    NodeHolder nodeHolder = collider.gameObject.GetComponent<NodeHolder>();
    if (!nodeHolder) return;

    node.links.Add(nodeHolder.Node);
    nodeHolder.Node.links.Add(node);
  }

  private void OnTriggerExit2D(Collider2D collider) {
    NodeHolder nodeHolder = collider.gameObject.GetComponent<NodeHolder>();
    if (!nodeHolder) return;

    node.links.Remove(nodeHolder.Node);
    nodeHolder.Node.links.Remove(node);
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if (collision.gameObject.name == "WallsTilemap") {
      movingRight = !movingRight;
    }
  }
}
