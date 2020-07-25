using System.Collections.Generic;

// Unity can't serialize lists of lists, so I have to create a wrapper class.
[System.Serializable]
public class Conversation {
    public List<Sentence> sentences;

    public Conversation() {
        sentences = new List<Sentence>();
    }
}
