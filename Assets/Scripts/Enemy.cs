using UnityEngine;

public class Enemy : Entity
{
    public override void SetUI()
    {
        base.SetUI();
        GameManager.Instance.boardUI.SetEnemyUI();
    }
}
