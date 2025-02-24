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
        challenges.ForEach(challenge => challenge.isConcealed = false);

        List<MenuGameObjects> challengeMenuOptions = createMenuOptions<Challenge>(menuCanvasTransform, menuObjectPrefab, challenges);

        if (challengeMenuOptions.Count > 1 && challengeMenuOptions[1].menuItem is Challenge topChallenge) {
            topChallenge.isConcealed = true;
        }

        if (challengeMenuOptions.Count > 2 && challengeMenuOptions[2].menuItem is Challenge rightChallenge) {
            rightChallenge.isConcealed = true;
        }

        return challengeMenuOptions;
    }

    public void removeChallengeFromPool(Challenge challengeToRemove) {
        challenges.RemoveAll(challenge => challenge.effect == challengeToRemove.effect);        
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