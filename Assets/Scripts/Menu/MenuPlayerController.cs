using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuPlayerController : PlayerController
{
  private bool init = false;
  private float delay = 0.1f;

  private void Start() {
    this.speed = 60f;
  }

  public override void RegenerateMap() {
    List<MenuNode> menuNodes = FindObjectsOfType<MenuNode>()
      .Where(n => n.gameObject.activeSelf)
      .ToList();
    MenuNode rootMenuNode = menuNodes.First(n => n.isRoot);
    Map.Node root = new Map.Node(Map.Node.NodeType.Roomba, 0);
    rootMenuNode.node = root;
    rootMenuNode.player = this;
    root.SetPosition(new Vector3(rootMenuNode.transform.position.x, rootMenuNode.transform.position.y));

    List<Map.Node> nodes = new List<Map.Node>();
    nodes.Add(root);

    menuNodes.Where(n => !n.isRoot).ToList().ForEach(n => {
      Map.Node node = new Map.Node(Map.Node.NodeType.Roomba, 0);
      node.SetPosition(new Vector3(n.transform.position.x, n.transform.position.y));
      node.isKnown = true;

      if (!n.isHidden) {
        nodes.Add(node);
        root.links.Add(node);
      }

      n.node = node;
      n.player = this;
    });


    Map.Node currentLocationNode = new Map.Node(Map.Node.NodeType.Roomba, 0);
    currentLocationNode.SetPosition(new Vector3(transform.position.x, transform.position.y));

    currentLocationNode.links.Add(root);

    map = new Map(currentLocationNode, nodes, null, null, null, null);
    if (!init) {
      transform.position = map.CurrentNode.position;
      init = true;
    }

    StartCoroutine(NavigateAfterDelay(currentLocationNode, root));
  }

  // This idea didn't work
  // leaving the code to debug after the gamejam
  internal override bool ShouldNavigate() {
    if (!Input.GetMouseButtonDown(0)) return false;

    var maskedAreas = FindObjectsOfType<MaskedArea>();
    var activeMaskedAreas = FindObjectsOfType<MaskedArea>()
      .Where(area => area.gameObject.activeSelf).ToList();

    MaskedArea maskedArea = FindObjectsOfType<MaskedArea>()
      .Where(area => area.gameObject.activeSelf)
      .FirstOrDefault(area => area.gameObject.GetComponent<RectTransform>().rect.Contains(Input.mousePosition));
    return maskedArea == null;
  }

  IEnumerator NavigateAfterDelay(Map.Node origin, Map.Node dest) {
    yield return new WaitForSeconds(delay);
    NavigateBetween(origin, dest);
  }
}
