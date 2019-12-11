using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Azure.SpatialAnchors.Unity {

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
#if !UNITY_EDITOR
        public MyAnchorExchanger anchorExchanger = new MyAnchorExchanger ();
#endif
        private List<string> anchorList = new List<string> ();
        private string baseSharingUrl = "";
        private List<CloudSpatialAnchor> _allAnchors = new List<CloudSpatialAnchor> ();

        // UI Elements
        public UIHandler uiHandler;
        private string anchorIdToLocate;

        // Messung
        private Stopwatch stopwatchTimer = new Stopwatch ();
        private Int64 elapsedSeconds;

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

        private string currentAnchorId = "";

        public override void Start () {
            UnityEngine.Debug.Log (">>Azure Spatial Anchors Demo Script Start");

            base.Start ();

            if (!SanityCheckAccessConfiguration ()) {
                return;
            }

            feedbackBox.text = "App is loading...";

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

            UnityEngine.Debug.Log ("Azure Spatial Anchors Demo script started");
        } // End Start

        // Update is called once per frame
        public override void Update () {

            if (IsPlacingObject () && CloudManager.FeaturePoints != null) {
                feedbackBoxExtra.text = $"PointCloud: {CloudManager.FeaturePoints.Count}";
            }

            if (currentAppState == AppState.LookingForAnchor) {
                speechBubbleText.text = $"Progress: {CloudManager.SessionStatus.RecommendedForCreateProgress:0%}. ";
            }

            base.Update ();
        }

        //////////////////////////////////////////////////////////////////////
        /////////////////// UI Methods //////////////////////////////////////
        //////////////////////////////////////////////////////////////////////

        #region UI Methods
        public void toggleFeedbackText () {
            feedbackBox.enabled = !feedbackBox.enabled;
            feedbackBoxExtra.enabled = !feedbackBoxExtra.enabled;
        }

        public void SetPropertiesPanel (GameObject obj) //string init
        {
            AnchorData data = obj.GetComponent<AnchorData> ();
            feedbackBox.text += "AnchorData Abrufen - Name " + data.AnchorName + ". ";

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

                SaveDataToJson saveObject = new SaveDataToJson();
                float progressLooking = CloudManager.SessionStatus.RecommendedForCreateProgress;
                saveObject.SaveData(obj, elapsedSeconds.ToString(), progressLooking);
                feedbackBox.text += "Data saved. ";
                
            } else {
                feedbackBox.text += "Daten wurden nicht gespeichert.";
            }
        }

        public void ToggleInputCanvas () {
            if (currentAppState == AppState.PlacingAnchor) {
                currentAppState = AppState.InputAnchorData;
                switchUiElements ();
            }
        }

        public void ToggleOutputCanvas () {
            if (currentAppState == AppState.FoundAnchor) {
                currentAppState = AppState.ShowAnchorData;
                switchUiElements ();
            } else if (currentAppState == AppState.ShowAnchorData) {
                currentAppState = AppState.FoundAnchor;
                switchUiElements ();
            }
        }

        public void ToggleIdInput () {
            currentAppState = AppState.InputAnchorDataLocalize;
            switchUiElements ();
        }
        #endregion UI Methods

        public void switchUiElements () {
            switch (currentAppState) {
                case AppState.LoadingKeys:
                    feedbackBox.text += "Switch Loading Keys";
                    uiHandler.placingButton.interactable = false; ///// Wait for App Loaded
                    uiHandler.localizeButton.interactable = false; ///// Wait for App Loaded

                    break;
                case AppState.Default:
                    feedbackBox.text += "Switch Default";
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
                    uiHandler.placingButton.gameObject.SetActive (false);
                    uiHandler.localizeButton.gameObject.SetActive (false);

                    uiHandler.finishPlacing.gameObject.SetActive (true);
                    break;
                case AppState.InputAnchorData:
                    uiHandler.ToggleInputCanvas ();
                    uiHandler.finishPlacing.gameObject.SetActive (false);
                    break;
                case AppState.SavingAnchor:
                    uiHandler.ToggleInputCanvas ();
                    uiHandler.finishCollecting.gameObject.SetActive (true);
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

        //////////////////////////////////////////////////////////////////////
        /////////////////// Key/ID Methods //////////////////////////////////////
        //////////////////////////////////////////////////////////////////////

        #region KeyMethods

#pragma warning disable CS1998
        public async void StoreAllAnchorKeys ()
#pragma warning restore CS1998
        {
#if !UNITY_EDITOR
            //Saves Key-Sequenz of last key 
            long _anchorNumber = -1;
            string _lastAnchorId = await anchorExchanger.RetrieveLastAnchorKey ();
            string _currentAnchorId;

            feedbackBoxExtra.text += "Letzer Key: " + _lastAnchorId + ". ";

            if (!String.IsNullOrWhiteSpace (_lastAnchorId)) {
                //Loop, that counts all numbers, till last key is reached -> get last key-number
                do {
                    _anchorNumber++; //First loop, Number 0 is checked
                    _currentAnchorId = await anchorExchanger.RetrieveAnchorKey (_anchorNumber);
                } while (string.Compare (_currentAnchorId, _lastAnchorId) != 0);

                feedbackBoxExtra.text += "Letzter Key ist die Nummer: " + _anchorNumber + ". ";
                anchorList.Clear ();

                //Backwards: counts from last key-number down, till 0 reached. Stores all key-sequenzes in a List.
                while (_anchorNumber >= 0) {
                    if (!string.IsNullOrWhiteSpace (await anchorExchanger.RetrieveAnchorKey (_anchorNumber))) {
                        anchorList.Add (await anchorExchanger.RetrieveAnchorKey (_anchorNumber));
                        _anchorNumber--;
                    }
                }
                feedbackBoxExtra.text += "Alle Keys gespeichert! Anzahl: " + anchorList.Count + ". ";
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
        #endregion KeyMethods

        //////////////////////////////////////////////////////////////////////
        /////////////////// Override Methods //////////////////////////////////
        //////////////////////////////////////////////////////////////////////

        #region Override Methods

        protected override bool IsPlacingObject () {
            return currentAppState == AppState.PlacingAnchor;
        }
        protected override async Task OnSaveCloudAnchorSuccessfulAsync () {
            await base.OnSaveCloudAnchorSuccessfulAsync ();

            UnityEngine.Debug.Log ("Anchor created, yay!");

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

        /*protected override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
        {
            base.OnCloudAnchorLocated(args);

            if (currentAppState == AppState.LookingForAnchor) //  && !_spawnedObjectsWithIds.ContainsKey(args.Anchor.Identifier)
            {
                feedbackBox.text += "Anker gefunden.";

                if (args.Status == LocateAnchorStatus.Located)
                {
                    currentCloudAnchor = args.Anchor;
                    feedbackBox.text += "Identifier: " + currentCloudAnchor.Identifier + ". ";
                    _allAnchors.Add(currentCloudAnchor); // Save Anchor in List
                    anchorsLocated++;
                }

                feedbackBox.text = "You located " + anchorsLocated + "/" + anchorList.Count + " Anchor(s). Press Show to see where they are.";
            }
        }*/

        protected override void OnCloudAnchorLocated (AnchorLocatedEventArgs args) {
            base.OnCloudAnchorLocated (args);

            if (currentAppState == AppState.LookingForAnchor && args.Anchor.AppProperties[@"id"] == anchorIdToLocate) //  && !_spawnedObjectsWithIds.ContainsKey(args.Anchor.Identifier)
            {
                stopwatchTimer.Stop ();
                elapsedSeconds = stopwatchTimer.ElapsedMilliseconds;
                feedbackBox.text += "Anker gefunden.";

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

        public async void StartPlacingSession () {
            currentAppState = AppState.PlacingAnchor; ////// App State
            switchUiElements ();

            feedbackBox.text = "Trying to place an Object now. ";
            //feedbackBoxExtra.text = $"Point Cloud Data: {CloudManager.FeaturePoints}";

            if (CloudManager.Session == null) {
                await CloudManager.CreateSessionAsync ();
                feedbackBox.text += "Session created. ";
            } else {
                await CloudManager.ResetSessionAsync ();
            }
            currentAnchorId = "";
            currentCloudAnchor = null;

            await CloudManager.StartSessionAsync ();
            feedbackBox.text += "Session started. ";
        }

        public async void DonePlacingObjects () {
            feedbackBox.text = "Currend App State: " + currentAppState + ". ";
            if (currentAppState == AppState.InputAnchorData) {
                currentAppState = AppState.SavingAnchor;
                switchUiElements ();

                feedbackBoxExtra.text = $"Point Cloud Data: {CloudManager.FeaturePoints.Count}";
                feedbackBox.text = "Currend App State: " + currentAppState + ". ";

                anchorName = uiHandler.nameInput.text.ToString ();
                anchorId = uiHandler.idInput.text.ToString ();
                anchorInfo = uiHandler.infoInput.text.ToString ();

                uiHandler.nameInput.text = "";
                uiHandler.idInput.text = "";
                uiHandler.infoInput.text = "";

                feedbackBoxExtra.text += "Eingegebene Name: " + anchorName + ". ";

                if (!String.IsNullOrWhiteSpace (anchorName) && !String.IsNullOrWhiteSpace (anchorId)) {
                    feedbackBox.text = "Saving Anchor, please wait.";

                    if (spawnedObject != null) {
                        await SaveCurrentObjectAnchorToCloudAsync ();
                        StoreAllAnchorKeys ();
                    }

                    CloudManager.StopSession ();
                    CleanupSpawnedObjects ();

                    feedbackBox.text += "Session endet. Name: " + currentCloudAnchor.AppProperties[@"name"] + ". ";

                    await CloudManager.ResetSessionAsync (); // Attention! 

                    currentAppState = AppState.Default; ////// App State
                    switchUiElements ();
                }
            }
        }

        public async void DeletingAnchor () {
            if (currentCloudAnchor != null) {
                await CloudManager.DeleteAnchorAsync (currentCloudAnchor);
                feedbackBox.text += "Anchor deleted.";
            } else {
                feedbackBox.text += "No Anchor found to delete.";
            }

            CloudManager.StopSession ();
            currentWatcher = null;
            currentCloudAnchor = null;
            CleanupSpawnedObjects ();
            feedbackBox.text += "Session beendet. ";

            currentAppState = AppState.Default;

        }

        public async void LookingForObject () {
            currentAppState = AppState.LookingForAnchor; ////// App State
            switchUiElements ();
            feedbackBox.text = "Trying to look for an Object now. ";

            anchorIdToLocate = uiHandler.idInputLocalize.text.ToString ();

            if (CloudManager.Session == null) {
                await CloudManager.CreateSessionAsync ();
                feedbackBox.text += "Session created. ";
            } else {
                await CloudManager.ResetSessionAsync ();
            }

            currentAnchorId = "";
            currentCloudAnchor = null;
            //ConfigureSession ();

            await CloudManager.StartSessionAsync ();
            feedbackBox.text += "Session started. ";

            // Watching Part

            SetIdCriteria (anchorList.ToArray ());
            feedbackBox.text += "Kriterien erstellt: " + anchorLocateCriteria.Identifiers + ". Session: " + CloudManager.Session + ". ";
            currentWatcher = CreateWatcher ();
            stopwatchTimer.Start ();
            feedbackBox.text += "Watcher erstellt. ";

            // NOW ITS LOOKING FOR ANCHORS

        }

        public async void StoppingSession () {
            if (currentAppState == AppState.FoundAnchor) {

                CloudManager.StopSession ();
                currentWatcher = null;
                currentCloudAnchor = null;
                CleanupSpawnedObjects ();
                _allAnchors.Clear ();

                feedbackBox.text += "Session beendet. ";

                await CloudManager.ResetSessionAsync (); // Attention! 

                feedbackBox.text = "Welcome!";

                currentAppState = AppState.Default; ////// App State
            }

        }

        private void ConfigureSession () {
            List<string> anchorsToFind = new List<string> ();
            if (currentAppState == AppState.PlacingAnchor) {
                anchorsToFind.Add (currentAnchorId);
            }

            SetAnchorIdsToLocate (anchorsToFind);
        }

        public string BaseSharingUrl { get => baseSharingUrl; set => baseSharingUrl = value; }

    }
}