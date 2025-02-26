using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static SharedResources;
using static GameManager;

public class SpawnedArrowManager: MonoBehaviour {

    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject upArrow;
    [SerializeField] GameObject rightArrow;
    [SerializeField] GameObject downArrow;

    [SerializeField] GameObject leftArrowPrefab;
    [SerializeField] GameObject upArrowPrefab;
    [SerializeField] GameObject rightArrowPrefab;
    [SerializeField] GameObject downArrowPrefab;

    [SerializeField] GameManager gameManager;

    [SerializeField] ParticleSystem fireEffectArrowParticleSystem;
    [SerializeField] AudioClip fireEffectClip;

    public Transform leftSpawn, upSpawn, rightSpawn, downSpawn;

    float defaultSpeed = 1f;
    private List<ArrowSpawnData> arrowSpawnDataList = new List<ArrowSpawnData>();
    private int currentArrowIndex;
    private bool shouldSpawnArrows;
    private float maximumBasePointsForSong; // The score for the song assuming no upgrades and every feedback is perfect.

    private List<GameObject> currentlyExistingArrows = new List<GameObject>();    

    private Direction deathBeamRandomDirection;
    public Direction detourRandomDirection;

    private void Update() {
        if (shouldSpawnArrows) {
            float songTime = gameManager.getSongTime();
            if (currentArrowIndex < arrowSpawnDataList.Count && arrowSpawnDataList[currentArrowIndex].arrowData.timestamp <= songTime) {
                spawnArrow(arrowSpawnDataList[currentArrowIndex]);
                currentArrowIndex += 1;
            }
        }
    }

    public void resetSpawnedArrowManager() {
        arrowSpawnDataList = new List<ArrowSpawnData>();
        currentArrowIndex = 0;
        shouldSpawnArrows = false;
    }

    public void setup(SongPreset song) {
        deathBeamRandomDirection = (Direction)Random.Range(0, 4);
        detourRandomDirection = (Direction)Random.Range(0, 4);

        defaultSpeed = song.default_speed;
        createArrowSpawnDataList(song);
        preproccessArrowTimestampForArrivalTime();
        preprocessArrowsForArrowEffects();
        maximumBasePointsForSong = arrowSpawnDataList.Count * 100.0f;
    }

    public void setShouldSpawnArrows(bool value) {
        shouldSpawnArrows = value;
    }

    public float getMaximumBasePointsForCurrentSong() {
        return maximumBasePointsForSong;
    }

    private void createArrowSpawnDataList(SongPreset song) {
        int index = 0;
        foreach (ArrowData arrowData in song.arrows) {
            Direction spawnDirection = getPotentiallyModifiedArrowDirection(arrowData.arrow_direction, index, song.arrows.Count);            
            ArrowSpawnData spawnData = GetSpawnData(spawnDirection, arrowData);
            if (spawnData != null) {
                arrowSpawnDataList.Add(spawnData);
            }
            index += 1;
        }
    }

    /**
     * We don't care when arrows leave, we care when they arrive. 
     * This function calculates when the arrow would need to leave it's spawn point in order to arrive at it's destination at the expected timestamp
    **/
    private void preproccessArrowTimestampForArrivalTime() {
        foreach (ArrowSpawnData spawnData in arrowSpawnDataList) {
            Vector3 startPosition = spawnData.spawnPoint.transform.position;
            Vector3 targetPosition = spawnData.targetPosition.position;
            float journeyLength = Vector3.Distance(startPosition, targetPosition);

            float arrowSpeed = spawnData.arrowData.arrow_speed != 0 ? spawnData.arrowData.arrow_speed : defaultSpeed;

            //We have to check for arrowSpeed modifiers here before the timestamps are processed.
            if (LevelBonusTracker.getActiveBonusEffect() == LevelBonus.LevelBonusEffect.slowArrows) {
                arrowSpeed -= 0.7f;
            }

            if (ChallengeTracker.getChallenge() != null && ChallengeTracker.getChallenge().effect == Challenge.ChallengeEffect.Supersonic) {
                arrowSpeed += ChallengeTracker.getChallenge().getSeverityMultiplier() * 0.2f;
            }

            if (ChallengeTracker.getChallenge() != null && ChallengeTracker.getChallenge().effect == Challenge.ChallengeEffect.DeathBeam) {
                if (deathBeamRandomDirection == SharedResources.convertStringToDirection(spawnData.arrowData.arrow_direction)) {
                    arrowSpeed += ChallengeTracker.getChallenge().getSeverityMultiplier() * 0.5f;
                }
                    
            }

            spawnData.arrowData.arrow_speed = arrowSpeed;

            float travelTime = journeyLength / arrowSpeed;
            spawnData.arrowData.timestamp -= travelTime;
        }


        alterSpawnedArrowsForChallengeIfNeeded();

        arrowSpawnDataList.Sort((a, b) => a.arrowData.timestamp.CompareTo(b.arrowData.timestamp));
    }

