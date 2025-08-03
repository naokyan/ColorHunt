using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Proyecto26;
using System.IO;
using System;

public class FirebaseManager : MonoBehaviour
{
    public bool levelCleared;
    public double levelStartTime;

    private const string DatabaseUrl = "https://project-6115520849454422510-default-rtdb.firebaseio.com/";
    private string authToken;

    private void Awake()
    {
        levelStartTime = Time.time;
    }

    public void SendDataToDatabase(int restart, double time, int deathCount, int lightMixedCount,
        List<Vector2> restartPos, List<Vector2> deathPos, int enemyTrappedCount, int enemyKilledCount, List<Vector2> colorMixPos)
    {
        string currentLevel = SceneManager.GetActiveScene().name;

        // Create a user object with the data to send
        User user = new User(currentLevel, restart, time, deathCount, lightMixedCount, 
            restartPos, deathPos, enemyTrappedCount, enemyKilledCount, colorMixPos);

        // Get the current date and time and format it as a string for the user key
        string dateTimeNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        // Create the URL with the current date and time
        string url = $"{DatabaseUrl}BetaMilestoneBuild/{dateTimeNow}.json?auth={authToken}";

        RestClient.Put(url, user);
    }

    // Update is called once per frame
    void Update()
    {
        if (levelCleared)
        {
            int restartCount = PlayerPrefs.GetInt("RestartCount", 0);
            int deathCount = PlayerPrefs.GetInt("DeathCount", 0);
            int lightMixedCount = PlayerPrefs.GetInt("lightMixedCount", 0);
            int enemyTrappedCount = PlayerPrefs.GetInt("EnemyTrappedCount", 0);
            int enemyKilledCount = PlayerPrefs.GetInt("EnemyKilledCount", 0);
            
            List<Vector2> restartPos = DecodeStringToVector2List(PlayerPrefs.GetString("RestartLocation", "0"));
            List<Vector2> deathPos = DecodeStringToVector2List(PlayerPrefs.GetString("DeathLocation", "0"));
            List<Vector2> colorMixPos = DecodeStringToVector2List(PlayerPrefs.GetString("ColorMixedLocation", "0"));

            levelCleared = !levelCleared;
            double timeSpentOnLevel = Time.time - levelStartTime;
            string savedString = PlayerPrefs.GetString("TimeSpent", "0");
            double savedTimeSpent = double.Parse(savedString);

            SendDataToDatabase(restartCount, timeSpentOnLevel + savedTimeSpent, deathCount, 
                lightMixedCount, restartPos, deathPos, enemyTrappedCount, enemyKilledCount, colorMixPos);

            // Reset after data is sent, also reset in game manager main menu
            PlayerPrefs.SetInt("DeathCount", 0);//in game manager
            PlayerPrefs.Save();

            PlayerPrefs.SetInt("lightMixedCount", 0);//in light switch
            PlayerPrefs.Save();

            double zeroTime = 0.0;
            PlayerPrefs.SetString("TimeSpent", zeroTime.ToString());
            PlayerPrefs.Save();

            PlayerPrefs.SetInt("RestartCount", 0);//in game manager
            PlayerPrefs.Save();

            PlayerPrefs.SetString("RestartLocation", "");//in game manager
            PlayerPrefs.Save();

            PlayerPrefs.SetString("DeathLocation", "");//in player movement
            PlayerPrefs.Save();

            PlayerPrefs.SetInt("EnemyTrappedCount", 0);//in enemy patrol
            PlayerPrefs.Save();

            PlayerPrefs.SetInt("EnemyKilledCount", 0);//in light shades
            PlayerPrefs.Save();

            PlayerPrefs.SetString("ColorMixedLocation", "");//in light switch
            PlayerPrefs.Save();
        }
    }

    private List<Vector2> DecodeStringToVector2List(string vectorString)
    {
        List<Vector2> vectorList = new List<Vector2>();

        // Split the string by the semicolon separator
        string[] vectorParts = vectorString.Split(';');

        foreach (string part in vectorParts)
        {
            // Remove parentheses and split by comma
            string cleanPart = part.Trim('(', ')');
            string[] coordinates = cleanPart.Split(',');

            if (coordinates.Length == 2)
            {
                // Parse x and y values from the string
                if (float.TryParse(coordinates[0], out float x) && float.TryParse(coordinates[1], out float y))
                {
                    Vector2 vector = new Vector2(x, y);
                    vectorList.Add(vector);
                }
            }
        }

        return vectorList;
    }

    [System.Serializable]
    public class User
    {
        public string the_level_is;
        public double time_spent;
        public int times_of_death;
        public List<Vector2> death_locations;
        public int times_of_restart;
        public List<Vector2> restart_locations;
        public int times_of_light_mixed;
        public int times_of_enemy_trapped;
        public int times_of_enemy_killed;
        public List<Vector2> color_mix_locations;

        public User(string currentLevel, int restart, double time, int death, int lightMixed, 
            List<Vector2> restartPos, List<Vector2> deathPos, int enemyTrappedCount, int enemyKilledCount, List<Vector2> colorMixPos)
        {
            this.the_level_is = currentLevel;
            this.times_of_restart = restart;
            this.time_spent = time;
            this.times_of_death = death;
            this.times_of_light_mixed = lightMixed;
            this.restart_locations = restartPos;
            this.death_locations = deathPos;
            this.times_of_enemy_trapped = enemyTrappedCount;
            this.times_of_enemy_killed = enemyKilledCount;
            this.color_mix_locations = colorMixPos;
        }
    }
}
