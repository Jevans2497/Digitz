using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MenuItemManager {

    List<Upgrade> upgrades;
    private JSONLoader jsonLoader;
    private GameObject menuObjectPrefab;

    public UpgradeManager(JSONLoader jsonLoader, GameObject menuObjectPrefab) {
        this.jsonLoader = jsonLoader;
        this.menuObjectPrefab = menuObjectPrefab;
        this.upgrades = jsonLoader.loadUpgrades();
    }

    public List<MenuGameObjects> createUpgradeOptions(Transform menuCanvasTransform, GameObject menuObjectPrefab) {
        return createMenuOptions<Upgrade>(menuCanvasTransform, menuObjectPrefab, upgrades);
    }
}

public static class UpgradeTracker {
    private static List<Upgrade> currentActiveUpgrades = new List<Upgrade>();

    public static void addUpgrade(Upgrade upgrade) {
        Debug.Log("Adding upgrade: " + upgrade.name);
        currentActiveUpgrades.Add(upgrade);

        if (upgrade.effect == Upgrade.UpgradeEffect.LoadedDice) {
            FeedbackData.loadedDiceCounter = 25;
        }
    }

    public static List<Upgrade> GetUpgrades() {
        return currentActiveUpgrades.FindAll(upgrade => upgrade.isEnabled);
    }

    public static bool hasUpgrade(Upgrade.UpgradeEffect effect) {
        return currentActiveUpgrades.Any(upgrade => upgrade.effect == effect && upgrade.isEnabled);
    }

    public static void enableAllUpgrades() {
        foreach (var upgrade in currentActiveUpgrades) {
            upgrade.isEnabled = true;
        }
    }

    public static void removeLastAcquiredUpgrade() {
        if (currentActiveUpgrades.Count > 0) {
            Debug.Log("Removing upgrade " + currentActiveUpgrades[currentActiveUpgrades.Count - 1].name);
            currentActiveUpgrades.RemoveAt(currentActiveUpgrades.Count - 1);
        }
    }

    public static void upgradeTriggered(Upgrade upgrade) {
        if (currentActiveUpgrades.Contains(upgrade)) {
            upgrade.numOfTimesTriggered += 1;
        }
    }

    public static void upgradeTriggered(Upgrade.UpgradeEffect effect) {
        Upgrade upgrade = currentActiveUpgrades.FirstOrDefault(upgrade => upgrade.effect == effect);
        if (upgrade != null) {
            upgrade.numOfTimesTriggered += 1;
        }
    }
}
