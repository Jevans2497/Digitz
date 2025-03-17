using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTextManager : MonoBehaviour {

    public TextMeshProUGUI dialogueText;
    private float typingSpeed = 0.025f;
    //private float typingSpeed = 0.0001f;
    public GameObject pressSpaceButton;

    public enum TutorialMessage {
        introduction, royaltyFree1, gameBasics1, gameBasics2, upgrades1, upgrades2, challenges1, challenges2, challenges3, challenges4, simultaneousArrows, royaltyFree2, gameGoal1, gameGoal2, gameGoal3
    }

    Dictionary<TutorialMessage, string> messages = new Dictionary<TutorialMessage, string>();

    private void Start() {
        messages[TutorialMessage.introduction] = "Welcome to Digitz! Let's start with the basics. Use the arrow keys (or WASD) to hit the spawned arrows at the proper time.";
        messages[TutorialMessage.royaltyFree1] = "We'll start you off with some corny royalty free music and see how you do before hitting the big leagues.";
        messages[TutorialMessage.gameBasics1] = "That was<0.75ff>... <r>really good!<1f> <0.015f>For your first time.<0.85f> <r>You may have noticed that when you hit the arrows, you received \"feedback.\"";
        messages[TutorialMessage.gameBasics2] = "There are 5 main feedback types, each with a default score. But wait! That's just the start...";
        messages[TutorialMessage.upgrades1] = "After each song, you'll select a permanent upgrade. Upgrades can affect feedback, how fast arrows move, and a whole lot more!";
        messages[TutorialMessage.upgrades2] = "Hover your mouse over the upgrades to see what they do. Don't worry too much about the details for now.";
        messages[TutorialMessage.challenges1] = "You'll also need to select a challenge. Challenges only affect the next song.";
        messages[TutorialMessage.challenges2] = "Keep an eye out for the challenge's Severity. Challenges with higher Severity are harder.";
        messages[TutorialMessage.challenges3] = "Accepting the first challenge grants a level-specific bonus. Revealing other challenges forfeits this bonus so choose wisely.";
        messages[TutorialMessage.challenges4] = "Challenge Severity's increase as you progress through each level, but you'll get stronger as well.";
        messages[TutorialMessage.royaltyFree2] = "Let's try another song with your shiny new upgrade and challenge. Still royalty free, but a little jazzier. Full disclosure, this one's tough.";
        messages[TutorialMessage.simultaneousArrows] = "This song includes some \"simultaneous arrows\" at the end, you can't miss em.";
        messages[TutorialMessage.gameGoal1] = "Awesome! We're almost done. The goal of the game is to make it through every level<0.45f> <r>(I know,<0.45f> <r>cliche).";
        messages[TutorialMessage.gameGoal2] = "Each Level has a completion score needed to clear it, viewable in the top left. The completion score is based on the current level and the song chosen.";
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
                    if (manipulateTimeString.Contains("r")) { //reset typing speed to default
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
