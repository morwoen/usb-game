using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
  [SerializeField]
  private Tilemap tilemap;
  [SerializeField]
  private Tile[] tiles;

  public Tilemap Tilemap
  {
    get { return tilemap; }
  }

  private Stack<Map.Node> nodeStack;
  private HashSet<Map.Node> visitedNodes;

  private int wallTileIndex = 48;
  private int ceilingTileIndex = 50;

  private int receptionDeskLeft = 2;
  private int receptionDeskRight = 3;
  private int receptionDeskFill = 4;
  private int screen = 220;

  public void Render(Map map) {
    tilemap.ClearAllTiles();

    map.walls.ForEach(vec => tilemap.SetTile(vec, tiles[wallTileIndex]));
    map.ceilings.ForEach(vec => tilemap.SetTile(vec, tiles[ceilingTileIndex]));

    Map.Node cursor = map.CurrentNode;
    nodeStack = new Stack<Map.Node>();
    visitedNodes = new HashSet<Map.Node>();
    visitedNodes.Add(cursor);

    // render root desk
    Vector3Int computerLocation = cursor.tilemapPosition;
    if (cursor.renderVariant == 1) {
      computerLocation = new Vector3Int(computerLocation.x + 3, computerLocation.y, 0);
    }
    Vector3Int computer2Location = new Vector3Int(computerLocation.x - 3, computerLocation.y, 0);
    Vector3Int deskLeft = new Vector3Int(computerLocation.x - 4, computerLocation.y - 1, 0);
    Vector3Int deskRight = new Vector3Int(computerLocation.x + 1, computerLocation.y - 1, 0);
    tilemap.SetTile(computerLocation, tiles[screen]);
    tilemap.SetTile(computer2Location, tiles[screen]);
    tilemap.SetTile(deskLeft, tiles[receptionDeskLeft]);
    tilemap.SetTile(deskRight, tiles[receptionDeskRight]);
    for (int i = computerLocation.x - 3; i <= computerLocation.x; i++) {
      tilemap.SetTile(new Vector3Int(i, computerLocation.y - 1, 0), tiles[receptionDeskFill]);
    }

    // render all screens
    foreach (Map.Node node in cursor.links) {
      nodeStack.Push(node);
    }

    while (nodeStack.Count > 0) {
      cursor = nodeStack.Pop();

      if (!visitedNodes.Add(cursor)) {
        continue;
      }

      computerLocation = cursor.tilemapPosition;
      if (cursor.renderVariant == 1) {
        computerLocation = new Vector3Int(computerLocation.x + 1, computerLocation.y, 0);
      }
      computer2Location = new Vector3Int(computerLocation.x - 1, computerLocation.y, 0);
      deskLeft = new Vector3Int(computerLocation.x - 1, computerLocation.y - 1, 0);
      deskRight = new Vector3Int(computerLocation.x, computerLocation.y - 1, 0);
      tilemap.SetTile(computerLocation, tiles[screen]);
      tilemap.SetTile(computer2Location, tiles[screen]);
      tilemap.SetTile(deskLeft, tiles[receptionDeskLeft]);
      tilemap.SetTile(deskRight, tiles[receptionDeskRight]);

      foreach (Map.Node node in cursor.links) {
        nodeStack.Push(node);
      }
    }
  }
}
