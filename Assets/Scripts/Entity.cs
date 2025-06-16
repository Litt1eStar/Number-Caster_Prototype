using UnityEngine;

public class Entity : MonoBehaviour
{
    public ClassSO classSO;
    public DeckSO deckSO;
    public int HP = 20;
    public int ARMOR = 0;

    public void SetData(ClassSO _classSO, DeckSO _deckSO)
    {
        classSO = _classSO;
        deckSO = _deckSO;
    }
}
