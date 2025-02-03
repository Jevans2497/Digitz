using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDisplayManager: MonoBehaviour {

    [SerializeField] GameObject upgradeDisplayPrefab;
    [SerializeField] Transform upgradeDisplayTransform;

    private List<KeyValuePair<Upgrade, GameObject>> upgradeDisplayObjects = new();
    private float spacing = 85;

    private void Start() {
        UpgradeTracker.setUpgradeDisplayManager(this);
    }

    public void upgradeAdded(Upgrade upgrade) {
        GameObject upgradeDisplayObject = Instantiate(upgradeDisplayPrefab, upgradeDisplayTransform);
        setUpgradeDisplay(upgrade, upgradeDisplayObject);
        upgradeDisplayObjects.Add(new(upgrade, upgradeDisplayObject));
    }

    private void setUpgradeDisplay(Upgrade upgrade, GameObject upgradeDisplayObject) {
        // Set Xpos
        float newXPosition = upgradeDisplayObjects.Count * spacing;
        upgradeDisplayObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(newXPosition, 0);

        GameObject foreground = upgradeDisplayObject.transform.Find("UpgradeDisplayForeground").gameObject;
        foreground.GetComponent<Image>().color = SharedResources.hexToColor(upgrade.color);

        // Set image
        GameObject image = foreground.transform.Find("UpgradeDisplayImage").gameObject;
        Sprite sprite = Resources.Load<Sprite>($"MenuItems/Upgrades/UpgradeIcons/{upgrade.sprite_name}");
        image.GetComponent<Image>().sprite = sprite;

        // Set Tooltip
        Tooltip tooltip = upgradeDisplayObject.GetComponent<Tooltip>();
        tooltip.message = upgrade.name + "\n" + upgrade.description;
    }

    public void upgradeRemoved(Upgrade upgrade) {
        KeyValuePair<Upgrade, GameObject> upgradeToRemove = upgradeDisplayObjects
            .Find(pair => pair.Key == upgrade);

        if (upgradeToRemove.Equals(default(KeyValuePair<Upgrade.UpgradeEffect, GameObject>))) {
            Debug.LogWarning("Upgrade not found!");
            return;
        }

        upgradeDisplayObjects.Remove(upgradeToRemove);
        Destroy(upgradeToRemove.Value);
    }

    public void upgradeDisabled(Upgrade upgrade) {
        KeyValuePair<Upgrade, GameObject> upgradeToDisable = upgradeDisplayObjects
    .Find(pair => pair.Key == upgrade);

        if (upgradeToDisable.Equals(default(KeyValuePair<Upgrade.UpgradeEffect, GameObject>))) {
            Debug.LogWarning("Unable to disable upgrade, not found!");
            return;
        }

        setUpgradeDisplayColor(upgradeToDisable.Value, SharedResources.hexToColor("#DEDEDE"));
    }

    public void upgradeEnabled(Upgrade upgrade) {
        KeyValuePair<Upgrade, GameObject> upgradeToEnable = upgradeDisplayObjects
    .Find(pair => pair.Key == upgrade);

        if (upgradeToEnable.Equals(default(KeyValuePair<Upgrade.UpgradeEffect, GameObject>))) {
            Debug.LogWarning("Unable to disable upgrade, not found!");
            return;
        }

        setUpgradeDisplayColor(upgradeToEnable.Value, SharedResources.hexToColor(upgrade.color));
    }

    private void setUpgradeDisplayColor(GameObject upgradeDisplayObject, Color color) {
        GameObject foreground = upgradeDisplayObject.transform.Find("UpgradeDisplayForeground").gameObject;
        foreground.GetComponent<Image>().color = color;
    }
}
