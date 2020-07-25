using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public enum DialogSpeaker {
   // TODO: Change this once speakers are defined
   Me, Maid
}
public enum LookatTargets {
   NoChange, Player, Mug, Down, Up, Bonsai
}
public class SpeakerManager : MonoBehaviour {
    public static SpeakerManager instance;
    // Start is called before the first frame update
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }


    public Color[] dialogBoxColors;
    public Sprite npcTextbox;
    public Sprite playerTextbox;

    [Header("Speakers")]
    public DuckNPC testDuck;

    [Header("Lookat Targets")]
    public Transform lookatMugTransform;
    public Transform lookDownTransform, lookUpTransform, lookatBonsaiTransform;


    private void Start() {
        // Validate that we have enough colors in our array
        if (dialogBoxColors.Length != System.Enum.GetValues(typeof(DialogSpeaker)).Length) {
            Debug.LogError("Missing a few colors for some speakers!");
        }
    }
    public Color GetSpeakerColor(DialogSpeaker currentSpeaker) {
        return dialogBoxColors[(int)currentSpeaker];
    }
    public Transform GetLookatTarget(LookatTargets lookatTarget) {
        switch (lookatTarget) {
            case LookatTargets.NoChange: return null;
            case LookatTargets.Player: return Player.instance.hmdTransform;
            case LookatTargets.Mug: return lookatMugTransform;
            case LookatTargets.Down: return lookDownTransform;
            case LookatTargets.Up: return lookUpTransform;
            case LookatTargets.Bonsai: return lookatBonsaiTransform;
            default: Debug.LogError("Lookat target doesn't exist!");
                     return Player.instance.hmdTransform;
        }
    }

    public DuckNPC GetNPCSpeaker(DialogSpeaker speaker) {
        switch(speaker) {
            // TODO: Change this once speakers are defined
            case DialogSpeaker.Maid: return testDuck;
            default: return null;
        }
    }
}
