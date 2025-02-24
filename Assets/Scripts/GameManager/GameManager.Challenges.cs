using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour {

    private void handleChallenges() {
        handleSecurityCameraUpgrade();
        handleOverclockChallenge();
        handleTheGreatBelowChallenge();
    }

    private void handleOverclockChallenge() {
        if (ChallengeTracker.hasChallenge(Challenge.ChallengeEffect.Overclock)) {
            Challenge overclock = ChallengeTracker.getChallenge();
            float overclockSpeed = 0.02f * overclock.getSeverityMultiplier();
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
}
