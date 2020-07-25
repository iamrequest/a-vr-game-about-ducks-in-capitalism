using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sentence {
    public AnimationState animationState;
    public DialogSpeaker currentSpeaker;
    public string text;

    public Sentence() { }
    public Sentence(AnimationState animationState, 
        DialogSpeaker currentSpeaker, 
        string text) {

            this.animationState = animationState;
            this.currentSpeaker = currentSpeaker;
            this.text = text;
    }
}
