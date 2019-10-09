# SpatialAnchorsManager
Das Skript *"SpatialAnchorManager"* besteht aus 9 Regionen.

0. Zu Beginn wird eine **enum-Liste** erzeugt mit dem Modus der Authentifikation.  
*(Das Schlüsselwort enum wird zum Deklarieren einer Enumeration verwendet. 
Dies ist ein eigener Typ, der aus einer Gruppe benannter Konstanten besteht, 
die Enumeratorliste genannt wird.)*
Diese Liste besteht aus den Modi *"ApiKey"* (Account ID and Account Key) 
und *"AAD"* (Azure Active Directory)

1. Member Variables
    * Variablen für die Session
      * **bool**: *isSessionStarted* = false
      * **CloudSpatialAnchorSession**: *session* = null
      * **SessionStatus**: *sessionStatus* = null
     

2. Unity Inspector Variables
  * Variablen, welche im Unity-Inspector eingegeben und eingestellt werden können, dazu zählen:
    * **AuthenticationMode**: authenticationMode = AuthenticationMode.ApiKey
    * **string**: spatialAnchorsAccountId
    * **string**: spatialAnchorsAccountKey
    * **string**: clientId
    * **string**: tenantId
    * **SessionLogLevel**: logLevel = SessionLogLevel.All
    
3. Internal Methods
  * Interne Methoden 
    * **void**: EnsureSessionStarted()
    * **async Task<bool>**: EnsureValidConfiguration(**bool** disable, **bool** exception)
    * **void**: ProcessLatestFrame():
    * **internal static ARReferencePoint**: ReferencePointFromPointer(**IntPtr** intPtr)
    * **void**: ProcessPendingEventArgs()

4. Overridables
  * Diese Methoden sind alle virtual und können überschrieben werden. 
    * **virtual async Task<string>**: GetAADTokenAsync()
    _Diese Methode ist auskommentiert, da sie nur bei der Verwendung von AAD Anwendung findet._
    * **virtual void**: LoadConfiguration
    * **async virtual Task<bool>**: IsValidateConfiguration()
    * **virtual void**: OnAnchorLocated(**object** sender, **AnchorsLocatedEventArgs** args)
    * **virtual void**: OnError(**object** sender, **SessionErrorEventArgs** args)
    * **virtual void**: OnLocateAnchorsCompleted(**object** sender, **LocateAnchorsCompletedEventArgs** args)
    * **virtual void**: OnLogDebug(**object** sender, **OnLogDebugEventArgs** args)
    * **virtual void**: OnSessionChanged()
    * **virtual void** OnSessionCreated()
    * **virtual void** OnSessionDestroyed()
    * **virtual void**: OnSessionStarted()
    * **virtual void**: OnSessionStopped()
    * **virtual void**: OnSessionUpdated(**object** sender, **SessionUpdatedEventArgs** args)

5. Event Handlers
  * bla
    * **async void**: Session_TokenRequired(**object** sender, **TokenRequiredEventArgs** args)
    * **void**: ARReferencePointManager_referendePointsChanged (**ARReferencePointsChangedEventArgs** obj)
    * **void**: ArCameraManager_frameReceived(**ARCameraFrameEventArgs** obj)
6. Unity Overrides 
7. Public Methods
8. Public Properties
9. Public Events