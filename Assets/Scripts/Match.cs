using System.Collections;
using UnityEngine;

public class Match : MonoBehaviour
{
    ClassSO[] classes;
    DeckSO[] decks;

    ClassSO playerClass = null;
    ClassSO enemyClass = null;

    DeckSO playerDeck = null;
    DeckSO enemyDeck = null;
    
    public void Init(ClassSO _playerClass, ClassSO _enemyClass) //Init would have to recieve player class, enemy class, player deck
    {
        LoadMockupDataFromResources();

        playerClass = _playerClass; // AAssuming the first class is for the player
        enemyClass = _enemyClass; // Assuming the second class is for the enemy

        playerDeck = decks[1]; // Assuming the second deck is for the player
        enemyDeck = decks[0]; // Assuming the first deck is for the enemy

        InitPlayerData(playerClass, playerDeck);
        InitEnemyData(enemyClass, enemyDeck);

        TurnManager.Instance.InitTurnSystem();
    }
    

    private void InitPlayerData(ClassSO _playerClass, DeckSO _playerDeck)
    {
        Player player = new GameObject("Player").AddComponent<Player>();
        GameManager.Instance.SetPlayer(player);
        player.SetData(_playerClass, _playerDeck);
        player.SetUI();
    }

    private void InitEnemyData(ClassSO _enemyClass, DeckSO _enemyDeck)
    {
        Enemy enemy = new GameObject("Enemy").AddComponent<Enemy>();
        GameManager.Instance.SetEnemy(enemy);
        enemy.SetData(_enemyClass, _enemyDeck);
        enemy.SetUI();

    }

    private void LoadMockupDataFromResources()
    {
        classes = Resources.LoadAll<ClassSO>("Data/ClassSO");
        if (classes.Length == 0)
        {
            Debug.LogError("No ClassSO data found in Resources/Data/ClassSO.");
            return; 
        }

        decks = Resources.LoadAll<DeckSO>("Data/DeckSO");
        if (decks.Length == 0)
        {
            Debug.LogError("No DeckSO data found in Resources/Data/DeckSO.");
            return; 
        }
    }
}
