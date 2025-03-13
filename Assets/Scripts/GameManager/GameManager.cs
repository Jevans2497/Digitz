using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public partial class GameManager: MonoBehaviour {

    public bool isInGenerateSongJSONMode = false; // Set to true when generating spawned Arrow JSON files
    public bool isInTestSongMode = false; // Prevents game from being won/lost
    public float skipToTime = 0.0f; // Skip to x amount of seconds in song track

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

    private bool hasArrowsStarted;
    private bool hasSongStarted;

    private float songTime;
    private float delayStartSeconds = 3.0f;
    private float score;

    private string currentSong;
    private List<Song> songs;

    private List<Level> levels;
    private Level currentLevel;

    private float scoreNeededToClearLevel;

    [SerializeField] JSONLoader jsonLoader;
    [SerializeField] SpawnedArrowManager spawnedArrowManager;

    private bool inSongLoop = false;

    private bool isInitialGameStart = true;
    private bool isGameComplete;

    private void Start() {
        isTutorial = SceneManager.GetActiveScene().name == "Tutorial";
        if (isTutorial) {
            setupForTutorial();
        } else {
            songs = jsonLoader.loadSongs();
            levels = jsonLoader.loadLevels();
            currentSong = "Dizzy";
        }

        if (levels.Count > 0) {
            currentLevel = levels[0];
        }

        setupFireworks();
        arrowsList = new List<Arrow> { leftArrow, upArrow, rightArrow, downArrow };
        if (skipToTime > 0.0f) {
            songTime = skipToTime;
        }

        pressSpaceButton.SetActive(true);

        if (isInGenerateSongJSONMode || isInTestSongMode) {
            setupForDevMode();
        }

        setupSong();
    }

    private void Update() {
        if (isTutorial) {
            updateForTutorial();
        } else {
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (isGameComplete) {
                    SceneManager.LoadScene("Main Menu");
                } else {
                    if (!isTutorial && isInitialGameStart && !isInGenerateSongJSONMode && !isInTestSongMode) {
                        startMenuLoop();
                        isInitialGameStart = false;
                    }
                    if (songCompleteDisplay.enabled) {
                        startMenuLoop();
                    }
                    if (menuCanvasManager.isMenuLoopFinished) {
                        startSongLoop();
                    }
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
    }

    private void setupForDevMode() {
        progressBar.gameObject.SetActive(false);
        scoreDisplay.gameObject.SetActive(false);
        displayTimer.gameObject.SetActive(true);
    }

    private void startMenuLoop() {
        songCompleteDisplay.enabled = false;
        pressSpaceButton.SetActive(false);
        stopFireworks();
        menuCanvasManager.startMenuLoop();        
    }

    private void startSongLoop() {
        if (AudioManager.Instance != null) {
            AudioManager.Instance.stopMenuMusic();
        }
        pressSpaceButton.SetActive(false);
        setupSpawnedArrowManager();        
        setupLevel();
        levelNameDisplay.enabled = true;
        handleUpgradesAndChallenges();
        StartCoroutine(showAndFadeLevelName(levelNameDisplay, delayStartSeconds));
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 1.0f, 1.5f, 0.9f));
        inSongLoop = true;
    }

    private void manageSongLoop() {
        songTime += Time.deltaTime * audioSource.pitch;
        displayTimer.text = songTime.ToString("F2");

        if (isTutorial) {
            scoreDisplay.text = $"<size=200%>{score.ToString("N0")}</size>";
        } else {
            scoreDisplay.text = $"<size=200%>{score.ToString("N0")}</size> / {scoreNeededToClearLevel.ToString("N0")}";
        }

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

    public Level getCurrentLevel() {
        return currentLevel;
    }

    public int getLevelNumber() {
        return currentLevel.level_number;
    }

    public void setSong(string songFileName) {
        currentSong = songFileName;
        setupSong();
    }

    public Song getCurrentSong() {
        return songs.Find(song => normalizeSongString(song.name) == normalizeSongString(currentSong));
    }

    private string normalizeSongString(string songString) {
        return songString.Replace(" ", "").ToLower();
    }

    public void setPressSpaceButtonActive(bool isActive) {
        pressSpaceButton.SetActive(isActive);
    }

    private void resetSongLoop() {
        hasArrowsStarted = false;
        hasSongStarted = false;
        spawnedArrowManager.resetSpawnedArrowManager();
        score = 0.0f;
        scoreDisplay.text = "";
        delayStartSeconds = 3.0f;
        songTime = 0.0f;
        resetArrows();
        FeedbackData.currentSongFeedbackCounter = new Dictionary<FeedbackType, int>();
        detourGameObject.SetActive(false);
    }

    private void setupLevel() {
        Sprite background = Resources.Load<Sprite>($"Levels/Backgrounds/{currentLevel.level_sprites[0].sprite_name}");
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = background;
        levelNameDisplay.text = currentLevel.name;
        setupLevelProgress(currentLevel);
    }

    private void setupLevelProgress(Level currentLevel) {
        scoreNeededToClearLevel = (currentLevel.completion_percent / 100.0f) * spawnedArrowManager.getMaximumBasePointsForCurrentSong();
        progressBar.value = 0.0f;

        if (LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.tenPercentCoupon) {
            score = scoreNeededToClearLevel / 10.0f;
            progressBar.value = score;
        }

        progressBar.maxValue = scoreNeededToClearLevel;
    }

    private void handleUpgradesAndChallenges() {
        UpgradeTracker.enableAllUpgrades();
        handleSecurityCameraUpgrade();
        handleChallenges();
    }
}