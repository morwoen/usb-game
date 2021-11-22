using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerController : BasePlayerController
{
  protected override void RegenerateMap() {
    MapGenerator generator = new MapGenerator(mapRenderer.WallsTilemap);
    map = generator.Generate();
    mapRenderer.Render(map);
    MissionManager.PopulateMissions(map);
    actionsRenderer.UpdateActions(map);
  }
}
