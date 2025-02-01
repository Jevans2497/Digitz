using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string message;

    public void OnPointerEnter(PointerEventData eventData) {
        TooltipManager.instance.setAndShowTooltip(message, this.transform.position);
    }

    public void OnPointerExit(PointerEventData eventData) {
        TooltipManager.instance.hideTooltip();
    }
}