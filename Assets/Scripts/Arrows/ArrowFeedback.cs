using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrowFeedback : MonoBehaviour {

    TextMeshProUGUI tmPro = null;

    public void Awake() {
        tmPro = GetComponentInChildren<TextMeshProUGUI>();
        if (tmPro == null) {
            Debug.LogError("TextMeshProUGUI component missing on ArrowFeedback prefab or its children!");
        } else {
            tmPro.enabled = false;
        }
    }

    public void displayFeedback(FeedbackData feedbackData) {
        //GameObject spawnedArrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
        tmPro.text = feedbackData.text;
        tmPro.color = feedbackData.color;

        //Handle Gradient for "PERFECT!" case
        if (feedbackData.isGradient) {
            tmPro.enableVertexGradient = true;
            tmPro.colorGradient = feedbackData.gradient;
        } else {
            tmPro.enableVertexGradient = false;
        }

        tmPro.enabled = true;
        StartCoroutine(displayFeedbackAnimation());
    }

    private IEnumerator displayFeedbackAnimation() {
        float elapsedTime = 0.0f;
        float duration = 0.25f;

        Vector3 startPosition = tmPro.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, 50, 0);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            tmPro.transform.position = Vector3.Lerp(tmPro.transform.position, endPosition, elapsedTime / duration);

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
