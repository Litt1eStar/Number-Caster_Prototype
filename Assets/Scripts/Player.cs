using UnityEngine;

public enum PlayerSide
{
    Player1,
    Player2 
}
public class Player : MonoBehaviour
{
    public Transform handParent;
    public PlayerSide playerSide;
}
