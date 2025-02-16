using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager: MonoBehaviour {

    private bool isTutorial;
    [SerializeField] TextMeshProUGUI tutorialText;
    private TutorialTextManager tutorialTextManager;
    [SerializeField] MenuCanvasManagerTutorial menuCanvasManagerTutorial;

    [SerializeField] GameObject allArrows;
    [SerializeField] GameObject feedbackExamples;

    private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    private void setupForTutorial() {
        isTutorial = true;
        tutorialTextManager = tutorialText.GetComponent<TutorialTextManager>();

        songs = jsonLoader.loadSongs(true);
        levels = jsonLoader.loadLevels(true);
        currentSong = "HappyUkulele";

        tutorialTextManager.pressSpaceButton = pressSpaceButton;
        tutorialTextManager.showMessage(TutorialTextManager.TutorialMessage.introduction);

        setupTutorialSteps();
    }

    private void setupTutorialSteps() {
        tutorialSteps = new List<TutorialStep> {
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.royaltyFree1)),
            new TutorialStep(() => playSong("HappyUkulele")),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.gameBasics1)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.gameBasics2)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.upgrades1)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.upgrades2)),
            new TutorialStep(() => showMenuOption(MenuCanvasManagerTutorial.TutorialMenuStep.upgrade)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.challenges1)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.challenges2)),
            new TutorialStep(() => showMenuOption(MenuCanvasManagerTutorial.TutorialMenuStep.challenge)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.royaltyFree2)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.simultaneousArrows)),
            new TutorialStep(() => playSong("BigBandExplosion")),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.gameGoal1)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.gameGoal2)),
            new TutorialStep(() => showTutorialMessage(TutorialTextManager.TutorialMessage.gameGoal3)),
        };
    }

    private void updateForTutorial() {
        if (Input.GetKeyDown(KeyCode.Space) && pressSpaceButton.activeInHierarchy) {
            executeNextTutorialStep();
        }
    }

    public void executeNextTutorialStep() {
        resetForTutorialText();
        TutorialStep currentStep = tutorialSteps.FirstOrDefault(step => !step.isCompleted);
        if (currentStep != null) {
            currentStep.execute();
            currentStep.isCompleted = true;
        } else {
            tutorialCompleted();
        }
    }

    private void playSong(string songName) {
        setSong(songName);
        startSongLoop();
        tutorialText.gameObject.SetActive(false);
    }

    private void showTutorialMessage(TutorialTextManager.TutorialMessage message) {
        tutorialText.gameObject.SetActive(true);
        tutorialTextManager.showMessage(message);

        if (message == TutorialTextManager.TutorialMessage.gameBasics2) {
            allArrows.SetActive(false);
            feedbackExamples.SetActive(true);
        }

        if (message == TutorialTextManager.TutorialMessage.gameGoal2) {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0.0f;
            scoreDisplay.gameObject.SetActive(true);
            scoreDisplay.text = $"<size=200%>0</size> / 10,000";
        }
    }

    private void showMenuOption(MenuCanvasManagerTutorial.TutorialMenuStep menuTutorialStep) {
        resetForTutorialText();
        menuCanvasManagerTutorial.startMenuLoop(menuTutorialStep);
        tutorialText.gameObject.SetActive(false);
    }

    private void tutorialCompleted() {
        UpgradeTracker.reset();
        ChallengeTracker.reset();
        SceneManager.LoadSceneAsync("DDR");
    }

    private void resetForTutorialText() {
        songCompleteDisplay.enabled = false;
        pressSpaceButton.SetActive(false);
        allArrows.SetActive(true);
        feedbackExamples.SetActive(false);
        stopFireworks();
    }
}

class TutorialStep {
    public bool isCompleted;
    private Action action;

    public TutorialStep(Action action) {
        this.action = action;
    }

    public void execute() {
        action?.Invoke();
    }
}
