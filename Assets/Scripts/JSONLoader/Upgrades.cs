using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class UpgradeList {
    public List<Upgrade> upgrades;
}

[System.Serializable]
public class Upgrade: MenuItem {

    public enum UpgradeType {
        feedback,
        arrowSpeed,
        avoidErrors,
        other
    }

    public enum UpgradeRarity {
        always,
        common,
        uncommon,
        rare,
        godly
    }

    public enum UpgradeEffect {
        Anarchy,
        Bandit,
        ControlTower,
        Fertilizer,
        Goalie,
        Houston,
        LoadedDice,
        RepeatOffender,
        SecurityCamera,
        Sharpshooter,
        TwoPerfect,
        None
    }

    public string name;
    public string sprite_name;
    public string upgrade_type_string;
    public UpgradeType upgrade_type;
    public string color;
    public string rarity_string;
    public UpgradeRarity rarity;
    public UpgradeEffect effect;
    public string description;
    public int numOfTimesTriggered;
    public bool isEnabled = true;

    public string Name => name;
    public string SpriteName => sprite_name;
    public string Color => color;
    public string RarityString => rarity_string;
    public string Description => description;


    public void InitializeFromJSON() {
        if (!Enum.TryParse(upgrade_type_string, true, out upgrade_type)) {
            Debug.LogWarning($"Invalid upgrade_type: {upgrade_type_string} for Upgrade {name}. Defaulting to 'feedback'.");
            upgrade_type = UpgradeType.feedback;
        }

        if (!Enum.TryParse(rarity_string, true, out rarity)) {
            Debug.LogWarning($"Invalid rarity: {rarity_string} for Upgrade {name}. Defaulting to 'common'.");
            rarity = UpgradeRarity.common;
        }

        if (!Enum.TryParse(name.Replace(" ", ""), true, out effect)) {
            Debug.LogWarning($"Invalid effect: {name} for Upgrade {name}. Defaulting to 'none'.");
            effect = UpgradeEffect.None;
        }
    }

    public int GetRarityWeight() {
        switch (rarity) {
            case UpgradeRarity.always: return 1000; //Exclusively for testing, guarantees upgrade will appear
            case UpgradeRarity.godly: return 1;
            case UpgradeRarity.rare: return 3;
            case UpgradeRarity.uncommon: return 5;
            case UpgradeRarity.common: return 10;
            default: return 1;
        }
    }
}