using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Maintain a rolling history of sentences, as they come in
public class ConversationLog : MonoBehaviour {
    public static ConversationLog instance;
    public TextMeshProUGUI textUI;

    // Current settings: 19,35
    public int totalAvailableLines, totalCharsPerLine;

    public Sentence[] conversationHistory;
    private int lastSentence = 0;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    private void Start() {
        conversationHistory = new Sentence[totalAvailableLines];
        textUI.text = "";
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.D)) {
            Sentence s = new Sentence();
            s.text = Time.time.ToString();
            AddSentence(s);
        }
    }

    public void AddSentence(Sentence sentence) {
        conversationHistory[lastSentence] = sentence;
        lastSentence = (lastSentence + 1) % totalAvailableLines;

        UpdateText();
    }

    private void UpdateText() {
        textUI.text = "";
        for (int i = 0; i < conversationHistory.Length; i++) {
            int index = (lastSentence + i) % totalAvailableLines;

            if (conversationHistory[index] != null && conversationHistory[index].text != null) {
                string thisDialog = conversationHistory[index].currentSpeaker.ToString() + ": " + conversationHistory[index].text + "\n";
                textUI.text += thisDialog;
            }
        }
    }
}
