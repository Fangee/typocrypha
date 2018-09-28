using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB2
{
    // Testing script for testing events
    public class TestInput : MonoBehaviour
    {
        public InputField input;
        public Text outputText;
        public Image outputImage;
        public Actor testActor;
        public List<string> stateNames;
        Dictionary<int, string> stateNameMap = new Dictionary<int, string>();

        void Awake()
        {
            foreach (string stateName in stateNames)
                stateNameMap.Add(Animator.StringToHash(stateName), stateName);
        }

        void Update()
        {
            if (stateNameMap.ContainsKey(testActor.currStateHash))
            {
                outputText.text = stateNameMap[testActor.currStateHash];
            }
            if (testActor.pause == true)
            {
                outputImage.color = Color.red;
            }
            else
            {
                outputImage.color = Color.green;
            }
        }

        public void checkInput()
        {
            testActor.sendEvent(input.text);
        }
    }
}

