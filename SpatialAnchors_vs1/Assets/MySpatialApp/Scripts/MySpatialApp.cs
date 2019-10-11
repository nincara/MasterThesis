using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Microsoft.Azure.SpatialAnchors.Unity {

    public class MySpatialApp : MyAppBase 
    {
        internal enum AppState
        {
            InitSession = 0
        }

        private readonly Dictionary<AppState, DemoStepParams> stateParams = new Dictionary<AppState, DemoStepParams>
        {
            { AppState.InitSession,new DemoStepParams() { StepMessage = "Next: Create Azure Spatial Anchors Session", StepColor = Color.clear }},
            
        };

        public override void Start () {

           
        }

        // Update is called once per frame
        void Update () {

        }

        #region Override Methods

        protected override bool IsPlacingObject()
        {
            return currentAppState == AppState.DemoStepCreateLocalAnchor;
        }

        protected override Color GetStepColor()
        {
            return stateParams[currentAppState].StepColor;
        }

        public async override Task AdvanceDemoAsync()
        {
            switch (currentAppState)
            {
                default:
                break;
            }
        }
        #endregion Override Methods
        
    }
}