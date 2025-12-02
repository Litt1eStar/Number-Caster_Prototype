using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Scriptable Objects/DeckSO")]
public class DeckSO : ScriptableObject
{
    public List<GameObject> cards;
    public int amountOfNumberCards = 22;
    public int amountOfOperatorCards = 4;
    public int amountOfSkillCards = 4;  
    public int amountOfBaseTwoTen = 13; 
    public int amountOfBaseSixteen = 5;

    public void InitialDeck()
    {
        //Init Deck with Card Lvl1
        //Have size of 30
        //Number cards for 18, Base 2, 10 => 13 Cards | Base 16 => 5 Cards
        //Operator cards for 8
        //Skill cards for 4
        cards = new List<GameObject>();

        GameObject[] numberCardsCatalog = Resources.LoadAll<GameObject>("Prefab/Card/Level1/Number");
        GameObject[] operatorCardsCatalog = Resources.LoadAll<GameObject>("Prefab/Card/Level1/Operator");
        GameObject[] skillCardsCatalog = Resources.LoadAll<GameObject>("Prefab/Card/Level1/Skill");
        List<string> baseSixteenValues = new List<string> { "A", "B", "C", "D", "E", "F" };
        List<GameObject> baseTwoTen = new List<GameObject>();
        List<GameObject> baseSixteen = new List<GameObject>();

        foreach (GameObject card in numberCardsCatalog)
        {
            string lastTerm = card.gameObject.name.Split('_').Last();
            if (baseSixteenValues.Contains(lastTerm))
            {
                baseSixteen.Add(card);
                Debug.Log(lastTerm);
            }
            else
            {
                baseTwoTen.Add(card);
            }
        }

        for(int i = 0; i < amountOfBaseTwoTen; i++)
        {
            int randomIndex = Random.Range(0, baseTwoTen.Count);
            cards.Add(baseTwoTen[randomIndex]);   
        }

        for(int i = 0; i < amountOfBaseSixteen; i++)
        {
            int randomIndex = Random.Range(0, baseSixteen.Count);
            cards.Add(baseSixteen[randomIndex]);
        }

        for(int i = 0; i < amountOfOperatorCards; i++)
        {
            int randomIndex = Random.Range(0, operatorCardsCatalog.Length);
            GameObject operatorCard = operatorCardsCatalog[randomIndex];
            cards.Add(operatorCard);
        }

        for(int i = 0; i < amountOfSkillCards; i++)
        {
            int randomIndex = Random.Range(0, skillCardsCatalog.Length);
            GameObject skillCard = skillCardsCatalog[randomIndex];
            cards.Add(skillCard);
        }

        List<GameObject> shuffledList = cards.OrderBy(card => Random.value).ToList();
        cards = shuffledList;
    }
}
