using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelBonusesList {
    public List<LevelBonus> levelBonuses;
}

[System.Serializable]
public class LevelBonus: MenuItem {

    public enum LevelBonusEffect {
        none,
        slowArrows,
        tenPercentCoupon,
        easyStreet,
        rainbowRoad,
        DoubleDown,
        yellowBrick,
        aintBroke
    }

    public int level_number;
    public string level_bonus_name;
    public string level_bonus_string;
    public LevelBonusEffect levelBonusEffect;
    public string level_bonus_sprite;
    public string level_bonus_description;

    public string Name => level_bonus_name;
    public string SpriteName => level_bonus_sprite;
    public string Color => "#0084FF";
    public string RarityString => throw new NotImplementedException();
    public string Description => level_bonus_description;

    public void InitializeFromJSON() {
        if (!Enum.TryParse(level_bonus_string, true, out levelBonusEffect)) {
            Debug.LogWarning($"Invalid level_bonus_effect: {level_bonus_string} for LevelBonus {level_bonus_name}. Defaulting to 'none'.");
            levelBonusEffect = LevelBonusEffect.none;
        }
    }

    public int GetRarityWeight() {
        throw new NotImplementedException();
    }
}