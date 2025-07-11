using UnityEngine;

public enum StageLevel
{
    Monster,
    MiniBoss,
    Boss,
}
public class PVE_StageButton : MonoBehaviour
{
    [SerializeField] private ClassSO enemyClass;
    [SerializeField] private bool isClear = false;
    [SerializeField] private StageLevel stageLevel;
    [SerializeField] private Sprite stageSprite;
}
