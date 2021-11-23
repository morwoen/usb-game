using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuPlayerController : PlayerController
{
  private bool init = false;
  private float delay = 0.1f;

  private void Awake() {
    this.speed = 60f;
  }

  public override void RegenerateMap() {
    List<MenuNode> menuNodes = FindObjectsOfType<MenuNode>().Where(n => n.gameObject.activeSelf).ToList();
    MenuNode rootMenuNode = menuNodes.First(n => n.isRoot);
    Map.Node root = new Map.Node(Map.Node.NodeType.Roomba, 0);
    rootMenuNode.node = root;
    rootMenuNode.player = this;
    root.SetPosition(new Vector3(rootMenuNode.transform.position.x, rootMenuNode.transform.position.y));

    List<Map.Node> nodes = new List<Map.Node>();
    menuNodes.Where(n => !n.isRoot).ToList().ForEach(n => {
      Map.Node node = new Map.Node(Map.Node.NodeType.Roomba, 0);
      node.SetPosition(new Vector3(n.transform.position.x, n.transform.position.y));
      node.isKnown = true;
      nodes.Add(node);
      root.links.Add(node);
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

  IEnumerator NavigateAfterDelay(Map.Node origin, Map.Node dest) {
    yield return new WaitForSeconds(delay);
    NavigateBetween(origin, dest);
  }
}
