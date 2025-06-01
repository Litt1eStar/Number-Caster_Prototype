using UnityEngine;

public enum Turn
{
    Player1,
    Player2 
}
public class Player : MonoBehaviour
{
    public Transform handParent;
    public Turn playerSide;
}
