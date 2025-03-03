using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;

public class MenuCanvasManager: MonoBehaviour {

    [SerializeField] GameManager gameManager;
    [SerializeField] Canvas menuCanvas;
    [SerializeField] JSONLoader jsonLoader;
    [SerializeField] GameObject menuObjectPrefab;
    [SerializeField] TextMeshProUGUI menuOptionExplanationText;
    [SerializeField] GameObject concealedChallengeExplanation;
    [SerializeField] GameObject theCurseExplanation;

    public AudioClip upgradeSelectedClip;
    public AudioClip challengeSelectedClip;
    public AudioClip songSelectedClip;
    public AudioClip levelBonusSelectedClip;
    public AudioClip concealedChallengeRevealedClip;

    [SerializeField] GameObject levelBonusGameObject;
    private bool isLevelBonusOptionAvailable;

    List<MenuGameObjects> menuGameObjects = new List<MenuGameObjects>();

    UpgradeManager upgradeManager;
    LevelBonusManager levelBonusManager;
    ChallengeManager challengeManager;
    SongManager songManager;

    public bool isMenuLoopFinished = true;

    private bool isInitialMenuLoop = true;
    private bool isUserInteractionEnabled = true;
    private bool isSelectingUpgrade;

    public void startMenuLoop() {
        LevelBonusTracker.reset();
        isLevelBonusOptionAvailable = false;        
        presentUpgradeOptions();
        isMenuLoopFinished = false;
        menuCanvas.enabled = true;

        if (isInitialMenuLoop) {
            isInitialMenuLoop = false;
            presentChallengeOptions();
        } else {
            presentUpgradeOptions();
        }

        menuOptionExplanationText.gameObject.SetActive(true);
    }

    void Start() {
        menuCanvas.enabled = false;

        upgradeManager = new(jsonLoader, menuObjectPrefab);
        levelBonusManager = new(jsonLoader, levelBonusGameObject);
        challengeManager = new(jsonLoader, menuObjectPrefab);
        songManager = new(jsonLoader, menuObjectPrefab);
    }

    private void Update() {
        if (menuCanvas.enabled) {
            checkForInput();
            animateMenuItems();
        }
    }

