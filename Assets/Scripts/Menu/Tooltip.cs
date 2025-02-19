using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string message;
    public TooltipManager.PrefabType prefabType;
    private bool isPointerInsideGameObject;

    public void OnPointerEnter(PointerEventData eventData) {
        isPointerInsideGameObject = true;
        TooltipManager.instance.setAndShowTooltip(message, this.transform.position, prefabType);
    }

    public void manuallySetMessage(string message) {
        if (isPointerInsideGameObject) {
            this.message = message;
            TooltipManager.instance.setTooltipMessage(message);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        isPointerInsideGameObject = false;
        TooltipManager.instance.hideTooltip();
    }
}