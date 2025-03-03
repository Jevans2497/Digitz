using System;
using UnityEngine;

[System.Serializable]
public class ArrowData {

    public enum ArrowEffect {
        regular, simultaneous, golden, rainbow, freeze, lightning, fire
    }

    public float timestamp;
    public string arrow_direction;
    public float arrow_speed;
    public string arrow_effect;
    public ArrowEffect arrowEffect;
    public Color color;
    public int layer;

    public ArrowData(float timestamp, string arrow_direction, ArrowEffect arrowEffect) {
        this.timestamp = timestamp;
        this.arrow_direction = arrow_direction;
        this.arrow_speed = 0f;
        this.arrowEffect = arrowEffect;
        applyEffectToArrow();
    }

    public void applyEffectToArrow() {
        //If the arrow is default, check if the JSON has already given it a special effect. 
        if (arrowEffect == ArrowEffect.regular) {
            arrowEffect = convertEffectStringToEffect();
        }

        arrowEffect = rollForSpecialArrowEffect(arrowEffect);

        switch (arrowEffect) {
            case ArrowEffect.regular:
            color = Color.white;
            layer = 0;
            break;
            case ArrowEffect.simultaneous:
            color = SharedResources.hexToColor("#FFD700");
            layer = 6;
            break;
            case ArrowEffect.golden:            
        color = Color.yellow;
            layer = 7;
            break;
            case ArrowEffect.rainbow:
            color = Color.red;
            layer = 8;
            break;
            case ArrowEffect.freeze:
            color = SharedResources.hexToColor("#7DEDFF");
            layer = 9;
            break;
            case ArrowEffect.lightning:
            color = SharedResources.hexToColor("#fcdda7");
            layer = 10;
            break;
            case ArrowEffect.fire:
            color = SharedResources.hexToColor("#ffb0b0");
            layer = 11;
            break;
        }
    }

    private ArrowEffect rollForSpecialArrowEffect(ArrowEffect originalEffect) {
        //Any arrow in the game has a small chance to be golden or rainbow
        int rainbowArrowSpawnRate = 10000;
        int goldenArrowSpawnRate = 5000;

        if (LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.rainbowRoad) {
            rainbowArrowSpawnRate /= 4;
        }

        if (LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.yellowBrick) {
            goldenArrowSpawnRate /= 2;
        }

        UpgradeTracker.GetUpgrades().ForEach(upgrade => {
            if (upgrade.effect == Upgrade.UpgradeEffect.GoldMine) {
                goldenArrowSpawnRate /= 2;
            }
            if (upgrade.effect == Upgrade.UpgradeEffect.AfterTheRain) {
                rainbowArrowSpawnRate /= 2;
            }
        });

        int randomRainbowArrowRoll = UnityEngine.Random.Range(0, rainbowArrowSpawnRate);
        int randomGoldenArrowRoll = UnityEngine.Random.Range(0, goldenArrowSpawnRate);

        if (randomRainbowArrowRoll == 0) {
            return ArrowEffect.rainbow;
        }

        if (randomGoldenArrowRoll == 0) {
            return ArrowEffect.golden;
        }

        return originalEffect;
    }

    private ArrowEffect convertEffectStringToEffect() {
        switch (arrow_effect) {
            case "simultaneous":
            return ArrowEffect.simultaneous;
            case "golden":
            return ArrowEffect.golden;
            case "rainbow":
            return ArrowEffect.rainbow;
            default:
            return ArrowEffect.regular;
        }
    }
}