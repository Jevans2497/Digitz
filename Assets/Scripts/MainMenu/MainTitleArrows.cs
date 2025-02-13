using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTitleArrows: MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color defaultColor = Color.white;
    private float startTime;
    private float flashTime;

    private void Start() {
        this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        flashTime = Random.Range(0.25f, 25.0f);
    }

    void Update() {
        startTime += Time.deltaTime;
        if (startTime >= flashTime) {
            Color arrowColor = pickRandomArrowColor();
            bool isGradient = arrowColor == Color.white;
            StartCoroutine(changeColorOfArrow(arrowColor, isGradient));
            startTime = 0.0f;
            flashTime = Random.Range(0.25f, 25.0f);
        }
    }

    private Color pickRandomArrowColor() {
        Color[] colors = { Color.blue, Color.green, Color.red, Color.gray, Color.white };
        return colors[Random.Range(0, colors.Length)];
    }

    private IEnumerator changeColorOfArrow(Color color, bool isGradient) {
        if (isGradient) {
            StartCoroutine(applyRainbowEffect(spriteRenderer, 0.5f));
            yield return null;
        } else {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.35f);
            spriteRenderer.color = defaultColor;
        }

    }

    public IEnumerator applyRainbowEffect(SpriteRenderer spriteRenderer, float duration) {
        float elapsedTime = 0f;
        float rainbowSpeed = 3.5f;

        while (elapsedTime < duration) {
            // Generate a color using HSV (Hue, Saturation, Value)
            float hue = Mathf.Repeat(elapsedTime * rainbowSpeed, 1f); // Cycles hue from 0 to 1
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f); // Saturation and Value are maxed
            spriteRenderer.color = rainbowColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = defaultColor;
    }
}
