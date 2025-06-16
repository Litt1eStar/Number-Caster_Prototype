using UnityEngine;

public class Match : MonoBehaviour
{
    ClassSO[] classes;

    public void Init()
    {
        LoadMockupClassData();
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

    private void LoadMockupClassData()
    {
        classes = Resources.LoadAll<ClassSO>("Data/ClassSO");
    }
}
