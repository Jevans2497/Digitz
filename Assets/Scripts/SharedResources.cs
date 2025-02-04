using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class SharedResources: MonoBehaviour {

    public enum Direction { Left, Up, Right, Down, None }

    public static Direction convertStringToDirection(string directionString) {
        switch (directionString) {
            case "left":
            return Direction.Left;
            case "up":
            return Direction.Up;
            case "right":
            return Direction.Right;
            case "down":
            return Direction.Down;
            default:
            return Direction.None;
        }
    }

    public static string getRandomDirectionAsString() {
        int randomInt = UnityEngine.Random.Range(0, 4);
        switch (randomInt) {
            case 0:
            return "left";
            case 1:
            return "up";
            case 2:
            return "right";
            case 3:
            return "down";
        }
        return "right";
    }

    public static Color hexToColor(string hex) {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color)) {
            return color;
        } else {
            Debug.LogError("Invalid Hex Color string");
            return Color.white;
        }
    }
}