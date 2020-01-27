using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Azure.SpatialAnchors.Unity {

    public class MySpatialApp : MyAppBase {
        internal enum AppState {
            Default = 0,
            StartingSession,
            PlacingAnchor,
            StoppingSession,
            LookingForAnchor,
            SavingAnchor,
            InfoCanvas,
            FoundAnchor
        }

        #region Variables

        private AppState _currentAppState = AppState.Default;
#if !UNITY_EDITOR
        public MyAnchorExchanger anchorExchanger = new MyAnchorExchanger ();
#endif
        private List<string> anchorList = new List<string> ();

        private string baseSharingUrl = "";
        private int anchorsLocated;
        private Button placingButton, findingButton, finishButton, showButton, doneButton;
        GameObject toggleCanvas, toggleOutput;
        Text nameText, descriptionText;

        AppState currentAppState {
            get {
                return _currentAppState;
            }
            set {
                if (_currentAppState != value) {
                    Debug.LogFormat ("State from {0} to {1}", _currentAppState, value);
                    _currentAppState = value;
                }
            }
        }

        private string currentAnchorId = "";

        private List<GameObject> _spawnedObjects = new List<GameObject> ();
        readonly Dictionary<string, GameObject> _spawnedObjectsWithIds = new Dictionary<string, GameObject> ();
        private List<CloudSpatialAnchor> _allAnchors = new List<CloudSpatialAnchor> ();

        #endregion Variables

        public override void Start () {
            Debug.Log (">>Azure Spatial Anchors Demo Script Start");

            base.Start ();

            if (!SanityCheckAccessConfiguration ()) {
                return;
            }

            speechBubbleText.text = "App is loading...";

            placingButton = GameObject.Find ("PlacingObject").GetComponent<Button> ();
            findingButton = GameObject.Find ("FindingObject").GetComponent<Button> ();
            finishButton = GameObject.Find ("FinishedPlacing").GetComponent<Button> ();
            showButton = GameObject.Find ("ShowAllAnchors").GetComponent<Button> ();
            doneButton = GameObject.Find ("DoneFinding").GetComponent<Button> ();

            finishButton.gameObject.SetActive (false); ///// BUTTON
            showButton.gameObject.SetActive (false); ///// BUTTON
            doneButton.gameObject.SetActive (false); ///// BUTTON

            placingButton.interactable = false; ///// BUTTON
            findingButton.interactable = false; ///// BUTTON

            nameText = GameObject.Find ("NameOutput").GetComponent<Text> ();
            descriptionText = GameObject.Find ("DescriptionOutput").GetComponent<Text> ();

            toggleCanvas = GameObject.Find ("InputCanvas");
            toggleOutput = GameObject.Find ("OutputCanvas");
            toggleCanvas.SetActive (false);
            toggleOutput.SetActive (false);

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
            StoreAllAnchorKeys ();

            Debug.Log ("Azure Spatial Anchors Demo script started");
        } // End Start

        // Update is called once per frame
        public override void Update () {
            base.Update ();

            if (spawnedObjectMat != null) {
                float createProgress = 0f;
                if (CloudManager.SessionStatus != null) {
                    createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;
                }
            }

            if (Input.touchCount > 0 && currentAppState == AppState.FoundAnchor) {
                for (int i = 0; i < Input.touchCount; i++) {
                    Touch touch = Input.GetTouch (i);
                    if (touch.phase == TouchPhase.Ended) {
                        Ray screenRay = Camera.main.ScreenPointToRay (touch.position);
                        RaycastHit hit;

                        if (Physics.Raycast (screenRay, out hit)) {
                            if (hit.collider.CompareTag ("3DObject")) {
                                GameObject obj = hit.collider.gameObject;
                                SetPropertiesPanel (obj);
                            }
                        }
                    }
                }
            }
        }

        #region Override Methods

        protected override bool IsPlacingObject () {
            return currentAppState == AppState.PlacingAnchor;
        }

        protected override async Task OnSaveCloudAnchorSuccessfulAsync () {
            await base.OnSaveCloudAnchorSuccessfulAsync ();

            Debug.Log ("Anchor created, yay!");

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

            if (currentAppState == AppState.LookingForAnchor) //  && !_spawnedObjectsWithIds.ContainsKey(args.Anchor.Identifier)
            {
                if (args.Status == LocateAnchorStatus.Located) {
                    currentCloudAnchor = args.Anchor;
                    _allAnchors.Add (currentCloudAnchor); // Save Anchor in List
                    anchorsLocated++;
                }
                speechBubbleText.text = "You located " + anchorsLocated + "/" + anchorList.Count + " Anchor(s). Press Show to see where they are.";
            }
        }

        protected override void SpawnOrMoveCurrentAnchoredObject (Vector3 worldPos, Quaternion worldRot) {
            if (currentCloudAnchor != null && _spawnedObjectsWithIds.ContainsKey (currentCloudAnchor.Identifier)) {
                spawnedObject = _spawnedObjectsWithIds[currentCloudAnchor.Identifier];
            }

            bool spawnedNewObject = spawnedObject == null;

            base.SpawnOrMoveCurrentAnchoredObject (worldPos, worldRot);

            if (spawnedNewObject) {
                _spawnedObjects.Add (spawnedObject);
                if (currentCloudAnchor != null && _spawnedObjectsWithIds.ContainsKey (currentCloudAnchor.Identifier) == false) {
                    _spawnedObjectsWithIds.Add (currentCloudAnchor.Identifier, spawnedObject);
                }
            }

#if WINDOWS_UWP || UNITY_WSA
            if (currentCloudAnchor != null &&
                _spawnedObjectsWithIds.ContainsKey (currentCloudAnchor.Identifier) == false) {
                _spawnedObjectsWithIds.Add (currentCloudAnchor.Identifier, spawnedObject);
            }
#endif
        }

        protected override void CleanupSpawnedObjects () {
            base.CleanupSpawnedObjects ();
            foreach (GameObject obj in _spawnedObjects) {
                if (obj != null) {
                    Destroy (obj);
                }
            }
            _spawnedObjects.Clear ();
        }

        #endregion Override Methods

        #region UI Methods

        public void SetPropertiesPanel (GameObject obj) //string init
        {
            AnchorData data = obj.GetComponent<AnchorData> ();
            if (data.AnchorName != null) {
                anchorName = obj.GetComponent<AnchorData> ().AnchorName;
                anchorDescription = obj.GetComponent<AnchorData> ().AnchorDescription;

                nameText.text = anchorName;
                descriptionText.text = anchorDescription;
                ToggleOutputCanvas ();
            } else {
                feedbackBox.text += "Daten wurden nicht gespeichert.";
            }
        }

        public void toggleFeedbackText () {
            feedbackBox.enabled = !feedbackBox.enabled;
            feedbackBoxExtra.enabled = !feedbackBoxExtra.enabled;
        }

        public void ToggleInputCanvas () {
            if (toggleCanvas.activeSelf) {
                toggleCanvas.SetActive (false);
            } else {
                toggleCanvas.SetActive (true);
                currentAppState = AppState.SavingAnchor;
            }
        }

        public void ToggleOutputCanvas () {
            if (toggleOutput.activeSelf) {
                toggleOutput.SetActive (false);
                currentAppState = AppState.FoundAnchor;
            } else {
                toggleOutput.SetActive (true);
                currentAppState = AppState.InfoCanvas;
            }
        }

        #endregion UI Methods

        #region Key Methods
        public string GetLocalIdentifier (GameObject obj) {
            string init = "";
            for (int i = 0; i < _spawnedObjects.Count; i++) {
                if (obj.Equals (_spawnedObjects[i])) {
                    init = _allAnchors[i].Identifier;
                }
            }
            return init;
        }

#pragma warning disable CS1998
        public async void StoreAllAnchorKeys ()
#pragma warning restore CS1998
        {
#if !UNITY_EDITOR
            //Saves Key-Sequenz of last key 
            long _anchorNumber = -1;
            string _lastAnchorId = await anchorExchanger.RetrieveLastAnchorKey ();
            string _currentAnchorId = "";

            anchorList.Clear ();

            if (!String.IsNullOrWhiteSpace (_lastAnchorId)) {
                //Loop, that counts all numbers, till last key is reached -> get last key-number
                while (string.Compare (_currentAnchorId, _lastAnchorId) != 0) {
                    _anchorNumber++; //First loop, Number 0 is checked
                    _currentAnchorId = await anchorExchanger.RetrieveAnchorKey (_anchorNumber);
                    if (!string.IsNullOrWhiteSpace (_currentAnchorId)) {
                        anchorList.Add (_currentAnchorId);
                    }
                }

                feedbackBoxExtra.text += "Last Key has number: " + _anchorNumber + ". ";
                feedbackBoxExtra.text += "All Keys saved. Total: " + anchorList.Count + ". ";
            } else {
                feedbackBox.text += "No Keys found.";
            }

            if (currentAppState == AppState.Default) {
                speechBubbleText.text = "Welcome!";
            } else if (currentAppState == AppState.PlacingAnchor) {
                speechBubbleText.text = "Saving successful.";
            }
            placingButton.interactable = true;
            findingButton.interactable = true;
#endif
        }

        #endregion Key Methods

        #region App Methods
        public async void StartingSession () {
            currentAppState = AppState.StartingSession; ////// App State
            speechBubbleText.text = "Trying to place an Object now. ";

            finishButton.gameObject.SetActive (true); ////// BUTTON 
            placingButton.interactable = false; ////// BUTTON
            findingButton.interactable = false;

            if (CloudManager.Session == null) {
                await CloudManager.CreateSessionAsync ();
                feedbackBox.text = "Session created. ";
            } else {
                await CloudManager.ResetSessionAsync ();
            }
            currentAnchorId = "";
            currentCloudAnchor = null;

            currentAppState = AppState.PlacingAnchor;

            await CloudManager.StartSessionAsync ();
            feedbackBox.text += "Session started. ";

            currentAppState = AppState.PlacingAnchor; ////// App State
        }

        public async void DonePlacingObjects () {
            if (currentAppState == AppState.SavingAnchor) {
                InputField nameInput = GameObject.Find ("NameInput").GetComponent<InputField> ();
                InputField desInput = GameObject.Find ("DescriptionInput").GetComponent<InputField> ();

                anchorName = nameInput.text.ToString ();
                anchorDescription = desInput.text.ToString ();

                nameInput.text = "";
                desInput.text = "";

                if (!String.IsNullOrWhiteSpace (anchorName) && !String.IsNullOrWhiteSpace (anchorDescription)) {
                    ToggleInputCanvas ();

                    currentAppState = AppState.StoppingSession; ////// App State
                    finishButton.gameObject.SetActive (false); ////// BUTTON 

                    speechBubbleText.text = "Saving Anchor, please wait.";

                    if (spawnedObject != null) {
                        await SaveCurrentObjectAnchorToCloudAsync ();
                        StoreAllAnchorKeys ();
                    }

                    CloudManager.StopSession ();
                    CleanupSpawnedObjects ();

                    speechBubbleText.text += "Session endet. Name: " + currentCloudAnchor.AppProperties[@"name"] + ". ";

                    await CloudManager.ResetSessionAsync (); // Attention! 

                    currentAppState = AppState.Default; ////// App State
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
            speechBubbleText.text = "Trying to look for an Object now. ";

            findingButton.interactable = false; ////// BUTTON 
            placingButton.interactable = false;
            showButton.gameObject.SetActive (true); ////// BUTTON 

            if (CloudManager.Session == null) {
                await CloudManager.CreateSessionAsync ();
                feedbackBox.text = "Session created. ";
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
            currentWatcher = CreateWatcher ();
            feedbackBox.text += "Watcher erstellt. ";

            // NOW ITS LOOKING FOR ANCHORS

        }

        public async void StoppingSession () {
            if (currentAppState == AppState.FoundAnchor) {
                doneButton.gameObject.SetActive (false);

                CloudManager.StopSession ();
                currentWatcher = null;
                currentCloudAnchor = null;
                CleanupSpawnedObjects ();
                _allAnchors.Clear ();

                feedbackBox.text += "Session beendet. ";

                await CloudManager.ResetSessionAsync (); // Attention! 
                findingButton.interactable = true;
                placingButton.interactable = true;

                speechBubbleText.text = "Welcome!";

                currentAppState = AppState.Default; ////// App State
            }

        }

        public void ShowAllAnchors () {
            showButton.gameObject.SetActive (false); ///// BUTTON
            doneButton.gameObject.SetActive (true); ///// BUTTON

            currentAppState = AppState.FoundAnchor;

            speechBubbleText.text = "There should be " + _allAnchors.Count + " Anchor(s) here.";
            anchorsLocated = 0;
            CleanupSpawnedObjects ();
            UnityDispatcher.InvokeOnAppThread (() => {
                foreach (CloudSpatialAnchor anchor in _allAnchors) {
                    Pose anchorPose = Pose.identity;

#if UNITY_ANDROID || UNITY_IOS
                    anchorPose = anchor.GetPose ();
#endif
                    // HoloLens: The position will be set based on the unityARUserAnchor that was located.
                    GameObject _localSpawnedObject = SpawnNewAnchoredObject (anchorPose.position, anchorPose.rotation, anchor);

                    _spawnedObjects.Add (_localSpawnedObject);
                }
            });
        }

        #endregion

        public string BaseSharingUrl { get => baseSharingUrl; set => baseSharingUrl = value; }
    }
}