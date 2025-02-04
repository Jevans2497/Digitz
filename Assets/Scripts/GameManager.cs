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
                    UpgradeTracker.disableUpgrade(upgrade);
                }
            }
        }
    }

    private IEnumerator showAndFadeLevelName(TextMeshProUGUI textMeshObject, float duration) {
        Color white = Color.white;
        Color transparentColor = new Color(white.r, white.g, white.b, 0);
        textMeshObject.color = transparentColor;

        float elapsedTime = 0f;
        float fifthOfDuration = duration / 5.0f;

        textMeshObject.enabled = true;

        //Fade in
        while (elapsedTime < (fifthOfDuration * 2.0f)) {
            float t = elapsedTime / (fifthOfDuration * 2.0f);
            textMeshObject.color = Color.Lerp(transparentColor, white, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMeshObject.color = white;

        yield return new WaitForSeconds(fifthOfDuration * 2);
        elapsedTime = 0.0f;

        while (elapsedTime < fifthOfDuration) {
            float t = elapsedTime / fifthOfDuration;
            textMeshObject.color = Color.Lerp(white, transparentColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMeshObject.color = transparentColor;

        textMeshObject.enabled = false;
    }

    private IEnumerator changeSpriteAlpha(SpriteRenderer spriteRenderer, float delay, float duration, float alpha) {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;

        Color originalColor = spriteRenderer.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}