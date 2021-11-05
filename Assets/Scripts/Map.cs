using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map
{
  Node currentNode;
  public List<Vector3Int> walls {
    get;
    private set;
  }

  public List<Vector3Int> ceilings {
    get;
    private set;
  }

  public Node CurrentNode
  {
    get { return currentNode; }
    private set { currentNode = value; }
  }

  public Map(Node root, List<Vector3Int> walls, List<Vector3Int> ceilings) {
    this.currentNode = root;
    this.walls = walls;
    this.ceilings = ceilings;
  }

  public void FollowLink(Node node) {
    this.currentNode = node;
  }

  public class Node
  {
    public enum NodeType
    {
      DeskRight,
      DeskLeft,
      CEODesk,
      Door,
      Light,
      Server,
    }

    public Node(NodeType type, Vector3Int tilemapPosition, Tilemap tilemap) {
      this.type = type;
      this.tilemapPosition = tilemapPosition;
      this.position = tilemap.CellToWorld(tilemapPosition);
      this.links = new List<Node>();
    }

    public List<Node> links;
    public Vector3Int tilemapPosition;
    public Vector3 position;
    public NodeType type;
  }
}
