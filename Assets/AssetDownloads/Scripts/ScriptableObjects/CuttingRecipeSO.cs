using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject {


    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;


}