using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDisplayManager : MonoBehaviour {

    [SerializeField] GameObject upgradeDisplayPrefab;
    [SerializeField] Transform upgradeDisplayTransform;

    //private Dictionary<Upgrade.UpgradeEffect, GameObject> upgradeDisplayObjects = new();
    private List<KeyValuePair<Upgrade.UpgradeEffect, GameObject>> upgradeDisplayObjects = new();
    private float spacing = 85;

    private void Start() {
        UpgradeTracker.setUpgradeDisplayManager(this);
    }

    public void upgradeAdded(Upgrade upgrade) {
        GameObject upgradeDisplayObject = Instantiate(upgradeDisplayPrefab, upgradeDisplayTransform);
        setUpgradeDisplay(upgrade, upgradeDisplayObject);
        upgradeDisplayObjects.Add(new(upgrade.effect, upgradeDisplayObject));
        //upgradeDisplayObjects.Add(upgrade.effect, upgradeDisplayObject);
    }

    private void setUpgradeDisplay(Upgrade upgrade, GameObject upgradeDisplayObject) {
        // Set Xpos
        float newXPosition = upgradeDisplayObjects.Count * spacing;
        upgradeDisplayObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(newXPosition, 0);

        // Set color
        GameObject foreground = upgradeDisplayObject.transform.Find("UpgradeDisplayForeground").gameObject;
        foreground.GetComponent<Image>().color = SharedResources.hexToColor(upgrade.color);

        // Set image
        GameObject image = foreground.transform.Find("UpgradeDisplayImage").gameObject;
        Sprite sprite = Resources.Load<Sprite>($"MenuItems/Upgrades/UpgradeIcons/{upgrade.sprite_name}");
        image.GetComponent<Image>().sprite = sprite;
    }

    public void upgradeRemoved(Upgrade upgrade) {
        KeyValuePair<Upgrade.UpgradeEffect, GameObject> upgradeToRemove = upgradeDisplayObjects
            .Find(pair => pair.Key == upgrade.effect);

        if (upgradeToRemove.Equals(default(KeyValuePair<Upgrade.UpgradeEffect, GameObject>))) {
            Debug.LogWarning("Upgrade not found!");
            return;
        }

        upgradeDisplayObjects.Remove(upgradeToRemove);
        Destroy(upgradeToRemove.Value);
    }

}
