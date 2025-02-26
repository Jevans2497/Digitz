using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChallengeTracker {
    private static Challenge currentActiveChallenge;
    private static ChallengeDisplayManager challengeDisplayManager;

    //A special challenge that increases the likelihood for challenges to be more severe, store it if the player has taken the curse. 
    private static Challenge theCurse;

    //A special challenge that increases the severity of all future challenges, store it if the player has taken Pandora's Box.
    private static Challenge pandorasBox;

    public static void setChallengeDisplayManager(ChallengeDisplayManager cdm) {
        challengeDisplayManager = cdm;
    }

    public static void addChallenge(Challenge challenge) {
        if (challenge.effect == Challenge.ChallengeEffect.TheCurse) {
            theCurse = challenge;
        }

        if (challenge.effect == Challenge.ChallengeEffect.PandorasBox) {
            pandorasBox = challenge;
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

    public static Challenge getPandorasBoxIfActive() {
        return pandorasBox;
    }
}
