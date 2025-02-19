using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelBonus;
using static LevelBonus.LevelBonusEffect;

public static class LevelBonusTracker {
    private static LevelBonusEffect bonusEffect = none;

    public static void addLevelBonusEffect(LevelBonusEffect newBonusEffect) {
        bonusEffect = newBonusEffect;

        if (newBonusEffect == LevelBonusEffect.DoubleDown) {
            UpgradeTracker.duplicateRandomUpgrade();
        }
    }

    public static LevelBonusEffect getActiveBonusEffect() {
        return bonusEffect;
    }

    public static void reset() {
        bonusEffect = none;
    }
}