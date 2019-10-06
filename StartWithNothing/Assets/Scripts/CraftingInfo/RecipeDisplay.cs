using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDisplay : MonoBehaviour
{
    [SerializeField] private Transform ingredientsParent;
    [SerializeField] private ItemDisplay resultDisplay;
    [SerializeField] private GameObject itemDisplayPrefab;
    [SerializeField] private GameObject separatorPrefab;

    private CraftingRecipe _recipe;

    public CraftingRecipe Recipe
    {
        get => _recipe;
        set => SetRecipe(value);
    }

    private void SetRecipe(CraftingRecipe recipe)
    {
        _recipe = recipe;
        var result = ItemResolver.ResolveItem(recipe.Result.Item);
        result.Quantity = recipe.Result.Amount;
        resultDisplay.Item = result;

        ingredientsParent.gameObject.DestroyChildren();
        for (int index = 0; index < _recipe.Ingredients.Length; index++)
        {
            if (index > 0)
            {
                Instantiate(separatorPrefab, ingredientsParent);
            }
            var ingredientObj = Instantiate(itemDisplayPrefab, ingredientsParent);
            var ingredientDisplay = ingredientObj.GetComponent<ItemDisplay>();
            var ingredient = ItemResolver.ResolveItem(_recipe.Ingredients[index].Ingredient);
            ingredient.Quantity = _recipe.Ingredients[index].Amount;
            ingredientDisplay.Item = ingredient;
        }
    }
}
