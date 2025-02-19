using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelBonus;
using static LevelBonus.LevelBonusEffect;

public static class LevelBonusTracker {
    private static LevelBonusEffect bonusEffect = none;
    private static LevelBonusDisplayManager levelBonusDisplayManager;

    public static void setLevelBonusDisplayManager(LevelBonusDisplayManager ldm) {
        levelBonusDisplayManager = ldm;
    }

    public static void addLevelBonusEffect(LevelBonusEffect newBonusEffect) {
        bonusEffect = newBonusEffect;

        if (newBonusEffect == LevelBonusEffect.DoubleDown) {
            UpgradeTracker.duplicateRandomUpgrade();
        }

        levelBonusDisplayManager.levelBonusAdded(bonusEffect);
    }

    public static LevelBonusEffect getActiveBonusEffect() {
        return bonusEffect;
    }

    public static void reset() {
        bonusEffect = none;
    }
}