using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Node[] links;
    public Vector3Int tilemapPosition;
    public Vector3 position;
    public int renderVariant;
  }
}
