using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class GameManager: MonoBehaviour {

    [SerializeField] TextMeshProUGUI displayTimer;
    [SerializeField] TextMeshProUGUI levelNameDisplay;
    [SerializeField] TextMeshProUGUI scoreDisplay;
    [SerializeField] AudioSource audioSource;
    [SerializeField] MenuCanvasManager menuCanvasManager;
    [SerializeField] SpriteRenderer blackBackgroundOverlay;
    [SerializeField] Slider progressBar;

    [SerializeField] Arrow leftArrow;
    [SerializeField] Arrow upArrow;
    [SerializeField] Arrow rightArrow;
    [SerializeField] Arrow downArrow;
    List<Arrow> arrowsList;

    [SerializeField] TextMeshProUGUI multiplierDisplay;
    [SerializeField] TextMeshProUGUI songCompleteDisplay;
    [SerializeField] GameObject pressSpaceButton;
    [SerializeField] ParticleSystem particleSystem;

    public bool isInGenerateSongJSONMode = false;
    public float skipToTime = 0.0f; // Used while making arrow jsons to skip to a certain part. 
    private bool hasArrowsStarted;
    private bool hasSongStarted;

    private float songTime;
    private float delayStartSeconds = 3.0f;
    private float score;

    private string currentSong;
    private List<Song> songs;

    private int level;
    private List<Level> levels;

    private float scoreNeededToClearLevel;

    [SerializeField] JSONLoader jsonLoader;
    [SerializeField] SpawnedArrowManager spawnedArrowManager;

    private bool inSongLoop = false;

    private void Start() {
        songs = jsonLoader.loadSongs();
        levels = jsonLoader.loadLevels();
        currentSong = "FreakingOutTheNeighborhood";
        arrowsList = new List<Arrow> { leftArrow, upArrow, rightArrow, downArrow };
        if (skipToTime > 0.0f) {
            songTime = skipToTime;
        }
        setupSong();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (songCompleteDisplay.enabled) {
                songCompleteDisplay.enabled = false;
                pressSpaceButton.SetActive(false);
                menuCanvasManager.startMenuLoop();
            }
            if (menuCanvasManager.isMenuLoopFinished) {
                startSongLoop();
            }
        }

        if (inSongLoop) {
            manageSongLoop();
        }

        // Cheat codes for debugging
        if (Input.GetKeyDown(KeyCode.P)) {
            score += 5000f;
        }
        if (Input.GetKeyDown(KeyCode.N)) {
            songFinished();
        }
    }

    private void startSongLoop() {
        pressSpaceButton.SetActive(false);
        setupSpawnedArrowManager();
        inSongLoop = true;
        level += 1;
        setupLevel();
        levelNameDisplay.enabled = true;
        handleUpgradesAndChallenges();
        StartCoroutine(showAndFadeLevelName(levelNameDisplay, delayStartSeconds));
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 1.0f, 1.5f, 0.9f));
    }

    private void manageSongLoop() {
        songTime += Time.deltaTime * audioSource.pitch;
        displayTimer.text = songTime.ToString("F2");
        scoreDisplay.text = $"<size=200%>{score.ToString("N0")}</size> / {scoreNeededToClearLevel.ToString("N0")}";

        if (!hasArrowsStarted) {
            startSpawningArrows();
        }
        if (!hasSongStarted) {
            audioSource.PlayScheduled(AudioSettings.dspTime + delayStartSeconds);
            hasSongStarted = true;
            if (skipToTime > 0.0f) {
                audioSource.time = songTime;
            }
        }

        if (isSongComplete()) {
            songFinished();
        }
    }

    private void setupSong() {
        SongPreset song = jsonLoader.LoadSong(currentSong);

        string songPath = "SongMp3s/" + currentSong;
        AudioClip songClip = Resources.Load<AudioClip>(songPath);

        if (audioSource != null && song != null && songClip != null) {
            audioSource.clip = songClip;
        }
    }

    private void setupSpawnedArrowManager() {
        SongPreset song = jsonLoader.LoadSong(currentSong);
        if (!isInGenerateSongJSONMode) {
            spawnedArrowManager.setup(song);
        }
    }

    private void startSpawningArrows() {
        hasArrowsStarted = true;
        if (!isInGenerateSongJSONMode) {
            spawnedArrowManager.setShouldSpawnArrows(true);
        }
    }

    public float getSongTime() {
        return songTime;
    }

    public float getScore() {
        return score;
    }

    public void addToScore(float points) {
        score += points;
        progressBar.value = score;
    }

    public void multiplyScore(float multiplier) {
        score *= multiplier;
        progressBar.value = score;
        multiplierDisplay.text = "x" + multiplier;
        StartCoroutine(animateMultiplierDisplay());
    }

    public bool isInSongLoop() {
        return inSongLoop;
    }

    public int getLevel() {
        return level;
    }

    public void setSong(Song song) {
        currentSong = song.song_file_name;
        setupSong();
    }

    private bool isSongComplete() {
        bool didPlayerBeatSong = score >= scoreNeededToClearLevel;
        bool didSongReachEnd = !audioSource.isPlaying && hasSongStarted == true;
        return (didPlayerBeatSong || didSongReachEnd) && inSongLoop;
    }

    private void songFinished() {
        inSongLoop = false;
        spawnedArrowManager.destroyCurrentExistingArrows();

        bool didPlayerBeatSong = score >= scoreNeededToClearLevel;
        bool didSongReachEnd = !audioSource.isPlaying && hasSongStarted == true;

        if (didSongReachEnd) {
            StartCoroutine(gameOver());
        } else if (didPlayerBeatSong) {
            StartCoroutine(playerBeatSong());
        } else {
            StartCoroutine(playerBeatSong());
        }

        resetSongLoop();
    }

    private IEnumerator playerBeatSong() {
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 1.0f, 1.0f));
        yield return StartCoroutine(fadeOutAudio(1.0f));
        songCompleteDisplay.text = "Song Complete!";
        songCompleteDisplay.color = new Color32(0, 236, 117, 255);
        songCompleteDisplay.enabled = true;
        pressSpaceButton.SetActive(true);
    }

    private IEnumerator fadeOutAudio(float fadeDuration) {
        float startVolume = audioSource.volume;
        float targetVolume = 0f; 

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration) {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        audioSource.volume = targetVolume;
        audioSource.Stop();
        audioSource.volume = startVolume; //reset
    }

    private IEnumerator gameOver() {
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 0.5f, 1.0f));
        yield return new WaitForSeconds(1.0f);
        songCompleteDisplay.text = "Game Over";
        songCompleteDisplay.color = Color.red;
        songCompleteDisplay.enabled = true;
        pressSpaceButton.SetActive(true);
    }

    private void resetSongLoop() {
        hasArrowsStarted = false;
        hasSongStarted = false;
        spawnedArrowManager.resetSpawnedArrowManager();
        score = 0.0f;
        scoreDisplay.text = score.ToString("N0");
        delayStartSeconds = 3.0f;
        songTime = 0.0f;
        resetArrows();
    }

    private void setupLevel() {
        if (level < levels.Count) {
            Level currentLevel = levels[level - 1];
            Sprite background = Resources.Load<Sprite>($"Levels/Backgrounds/{currentLevel.level_sprites[0].sprite_name}");
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = background;
            levelNameDisplay.text = currentLevel.name;
            scoreNeededToClearLevel = (currentLevel.completion_percent / 100.0f) * spawnedArrowManager.getMaximumBasePointsForCurrentSong();
            setupLevelProgress(currentLevel);
        }
    }

    private void setupLevelProgress(Level currentLevel) {
        progressBar.value = 0.0f;
        progressBar.maxValue = scoreNeededToClearLevel;
    }

    private void handleUpgradesAndChallenges() {
        UpgradeTracker.enableAllUpgrades();
        handleOutageChallenge();
        handleSecurityCameraUpgrade();
        handleOverclockChallenge();
    }
}