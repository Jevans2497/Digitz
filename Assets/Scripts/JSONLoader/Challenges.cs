using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ChallengeList {
    public List<Challenge> challenges;
}

[System.Serializable]
public class Challenge: MenuItem {

    public enum ChallengeType {
        feedback,
        arrowSpeed,
        avoidErrors,
        other
    }

    public enum ChallengeRarity {
        always,
        common,
        uncommon,
        rare,
        godly
    }

    public enum ChallengeEffect {
        Bombardment,
        BrokenTape,
        EarlyBird,
        Graveyard,
        HighPressure,
        LaterGator,
        OppositeDay,
        Outage,
        Overclock,
        ShatteredSword,
        StartWithNegative,
        Supersonic,
        TheCurse,
        TheMiddlePath,
        None
    }

    public enum ChallengeSeverity {
        veryLow, low, medium, high, veryHigh, none
    }

    public string name;
    public string sprite_name;
    public string challenge_type_string;
    public ChallengeType challenge_type;
    public string color;
    public string rarity_string;
    public ChallengeRarity rarity;
    public ChallengeEffect effect;
    public bool hasSeverity;
    public ChallengeSeverity severity;
    public bool hasSeverityBeenSet;
    public string description;
    public bool isInitiallyHidden;

    public string Name => name;
    public string SpriteName => sprite_name;
    public string Color => color;
    public string RarityString => rarity_string;
    public string Description => description;

    public void InitializeFromJSON() {
        if (!Enum.TryParse(challenge_type_string, true, out challenge_type)) {
            Debug.LogWarning($"Invalid challenge_type: {challenge_type_string} for Challenge {name}. Defaulting to 'feedback'.");
            challenge_type = ChallengeType.feedback;
        }

        if (!Enum.TryParse(rarity_string, true, out rarity)) {
            Debug.LogWarning($"Invalid rarity: {rarity_string} for Upgrade {name}. Defaulting to 'common'.");
            rarity = ChallengeRarity.common;
        }

        if (!Enum.TryParse(name.Replace(" ", ""), true, out effect)) {
            Debug.LogWarning($"Invalid effect: {name} for Challenge {name}. Defaulting to 'none'.");
            effect = ChallengeEffect.None;
        }
    }

    public int GetRarityWeight() {
        switch (rarity) {
            case ChallengeRarity.always: return 1000; //Exclusively for testing, guarantees challenge will appear
            case ChallengeRarity.godly: return 1;
            case ChallengeRarity.rare: return 3;
            case ChallengeRarity.uncommon: return 5;
            case ChallengeRarity.common: return 10;
            default: return 1;
        }
    }

    public float getSeverityMultiplier() {
        switch (severity) {
            case ChallengeSeverity.veryLow: return 1;
            case ChallengeSeverity.low: return 2;
            case ChallengeSeverity.medium: return 3;
            case ChallengeSeverity.high: return 4;
            case ChallengeSeverity.veryHigh: return 5;
            default: return 1;
        }
    }

    public string hexForSeverity(Challenge.ChallengeSeverity severity) {
        switch (severity) {
            case Challenge.ChallengeSeverity.veryLow:
            return "#1D7A1D"; // Bright Green  
            case Challenge.ChallengeSeverity.low:
            return "#B78F00"; // Strong Yellow  
            case Challenge.ChallengeSeverity.medium:
            return "#3498DB"; // Vibrant Blue  
            case Challenge.ChallengeSeverity.high:
            return "#E67E22"; // Deep Orange  
            case Challenge.ChallengeSeverity.veryHigh:
            return "#E74C3C"; // Strong Red  
            default:
            return "#964B00"; // White for fallback
        }
    }


    public string severityAsString(Challenge.ChallengeSeverity severity) {
        switch (severity) {
            case Challenge.ChallengeSeverity.veryLow:
            return "Very Low";
            case Challenge.ChallengeSeverity.low:
            return "Low";
            case Challenge.ChallengeSeverity.medium:
            return "Medium";
            case Challenge.ChallengeSeverity.high:
            return "High";
            case Challenge.ChallengeSeverity.veryHigh:
            return "Very High";
            default:
            return "None";
        }
    }
}