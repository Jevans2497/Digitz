using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelsList {
    public List<Level> levels;
}

[System.Serializable]
public class Level {

    public enum LevelBonusEffect {
        none,
        arrowSpeedLowered,
        tenPercentPoints,
        challengeSeverityLowered,
        rainbowArrowBoost,
        randomUpgradeDuplicated,
        goldenArrowBoost,
        favoriteFeedbackBoost
    }

    public string name;
    public int level_number;
    public string level_bonus_string;
    public LevelBonusEffect levelBonusEffect;
    public float completion_percent;
    public List<LevelSprite> level_sprites;

    public void InitializeFromJSON() {
        if (!Enum.TryParse(level_bonus_string, true, out levelBonusEffect)) {
            Debug.LogWarning($"Invalid level_bonus_effect: {level_bonus_string} for LevelBonus {name}. Defaulting to 'arrowSpeedLowered'.");
            levelBonusEffect = LevelBonusEffect.arrowSpeedLowered;
        }
    }
}

[System.Serializable]
public class LevelSprite {
    public string sprite_name;
    public string altername_name;
}