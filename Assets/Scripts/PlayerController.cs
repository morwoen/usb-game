using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
  [SerializeField]
  private float speed = 12f;
  private float directionIndicatorDetectionAngle = 30f;

  [SerializeField]
  private GameObject directionIndicator;

  [SerializeField]
  private GameObject directionLinePrefab;

  [SerializeField]
  private AnimationCurve directionLineWidthCurveSelected;
  [SerializeField]
  private AnimationCurve directionLineWidthCurveDeselected;

  private bool isMoving = false;
  private List<LineRenderer> lineRenderers;
  private MapRenderer mapRenderer;
  private FogOfWar fow;
  private Map map;

  public bool IsMoving {
    get { return isMoving; }
    private set {
      isMoving = value;
      directionIndicator.SetActive(!isMoving);
    }
  }

  private void OnEnable() {
    mapRenderer = FindObjectOfType<MapRenderer>();
    fow = FindObjectOfType<FogOfWar>();
    lineRenderers = GetComponentsInChildren(typeof(LineRenderer))
      .Select(c => (LineRenderer)c)
      .ToList();

    RegenerateMap();
    transform.position = map.CurrentNode.position;
  }

  private void RegenerateMap() {
    MapGenerator generator = new MapGenerator(mapRenderer.WallsTilemap);
    map = generator.Generate();
    mapRenderer.Render(map);
    fow.ResetFog();
  }

  private void Update() {
    UpdateNavigationLines();
    Navigation();

    if (Input.GetKeyDown(KeyCode.E)) {
      if (!Interaction.IsInteracting) {
        Interaction.Interact(1, didComplete => {
          if (didComplete) {
            map.CurrentNode.links.ForEach(node => node.isKnown = true);
          }
        });
      }
    }

    fow.UpdateLocation(transform.position);
  }

  private void Navigation() {
    if (IsMoving) return;

    RotateIndicatorTowardsMouse();

    Map.Node minTarget = null;
    float minAngle = 360;
    int minIndex = -1;
    List<Map.Node> visibleNodes = map.CurrentNode.links.Where(node => node.isKnown).ToList();
    for (int i = 0; i < visibleNodes.Count; i++) {
      Map.Node target = visibleNodes[i];
      Vector3 direction = target.position - directionIndicator.transform.position;
      float angle = Vector3.Angle(directionIndicator.transform.localPosition, direction);
      if (angle < minAngle) {
        minAngle = angle;
        minTarget = target;
        minIndex = i;
      }
    }

    foreach (LineRenderer lr in lineRenderers) {
      lr.widthCurve = directionLineWidthCurveDeselected;
    }

    if (minAngle <= directionIndicatorDetectionAngle) {
      lineRenderers[minIndex].widthCurve = directionLineWidthCurveSelected;

      if (Input.GetMouseButtonDown(0)) {
        Interaction.Interrupt();
        // TODO: generate an interesting path
        Vector3[] path = new Vector3[] {
          minTarget.position
        };
        Move(path);
        map.FollowLink(minTarget);
      }
    }
  }

  void UpdateNavigationLines() {
    if (IsMoving) {
      foreach(LineRenderer lr in lineRenderers) {
        lr.gameObject.SetActive(false);
      }
    } else {
      int activeRenderers = lineRenderers.Count;
      List<Map.Node> requiredRenderers = map.CurrentNode.links.Where(node => node.isKnown).ToList();
      int requiredRenderersCount = requiredRenderers.Count;
      if (activeRenderers > requiredRenderersCount) {
        for (int i = requiredRenderersCount; i < activeRenderers; i++) {
          Destroy(lineRenderers[i].gameObject);
        }
        lineRenderers.RemoveRange(requiredRenderersCount, activeRenderers - requiredRenderersCount);
      } else if (activeRenderers < requiredRenderersCount) {
        for (int i = 0; i < requiredRenderersCount - activeRenderers; i++) {
          GameObject lrGO = Instantiate(directionLinePrefab, transform);
          LineRenderer lr = lrGO.GetComponent<LineRenderer>();
          lineRenderers.Add(lr);
        }
      }

      for (int i = 0; i < requiredRenderersCount; i++) {
        Vector3[] directions = new Vector3[] {
          transform.position, requiredRenderers[i].position
        };

        lineRenderers[i].gameObject.SetActive(true);
        lineRenderers[i].SetPositions(directions);

        fow.See(requiredRenderers[i]);
      }
    }
  }

  void RotateIndicatorTowardsMouse() {
    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f;

    Vector3 playerToMouseDirection = mouseWorldPos - transform.position;
    Vector3 indicatorDirection = directionIndicator.transform.localPosition;

    float targetAngle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
    float currentAngle = Mathf.Atan2(indicatorDirection.y, indicatorDirection.x) * Mathf.Rad2Deg;

    directionIndicator.transform.RotateAround(transform.position, transform.forward, targetAngle - currentAngle);
  }

  void OnTweenComplete() {
    IsMoving = false;
  }

  void Move(Vector3[] path) {
    IsMoving = true;
    transform.DOPath(path, speed, gizmoColor: Color.red)
      .SetSpeedBased()
      .SetEase(Ease.Linear)
      .OnComplete(OnTweenComplete);
  }
}
