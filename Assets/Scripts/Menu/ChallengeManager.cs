using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChallengeManager: MenuItemManager {

    List<Challenge> challenges;
    private JSONLoader jsonLoader;
    private GameObject menuObjectPrefab;

    public ChallengeManager(JSONLoader jsonLoader, GameObject menuObjectPrefab, bool isTutorial = false) {
        this.jsonLoader = jsonLoader;
        this.menuObjectPrefab = menuObjectPrefab;
        this.challenges = jsonLoader.loadChallenges(isTutorial);
    }

    public List<MenuGameObjects> createChallengeOptions(Transform menuCanvasTransform, GameObject menuObjectPrefab) {
        return createMenuOptions<Challenge>(menuCanvasTransform, menuObjectPrefab, challenges);
    }

    public Challenge.ChallengeSeverity getSeverityForChallenge(int currentLevel, bool hasSeverity) {
        if (!hasSeverity) {
            return Challenge.ChallengeSeverity.none;
        }

        float difficultyFactor = (currentLevel - 1) / 6f; // Scale level 1-7 to 0-1

        float randomValue = UnityEngine.Random.value; // Get random float between 0 and 1

        Challenge theCurse = ChallengeTracker.getTheCurseIfActive();
        if (theCurse != null) {
            float curseEffect = theCurse.getSeverityMultiplier() / 20.0f;
            randomValue -= curseEffect;
        }

        //If level is 1, difficulty factor is 0, meaning lerp will select the first value. If level is 7, will choose the second value. 
        if (randomValue < Mathf.Lerp(0.50f, 0.00f, difficultyFactor)) return Challenge.ChallengeSeverity.veryLow;
        if (randomValue < Mathf.Lerp(0.85f, 0.05f, difficultyFactor)) return Challenge.ChallengeSeverity.low;
        if (randomValue < Mathf.Lerp(1.00f, 0.30f, difficultyFactor)) return Challenge.ChallengeSeverity.medium;
        if (randomValue < Mathf.Lerp(1.00f, 0.70f, difficultyFactor)) return Challenge.ChallengeSeverity.high;

        return Challenge.ChallengeSeverity.veryHigh;
    }

}

public static class ChallengeTracker {
    private static Challenge currentActiveChallenge;
    private static ChallengeDisplayManager challengeDisplayManager;

    //A special challenge that increases the severity of all future challenges, store it if the player has taken the curse. 
    private static Challenge theCurse;

    public static void setChallengeDisplayManager(ChallengeDisplayManager cdm) {
        challengeDisplayManager = cdm;
    }

    public static void addChallenge(Challenge challenge) {
        Debug.Log("Adding challenge: " + challenge.name);

        if (challenge.effect == Challenge.ChallengeEffect.TheCurse) {
            theCurse = challenge;
        }

        if (challenge.effect == Challenge.ChallengeEffect.ShatteredSword) {
            UpgradeTracker.removeLastAcquiredUpgrade();
        }

        challengeDisplayManager.challengeAdded(challenge);

        currentActiveChallenge = challenge;
    }

    public static Challenge getChallenge() {
        return currentActiveChallenge;
    }

    public static void reset() {
        currentActiveChallenge = null;
    }

    public static bool hasChallenge(string name) {
        if (currentActiveChallenge == null) {
            return false;
        }
        if (currentActiveChallenge.name == name) {
            return true;
        }
        return false;
    }

    public static bool hasChallenge(Challenge.ChallengeEffect effect) {
        if (currentActiveChallenge == null) {
            return false;
        }
        return currentActiveChallenge.effect == effect;
    }

    public static Challenge getTheCurseIfActive() {
        return theCurse;
    }
}
