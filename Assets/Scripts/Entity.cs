using UnityEngine;

public class Entity : MonoBehaviour
{
    public ClassSO classSO;
    public DeckSO deckSO;
    public int HP = 20;
    public int ARMOR = 0;
    public int MAX_MANA = 10;
    public int currentMaxMana = 0;
    public int currentMana = 0;

    public void SetData(ClassSO _classSO, DeckSO _deckSO)
    {
        classSO = _classSO;
        deckSO = _deckSO;
    }

    public virtual void SetUI() { }
    public void IncreaseMaxMana()
    {
        currentMaxMana += 1;
        currentMana = currentMaxMana;
    }
}
