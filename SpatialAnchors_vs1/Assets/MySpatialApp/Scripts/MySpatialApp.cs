using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Azure.SpatialAnchors.Unity
{

    public class MySpatialApp : MyAppBase
    {
        internal enum AppState
        {
            Default = 0,
            StartingSession,
            PlacingAnchor,
            StoppingSession,
            LookingForAnchor,
            SavingAnchor
        }

        private readonly Dictionary<AppState, DemoStepParams> stateParams = new Dictionary<AppState, DemoStepParams> {
            { AppState.Default, new DemoStepParams () { StepMessage = "Choose an Option.", StepColor = Color.red  } },
            { AppState.StartingSession, new DemoStepParams () { StepMessage = "Starting a Session", StepColor = Color.black} },
            { AppState.PlacingAnchor, new DemoStepParams () { StepMessage = "Place an Anchor.", StepColor = Color.black } },
            { AppState.StoppingSession, new DemoStepParams () { StepMessage = "Stopping the Session.", StepColor = Color.black} },
            { AppState.LookingForAnchor, new DemoStepParams () { StepMessage = "Looking for Anchors.", StepColor = Color.green  } },
            { AppState.SavingAnchor, new DemoStepParams () { StepMessage = "Saving Anchor.", StepColor = Color.green  } }
        };

        private AppState _currentAppState = AppState.Default;
#if !UNITY_EDITOR
        public MyAnchorExchanger anchorExchanger = new MyAnchorExchanger();
#endif
        private List<string> anchorList = new List<string>();

        private Button placingButton, finishButton, lookingButton, deletingButton;
        private string baseSharingUrl = "";
        //private List<GameObject> allSpawnedObjects = new List<GameObject>();
        //private int anchorsLocated = 0;


        AppState currentAppState
        {
            get
            {
                return _currentAppState;
            }
            set
            {
                if (_currentAppState != value)
                {
                    Debug.LogFormat("State from {0} to {1}", _currentAppState, value);
                    _currentAppState = value;

                    if (spawnedObjectMat != null)
                    {
                        spawnedObjectMat.color = stateParams[_currentAppState].StepColor;
                    }

                    if (!isErrorActive)
                    {
                        feedbackBox.text = stateParams[_currentAppState].StepMessage;
                    }
                }
            }
        }

        private string currentAnchorId = "";

        public override void Start()
        {
            Debug.Log(">>Azure Spatial Anchors Demo Script Start");

            base.Start();

            if (!SanityCheckAccessConfiguration())
            {
                return;
            }
            feedbackBox.text = stateParams[currentAppState].StepMessage;

            placingButton = GameObject.Find("Placing").GetComponent<Button>();
            finishButton = GameObject.Find("Finished").GetComponent<Button>();
            lookingButton = GameObject.Find("Looking").GetComponent<Button>();
            deletingButton = GameObject.Find("Deleting").GetComponent<Button>();


            deletingButton.gameObject.SetActive(false);
            finishButton.gameObject.SetActive(false);

            SpatialAnchorSamplesConfig samplesConfig = Resources.Load<SpatialAnchorSamplesConfig>("SpatialAnchorSamplesConfig");
            feedbackBox.text += "Samples Config: " + samplesConfig + " . ";
            feedbackBox.text += "Samples Config BaseURL: " + samplesConfig.BaseSharingURL + " . ";

            if (string.IsNullOrWhiteSpace(BaseSharingUrl) && samplesConfig != null)
            {
                BaseSharingUrl = samplesConfig.BaseSharingURL;
            }

            if (string.IsNullOrEmpty(BaseSharingUrl))
            {
                feedbackBox.text += $"Need to set {nameof(BaseSharingUrl)}.";
                return;
            }
            else
            {
                Uri result;
                if (!Uri.TryCreate(BaseSharingUrl, UriKind.Absolute, out result))
                {
                    feedbackBox.text = $"{nameof(BaseSharingUrl)} is not a valid url";
                    return;
                }
                else
                {
                    BaseSharingUrl = $"{result.Scheme}://{result.Host}/api/anchors";
                }
            }

#if !UNITY_EDITOR
            anchorExchanger.WatchKeys(BaseSharingUrl);
#endif
            StoreAllAnchorKeys();


            Debug.Log("Azure Spatial Anchors Demo script started");
        } // End Start

        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            if (spawnedObjectMat != null)
            {
                float rat = 0.1f;
                float createProgress = 0f;
                if (CloudManager.SessionStatus != null)
                {
                    createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;
                }
                rat += (Mathf.Min(createProgress, 1) * 0.9f);
                spawnedObjectMat.color = GetStepColor() * rat;
            }
        }

        #region Override Methods

        protected override bool IsPlacingObject()
        {
            return currentAppState == AppState.PlacingAnchor;
        }

        protected override Color GetStepColor()
        {
            return stateParams[currentAppState].StepColor;
        }

        protected override async Task OnSaveCloudAnchorSuccessfulAsync()
        {
            await base.OnSaveCloudAnchorSuccessfulAsync();

            Debug.Log("Anchor created, yay!");

            currentAnchorId = currentCloudAnchor.Identifier;
            feedbackBoxExtra.text += "Id: " + currentAnchorId + ". ";

#if !UNITY_EDITOR
            var anchorNumber = (await anchorExchanger.StoreAnchorKey(currentCloudAnchor.Identifier));
            feedbackBoxExtra.text += "Anchor has the number: " + anchorNumber + ". ";
#endif

            // Sanity check that the object is still where we expect
            Pose anchorPose = Pose.identity;

#if UNITY_ANDROID || UNITY_IOS
            anchorPose = currentCloudAnchor.GetPose ();
#endif
            // HoloLens: The position will be set based on the unityARUserAnchor that was located.

            SpawnOrMoveCurrentAnchoredObject(anchorPose.position, anchorPose.rotation);

            currentAppState = AppState.Default;
        }

        protected override void OnSaveCloudAnchorFailed(Exception exception)
        {
            base.OnSaveCloudAnchorFailed(exception);

            currentAnchorId = string.Empty;
        }

        public async void StartingSession()
        {

            placingButton.gameObject.SetActive(false);
            lookingButton.gameObject.SetActive(false);

            currentAppState = AppState.StartingSession;
            feedbackBox.text = "Trying to place an Object now. ";
            if (CloudManager.Session == null)
            {
                await CloudManager.CreateSessionAsync();
                feedbackBox.text += "Session created. ";
            }
            currentAnchorId = "";
            currentCloudAnchor = null;
            ConfigureSession();

            currentAppState = AppState.PlacingAnchor;

            await CloudManager.StartSessionAsync();
            feedbackBox.text += "Session started. ";

            finishButton.gameObject.SetActive(true);
            currentAppState = AppState.PlacingAnchor;
        }

        public async void PlacingObjects()
        {
            currentAppState = AppState.StoppingSession;
            finishButton.gameObject.SetActive(false);

            if (spawnedObject != null)
            {
                await SaveCurrentObjectAnchorToCloudAsync();
            }

            CloudManager.StopSession();
            CleanupSpawnedObjects();
            feedbackBox.text += "Session beendet. ";

            placingButton.gameObject.SetActive(true);
            lookingButton.gameObject.SetActive(true);

            currentAppState = AppState.Default;
        }

        public async void DeletingAnchor()
        {
            if (currentCloudAnchor != null)
            {
                await CloudManager.DeleteAnchorAsync(currentCloudAnchor);
                feedbackBox.text += "Anchor deleted.";
            }
            else
            {
                feedbackBox.text += "No Anchor found to delete.";
            }

            CloudManager.StopSession();
            currentWatcher = null;
            currentCloudAnchor = null;
            CleanupSpawnedObjects();
            feedbackBox.text += "Session beendet. ";


            placingButton.gameObject.SetActive(true);
            deletingButton.gameObject.SetActive(false);
            lookingButton.gameObject.SetActive(true);

            currentAppState = AppState.Default;

        }

        /*public void setIdText()
        {
            if (idInputField.interactable)
            {
                return;
            }
            idInputField.interactable = true;
        }*/

        public async void LookingForObject()
        {
            /*if (!idInputField.interactable)
            {
                return;
            }*/

            placingButton.gameObject.SetActive(false);
            lookingButton.gameObject.SetActive(false);

            currentAppState = AppState.LookingForAnchor;
            feedbackBox.text = "Trying to look for an Object now. ";
            //string idText = idInputField.text.ToString();

            //feedbackBox.text += "Eingegeben ID: " + idText + ". ";
            //idInputField.interactable = false;

            if (CloudManager.Session == null)
            {
                await CloudManager.CreateSessionAsync();
                feedbackBox.text += "Session created. ";
            }

            currentAnchorId = "";
            currentCloudAnchor = null;
            //ConfigureSession ();

            await CloudManager.StartSessionAsync();
            feedbackBox.text += "Session started. ";

            // Watching Part

            SetIdCriteria(anchorList.ToArray());
            feedbackBox.text += "Kriterien erstellt: " + anchorLocateCriteria.Identifiers + ". Session: " + CloudManager.Session + ". ";
            currentWatcher = CreateWatcher();
            feedbackBox.text += "Watcher erstellt. ";

        }

        public async void StoreAllAnchorKeys()
        {
            long _anchorNumber = 0;
#if !UNITY_EDITOR
            while (!string.IsNullOrWhiteSpace(await anchorExchanger.RetrieveAnchorKey(_anchorNumber)))
            {
                anchorList.Add(await anchorExchanger.RetrieveAnchorKey(_anchorNumber));
                _anchorNumber++;
            }

            feedbackBoxExtra.text += "Alle Keys gespeichert! Anzahl: + "+anchorList.Count + ". ";
#endif
        }

        protected override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
        {
            base.OnCloudAnchorLocated(args);

            currentAppState = AppState.SavingAnchor;
            feedbackBox.text += "Anker gefunden.";
            deletingButton.gameObject.SetActive(true);

            if (args.Status == LocateAnchorStatus.Located)
            {
                currentCloudAnchor = args.Anchor;

                if (currentCloudAnchor.AppProperties["name"] == "Default Name")
                {
                    feedbackBox.text += "Der Name des gefundenen Anchors ist " + currentCloudAnchor.AppProperties[@"name"] + ".";
                }

                UnityDispatcher.InvokeOnAppThread(() =>
                {
                    //currentAppState = AppState.DemoStepDeleteFoundAnchor;
                    Pose anchorPose = Pose.identity;

#if UNITY_ANDROID || UNITY_IOS
                    anchorPose = currentCloudAnchor.GetPose();
#endif
                    // HoloLens: The position will be set based on the unityARUserAnchor that was located.
                    SpawnOrMoveCurrentAnchoredObject(anchorPose.position, anchorPose.rotation);

                });
            }
        }

        public async override Task AdvanceDemoAsync() {

        }

        #endregion Override Methods

        private void ConfigureSession()
        {
            List<string> anchorsToFind = new List<string>();
            if (currentAppState == AppState.PlacingAnchor)
            {
                anchorsToFind.Add(currentAnchorId);
            }

            SetAnchorIdsToLocate(anchorsToFind);
        }

        public string BaseSharingUrl { get => baseSharingUrl; set => baseSharingUrl = value; }

    }
}