using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBonusDisplayManager: MonoBehaviour {

    [SerializeField] GameObject levelBonusDisplayPrefab;
    [SerializeField] Transform levelBonusDisplayTransform;

    private KeyValuePair<LevelBonus, GameObject> levelBonusDisplayObject = new();

    private void Start() {
        LevelBonusTracker.setLevelBonusDisplayManager(this);
    }

    public void levelBonusAdded(LevelBonus levelBonus) {
        GameObject levelBonusDisplayObject = Instantiate(levelBonusDisplayPrefab, levelBonusDisplayTransform);
        setLevelBonusDisplay(levelBonus, levelBonusDisplayObject);
        this.levelBonusDisplayObject = new KeyValuePair<LevelBonus, GameObject>(levelBonus, levelBonusDisplayObject);
    }

    private void setLevelBonusDisplay(LevelBonus levelBonus, GameObject levelBonusDisplayObject) {
        GameObject foreground = levelBonusDisplayObject.transform.Find("LevelBonusDisplayForeground").gameObject;
        foreground.GetComponent<Image>().color = SharedResources.hexToColor(levelBonus.Color);

        // Set image
        GameObject image = foreground.transform.Find("LevelBonusDisplayImage").gameObject;
        Sprite sprite = Resources.Load<Sprite>($"MenuItems/LevelBonuses/LevelBonusIcons/{levelBonus.SpriteName}");
        image.GetComponent<Image>().sprite = sprite;

        // Set Tooltip
        Tooltip tooltip = levelBonusDisplayObject.GetComponent<Tooltip>();
        tooltip.message = levelBonus.Name + ":\n\n" + levelBonus.Description;
    }

    public void levelBonusRemoved(LevelBonus levelBonus) {
        Destroy(levelBonusDisplayObject.Value);
    }
}
