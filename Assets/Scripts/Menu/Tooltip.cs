using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string message;
    public TooltipManager.PrefabType prefabType;

    public void OnPointerEnter(PointerEventData eventData) {
        TooltipManager.instance.setAndShowTooltip(message, this.transform.position, prefabType);
    }

    public void OnPointerExit(PointerEventData eventData) {
        TooltipManager.instance.hideTooltip();
    }
}