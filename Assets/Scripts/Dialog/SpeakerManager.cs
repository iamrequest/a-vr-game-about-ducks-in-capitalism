using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogSpeaker {
   Me, Maid
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

    private void Start() {
        // Validate that we have enough colors in our array
        if (dialogBoxColors.Length != System.Enum.GetValues(typeof(DialogSpeaker)).Length) {
            Debug.LogError("Missing a few colors for some speakers!");
        }
    }
    public Color GetSpeakerColor(DialogSpeaker currentSpeaker) {
        return dialogBoxColors[(int)currentSpeaker];
    }
}
