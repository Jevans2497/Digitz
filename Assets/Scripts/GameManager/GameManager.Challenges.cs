using System.Collections;
using System.Collections.Generic;
using static SharedResources;
using UnityEngine;

public partial class GameManager : MonoBehaviour {

    [SerializeField] GameObject detourGameObject;

    private void handleChallenges() {
        handleSecurityCameraUpgrade();
        handleOverclockChallenge();
        handleTheGreatBelowChallenge();
        handleDetourChallenge();
    }

    private void handleOverclockChallenge() {
        if (ChallengeTracker.hasChallenge(Challenge.ChallengeEffect.Overclock)) {
            Challenge overclock = ChallengeTracker.getChallenge();
            float overclockSpeed = 0.015f * overclock.getSeverityMultiplier();
            audioSource.pitch = 1 + overclockSpeed;
        } else {
            audioSource.pitch = 1;
        }
    }

    private void handleOutageChallenge() {
        if (ChallengeTracker.hasChallenge(Challenge.ChallengeEffect.Outage)) {
            Challenge outage = ChallengeTracker.getChallenge();
            foreach (var upgrade in UpgradeTracker.GetUpgrades()) {
                int randomInt = UnityEngine.Random.Range(0, 10);
                if (randomInt <= outage.getSeverityMultiplier() - 1) {
                    UpgradeTracker.disableUpgrade(upgrade);
                }
            }
        }
    }

    private void handleTheGreatBelowChallenge() {
        if (ChallengeTracker.hasChallenge(Challenge.ChallengeEffect.TheGreatBelow)) {
            Challenge theGreatBelow = ChallengeTracker.getChallenge();
            score -= theGreatBelow.getSeverityMultiplier() * (scoreNeededToClearLevel * 0.1f);
        }
    }

    private void handleDetourChallenge() {
        if (ChallengeTracker.hasChallenge(Challenge.ChallengeEffect.Detour)) {
            detourGameObject.SetActive(true);
            Direction detourDirection = spawnedArrowManager.detourRandomDirection;

            Vector2 newPosition = Vector2.zero;

            switch (detourDirection) {
                case Direction.Left:
                newPosition = new Vector2(-150, 0);
                break;
                case Direction.Right:
                newPosition = new Vector2(150, 0);
                break;
                case Direction.Up:
                newPosition = new Vector2(0, 100);
                break;
                case Direction.Down:
                newPosition = new Vector2(0, -100);
                break;
            }

            detourGameObject.GetComponent<RectTransform>().anchoredPosition = newPosition;
        } else {
            detourGameObject.SetActive(false);
        }
    }


}
