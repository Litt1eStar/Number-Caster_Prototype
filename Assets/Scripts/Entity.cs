using UnityEngine;

public class Entity : MonoBehaviour
{
    public ClassSO classSO;
    public int HP = 20;
    public int ARMOR = 0;

    public void SetClass(ClassSO _classSO)
    {
        classSO = _classSO;
    }
}
