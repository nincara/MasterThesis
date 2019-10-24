# Anker erstellen:

1. Session erstellen, falls keine Session vorhanden
2. currentAnchorId und currentCloudAnchor auf empty setzen
3. Session configurieren 
4. Session wird gestartet --> Platzierung eines Ankers
5. Wurde Anker platziert, wird überprüft ob ein Objekt gespawned wurde, falls ja, wird der Anker in der Cloud gespeichert. Hier wird die ID gespeichert! 
6. Session wird gestoppt und alle Objekte werden lokal gelöscht