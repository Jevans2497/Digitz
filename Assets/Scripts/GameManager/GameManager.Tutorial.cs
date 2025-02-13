using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public partial class GameManager : MonoBehaviour {

    private bool isTutorial;
    [SerializeField] TextMeshProUGUI tutorialText;
    private TutorialTextManager tutorialTextManager;

    private bool playerHasSeenInitialMessage;
    private bool playerHasCompletedInitialSong;

    private void setupForTutorial() {
        isTutorial = true;
        tutorialTextManager = tutorialText.GetComponent<TutorialTextManager>();

        songs = jsonLoader.loadSongsTutorial();
        levels = jsonLoader.loadLevelsTutorial();
        currentSong = "MaryHadALittleLamb";

        levelNameDisplay.gameObject.SetActive(false);

        tutorialTextManager.pressSpaceButton = pressSpaceButton;
        tutorialTextManager.showMessage(TutorialTextManager.TutorialMessage.introduction);
        playerHasSeenInitialMessage = true;
    }

    private void updateForTutorial() {
        if (Input.GetKeyDown(KeyCode.Space) && pressSpaceButton.activeInHierarchy) {
            if (playerHasSeenInitialMessage) {
                startSongLoop();
            }
        }
    }
}