    private void preprocessArrowsForArrowEffects() {
        if (UpgradeTracker.hasUpgrade(Upgrade.UpgradeEffect.Houston)) {
            ArrowData lastArrowData = arrowSpawnDataList[^1].arrowData;
            lastArrowData.arrowEffect = ArrowData.ArrowEffect.golden;
        }

        Challenge challenge = ChallengeTracker.getChallenge();
        if (challenge != null && isChallengeElemental(challenge)) {
            addChallengeEffectToArrows(challenge);
        }

        arrowSpawnDataList.ForEach(spawnData => spawnData.arrowData.applyEffectToArrow());
    }

    private bool isChallengeElemental(Challenge challenge) {
        return challenge.effect == Challenge.ChallengeEffect.Frosty || challenge.effect == Challenge.ChallengeEffect.ShortCircuit || challenge.effect == Challenge.ChallengeEffect.Blaze || challenge.effect == Challenge.ChallengeEffect.ElementalChaos;
    }

    private void addChallengeEffectToArrows(Challenge challenge) {
        ArrowData.ArrowEffect challengeEffect = ArrowData.ArrowEffect.regular;
        switch (challenge.effect) {
            case Challenge.ChallengeEffect.Frosty:
            challengeEffect = ArrowData.ArrowEffect.freeze;
            break;
            case Challenge.ChallengeEffect.ShortCircuit:
            challengeEffect = ArrowData.ArrowEffect.lightning;
            break;
            case Challenge.ChallengeEffect.Blaze:
            challengeEffect = ArrowData.ArrowEffect.fire;
            break;            
        }        

        int numberOfEffectArrows = (int)challenge.getSeverityMultiplier() * 30;
        for (int i = 0; i < numberOfEffectArrows; i++) {
            int randomIndex = Random.Range(0, arrowSpawnDataList.Count);
            if (challenge.effect == Challenge.ChallengeEffect.ElementalChaos) {
                challengeEffect = getRandomElementalEffect();
            }
            arrowSpawnDataList[randomIndex].arrowData.arrowEffect = challengeEffect;
        }
    }

    private ArrowData.ArrowEffect getRandomElementalEffect() {
        ArrowData.ArrowEffect[] effects = {
        ArrowData.ArrowEffect.fire,
        ArrowData.ArrowEffect.freeze,
        ArrowData.ArrowEffect.lightning
    };
        return effects[Random.Range(0, effects.Length)];
    }

    private void spawnArrow(ArrowSpawnData arrowSpawnData) {
        if (arrowSpawnData.spawnPoint != null && arrowSpawnData.arrowPrefab != null) {
            GameObject spawnedArrow = Instantiate(arrowSpawnData.arrowPrefab, arrowSpawnData.spawnPoint.position, Quaternion.identity);
            currentlyExistingArrows.Add(spawnedArrow);
            SpriteRenderer spriteRenderer = spawnedArrow.GetComponent<SpriteRenderer>();
            spriteRenderer.color = arrowSpawnData.arrowData.color;
            spawnedArrow.layer = arrowSpawnData.arrowData.layer;
            StartCoroutine(moveSpawnedArrowToTargetArrow(arrowSpawnData, spawnedArrow));
        }
    }

