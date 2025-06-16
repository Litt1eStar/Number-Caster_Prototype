using UnityEngine;

public class Match : MonoBehaviour
{
    public void Init()
    {
        InitPlayerData();
        InitEnemyData();
    }

    private void InitPlayerData()
    {
        Player player = new GameObject("Player").AddComponent<Player>();
        Debug.Log(player.name);
    }

    private void InitEnemyData()
    {
        Enemy enemy = new GameObject("Enemy").AddComponent<Enemy>();
        Debug.Log(enemy.name);
    }
}
