using Google.Cloud.Firestore;

namespace LabLog.Services;

public class FirebaseService
{
    // Este servicio es el puente entre nuestra app y Firestore

    private readonly FirestoreDb _firestoreDb;

    public FirebaseService()
    {
        // Ruta al archivo de credenciales
        var credentialPath = Path.Combine(
            AppContext.BaseDirectory,
            "Config",
            "firebase-credentials.json"
        );

        // Variable que utiliza el SDK de Google
        Environment.SetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS",
            credentialPath
        );

        // Project ID de Firebase
        _firestoreDb = FirestoreDb.Create("lablog-33ab1");
    }

    // Devuelve una colección de Firestore
    public CollectionReference GetCollection(string collectionName)
    {
        return _firestoreDb.Collection(collectionName);
    }
}