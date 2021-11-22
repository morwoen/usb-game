using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticMapPlayerController : BasePlayerController
{
  protected override void RegenerateMap() {
    MapParser parser = new MapParser(mapRenderer.WallsTilemap);
    map = parser.Parse();
    mapRenderer.Render(map);
    MissionManager.PopulateMissions(map);
    actionsRenderer.UpdateActions(map);
  }
}
