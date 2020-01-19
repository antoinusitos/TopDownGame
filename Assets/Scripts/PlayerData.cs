using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private int myLife = 100;
    private int myMaxLife = 100;
    private int myMana = 100;
    private int myMaxMana = 100;

    private bool isAlive = true;

    public int GetCurrentLife()
    {
        return myLife;
    }

    public int GetMaxLife()
    {
        return myMaxLife;
    }

    public int GetCurrentMana()
    {
        return myMana;
    }

    public int GetMaxMana()
    {
        return myMaxMana;
    }

    public void SetLife(int aValue)
    {
        myLife = Mathf.Clamp(aValue, 0, myMaxLife);
    }

    public void SetMana(int aValue)
    {
        myMana = Mathf.Clamp(aValue, 0, myMaxMana);
    }

    public float GetLifeRatio()
    {
        return myLife / (float)myMaxLife;
    }

    public float GetManaRatio()
    {
        return myMana / (float)myMaxMana;
    }

    public void AddLife(int aValue)
    {
        myLife = Mathf.Clamp(myLife + aValue, 0, myMaxLife);
    }

    public void RemoveLife(int aValue)
    {
        myLife = Mathf.Clamp(myLife - aValue, 0, myMaxLife);
    }

    public void AddMana(int aValue)
    {
        myMana = Mathf.Clamp(myMana + aValue, 0, myMaxMana);
    }

    public void RemoveMana(int aValue)
    {
        myMana = Mathf.Clamp(myMana - aValue, 0, myMaxMana);
    }
}
