using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager: MonoBehaviour {
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
            gameWon();
        }
    }

    private void playerBeatSong() {
        showFireworks();
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 1.0f, 1.0f));
        StartCoroutine(fadeOutAudio(1.0f));
        songCompleteDisplay.text = "Song Complete!";
        songCompleteDisplay.color = new Color32(0, 236, 117, 255);
        songCompleteDisplay.enabled = true;
        pressSpaceButton.SetActive(true);
    }

    private IEnumerator gameOver() {
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 0.5f, 1.0f));
        yield return new WaitForSeconds(1.0f);
        songCompleteDisplay.text = "Game Over";
        songCompleteDisplay.color = Color.red;
        songCompleteDisplay.enabled = true;
        pressSpaceButton.SetActive(true);
    }

    private void gameWon() {
        // The player beat the whole game!
    }
}
