using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Level.LevelBonusEffect;

public static class LevelBonusTracker {
    private static Level.LevelBonusEffect bonusEffect = none;

    public static void addLevelBonusEffect(Level.LevelBonusEffect newBonusEffect) {
        bonusEffect = newBonusEffect;
        Debug.Log("adding effect" + newBonusEffect.ToString());
    }

    public static Level.LevelBonusEffect getActiveBonusEffect(Upgrade.UpgradeEffect effect) {
        return bonusEffect;
    }

    public static void reset() {
        bonusEffect = none;
    }
}