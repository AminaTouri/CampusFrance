using CampusFrance.Test;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class UserDataLoader
{
    public static List<UserRegistrationData> LoadFromJson(string cheminFichier)
    {
        // Lire tout le contenu du fichier JSON
        string json = File.ReadAllText(cheminFichier);

        // Désérialiser le JSON en liste d'utilisateurs
        List<UserRegistrationData> utilisateurs = JsonSerializer.Deserialize<List<UserRegistrationData>>(json);

        return utilisateurs;
    }
}
