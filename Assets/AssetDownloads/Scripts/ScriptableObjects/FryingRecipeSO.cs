using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FryingRecipeSO : ScriptableObject {


    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimerMax;


}