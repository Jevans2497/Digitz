using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UpgradeEffect = Upgrade.UpgradeEffect;

public enum FeedbackType {
    bandit, perfect, great, good, stinky, miss
}

public class FeedbackData {
    public float score;
    public string text;
    public Color color;
    public bool isGradient = false;
    public VertexGradient gradient;

    public static float perfectThreshold = 0.1f;
    public static float greatThreshold = 0.4f;
    public static float goodThreshold = 0.7f;
    public static float stinkyThreshold = 1.0f;

    private float defaultPerfectScore = 100.0f;
    private float defaultGreatScore = 75.0f;
    private float defaultGoodScore = 50.0f;
    private float defaultStinkyScore = 25.0f;
    private float defaultMissScore = -50.0f;

    private float banditThreshold = -1f;
    private float banditDefaultScore = 5.0f;

    GameManager gameManager;

    List<Upgrade> upgrades = UpgradeTracker.GetUpgrades();
    Challenge challenge = ChallengeTracker.getChallenge();

    private static FeedbackType mostRecentFeedback;
    private static int feedbackStreak = 0;

    private static HashSet<FeedbackType> anarchyUpgradeFeedbackTracker = new HashSet<FeedbackType>();

    public static Dictionary<FeedbackType, int> fullGameFeedbackCounter = new Dictionary<FeedbackType, int>();
    public static Dictionary<FeedbackType, int> currentSongFeedbackCounter = new Dictionary<FeedbackType, int>();

    public FeedbackData(float baseThreshold, GameManager gameManager, bool isGoldenArrow) {
        this.gameManager = gameManager;

        float modifiedThreshold = modifyThresholdForUpgrades(baseThreshold);
        modifiedThreshold = modifyThresholdForChallenge(modifiedThreshold);

        modifyDefaultScoresForUpgrade();
        modifyDefaultScoresForChallenge();
        modifyDefaultScoresForLevelBonus();
        score = calculateScore(modifiedThreshold);

        string scoreSign = score >= 0 ? " +" : " -";
        if (modifiedThreshold == banditThreshold) {
            text = "Bandit! " + scoreSign + score.ToString("N0");
            color = Color.white;
        } else if (modifiedThreshold <= perfectThreshold) {
            text = "PERFECT! " + scoreSign + score.ToString("N0");
            color = Color.white;
            isGradient = true;
            gradient = new VertexGradient(
                new Color(1f, 0f, 0f), // Top left (Red)
                new Color(0f, 1f, 0f), // Top right (Green)
                new Color(0f, 0f, 1f), // Bottom left (Blue)
                new Color(1f, 1f, 0f)  // Bottom right (Yellow)
            );
        } else if (modifiedThreshold <= greatThreshold) {
            text = "GREAT! " + scoreSign + score.ToString("N0");
            color = Color.green;
        } else if (modifiedThreshold <= goodThreshold) {
            text = "Good " + scoreSign + score.ToString("N0");
            color = Color.blue;
        } else if (modifiedThreshold <= stinkyThreshold) {
            text = "Stinky ): " + scoreSign + score.ToString("N0");
            color = Color.gray;
        } else {
            text = "Miss " + scoreSign + score.ToString("N0");
            color = Color.red;
        }

        if (isGoldenArrow) {
            handleGoldenArrow();
        }
    }

    private float calculateScore(float modifiedThreshold) {
        float calculatedScore;
        FeedbackType feedbackForScore;

        if (modifiedThreshold == banditThreshold) {
            feedbackForScore = FeedbackType.bandit;
            int numberOfBandits = UpgradeTracker.GetUpgrades().FindAll(upgrade => upgrade.effect == UpgradeEffect.Bandit).Count;
            calculatedScore = banditDefaultScore * gameManager.getLevelNumber() * numberOfBandits;
        } else if (modifiedThreshold <= perfectThreshold) {
            feedbackForScore = FeedbackType.perfect;
            calculatedScore = defaultPerfectScore;
        } else if (modifiedThreshold <= greatThreshold) {
            feedbackForScore = FeedbackType.great;
            calculatedScore = defaultGreatScore;
        } else if (modifiedThreshold <= goodThreshold) {
            feedbackForScore = FeedbackType.good;
            calculatedScore = defaultGoodScore;
        } else if (modifiedThreshold <= stinkyThreshold) {
            feedbackForScore = FeedbackType.stinky;
            calculatedScore = defaultStinkyScore;
        } else {
            feedbackForScore = FeedbackType.miss;
            calculatedScore = defaultMissScore;
        }

        if (feedbackForScore != FeedbackType.bandit) {
            feedbackStreak = mostRecentFeedback == feedbackForScore ? feedbackStreak + 1 : 0;
            mostRecentFeedback = feedbackForScore;
        }

        incrementFeedbackCounters(feedbackForScore);

        float modifiedScore = modifyScoreForUpgrades(calculatedScore, feedbackForScore);

        return modifiedScore;
    }

