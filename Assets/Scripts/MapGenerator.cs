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

  public static int levels
  {
    get;
    private set;
  } = 5;

  public static int buildingWidth
  {
    get;
    private set;
  } = 32;

  public static int ceilingHeight
  {
    get;
    private set;
  } = 6;

  private int buildingHalfWidth;
  private int receptionDeskOffset = 4;
  private int ceoOfficeWidth = 21;
  private int minRoomSize = 5;
  private int computerSpaceAround = 2;

  private int roombaChancePerFloor = 70;
  private int waterDispenserChancePerFloor = 70;
  private int coffeeMachineChancePerFloor = 70;

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
    Map.Node server = null;
    List<Map.Node> previousLevel = null;

    List<Map.Node> nodes = new List<Map.Node>();

    List<Vector3Int> walls = new List<Vector3Int>();
    List<Vector3Int> ceiling = new List<Vector3Int>();
    List<Vector3Int> debug = new List<Vector3Int>();
    List<Vector3Int> roombas = new List<Vector3Int>();

    // bottom floor
    for (int i = -buildingHalfWidth + 1; i < buildingHalfWidth - 1; i++) {
      ceiling.Add(new Vector3Int(i, -1, 0));
    }

    for (int level = 0; level < levels; level++) {
      int yOffset = level * (ceilingHeight + 1);

      // generate walls
      for (int wallIndex = -1; wallIndex < ceilingHeight + 1; wallIndex++) {
        walls.Add(new Vector3Int(-buildingHalfWidth, yOffset + wallIndex, 0));
        walls.Add(new Vector3Int(buildingHalfWidth - 1, yOffset + wallIndex, 0));
      }

      // generate ceilings
      for (int ceilingIndex = -buildingHalfWidth + 1; ceilingIndex < buildingHalfWidth - 1; ceilingIndex++) {
        ceiling.Add(new Vector3Int(ceilingIndex, yOffset + ceilingHeight, 0));
      }

      // Should spawn a roomba
      if (Random.Range(0, 101) < roombaChancePerFloor) {
        roombas.Add(new Vector3Int(0, yOffset, level));
      }

      // generate rooms
      if (level == 0) {
        // first level is always the reception
        int deskPosition = Random.Range(-receptionDeskOffset, receptionDeskOffset);
        root = new Map.Node(
          Map.Node.NodeType.Desk,
          new Vector3Int(deskPosition, 1, 0),
          tilemap,
          level
        );
        nodes.Add(root);
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
          tilemap,
          level
        );
        nodes.Add(door);

        for (int yIndex = yOffset + 3; yIndex < yOffset + ceilingHeight; yIndex++) {
          walls.Add(new Vector3Int(-buildingHalfWidth + leftRoomSize, yIndex, 0));
        }

        int xOffset = variant == 0 ? ceoOfficeWidth + 1 : 0;
        server = new Map.Node(
          Map.Node.NodeType.Server,
          new Vector3Int(-buildingHalfWidth + xOffset + Mathf.FloorToInt(serverRoomSize / 2), yOffset + 1, 0),
          tilemap,
          level
        );
        nodes.Add(server);

        xOffset = variant == 0 ? 0 : serverRoomSize + 1;
        Map.Node ceoComputer = new Map.Node(
          Map.Node.NodeType.CEODesk,
          new Vector3Int(-buildingHalfWidth + xOffset + Mathf.FloorToInt(ceoOfficeWidth / 2), yOffset + 1, 0),
          tilemap,
          level
        );
        nodes.Add(ceoComputer);

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

        bool hasCoffeeMachine = false;
        bool hasWaterDispenser = false;

        List<Map.Node> nodeOnLevel = new List<Map.Node>();
        Map.Node door = null;

        List<Map.Node> nodesToLinkToFromPreviousIteration = new List<Map.Node>();

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
              Map.Node.NodeType.Desk,
              new Vector3Int(nodeGlobalOffset, yOffset + 1, 0),
              tilemap,
              level
            );
            nodes.Add(node);

            if (debugging) {
              debug.Add(node.tilemapPosition);
              Debug.Log($"{nodeGlobalOffset} {yOffset + 1}");
            }

            // link to any nodes from the previous room
            if (nodesToLinkToFromPreviousIteration.Count > 0) {
              foreach (Map.Node nodeFromPreviousIteration in nodesToLinkToFromPreviousIteration) {
                node.links.Add(nodeFromPreviousIteration);
                nodeFromPreviousIteration.links.Add(node);
              }
              nodesToLinkToFromPreviousIteration = new List<Map.Node>();
            }

            // link the door next to it if present
            if (nodeIndex == 0 && door != null) {
              node.links.Add(door);
              door.links.Add(node);

              if (debugging) {
                Debug.DrawLine(door.position, node.position, Color.red, 60);
              }
            } else if (nodeIndex > 0) {
              Map.Node previousNode = nodeOnLevel[nodeOnLevel.Count - 1];
              previousNode.links.Add(node);
              node.links.Add(previousNode);

              if (debugging) {
                Debug.DrawLine(previousNode.position, node.position, Color.red, 60);
              }
            }

            nodeOnLevel.Add(node);

            nodeOffset += nodeSize + computerSpaceAround * 2;

            if (!hasCoffeeMachine) {
              int coffeeMachineGlobalOffset = nodeGlobalOffset + computerSpaceAround + 2;
              if (maxNodeSpace - nodeSize > 2) {
                if (Random.Range(0, 101) < coffeeMachineChancePerFloor) {
                  Map.Node coffeeMachine = new Map.Node(
                    Map.Node.NodeType.CoffeeMachine,
                    new Vector3Int(coffeeMachineGlobalOffset, yOffset + 2, 0),
                    tilemap,
                    level
                  );
                  nodes.Add(coffeeMachine);

                  node.links.Add(coffeeMachine);
                  coffeeMachine.links.Add(node);
                  nodesToLinkToFromPreviousIteration.Add(coffeeMachine);

                  nodeOffset += 2;
                  hasCoffeeMachine = true;
                  continue;
                }
              }
            }

            if (!hasWaterDispenser) {
              int waterDispenserGlobalOffset = nodeGlobalOffset + computerSpaceAround + 1;
              if (maxNodeSpace - nodeSize > 1) {
                if (Random.Range(0, 101) < waterDispenserChancePerFloor) {
                  Map.Node waterDispenser = new Map.Node(
                    Map.Node.NodeType.WaterDispenser,
                    new Vector3Int(waterDispenserGlobalOffset, yOffset + 2, 0),
                    tilemap,
                    level
                  );
                  nodes.Add(waterDispenser);

                  node.links.Add(waterDispenser);
                  waterDispenser.links.Add(node);
                  nodesToLinkToFromPreviousIteration.Add(waterDispenser);

                  nodeOffset += 1;
                  hasWaterDispenser = true;
                  continue;
                }
              }
            }
          }

          if (remainingRooms > 0) {
            // create and link door
            door = new Map.Node(
              Map.Node.NodeType.Door,
              new Vector3Int(roomGlobalOffset + roomSize + 1, yOffset + 1, 0),
              tilemap,
              level
            );
            nodes.Add(door);

            if (nodeOnLevel.Count > 0) {
              Map.Node finalNodeInLastRoom = nodeOnLevel[nodeOnLevel.Count - 1];
              door.links.Add(finalNodeInLastRoom);
              finalNodeInLastRoom.links.Add(door);

              if (debugging) {
                debug.Add(door.tilemapPosition);
                Debug.DrawLine(door.position, finalNodeInLastRoom.position, Color.red, 60);
              }
            }
          }

          xOffset += roomSize + 1;
        }

        // link to previous level
        int linksBetweenLevels = Random.Range(1, Mathf.Min(3, nodeOnLevel.Count));
        HashSet<Tuple<Map.Node, Map.Node>> links = new HashSet<Tuple<Map.Node, Map.Node>>();
        for (int linkIndex = 0; linkIndex < linksBetweenLevels; linkIndex++) {
          int rndSourceIndex = Random.Range(0, nodeOnLevel.Count);
          int rndTargetIndex = Random.Range(0, previousLevel.Count);
          Map.Node sourceNode = nodeOnLevel[rndSourceIndex];
          Map.Node targetNode = previousLevel[rndTargetIndex];
          Tuple<Map.Node, Map.Node> tuple = Tuple.Create(sourceNode, targetNode);
          bool found = links.Add(tuple);

          int iterations = 0;
          while (!found && iterations < maxIterationsOnLinks) {
            rndSourceIndex = Random.Range(0, nodeOnLevel.Count);
            rndTargetIndex = Random.Range(0, previousLevel.Count);
            sourceNode = nodeOnLevel[rndSourceIndex];
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
          nodeOnLevel[0].links.Add(previousLevel[0]);
          previousLevel[0].links.Add(nodeOnLevel[0]);

          if (debugging) {
            Debug.DrawLine(nodeOnLevel[0].position, previousLevel[0].position, Color.red, 60);
          }
        }

        previousLevel = nodeOnLevel;
      }
    }

    root.isKnown = true;
    Map map = new Map(root, nodes, walls, ceiling, roombas, debug);

    return map;
  }
}
