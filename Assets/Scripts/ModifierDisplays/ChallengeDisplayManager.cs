using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeDisplayManager: MonoBehaviour {

    [SerializeField] GameObject challengeDisplayPrefab;
    [SerializeField] Transform challengeDisplayTransform;    

    private KeyValuePair<Challenge, GameObject> challengeDisplayObject = new();

    private void Start() {
        ChallengeTracker.setChallengeDisplayManager(this);
    }

    public void challengeAdded(Challenge challenge) {
        GameObject challengeDisplayObject = Instantiate(challengeDisplayPrefab, challengeDisplayTransform);
        setChallengeDisplay(challenge, challengeDisplayObject);
        this.challengeDisplayObject = new KeyValuePair<Challenge, GameObject>(challenge, challengeDisplayObject);
    }

    private void setChallengeDisplay(Challenge challenge, GameObject challengeDisplayObject) {
        GameObject foreground = challengeDisplayObject.transform.Find("ChallengeDisplayForeground").gameObject;
        foreground.GetComponent<Image>().color = SharedResources.hexToColor(challenge.color);

        // Set image
        GameObject image = foreground.transform.Find("ChallengeDisplayImage").gameObject;
        Sprite sprite = Resources.Load<Sprite>($"MenuItems/Challenges/ChallengeIcons/{challenge.sprite_name}");
        image.GetComponent<Image>().sprite = sprite;

        // Set Tooltip
        Tooltip tooltip = challengeDisplayObject.GetComponent<Tooltip>();
        tooltip.message = challenge.name + ":\n\n" + challenge.description;

        setupPandorasBoxDisplay(challengeDisplayObject);
    }

    private void setupPandorasBoxDisplay(GameObject challengeDisplayObject) {
        Challenge pandorasBox = ChallengeTracker.getPandorasBoxIfActive();
        if (pandorasBox != null) {
            GameObject pandorasBoxObject = challengeDisplayObject.transform.Find("PandorasBoxDisplay").gameObject;
            pandorasBoxObject.SetActive(true);
        }
    }

    public void challengeRemoved(Challenge challenge) {
        Destroy(challengeDisplayObject.Value);
    }
}
