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

        deckSO.InitialDeck();
    }

    public virtual void SetUI() { }
    public void IncreaseMaxMana()
    {
        currentMaxMana += 1;
        currentMana = currentMaxMana;

        GameManager.Instance.boardUI.AddManaToContainer(currentMaxMana);
    }
    public void TakeDamage(int damage)
    {
        if (ARMOR < 0)
        {
            if (HP - damage > 0)
            {
                HP -= damage;
                Debug.Log($"{this.name} Got hit took {damage} damage. Remaining HP: {HP}");
            }
            else
            {
                HP = 0;
                Debug.Log($"{this.name} has been defeated!");
            }
        }
        else
        {
            //damage -> 3
            //ARMOR -> 2
            //damageToArmor = 2
            //remainingDamage = damage - damageToArmor = 1
            int damageToArmor = Mathf.Min(ARMOR, damage); 
            int remainingDamage = damage - damageToArmor;
            ARMOR -= damageToArmor;
            if(HP - remainingDamage > 0)
            {
                HP -= remainingDamage;
                Debug.Log($"{this.name} Got hit took {damage} damage. Remaining HP: {HP}");
            }
            else
            {
                HP = 0;
                Debug.Log($"{this.name} has been defeated!");
            }
        }
    }

    public void IncreaseShield(int value)
    {
        ARMOR += value;
        Debug.Log($"{this.name} gained {value} armor. Total Armor: {ARMOR}");
    }
    public void UseCard(int mana)
    {
        if(currentMana - mana >= 0)
        {
            currentMana -= mana;
            GameManager.Instance.boardUI.UpdateManavVisual();
        }

        Debug.Log("Mana Left : " + currentMana);
    }
}
