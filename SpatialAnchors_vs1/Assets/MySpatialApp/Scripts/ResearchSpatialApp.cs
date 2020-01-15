using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Microsoft.Azure.SpatialAnchors.Unity {

    #region App State
    public class ResearchSpatialApp : ResearchAppBase {
        internal enum AppState {
            LoadingKeys = 0,
            Default,
            PlacingAnchor,
            InputAnchorData,
            SavingAnchor,
            InputAnchorDataLocalize,
            LookingForAnchor,
            ShowAnchorData,
            FoundAnchor
        }

        private AppState _currentAppState = AppState.LoadingKeys;
        AppState currentAppState {
            get {
                return _currentAppState;
            }
            set {
                if (_currentAppState != value) {
                    UnityEngine.Debug.LogFormat ("State from {0} to {1}", _currentAppState, value);
                    _currentAppState = value;
                }
            }
        }
        #endregion App State

        #region Variables

#if !UNITY_EDITOR
        public MyAnchorExchanger anchorExchanger = new MyAnchorExchanger ();
#endif
        private List<string> anchorList = new List<string> ();
        private List<CloudSpatialAnchor> _allAnchors = new List<CloudSpatialAnchor> ();
        private string baseSharingUrl = "";
        private string currentAnchorId = "";

        // UI Elements
        public UIHandler uiHandler;
        private string anchorIdToLocate;
        private string testPhase;

        // Messung
        private Stopwatch stopwatchTimer = new Stopwatch ();
        private float elapsedSeconds;
        private int maxFeaturePoints = 0;

        #endregion Variables

        public override void Start () {

            base.Start ();

            if (!SanityCheckAccessConfiguration ()) {
                return;
            }

            speechBubbleText.text = "App is loading...";

            SpatialAnchorSamplesConfig samplesConfig = Resources.Load<SpatialAnchorSamplesConfig> ("SpatialAnchorSamplesConfig");

            if (string.IsNullOrWhiteSpace (BaseSharingUrl) && samplesConfig != null) {
                BaseSharingUrl = samplesConfig.BaseSharingURL;
            }

            if (string.IsNullOrEmpty (BaseSharingUrl)) {
                feedbackBox.text += $"Need to set {nameof(BaseSharingUrl)}.";
                return;
            } else {
                Uri result;
                if (!Uri.TryCreate (BaseSharingUrl, UriKind.Absolute, out result)) {
                    feedbackBox.text = $"{nameof(BaseSharingUrl)} is not a valid url";
                    return;
                } else {
                    BaseSharingUrl = $"{result.Scheme}://{result.Host}/api/anchors";
                }
            }

#if !UNITY_EDITOR
            anchorExchanger.WatchKeys (BaseSharingUrl);
#endif
            switchUiElements ();
            StoreAllAnchorKeys ();

            speechBubbleText.text = "App loaded successfully.";
        }

        public override void Update () {

            if (IsPlacingObject () && CloudManager.FeaturePoints != null) {
                //feedbackBoxExtra.text = $"PointCloud: {CloudManager.FeaturePoints.Count}";
            }

            if (currentAppState == AppState.LookingForAnchor) {
                speechBubbleText.text = $"Progress: {CloudManager.SessionStatus.RecommendedForCreateProgress:0%}. ";
            }

            if (CloudManager.FeaturePoints.Count > maxFeaturePoints && CloudManager.FeaturePoints != null && currentAppState == AppState.LookingForAnchor) {
                maxFeaturePoints = CloudManager.FeaturePoints.Count;
                //feedbackBox.text += "Max Feature Point: " + maxFeaturePoints + ". ";
            }

            base.Update ();
        }

        //////////////////////////////////////////////////////////////////////
        /////////////////// UI Methods //////////////////////////////////////
        //////////////////////////////////////////////////////////////////////

        #region UI Methods

        public void ToggleFeedbackText () {
            feedbackBox.enabled = !feedbackBox.enabled;
            feedbackBoxExtra.enabled = !feedbackBoxExtra.enabled;
        }

        public void SetPropertiesPanel (GameObject obj) {
            AnchorData data = obj.GetComponent<AnchorData> ();
            //feedbackBox.text += "AnchorData Abrufen - Name " + data.AnchorName + ". ";

            if (data.AnchorName != null) {

                uiHandler.nameOutput.text = data.AnchorName;
                uiHandler.idOutput.text = data.AnchorId;
                uiHandler.infoInput.text = data.AnchorInfo;
                uiHandler.secondsOutput.text = elapsedSeconds.ToString ();
                uiHandler.dateOutput.text = data.AnchorDate;
                uiHandler.progressOutput.text = data.AnchorProgress;
                uiHandler.keyOutput.text = data.AnchorKey;
                uiHandler.positionOutput.text = data.AnchorPosition;
                uiHandler.rotationOutput.text = data.AnchorRotation;

                //feedbackBox.text = "Position: " + data.AnchorPositionLocalization + ", Rotation: " + data.AnchorRotationLocalization + ". ";

                SaveDataToJson saveObject = new SaveDataToJson ();
                float progressLooking = CloudManager.SessionStatus.RecommendedForCreateProgress;

                saveObject.SaveData (obj, elapsedSeconds.ToString (), progressLooking.ToString (), maxFeaturePoints, testPhase);
                feedbackBox.text += "Data saved. ";
                testPhase = "";
                anchorIdToLocate = "";

            } else {
                feedbackBox.text += "Daten wurden nicht gespeichert.";
            }
        }

        public void ToggleControll () {
            switch (currentAppState) {
                case AppState.PlacingAnchor:
                    currentAppState = AppState.InputAnchorData;
                    break;
                case AppState.FoundAnchor:
                    currentAppState = AppState.ShowAnchorData;
                    break;
                case AppState.ShowAnchorData:
                    currentAppState = AppState.FoundAnchor;
                    break;
                case AppState.Default:
                    if (EventSystem.current.currentSelectedGameObject.name == "ButtonPlacing") { currentAppState = AppState.InputAnchorData; }
                    if (EventSystem.current.currentSelectedGameObject.name == "ButtonLocalize") { currentAppState = AppState.InputAnchorDataLocalize; }
                    break;
                default:
                    feedbackBox.text = "Nothing to toggle.";
                    return;
            }
            switchUiElements ();
        }

        public void switchUiElements () {
            switch (currentAppState) {
                case AppState.LoadingKeys:
                    //feedbackBox.text += "Switch Loading Keys";
                    uiHandler.placingButton.interactable = false; ///// Wait for App Loaded
                    uiHandler.localizeButton.interactable = false; ///// Wait for App Loaded

                    break;
                case AppState.Default:
                    //feedbackBox.text += "Switch Default";
                    if (!uiHandler.placingButton.gameObject.activeSelf) {
                        uiHandler.placingButton.gameObject.SetActive (true);
                        uiHandler.localizeButton.gameObject.SetActive (true);
                    }

                    uiHandler.placingButton.interactable = true; ///// Wait for App Loaded
                    uiHandler.localizeButton.interactable = true; ///// Wait for App Loaded

                    if (uiHandler.finishCollecting.gameObject.activeSelf) { uiHandler.finishCollecting.gameObject.SetActive (false); }
                    break;
                case AppState.PlacingAnchor:
                    // Deactivate Starting Buttons
                    uiHandler.ToggleInputCanvas ();
                    uiHandler.finishPlacing.gameObject.SetActive (true);
                    break;
                case AppState.InputAnchorData:
                    uiHandler.ToggleInputCanvas ();

                    uiHandler.placingButton.gameObject.SetActive (false);
                    uiHandler.localizeButton.gameObject.SetActive (false);
                    break;
                case AppState.SavingAnchor:
                    uiHandler.finishCollecting.gameObject.SetActive (true);
                    uiHandler.finishPlacing.gameObject.SetActive (false);
                    break;
                case AppState.InputAnchorDataLocalize:
                    uiHandler.placingButton.gameObject.SetActive (false);
                    uiHandler.localizeButton.gameObject.SetActive (false);

                    uiHandler.toggleIdInput.SetActive (true);
                    uiHandler.saveIdLocalize.gameObject.SetActive (true);
                    break;
                case AppState.LookingForAnchor:
                    uiHandler.toggleIdInput.SetActive (false);
                    uiHandler.saveIdLocalize.gameObject.SetActive (false);
                    break;
                case AppState.ShowAnchorData:
                    SetPropertiesPanel (spawnedObject);
                    uiHandler.showData.gameObject.SetActive (false);
                    uiHandler.ToggleOutputCanvas ();
                    break;
                case AppState.FoundAnchor:
                    uiHandler.showData.gameObject.SetActive (true);
                    currentAppState = AppState.ShowAnchorData;
                    break;
                default:
                    break;
            }
        }

        #endregion UI Methods

        //////////////////////////////////////////////////////////////////////
        /////////////////// Key/ID Methods //////////////////////////////////////
        //////////////////////////////////////////////////////////////////////

        #region Key Method

#pragma warning disable CS1998
        public async void StoreAllAnchorKeys ()
#pragma warning restore CS1998
        {
#if !UNITY_EDITOR
            //Saves Key-Sequenz of last key 
            long _anchorNumber = -1;
            string _lastAnchorId = await anchorExchanger.RetrieveLastAnchorKey ();
            string _currentAnchorId;

            //feedbackBoxExtra.text += "Last Key: " + _lastAnchorId + ". ";

            if (!String.IsNullOrWhiteSpace (_lastAnchorId)) {
                //Loop, that counts all numbers, till last key is reached -> get last key-number
                do {
                    _anchorNumber++; //First loop, Number 0 is checked
                    _currentAnchorId = await anchorExchanger.RetrieveAnchorKey (_anchorNumber);
                } while (string.Compare (_currentAnchorId, _lastAnchorId) != 0);

                feedbackBoxExtra.text += "Last Key has number: " + _anchorNumber + ". ";
                anchorList.Clear ();

                //Backwards: counts from last key-number down, till 0 reached. Stores all key-sequenzes in a List.
                while (_anchorNumber >= 0) {
                    if (!string.IsNullOrWhiteSpace (await anchorExchanger.RetrieveAnchorKey (_anchorNumber))) {
                        anchorList.Add (await anchorExchanger.RetrieveAnchorKey (_anchorNumber));
                        _anchorNumber--;
                    }
                }
                feedbackBoxExtra.text += "All Keys saved. Total: " + anchorList.Count + ". ";
            } else {
                feedbackBox.text += "No Keys found.";
            }

            if (currentAppState == AppState.LoadingKeys) {
                currentAppState = AppState.Default;
                switchUiElements ();
                feedbackBox.text = "Welcome!";
            } else if (currentAppState == AppState.SavingAnchor) {
                currentAppState = AppState.Default;
                switchUiElements ();
                feedbackBox.text = "Saving successful.";
            }
#endif
        }
        #endregion Key Method

        //////////////////////////////////////////////////////////////////////
        /////////////////// Override Methods //////////////////////////////////
        //////////////////////////////////////////////////////////////////////

        #region Override Methods

        protected override bool IsPlacingObject () {
            return currentAppState == AppState.PlacingAnchor;
        }

        protected override async Task OnSaveCloudAnchorSuccessfulAsync () {
            await base.OnSaveCloudAnchorSuccessfulAsync ();

            currentAnchorId = currentCloudAnchor.Identifier;

#if !UNITY_EDITOR
            var anchorNumber = (await anchorExchanger.StoreAnchorKey (currentCloudAnchor.Identifier));
#endif

            // Sanity check that the object is still where we expect
            Pose anchorPose = Pose.identity;

#if UNITY_ANDROID || UNITY_IOS
            anchorPose = currentCloudAnchor.GetPose ();
#endif
            // HoloLens: The position will be set based on the unityARUserAnchor that was located.

            SpawnOrMoveCurrentAnchoredObject (anchorPose.position, anchorPose.rotation);
            spawnedObject = null;
            currentCloudAnchor = null;
            currentAppState = AppState.Default;
        }

        protected override void OnSaveCloudAnchorFailed (Exception exception) {
            base.OnSaveCloudAnchorFailed (exception);

            currentAnchorId = string.Empty;
        }

        protected override void OnCloudAnchorLocated (AnchorLocatedEventArgs args) {
            base.OnCloudAnchorLocated (args);

            //Checking if found anchor equals the anchor we are looking for.
            if (currentAppState == AppState.LookingForAnchor && args.Anchor.AppProperties[@"id"] == anchorIdToLocate) {
                stopwatchTimer.Stop ();
                elapsedSeconds = stopwatchTimer.ElapsedMilliseconds;

                if (args.Status == LocateAnchorStatus.Located) {
                    currentCloudAnchor = args.Anchor;

                    UnityDispatcher.InvokeOnAppThread (() => {
                        currentAppState = AppState.FoundAnchor;
                        switchUiElements ();
                        Pose anchorPose = Pose.identity;

#if UNITY_ANDROID || UNITY_IOS
                        anchorPose = currentCloudAnchor.GetPose ();
#endif
                        // HoloLens: The position will be set based on the unityARUserAnchor that was located.
                        SpawnOrMoveCurrentAnchoredObject (anchorPose.position, anchorPose.rotation);
                    });
                }
            }
        }
        #endregion Override Methods

        //////////////////////////////////////////////////////////////////////
        /////////////////// App Methods //////////////////////////////////////
        //////////////////////////////////////////////////////////////////////

        #region App Specific Methods

        public async Task InitializeSession () {
            if (CloudManager.Session == null) {
                await CloudManager.CreateSessionAsync ();
                feedbackBox.text += "Session created. ";
            } else {
                await CloudManager.ResetSessionAsync ();
            }
            currentAnchorId = "";
            currentCloudAnchor = null;

            await CloudManager.StartSessionAsync ();
        }

        public async void StartPlacingSession () {
            currentAppState = AppState.PlacingAnchor; ////// App State

            anchorName = uiHandler.nameInput.text.ToString ();
            anchorId = uiHandler.idInput.text.ToString ();
            anchorInfo = uiHandler.infoInput.text.ToString ();

            uiHandler.nameInput.text = "";
            uiHandler.idInput.text = "";
            uiHandler.infoInput.text = "";

            switchUiElements ();

            await InitializeSession ();

            feedbackBox.text += "Session started. ";
            speechBubbleText.text = "Place an Object now. ";
        }

        public async void DonePlacingObjects () {
            //feedbackBox.text = "Currend App State: " + currentAppState + ". ";
            if (currentAppState == AppState.PlacingAnchor) {
                currentAppState = AppState.SavingAnchor;
                switchUiElements ();

                if (!String.IsNullOrWhiteSpace (anchorName) && !String.IsNullOrWhiteSpace (anchorId)) {
                    speechBubbleText.text = "Saving Anchor, please wait.";

                    if (spawnedObject != null) {
                        await SaveCurrentObjectAnchorToCloudAsync ();
                        StoreAllAnchorKeys ();
                    }

                    CloudManager.StopSession ();
                    CleanupSpawnedObjects ();

                    await CloudManager.ResetSessionAsync (); // Attention! 

                    currentAppState = AppState.Default; ////// App State
                    switchUiElements ();
                }
            }
        }

        public async void LookingForObject () {
            currentAppState = AppState.LookingForAnchor; ////// App State
            switchUiElements ();
            speechBubbleText.text = "Trying to look for an Object now. ";

            anchorIdToLocate = uiHandler.idInputLocalize.text.ToString ();
            testPhase = uiHandler.testPhaseInput.text.ToString ();

            await InitializeSession ();

            // Watching Part
            //The anchor list contains all anchor keys from the Webapp (Anchor Exchanger).
            SetIdCriteria (anchorList.ToArray ());
            currentWatcher = CreateWatcher ();
            stopwatchTimer.Start ();

            // NOW ITS LOOKING FOR ANCHORS
        }

        #endregion 

        public string BaseSharingUrl { get => baseSharingUrl; set => baseSharingUrl = value; }
    }
}