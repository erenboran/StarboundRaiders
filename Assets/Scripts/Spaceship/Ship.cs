using UnityEngine;

[CreateAssetMenu(fileName = "NewShip", menuName = "Ship")]
public class Ship : ScriptableObject
{
    public string shipName;
    public int price;
    public bool isUnlocked;
    public string shipDamage;
    public string shipHealth;
    public string shipFireRate;

    public Sprite shipSprite; // UI'da butonda görünecek resim
    public GameObject shipPrefab; // Oyunda spawn olacak model
}
