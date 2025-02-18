using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelBonus;
using static LevelBonus.LevelBonusEffect;

public static class LevelBonusTracker {
    private static LevelBonusEffect bonusEffect = none;

    public static void addLevelBonusEffect(LevelBonusEffect newBonusEffect) {
        bonusEffect = newBonusEffect;
        Debug.Log("adding effect" + newBonusEffect.ToString());
    }

    public static LevelBonusEffect getActiveBonusEffect() {
        return bonusEffect;
    }

    public static void reset() {
        bonusEffect = none;
    }
}