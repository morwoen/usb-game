using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MapParser
{
  private Tilemap tilemap;

  protected List<StaticMapNode> staticNodes = null;

  protected StaticMapPlayerController GetPlayerController() {
    return SceneManager
      .GetActiveScene()
      .GetRootGameObjects()
      .Select(ro => ro.GetComponentsInChildren<StaticMapPlayerController>())
      .Where(pc => pc.Length > 0)
      .SelectMany(x => x) // flatten
      .ElementAtOrDefault(0);
  }

  protected Map.Node ConstructFullNode(StaticMapNode placeholder) {
    return new Map.Node(placeholder.GetNodeType(), Vector3Int.RoundToInt(placeholder.transform.position), tilemap, 0);
  }

  protected Map.Node ConstructSimpleNode(StaticMapNode placeholder) {
    return new Map.Node(placeholder.GetNodeType(), 0);
  }

  protected List<Map.Node> GetNodes(Map.Node.NodeType[] types, Map.Node root, Func<StaticMapNode, Map.Node> ConstructNode) {
    if (staticNodes == null) {
      staticNodes = SceneManager
        .GetActiveScene()
        .GetRootGameObjects()
        .Select(ro => ro.GetComponentsInChildren<StaticMapNode>())
        .SelectMany(x => x) // flatten
        .Cast<StaticMapNode>()
        .ToList();
    }

    return staticNodes
      .Where(n => types.Contains(n.GetNodeType()))
      .Select(n => {
        Map.Node node = ConstructNode(n);

        node.isKnown = n.IsKnown();

        if (n.LinksToRoot()) {
          root.links.Add(node);
        }

        return node;
      })
      .ToList();
  }

  protected List<Map.Node> GetFullNodes(Map.Node.NodeType[] types, Map.Node root) {
    return GetNodes(types, root, ConstructFullNode);
  }

  protected List<Vector3Int> GetSimpleNodes(Map.Node.NodeType[] types, Map.Node root) {
    return GetNodes(types, root, ConstructSimpleNode).Select(n => n.tilemapPosition).ToList();
  }

  protected Map.Node CreateRootNode() {
    StaticMapPlayerController playerController = GetPlayerController();
    Debug.Assert(playerController != null, "MapParser requires a StaticMapPlayerController in the scene");

    Map.Node root = new Map.Node(
      Map.Node.NodeType.Server,
      Vector3Int.RoundToInt(playerController.transform.position),
      tilemap,
      0 // level
    );

    root.isKnown = true;

    return root;
  }

  public Map Parse() {
    Map.Node.NodeType[] staticNodeTypes = new Map.Node.NodeType[] {
      Map.Node.NodeType.CEODesk,
      Map.Node.NodeType.CoffeeMachine,
      Map.Node.NodeType.Desk,
      Map.Node.NodeType.Door,
      Map.Node.NodeType.Server,
      Map.Node.NodeType.WaterDispenser
    };

    Map.Node.NodeType[] positionNodeTypes = new Map.Node.NodeType[] {
      Map.Node.NodeType.Roomba
    };

    Map.Node root = CreateRootNode();
    List<Map.Node> nodes = GetFullNodes(staticNodeTypes, root);
    List<Vector3Int> roombas = GetSimpleNodes(positionNodeTypes, root);
    List<Vector3Int> walls = new List<Vector3Int>();
    List<Vector3Int> ceilings = new List<Vector3Int>();
    List<Vector3Int> debug = new List<Vector3Int>();

    return new Map(root, nodes, walls, ceilings, roombas, debug);
  }

  public MapParser(Tilemap tilemap) {
    this.tilemap = tilemap;
  }
}
