﻿using System;
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
        private Button placingButton, finishButton, lookingButton, deletingButton;


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


            Debug.Log("Azure Spatial Anchors Demo script started");
        }

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

        public void setIdText()
        {
            if (idInputField.interactable)
            {
                return;
            }
            idInputField.interactable = true;
        }

        public async void LookingForObject()
        {
            if (!idInputField.interactable)
            {
                return;
            }

            placingButton.gameObject.SetActive(false);
            lookingButton.gameObject.SetActive(false);
            
            currentAppState = AppState.LookingForAnchor;
            feedbackBox.text = "Trying to look for an Object now. ";
            string idText = idInputField.text.ToString();
            
            feedbackBox.text += "Eingegeben ID: " + idText + ". ";
            idInputField.interactable = false;

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

            SetIdCriteria(idText);
            feedbackBox.text += "Kriterien erstellt: " + anchorLocateCriteria.Identifiers + ". Session: " + CloudManager.Session + ". ";
            currentWatcher = CreateWatcher();
            feedbackBox.text += "Watcher erstellt. ";

            /*CloudManager.Session.AnchorLocated += (object sender, AnchorLocatedEventArgs args) => {
                switch (args.Status) {
                    case LocateAnchorStatus.Located:
                        CloudSpatialAnchor foundAnchor = args.Anchor;
                        feedbackBox.text += "Anker gefunden! ";
                        break;
                    case LocateAnchorStatus.AlreadyTracked:
                        // This anchor has already been reported and is being tracked
                        break;
                    case LocateAnchorStatus.NotLocatedAnchorDoesNotExist:
                        // The anchor was deleted or never existed in the first place
                        // Drop it, or show UI to ask user to anchor the content anew
                        break;
                    case LocateAnchorStatus.NotLocated:
                        // The anchor hasn't been found given the location data
                        // The user might in the wrong location, or maybe more data will help
                        // Show UI to tell user to keep looking around
                        feedbackBox.text += "Anker nicht gefunden! ";
                        break;
                }
            };*/

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

        public void StartingApp()
        {

        }
        public async override Task AdvanceDemoAsync()
        {
            switch (currentAppState)
            {

                case AppState.PlacingAnchor:

                    break;
                case AppState.LookingForAnchor:

                    break;
                default:
                    break;
            }
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

    }
}