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
    private float defaultMissScore = -25.0f;

    private float banditThreshold = -1f;
    private float banditDefaultScore = 5.0f;

    GameManager gameManager;

    List<Upgrade> upgrades = UpgradeTracker.GetUpgrades();
    Challenge challenge = ChallengeTracker.getChallenge();

    private static FeedbackType mostRecentFeedback;
    private static int feedbackStreak = 0;

    public static int loadedDiceCounter = 25;

    private static HashSet<FeedbackType> anarchyUpgradeFeedbackTracker = new HashSet<FeedbackType>();

    public static Dictionary<FeedbackType, int> feedbackCounter = new Dictionary<FeedbackType, int>();

    public FeedbackData(float baseThreshold, GameManager gameManager, bool isGoldenArrow) {
        this.gameManager = gameManager;

        float modifiedThreshold = modifyThresholdForUpgrades(baseThreshold);
        modifiedThreshold = modifyThresholdForChallenge(modifiedThreshold);

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
            calculatedScore = banditDefaultScore * gameManager.getLevelNumber();
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

        feedbackStreak = mostRecentFeedback == feedbackForScore ? feedbackStreak + 1 : 0;
        mostRecentFeedback = feedbackForScore;

        incrementFeedbackCounter(feedbackForScore);

        float modifiedScore = modifyScoreForUpgrades(calculatedScore, feedbackForScore);

        return modifiedScore;
    }

    private void incrementFeedbackCounter(FeedbackType feedbackForScore) {
        if (!feedbackCounter.Keys.Contains(feedbackForScore)) {
            feedbackCounter[feedbackForScore] = 0;
        }
        feedbackCounter[feedbackForScore] += 1;

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
                UpgradeTracker.upgradeTriggered(UpgradeEffect.Goalie);
            }

            //LoadedDice
            if (upgrade.effect == UpgradeEffect.LoadedDice) {
                if (modifiedThreshold > stinkyThreshold && loadedDiceCounter > 0) {
                    modifiedThreshold = perfectThreshold;
                    mostRecentFeedback = FeedbackType.perfect;
                    loadedDiceCounter -= 1;
                    UpgradeTracker.upgradeTriggered(upgrade);
                }
            }

            //Sharpshooter
            if (upgrade.effect == UpgradeEffect.Sharpshooter) {
                perfectThreshold = 0.125f;
                UpgradeTracker.upgradeTriggered(upgrade);
            }
        }

        return modifiedThreshold;
    }

    private float modifyScoreForUpgrades(float baseScore, FeedbackType feedbackForScore) {
        float modifiedScore = baseScore;

        foreach (var upgrade in upgrades) {

            //TwoPerfect
            if (upgrade.effect == UpgradeEffect.TwoPerfect) {
                if (feedbackForScore == FeedbackType.perfect) {
                    UpgradeTracker.upgradeTriggered(upgrade);
                    modifiedScore *= 2;
                }
            }

            //RepeatOffender
            if (upgrade.effect == UpgradeEffect.RepeatOffender) {
                UpgradeTracker.upgradeTriggered(upgrade);
                modifiedScore += 25.0f * feedbackStreak;
            }

            //Anarchy
            if (upgrade.effect == UpgradeEffect.Anarchy) {
                UpgradeTracker.upgradeTriggered(upgrade);
                if (feedbackForScore != FeedbackType.bandit) {
                    anarchyUpgradeFeedbackTracker.Add(feedbackForScore);
                }
                if (anarchyUpgradeFeedbackTracker.Count == System.Enum.GetValues(typeof(FeedbackType)).Length - 1) {
                    gameManager.multiplyScore(1.25f);
                    anarchyUpgradeFeedbackTracker.Clear();
                }
            }

            //Fertilizer
            if (upgrade.effect == UpgradeEffect.Fertilizer) {
                if (feedbackForScore != FeedbackType.miss) {
                    modifiedScore *= 1.25f;
                }
                UpgradeTracker.upgradeTriggered(upgrade);
            }
        }

        return modifiedScore;
    }

    private void modifyDefaultScoresForChallenge() {
        if(challenge != null && challenge.effect == Challenge.ChallengeEffect.HighPressure) {
            float multiplier = challenge.getSeverityMultiplier();
            defaultStinkyScore = -25 * multiplier;
            defaultMissScore = -125 * multiplier;
        }
    }

    private void modifyDefaultScoresForLevelBonus() {
        if (LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.aintBroke) {
            FeedbackType mostReceivedFeedback = FeedbackData.feedbackCounter.OrderByDescending(kvp => kvp.Value).First().Key;
            switch (mostReceivedFeedback) {
                case FeedbackType.bandit:
                banditDefaultScore *= 1.25f;
                break;
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
            gameManager.multiplyScore(5.0f);
            break;

            case FeedbackType.great:
            gameManager.multiplyScore(3.0f);
            break;

            case FeedbackType.good:
            gameManager.multiplyScore(2.0f);
            break;

            case FeedbackType.stinky:
            gameManager.multiplyScore(1.5f);
            break;
        }
    }
}