    private void checkForInput() {
        if (!isUserInteractionEnabled) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (menuGameObjects[0] != null) {
                activateMenuItem(menuGameObjects[0].menuItem);
            }
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (menuGameObjects[1] != null) {
                activateMenuItem(menuGameObjects[1].menuItem);
            }
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (menuGameObjects[2] != null) {
                activateMenuItem(menuGameObjects[2].menuItem);
            }
        } else if (Input.GetKeyDown(KeyCode.DownArrow) && isSelectingUpgrade) {
            passOnUpgrades();
        }        
    }

    private void activateMenuItem(MenuItem menuItem) {
        if (menuItem is Upgrade upgrade) {
            activateUpgradeMenuOption(menuItem);
        } else if (menuItem is Challenge challenge) {
            activateChallengeMenuItem(challenge);
        } else if (menuItem is Song song) {
            addSong(song);
        } else if (menuItem is LevelBonus levelBonus) {
            activateChallengeMenuItem((Challenge)menuGameObjects[0].menuItem);
        } else {
            Debug.LogWarning("Unknown MenuItem type");
        }
    }

    private void activateChallengeMenuItem(Challenge challenge) {
        concealedChallengeExplanation.SetActive(false);
        if (challenge.isConcealed) {
            AudioManager.Instance.playSound(concealedChallengeRevealedClip);
            menuGameObjects.ForEach(menuGameObjects => {
                if (menuGameObjects.menuItem is Challenge challenge) {
                    challenge.isConcealed = false;
                    levelBonusGameObject.SetActive(false);
                    isLevelBonusOptionAvailable = false;
                }
            });
            setupMenuOptions();
        } else {
            if (isLevelBonusOptionAvailable) {
                AudioManager.Instance.playSound(levelBonusSelectedClip);
                addLevelBonus();
            }
            addChallenge(challenge);
        }
    }

    private void addLevelBonus() {
        MenuItem levelBonusMenuItem = levelBonusManager.getLevelBonusMenuItemForLevel(gameManager.getLevelNumber());
        if (levelBonusMenuItem is LevelBonus levelBonus) {
            LevelBonusTracker.addLevelBonus(levelBonus);
        }
    }

    private void activateUpgradeMenuOption(MenuItem menuItem) {
        int activeMenuObjects = menuGameObjects.Count(mgo => mgo.background.activeSelf);
        if (UpgradeTracker.hasUpgrade(Upgrade.UpgradeEffect.Marshmallow) && activeMenuObjects > 1) {
            addUpgrade((Upgrade)menuItem, false);
            var menuGameObject = menuGameObjects.Find(mgo => mgo.menuItem == menuItem);
            if (menuGameObject != null) {
                menuGameObject.background.SetActive(false);
            }
        } else if (UpgradeTracker.hasUpgrade(Upgrade.UpgradeEffect.Marshmallow) && activeMenuObjects == 1) {
            UpgradeTracker.removeMarshmallowUpgrade();
            addUpgrade((Upgrade)menuItem);
        } else { 
            addUpgrade((Upgrade)menuItem);
        }
    }

    private void passOnUpgrades() {
        //Remove marshmallow upgrade if it exists
        UpgradeTracker.removeMarshmallowUpgrade();
        presentChallengeOptions();
    }

    private void addUpgrade(Upgrade upgrade, bool isCompleteAfterUpgradeAdded = true) {
        AudioManager.Instance.playSound(upgradeSelectedClip);
        UpgradeTracker.addUpgrade(upgrade);
        if (isCompleteAfterUpgradeAdded) {
            isSelectingUpgrade = false;
            presentChallengeOptions();
        }
    }

    private void addChallenge(Challenge challenge) {
        if (LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.easyStreet) {
            challenge.severity = challenge.reduceSeverityByOneGrade();
            challenge.color = challenge.hexForSeverity(challenge.severity);
        }
        AudioManager.Instance.playSound(challengeSelectedClip);
        ChallengeTracker.addChallenge(challenge);
        levelBonusGameObject.SetActive(false);
        theCurseExplanation.SetActive(false);
        challengeManager.removeChallengeFromPool(challenge);
        presentSongOptions();
    }

    private void addSong(Song song) {
        AudioManager.Instance.playSound(songSelectedClip);
        menuCanvas.enabled = false;
        songManager.removeSongFromPool(song);        
        gameManager.setSong(song.song_file_name);
        isMenuLoopFinished = true;
        gameManager.setPressSpaceButtonActive(true);
    }

    private void presentUpgradeOptions() {
        destroyPreexistingMenuObjects();
        isSelectingUpgrade = true;
        string menuExplanation = UpgradeTracker.hasUpgrade(Upgrade.UpgradeEffect.Marshmallow) ? "Upgrade - Marshmallow Upgrade Active! Select upgrade(s) or press the down arrow at any time to continue." : "Upgrade";
        menuOptionExplanationText.text = menuExplanation;        
        menuGameObjects = upgradeManager.createUpgradeOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 3) {
            setupMenuOptions();
        }
        menuGameObjects.ForEach(menuGameObject => StartCoroutine(animateMenuOptionGrowing(menuGameObject.background)));
    }

    private void presentChallengeOptions() {
        destroyPreexistingMenuObjects();
        menuOptionExplanationText.text = "Challenge";
        concealedChallengeExplanation.SetActive(true);
        setupLevelBonusOption();
        menuGameObjects = challengeManager.createChallengeOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 3) {
            setupMenuOptions();
        }
        menuGameObjects.ForEach(menuGameObject => StartCoroutine(animateMenuOptionGrowing(menuGameObject.background)));
        showTheCurseExplanationIfTheCurseActive();
    }

    private void showTheCurseExplanationIfTheCurseActive() {
        Challenge theCurse = ChallengeTracker.getTheCurseIfActive();
        if (theCurse != null) {
            theCurseExplanation.GetComponent<Image>().color = SharedResources.hexToColor(theCurse.color);
            Tooltip tooltip = theCurseExplanation.GetComponent<Tooltip>();
            tooltip.message = "The Curse\n" + getSeverityString(theCurse, false);            
            theCurseExplanation.SetActive(true);
        }
    }

    private void setupLevelBonusOption() {
        levelBonusGameObject.SetActive(true);
        isLevelBonusOptionAvailable = true;
        MenuItem menuItem = levelBonusManager.getLevelBonusMenuItemForLevel(gameManager.getLevelNumber());
        MenuGameObjects levelBonusGameObjects = levelBonusManager.createLevelBonusOption(menuItem, levelBonusGameObject);
        customizeMenuOption(levelBonusGameObjects, menuItem.Name, menuItem.Color, levelBonusGameObjects.path, menuItem.Description);
        StartCoroutine(animateMenuOptionGrowing(levelBonusGameObjects.background));
    }

    private void presentSongOptions() {
        destroyPreexistingMenuObjects();
        menuOptionExplanationText.text = "Song";
        menuGameObjects = songManager.createSongOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 3) {
            setupMenuOptions();
        }
        menuGameObjects.ForEach(menuGameObject => StartCoroutine(animateMenuOptionGrowing(menuGameObject.background)));
    }

    private void destroyPreexistingMenuObjects() {
        foreach (var menuGameObject in menuGameObjects) {
            menuGameObject.destroy();
        }
    }

    private void setupMenuOptions() {
        foreach (var gameObjects in menuGameObjects) {
            MenuItem menuItem = gameObjects.menuItem;
            if (menuItem is Challenge challenge && challenge.isConcealed) {
                string hiddenItemSpritePath = "MenuItems/Challenges/ChallengeIcons/QuestionMark";
                customizeMenuOption(gameObjects, "???", "#DEDEDE", hiddenItemSpritePath, "");
            } else {
                if (menuItem is Challenge) {
                    setupMenuItemForChallenge(menuItem);
                }
                customizeMenuOption(gameObjects, menuItem.Name, menuItem.Color, gameObjects.path, menuItem.Description);
            }
        }
    }


    private void customizeMenuOption(MenuGameObjects gameObjects, string text, string color, string spritePath, string tooltipMessage) {
        //Text and color
        gameObjects.text.GetComponent<TextMeshProUGUI>().text = text;
        gameObjects.background.GetComponent<Image>().color = SharedResources.hexToColor(color);

        //Sprite
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        gameObjects.image.GetComponent<Image>().sprite = sprite;

        //Tooltip
        Tooltip tooltip = gameObjects.background.GetComponent<Tooltip>();
        tooltip.message = tooltipMessage;
        if (gameObjects.menuItem is Challenge challenge && !challenge.isConcealed) {
            string message = tooltipMessage + getSeverityString((Challenge)gameObjects.menuItem);
            tooltip.message = message;
            tooltip.manuallySetMessage(message);            
        }

        //Button
        Button button = gameObjects.background.GetComponent<Button>();
        button.onClick.AddListener(() => activateMenuItem(gameObjects.menuItem));

    }

    private void setupMenuItemForChallenge(MenuItem menuItem) {
        Challenge challenge = (Challenge)menuItem;
        if (UpgradeTracker.hasUpgrade(Upgrade.UpgradeEffect.MostWanted)) {
            challenge.severity = Challenge.ChallengeSeverity.veryHigh;
            challenge.color = challenge.hexForSeverity(challenge.severity);
        }

        //We need hasSeverityBeenSet so that the challenge doesn't change when concealed challenges are revealed.
        if (!challenge.hasSeverityBeenSet) {
            challenge.severity = challengeManager.getSeverityForChallenge(gameManager.getLevelNumber(), challenge.hasSeverity);
            challenge.color = challenge.hexForSeverity(challenge.severity);
            challenge.hasSeverityBeenSet = true;
        }
    }

    private string getSeverityString(Challenge challenge, bool withNewLines = true) {
        string severityColor = challenge.hexForSeverity(challenge.severity);
        string newLines = withNewLines ? "\n\n" : "";
        return $"{newLines}Severity: <b><size=120%><color={severityColor}>{challenge.severityAsString(challenge.severity)}</color></size></b>";
    }

    private float animationSpeed = 5f; // Speed of the up and down motion
    private float animationHeight = 0.035f; // Height of the motion
    private float animationTime = 0f; // Tracks time for the sine wave

    private void animateMenuItems() {
        if (menuGameObjects.Count > 0) {
            animationTime += Time.deltaTime * animationSpeed;
            float yOffset = Mathf.Sin(animationTime) * animationHeight;

            foreach (var gameObjects in menuGameObjects) {
                animateObjectMovingUpAndDown(gameObjects.background, yOffset);
            }

            if (levelBonusGameObject.activeInHierarchy) {
                animateObjectMovingUpAndDown(levelBonusGameObject.gameObject, yOffset);
            }
        }
    }

    // Animations
    private void animateObjectMovingUpAndDown(GameObject gameObject, float yOffset) {
        if (gameObject != null) {
            Vector3 originalPosition = gameObject.transform.position;
            gameObject.transform.position = new Vector3(
                originalPosition.x,
                originalPosition.y + yOffset,
                originalPosition.z
            );
        }
    }

    private IEnumerator animateMenuOptionGrowing(GameObject gameObject) {
        isUserInteractionEnabled = false;
        float startTime = 0.0f;
        float duration = 0.25f;
        Vector3 originalScale = gameObject.transform.localScale;

        gameObject.transform.localScale = new Vector3(0, 0, 0);

        while (startTime < duration) {
            startTime += Time.deltaTime;
            if (gameObject != null) {
                gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, startTime / duration);
            }
            yield return null;
        }

        if (gameObject != null) {
            gameObject.transform.localScale = originalScale;
        }

        isUserInteractionEnabled = true;
    }
}