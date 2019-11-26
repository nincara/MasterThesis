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
        public Button saveIdLocalize, showData;
        public GameObject toggleCanvas, toggleOutput, toggleIdInput;
        public Text nameOutput, idOutput, dateOutput, secondsOutput, infoOutput, progressOutput, keyOutput;
        public Text positionOutput, rotationOutput;
        public InputField nameInput, idInput, infoInput, idInputLocalize;
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

            // Localizing Buttons
            saveIdLocalize = GameObject.Find("SaveId").GetComponent<Button>();
            showData = GameObject.Find("ShowData").GetComponent<Button>();

            ////// ID to localize input at the beginning
            idInputLocalize = GameObject.Find("IdInputLocalize").GetComponent<InputField>();
            toggleIdInput = GameObject.Find("IdInputCanvas");

            // Input Output Canvas
            toggleCanvas = GameObject.Find("InputCanvas");
            toggleOutput = GameObject.Find("OutputCanvas");

            //Text Output
            nameOutput = GameObject.Find("NameOutput").GetComponent<Text>();
            idOutput = GameObject.Find("IDOutput").GetComponent<Text>();
            dateOutput = GameObject.Find("DateOutput").GetComponent<Text>();
            secondsOutput = GameObject.Find("SecondsOutput").GetComponent<Text>();
            infoOutput = GameObject.Find("InfoOutput").GetComponent<Text>();
            progressOutput = GameObject.Find("ProgressOutput").GetComponent<Text>();
            keyOutput = GameObject.Find("KeyOutput").GetComponent<Text>();
            positionOutput = GameObject.Find("PositionOutput").GetComponent<Text>();
            rotationOutput = GameObject.Find("RotationOutput").GetComponent<Text>();

            // Cant find GameObject, if parent is inactive! 
            finishPlacing.gameObject.SetActive(false);
            finishCollecting.gameObject.SetActive(false);
            saveIdLocalize.gameObject.SetActive(false);
            showData.gameObject.SetActive(false);
            toggleIdInput.SetActive(false);
            toggleCanvas.SetActive(false);
            toggleOutput.SetActive(false);

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

        public void ToggleOutputCanvas()
        {
            if (toggleOutput.activeSelf)
            {
                toggleOutput.SetActive(false);
            }
            else
            {
                toggleOutput.SetActive(true);
            }
        }
    }
}