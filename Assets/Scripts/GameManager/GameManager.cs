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
        if (Input.GetKeyDown(KeyCode.Space) && menuCanvasManager.isMenuLoopFinished) {
            startSongLoop();
        }
        if (inSongLoop) {
            manageSongLoop();
        }

        // Cheat codes for debugging
        if (Input.GetKeyDown(KeyCode.P)) {
            score += 1000f;
        }
        if (Input.GetKeyDown(KeyCode.N)) {
            songFinished();
        }
    }

    private void startSongLoop() {
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
        return didPlayerBeatSong || didSongReachEnd;
    }

    private void songFinished() {
        spawnedArrowManager.destroyCurrentExistingArrows();

        bool didPlayerBeatSong = score >= scoreNeededToClearLevel;
        bool didSongReachEnd = !audioSource.isPlaying && hasSongStarted == true;

        if (didSongReachEnd) {
            gameOver();
        } else if (didPlayerBeatSong) {
            playerBeatSong();
        } else {
            //For debugging purposes, default to playerBeatSong
            playerBeatSong();
        }

        resetSongLoop();
    }

    private void playerBeatSong() {
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 0.5f, 1.0f));
        menuCanvasManager.startMenuLoop();
    }

    private void gameOver() {

    }

    private void resetSongLoop() {
        inSongLoop = false;
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