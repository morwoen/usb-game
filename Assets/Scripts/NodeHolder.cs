using UnityEngine;

public class NodeHolder : MonoBehaviour
{
  public Map.Node Node;
  private SpriteRenderer spriteRenderer;

  private void OnEnable() {
    spriteRenderer = GetComponent<SpriteRenderer>();
    if (!spriteRenderer) {
      spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
  }

  private void Update() {
    spriteRenderer.enabled = Node.isKnown;
  }
}
