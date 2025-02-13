using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public partial class GameManager : MonoBehaviour {

    private bool isTutorial;
    [SerializeField] TextMeshProUGUI tutorialText;
    private TutorialTextManager tutorialTextManager;

    private void setupForTutorial() {
        isTutorial = true;
        tutorialTextManager = tutorialText.GetComponent<TutorialTextManager>();
        songs = jsonLoader.loadSongsTutorial();
        levels = jsonLoader.loadLevelsTutorial();
        currentSong = "MaryHadALittleLamb";
        tutorialTextManager.pressSpaceButton = pressSpaceButton;
        tutorialTextManager.showMessage(TutorialTextManager.TutorialMessage.introduction);
    }
}
