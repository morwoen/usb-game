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

  public List<Vector3Int> debug
  {
    get;
    private set;
  }

  public Node CurrentNode
  {
    get { return currentNode; }
    private set { currentNode = value; }
  }

  public Map(Node root, List<Vector3Int> walls, List<Vector3Int> ceilings, List<Vector3Int> debug) {
    this.currentNode = root;
    this.walls = walls;
    this.ceilings = ceilings;
    this.debug = debug;
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
      Light, // Not generated or rebdered ATM
      Server,
    }

    public List<Node> links
    {
      get;
      private set;
    }

    public Vector3Int tilemapPosition
    {
      get;
      private set;
    }

    public Vector3 position
    {
      get;
      private set;
    }

    public NodeType type
    {
      get;
      private set;
    }

    public bool isKnown;

    public Node(NodeType type, Vector3Int tilemapPosition, Tilemap tilemap) {
      this.type = type;
      this.tilemapPosition = tilemapPosition;
      this.position = tilemap.CellToWorld(tilemapPosition);
      this.links = new List<Node>();
      this.isKnown = false;
    }
  }
}
