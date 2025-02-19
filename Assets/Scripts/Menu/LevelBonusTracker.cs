using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelBonus;
using static LevelBonus.LevelBonusEffect;

public static class LevelBonusTracker {
    private static LevelBonus levelBonus;
    private static LevelBonusDisplayManager levelBonusDisplayManager;

    public static void setLevelBonusDisplayManager(LevelBonusDisplayManager ldm) {
        levelBonusDisplayManager = ldm;
    }

    public static void addLevelBonus(LevelBonus newLevelBonus) {
        levelBonus = newLevelBonus;
        if (newLevelBonus.levelBonusEffect == LevelBonusEffect.DoubleDown) {
            UpgradeTracker.duplicateRandomUpgrade();
        }

        levelBonusDisplayManager.levelBonusAdded(newLevelBonus);
    }

    public static LevelBonusEffect getActiveBonusEffect() {
        if (levelBonus == null) {
            return none;
        }
        return levelBonus.levelBonusEffect;
    }

    public static void reset() {
        levelBonus = null;
        levelBonusDisplayManager.levelBonusRemoved();
    }
}