using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDisplayManager: MonoBehaviour {

    [SerializeField] GameObject upgradeDisplayPrefab;
    [SerializeField] Transform upgradeDisplayTransform;
    [SerializeField] ParticleSystem destroyLoadedDiceParticleSystem;
    [SerializeField] ParticleSystem destroyUpgradeParticleSystem;

    private List<KeyValuePair<Upgrade, GameObject>> upgradeDisplayObjects = new();
    private float spacing = 85;

    private bool isLevelBonusActive;

    private void Start() {
        UpgradeTracker.setUpgradeDisplayManager(this);
    }

    public void upgradeAdded(Upgrade upgrade) {
        GameObject upgradeDisplayObject = Instantiate(upgradeDisplayPrefab, upgradeDisplayTransform);
        setUpgradeDisplay(upgrade, upgradeDisplayObject);
        upgradeDisplayObjects.Add(new(upgrade, upgradeDisplayObject));
        layoutUpgradeDisplayObjects();        
    }

    private void setUpgradeDisplay(Upgrade upgrade, GameObject upgradeDisplayObject) {
        // Set Xpos
        float newXPosition = (upgradeDisplayObjects.Count * spacing);
        upgradeDisplayObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(newXPosition, 0);

        GameObject foreground = upgradeDisplayObject.transform.Find("UpgradeDisplayForeground").gameObject;
        string upgradeColor = upgrade.color == "rainbow" ? "00ffd4" : upgrade.color;
        foreground.GetComponent<Image>().color = SharedResources.hexToColor(upgradeColor);

        // Set image
        GameObject image = foreground.transform.Find("UpgradeDisplayImage").gameObject;
        Sprite sprite = Resources.Load<Sprite>($"MenuItems/Upgrades/UpgradeIcons/{upgrade.sprite_name}");
        image.GetComponent<Image>().sprite = sprite;

        // Set Tooltip
        Tooltip tooltip = upgradeDisplayObject.GetComponent<Tooltip>();
        tooltip.message = upgrade.name + ":\n\n" + upgrade.description;
    }

    public void layoutUpgradeDisplayObjects() {
        float levelBonusSpacer = LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.none ? 0.0f : 105.0f;
        for (int i = 0; i < upgradeDisplayObjects.Count; i++) {
            var upgradeDisplayObject = upgradeDisplayObjects[i];
            float newXPosition = (i * spacing) + levelBonusSpacer;
            upgradeDisplayObject.Value.GetComponent<RectTransform>().anchoredPosition = new Vector2(newXPosition, 0);
        }
    }

    public void upgradeRemoved(Upgrade upgrade) {
        KeyValuePair<Upgrade, GameObject> upgradeToRemove = upgradeDisplayObjects
            .FirstOrDefault(pair => pair.Key == upgrade);
        
        if (upgradeToRemove.Equals(default(KeyValuePair<Upgrade.UpgradeEffect, GameObject>))) {
            Debug.LogWarning("Upgrade not found!");
            return;
        }

        ParticleSystem particleSystem = upgradeToRemove.Key.effect == Upgrade.UpgradeEffect.LoadedDice ? destroyLoadedDiceParticleSystem : destroyUpgradeParticleSystem;
        StartCoroutine(removeUpgradeWithEffect(upgradeToRemove, particleSystem));
    }

    private IEnumerator removeUpgradeWithEffect(KeyValuePair<Upgrade, GameObject> upgradeToRemove, ParticleSystem particleSystem) {
        RectTransform upgradeRectTransform = upgradeToRemove.Value.GetComponent<RectTransform>();

        // Convert UI position to world space
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(null, upgradeRectTransform.position);

        // Convert screen position to world position in the world canvas
        Vector3 worldPosition;
        bool success = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            upgradeDisplayTransform as RectTransform, screenPosition, Camera.main, out worldPosition);

        if (success) {
            particleSystem.transform.position = worldPosition;
        } else {
            Debug.LogWarning("Failed to convert UI position to world position.");
        }

        particleSystem.Play();

        yield return new WaitForSeconds(0.1f);

        upgradeDisplayObjects.Remove(upgradeToRemove);
        Destroy(upgradeToRemove.Value);
        layoutUpgradeDisplayObjects();
    }

    public void upgradeDisabled(Upgrade upgrade) {
        KeyValuePair<Upgrade, GameObject> upgradeToDisable = upgradeDisplayObjects
    .Find(pair => pair.Key == upgrade);

        if (upgradeToDisable.Equals(default(KeyValuePair<Upgrade.UpgradeEffect, GameObject>))) {
            Debug.LogWarning("Unable to disable upgrade, not found!");
            return;
        }

        setUpgradeDisplayColor(upgradeToDisable.Value, "#DEDEDE");
    }

    public void upgradeEnabled(Upgrade upgrade) {
        KeyValuePair<Upgrade, GameObject> upgradeToEnable = upgradeDisplayObjects
    .Find(pair => pair.Key == upgrade);

        if (upgradeToEnable.Equals(default(KeyValuePair<Upgrade.UpgradeEffect, GameObject>))) {
            Debug.LogWarning("Unable to disable upgrade, not found!");
            return;
        }

        string upgradeColor = upgrade.color == "rainbow" ? "00ffd4" : upgrade.color;
        setUpgradeDisplayColor(upgradeToEnable.Value, upgradeColor);
    }

    private void setUpgradeDisplayColor(GameObject upgradeDisplayObject, string color) {
        GameObject foreground = upgradeDisplayObject.transform.Find("UpgradeDisplayForeground").gameObject;
        foreground.GetComponent<Image>().color = SharedResources.hexToColor(color);
    }
}
