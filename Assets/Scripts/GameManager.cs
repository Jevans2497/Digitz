using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager: MonoBehaviour {

    [SerializeField] TextMeshProUGUI displayTimer;
    [SerializeField] TextMeshProUGUI countdown;
    [SerializeField] TextMeshProUGUI scoreDisplay;
    [SerializeField] AudioSource audioSource;
    [SerializeField] MenuCanvasManager menuCanvasManager;

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
        handleUpgradesAndChallenges();
    }

    private void manageSongLoop() {
        songTime += Time.deltaTime * audioSource.pitch;
        displayTimer.text = songTime.ToString("F2");
        scoreDisplay.text = score.ToString("N0");

        manageDelayCountdown();

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

    private void manageDelayCountdown() {
        if (delayStartSeconds >= 0.0f) {
            countdown.enabled = true;
            updateDelayCountdown();
            delayStartSeconds -= Time.deltaTime * audioSource.pitch;
        } else if (delayStartSeconds <= 0.0f) {
            countdown.enabled = false;
        }
    }

    private void updateDelayCountdown() {
        countdown.text = delayStartSeconds.ToString("F0");
    }

    private void songFinished() {
        resetSongLoop();
        menuCanvasManager.startMenuLoop();
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
        }
    }

    private void handleUpgradesAndChallenges() {
        handleSecurityCameraUpgrade();
        handleOverclockChallenge();
        handleOutageChallenge();
        handleShatteredSwordChallenge();
    }

    private void handleSecurityCameraUpgrade() {
        if (UpgradeTracker.hasUpgrade(Upgrade.UpgradeEffect.SecurityCamera)) {
            resetArrows();
            Arrow randomArrow = arrowsList[UnityEngine.Random.Range(0, arrowsList.Count)];
            randomArrow.setThisArrowToSecurityCameraUpgradeArrow();
        }
    }

    private void resetArrows() {
        foreach (var arrow in arrowsList) {
            arrow.resetArrowForSecurityCameraUpgrade();
        }
    }

    private void handleOverclockChallenge() {
        if (ChallengeTracker.hasChallenge(Challenge.ChallengeEffect.Overclock)) {
            Challenge overclock = ChallengeTracker.getChallenge();
            float overclockSpeed = 0.02f * overclock.getSeverityMultiplier();
            audioSource.pitch = 1 + overclockSpeed;
        } else {
            audioSource.pitch = 1;
        }
    }

    private void handleOutageChallenge() {
        if (ChallengeTracker.hasChallenge(Challenge.ChallengeEffect.Outage)) {
            Challenge outage = ChallengeTracker.getChallenge();
            foreach (var upgrade in UpgradeTracker.GetUpgrades()) {
                int randomInt = UnityEngine.Random.Range(0, 10);
                if (randomInt <= outage.getSeverityMultiplier() - 1) {
                    upgrade.isEnabled = false;
                }
            }
        } else {
            UpgradeTracker.enableAllUpgrades();
        }
    }

    private void handleShatteredSwordChallenge() {
        Challenge challenge = ChallengeTracker.getChallenge();
        if (challenge != null && challenge.effect == Challenge.ChallengeEffect.ShatteredSword) {
            UpgradeTracker.removeLastAcquiredUpgrade();
        }
    }
}