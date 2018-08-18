using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogViewChat : DialogView
{
    public GameObject dialogBoxPrefab; // Prefab for dialogue box object
    public RectTransform ChatContent; // Content of chat scroll view (contains dialogue boxes)
    public Scrollbar scrollBar; // Scroll bar of chat dialogue window
    public GameObject spacebar_icon_chat; // Spacebar icon CHAT view
    public Animator animator_spacebar_chat; // Spacebar icon key animator
    public float defaultWindowHeight = 16f; // Default chat window height
    public float scrollBarTime = 0.15f; // Time it takes to automatically update window

    float windowHeight; // Height of chat content window
    Coroutine slideScrollCR; // Coroutine for smoothly sliding scroll bar
    
    void Awake()
    {
        windowHeight = defaultWindowHeight;
    }

    public override DialogBox newDialog(DialogItem data)
    {
        #region Check Arguments
        DialogItemChat item = data as DialogItemChat;
        if (item == null)
            throw new System.Exception("Incorrect Type of dialog Item for the Chat view mode (requires DialogItemChat)");
        #endregion

        #region Instantiate and initialize new Dialog box
        GameObject obj = GameObject.Instantiate(dialogBoxPrefab, ChatContent);
        Image leftIcon = obj.transform.Find("DialogueLeftIcon").GetComponent<Image>();
        Image rightIcon = obj.transform.Find("DialogueRightIcon").GetComponent<Image>();
        if (item.iconSide == IconSide.LEFT || item.iconSide == IconSide.BOTH)
        {
            leftIcon.sprite = item.leftIcon;
            leftIcon.enabled = true;
        }
        if (item.iconSide == IconSide.RIGHT || item.iconSide == IconSide.BOTH)
        {
            rightIcon.sprite = item.rightIcon;
            rightIcon.enabled = true;
        }
        DialogBox dialogBox = obj.GetComponent<DialogBox>();
        #endregion

        dialogBox.dialogueBoxStart(item);
        setWindowSize(dialogBox.setBoxHeight() + ChatContent.GetComponent<VerticalLayoutGroup>().spacing);
        return dialogBox;
    }

    public override void setEnabled(bool e)
    {
        gameObject.SetActive(e);
        if (!e)
        {
            clearLog();
        }
    }

    // Remove all chat messages
    public void clearLog()
    {
        bool skipSpacer = true; // Skip initial spacer object
        foreach (Transform child in ChatContent)
        {
            if (skipSpacer)
            {
                skipSpacer = false;
                continue;
            }
            Destroy(child.gameObject);
        }
        resetWindowSize();
    }

    #region chat window height management
    // Increases window size to fit new dialogue box
    void setWindowSize(float boxHeight)
    {
        windowHeight += boxHeight;
        ChatContent.sizeDelta = new Vector2(ChatContent.sizeDelta.x, windowHeight);
        stopSlideScroll();
        slideScrollCR = StartCoroutine(slideScroll());
    }
    
    // Resets height of chat window
    void resetWindowSize()
    {
        windowHeight = defaultWindowHeight;
        ChatContent.sizeDelta = new Vector2(ChatContent.sizeDelta.x, windowHeight);
        stopSlideScroll();
    }

    // Stops slide adjustment of window
    void stopSlideScroll()
    {
        if (slideScrollCR != null) StopCoroutine(slideScrollCR);
    }

    // Coroutine that smoothly slides scroll bar to bottom
    IEnumerator slideScroll()
    {
        yield return new WaitUntil(() => scrollBar.value > Mathf.Epsilon);
        float vel = 0;
        while (scrollBar.value > Mathf.Epsilon)
        {
            scrollBar.value = Mathf.SmoothDamp(scrollBar.value, 0, ref vel, scrollBarTime);
            yield return null;
        }
    }
    #endregion
}