    private void incrementFeedbackCounters(FeedbackType feedbackForScore) {
        if (!fullGameFeedbackCounter.Keys.Contains(feedbackForScore)) {
            fullGameFeedbackCounter[feedbackForScore] = 0;
        }
        if (!currentSongFeedbackCounter.Keys.Contains(feedbackForScore)) {
            currentSongFeedbackCounter[feedbackForScore] = 0;
        }
        fullGameFeedbackCounter[feedbackForScore] += 1;
        currentSongFeedbackCounter[feedbackForScore] += 1;

    }

    private float modifyThresholdForUpgrades(float baseThreshold) {
        float modifiedThreshold = baseThreshold;

        foreach (var upgrade in upgrades) {

            //Goalie
            if (upgrade.effect == UpgradeEffect.Goalie && !UpgradeTracker.hasUpgrade(UpgradeEffect.LoadedDice)) {
                if (modifiedThreshold > stinkyThreshold && modifiedThreshold != banditThreshold) {
                    modifiedThreshold = stinkyThreshold;
                    mostRecentFeedback = FeedbackType.miss;                    
                }
            }

            //Sharpshooter
            if (upgrade.effect == UpgradeEffect.Sharpshooter) {
                perfectThreshold = 0.125f;
            }

            if (upgrade.effect == UpgradeEffect.GreatResponsibility) {
                greatThreshold = stinkyThreshold;
            }
        }

        //LoadedDice
        Upgrade firstLoadedDice = UpgradeTracker.GetUpgrades().FirstOrDefault(upgrade => upgrade.effect == UpgradeEffect.LoadedDice);
        if (firstLoadedDice != null) {            
            if (modifiedThreshold > stinkyThreshold) {
                modifiedThreshold = perfectThreshold;
                mostRecentFeedback = FeedbackType.perfect;
                firstLoadedDice.loadedDiceCounter -= 1;

                if (firstLoadedDice.loadedDiceCounter <= 0) {
                    UpgradeTracker.removeFirstUpgradeWithEffect(UpgradeEffect.LoadedDice);
                }
            }
        }

        return modifiedThreshold;
    }

    private float modifyScoreForUpgrades(float baseScore, FeedbackType feedbackForScore) {
        float modifiedScore = baseScore;

        foreach (var upgrade in upgrades) {
         
            //RepeatOffender
            if (upgrade.effect == UpgradeEffect.RepeatOffender) {
                modifiedScore += 10.0f * feedbackStreak;
            }

            //Anarchy
            if (upgrade.effect == UpgradeEffect.Anarchy) {
                anarchyUpgradeFeedbackTracker.Add(feedbackForScore);
                if (anarchyUpgradeFeedbackTracker.Count == getCurrentPotentialFeedbackTypes().Count) {
                    gameManager.multiplyScore(1.05f);
                    anarchyUpgradeFeedbackTracker.Clear();
                }
            }                       

            //Pressure Cooker
            if (upgrade.effect == UpgradeEffect.PressureCooker) {
                Song currentSong = gameManager.getCurrentSong();
                int difficultyLevel = (int)currentSong.difficulty + 1;
                float multiplier = 1f + (0.1f * difficultyLevel);
                modifiedScore *= multiplier;
            }

            //Abacus
            if (upgrade.effect == UpgradeEffect.Abacus && feedbackForScore != FeedbackType.bandit) {
                modifiedScore += currentSongFeedbackCounter[feedbackForScore];
            }
        }

        return modifiedScore;
    }

