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
    public void TakeDamage(int damage)
    {
        if (HP - damage > 0)
        {
            HP -= damage;
            Debug.Log($"Got hit {this.name} took {damage} damage. Remaining HP: {HP}");
        }
        else
        {
            HP = 0;
            Debug.Log($"{this.name} has been defeated!");
            // Handle defeat logic here, e.g., end the game or trigger a defeat animation
        }
    }
}
