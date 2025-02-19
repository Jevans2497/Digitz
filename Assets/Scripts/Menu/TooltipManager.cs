using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Nobi.UiRoundedCorners;

public class TooltipManager: MonoBehaviour {

    public static TooltipManager instance;
    [SerializeField] TextMeshProUGUI textComponent;

    private Vector2 offset = new Vector2(90, 20);

    public enum PrefabType {
        menuOption, upgradeDisplay, challengeDisplay, levelBonusDisplay
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    void Start() {
        textComponent.raycastTarget = false;
        gameObject.SetActive(false);        
    }

    public void setAndShowTooltip(string message, Vector2 target, PrefabType prefabType) {
        gameObject.SetActive(true);
        setTooltipMessage(message);

        RectTransform rectTransform = GetComponent<RectTransform>();
        if (prefabType == PrefabType.challengeDisplay) {
            rectTransform.pivot = new Vector2(1, 1);  // Bottom-left for challenge display
        } else {
            rectTransform.pivot = new Vector2(0, 0);  // Default top-right for others
        }

        transform.position = target + getOffsetForPrefabType(prefabType);
        transform.SetAsLastSibling();
    }

    public void setTooltipMessage(string message) {
        textComponent.text = message;
    }


    private Vector2 getOffsetForPrefabType(PrefabType prefabType) {
        switch (prefabType) {
            case PrefabType.menuOption:
            return new Vector2(90, 20);
            case PrefabType.upgradeDisplay:
            return new Vector2(25, 35);
            case PrefabType.challengeDisplay:
            return new Vector2(-25, -35);
            case PrefabType.levelBonusDisplay:
            return new Vector2(45, 45);
            default:
            return new Vector2(0, 0);
        }
    }

    public void hideTooltip() {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}
