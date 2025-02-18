using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelBonusManager: MenuItemManager {

    List<LevelBonus> levelBonuses;
    private JSONLoader jsonLoader;
    private GameObject levelBonusPrefab;

    public LevelBonusManager(JSONLoader jsonLoader, GameObject levelBonusPrefab, bool isTutorial = false) {
        this.jsonLoader = jsonLoader;
        this.levelBonusPrefab = levelBonusPrefab;
        this.levelBonuses = jsonLoader.loadLevelBonuses(isTutorial);
    }

    public MenuItem getLevelBonusMenuItemForLevel(int level) {
        if (level < levelBonuses.Count) {
            return levelBonuses[level];
        }

        return null;
    }

    public MenuGameObjects createLevelBonusOption(MenuItem levelBonusMenuItem, GameObject levelBonusPrefab) {
        GameObject background = levelBonusPrefab;
        GameObject image = background.transform.Find("MenuOptionImage").gameObject;
        GameObject text = background.transform.Find("MenuOptionText").gameObject;
        return new(levelBonusMenuItem, background, image, text);
    }
}