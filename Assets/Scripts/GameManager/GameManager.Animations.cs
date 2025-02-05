using System.Collections;
using UnityEngine;
using TMPro;

public partial class GameManager {
    private IEnumerator showAndFadeLevelName(TextMeshProUGUI textMeshObject, float duration) {
        Color white = Color.white;
        Color transparentColor = new Color(white.r, white.g, white.b, 0);
        textMeshObject.color = transparentColor;

        float elapsedTime = 0f;
        float fifthOfDuration = duration / 5.0f;

        textMeshObject.enabled = true;

        // Fade in
        while (elapsedTime < (fifthOfDuration * 2.0f)) {
            float t = elapsedTime / (fifthOfDuration * 2.0f);
            textMeshObject.color = Color.Lerp(transparentColor, white, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMeshObject.color = white;
        yield return new WaitForSeconds(fifthOfDuration * 2);
        elapsedTime = 0.0f;

        // Fade out
        while (elapsedTime < fifthOfDuration) {
            float t = elapsedTime / fifthOfDuration;
            textMeshObject.color = Color.Lerp(white, transparentColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMeshObject.color = transparentColor;
        textMeshObject.enabled = false;
    }

    private IEnumerator changeSpriteAlpha(SpriteRenderer spriteRenderer, float delay, float duration, float alpha) {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
