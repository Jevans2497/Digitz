using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class SharedResources: MonoBehaviour {

    public enum Direction { Left, Up, Right, Down, None }

    public static Direction convertStringToDirection(string directionString) {
        switch (directionString) {
            case "left":
            return Direction.Left;
            case "up":
            return Direction.Up;
            case "right":
            return Direction.Right;
            case "down":
            return Direction.Down;
            default:
            return Direction.None;
        }
    }

    public static Direction getRandomDirection(Direction excludeDirection = Direction.None) {
        List<Direction> directions = new List<Direction> { Direction.Left, Direction.Up, Direction.Right, Direction.Down };
        directions.Remove(excludeDirection);
        return directions[UnityEngine.Random.Range(0, directions.Count)];
    }

    public static string getRandomDirectionAsString() {
        int randomInt = UnityEngine.Random.Range(0, 4);
        switch (randomInt) {
            case 0:
            return "left";
            case 1:
            return "up";
            case 2:
            return "right";
            case 3:
            return "down";
        }
        return "right";
    }

    public static Color hexToColor(string hex) {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color)) {
            return color;
        } else {
            Debug.LogError("Invalid Hex Color string");
            return Color.white;
        }
    }

    public static IEnumerator fadeInAudio(float fadeDuration, AudioSource audioSource) {
        float targetVolume = 1.0f;
        float startVolume = 0f;

        audioSource.Play();

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration) {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }


    public static IEnumerator fadeOutAudio(float fadeDuration, AudioSource audioSource) {
        if (audioSource == null || audioSource.gameObject == null) {
            Debug.LogWarning("fadeOutAudio called with a null or destroyed AudioSource.");
            yield break;
        }

        float startVolume = audioSource.volume;
        float targetVolume = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            if (audioSource == null || audioSource.gameObject == null) {
                Debug.LogWarning("AudioSource was destroyed during fadeOutAudio.");
                yield break;
            }

            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (audioSource != null && audioSource.gameObject != null) {
            audioSource.volume = targetVolume;
            audioSource.Stop();
            audioSource.volume = startVolume;
        }
    }

    public static IEnumerator applyRainbowEffect(SpriteRenderer spriteRenderer, float duration, Color defaultColor) {
        float elapsedTime = 0f;
        float rainbowSpeed = 3.5f;

        while (elapsedTime < duration) {
            if (spriteRenderer != null) {
                // Generate a color using HSV (Hue, Saturation, Value)
                float hue = Mathf.Repeat(elapsedTime * rainbowSpeed, 1f); // Cycles hue from 0 to 1
                Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f); // Saturation and Value are maxed
                spriteRenderer.color = rainbowColor;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (spriteRenderer != null) {
                spriteRenderer.color = defaultColor;
            }
        }
    }
}