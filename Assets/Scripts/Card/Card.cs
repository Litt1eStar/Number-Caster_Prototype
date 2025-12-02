using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardSO cardData;
   
    private Turn owner;
    private float rotateSpeed;
    public void SetOwner(Turn newOwner, float rotateSpeed)
    {
        owner = newOwner;
        FlipCard(rotateSpeed);
    }

    public void FlipCard(float rotateSpeed = 0.75f)
    {
        Vector3 cardRotation = Vector3.zero;
        this.rotateSpeed = rotateSpeed;
        if (owner == Turn.PLAYER)
        {
            cardRotation = new Vector3(0, 0, 180);
        }
        else if(owner == Turn.ENEMY)
        {
            cardRotation = new Vector3(0, 0, 0);
        }

        this.transform.DOLocalRotate(cardRotation, this.rotateSpeed).SetEase(Ease.OutQuart);
    }

    public void FlipCardToAnotherSide()
    {
        if(owner == Turn.ENEMY)
        {
            this.transform.DOLocalRotate(new Vector3(0, 0, -90), rotateSpeed).SetEase(Ease.OutQuart);
        }
    }
}
