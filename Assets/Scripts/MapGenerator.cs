using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator
{
  private static bool debugging = false;

  private int seed;
  private Tilemap tilemap;

  public static int levels {
    get;
    private set;
  } = 5;

  public static int buildingWidth {
    get;
    private set;
  } = 34;

  public static int ceilingHeight {
    get;
    private set;
  } = 6;

  private int buildingHalfWidth;
  private int receptionDeskOffset = 4;
  private int ceoOfficeWidth = 21;
  private int minRoomSize = 5;
  private int computerSpaceAround = 2;

  // prevent infinite/long loops due to randomness
  private int maxIterationsOnLinks = 10;

  public MapGenerator(int seed, Tilemap tilemap) {
    this.seed = seed;
    this.tilemap = tilemap;
    buildingHalfWidth = buildingWidth / 2;
  }

  public MapGenerator(Tilemap tilemap) : this((int)DateTimeOffset.Now.ToUnixTimeSeconds(), tilemap) {
  }

  public Map Generate() {
    Random.InitState(this.seed);

    Map.Node root = null;
    List<Map.Node> previousLevel = null;

    List<Vector3Int> walls = new List<Vector3Int>();
    List<Vector3Int> ceiling = new List<Vector3Int>();
    List<Vector3Int> debug = new List<Vector3Int>();
    for (int level = 0; level < levels; level++) {
      int yOffset = level * (ceilingHeight + 1);

      // generate walls
      for (int wallIndex = 0; wallIndex < ceilingHeight + 1; wallIndex++) {
        walls.Add(new Vector3Int(-buildingHalfWidth, yOffset + wallIndex, 0));
        walls.Add(new Vector3Int(buildingHalfWidth - 1, yOffset + wallIndex, 0));
      }

      // generate ceilings
      for (int ceilingIndex = -buildingHalfWidth + 1; ceilingIndex < buildingHalfWidth - 1; ceilingIndex++) {
        ceiling.Add(new Vector3Int(ceilingIndex, yOffset + ceilingHeight, 0));
      }

      // generate rooms
      if (level == 0) {
        // first level is always the reception
        int deskPosition = Random.Range(-receptionDeskOffset, receptionDeskOffset);
        root = new Map.Node(
          Random.Range(0, 2) == 0 ? Map.Node.NodeType.DeskRight : Map.Node.NodeType.DeskLeft,
          new Vector3Int(deskPosition, 1, 0),
          tilemap
        );
        previousLevel = new List<Map.Node>();
        previousLevel.Add(root);
      } else if (level == levels - 1) {
        // last level should always have the server room and CEO room
        int variant = Random.Range(0, 2);
        int spaceAvailable = buildingWidth - 2;
        int serverRoomSize = spaceAvailable - ceoOfficeWidth - 1;

        // variant 0 - ceo office on the left
        // variant 1 - server room on the left
        int leftRoomSize = ceoOfficeWidth;
        if (variant == 1) {
          leftRoomSize = serverRoomSize;
        }

        Map.Node door = new Map.Node(
          Map.Node.NodeType.Door,
          new Vector3Int(-buildingHalfWidth + leftRoomSize, yOffset + 1, 0),
          tilemap
        );

        for (int yIndex = yOffset + 3; yIndex < yOffset + ceilingHeight; yIndex++) {
          walls.Add(new Vector3Int(-buildingHalfWidth + leftRoomSize, yIndex, 0));
        }

        int xOffset = variant == 0 ? ceoOfficeWidth + 1 : 0;
        Map.Node server = new Map.Node(
          Map.Node.NodeType.Server,
          new Vector3Int(-buildingHalfWidth + xOffset + Mathf.FloorToInt(serverRoomSize / 2), yOffset + 1, 0),
          tilemap
        );

        xOffset = variant == 0 ? 0 : serverRoomSize + 1;
        Map.Node ceoComputer = new Map.Node(
          Map.Node.NodeType.CEODesk,
          new Vector3Int(-buildingHalfWidth + xOffset + Mathf.FloorToInt(ceoOfficeWidth / 2), yOffset + 1, 0),
          tilemap
        );

        ceoComputer.links.Add(door);
        door.links.Add(ceoComputer);

        int computerLinkNodeIndex = Random.Range(0, previousLevel.Count);
        int serverLinkNodeIndex = Random.Range(0, previousLevel.Count);
        Map.Node computerLinkNode = previousLevel[computerLinkNodeIndex];
        Map.Node serverLinkNode = previousLevel[serverLinkNodeIndex];
        computerLinkNode.links.Add(ceoComputer);
        ceoComputer.links.Add(computerLinkNode);
        serverLinkNode.links.Add(server);
        server.links.Add(serverLinkNode);

        if (debugging) {
          debug.Add(door.tilemapPosition);
          debug.Add(ceoComputer.tilemapPosition);
          debug.Add(server.tilemapPosition);
          Debug.DrawLine(ceoComputer.position, computerLinkNode.position, Color.red, 60);
          Debug.DrawLine(server.position, serverLinkNode.position, Color.red, 60);
          Debug.DrawLine(door.position, ceoComputer.position, Color.red, 60);
        }

        // No need to update the previousLevel list as this is the last level
      } else {
        int spaceAvailable = buildingWidth - 2;
        int rooms = Random.Range(1, 4);
        int xOffset = 0;

        List<Map.Node> nodes = new List<Map.Node>();
        Map.Node door = null;

        for (int room = 0; room < rooms; room++) {
          int remainingRooms = rooms - room - 1;
          int maxRoomSpace = spaceAvailable - xOffset - remainingRooms * (minRoomSize + 1);
          int roomSize = remainingRooms == 0 ? maxRoomSpace : Random.Range(minRoomSize, maxRoomSpace);
          int roomGlobalOffset = -buildingHalfWidth + xOffset;

          for (int yIndex = yOffset + 3; yIndex < yOffset + ceilingHeight; yIndex++) {
            walls.Add(new Vector3Int(roomGlobalOffset + roomSize + 1, yIndex, 0));
          }

          // generate nodes
          int maxNodes = roomSize / (computerSpaceAround * 2 + 1);
          int minNodes = roomSize == minRoomSize ? 0 : Mathf.Clamp(maxNodes - 1, 1, maxNodes);
          int numberOfNodes = Random.Range(minNodes, maxNodes);

          int nodeOffset = 0;
          for (int nodeIndex = 0; nodeIndex < numberOfNodes; nodeIndex++) {
            int remainingNodes = numberOfNodes - nodeIndex - 1;
            int maxNodeSpace = roomSize - nodeOffset - remainingNodes * (computerSpaceAround * 2 + 1) - (computerSpaceAround * 2);
            int nodeSize = Random.Range(1, maxNodeSpace);
            int nodeGlobalOffset = roomGlobalOffset + nodeOffset + computerSpaceAround + nodeSize;

            Map.Node node = new Map.Node(
              Random.Range(0, 2) == 0 ? Map.Node.NodeType.DeskRight : Map.Node.NodeType.DeskLeft,
              new Vector3Int(nodeGlobalOffset, yOffset + 1, 0),
              tilemap
            );

            if (debugging) {
              debug.Add(node.tilemapPosition);
              Debug.Log($"{nodeGlobalOffset} {yOffset + 1}");
            }

            // link the door next to it if present
            if (nodeIndex == 0 && door != null) {
              node.links.Add(door);
              door.links.Add(node);
              
              if (debugging) {
                Debug.DrawLine(door.position, node.position, Color.red, 60);
              }
            } else if (nodeIndex > 0) {
              Map.Node previousNode = nodes[nodes.Count - 1];
              previousNode.links.Add(node);
              node.links.Add(previousNode);

              if (debugging) {
                Debug.DrawLine(previousNode.position, node.position, Color.red, 60);
              }
            }

            nodes.Add(node);

            nodeOffset += nodeSize + computerSpaceAround * 2;
          }

          if (remainingRooms > 0 && nodes.Count > 0) {
            // create and link door
            door = new Map.Node(
              Map.Node.NodeType.Door,
              new Vector3Int(roomGlobalOffset + roomSize + 1, yOffset + 1, 0),
              tilemap
            );


            var finalNodeInLastRoom = nodes[nodes.Count - 1];
            door.links.Add(finalNodeInLastRoom);
            finalNodeInLastRoom.links.Add(door);
            
            if (debugging) {
              debug.Add(door.tilemapPosition);
              Debug.DrawLine(door.position, finalNodeInLastRoom.position, Color.red, 60);
            }
          }

          xOffset += roomSize + 1;
        }

        // link to previous level
        int linksBetweenLevels = Random.Range(1, Mathf.Min(3, nodes.Count));
        HashSet<Tuple<Map.Node, Map.Node>> links = new HashSet<Tuple<Map.Node, Map.Node>>();
        for (int linkIndex = 0; linkIndex < linksBetweenLevels; linkIndex++) {
          int rndSourceIndex = Random.Range(0, nodes.Count);
          int rndTargetIndex = Random.Range(0, previousLevel.Count);
          Map.Node sourceNode = nodes[rndSourceIndex];
          Map.Node targetNode = previousLevel[rndTargetIndex];
          Tuple<Map.Node, Map.Node> tuple = Tuple.Create(sourceNode, targetNode);
          bool found = links.Add(tuple);

          int iterations = 0;
          while (!found && iterations < maxIterationsOnLinks) {
            rndSourceIndex = Random.Range(0, nodes.Count);
            rndTargetIndex = Random.Range(0, previousLevel.Count);
            sourceNode = nodes[rndSourceIndex];
            targetNode = previousLevel[rndTargetIndex];
            tuple = Tuple.Create(sourceNode, targetNode);
            found = links.Add(tuple);
            iterations++;
          }

          if (found) {
            sourceNode.links.Add(targetNode);
            targetNode.links.Add(sourceNode);

            if (debugging) {
              Debug.DrawLine(sourceNode.position, targetNode.position, Color.red, 60);
            }
          }
        }

        // In case the randomness fails us, just create a link so the game can continue
        if (links.Count == 0) {
          nodes[0].links.Add(previousLevel[0]);
          previousLevel[0].links.Add(nodes[0]);

          if (debugging) {
            Debug.DrawLine(nodes[0].position, previousLevel[0].position, Color.red, 60);
          }
        }

        previousLevel = nodes;
      }
    }

    root.isKnown = true;
    Map map = new Map(root, walls, ceiling, debug);

    return map;
  }
}
