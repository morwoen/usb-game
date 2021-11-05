using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
  [SerializeField]
  private Tilemap tilemap;
  [SerializeField]
  public Tile[] tiles;

  public Tilemap Tilemap
  {
    get { return tilemap; }
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
    tilemap.ClearAllTiles();

    map.walls.ForEach(vec => tilemap.SetTile(vec, tiles[wallTileIndex]));
    map.ceilings.ForEach(vec => tilemap.SetTile(vec, tiles[ceilingTileIndex]));

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
    tilemap.SetTile(computerLocation, tiles[screenIndex]);
    tilemap.SetTile(computer2Location, tiles[screenIndex]);
    tilemap.SetTile(deskLeft, tiles[receptionDeskLeftIndex]);
    tilemap.SetTile(deskRight, tiles[receptionDeskRightIndex]);
    for (int i = computerLocation.x - 3; i <= computerLocation.x; i++) {
      tilemap.SetTile(new Vector3Int(i, computerLocation.y - 1, 0), tiles[receptionDeskFillIndex]);
    }

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

        tilemap.SetTile(computerLocation, tiles[screenIndex]);
        tilemap.SetTile(computer2Location, tiles[screenIndex]);
        tilemap.SetTile(deskLeft, tiles[receptionDeskLeftIndex]);
        tilemap.SetTile(deskRight, tiles[receptionDeskRightIndex]);
      } else if (cursor.type == Map.Node.NodeType.CEODesk) {
        computerLocation = cursor.tilemapPosition;
        deskLeft = new Vector3Int(computerLocation.x - 4, computerLocation.y - 1, 0);
        deskRight = new Vector3Int(computerLocation.x + 1, computerLocation.y - 1, 0);

        tilemap.SetTile(computerLocation, tiles[screenIndex]);
        tilemap.SetTile(deskLeft, tiles[receptionDeskLeftIndex]);
        tilemap.SetTile(deskRight, tiles[receptionDeskRightIndex]);

        for (int i = computerLocation.x - 3; i <= computerLocation.x; i++) {
          tilemap.SetTile(new Vector3Int(i, computerLocation.y - 1, 0), tiles[receptionDeskFillIndex]);
        }
      } else if (cursor.type == Map.Node.NodeType.Door) {
        Vector3Int doorUp = new Vector3Int(cursor.tilemapPosition.x, cursor.tilemapPosition.y + 1, 0);
        Vector3Int doorMid = cursor.tilemapPosition;
        Vector3Int doorDown= new Vector3Int(cursor.tilemapPosition.x, cursor.tilemapPosition.y - 1, 0);

        tilemap.SetTile(doorUp, tiles[doorIndex]);
        tilemap.SetTile(doorMid, tiles[doorIndex]);
        tilemap.SetTile(doorDown, tiles[doorIndex]);
      } else if (cursor.type == Map.Node.NodeType.Server) {
        Vector3Int terminalBase = new Vector3Int(cursor.tilemapPosition.x, cursor.tilemapPosition.y - 1, 0);
        
        tilemap.SetTile(cursor.tilemapPosition, tiles[screenIndex]);
        tilemap.SetTile(terminalBase, tiles[terminalBaseIndex]);
      }

      foreach (Map.Node node in cursor.links) {
        toBeVisited.Push(node);
      }
    }

    foreach (var node in map.debug) {
      tilemap.SetTile(node, tiles[debugIndex]);
    }
  }
}