    private IEnumerator moveSpawnedArrowToTargetArrow(ArrowSpawnData arrowSpawnData, GameObject spawnedArrow) {
        Transform arrowTransform = spawnedArrow.transform;
        Vector3 startPosition = arrowTransform.position;
        float journeyLength = Vector3.Distance(startPosition, arrowSpawnData.targetPosition.position);
        float startTime = Time.time;

        while (arrowTransform != null) {
            float arrowSpeed = arrowSpawnData.arrowData.arrow_speed;
            float distanceCovered = (Time.time - startTime) * arrowSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            arrowTransform.position = Vector3.Lerp(startPosition, arrowSpawnData.targetPosition.position, fractionOfJourney);

            handleGraveyardChallenge(fractionOfJourney, spawnedArrow);

            if (fractionOfJourney >= 1.00) {
                break;
            }

            yield return null;
        }

        if (arrowTransform != null) {
            StartCoroutine(destroySpawnedArrowAfterDelay(arrowSpawnData.targetArrow, arrowTransform, spawnedArrow.layer));
        }
    }

    private void alterSpawnedArrowsForChallengeIfNeeded() {
        Challenge challenge = ChallengeTracker.getChallenge();
        if (challenge == null) {
            return;
        }

        if (challenge.effect == Challenge.ChallengeEffect.EarlyBird) {
            modifyTimeStampsBy(challenge.getSeverityMultiplier() * -0.025f);
        } else if (challenge.effect == Challenge.ChallengeEffect.LaterGator) {
            modifyTimeStampsBy(challenge.getSeverityMultiplier() * 0.025f);
        } else if (challenge.effect == Challenge.ChallengeEffect.BrokenTape) {
            modifyTimeStampsForBrokenTapeChallenge(challenge.getSeverityMultiplier());
        } else if (challenge.effect == Challenge.ChallengeEffect.Bombardment) {
            addSpeedyArrowsForBombardmentChallenge(challenge.getSeverityMultiplier());
        }
    }

    private void modifyTimeStampsBy(float timeToModify) {
        arrowSpawnDataList.ForEach(spawnData => spawnData.arrowData.timestamp += timeToModify);
    }

    private void modifyTimeStampsForBrokenTapeChallenge(float challengeSeverityMultiplier) {
        foreach (ArrowSpawnData spawnData in arrowSpawnDataList) {
            int randomInt = UnityEngine.Random.Range(0, 4);
            if (randomInt == 0) {
                spawnData.arrowData.timestamp += (challengeSeverityMultiplier * -.025f);
            } else if (randomInt == 1) {
                spawnData.arrowData.timestamp += (challengeSeverityMultiplier * .025f);
            }
            //Else, leave the timestamp as is
        }
    }

    private void addSpeedyArrowsForBombardmentChallenge(float challengeSeverityMultiplier) {
        float lastArrowTimestamp = arrowSpawnDataList[arrowSpawnDataList.Count - 1].arrowData.timestamp;
        float numOfSpeedyArrowsToSpawn = 6 * challengeSeverityMultiplier;
        for (int i = 0; i < numOfSpeedyArrowsToSpawn; i++) {

            //Get random timestamp to spawn arrow at
            float randomFloat = UnityEngine.Random.Range(0, lastArrowTimestamp - 2);

            //Get random direction            
            Direction spawnDirection = getRandomDirection();            

            //Create spawn data
            ArrowData speedyArrowData = new(randomFloat, getRandomDirectionAsString(), ArrowData.ArrowEffect.regular);
            speedyArrowData.arrow_speed = defaultSpeed * 1.5f;
            ArrowSpawnData spawnData = GetSpawnData(spawnDirection, speedyArrowData);

            //Add to list
            arrowSpawnDataList.Add(spawnData);
        }
    }

    private void handleGraveyardChallenge(float fractionOfJourney, GameObject spawnedArrow) {
        Challenge challenge = ChallengeTracker.getChallenge();
        if (challenge != null && challenge.effect == Challenge.ChallengeEffect.Graveyard) {
            float startingAlpha = -.1f - (.135f * challenge.getSeverityMultiplier());            
            float alpha = Mathf.Lerp(startingAlpha, 0.85f, fractionOfJourney);

            Color alphaAdjustedColor = spawnedArrow.GetComponent<SpriteRenderer>().color;
            alphaAdjustedColor.a = alpha;
            spawnedArrow.GetComponent<SpriteRenderer>().color = alphaAdjustedColor;
        }
    }

