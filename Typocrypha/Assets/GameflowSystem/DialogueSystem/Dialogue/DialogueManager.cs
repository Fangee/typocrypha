using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages dialogue boxes
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager main = null; // Global static reference
    public DialogUI UIManager;

    public float scroll_time; // Time it takes to automatically update window
    public float scroll_delay = 0.01f; // Time it takes for text to scroll 
    public float scroll_scale; // Scroll delay multiplier
    public float top_space; // Space between top of window and dialogue

    [HideInInspector] public bool pause_scroll; // Pause text scroll
    [HideInInspector] public bool block_input; // Blocks user input
    [HideInInspector] public bool is_dump; // Is text being dumped?
    [HideInInspector] public string answer; // Player's input

    float window_height; // Height of dialogue history
    float default_window_height; // Default Height of dialogue history (to reset)
    Coroutine slide_scroll_cr; // Coroutine that smoothly adjusts window
    Coroutine dump_cr; // Coroutine that dumps text
    List<DialogueBox> history; // List of all dialogue boxes
    string stringEdit = "";

    bool input; // Are we waiting for input?

    private bool isInterrupt = false; //is this dialogue event a oneshot (interrupts, etc)

    void Awake()
    {
        if (main == null) main = this;
        else Destroy(this);
        pause_scroll = false;
        block_input = false;
        window_height = top_space;
        default_window_height = top_space;
        history = new List<DialogueBox>();
        input = false;
        scroll_scale = 1f;
    }

    void Update()
    {
        #region pause and spacebar stuff (TO BE MOVED OR REWORKED)
        //if (Pause.main.isPaused())
        //{
        //    if (input_field.enabled)
        //    {
        //        input_field.enabled = false;
        //    }
        //    return;
        //}
        //input_field.enabled = true;
        //if (history.Count > 0 && history[history.Count - 1].cr_scroll == null && !input)
        //{
        //    curr_spacebar_icon.SetActive(true);
        //}
        //if (block_input)
        //{
        //    curr_spacebar_icon.SetActive(true);
        //    if (Input.GetKeyDown(KeyCode.Space)) AudioPlayer.main.playSFX("sfx_botch");
        //    if (curr_spacebar_icon.activeInHierarchy)
        //        curr_spacebar_animator.Play("anim_key_spacebar_no");
        //    return;
        //}
        //else
        //{
        //    if (history.Count > 0 && history[history.Count - 1].cr_scroll != null)
        //    {
        //        curr_spacebar_icon.SetActive(false);
        //    }
        //    if (curr_spacebar_icon.activeInHierarchy)
        //    {
        //        curr_spacebar_animator.Play("anim_key_spacebar");
        //    }
        //}
        //if (isInterrupt)
        //    player_ui.localScale = new Vector3(0, 0, 0);
        #endregion

        if (is_dump)
            return;
        if (Input.GetKeyDown(KeyCode.Space) && !input)
        {
            if (history.Count > 0 && history[history.Count - 1].cr_scroll != null)
            {
                is_dump = true; // Flag is set back to false by 'dumpText()'
                dump_cr = StartCoroutine(history[history.Count - 1].dumpText());
            }
            else
                finishDialogue();
        }
    }

    public void setEnabled(bool e)
    {
        enabled = e;
        UIManager.setEnabled(e);
    }

    // Starts a new dialogue scene
    public void startDialogue(DialogueItem lineData)
    {
        setEnabled(true);

        DialogueBox d_box = null;
        //else if (curr_dialogue.GetType() == typeof(DialogueItemChat)) {
        //    //clearLog (ChatView);
        //    GameObject d_obj = Instantiate(dialogue_box_prefab, ChatContent);
        //    d_box = d_obj.GetComponent<DialogueBox>();
        //} else if (curr_dialogue.GetType() == typeof(DialogueItemAN)) {
        //    //clearLog (ANView);
        //    GameObject d_obj = Instantiate(an_dialouge_box_prefab, ANContent);
        //    d_box = d_obj.GetComponent<DialogueBox>();
        //}
        d_box.d_item = lineData;
        d_box.speaker = DialogueParser.main.substituteMacros(lineData.speaker_name);
        // Remove old text effects
        FXTextEffect[] fx_arr = d_box.fx_text.gameObject.GetComponents<FXTextEffect>();
        foreach (FXTextEffect fx in fx_arr)
        {
            d_box.fx_text.removeEffect(fx);
            Destroy(fx);
        }
        d_box.text = DialogueParser.main.parse(lineData, d_box);
        // Add new text effects
        foreach (FXTextEffect text_effect in lineData.fx_text_effects)
            d_box.fx_text.addEffect(text_effect);
        // Add dialogue box to history (only really works for Chat items)
        history.Add(d_box);
        // Start scroll
        d_box.scroll_delay = scroll_delay;
        Coroutine d_box_init = d_box.dialogueBoxStart(lineData);
        //Prompt input if necessary
    }

    // Starts a new dialogue scene
    public void startInterrupt(GameObject new_dialogue_interrupt)
    {
        isInterrupt = true;
        //startDialogue(new_dialogue_interrupt);
    }
    //Ends a non-interrupt dialogue scene
    public void finishDialogue()
    {
        if (isInterrupt)
        {
            if (!BattleManagerS.main.playSceneFromQueue())
            {
                //player_ui.localScale = new Vector3(1, 1, 1);
                BattleManagerS.main.setPause(false);
                BattleManagerS.main.postInterrupt();
                isInterrupt = false;
                setEnabled(false);
            }
            else
                return;
        }
        TypocryphaGameflow.GameflowManager.main.next();
    }
 

    // Forces next line
    public void forceNextLine()
    {
        history[history.Count - 1].StopAllCoroutines();
        history[history.Count - 1].cr_scroll = null;
        //if (!nextLine() && !isInterrupt)
            finishDialogue();
    }

    // Changes scroll delay of currently scrolling dialogue
    public void setScrollDelay(float delay)
    {
        history[history.Count - 1].scroll_delay = delay;
    }

    #region chat window controls
    //// Expands height of chat window
    //public void expandWindow(float box_height)
    //{
    //    float expand = box_height + ChatContent.GetComponent<VerticalLayoutGroup>().spacing;
    //    window_height += expand;
    //    ChatContent.sizeDelta = new Vector2(ChatContent.sizeDelta.x, window_height);
    //    stopSlideScroll();
    //    slide_scroll_cr = StartCoroutine(slideScrollCR());
    //}

    //// Resets height of chat window
    //public void resetWindowSize()
    //{
    //    window_height = default_window_height;
    //    ChatContent.sizeDelta = new Vector2(ChatContent.sizeDelta.x, window_height);
    //    stopSlideScroll();
    //}

    //// Stops slide adjustment of window
    //public void stopSlideScroll()
    //{
    //    if (slide_scroll_cr != null) StopCoroutine(slide_scroll_cr);
    //}

    //// Coroutine that smoothly slides scroll bar to bottom
    //IEnumerator slideScrollCR()
    //{
    //    yield return new WaitUntil(() => scroll_bar.value > Mathf.Epsilon);
    //    float vel = 0;
    //    while (scroll_bar.value > Mathf.Epsilon)
    //    {
    //        scroll_bar.value = Mathf.SmoothDamp(scroll_bar.value, 0, ref vel, scroll_time);
    //        yield return null;
    //    }
    //}

    //// Clears the given chat log of all objects in its hierarchy (general function)
    //public void clearLog(GameObject textView, int offset)
    //{
    //    Transform content = textView.transform.GetChild(0).GetChild(0);
    //    int skipSpacer = 0; // skip the Spacer object in the content hierarchy
    //                        //VerticalLayoutGroup layout = content.GetComponents<VerticalLayoutGroup>();
    //    Transform[] contentArray = content.GetComponentsInChildren<Transform>();
    //    int skipCurrentBox = 0; // tracks when to stop going through log as to not delete current textbox
    //    int currentBoxPosition = contentArray.Length - (offset * 2); // when to stop deleting items
    //                                                                 //Debug.Log ("current box position = " + currentBoxPosition);
    //    foreach (Transform tr in content.transform)
    //    {
    //        if (skipSpacer > 0 && skipCurrentBox < currentBoxPosition)
    //        {
    //            Destroy(tr.gameObject);
    //            skipCurrentBox += offset;
    //        }
    //        else
    //        {
    //            ++skipSpacer;
    //            ++skipCurrentBox;
    //        }
    //        //Debug.Log ("skip current box = " + skipCurrentBox);
    //    }
    //    resetWindowSize();
    //}

    //// Pops off the latest item in the given chat log of all objects in its hierarchy (general function)
    //// BUGGED
    //public void popLog(GameObject textView, int offset)
    //{
    //    //Debug.Log ("popping log");
    //    resetWindowSize();
    //    Transform content = textView.transform.GetChild(0).GetChild(0);
    //    int skipSpacer = 0; // skip the Spacer object in the content hierarchy
    //                        //VerticalLayoutGroup layout = content.GetComponents<VerticalLayoutGroup>();
    //    Transform[] contentArray = content.GetComponentsInChildren<Transform>();
    //    int skipCurrentBox = 0; // tracks when to stop going through log as to not delete current textbox
    //    int minBoxPosition = contentArray.Length - (offset * 3) - 1; // when to stop deleting items
    //    int maxBoxPosition = contentArray.Length - (offset * 2); // when to stop deleting items
    //                                                             //Debug.Log ("current box position = " + currentBoxPosition);
    //    foreach (Transform tr in content.transform)
    //    {
    //        if ((skipSpacer > 0) || ((minBoxPosition < skipCurrentBox) && (skipCurrentBox < maxBoxPosition)))
    //        {
    //            Destroy(tr.gameObject);
    //            skipCurrentBox += offset;
    //        }
    //        else
    //        {
    //            ++skipSpacer;
    //            ++skipCurrentBox;
    //        }
    //        //Debug.Log ("skip current box = " + skipCurrentBox);
    //    }
    //}

    //// Clears the Chat view log
    //public void clearChatLog()
    //{
    //    clearLog(ChatView, 4); // A chatview dialogue box has 4 items (the parent and 3 children (text, L/R icons))
    //}

    //// Clears the AN view log
    //public void clearANLog()
    //{
    //    clearLog(ANView, 2); // An AN view dialogue box has 2 items (the parent and 1 child (text))
    //}
    #endregion
}
