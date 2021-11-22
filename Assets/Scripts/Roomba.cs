using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roomba : MonoBehaviour
{
  [SerializeField]
  private Canvas canvas;
  [SerializeField]
  private Sprite spriteLeft;
  [SerializeField]
  private Sprite spriteRight;

  private int level;
  public int Level {
    get { return level; }
    set
    {
      level = value;
      if (node != null) {
        node.level = value;
      }
    }
  }

  private Map.Node node;
  private PlayerController player;
  private SpriteRenderer spriteRenderer;

  private bool movingRight = true;

  private float speed = 3;
  private float withPlayerSpeed = 10;
  private bool playerHasMoved = false;

  private void Update() {
    spriteRenderer.enabled = node.isKnown;

    node?.SetPosition(transform.position);
    if (node.playerIsOnNode) {
      if (!playerHasMoved && !canvas.gameObject.activeSelf) {
        canvas.gameObject.SetActive(true);
      }

      if (!player.IsMoving) {
        player.transform.position = transform.position;

        // NOTE: Do not change this to move through the rigid body
        // It is too funny how it bumps into walls
        // This is now a feature!
        if (Input.GetKey(KeyCode.A)) {
          playerHasMoved = true;
          transform.Translate(Vector3.left * Time.deltaTime * withPlayerSpeed);
          spriteRenderer.sprite = spriteLeft;
        } else if (Input.GetKey(KeyCode.D)) {
          playerHasMoved = true;
          transform.Translate(Vector3.right * Time.deltaTime * withPlayerSpeed);
          spriteRenderer.sprite = spriteRight;
        }

        if (playerHasMoved && canvas.gameObject.activeSelf) {
          canvas.gameObject.SetActive(false);
        }
      }
    } else {
      if (canvas.gameObject.activeSelf) {
        canvas.gameObject.SetActive(false);
      }

      playerHasMoved = false;

      if (movingRight) {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        spriteRenderer.sprite = spriteRight;
      } else {
        transform.Translate(Vector3.left * Time.deltaTime * speed);
        spriteRenderer.sprite = spriteLeft;
      }
    }
  }

  private void OnEnable() {
    node = new Map.Node(Map.Node.NodeType.Roomba, level);
    player = FindObjectOfType<PlayerController>();
    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
  }

  private void OnTriggerEnter2D(Collider2D collider) {
    NodeHolder nodeHolder = collider.gameObject.GetComponent<NodeHolder>();
    if (!nodeHolder) return;
    
    if (node.playerIsOnNode) {
      nodeHolder.Node.isKnown = true;
    }

    if (nodeHolder.Node.playerIsOnNode) {
      node.isKnown = true;
    }

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
