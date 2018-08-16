using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TypocryphaGameflow
{
    //New Manager for Dialog scenes
    public class DialogManager : MonoBehaviour
    {
        public enum DialogViewType
        {
            VN,
            AN,
            Chat,
        }

        private DialogViewType currentMode;
        private Dictionary<DialogViewType, DialogView> viewModes;

        #region View Modes
        public DialogViewVN VNView; // Visual Novel view object
        public DialogViewAN ANView; // Audio Novel view object
        public DialogViewChat ChatView; // Chat view object
        #endregion

        #region Input UI and variables
        public InputField input_field; // Input field for dialogue choices
        public GameObject input_display_choices; // Game object displaying dialogue choices
        private GameObject[] individual_choices;
        private bool isInput = false; // is currently polling for input?
        private string variableToSaveTo = string.Empty;
        #endregion

        private DialogBox curr_dialog_box; // Current active dialog box
        private Ref<bool> isNotDump = new Ref<bool>(true);
        private GameObject curr_spacebar_icon; // Spacebar icon VN view ref
        private Animator curr_spacebar_animator; // Spacebar icon key animator ref

        #region Initialization, Enabling, and Updates
        void Awake()
        {
            //Initialize the viewmode dictionary
            viewModes = new Dictionary<DialogViewType, DialogView>{
            { DialogViewType.VN, VNView },
            { DialogViewType.AN,  ANView },
            { DialogViewType.Chat, ChatView },
        };
            //Get the individual choice prompts from the parent gameobject
            individual_choices = input_display_choices.GetChildren().ToArray();
            //Initialize in AN mode
            currentMode = DialogViewType.AN;
            setEnabled(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isNotDump.Obj && !isInput)
            {
                if (curr_dialog_box.cr_scroll != null)
                {
                    isNotDump.Obj = false;
                    StartCoroutine(curr_dialog_box.dumpText(isNotDump));
                }
                else
                    finishDialogLine();
            }
        }

        public void setEnabled(bool e)
        {
            enabled = e;
        }
        #endregion

        #region Dialog Mode/Line Handling
        public void setViewMode(DialogViewType mode)
        {
            if (mode == currentMode)
                return;
            viewModes[currentMode].setEnabled(false);
            viewModes[mode].setEnabled(true);
            currentMode = mode;
        }

        public void startDialogLine(DialogViewType mode, DialogItem lineData)
        {
            setViewMode(mode);
            curr_dialog_box = viewModes[currentMode].newDialog(lineData);
        }

        private void finishDialogLine()
        {
            TypocryphaGameflow.GameflowManager.main.next();
        }
        #endregion

        #region InputDisplay
        //Exposed function to call an input prompt from
        public void showInputPrompt(string[] prompts = null, string saveInputToVariable = "")
        {
            variableToSaveTo = saveInputToVariable;
            StartCoroutine(showInput(prompts));
        }

        //Show input options when text is done scrolling
        IEnumerator showInput(string[] prompts)
        {
            yield return new WaitUntil(() => curr_dialog_box.cr_init == null); // Wait for d_box to finish setting up
            yield return new WaitUntil(() => curr_dialog_box.cr_scroll == null); // Wait for scroll to end
            isInput = true;
            input_field.gameObject.SetActive(true);
            input_field.ActivateInputField();
            //spacebar_icon_vn.SetActive(true);
            //curr_spacebar_animator.Play("anim_key_spacebar_no");
            //if (d_item.input_display != null)
            //    input_display = Instantiate(d_item.input_display, transform);
            if (prompts != null && prompts.Length > 0)
            {
                populateChoices(prompts);
                input_display_choices.SetActive(true);
            }
        }

        // Fill the choice display boxes with current options text
        void populateChoices(string[] prompts)
        {
            Text[] choiceText = input_display_choices.GetComponentsInChildren<Text>();
            for (int i = 0; i < prompts.Length; ++i)
            {
                if (string.IsNullOrEmpty(prompts[i]))
                    individual_choices[i].SetActive(false);
                else
                {
                    individual_choices[i].SetActive(true);
                    choiceText[(i * 2) + 1].text = prompts[i];
                }
            }
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
            input_field.gameObject.SetActive(false);
            input_display_choices.SetActive(false);
            input_field.text = "";
            //Destroy(input_display);
            isInput = false;
            PlayerDataManager.main.setData(PlayerDataManager.defaultPromptKey, answer);
            if (!string.IsNullOrEmpty(variableToSaveTo))
            {
                PlayerDataManager.main.setData(variableToSaveTo, answer);
                variableToSaveTo = string.Empty;
            }
            finishDialogLine();
        }
        #endregion
    }
}
