using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
  [SerializeField]
  private Tilemap tilemap;
  [SerializeField]
  private TileBase tile;
  [SerializeField]
  private int range = 6;

  private Color unseen = new Color(0, 0, 0, 1);
  private Color seen = new Color(0, 0, 0, 0.5f);
  private Color present = new Color(0, 0, 0, 0);

  private Vector3Int previousLocation;

  public void ResetFog() {
    int width = MapGenerator.buildingWidth / 2;
    for (int x = -width; x < width; x++) {
      for (int y = 0; y < (MapGenerator.levels * (MapGenerator.ceilingHeight + 1)); y++) {
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, unseen);
      }
    }
  }

  public void See(Map.Node node) {
    SetColorAround(node.tilemapPosition, seen, true);
  }

  public void UpdateLocation(Vector3 location) {
    Vector3Int tilemapPosition = tilemap.WorldToCell(location);

    if (tilemapPosition != previousLocation) {
      SetColorAround(previousLocation, seen);
      previousLocation = tilemapPosition;
      SetColorAround(tilemapPosition, present);
    }
  }

  private void SetColorAround(Vector3Int position, Color color, bool ifNotPresent = false) {
    for (int x = position.x - range; x < position.x + range; x++) {
      for (int y = position.y - range; y < position.y + range; y++) {
        var pos = new Vector3Int(x, y, 0);

        var prevColor = tilemap.GetColor(pos);
        if (ifNotPresent && prevColor == present) {
          continue;
        }

        tilemap.SetColor(pos, color);
      }
    }
  }


}
