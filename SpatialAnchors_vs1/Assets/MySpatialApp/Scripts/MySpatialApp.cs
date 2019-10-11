using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;

namespace Microsoft.Azure.SpatialAnchors.Unity
{

    public class MySpatialApp : MyAppBase
    {
        internal enum AppState
        {
            Default = 0,
            PlacingAnchor,
            LookingForAnchor
        }

        private readonly Dictionary<AppState, DemoStepParams> stateParams = new Dictionary<AppState, DemoStepParams>
        {
            { AppState.Default,new DemoStepParams() { StepMessage = "Choose an Option."}},
            { AppState.PlacingAnchor,new DemoStepParams() { StepMessage = "Place an Anchor."}},
            { AppState.LookingForAnchor,new DemoStepParams() { StepMessage = "Looking for Anchors."}}
        };
        private AppState _currentAppState = AppState.Default;

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

                    /* if (spawnedObjectMat != null)
                    {
                        spawnedObjectMat.color = stateParams[_currentAppState].StepColor;
                    } */

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

            Debug.Log("Azure Spatial Anchors Demo script started");
        }

        // Update is called once per frame
        void Update()
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
                //spawnedObjectMat.color = GetStepColor() * rat;
            }
        }

        #region Override Methods

        protected override bool IsPlacingObject()
        {
            return currentAppState == AppState.PlacingAnchor;
        }

        /* protected override Color GetStepColor()
        {
            return stateParams[currentAppState].StepColor;
        } */

        protected override async Task OnSaveCloudAnchorSuccessfulAsync()
        {
            await base.OnSaveCloudAnchorSuccessfulAsync();

            Debug.Log("Anchor created, yay!");

            currentAnchorId = currentCloudAnchor.Identifier;

            // Sanity check that the object is still where we expect
            Pose anchorPose = Pose.identity;

            #if UNITY_ANDROID || UNITY_IOS
            anchorPose = currentCloudAnchor.GetPose();
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
        public async void PlacingObjects()
        {
            
            currentAppState = AppState.PlacingAnchor;
            feedbackBox.text = "Trying to place an Object now.";
            if (CloudManager.Session == null)
            {
                await CloudManager.CreateSessionAsync();
                feedbackBox.text += "Session created.";
            }
            currentAnchorId = "";
            currentCloudAnchor = null;
            ConfigureSession();

            await CloudManager.StartSessionAsync();
            feedbackBox.text += "Session started.";

            if (spawnedObject != null)
            {
                await SaveCurrentObjectAnchorToCloudAsync();
            }

            CloudManager.StopSession();
            feedbackBox.text += "Session beendet.";
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