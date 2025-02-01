using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour {

    public static TooltipManager instance;
    [SerializeField] TextMeshProUGUI textComponent;

    private Vector2 offset = new Vector2(100, 30);

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

    public void setAndShowTooltip(string message, Vector2 target) {
        gameObject.SetActive(true);
        textComponent.text = message;
        transform.position = target + offset;
        transform.SetAsLastSibling();
    }

    public void hideTooltip() {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}
