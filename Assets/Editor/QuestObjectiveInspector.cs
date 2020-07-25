using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEditor {
    [CustomEditor(typeof(QuestObjective))]
    public class QuestObjectiveInspector : Editor {
        private List<bool> expandSentences;
        public List<string> convoLabels;

        public void Reset() {
            if (expandSentences == null) {
                expandSentences = new List<bool>();
                expandSentences.Add(false);
                expandSentences.Add(false);
                expandSentences.Add(false);
            }
        }

        public override void OnInspectorGUI() {
            // Could remove this, but having the rest of the serialized vars in the inspector automatically is nice
            base.OnInspectorGUI();
            Reset();

            QuestObjective objective = target as QuestObjective;

            EditorGUILayout.Space();

            AddSentenceEditor(0, "Initial Dialog", objective.initialDialog);
            AddSentenceEditor(1, "Repeat Dialog", objective.repeatDialog);
            AddSentenceEditor(2, "Final Dialog", objective.onCompleteDialog);
        }

        private void AddSentenceEditor(int i, string label, Conversation convo) {
            Sentence sentenceToDelete;
            expandSentences[i] = EditorGUILayout.Foldout(expandSentences[i], label);
            EditorGUI.indentLevel += 1;

            if (expandSentences[i]) {
                sentenceToDelete = null;

                foreach (Sentence s in convo.sentences) {
                    if (s == null) break;
                    EditorGUI.BeginChangeCheck();

                    // -- Horizontal section
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Speaker:", GUILayout.MaxWidth(70));
                    s.currentSpeaker = (DialogSpeaker)EditorGUILayout.EnumPopup(s.currentSpeaker);

                    if (s.currentSpeaker != DialogSpeaker.Me) {
                        EditorGUILayout.LabelField("Lookat:", GUILayout.MaxWidth(70));
                        s.lookatTarget = (LookatTargets)EditorGUILayout.EnumPopup(s.lookatTarget);
                    }

                    //EditorGUILayout.LabelField("Animation:", GUILayout.MaxWidth(70));
                    //s.animationState = (AnimationState)EditorGUILayout.EnumPopup(s.animationState);

                    if (GUILayout.Button("-")) {
                        // Can't remove an element in the middle of a foreach loop
                        sentenceToDelete = s;
                        Undo.RecordObject(target, "Remove Sentence");
                        EditorUtility.SetDirty(target);
                    }
                    EditorGUILayout.EndHorizontal();

                    // -- Text field
                    s.text = EditorGUILayout.TextField(s.text);

                    if(EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(target, "sentence change");
                        EditorUtility.SetDirty(target);
                    }
                }

                if (sentenceToDelete != null) {
                    convo.sentences.Remove(sentenceToDelete);
                }

                // -- Add sentence / remove convo
                if (GUILayout.Button("Add Sentence")) {
                    convo.sentences.Add(new Sentence());
                    Undo.RecordObject(target, "Add Sentence");
                    EditorUtility.SetDirty(target);
                }

            }

            EditorGUI.indentLevel -= 1;
        }
    }
}
