using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{

    public enum DialogViewType
    {
        VN,
        AN,
        Chat,
    }

    private DialogViewType currentMode;
    private Dictionary<DialogViewType, GameObject> viewObjects;

    #region VN UI
    public GameObject VNView; // Visual Novel view hiearchy
    public DialogueBox VNDialogueBox; // Visual novel style dialogue box
    public FXText VNSpeaker; // Text that contains speaker's name for VN style
    public SpriteRenderer VNMCSprite; // Holds mc's sprite
    public SpriteRenderer VNCodecSprite; // Holds codec call sprites (right side)
    public GameObject spacebar_icon_vn; // Spacebar icon VN view
    public Animator animator_spacebar_vn; // Spacebar icon key animator
    #endregion

    #region AN UI
    public GameObject ANView; // Audio Novel view hiearchy
    public RectTransform ANContent; // Content of scroll view
    public GameObject spacebar_icon_an; // Spacebar icon AN view
    public Animator animator_spacebar_an; // Spacebar icon key animator
    #endregion

    #region Chat UI
    public GameObject ChatView; // Chat view hiearchy
    public RectTransform ChatContent; // Content of chat scroll view (contains dialogue boxes)
    public Scrollbar scroll_bar; // Scroll bar of chat dialogue window
    public GameObject spacebar_icon_chat; // Spacebar icon CHAT view
    public Animator animator_spacebar_chat; // Spacebar icon key animator
    #endregion

    public RectTransform player_ui; // the Typocrypha UI

    public InputField input_field; // Input field for dialogue choices
    public GameObject input_display_choices; // Game object displaying dialogue choices
    public GameObject input_display_C; // Dialogue choice C box
    GameObject input_display; // Display image for input

    public GameObject dialogue_box_prefab; // Prefab of dialogue box object
    public GameObject an_dialouge_box_prefab; // Prefab of audio novel dialogue 

    private GameObject curr_spacebar_icon; // Spacebar icon VN view ref
    private Animator curr_spacebar_animator; // Spacebar icon key animator ref

    // Use this for initialization
    void Start()
    {
        viewObjects = new Dictionary<DialogViewType, GameObject>{
            { DialogViewType.VN, VNView },
            { DialogViewType.AN,  ANView },
            { DialogViewType.Chat, ChatView },
        };
        //Initialize in AN mode
        currentMode = DialogViewType.AN;
        curr_spacebar_icon = spacebar_icon_an;
        curr_spacebar_animator = animator_spacebar_an;
    }

    public void setEnabled(bool e)
    {
        ChatView.SetActive(e);
        VNView.SetActive(e);
        ANView.SetActive(e);
    }

    public void setViewMode(DialogViewType mode)
    {
        if (mode == currentMode)
            return;
        viewObjects[currentMode].SetActive(false);
        viewObjects[mode].SetActive(true);
        switch (mode)
        {
            case DialogViewType.VN:
                curr_spacebar_icon = spacebar_icon_vn;
                curr_spacebar_animator = animator_spacebar_vn;
                break;
            case DialogViewType.AN:
                curr_spacebar_icon = spacebar_icon_an;
                curr_spacebar_animator = animator_spacebar_an;
                break;
            case DialogViewType.Chat:
                curr_spacebar_icon = spacebar_icon_chat;
                curr_spacebar_animator = animator_spacebar_chat;
                break;
        }
        currentMode = mode;
    }

    #region InputDisplay
    //Show input options when text is done scrolling
    IEnumerator showInput(DialogueItem d_item, DialogueBox d_box, Coroutine d_box_init)
    {
        yield return d_box_init; // Wait for d_box to finish setting up
        yield return new WaitUntil(() => d_box.cr_scroll == null); // Wait for scroll to end
        input_field.gameObject.SetActive(true);
        input_field.ActivateInputField();
        //spacebar_icon_vn.SetActive(true);
        curr_spacebar_animator.Play("anim_key_spacebar_no");
        //if (d_item.input_display != null)
        //    input_display = Instantiate(d_item.input_display, transform);
        //if (d_item.input_options.Length > 0)
        //{
        //    switch (d_item.input_options.Length)
        //    {
        //        case 2:
        //            input_display_C.SetActive(false);
        //            break;
        //        case 3:
        //            input_display_C.SetActive(true);
        //            break;
        //    }
        //    populateChoices();
        //    input_display_choices.SetActive(true);
        //}
    }

    // Fill the choice display boxes with current options text
    void populateChoices()
    {
        //Text[] choiceText = input_display_choices.GetComponentsInChildren<Text>();
        //DialogueItem d_item = curr_dialogue.GetComponents<DialogueItem>()[curr_line];
        //int j = 0;
        //for (int i = 1; (i < choiceText.Length) && (j < d_item.input_options.Length); i = i + 2)
        //{
        //    choiceText[i].text = d_item.input_options[j];
        //    ++j;
        //}
    }

    //Called when input field is submitted
    public void submitInput()
    {
        string answer = input_field.text;
        if (Input.GetKey(KeyCode.Escape) || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            input_field.ActivateInputField();
            return;
        }
        // CHECK IF CORRECT INPUT
        input_field.gameObject.SetActive(false);
        input_field.text = "";
        Destroy(input_display);
        input_display_choices.SetActive(false);
        //input = false;
        PlayerDataManager.main.setData(PlayerDataManager.defaultPromptKey, answer);
        //PlayerDataManager.main.setData((curr_dialogue as TypocryphaGameflow.DialogNodeInput).variableName, answer);
        //forceNextLine();
    }
    #endregion
}
