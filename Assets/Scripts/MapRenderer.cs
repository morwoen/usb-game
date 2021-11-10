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

  private int receptionDeskLeftIndex = 2;
  private int receptionDeskRightIndex = 3;
  private int receptionDeskFillIndex = 4;
  private int screenIndex = 220;
  private int terminalBaseIndex = 289;
  private int doorIndex = 179;

  private int debugIndex = 15;

  public void Render(Map map) {
    wallsTilemap.ClearAllTiles();
    groundTilemap.ClearAllTiles();

    for (int i = 0; i < nodeParent.childCount; i++) {
      Destroy(nodeParent.GetChild(i).gameObject);
    }

    map.walls.ForEach(vec => wallsTilemap.SetTile(vec, tiles[wallTileIndex]));
    map.ceilings.ForEach(vec => groundTilemap.SetTile(vec, tiles[ceilingTileIndex]));

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

    //nodeTilemap.SetTile(computerLocation, tiles[screenIndex]);
    //nodeTilemap.SetTile(computer2Location, tiles[screenIndex]);
    //nodeTilemap.SetTile(deskLeft, tiles[receptionDeskLeftIndex]);
    //nodeTilemap.SetTile(deskRight, tiles[receptionDeskRightIndex]);

    //nodeTilemap.SetColliderType(cursor.type == Map.Node.NodeType.DeskLeft ? computerLocation : computer2Location, Tile.ColliderType.None);
    //nodeTilemap.SetColliderType(deskLeft, Tile.ColliderType.None);
    //nodeTilemap.SetColliderType(deskRight, Tile.ColliderType.None);
    //for (int i = computerLocation.x - 3; i <= computerLocation.x; i++) {
    //  Vector3Int pos = new Vector3Int(i, computerLocation.y - 1, 0);
    //  nodeTilemap.SetTile(pos, tiles[receptionDeskFillIndex]);
    //  nodeTilemap.SetColliderType(pos, Tile.ColliderType.None);
    //}

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
        computer2Location = new Vector3Int(computerLocation.x - 1, computerLocation.y, 0);
        deskLeft = new Vector3Int(computerLocation.x - 1, computerLocation.y - 1, 0);
        deskRight = new Vector3Int(computerLocation.x, computerLocation.y - 1, 0);

        wallsTilemap.SetTile(computerLocation, tiles[screenIndex]);
        wallsTilemap.SetTile(computer2Location, tiles[screenIndex]);
        wallsTilemap.SetTile(deskLeft, tiles[receptionDeskLeftIndex]);
        wallsTilemap.SetTile(deskRight, tiles[receptionDeskRightIndex]);
      } else if (cursor.type == Map.Node.NodeType.CEODesk) {
        computerLocation = cursor.tilemapPosition;
        deskLeft = new Vector3Int(computerLocation.x - 4, computerLocation.y - 1, 0);
        deskRight = new Vector3Int(computerLocation.x + 1, computerLocation.y - 1, 0);

        wallsTilemap.SetTile(computerLocation, tiles[screenIndex]);
        wallsTilemap.SetTile(deskLeft, tiles[receptionDeskLeftIndex]);
        wallsTilemap.SetTile(deskRight, tiles[receptionDeskRightIndex]);

        for (int i = computerLocation.x - 3; i <= computerLocation.x; i++) {
          wallsTilemap.SetTile(new Vector3Int(i, computerLocation.y - 1, 0), tiles[receptionDeskFillIndex]);
        }
      } else if (cursor.type == Map.Node.NodeType.Door) {
        Vector3Int doorUp = new Vector3Int(cursor.tilemapPosition.x, cursor.tilemapPosition.y + 1, 0);
        Vector3Int doorMid = cursor.tilemapPosition;
        Vector3Int doorDown= new Vector3Int(cursor.tilemapPosition.x, cursor.tilemapPosition.y - 1, 0);

        wallsTilemap.SetTile(doorUp, tiles[doorIndex]);
        wallsTilemap.SetTile(doorMid, tiles[doorIndex]);
        wallsTilemap.SetTile(doorDown, tiles[doorIndex]);
      } else if (cursor.type == Map.Node.NodeType.Server) {
        Vector3Int terminalBase = new Vector3Int(cursor.tilemapPosition.x, cursor.tilemapPosition.y - 1, 0);

        wallsTilemap.SetTile(cursor.tilemapPosition, tiles[screenIndex]);
        wallsTilemap.SetTile(terminalBase, tiles[terminalBaseIndex]);
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
