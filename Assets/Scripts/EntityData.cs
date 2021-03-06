﻿using System.Collections.Generic;
using UnityEngine;

public class EntityData : MonoBehaviour
{
    public Quickslot[]  myQuickslots = null;

    protected int       myLife = 100;
    protected int       myMaxLife = 100;
    protected int       myMana = 100;
    protected int       myMaxMana = 100;

    protected List<Item> myInventory = new List<Item>();

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

    public void AddToInventory(Collectible aCollectible)
    {
        for(int i = 0; i < myInventory.Count; ++i)
        {
            if(myInventory[i].myID == aCollectible.myItem.myID)
            {
                Item item = myInventory[i];
                item.myQuantity += aCollectible.myItem.myQuantity;
                myInventory[i] = item;

                if (myQuickslots != null)
                {
                    for (int q = 0; q < myQuickslots.Length; q++)
                    {
                        if (myQuickslots[i].myItem.myID == item.myID)
                        {
                            myQuickslots[i].mySlotQuantity.text = item.myQuantity.ToString();
                            break;
                        }
                    }
                }

                return;
            }
        }

        myInventory.Add(aCollectible.myItem);
        if(myQuickslots != null)
        {
            for(int i = 0; i < myQuickslots.Length; i++)
            {
                if(!myQuickslots[i].myIsFilled)
                {
                    myQuickslots[i].mySlotImage.sprite = aCollectible.myItem.mySprite;
                    myQuickslots[i].mySlotQuantity.text = aCollectible.myItem.myQuantity.ToString();
                    myQuickslots[i].myIsFilled = true;
                    myQuickslots[i].myItem = aCollectible.myItem;
                    break;
                }
            }
        }
    }
}
