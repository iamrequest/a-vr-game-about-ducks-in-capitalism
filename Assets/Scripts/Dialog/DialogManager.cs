using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR.InteractionSystem;

public class DialogManager : MonoBehaviour {
    private BaseDialog activeDialog; // A reference to the base dialog, so we can let it know when we finish the convo
    private Queue<Sentence> sentences;
    private bool skipCurrentSentence;
    private Sentence currentSentence;
    private DuckNPC currentNPC;

    public bool isDialogActive { get; private set; }

    [Header("UI Elements")]
    //public CanvasCharacter canvasCharacter;
    public TextMeshProUGUI textUI;
    public SpriteRenderer dialogBG;
    public SpriteRenderer nameboxBG;
    public TextMeshProUGUI nameboxTextUI;
    public Animator animator;

    [Header("Writing dialog to UI")]
    public float dialogSpeed;
    private float timeSinceLastLetterTyped;
    private bool completedCurrentSentence;

    public AudioSource audioSource;
    public AudioClip typingAudio;

    // Singleton
    public static DialogManager instance;
    void Awake() {
        // Configure singleton
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        // -- Configure other things
        sentences = new Queue<Sentence>();
        completedCurrentSentence = true;
    }

    public void StartDialog(BaseDialog dialog, Sentence newSentence, bool clearExistingDialog) {
        activeDialog = dialog;
        animator.SetBool("isDialogBoxOpen", true);
        isDialogActive = true;

        if (clearExistingDialog) {
            sentences.Clear();
            completedCurrentSentence = true;
        }

        sentences.Enqueue(newSentence);

        if (clearExistingDialog) {
            DisplayNextSentence();
        }
    }
    public void StartDialog(BaseDialog dialog, Conversation convo, bool clearExistingDialog) {
        StartDialog(dialog, convo.sentences, clearExistingDialog);
    }
    public void StartDialog(BaseDialog dialog, List<Sentence> newSentences, bool clearExistingDialog) {
        activeDialog = dialog;
        animator.SetBool("isDialogBoxOpen", true);
        isDialogActive = true;

        if (clearExistingDialog) {
            sentences.Clear();
            completedCurrentSentence = true;
        }

        foreach (Sentence s in newSentences) {
            sentences.Enqueue(s);
        }

        if (clearExistingDialog) {
            DisplayNextSentence();
        }
    }

    public void SkipCurrentSentence() {
        skipCurrentSentence = true;
    }
    public void DisplayNextSentence() {
        if (!isDialogActive) return;

        if (!completedCurrentSentence) {
            //skipCurrentSentence = true;
            return;
        }

        if (sentences.Count == 0) {
            EndDialog(true);
            return;
        }

        // Prepare the dialog. 
        // Advance until we have a sentence with text. 
        //  By skipping empty texts, we can create empty sentences, we can update an NPC's lookdir without having them say anything
        Sentence tmpSentence;
        do {
            tmpSentence = sentences.Dequeue();

            // For this sentence, make the NPC look at something
            currentNPC = SpeakerManager.instance.GetNPCSpeaker(tmpSentence.currentSpeaker);
            if (currentNPC != null) {
                // If the speaker is an NPC, look at the specified transform
                currentNPC.lookatTarget = SpeakerManager.instance.GetLookatTarget(tmpSentence.npcAnimation);

                // Set animation
                // This can null out the lookat target, if the animation moves the head (eg: sipping coffee)
                currentNPC.SetAnimation(tmpSentence.npcAnimation);

                // Don't talk if the only dialog is "...", or ""
                if (tmpSentence.text.Trim() == "") {
                    currentNPC.isTalking = false;
                    currentNPC.isShouting = false;
                }
                if (tmpSentence.text.Trim() == "...") {
                    currentNPC.isTalking = false;
                    currentNPC.isShouting = false;
                }
            }
        } while (sentences.Count > 0 && tmpSentence.text.Trim() == "");

        // This solves a bug with the last sentence overwriting the textbox image/name
        if (tmpSentence.text.Trim() == "") {
            if (sentences.Count == 0) {
                EndDialog(true);
            }
            return;
        }

        currentSentence = tmpSentence;
        ConversationLog.instance.AddSentence(currentSentence);

        // Apply the animation state, prepare the textbox
        ConfigureTextboxImages();

        // Start typing
        StartCoroutine(TypeSentence());
    }

    public void EndDialogEarly() {
        sentences.Clear();
        EndDialog(false);
    }
    private void EndDialog(bool wasDialogFullyCompleted) {
        textUI.text = "";
        isDialogActive = false;
        completedCurrentSentence = true;
        animator.SetBool("isDialogBoxOpen", false);

        if (currentNPC != null) {
            currentNPC.isTalking = false;
            currentNPC.isShouting = false;
        }

        if (activeDialog != null) {
            activeDialog.OnDialogEnd(wasDialogFullyCompleted);
        }
    }

    private IEnumerator TypeSentence() {
        textUI.text = "";
        skipCurrentSentence = false;
        timeSinceLastLetterTyped = 0f;
        completedCurrentSentence = false;

        // TODO: Wait for opening animation
        foreach (char letter in currentSentence.text.ToCharArray()) {
            // If the dialog ended early for some reason, stop this typing coroutine
            //  This stops a bug where old text can bleed into a new convo when the player ends dialog early
            //  and then starts a new dialog quickly.
            if (!isDialogActive) {
                if (currentNPC != null) {
                    currentNPC.isTalking = false;
                    currentNPC.isShouting = false;
                }
                yield break;
            }

            // If the player hit the (figurative) B button, skip the dialog to the end.
            if (skipCurrentSentence) {
                // -- Skip through all the audio
                textUI.text = currentSentence.text;
                audioSource.PlayOneShot(typingAudio);
                if (currentNPC != null) {
                    currentNPC.isTalking = false;
                    currentNPC.isShouting = false;
                }

                completedCurrentSentence = true;
                yield break;
            }

            // Wait for the next character to be typed
            do {
                timeSinceLastLetterTyped += Time.deltaTime;
                yield return null;
            } while (dialogSpeed > timeSinceLastLetterTyped);

            // -- Type a single character
            timeSinceLastLetterTyped = 0f;
            textUI.text += letter;
            audioSource.PlayOneShot(typingAudio);

            yield return null;
        }

        if (currentNPC != null) {
            currentNPC.isTalking = false;
            currentNPC.isShouting = false;
        }
        completedCurrentSentence = true;
    }

    private void ConfigureTextboxImages() {
        // -- Set namebox color and text
        nameboxBG.color = SpeakerManager.instance.GetSpeakerColor(currentSentence.currentSpeaker);
        switch (currentSentence.currentSpeaker) {
            case DialogSpeaker.WORK_DUCK_ANON:
            case DialogSpeaker.ART_DUCK_ANON:
                nameboxTextUI.text = "Customer";
                break;
            default:
                nameboxTextUI.text = currentSentence.currentSpeaker.ToString();
                break;
        }

        // -- Set dialog box color and bg image
        dialogBG.color = SpeakerManager.instance.GetSpeakerColor(currentSentence.currentSpeaker);

        if (currentSentence.currentSpeaker == DialogSpeaker.Me) {
            dialogBG.sprite = SpeakerManager.instance.playerTextbox;
        } else {
            dialogBG.sprite = SpeakerManager.instance.npcTextbox;
        }
    }
}
