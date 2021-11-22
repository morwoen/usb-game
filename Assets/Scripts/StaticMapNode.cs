using UnityEngine;
using UnityEngine.UIElements;

public class StaticMapNode : MonoBehaviour
{
  [SerializeField]
  private Map.Node.NodeType type;

  [SerializeField]
  private bool isKnown;

  [SerializeField]
  private bool linksToRoot;

  public Map.Node.NodeType GetNodeType() {
    return type;
  }

  public bool IsKnown() {
    return isKnown;
  }

  public bool LinksToRoot() {
    return linksToRoot;
  }
}
