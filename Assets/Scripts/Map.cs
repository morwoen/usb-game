using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map
{
  Node currentNode;

  public List<Node> nodes
  {
    get;
    private set;
  }

  public List<Vector3Int> walls {
    get;
    private set;
  }

  public List<Vector3Int> ceilings {
    get;
    private set;
  }

  public List<Vector3Int> roombas {
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
  public Node ServerNode
  {
    get { return nodes.FirstOrDefault(n => n.Type == Node.NodeType.Server); }
  }

  public Map(Node root, List<Node> nodes, List<Vector3Int> walls, List<Vector3Int> ceilings, List<Vector3Int> roombas, List<Vector3Int> debug) {
    this.currentNode = root;
    this.currentNode.playerIsOnNode = true;
    this.walls = walls;
    this.ceilings = ceilings;
    this.roombas = roombas;
    this.nodes = nodes;
    this.debug = debug;

    nodes.ForEach(n => n.map = this);
  }

  public void FollowLink(Node node) {
    this.currentNode.playerIsOnNode = false;
    this.currentNode = node;
    this.currentNode.playerIsOnNode = true;
  }

  public class Node
  {
    public enum NodeType
    {
      Desk,
      CEODesk,
      Server,
      Door,
      WaterDispenser,
      CoffeeMachine,
      Roomba,
    }

    public Map map;

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

    public int level
    {
      get;
      set;
    }

    [SerializeField]
    [Header("Desk Left and Desk Right are interchangeable")]
    private NodeType type;
    public NodeType Type
    {
      get { return type; }
      private set { type = value; }
    }

    public bool isKnown { get; set; }
    public bool playerIsOnNode { get; set; }

    public bool IsComputer
    {
      get
      {
        return Type == NodeType.Desk ||
          Type == NodeType.CEODesk ||
          Type == NodeType.Server;
      }
    }

    public void SetPosition(Vector3 pos) {
      if (Type != NodeType.Roomba) {
        throw new Exception("Only Roombas have the power to do this, you are not a Roomba");
      }
      position = pos;
    }

    public Node(NodeType type, Vector3Int tilemapPosition, Tilemap tilemap, int level) {
      this.Type = type;
      this.tilemapPosition = tilemapPosition;
      this.position = tilemap.CellToWorld(tilemapPosition);
      this.position = new Vector3(type == NodeType.CoffeeMachine ? this.position.x : this.position.x + 0.5f, this.position.y, this.position.z);
      this.links = new List<Node>();
      this.isKnown = false;
      this.level = level;
    }

    public Node(NodeType type, int level) {
      if (type != NodeType.Roomba) {
        throw new Exception("Use the other constructor for non-roomba nodes");
      }
      this.Type = type;
      this.isKnown = false;
      this.links = new List<Node>();
      this.level = level;
    }
  }
}
