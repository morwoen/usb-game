using UnityEngine;

public class NodeHolder : MonoBehaviour
{
  public Map.Node Node;
  public SpriteRenderer spriteRenderer;
  public bool alwaysShow;

  private void OnEnable() {
    spriteRenderer = GetComponent<SpriteRenderer>();
    if (!spriteRenderer) {
      spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
  }

  private void Update() {
    if (!alwaysShow) {
      spriteRenderer.enabled = Node.isKnown;
    }
  }
}
