using System;
using UnityEngine;

[System.Serializable]
public class ArrowData {

    public enum ArrowEffect {
        regular, simultaneous, golden, rainbow
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
        //If the arrow is default, check if it's a special effect. 
        if (arrowEffect == ArrowEffect.regular) {
            arrowEffect = convertEffectStringToEffect();
        }

        //Any arrow in the game has a small chance to be golden or rainbow
        float randomGoldenArrowSpawnFloat = UnityEngine.Random.Range(0, 5000);
        float randomRainbowArrowSpawnFloat = UnityEngine.Random.Range(0, 10000);

        if (randomGoldenArrowSpawnFloat == 0) {
            arrowEffect = ArrowEffect.golden;
        }

        if (randomRainbowArrowSpawnFloat == 0) {
            arrowEffect = ArrowEffect.rainbow;
        }

        switch (arrowEffect) {
            case ArrowEffect.regular:
                color = Color.white;
                layer = 0;
                break;
            case ArrowEffect.simultaneous:
                color = Color.magenta;
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
        }
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