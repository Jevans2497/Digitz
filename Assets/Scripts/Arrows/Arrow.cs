using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Arrow: MonoBehaviour {

    [SerializeField] GameManager gameManager;
    [SerializeField] ArrowFeedback arrowFeedbackPrefab;
    [SerializeField] SpawnedArrowManager spawnedArrowManager;

    [SerializeField] TextMeshProUGUI challengeHelperText;
    [SerializeField] ParticleSystem effectCompleteParticleSystem;
    [SerializeField] ParticleSystem freezeEffectParticleSystem;
    [SerializeField] ParticleSystem lightningEffectParticleSystem;
    [SerializeField] ParticleSystem fireEffectCentralizedParticleSystem;

    [SerializeField] AudioClip electricalInterferenceClip;

    private float scaleFactor = 1.2f;
    private float animationDuration = 0.1f;

    private bool isInputEnabled = true;

    private Color defaultColor = Color.white;

    private bool isSecurityCameraUpgradeArrow;

    private int frozenArrowDethawCounter = 0;
    private int tapsNeededToDethaw = 20;
    private static bool isElectricalInterferenceActive;

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

        // If electrical interference, mirror inputs
        if (isElectricalInterferenceActive) {
            keyToArrowMap = new Dictionary<KeyCode, string> {
        { KeyCode.LeftArrow, "RightArrow" },
        { KeyCode.UpArrow, "DownArrow" },
        { KeyCode.RightArrow, "LeftArrow" },
        { KeyCode.DownArrow, "UpArrow" }
    };
        }

        foreach (var entry in keyToArrowMap) {
            if (Input.GetKeyDown(entry.Key) && this.CompareTag(entry.Value)) {
                StartCoroutine(arrowButtonPressed(this.gameObject));
                break;
            }
        }
    }

    private IEnumerator arrowButtonPressed(GameObject arrow) {
        isInputEnabled = false;

        if (frozenArrowDethawCounter > 0) {
            dethawFrozenArrow();
        } else {
            checkContact(arrow);
        }

        Vector3 originalScale = arrow.transform.localScale;
        Vector3 targetScale = originalScale * scaleFactor;

        arrow.transform.localScale = targetScale;
        yield return new WaitForSeconds(animationDuration);

        arrow.transform.localScale = originalScale;

        isInputEnabled = true;

    }

    private void checkContact(GameObject arrow) {
        if (frozenArrowDethawCounter > 0) {
            dethawFrozenArrow();
            return;
        } 

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
        switch (closestArrow.layer) {
            case 8:
            detectedContactWithRainbowArrow();
            break;
            case 9:
            detectedContactWithFrozenArrow();
            break;
            case 10:
            detectedContactWithLightningArrow();
            break;
            case 11:
            detectedContactWithFireArrow();
            break;
        }

        Destroy(closestArrow);
        handleScoring(clampedDistance, isGoldenArrow);
    }

    private void detectedContactWithRainbowArrow() {
        UpgradeTracker.addRandomUpgrade(Upgrade.UpgradeEffect.AfterTheRain);
    }

    private void detectedContactWithFrozenArrow() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color fullFreezeColor = new Color(0f, 0.87f, 1f);
        spriteRenderer.color = fullFreezeColor;
        defaultColor = fullFreezeColor;
        frozenArrowDethawCounter = tapsNeededToDethaw;

        freezeEffectParticleSystem.transform.position = this.transform.position;
        freezeEffectParticleSystem.Play();

        TMP_FontAsset freezeFont = Resources.Load<TMP_FontAsset>("Fonts/FreezeFont");        
        StartCoroutine(showChallengeEffectHelper("Tap to dethaw!", freezeFont, SharedResources.hexToColor("#7DEDFF")));
    }

    private void dethawFrozenArrow() {
        frozenArrowDethawCounter -= 1;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color fullFreezeColor = new Color(0f, 0.87f, 1f);
        Color readyToDethawColor = new Color(0.686f, 0.941f, 0.980f);
        float lerpFactor = Mathf.Clamp01((tapsNeededToDethaw - frozenArrowDethawCounter) * 0.1f);
        spriteRenderer.color = Color.Lerp(fullFreezeColor, readyToDethawColor, lerpFactor);

        if (frozenArrowDethawCounter <= 0) {
            defaultColor = Color.white;
            spriteRenderer.color = Color.white;
            effectCompleteParticleSystem.transform.position = this.transform.position;
            effectCompleteParticleSystem.Play();
        }
    }

    private void detectedContactWithLightningArrow() {
        if (!lightningEffectParticleSystem.isPlaying) {
            StartCoroutine(activateElectricalInterference());
            TMP_FontAsset lightningFont = Resources.Load<TMP_FontAsset>("Fonts/LightningFont");            
            StartCoroutine(showChallengeEffectHelper("Arrow inputs mirrored!", lightningFont, SharedResources.hexToColor("#ffa100")));
        }
    }

    private IEnumerator activateElectricalInterference() {
        AudioManager.Instance.playSound(electricalInterferenceClip);
        lightningEffectParticleSystem.Play();
        isElectricalInterferenceActive = true;
        yield return new WaitForSeconds(10.0f);
        lightningEffectParticleSystem.Stop();
        isElectricalInterferenceActive = false;
    }

    private void detectedContactWithFireArrow() {
        fireEffectCentralizedParticleSystem.Play();
        StartCoroutine(spawnedArrowManager.destroyCurrentExistingArrowsWithFireEffect());
    }

    private IEnumerator showChallengeEffectHelper(string text, TMP_FontAsset font, Color color) {
        challengeHelperText.text = text;
        challengeHelperText.font = font;
        challengeHelperText.color = color;
        challengeHelperText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        challengeHelperText.gameObject.SetActive(false);
    }

    public void handleScoring(float threshold, bool isGoldenArrow) {
        GameObject canvas = GameObject.Find("MainCanvas");
        if (canvas != null) {
            ArrowFeedback arrowFeedbackInstance = Instantiate(arrowFeedbackPrefab, canvas.transform);

            if (isSecurityCameraUpgradeArrow) {
                threshold = Mathf.Min(threshold, FeedbackData.greatThreshold - 0.01f);
            }

            FeedbackData feedbackData = new(threshold, gameManager, isGoldenArrow);
            gameManager.addToScore(feedbackData.score);
            arrowFeedbackInstance.displayFeedback(feedbackData);
            if (frozenArrowDethawCounter <= 0) {
                StartCoroutine(changeColorOfArrow(feedbackData));
            }
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
                StartCoroutine(SharedResources.applyRainbowEffect(spriteRenderer, 0.5f, defaultColor));
                yield return null;
            } else {
                spriteRenderer.color = feedbackData.color;
                yield return new WaitForSeconds(0.35f);
                spriteRenderer.color = defaultColor;
            }
        }
    }
}