    private List<FeedbackType> getCurrentPotentialFeedbackTypes() {
        List<FeedbackType> feedbackTypes = System.Enum.GetValues(typeof(FeedbackType))
            .Cast<FeedbackType>()
            .ToList();

        if (!UpgradeTracker.hasUpgrade(UpgradeEffect.Bandit)) {
            feedbackTypes.Remove(FeedbackType.bandit);
        }

        if (UpgradeTracker.hasUpgrade(UpgradeEffect.Goalie) || UpgradeTracker.hasUpgrade(UpgradeEffect.LoadedDice)) {
            feedbackTypes.Remove(FeedbackType.miss);
        }

        if (UpgradeTracker.hasUpgrade(UpgradeEffect.GreatResponsibility)) {
            feedbackTypes.Remove(FeedbackType.good);
            feedbackTypes.Remove(FeedbackType.stinky);
        }

        if (ChallengeTracker.hasChallenge(Challenge.ChallengeEffect.TheMiddlePath)) {
            feedbackTypes.Remove(FeedbackType.perfect);
            feedbackTypes.Remove(FeedbackType.miss);
        }

        return feedbackTypes;
    }

    private void modifyDefaultScoresForUpgrade() {
        foreach (var upgrade in upgrades) {
            if (upgrade.effect == UpgradeEffect.TwoPerfect) {
                defaultPerfectScore += 100;
            }

            if (upgrade.effect == UpgradeEffect.TheGTrain) {
                defaultPerfectScore = 0.0f;
                defaultGreatScore += 225.0f;
                defaultGoodScore += 150.0f;
            }

            if (upgrade.effect == UpgradeEffect.GreatDay) {
                defaultGreatScore += 225.0f;
            }

            if (upgrade.effect == UpgradeEffect.GoodGraces) {
                defaultGoodScore += 350.0f;
            }

            if (upgrade.effect == UpgradeEffect.Fertilizer) {                
                defaultStinkyScore += 25 * .25f;
                defaultGoodScore += 50 * .25f;
                defaultGreatScore += 75 * .25f;
                defaultPerfectScore += 100 * .25f;
            }
        }
    }


    private void modifyDefaultScoresForChallenge() {
        if(challenge != null && challenge.effect == Challenge.ChallengeEffect.HighPressure) {
            float multiplier = challenge.getSeverityMultiplier();
            defaultStinkyScore = -25 * multiplier;
            defaultMissScore = -100 * multiplier;
        }
    }

    private void modifyDefaultScoresForLevelBonus() {
        if (LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.aintBroke) {
            FeedbackType mostReceivedFeedback = FeedbackData.fullGameFeedbackCounter.OrderByDescending(kvp => kvp.Value).First().Key;
            switch (mostReceivedFeedback) {
                case FeedbackType.perfect:
                defaultPerfectScore *= 1.25f;
                break;
                case FeedbackType.great:
                defaultGreatScore *= 1.25f;
                break;
                case FeedbackType.good:
                defaultGoodScore *= 1.25f;
                break;
                case FeedbackType.stinky:
                defaultStinkyScore *= 1.25f;
                break;
                default:
                defaultPerfectScore *= 1.25f;
                break;
            }
        }
    }

    private float modifyThresholdForChallenge(float baseThreshold) {

        //Opposite Day
        if (challenge != null && challenge.effect == Challenge.ChallengeEffect.OppositeDay) {
            if (baseThreshold <= perfectThreshold) {
                return stinkyThreshold;
            } else if (baseThreshold <= greatThreshold) {
                return goodThreshold;
            } else if (baseThreshold <= goodThreshold) {
                return greatThreshold;
            } else if (baseThreshold <= stinkyThreshold) {
                return perfectThreshold;
            } else {
                return baseThreshold;
            }
        }

        //The Middle Path
        if (challenge != null && challenge.effect == Challenge.ChallengeEffect.TheMiddlePath) {
            if (baseThreshold <= perfectThreshold) {
                return greatThreshold;
            } else if (baseThreshold > stinkyThreshold) {
                return stinkyThreshold;
            } else {
                return baseThreshold;
            }
        }

        return baseThreshold;
    }

    private void handleGoldenArrow() {
        switch (mostRecentFeedback) {
            case FeedbackType.perfect:
            gameManager.multiplyScore(1.15f);
            break;

            case FeedbackType.great:
            gameManager.multiplyScore(1.1f);
            break;

            case FeedbackType.good:
            gameManager.multiplyScore(1.05f);
            break;

            case FeedbackType.stinky:
            gameManager.multiplyScore(1.025f);
            break;
        }
    }
}
