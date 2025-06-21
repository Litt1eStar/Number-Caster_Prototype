using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardSO cardData;
   
    private Turn owner;
    private bool isOnTop = false;
    public void SetOwner(Turn newOwner, float rotateSpeed)
    {
        owner = newOwner;
        FlipCard(rotateSpeed);
    }

    public void FlipCard(float rotateSpeed)
    {
        Vector3 cardRotation = Vector3.zero;
        if (owner == Turn.PLAYER)
        {
            cardRotation = new Vector3(0, 0, 180);
        }
        else if(owner == Turn.ENEMY)
        {
            cardRotation = new Vector3(0, 0, 0);
        }

        this.transform.DOLocalRotate(cardRotation, rotateSpeed).SetEase(Ease.OutQuart);
    }

    public void FlipCardToAnotherSide()
    {

    }
}
