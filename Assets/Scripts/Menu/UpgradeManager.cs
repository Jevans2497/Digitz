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
    private static UpgradeDisplayManager upgradeDisplayManager;

    public static void setUpgradeDisplayManager(UpgradeDisplayManager udm) {
        upgradeDisplayManager = udm;
    }

    public static void addUpgrade(Upgrade upgrade) {
        currentActiveUpgrades.Add(upgrade);
        upgradeDisplayManager.upgradeAdded(upgrade);

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

    public static void disableUpgrade(Upgrade upgrade) {
        upgrade.isEnabled = false;
        upgradeDisplayManager.upgradeDisabled(upgrade);
    }

    public static void enableAllUpgrades() {
        foreach (var upgrade in currentActiveUpgrades) {
            upgrade.isEnabled = true;
            upgradeDisplayManager.upgradeEnabled(upgrade);
        }
    }

    public static void removeLastAcquiredUpgrade() {
        if (currentActiveUpgrades.Count > 0) {
            Upgrade upgradeToRemove = currentActiveUpgrades[^1];
            currentActiveUpgrades.Remove(upgradeToRemove);
            upgradeDisplayManager.upgradeRemoved(upgradeToRemove);
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
