using System;
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

  public Map(Node root, List<Vector3Int> walls, List<Vector3Int> ceilings, List<Vector3Int> roombas, List<Vector3Int> debug) {
    this.currentNode = root;
    this.currentNode.playerIsOnNode = true;
    this.walls = walls;
    this.ceilings = ceilings;
    this.roombas = roombas;
    this.debug = debug;
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
      DeskRight,
      DeskLeft,
      CEODesk,
      Server,
      Door,
      Light,
      WaterDispenser,
      CoffeeMachine,
      Roomba,
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
    public bool playerIsOnNode;

    public void SetPosition(Vector3 pos) {
      if (type != NodeType.Roomba) {
        throw new Exception("Only Roombas have the power to do this, you are not a Roomba");
      }
      position = pos;
    }

    public Node(NodeType type, Vector3Int tilemapPosition, Tilemap tilemap) {
      this.type = type;
      this.tilemapPosition = tilemapPosition;
      this.position = tilemap.CellToWorld(tilemapPosition);
      this.links = new List<Node>();
      this.isKnown = false;
    }

    public Node(NodeType type) {
      if (type != NodeType.Roomba) {
        throw new Exception("Use the other constructor for non-roomba nodes");
      }
      this.type = type;
      this.isKnown = false;
      this.links = new List<Node>();
    }
  }
}
