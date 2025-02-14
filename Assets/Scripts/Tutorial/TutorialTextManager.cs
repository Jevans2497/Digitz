using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTextManager : MonoBehaviour {

    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.01f;
    public GameObject pressSpaceButton;

    public enum TutorialMessage {
        introduction, upgrades, challenges1, challenges2, challenges3, multipleArrows, gameGoal1, gameGoal2, gameGoal3
    }

    Dictionary<TutorialMessage, string> messages = new Dictionary<TutorialMessage, string>();

    private void Start() {
        messages[TutorialMessage.introduction] = "Welcome to Digitz! Let's start with the basics. Use the arrow keys to hit the spawned arrows at the proper time.";
        messages[TutorialMessage.upgrades] = "Great job! After each song, you'll select a permanent upgrade. Hover your mouse over the upgrades to see what they do.";
        messages[TutorialMessage.challenges1] = "You'll also need to select a challenge. Challenges only affect the next song.";
        messages[TutorialMessage.challenges2] = "Keep an eye out for the challenge's Severity. Challenges with higher Severity are harder.";
        messages[TutorialMessage.challenges3] = "Challenge Severity's increase as you progress through each level, but you'll get stronger as well.";
        messages[TutorialMessage.multipleArrows] = "Let's try another song with your shiny new upgrade and challenge. This song includes some \"parallel arrows,\" you can't miss em.";
        messages[TutorialMessage.gameGoal1] = "Awesome! We're almost done. The goal of the game is to make it through every Level.";
        messages[TutorialMessage.gameGoal2] = "Each Level has a completion score needed to clear it. The completion score is calculated based on the current level and the song chosen.";
        messages[TutorialMessage.gameGoal3] = "Alright, I think you're ready! You won't pick an upgrade in Level 1, just the challenge and the song. Have fun!";
    }

    public void showMessage(TutorialMessage tutorialMessage) {
        string messageText = messages[tutorialMessage];
        if (messageText != null) {
            startTyping(messageText);
        }
    }

    private void startTyping(string message) {
        StopAllCoroutines();
        pressSpaceButton.SetActive(false);
        StartCoroutine(typeText(message));
    }

    private IEnumerator typeText(string message) {
        dialogueText.text = "";
        foreach (char letter in message.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        pressSpaceButton.SetActive(true);
    }
}
