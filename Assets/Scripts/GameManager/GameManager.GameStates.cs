using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager: MonoBehaviour {

    [SerializeField] AudioClip songCompleteClip;
    [SerializeField] AudioClip gameOverClip;

    private bool isSongComplete() {
        if (isInGenerateSongJSONMode) { return false; }
        bool didPlayerBeatSong = score >= scoreNeededToClearLevel && !isTutorial && !isInTestSongMode;
        bool didSongReachEnd = !audioSource.isPlaying && hasSongStarted == true;
        return (didPlayerBeatSong || didSongReachEnd) && inSongLoop;
    }

    private void songFinished() {
        inSongLoop = false;
        spawnedArrowManager.destroyCurrentExistingArrows();

        int currentLevelIndex = levels.FindIndex(level => level == currentLevel);
        if (currentLevelIndex + 1 < levels.Count) {
            currentLevel = levels[currentLevelIndex + 1];
            bool didPlayerBeatSong = score >= scoreNeededToClearLevel;
            bool didSongReachEnd = !audioSource.isPlaying && hasSongStarted == true;

            if (didSongReachEnd && !isTutorial) {
                StartCoroutine(gameOver());
            } else if (didPlayerBeatSong) {
                playerBeatSong();
            } else {
                playerBeatSong();
            }

            resetSongLoop();
        } else {
            if (isTutorial) {
                playerBeatSong();
            } else {
                gameWon();
            }
        }
    }

    private void playerBeatSong() {
        AudioManager.Instance.playSound(songCompleteClip);
        AudioManager.Instance.playMenuMusic(true);
        showFireworks();
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 1.0f, 1.0f));
        StartCoroutine(SharedResources.fadeOutAudio(1.0f, audioSource));
        songCompleteDisplay.text = "Song Complete!";
        songCompleteDisplay.color = new Color32(0, 236, 117, 255);
        songCompleteDisplay.enabled = true;
        pressSpaceButton.SetActive(true);
    }

    private IEnumerator gameOver() {
        AudioManager.Instance.playSound(gameOverClip);
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 0.5f, 1.0f));
        yield return new WaitForSeconds(1.0f);
        songCompleteDisplay.text = "Game Over";
        songCompleteDisplay.color = Color.red;
        songCompleteDisplay.enabled = true;
        pressSpaceButton.SetActive(true);
        isGameComplete = true;
    }

    private void gameWon() {
        AudioManager.Instance.playSound(songCompleteClip);
        AudioManager.Instance.playMenuMusic(true);
        isGameComplete = true;
        setupFireworksFinale();
        showFireworks();
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 1.0f, 1.0f));
        StartCoroutine(SharedResources.fadeOutAudio(1.0f, audioSource));
        songCompleteDisplay.text = "YOU WON!";
        songCompleteDisplay.color = new Color32(0, 236, 117, 255);
        songCompleteDisplay.enabled = true;
    }
}
