using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
  [SerializeField]
  private Tilemap wallsTilemap;
  [SerializeField]
  private Tilemap groundTilemap;
  [SerializeField]
  public Tile[] tiles;


  [Header("Prefabs")]
  [SerializeField]
  private Transform nodeParent;
  [SerializeField]
  private GameObject computerPrefab;
  [SerializeField]
  private GameObject roombaPrefab;
  [SerializeField]
  private GameObject doorPrefab;
  [SerializeField]
  private GameObject waterDispenserPrefab;
  [SerializeField]
  private GameObject coffeeMachinePrefab;

  public Tilemap WallsTilemap
  {
    get { return wallsTilemap; }
  }
  public Tilemap GroundTilemap
  {
    get { return groundTilemap; }
  }

  private Stack<Map.Node> toBeVisited;
  private HashSet<Map.Node> visitedNodes;

  private int wallTileIndex = 48;
  private int ceilingTileIndex = 50;

  private int debugIndex = 15;

  public void Render(Map map) {
    wallsTilemap.ClearAllTiles();
    groundTilemap.ClearAllTiles();

    for (int i = 0; i < nodeParent.childCount; i++) {
      Destroy(nodeParent.GetChild(i).gameObject);
    }

    map.walls.ForEach(vec => wallsTilemap.SetTile(vec, tiles[wallTileIndex]));
    map.ceilings.ForEach(vec => groundTilemap.SetTile(vec, tiles[ceilingTileIndex]));

    // Spawn roombas
    map.roombas.ForEach(vec => Instantiate(roombaPrefab, wallsTilemap.CellToWorld(vec), Quaternion.identity, nodeParent));

    Map.Node cursor = map.CurrentNode;
    toBeVisited = new Stack<Map.Node>();
    visitedNodes = new HashSet<Map.Node>();
    visitedNodes.Add(cursor);

    // render root desk
    Vector3Int computerLocation = cursor.tilemapPosition;
    if (cursor.type == Map.Node.NodeType.DeskLeft) {
      computerLocation = new Vector3Int(computerLocation.x + 3, computerLocation.y, 0);
    }
    Vector3Int computer2Location = new Vector3Int(computerLocation.x - 3, computerLocation.y, 0);
    Vector3Int deskLeft = new Vector3Int(computerLocation.x - 4, computerLocation.y - 1, 0);
    Vector3Int deskRight = new Vector3Int(computerLocation.x + 1, computerLocation.y - 1, 0);

    NodeHolder nodeHolder = Instantiate(computerPrefab, cursor.position, Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
    nodeHolder.Node = cursor;

    // render all screens
    foreach (Map.Node node in cursor.links) {
      toBeVisited.Push(node);
    }

    while (toBeVisited.Count > 0) {
      cursor = toBeVisited.Pop();

      if (!visitedNodes.Add(cursor)) {
        continue;
      }

      if (cursor.type == Map.Node.NodeType.DeskLeft || cursor.type == Map.Node.NodeType.DeskRight) {
        computerLocation = cursor.tilemapPosition;
        if (cursor.type == Map.Node.NodeType.DeskLeft) {
          computerLocation = new Vector3Int(computerLocation.x + 1, computerLocation.y, 0);
        }

        nodeHolder = Instantiate(computerPrefab, wallsTilemap.CellToWorld(cursor.tilemapPosition), Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.type == Map.Node.NodeType.CEODesk) {
        nodeHolder = Instantiate(computerPrefab, wallsTilemap.CellToWorld(cursor.tilemapPosition), Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.type == Map.Node.NodeType.Door) {
        nodeHolder = Instantiate(doorPrefab, wallsTilemap.CellToWorld(cursor.tilemapPosition), Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.type == Map.Node.NodeType.Server) {
        nodeHolder = Instantiate(computerPrefab, wallsTilemap.CellToWorld(cursor.tilemapPosition), Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.type == Map.Node.NodeType.WaterDispenser) {
        nodeHolder = Instantiate(waterDispenserPrefab, wallsTilemap.CellToWorld(cursor.tilemapPosition), Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.type == Map.Node.NodeType.CoffeeMachine) {
        nodeHolder = Instantiate(coffeeMachinePrefab, wallsTilemap.CellToWorld(cursor.tilemapPosition), Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      }

      foreach (Map.Node node in cursor.links) {
        toBeVisited.Push(node);
      }
    }

    foreach (var node in map.debug) {
      wallsTilemap.SetTile(node, tiles[debugIndex]);
    }
  }
}
