using UnityEngine;

public class Player : Entity
{
    public override void SetUI()
    {
        base.SetUI();
        GameManager.Instance.boardUI.SetPlayerUI(classSO, HP, ARMOR);
        GameManager.Instance.boardUI.InitDeckOnBoard(deckSO, Turn.PLAYER);
    }
}
