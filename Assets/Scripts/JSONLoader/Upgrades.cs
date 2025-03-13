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
        Abacus,
        AfterTheRain,
        Anarchy,
        Bandit,
        ControlTower,
        Fertilizer,
        Goalie,
        GoldMine,
        GoodGraces,
        GreatDay,
        GreatResponsibility,
        Houston,
        LoadedDice,
        Marshmallow,
        MostWanted,
        PressureCooker,
        RepeatOffender,
        SecurityCamera,
        Sharpshooter,
        Sloth,
        TheGTrain,
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
    public bool isEnabled = true;
    public int loadedDiceCounter = 25;
    public Guid guid;

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

        if (!Enum.TryParse(name.Replace(" ", "").Replace("-", ""), true, out effect)) {
            Debug.LogWarning($"Invalid effect: {name} for Upgrade {name}. Defaulting to 'none'.");
            effect = UpgradeEffect.None;
        }

        color = getColorForRarity();
    }

    public Upgrade Clone() {
        // Creates a clone with a different guid so duplicate upgrades will not be viewed as equal
        return new Upgrade {
            name = this.name,
            sprite_name = this.sprite_name,
            upgrade_type_string = this.upgrade_type_string,
            upgrade_type = this.upgrade_type,
            color = this.color,
            rarity_string = this.rarity_string,
            rarity = this.rarity,
            effect = this.effect,
            description = this.description,
            isEnabled = this.isEnabled,
            guid = Guid.NewGuid() // Assign a new GUID for the copy
        };
    }

    public void incrementRarityLevel() {
        switch (rarity) {
            case UpgradeRarity.godly:
            break;
            case UpgradeRarity.rare:
            rarity = UpgradeRarity.godly;
            break;
            case UpgradeRarity.uncommon:
            rarity = UpgradeRarity.rare;
            break;
            case UpgradeRarity.common:
            rarity = UpgradeRarity.uncommon;
            break;
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

    public String getColorForRarity() {
        switch (rarity) {
            case UpgradeRarity.godly:
            return "rainbow"; 
            case UpgradeRarity.rare:
            return "#FF00FE";
            case UpgradeRarity.uncommon:
            return "#44BAAD";
            case UpgradeRarity.common:
            return "#007FFF";
            default:
            return "#DEDEDE";
        }
    }
}