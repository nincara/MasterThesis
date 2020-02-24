// Copyright (c) Corinna Braun.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.Azure.SpatialAnchors.Unity
{
    public class AnchorData : MonoBehaviour
    {
        private string anchorKey, anchorName, anchorId, anchorDate, anchorGenerateMilliseconds;
        private string anchorDescription, anchorInfo, anchorProgress, anchorFeaturePoints; 
        private string anchorPosition, anchorRotation, anchorPositionLocalization, anchorRotationLocalization;

        public string AnchorKey { get => anchorKey; set => anchorKey = value; }
        public string AnchorName { get => anchorName; set => anchorName = value; }
        public string AnchorId { get => anchorId; set => anchorId = value; }
        public string AnchorDate { get => anchorDate; set => anchorDate = value; }
        public string AnchorInfo { get => anchorInfo; set => anchorInfo = value; }
        public string AnchorProgress{ get => anchorProgress; set => anchorProgress = value; }
        public string AnchorGenerateMilliseconds{ get => anchorGenerateMilliseconds; set => anchorGenerateMilliseconds = value; }
        public string AnchorFeaturePoints{ get => anchorFeaturePoints; set => anchorFeaturePoints = value; }
        public string AnchorDescription { get => anchorDescription; set => anchorDescription = value; }
        public string AnchorPosition { get => anchorPosition; set => anchorPosition = value; }
        public string AnchorRotation { get => anchorRotation; set => anchorRotation = value; }
        public string AnchorPositionLocalization { get => anchorPositionLocalization; set => anchorPositionLocalization = value; }
        public string AnchorRotationLocalization { get => anchorRotationLocalization; set => anchorRotationLocalization = value; }
    }
}