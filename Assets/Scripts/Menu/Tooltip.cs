using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string message;

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log($"Pointer Entered {gameObject.name}");
        TooltipManager.instance.setAndShowTooltip(message, this.transform.position);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log($"Pointer Exited {gameObject.name}");
        TooltipManager.instance.hideTooltip();
    }
}