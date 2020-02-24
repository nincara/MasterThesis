// Copyright (c) Corinna Braun.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Azure.SpatialAnchors.Unity
{
    public class UIHandler : MonoBehaviour
    {
        public Button placingButton, localizeButton;
        public Button finishPlacing, finishCollecting;
        public Button saveIdLocalize, finishedLocalize;
        public GameObject toggleCanvas, toggleIdInput;
        public Text attentionText;

        public InputField nameInput, idInput, infoInput, idInputLocalize, testPhaseInput;
        // Start is called before the first frame update
        void Start()
        {
            // Main Buttons
            placingButton = GameObject.Find("ButtonPlacing").GetComponent<Button>();
            localizeButton = GameObject.Find("ButtonLocalize").GetComponent<Button>();

            // Placing Buttons
            finishPlacing = GameObject.Find("FinishedPlacing").GetComponent<Button>();
            finishCollecting = GameObject.Find("FinishedCollecting").GetComponent<Button>();

            //Input Fields
            nameInput = GameObject.Find("NameInput").GetComponent<InputField>();
            idInput = GameObject.Find("IdInput").GetComponent<InputField>();
            infoInput = GameObject.Find("InfoInput").GetComponent<InputField>();
            attentionText = GameObject.Find("AttentionText").GetComponent<Text>();

            // Localizing Buttons
            saveIdLocalize = GameObject.Find("SaveId").GetComponent<Button>();
            finishedLocalize = GameObject.Find("FinishedLocalize").GetComponent<Button>();

            ////// ID to localize input at the beginning
            idInputLocalize = GameObject.Find("IdInputLocalize").GetComponent<InputField>();
            testPhaseInput = GameObject.Find("TestPhaseInput").GetComponent<InputField>();
            toggleIdInput = GameObject.Find("IdInputCanvas");

            // Input Output Canvas
            toggleCanvas = GameObject.Find("InputCanvas");

            // Cant find GameObject, if parent is inactive! 
            finishPlacing.gameObject.SetActive(false);
            finishCollecting.gameObject.SetActive(false);
            saveIdLocalize.gameObject.SetActive(false);
            finishedLocalize.gameObject.SetActive(false);
            toggleIdInput.SetActive(false);
            toggleCanvas.SetActive(false);
        }

        public void ToggleInputCanvas()
        {
            if (toggleCanvas.activeSelf)
            {
                toggleCanvas.SetActive(false);
            }
            else
            {
                toggleCanvas.SetActive(true);
            }
        }
    }
}