using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public partial class GameManager: MonoBehaviour {

    [SerializeField] TextMeshProUGUI displayTimer;
    [SerializeField] TextMeshProUGUI countdown;
    [SerializeField] TextMeshProUGUI scoreDisplay;
    [SerializeField] AudioSource audioSource;
    [SerializeField] MenuCanvasManager menuCanvasManager;
    [SerializeField] SpriteRenderer blackBackgroundOverlay;

    [SerializeField] Arrow leftArrow;
    [SerializeField] Arrow upArrow;
    [SerializeField] Arrow rightArrow;
    [SerializeField] Arrow downArrow;
    List<Arrow> arrowsList;

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
    }

    private void startSongLoop() {
        inSongLoop = true;
        level += 1;        
        setupLevel();
        countdown.enabled = true;
        handleUpgradesAndChallenges();
        StartCoroutine(showAndFadeLevelName(countdown, delayStartSeconds));
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 1.0f, 1.5f, 0.9f));
    }

    private void manageSongLoop() {
        songTime += Time.deltaTime * audioSource.pitch;
        displayTimer.text = songTime.ToString("F2");
        scoreDisplay.text = score.ToString("N0");

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

        if (!audioSource.isPlaying && hasSongStarted == true) {
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

    private void startSpawningArrows() {
        hasArrowsStarted = true;
        SongPreset song = jsonLoader.LoadSong(currentSong);

        if (!isInGenerateSongJSONMode) {
            spawnedArrowManager.setup(song);
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
    }

    public void multiplyScore(float multiplier) {
        score *= multiplier;
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

    private void songFinished() {
        resetSongLoop();
        menuCanvasManager.startMenuLoop();
        StartCoroutine(changeSpriteAlpha(blackBackgroundOverlay, 0, 0.5f, 1.0f));
    }

    private void resetSongLoop() {
        inSongLoop = false;
        hasArrowsStarted = false;
        hasSongStarted = false;
        spawnedArrowManager.resetSpawnedArrowManager();
        score = 0.0f;
        delayStartSeconds = 3.0f;
        songTime = 0.0f;
        resetArrows();
    }

    private void setupLevel() {
        if (level < levels.Count) {
            Level currentLevel = levels[level];
            Sprite background = Resources.Load<Sprite>($"Levels/Backgrounds/{currentLevel.level_sprites[0].sprite_name}");
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = background;
            countdown.text = currentLevel.name;
        }
    }

    private void handleUpgradesAndChallenges() {
        UpgradeTracker.enableAllUpgrades();
        handleOutageChallenge();
        handleSecurityCameraUpgrade();
        handleOverclockChallenge();
    }
}