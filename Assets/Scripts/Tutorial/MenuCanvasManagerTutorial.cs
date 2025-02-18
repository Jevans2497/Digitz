using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;

public class MenuCanvasManagerTutorial: MonoBehaviour {

    [SerializeField] GameManager gameManager;
    [SerializeField] Canvas menuCanvas;
    [SerializeField] JSONLoader jsonLoader;
    [SerializeField] GameObject menuObjectPrefab;

    List<MenuGameObjects> menuGameObjects = new List<MenuGameObjects>();

    UpgradeManager upgradeManager;
    ChallengeManager challengeManager;
    SongManager songManager;

    public bool isMenuLoopFinished = true;

    public enum TutorialMenuStep {
        upgrade, challenge, song
    }

    public void startMenuLoop(TutorialMenuStep step) {
        switch (step) {
            case TutorialMenuStep.upgrade:
            presentUpgradeOptions();
            break;
            case TutorialMenuStep.challenge:
            presentChallengeOptions();
            break;
            case TutorialMenuStep.song:
            presentSongOptions();
            break;
        }
        isMenuLoopFinished = false;
        menuCanvas.enabled = true;
    }

    void Start() {
        menuCanvas.enabled = false;

        upgradeManager = new(jsonLoader, menuObjectPrefab, true);
        challengeManager = new(jsonLoader, menuObjectPrefab, true);
        songManager = new(jsonLoader, menuObjectPrefab, true);
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
                GameObject background = gameObjects.background;

                if (background != null) {
                    Vector3 originalPosition = background.transform.position;
                    background.transform.position = new Vector3(
                        originalPosition.x,
                        originalPosition.y + yOffset,
                        originalPosition.z
                    );
                }
            }
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
            addChallenge(challenge);
        } else if (menuItem is Song song) {
            addSong(song);
        } else {
            Debug.LogWarning("Unknown MenuItem type");
        }
    }

    private void addUpgrade(Upgrade upgrade) {
        UpgradeTracker.addUpgrade(upgrade);
        menuCanvas.enabled = false;
        isMenuLoopFinished = true;
        gameManager.executeNextTutorialStep();
    }

    private void addChallenge(Challenge challenge) {
        ChallengeTracker.addChallenge(challenge);
        menuCanvas.enabled = false;
        isMenuLoopFinished = true;
        gameManager.executeNextTutorialStep();
    }

    private void addSong(Song song) {
        gameManager.setSong(song.song_file_name);
        menuCanvas.enabled = false;
        isMenuLoopFinished = true;
    }

    private void presentUpgradeOptions() {
        destroyPreexistingMenuObjects();
        menuGameObjects = upgradeManager.createUpgradeOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 3) {
            setMenuOptions();
        }
    }

    private void presentChallengeOptions() {
        destroyPreexistingMenuObjects();
        menuGameObjects = challengeManager.createChallengeOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 3) {
            setMenuOptions();
        }
    }

    private void presentSongOptions() {
        destroyPreexistingMenuObjects();
        menuGameObjects = songManager.createSongOptions(menuCanvas.transform, menuObjectPrefab);
        if (menuGameObjects.Count >= 1) {
            setMenuOptions();
        }
    }

    private void destroyPreexistingMenuObjects() {
        foreach (var menuGameObject in menuGameObjects) {
            menuGameObject.destroy();
        }
    }

    private void setMenuOptions() {
        int currentMenuItemIndex = 0;
        foreach (var gameObjects in menuGameObjects) {
            MenuItem menuItem = gameObjects.menuItem;

            if (menuItem is Challenge) {
                setupMenuItemForChallenge(menuItem);
            }

            //Text and background color
            gameObjects.text.GetComponent<TextMeshProUGUI>().text = menuItem.Name;
            gameObjects.background.GetComponent<Image>().color = SharedResources.hexToColor(menuItem.Color);

            //Sprite
            Sprite sprite = Resources.Load<Sprite>(gameObjects.path);
            gameObjects.image.GetComponent<Image>().sprite = sprite;

            //Tooltip
            Tooltip tooltip = gameObjects.background.GetComponent<Tooltip>();
            tooltip.message = menuItem.Description;
            if (menuItem is Challenge) {
                tooltip.message = menuItem.Description + getSeverityString((Challenge)menuItem);
            }

            currentMenuItemIndex += 1;
        }
    }

    private void setupMenuItemForChallenge(MenuItem menuItem) {
        Challenge challenge = (Challenge)menuItem;
        challenge.severity = challengeManager.getSeverityForChallenge(gameManager.getLevelNumber(), challenge.hasSeverity);
        challenge.color = challenge.hexForSeverity(challenge.severity);
    }

    private string getSeverityString(Challenge challenge) {
        string severityColor = challenge.hexForSeverity(challenge.severity);
        return $"\n\nSeverity: <b><size=120%><color={severityColor}>{challenge.severityAsString(challenge.severity)}</color></size></b>";
    }
}