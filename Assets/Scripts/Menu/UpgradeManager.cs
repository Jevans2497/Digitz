using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MenuItemManager {

    List<Upgrade> upgrades;
    private JSONLoader jsonLoader;
    private GameObject menuObjectPrefab;

    public UpgradeManager(JSONLoader jsonLoader, GameObject menuObjectPrefab, bool isTutorial = false) {
        this.jsonLoader = jsonLoader;
        this.menuObjectPrefab = menuObjectPrefab;
        this.upgrades = jsonLoader.loadUpgrades(isTutorial);
    }

    public List<MenuGameObjects> createUpgradeOptions(Transform menuCanvasTransform, GameObject menuObjectPrefab) {
        return createMenuOptions<Upgrade>(menuCanvasTransform, menuObjectPrefab, upgrades);
    }
}
