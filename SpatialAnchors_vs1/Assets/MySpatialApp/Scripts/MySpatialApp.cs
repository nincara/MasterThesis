using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Microsoft.Azure.SpatialAnchors.Unity {

    public class MySpatialApp : MyAppBase 
    {
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