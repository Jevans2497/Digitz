using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager: MonoBehaviour {

    [SerializeField] Slider progressBar;
    Color currentColor;
    Image fill;

    private bool isCurrentlyRainbow;
    private float rainbowElapsedTime;
    private float rainbowDuration;
    private float rainbowSpeed = 3.5f;

    Gradient gradient = new Gradient();

    private List<ProgressBarProgressMarker> progressMarkers = new List<ProgressBarProgressMarker> {
        //new(0.25f),
        //new(0.5f),
        //new(0.75f),
        //new(1.0f),
    };

    private void Start() {
        fill = progressBar.transform.Find("Fill Area").gameObject.transform.Find("Fill").GetComponent<Image>();

        gradient.SetKeys(
            new GradientColorKey[] {
            new GradientColorKey(new Color(0.4f, 1f, 0.6f), 0.0f),   // Minty Green
            new GradientColorKey(new Color(0.3f, 0.9f, 0.7f), 0.1f), // Teal
            new GradientColorKey(new Color(0.5f, 0.7f, 1f), 0.3f),   // Sky Blue
            new GradientColorKey(new Color(1f, 1f, 0.6f), 0.5f),     // Soft Yellow
            new GradientColorKey(new Color(1f, 0.9f, 0.3f), 0.6f),   // Gold
            new GradientColorKey(new Color(1f, 0.65f, 0f), 0.7f),    // Orange
            new GradientColorKey(new Color(1f, 0.4f, 0f), 0.85f),    // Deep Orange
            new GradientColorKey(new Color(0.8f, 0f, 0f), 1.0f)      // Intense Red
            },
            new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0.0f),
            new GradientAlphaKey(1f, 1.0f)
            }
        );
    }

    void Update() {
        if (!isCurrentlyRainbow) {
            setColorForProgressBar();
        } else {
            applyRainbowEffect();
        }
    }


    private void setColorForProgressBar() {
        float progress = progressBar.value / progressBar.maxValue;
        Color progressBarColor = gradient.Evaluate(progress);

        float oscillationSpeed = Mathf.Max(progress * 10f, 4f);
        float alpha = 1 - ((Mathf.Sin(Time.time * oscillationSpeed) + 1f) / 5.0f); // oscillates alpha between 1f and 0.8f
        progressBarColor.a = alpha;

        currentColor = progressBarColor;
        fill.color = currentColor;
    }

    private void triggerRainbowEffectIfExpected(float progress) {
        foreach (var progressMarker in progressMarkers) {
            if (!progressMarker.hasTriggered && Mathf.Abs(progressMarker.progressTriggerAmount - progress) <= 0.05f) {
                progressMarker.hasTriggered = true;
                rainbowDuration = 1.5f;
                rainbowElapsedTime = 0.0f;
                isCurrentlyRainbow = true;
            }
        }
    }

    private void applyRainbowEffect() {
        if (rainbowElapsedTime < rainbowDuration) {
            // Generate a color using HSV (Hue, Saturation, Value)
            float hue = Mathf.Repeat(rainbowElapsedTime * rainbowSpeed, 1f); // Cycles hue from 0 to 1
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f); // Saturation and Value are maxed
            fill.color = rainbowColor;

            rainbowElapsedTime += Time.deltaTime;
        } else {
            isCurrentlyRainbow = false;
            fill.color = currentColor;
        }
    }
}

class ProgressBarProgressMarker {
    public bool hasTriggered;
    public float progressTriggerAmount;

    public ProgressBarProgressMarker(float progressTriggerAmount) {
        this.progressTriggerAmount = progressTriggerAmount;
    }
}
