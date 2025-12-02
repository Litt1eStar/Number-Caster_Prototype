using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Instance { get; private set; }

    Dictionary<string, int> leaderboard = new Dictionary<string, int>()
    {
        { "Somsri", 2 },
        { "Suradech", 5 },
        { "Dywood", 4 },
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(string playerName, int score)
    {
        if (leaderboard.ContainsKey(playerName))
        {
            leaderboard[playerName] += score;
        }
        else
        {
            leaderboard[playerName] = score;
        }
    }
}
