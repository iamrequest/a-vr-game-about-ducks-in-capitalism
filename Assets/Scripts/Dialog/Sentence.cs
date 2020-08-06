using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sentence {
    public AnimationState animationState;
    public DialogSpeaker currentSpeaker;
    public NPCAnimation npcAnimation;
    public string text;

    public Sentence() { }
    public Sentence(AnimationState animationState, 
        DialogSpeaker currentSpeaker, 
        NPCAnimation npcAnimation,
        string text) {

            this.animationState = animationState;
            this.currentSpeaker = currentSpeaker;
            this.npcAnimation = npcAnimation;
            this.text = text;
    }
}
