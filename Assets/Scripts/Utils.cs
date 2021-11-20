using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
  public static List<Vector3> GeneratePath(Map.Node start, Map.Node end) {
    List<Vector3> path = new List<Vector3>();
    int levelHeight = MapGenerator.ceilingHeight + 1;

    // Go to the floor
    Vector3 currentPathPosition = new Vector3(start.position.x, levelHeight * start.level - 0.5f);
    path.Add(currentPathPosition);

    // Cross level navigation
    if (start.level < end.level) {
      for (int l = start.level; l < end.level; l++) {
        List<float> possiblePathsUp = new List<float>();

        // The 2 walls on each side of the building
        possiblePathsUp.Add(MapGenerator.buildingWidth / 2 - 0.5f);
        possiblePathsUp.Add(-MapGenerator.buildingWidth / 2 + 0.5f);

        // Any door node on the level as it signifies a wall
        start.map.nodes
          .Where(n => n.Type == Map.Node.NodeType.Door && n.level == l)
          .ToList()
          .ForEach(n => {
            possiblePathsUp.Add(n.position.x);
          });

        float nextX = possiblePathsUp
          .OrderBy(possibleX => Mathf.Abs(possibleX - currentPathPosition.x))
          .First();

        path.Add(new Vector3(nextX, l * levelHeight - 0.5f));
        currentPathPosition = new Vector3(nextX, (l + 1) * levelHeight - 0.5f);
        path.Add(currentPathPosition);
      }
    } else if (start.level > end.level) {
      for (int l = start.level - 1; l >= end.level; l--) {
        List<float> possiblePathsUp = new List<float>();

        // The 2 walls on each side of the building
        possiblePathsUp.Add(MapGenerator.buildingWidth / 2 - 0.5f);
        possiblePathsUp.Add(-MapGenerator.buildingWidth / 2 + 0.5f);
        
        // Any door node on the level below as it signifies a wall
        start.map.nodes
          .Where(n => n.Type == Map.Node.NodeType.Door && n.level == l - 1)
          .ToList()
          .ForEach(n => {
            possiblePathsUp.Add(n.position.x);
          });

        float nextX = possiblePathsUp
          .OrderBy(possibleX => Mathf.Abs(possibleX - currentPathPosition.x))
          .First();

        path.Add(new Vector3(nextX, (l + 1) * levelHeight - 0.5f));
        currentPathPosition = new Vector3(nextX, l * levelHeight - 0.5f);
        path.Add(currentPathPosition);
      }
    }

    // Move through the floor to the location of the target node
    path.Add(new Vector3(end.position.x, levelHeight * end.level - 0.5f));

    // Go up to the target node
    path.Add(end.position);
    return path;
  }
}
