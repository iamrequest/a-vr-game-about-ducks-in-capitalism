using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public enum DialogSpeaker {
   Me, Suzie, Sam, WORK_DUCK_ANON, ART_DUCK_ANON 
}
public enum NPCAnimation {
   // Set lookat target
   NoChange, LookPlayer, LookMug, LookDown, LookUp, LookBonsai,

   // Animation
   SetCoffee, UnsetCoffee, SipCoffee, Shout
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
    public DuckNPC workDuck;
    public DuckNPC artDuck;

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
    public Transform GetLookatTarget(NPCAnimation animation) {
        switch (animation) {
            // Lookat targets: Return an appropriate transform
            case NPCAnimation.NoChange: return null;
            case NPCAnimation.LookPlayer: return Player.instance.hmdTransform;
            case NPCAnimation.LookMug: return lookatMugTransform;
            case NPCAnimation.LookDown: return lookDownTransform;
            case NPCAnimation.LookUp: return lookUpTransform;
            case NPCAnimation.LookBonsai: return lookatBonsaiTransform;

            // Animations: No lookat target
            case NPCAnimation.SetCoffee: return null;
            case NPCAnimation.UnsetCoffee: return null;
            case NPCAnimation.SipCoffee: return null;
            case NPCAnimation.Shout: return null;
            default: Debug.LogError("Lookat target doesn't exist!");
                     return Player.instance.hmdTransform;
        }
    }

    public DuckNPC GetNPCSpeaker(DialogSpeaker speaker) {
        switch(speaker) {
            case DialogSpeaker.Suzie: return workDuck;
            case DialogSpeaker.Sam: return artDuck;

            case DialogSpeaker.WORK_DUCK_ANON: return workDuck;
            case DialogSpeaker.ART_DUCK_ANON: return artDuck;
            default: return null;
        }
    }
}