    private Direction getPotentiallyModifiedArrowDirection(string originalDirection, int spawnedArrowIndex, int songArrowsCount) {
        Challenge challenge = ChallengeTracker.getChallenge();

        if (challenge != null && challenge.effect == Challenge.ChallengeEffect.Subterfuge) {
            float numberOfArrowsToAlter = songArrowsCount * (0.15f * challenge.getSeverityMultiplier()); // 15% per severity level
            if (spawnedArrowIndex < numberOfArrowsToAlter) {
                return getRandomDirection();
            }
        }

        if (challenge != null && challenge.effect == Challenge.ChallengeEffect.Detour) {
            if (detourRandomDirection == SharedResources.convertStringToDirection(originalDirection)) {                
                return getRandomDirection(excludeDirection: detourRandomDirection);
            }
        }

        return convertStringToDirection(originalDirection);
    }

    private IEnumerator destroySpawnedArrowAfterDelay(GameObject targetArrow, Transform arrowTransform, int arrowLayer) {
        //Delay so the user has time to hit arrow before it counts as a miss.
        float destructionDelay = 0.1f;

        if (UpgradeTracker.hasUpgrade(Upgrade.UpgradeEffect.Sharpshooter)) {
            destructionDelay = 0.125f;
        }

        yield return new WaitForSeconds(destructionDelay);

        if (arrowTransform != null) {
            Arrow targetArrowComponent = targetArrow.GetComponent<Arrow>();
            if (targetArrowComponent != null && !isArrowNonScoring(arrowLayer)) { 
                // Report a miss
                targetArrowComponent.handleScoring(10.0f, false);
            }
            Destroy(arrowTransform.gameObject);
        }
    }

    private bool isArrowNonScoring(int arrowLayer) {
        int frozenArrowLayer = 9;
        int lightningArrowLayer = 10;
        int fireArrowLayer = 11;

        return arrowLayer == frozenArrowLayer || arrowLayer == lightningArrowLayer || arrowLayer == fireArrowLayer;
    }

    public void destroyCurrentExistingArrows() {
        foreach (GameObject arrow in currentlyExistingArrows.Where(arrow => arrow != null).ToList()) {
            Destroy(arrow);
        }
        currentlyExistingArrows = new List<GameObject>();
    }

    public IEnumerator destroyCurrentExistingArrowsWithFireEffect() {
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject arrow in currentlyExistingArrows.Where(arrow => arrow != null).ToList()) {
            ParticleSystem fireEffect = Instantiate(fireEffectArrowParticleSystem, arrow.transform.position, Quaternion.identity);
            fireEffect.Play();
            AudioManager.Instance.playSound(fireEffectClip);
            Destroy(arrow);
        }
        currentlyExistingArrows = new List<GameObject>();
    }

    private ArrowSpawnData GetSpawnData(Direction direction, ArrowData arrowData) {
        var directionMapping = new Dictionary<Direction, (Transform spawnPoint, Transform targetPosition, GameObject arrowPrefab, GameObject targetArrow)> {
        { Direction.Left, (leftSpawn, leftArrow.transform, leftArrowPrefab, leftArrow) },
        { Direction.Up, (upSpawn, upArrow.transform, upArrowPrefab, upArrow) },
        { Direction.Right, (rightSpawn, rightArrow.transform, rightArrowPrefab, rightArrow) },
        { Direction.Down, (downSpawn, downArrow.transform, downArrowPrefab, downArrow) }
    };

        // Get the spawn data using the direction
        if (directionMapping.TryGetValue(direction, out var spawnData)) {
            // Return the mapped data wrapped in an ArrowSpawnData object
            return new ArrowSpawnData(arrowData, direction, spawnData.spawnPoint, spawnData.targetPosition, spawnData.arrowPrefab, spawnData.targetArrow);
        }

        return null;
    }
}

public class ArrowSpawnData {
    public ArrowData arrowData;
    public Direction direction;
    public Transform spawnPoint;
    public Transform targetPosition;
    public GameObject arrowPrefab;
    public GameObject targetArrow;

    public ArrowSpawnData(ArrowData arrowData, Direction direction, Transform spawnPoint, Transform targetPosition, GameObject arrowPrefab, GameObject targetArrow) {
        this.arrowData = arrowData;
        this.spawnPoint = spawnPoint;
        this.targetPosition = targetPosition;
        this.arrowPrefab = arrowPrefab;
        this.targetArrow = targetArrow;
    }
}