using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public partial class GameManager {

    [SerializeField] ParticleSystem firework1;
    [SerializeField] ParticleSystem firework2;
    [SerializeField] ParticleSystem firework3;
    [SerializeField] ParticleSystem firework4;
    List<ParticleSystem> fireworks = new List<ParticleSystem>();

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

    private IEnumerator animateMultiplierDisplay() {
        multiplierDisplay.gameObject.SetActive(true);

        float duration = 1.5f;
        float elapsedTime = 0f;

        Vector3 startPosition = multiplierDisplay.transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, 50, 0); 

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            multiplierDisplay.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        multiplierDisplay.transform.position = startPosition;
        multiplierDisplay.gameObject.SetActive(false);
    }

    private void setupFireworks() {
        fireworks.AddRange(new ParticleSystem[] { firework1, firework2, firework3, firework4 });
    }

    private void showFireworks() {
        float randomHue = Random.Range(0.0f, 1.0f);
        foreach (var firework in fireworks) {
            Color brightFunColor = Color.HSVToRGB(randomHue, 1.0f, 1.0f, true);
            ParticleSystem.MainModule main = firework.main;
            main.startColor = brightFunColor;
            randomHue += 0.21f;

            firework.Play();
        }
    }

    private void stopFireworks() {
        foreach (var firework in fireworks) {
            firework.Stop();
        }
    }
}
