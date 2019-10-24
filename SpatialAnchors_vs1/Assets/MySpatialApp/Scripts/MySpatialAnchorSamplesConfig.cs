﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using UnityEngine;

namespace Microsoft.Azure.SpatialAnchors.Unity
{
    /// <summary>
    /// This menu item generates an optional configuration file which can be
    /// excluded from source control to avoid committing credentials there.
    /// </summary>
    [CreateAssetMenu(fileName = "SpatialAnchorSamplesConfig", menuName = "Azure Spatial Anchors/Samples Configuration")]
    public class MySpatialAnchorSamplesConfig : ScriptableObject
    {
        [Tooltip("The base URL for the example sharing service.")]
        public string BaseSharingURL = "";
    }
}
