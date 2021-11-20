using UnityEngine;
using TMPro;

public class TextMeshProWithBackground : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI text;
  [SerializeField]
  private RectTransform background;
  [SerializeField]
  private bool initOnAwake = false;

  private void Awake() {
    if (initOnAwake) {
      SetText(text.text);
    }
  }

  public void SetText(string txt) {
    text.SetText(txt);
    text.ForceMeshUpdate();
    background.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
    background.anchoredPosition = new Vector2(text.preferredWidth / 2, -text.preferredHeight / 2);
  }
}
