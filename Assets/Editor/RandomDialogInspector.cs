using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEditor
{
    [CustomEditor(typeof(RandomDialog))]
    public class RandomDialogInspector : Editor
    {
        private List<bool> expandSentences;
        public List<string> convoLabels;

        public void Reset() {
            RandomDialog dialog = target as RandomDialog;

            // -- These fields don't serialize properly, so they get overwritten on play mode
            // Initialize them manually
            if (expandSentences == null) {
                expandSentences = new List<bool>();
                foreach (Conversation c in dialog.conversations) {
                    expandSentences.Add(false);
                }
            }
            if (convoLabels == null) {
                convoLabels = new List<string>();
                for (int i = 0; i < dialog.conversations.Count; i++) {
                    // -- Populate the conversation TL;DR if possible
                    if (dialog.conversations[i].sentences[0] != null &&
                        dialog.conversations[i].sentences[0].text != "") {
                        convoLabels.Add(dialog.conversations[i].sentences[0].text);
                    } else {
                        convoLabels.Add("Conversation " + i);
                    }
                }
            }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            Reset();
            EditorGUILayout.LabelField("Conversations", EditorStyles.boldLabel);

            RandomDialog dialog = target as RandomDialog;
            for (int i = 0; i < dialog.conversations.Count; i++) {
                AddSentenceEditor(i);

                if (expandSentences[i]) {
                    EditorGUILayout.Space();
                }
            }

            if (GUILayout.Button("Add Conversation")) {
                Conversation newConvo = new Conversation();
                newConvo.sentences.Add(new Sentence());
                dialog.conversations.Add(newConvo);

                expandSentences.Add(true);
                convoLabels.Add("New Conversation");
            }
        }

        private void AddSentenceEditor(int i) {
            RandomDialog dialog = target as RandomDialog;

            Sentence sentenceToDelete;
            expandSentences[i] = EditorGUILayout.Foldout(expandSentences[i], convoLabels[i]);
            EditorGUI.indentLevel += 1;

            if (expandSentences[i]) {
                sentenceToDelete = null;

                // -- Editor-only field
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Convo topic:", GUILayout.MaxWidth(100));
                convoLabels[i] = EditorGUILayout.TextField(convoLabels[i]);
                EditorGUILayout.EndHorizontal();

                foreach (Sentence s in dialog.conversations[i].sentences) {
                    EditorGUI.BeginChangeCheck();
                    if (s == null) break;

                    // -- Horizontal section
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Speaker:", GUILayout.MaxWidth(70));
                    s.currentSpeaker = (DialogSpeaker)EditorGUILayout.EnumPopup(s.currentSpeaker);

                    if (s.currentSpeaker != DialogSpeaker.Me) {
                        EditorGUILayout.LabelField("Lookat:", GUILayout.MaxWidth(70));
                        s.npcAnimation = (NPCAnimation)EditorGUILayout.EnumPopup(s.npcAnimation);
                    }

                    //EditorGUILayout.LabelField("Animation:", GUILayout.MaxWidth(70));
                    //s.animationState = (AnimationState)EditorGUILayout.EnumPopup(s.animationState);

                    if (GUILayout.Button("-")) {
                        // Can't remove an element in the middle of a foreach loop
                        sentenceToDelete = s;
                        Undo.RecordObject(target, "Deleted Sentence");
                        EditorUtility.SetDirty(dialog);
                    }
                    EditorGUILayout.EndHorizontal();

                    // -- Text field
                    s.text = EditorGUILayout.TextField(s.text);

                    if(EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(dialog, "sentence change");
                        EditorUtility.SetDirty(dialog);
                    }
                }

                if (sentenceToDelete != null) {
                    dialog.conversations[i].sentences.Remove(sentenceToDelete);
                }

                // -- Add sentence / remove convo
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Sentence")) {
                    dialog.conversations[i].sentences.Add(new Sentence());
                    Undo.RecordObject(target, "Added Sentence");
                    EditorUtility.SetDirty(dialog);
                }

                if (GUILayout.Button("Delete conversation")) {
                    dialog.conversations.RemoveAt(i);
                    expandSentences.RemoveAt(i);
                    convoLabels.RemoveAt(i);
                    Undo.RecordObject(target, "Deleted Conversation");
                    EditorUtility.SetDirty(dialog);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel -= 1;
        }
    }
}
