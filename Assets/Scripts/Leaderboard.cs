using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    Dictionary<string, int> leaderboard = new Dictionary<string, int>()
    {
        { "Somsri", 2 },
        { "Suradech", 5 },
        { "Dywood", 4 },
    };

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
