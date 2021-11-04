using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator
{
  private int seed;
  private Tilemap tilemap;
  private int levels = 2;
  private int buildingWidth = 34;
  private int buildingHalfWidth;
  private int ceilingHeight = 6;

  private int receptionDeskOffset = 4;

  public MapGenerator(int seed, Tilemap tilemap) {
    this.seed = seed;
    this.tilemap = tilemap;
    buildingHalfWidth = buildingWidth / 2;
  }

  public MapGenerator(Tilemap tilemap) : this((int)DateTimeOffset.Now.ToUnixTimeSeconds(), tilemap) {
  }

  public Map Generate() {
    Random.InitState(this.seed);

    Map.Node root = new Map.Node();

    List<Vector3Int> walls = new List<Vector3Int>();
    List<Vector3Int> ceiling = new List<Vector3Int>();
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
        root.tilemapPosition = new Vector3Int(deskPosition, 1, 0);
        root.position = this.tilemap.CellToWorld(root.tilemapPosition);
        int variant = Random.Range(0, 2);
        root.renderVariant = variant;
      } else {
        int spaceAvailable = buildingWidth - 2;

        int distance = Mathf.FloorToInt(spaceAvailable / 3);

        Map.Node node1 = new Map.Node();
        Map.Node node2 = new Map.Node();
        Map.Node node3 = new Map.Node();

        node1.tilemapPosition = new Vector3Int(-buildingHalfWidth + distance, yOffset + 1, 0);
        node2.tilemapPosition = new Vector3Int(-buildingHalfWidth + distance * 2, yOffset + 1, 0);
        node3.tilemapPosition = new Vector3Int(-buildingHalfWidth + distance * 3, yOffset + 1, 0);
        node1.position = this.tilemap.CellToWorld(node1.tilemapPosition);
        node2.position = this.tilemap.CellToWorld(node2.tilemapPosition);
        node3.position = this.tilemap.CellToWorld(node3.tilemapPosition);

        node1.links = new Map.Node[] {
          node2, root
        };

        node2.links = new Map.Node[] {
          node1, node3, root
        };

        node3.links = new Map.Node[] {
          node2
        };

        root.links = new Map.Node[] {
          node1, node2
        };



        // chance to have AP, no AP on consecutive floors

        // doors

        // PCs


        // lamps
      }



    }














    Map map = new Map(root, walls, ceiling);


    return map;
  }
}
