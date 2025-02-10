using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Arrow: MonoBehaviour {

    [SerializeField] GameManager gameManager;
    [SerializeField] ArrowFeedback arrowFeedbackPrefab;
    [SerializeField] SpawnedArrowManager spawnedArrowManager;

    private float scaleFactor = 1.2f;
    private float animationDuration = 0.1f;

    private bool isInputEnabled = true;

    private Color defaultColor = Color.white;

    private bool isSecurityCameraUpgradeArrow;

    private void Update() {
        CheckForInput();
    }

    private void CheckForInput() {
        if (!isInputEnabled) {
            return;
        }

        Dictionary<KeyCode, string> keyToArrowMap = new Dictionary<KeyCode, string> {
        { KeyCode.LeftArrow, "LeftArrow" },
        { KeyCode.UpArrow, "UpArrow" },
        { KeyCode.RightArrow, "RightArrow" },
        { KeyCode.DownArrow, "DownArrow" }
    };

        foreach (var entry in keyToArrowMap) {
            if (Input.GetKeyDown(entry.Key) && this.CompareTag(entry.Value)) {
                StartCoroutine(arrowButtonPressed(this.gameObject));
                break;
            }
        }
    }

    private IEnumerator arrowButtonPressed(GameObject arrow) {
        isInputEnabled = false;

        checkContact(arrow);

        Vector3 originalScale = arrow.transform.localScale;
        Vector3 targetScale = originalScale * scaleFactor;

        arrow.transform.localScale = targetScale;
        yield return new WaitForSeconds(animationDuration);

        arrow.transform.localScale = originalScale;

        isInputEnabled = true;

    }

    private void checkContact(GameObject arrow) {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(arrow.transform.position, new Vector2(1.0f, 1.0f), 0.0f);
        GameObject closestArrow = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D collider in colliders) {
            if (collider.CompareTag(arrow.tag) && collider != this.GetComponent<BoxCollider2D>()) {
                // We only want the collider of the closest arrow, otherwise with overlapping spawned arrows, it'll arbitrarily pick which one
                float distance = Vector2.Distance(arrow.transform.position, collider.transform.position);
                if (distance < minDistance) {
                    minDistance = distance;
                    closestArrow = collider.gameObject;
                }
            }
        }

        if (closestArrow != null) {
            detectedContactWithArrow(minDistance, closestArrow);
        } else if (UpgradeTracker.hasUpgrade(Upgrade.UpgradeEffect.Bandit) && gameManager.isInSongLoop()) {
            handleScoring(-1, false);
        }
    }

    private void detectedContactWithArrow(float minDistance, GameObject closestArrow) {
        float clampedDistance = Mathf.Clamp01(minDistance);
        bool isGoldenArrow = closestArrow.layer == 7;
        Destroy(closestArrow);
        handleScoring(clampedDistance, isGoldenArrow);
    }

    public void handleScoring(float threshold, bool isGoldenArrow) {
        GameObject canvas = GameObject.Find("MainCanvas");
        if (canvas != null) {
            ArrowFeedback arrowFeedbackInstance = Instantiate(arrowFeedbackPrefab, canvas.transform);

            if (isSecurityCameraUpgradeArrow) {
                threshold = Mathf.Min(threshold, 0.29f);
            }

            FeedbackData feedbackData = new(threshold, gameManager, isGoldenArrow);
            gameManager.addToScore(feedbackData.score);
            arrowFeedbackInstance.displayFeedback(feedbackData);
            StartCoroutine(changeColorOfArrow(feedbackData));
        } else {
            Debug.LogError("Canvas not found in the scene!");
        }
    }

    public void setThisArrowToSecurityCameraUpgradeArrow() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.green;
        defaultColor = Color.green;
        isSecurityCameraUpgradeArrow = true;
    }

    public void resetArrowForSecurityCameraUpgrade() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        defaultColor = Color.white;
        isSecurityCameraUpgradeArrow = false;
    }

    private IEnumerator changeColorOfArrow(FeedbackData feedbackData) {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            if (feedbackData.isGradient) {
                StartCoroutine(applyRainbowEffect(spriteRenderer, 0.5f));
                yield return null;
            } else {
                spriteRenderer.color = feedbackData.color;
                yield return new WaitForSeconds(0.35f);
                spriteRenderer.color = defaultColor;
            }

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
