using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
  [SerializeField]
  private float speed = 6f;
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
  private Map map;

  public bool IsMoving {
    get { return isMoving; }
    private set {
      isMoving = value;
      directionIndicator.SetActive(!isMoving);
      UpdateNavigationLines();
    }
  }

  // Generate world
  // Draw generated world
  // Navigation based on the generated world
  // fog of war
  // Interact to scan the network
  // interact to do something hacking
  // highlight the direction of the data if not at the current place (when stealing data)
  // mission generation

  // click on 1 and have to go to another place before the timer runs out (can say send 3 different packets, gotta catch one)
  // loggers - install and have to do something in the meantime while it gathers data
  // finding random upgrades on machines
  // leave a spiderweb to catch something

  // minigame - balance cpu to not be detected when on the CEO computer

  // people AI
  // antivirus scans

  // music
  // sfx

  // menu
  // Settings
  // credits

  // intro (the USBus or just the person taking you in)

  // camera always fit https://answers.unity.com/questions/760671/resizing-orthographic-camera-to-fit-2d-sprite-on-s.html

  private void Awake() {
    mapRenderer = FindObjectOfType<MapRenderer>();
    lineRenderers = GetComponentsInChildren(typeof(LineRenderer))
      .Select(c => (LineRenderer)c)
      .ToList();
  }

  void Start() {
    RegenerateMap();

    transform.position = map.CurrentNode.position;

    UpdateNavigationLines();
  }

  private void RegenerateMap() {
    MapGenerator generator = new MapGenerator(mapRenderer.Tilemap);
    map = generator.Generate();
    mapRenderer.Render(map);
  }

  private void Update() {
    Navigation();
    if (Input.GetKeyDown(KeyCode.E)) {
      RegenerateMap();
    }
  }

  private void Navigation() {
    if (IsMoving) return;

    RotateIndicatorTowardsMouse();

    Map.Node minTarget = null;
    float minAngle = 360;
    int minIndex = -1;
    for (int i = 0; i < map.CurrentNode.links.Length; i++) {
      Map.Node target = map.CurrentNode.links[i];
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
      int requiredRenderers = map.CurrentNode.links.Length;
      if (activeRenderers > requiredRenderers) {
        for (int i = requiredRenderers; i < activeRenderers; i++) {
          Destroy(lineRenderers[i].gameObject);
        }
        lineRenderers.RemoveRange(requiredRenderers, activeRenderers - requiredRenderers);
      } else if (activeRenderers < requiredRenderers) {
        for (int i = 0; i < requiredRenderers - activeRenderers; i++) {
          GameObject lrGO = Instantiate(directionLinePrefab, transform);
          LineRenderer lr = lrGO.GetComponent<LineRenderer>();
          lineRenderers.Add(lr);
        }
      }

      for (int i = 0; i < requiredRenderers; i++) {
        Vector3[] directions = new Vector3[] {
          transform.position, map.CurrentNode.links[i].position
        };

        lineRenderers[i].gameObject.SetActive(true);
        lineRenderers[i].SetPositions(directions);
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
