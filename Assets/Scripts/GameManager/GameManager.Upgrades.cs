using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour {
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
}
