using UnityEngine;

public class Match : MonoBehaviour
{
    ClassSO[] classes;

    ClassSO playerClass = null;
    ClassSO enemyClass = null;
    
    public void Init() //Init would have to recieve player class, enemy class, player deck
    {
        LoadMockupClassData();

        playerClass = classes[0]; // AAssuming the first class is for the player
        enemyClass = classes[1]; // Assuming the second class is for the enemy

        InitPlayerData(playerClass);
        InitEnemyData(enemyClass);
    }

    private void InitPlayerData(ClassSO _playerClass)
    {
        Player player = new GameObject("Player").AddComponent<Player>();
        player.SetClass(_playerClass);
    }

    private void InitEnemyData(ClassSO _enemyClass)
    {
        Enemy enemy = new GameObject("Enemy").AddComponent<Enemy>();
        enemy.SetClass(_enemyClass);
    }

    private void LoadMockupClassData()
    {
        classes = Resources.LoadAll<ClassSO>("Data/ClassSO");
        if (classes.Length == 0)
        {
            Debug.LogError("No ClassSO data found in Resources/Data/ClassSO.");
            return; 
        }

    }
}
