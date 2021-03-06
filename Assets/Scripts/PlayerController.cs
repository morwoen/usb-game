using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
  [SerializeField]
  internal float speed = 12f;
  private float directionIndicatorDetectionAngle = 30f;

  [SerializeField]
  private float networkDiscoveryInteractionTime = 2f;
  [SerializeField]
  private float interactionTime = 5f;

  [SerializeField]
  private GameObject directionIndicator;

  [SerializeField]
  private GameObject directionLinePrefab;

  [SerializeField]
  private AnimationCurve directionLineWidthCurveSelected;
  [SerializeField]
  private AnimationCurve directionLineWidthCurveDeselected;
  [SerializeField]
  private ParticleSystem[] movementEffects;
  [SerializeField]
  private ParticleSystem playerEffect;

  private bool isMoving = false;
  private List<LineRenderer> lineRenderers;
  private MapRenderer mapRenderer;
  internal Map map;
  private ActionsRenderer actionsRenderer;
  private Tweener movement;
  private DynamicSoundEventManager soundManager;

  public bool IsMoving {
    get { return isMoving; }
    private set {
      isMoving = value;
      directionIndicator.SetActive(!isMoving);
    }
  }

  private void Awake() {
    DataManager.Load();
  }

  private void OnEnable() {
    mapRenderer = FindObjectOfType<MapRenderer>();
    actionsRenderer = FindObjectOfType<ActionsRenderer>();
    soundManager = GetComponent<DynamicSoundEventManager>();
    lineRenderers = GetComponentsInChildren(typeof(LineRenderer))
      .Select(c => (LineRenderer)c)
      .ToList();

    RegenerateMap();
    transform.position = map.CurrentNode.position;
  }

  public virtual void RegenerateMap() {
    MapGenerator generator = new MapGenerator(mapRenderer.WallsTilemap);
    map = generator.Generate();
    mapRenderer.Render(map);
    MissionManager.PopulateMissions(map);
    actionsRenderer.UpdateActions(map);
  }

  private void Update() {
    if (TransitionManager.IsTransitioning()) {
      return;
    }

    if (IsMoving) {
      foreach (ParticleSystem movementEffect in movementEffects) {
        if (movementEffect.isStopped) {
          movementEffect.Play();
        }
      }
    } else {
      foreach (ParticleSystem movementEffect in movementEffects) {
        if (movementEffect.isPlaying) {
          movementEffect.Stop();
        }
      }
    }

    UpdateNavigationLines();
    Navigation();

    if (!Interaction.IsInteracting && !IsMoving) {
      bool networkUndiscovered = map.CurrentNode.links.FirstOrDefault(node => !node.isKnown) != null;
      for (int key = (int)KeyCode.Alpha1; key < (int)KeyCode.Alpha9; key++) {
        if (Input.GetKeyDown((KeyCode)key)) {
          if (key == (int)KeyCode.Alpha1 && networkUndiscovered) {
            FindObjectOfType<TutorialManager>()?.OnInteract();
            // Discover network
            soundManager.PlayEvent("event:/hacking");
            Interaction.Interact(networkDiscoveryInteractionTime, didComplete => {
              soundManager.StopEvent("event:/hacking");
              if (didComplete) {
                soundManager.PlayEvent("event:/hack-done");
                map.CurrentNode.links.ForEach(node => node.isKnown = true);
                actionsRenderer.UpdateActions(map);
                FindObjectOfType<TutorialManager>()?.OnAfterInteract();
              }
            });
          } else {
            List<Mission.Goal> possibleActions = MissionManager.GetInteractions(map.CurrentNode);
            int interactionIndex = key - (int)KeyCode.Alpha1 - (networkUndiscovered ? 1 : 0);
            if (interactionIndex >= 0 && interactionIndex < possibleActions.Count) {
              RunInteraction(possibleActions[interactionIndex]);
            }
          }
        }
      }
    }
  }

  private void RunInteraction(Mission.Goal action) {
    soundManager.PlayEvent("event:/hacking");
    Interaction.Interact(interactionTime, didComplete => {
      soundManager.StopEvent("event:/hacking");
      if (didComplete) {
        soundManager.PlayEvent("event:/hack-done");
        MissionManager.Interact(action, map.CurrentNode);
        actionsRenderer.UpdateActions(map);
      }
    });
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

      if (ShouldNavigate()) {
        NavigateBetween(map.CurrentNode, minTarget);
      }
    }
  }

  internal virtual bool ShouldNavigate() {
    return Input.GetMouseButtonDown(0);
  }

  internal void NavigateBetween(Map.Node origin, Map.Node destination) {
    Interaction.Interrupt();
    soundManager.PlayEvent("event:/movement");
    Move(Utils.GeneratePath(origin, destination).ToArray());
    map.FollowLink(destination);
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
    StopMovementSound();

    IsMoving = false;
    actionsRenderer?.UpdateActions(map);

    map.CurrentNode.playerIsOnNode = true;

    bool isOnIot = map.CurrentNode.Type == Map.Node.NodeType.Door ||
      map.CurrentNode.Type == Map.Node.NodeType.CoffeeMachine ||
      map.CurrentNode.Type == Map.Node.NodeType.Roomba ||
      map.CurrentNode.Type == Map.Node.NodeType.WaterDispenser;
    if (isOnIot) {
      FindObjectOfType<TutorialManager>()?.OnIotDevice();
    }

    List<Mission.Goal> possibleActions = MissionManager.GetInteractions(map.CurrentNode);
    if (possibleActions.Count > 0) {
      FindObjectOfType<TutorialManager>()?.OnNodeWithMissions();
    }
  }

  void StopMovementSound() {
    soundManager.StopEvent("event:/movement");
  }

  void Move(Vector3[] path) {
    IsMoving = true;
    FindObjectOfType<TutorialManager>()?.OnMove();
    actionsRenderer?.HideActions();
    movement?.Kill();
    movement = transform.DOPath(path, speed, gizmoColor: Color.red)
      .SetSpeedBased()
      .SetEase(Ease.Linear)
      .OnKill(StopMovementSound)
      .OnComplete(OnTweenComplete);
  }

  public void Hide() {
    playerEffect.Stop();
  }

  public void Show() {
    playerEffect.Play();
  }
}
