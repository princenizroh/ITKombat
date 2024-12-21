using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour {


    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;


    public static DeliveryManager Instance { get; private set; }


    [SerializeField] private RecipeListSO recipeListSO;


    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipesAmount;


    private void Awake() {
        Instance = this;


        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {
        if (!IsServer) {
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                SpawnNewWaitingRecipe(waitingRecipeSOIndex);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex) {
        if (IsServer) return;
        SpawnNewWaitingRecipe(waitingRecipeSOIndex);
    }

    private void SpawnNewWaitingRecipe(int waitingRecipeSOIndex) {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                // Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    // Cycling through all ingredients in the Recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        // Cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            // Ingredient matches!
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound) {
                        // This Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe) {
                    // Player delivered the correct recipe!
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }

        // No matches found!
        // Player did not deliver a correct recipe
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc() {
        DeliverIncorrectRecipe();
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc() {
        if (IsServer) return;
        DeliverIncorrectRecipe();
    }

    private void DeliverIncorrectRecipe() {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex) {
        DeliverCorrectRecipe(waitingRecipeSOListIndex);
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex) {
        if (IsServer) return;
        DeliverCorrectRecipe(waitingRecipeSOListIndex);
    }

    private void DeliverCorrectRecipe(int waitingRecipeSOListIndex) {
        successfulRecipesAmount++;

        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }


    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }

}
