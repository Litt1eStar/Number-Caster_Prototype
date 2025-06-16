using UnityEngine;

public enum Turn
{
    PLAYER,
    ENEMY
}
public class Player : Entity
{
    public override void SetUI()
    {
        base.SetUI();
        GameManager.Instance.boardUI.SetPlayerUI();
    }
}
