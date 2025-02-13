using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Shadow shadow;

    void Start() {
        this.shadow = this.GetComponent<Shadow>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        shadow.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        shadow.enabled = false;
    }
}
