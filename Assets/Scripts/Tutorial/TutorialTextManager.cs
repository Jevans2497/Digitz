using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTextManager : MonoBehaviour {

    public TextMeshProUGUI dialogueText;
    private float typingSpeed = 0.025f;
    public GameObject pressSpaceButton;

    public enum TutorialMessage {
        introduction, royaltyFree1, upgrades, challenges1, challenges2, challenges3, simultaneousArrows, royaltyFree2, gameGoal1, gameGoal2, gameGoal3
    }

    Dictionary<TutorialMessage, string> messages = new Dictionary<TutorialMessage, string>();

    private void Start() {
        messages[TutorialMessage.introduction] = "Welcome to Digitz! Let's start with the basics. Use the arrow keys to hit the spawned arrows at the proper time.";
        messages[TutorialMessage.royaltyFree1] = "We'll start you off with some corny royalty free music and see how you do before hitting the big leagues.";
        messages[TutorialMessage.upgrades] = "That was<1f>... <r>really good!<1f> <r>For your first time.<0.85f> <r>After each song, you'll select a permanent upgrade. Hover your mouse over the upgrades to see what they do.";
        messages[TutorialMessage.challenges1] = "You'll also need to select a challenge. Challenges only affect the next song.";
        messages[TutorialMessage.challenges2] = "Keep an eye out for the challenge's Severity. Challenges with higher Severity are harder.";
        messages[TutorialMessage.challenges3] = "Challenge Severity's increase as you progress through each level, but you'll get stronger as well.";
        messages[TutorialMessage.royaltyFree2] = "Let's try another song with your shiny new upgrade and challenge. Still royalty free, but a little jazzier.";
        messages[TutorialMessage.simultaneousArrows] = "This song includes some \"simultaneous arrows\" at the end, you can't miss em.";
        messages[TutorialMessage.gameGoal1] = "Awesome! We're almost done. The goal of the game is to make it through every level (I know, cliche).";
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
        bool isManipulatingTime = false;
        string manipulateTimeString = "";
        float currentTypingSpeed = typingSpeed;

        //Added the ability to change typingSpeed based on string to make the tutorial more engaging. 
        foreach (char letter in message.ToCharArray()) {
            if (letter == '<') {
                isManipulatingTime = true;
            }
            
            if (isManipulatingTime) {
                manipulateTimeString += letter;
                if (letter == '>') {
                    isManipulatingTime = false;
                    if (manipulateTimeString.Contains("r")) {
                        currentTypingSpeed = typingSpeed;
                    } else {
                        currentTypingSpeed = extractFloatFromTag(manipulateTimeString);
                    }
                    manipulateTimeString = "";
                }
                yield return null;
            } else {
                dialogueText.text += letter;
                yield return new WaitForSeconds(currentTypingSpeed);
            }
        }
        pressSpaceButton.SetActive(true);
    }

    private float extractFloatFromTag(string tag) {
        Match match = Regex.Match(tag, @"<([\d.]+)f>");
        if (match.Success) {
            return float.Parse(match.Groups[1].Value);
        } else {
            return typingSpeed;
        }
    }
}
