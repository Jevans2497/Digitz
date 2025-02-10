using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager: MonoBehaviour {

    public static TooltipManager instance;
    [SerializeField] TextMeshProUGUI textComponent;

    private Vector2 offset = new Vector2(100, 30);

    public enum PrefabType {
        menuOption, upgradeDisplay, challengeDisplay
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    void Start() {
        gameObject.SetActive(false);
    }

    public void setAndShowTooltip(string message, Vector2 target, PrefabType prefabType) {
        gameObject.SetActive(true);
        textComponent.text = message;

        RectTransform rectTransform = GetComponent<RectTransform>();
        if (prefabType == PrefabType.challengeDisplay) {
            rectTransform.pivot = new Vector2(1, 1);  // Bottom-left for challenge display
        } else {
            rectTransform.pivot = new Vector2(0, 0);  // Default top-right for others
        }

        transform.position = target + getOffsetForPrefabType(prefabType);
        transform.SetAsLastSibling();
    }


    private Vector2 getOffsetForPrefabType(PrefabType prefabType) {
        switch (prefabType) {
            case PrefabType.menuOption:
            return new Vector2(100, 30);
            case PrefabType.upgradeDisplay:
            return new Vector2(25, 35);
            case PrefabType.challengeDisplay:
            return new Vector2(-25, -35);
            default:
            return new Vector2(0, 0);
        }
    }

    public void hideTooltip() {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}
