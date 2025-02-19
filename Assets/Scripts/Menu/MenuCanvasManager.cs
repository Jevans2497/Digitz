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

    [SerializeField] GameObject levelBonusGameObject;
    private bool isLevelBonusOptionAvailable;

    List<MenuGameObjects> menuGameObjects = new List<MenuGameObjects>();

    UpgradeManager upgradeManager;
    LevelBonusManager levelBonusManager;
    ChallengeManager challengeManager;
    SongManager songManager;

    public bool isMenuLoopFinished = true;

    private bool isInitialMenuLoop = true;

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

    private void checkForInput() {
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
        }
    }

    private void activateMenuItem(MenuItem menuItem) {
        if (menuItem is Upgrade upgrade) {
            addUpgrade(upgrade);
        } else if (menuItem is Challenge challenge) {
            activateChallengeMenuItem(challenge);
        } else if (menuItem is Song song) {
            addSong(song);
        } else {
            Debug.LogWarning("Unknown MenuItem type");
        }
    }

    private void activateChallengeMenuItem(Challenge challenge) {
        if (challenge.isConcealed) {
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
                addLevelBonus();
            }
            addChallenge(challenge);
        }
    }

    private void addLevelBonus() {
        MenuItem levelBonusMenuItem = levelBonusManager.getLevelBonusMenuItemForLevel(gameManager.getLevelNumber());
        if (levelBonusMenuItem is LevelBonus levelBonus) {
            LevelBonusTracker.addLevelBonusEffect(levelBonus.levelBonusEffect);
        }
    }

    private void addUpgrade(Upgrade upgrade) {
        UpgradeTracker.addUpgrade(upgrade);
        presentChallengeOptions();
    }

    private void addChallenge(Challenge challenge) {
        if (LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.easyStreet) {
            challenge.severity = challenge.reduceSeverityByOneGrade();
            challenge.color = challenge.hexForSeverity(challenge.severity);
        }
        ChallengeTracker.addChallenge(challenge);
        levelBonusGameObject.SetActive(false);
        presentSongOptions();
    }

    private void addSong(Song song) {
        gameManager.setSong(song.song_file_name);
        menuCanvas.enabled = false;
        isMenuLoopFinished = true;
        gameManager.setPressSpaceButtonActive(true);
    }

    private void presentUpgradeOptions() {
        destroyPreexistingMenuObjects();
        menuGameObjects = upgradeManager.createUpgradeOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 3) {
            setupMenuOptions();
        }
    }

    private void presentChallengeOptions() {
        destroyPreexistingMenuObjects();
        setupLevelBonusOption();
        menuGameObjects = challengeManager.createChallengeOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 3) {
            setupMenuOptions();
        }
    }

    private void setupLevelBonusOption() {
        levelBonusGameObject.SetActive(true);
        isLevelBonusOptionAvailable = true;
        MenuItem menuItem = levelBonusManager.getLevelBonusMenuItemForLevel(gameManager.getLevelNumber());
        MenuGameObjects levelBonusGameObjects = levelBonusManager.createLevelBonusOption(menuItem, levelBonusGameObject);
        Debug.Log(menuItem.SpriteName);
        customizeMenuOption(levelBonusGameObjects, menuItem.Name, menuItem.Color, levelBonusGameObjects.path, menuItem.Description);
    }

    private void presentSongOptions() {
        destroyPreexistingMenuObjects();
        menuGameObjects = songManager.createSongOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 3) {
            setupMenuOptions();
        }
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
            tooltip.message = tooltipMessage + getSeverityString((Challenge)gameObjects.menuItem);
        }

    }

    private void setupMenuItemForChallenge(MenuItem menuItem) {
        Challenge challenge = (Challenge)menuItem;
        if (!challenge.hasSeverityBeenSet) {
            challenge.severity = challengeManager.getSeverityForChallenge(gameManager.getLevelNumber(), challenge.hasSeverity);
            challenge.color = challenge.hexForSeverity(challenge.severity);
            challenge.hasSeverityBeenSet = true;
        }
    }

    private string getSeverityString(Challenge challenge) {
        string severityColor = challenge.hexForSeverity(challenge.severity);
        return $"\n\nSeverity: <b><size=120%><color={severityColor}>{challenge.severityAsString(challenge.severity)}</color></size></b>";
    }
}