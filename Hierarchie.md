#Hierarchie
##Camera Parent
    - Script: XR Camera Picker
        Prefabs:
        > HoloLensCamera
            - Spatial Mapping Collider: https://docs.unity3d.com/Manual/SpatialMappingCollider.html
            Erlaubt holographischem Inhalt mit realen psysikalischen Oberflächen zu interagieren
            --> Creating, updating und destroying von Oberflächen (GameObject Colliders)  
        > ARFoundationSessionStack
            - AR Session
            - AR Input Manager
            - AR Session Origin
                -- AR Camera (Camera-Objekt im Prefab ARFoundationSessionStack)
                    --- Tracked Pose Driver: https://docs.unity3d.com/2018.3/Documentation/ScriptReference/SpatialTracking.TrackedPoseDriver.html 
                    --- AR Camera Manager
                    --- AR Camera Background
            - AR Raycast Manager
            - AR Reference Point Manager
            - AR Plane Manager (Detection Mode: Nothing, Everything, Horizontal, Vertical)
                -- ARPlaneVisualizer
                    --- AR Plane
                    --- AR Plane Mesh Visualizer
                    --- Line Renderer
            - AR Point Cloud Manager
        > DefaultCamera

Das Prefab CameraParent (Camera Prefab) ist zur Erkennung der Oberflächen und für die AR Kamera vom Handy


##AzureSpatialAnchors
    - Script: Azure Handling (Demo ect)
    - Script: Spatial Anchors Manager

##EventSystem

##UX Parent

##Cursor
