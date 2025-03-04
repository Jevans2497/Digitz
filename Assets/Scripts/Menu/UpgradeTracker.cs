using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UpgradeTracker {
    private static List<Upgrade> allUpgrades = new List<Upgrade>();
    private static List<Upgrade> currentActiveUpgrades = new List<Upgrade>();
    private static UpgradeDisplayManager upgradeDisplayManager;

    public static void setAllUpgrades(List<Upgrade> upgrades) {
        allUpgrades = upgrades;
    }

    public static void setUpgradeDisplayManager(UpgradeDisplayManager udm) {
        upgradeDisplayManager = udm;
    }

    public static void addUpgrade(Upgrade upgrade) {
        if (upgrade.effect == Upgrade.UpgradeEffect.MostWanted) {
            duplicateAllUpgrades();
        }        

        // We want to clone upgrade so it uses a unique guid in case of duplicates. Otherwise, duplicate upgrades will reference same object
        Upgrade clonedUniqueUpgrade = upgrade.Clone();
        currentActiveUpgrades.Add(clonedUniqueUpgrade);
        upgradeDisplayManager.upgradeAdded(clonedUniqueUpgrade);
    }

    private static void duplicateAllUpgrades() {
        List<Upgrade> duplicates = currentActiveUpgrades
            .Select(existingUpgrade => existingUpgrade.Clone())
            .ToList();

        currentActiveUpgrades.AddRange(duplicates);

        foreach (var duplicatedUpgrade in duplicates) {
            upgradeDisplayManager.upgradeAdded(duplicatedUpgrade);
        }
    }

    public static void addRandomUpgrade(Upgrade.UpgradeEffect excludedUpgradeEffect) {                
        Upgrade randomUpgrade = allUpgrades[Random.Range(0, allUpgrades.Count)];
        while (randomUpgrade.effect == excludedUpgradeEffect) {
            randomUpgrade = allUpgrades[Random.Range(0, allUpgrades.Count)];
        }
        addUpgrade(randomUpgrade);
    }

    public static void duplicateRandomUpgrade() {
        if (currentActiveUpgrades.Count > 0) {
            Upgrade randomDuplicate = currentActiveUpgrades[UnityEngine.Random.Range(0, currentActiveUpgrades.Count)];
            addUpgrade(randomDuplicate);
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

    public static void removeFirstUpgradeWithEffect(Upgrade.UpgradeEffect effect) {
        var upgradeToRemove = currentActiveUpgrades.FirstOrDefault(upgrade => upgrade.effect == effect);
        if (upgradeToRemove != null) {
            currentActiveUpgrades.Remove(upgradeToRemove);
            upgradeDisplayManager.upgradeRemoved(upgradeToRemove);
        }
    }

    public static void removeLastAcquiredUpgrade() {
        if (currentActiveUpgrades.Count > 0) {
            Upgrade upgradeToRemove = currentActiveUpgrades[^1];
            currentActiveUpgrades.Remove(upgradeToRemove);
            upgradeDisplayManager.upgradeRemoved(upgradeToRemove);
        }
    }

    public static void reset() {
        currentActiveUpgrades = new List<Upgrade>();
    }
}