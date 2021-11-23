using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
  [SerializeField]
  private Tilemap backgroundTilemap;
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
  private GameObject serverPrefab;
  [SerializeField]
  private GameObject ceoPrefab;
  [SerializeField]
  private GameObject receptionPrefab;
  [SerializeField]
  private GameObject roombaPrefab;
  [SerializeField]
  private GameObject doorPrefab;
  [SerializeField]
  private GameObject waterDispenserPrefab;
  [SerializeField]
  private GameObject coffeeMachinePrefab;
  [SerializeField]
  private GameObject enemyAiPrefab;

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

  private int wallTileIndex = 9;
  private int ceilingTileIndex = 9;
  private int floorTileIndex = 4;
  private int floorCeilingTileIndex = 3;
  private int backgroundFloorTileIndex = 2;
  private int backgroundTileIndex = 0;
  private int backgroundCeilingTileIndex = 1;
  private int serverRoomTile1Index = 7;
  private int serverRoomTile2Index = 8;

  private int debugIndex = 6;

  private int GetCeilingTileIndex(int y) {
    if (y <= 0) {
      return floorTileIndex;
    }

    if (y < MapGenerator.levels * MapGenerator.ceilingHeight) {
      return floorCeilingTileIndex;
    }

    return ceilingTileIndex;
  }

  public void Render(Map map) {
    wallsTilemap.ClearAllTiles();
    groundTilemap.ClearAllTiles();

    for (int i = 0; i < nodeParent.childCount; i++) {
      Destroy(nodeParent.GetChild(i).gameObject);
    }

    map.walls.ForEach(vec => wallsTilemap.SetTile(vec, tiles[wallTileIndex]));
    map.ceilings.ForEach(vec => groundTilemap.SetTile(vec, tiles[GetCeilingTileIndex(vec.y)]));

    for (int level = 0; level < MapGenerator.levels; level++) {
      int yOffset = level * (MapGenerator.ceilingHeight + 1);
      for (int x = - MapGenerator.buildingWidth / 2; x < MapGenerator.buildingWidth / 2; x++) {
        for (int y = 0; y < MapGenerator.ceilingHeight; y++) {
          if (y == 0) {
            backgroundTilemap.SetTile(new Vector3Int(x, y + yOffset, 0), tiles[backgroundFloorTileIndex]);
          } else if (y < MapGenerator.ceilingHeight - 1) {
            backgroundTilemap.SetTile(new Vector3Int(x, y + yOffset, 0), tiles[backgroundTileIndex]);
          } else {
            backgroundTilemap.SetTile(new Vector3Int(x, y + yOffset, 0), tiles[backgroundCeilingTileIndex]);
          }
        }
      }
    }

    // render server room
    int serverLevel = map.ServerNode.level;
    Map.Node doorOnServerLevel = map.nodes.First(n => n.level == serverLevel && n.Type == Map.Node.NodeType.Door);
    int leftPoint;
    int rightPoint;
    if (doorOnServerLevel.tilemapPosition.x > map.ServerNode.tilemapPosition.x) {
      leftPoint = -MapGenerator.buildingWidth / 2;
      rightPoint = doorOnServerLevel.tilemapPosition.x;
    } else {
      leftPoint = doorOnServerLevel.tilemapPosition.x;
      rightPoint = MapGenerator.buildingWidth / 2;
    }
    int serverYOffset = map.ServerNode.level * (MapGenerator.ceilingHeight + 1);
    for (int x = leftPoint; x < rightPoint; x++) {
      for (int y = 0; y < MapGenerator.ceilingHeight; y++) {
        backgroundTilemap.SetTile(new Vector3Int(x, y + serverYOffset, 0), tiles[Random.Range(0, 2) == 0 ? serverRoomTile1Index : serverRoomTile2Index]);
      }
    }


    // Spawn roombas
    map.roombas.ForEach(vec => {
      int level = vec.z;
      Roomba roomba = Instantiate(roombaPrefab, wallsTilemap.CellToWorld(vec) + 0.25f * Vector3.up, Quaternion.identity, nodeParent).GetComponent<Roomba>();
      roomba.Level = level;
    });

    Map.Node cursor = map.CurrentNode;
    toBeVisited = new Stack<Map.Node>();
    visitedNodes = new HashSet<Map.Node>();
    visitedNodes.Add(cursor);

    // render root desk
    Vector3Int computerLocation = cursor.tilemapPosition;
    Vector3Int computer2Location = new Vector3Int(computerLocation.x - 3, computerLocation.y, 0);
    Vector3Int deskLeft = new Vector3Int(computerLocation.x - 4, computerLocation.y - 1, 0);
    Vector3Int deskRight = new Vector3Int(computerLocation.x + 1, computerLocation.y - 1, 0);

    NodeHolder nodeHolder = Instantiate(receptionPrefab, cursor.position, Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
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

      if (cursor.Type == Map.Node.NodeType.Desk) {
        computerLocation = cursor.tilemapPosition;
        nodeHolder = Instantiate(computerPrefab, cursor.position, Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.Type == Map.Node.NodeType.CEODesk) {
        nodeHolder = Instantiate(ceoPrefab, cursor.position, Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.Type == Map.Node.NodeType.Door) {
        nodeHolder = Instantiate(doorPrefab, cursor.position, Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.Type == Map.Node.NodeType.Server) {
        nodeHolder = Instantiate(serverPrefab, cursor.position, Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.Type == Map.Node.NodeType.WaterDispenser) {
        nodeHolder = Instantiate(waterDispenserPrefab, cursor.position, Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      } else if (cursor.Type == Map.Node.NodeType.CoffeeMachine) {
        nodeHolder = Instantiate(coffeeMachinePrefab, cursor.position, Quaternion.identity, nodeParent).GetComponent<NodeHolder>();
        nodeHolder.Node = cursor;
      }

      foreach (Map.Node node in cursor.links) {
        toBeVisited.Push(node);
      }
    }

    EnemyAI enemy = Instantiate(enemyAiPrefab, wallsTilemap.CellToWorld(map.ServerNode.tilemapPosition), Quaternion.identity, nodeParent).GetComponent<EnemyAI>();
    enemy.Node = map.ServerNode;

    foreach (var node in map.debug) {
      wallsTilemap.SetTile(node, tiles[debugIndex]);
    }
  }
}
