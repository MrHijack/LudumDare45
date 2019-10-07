using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeDisplay : MonoBehaviour
{
    [SerializeField] private Transform ingredientsParent;
    [SerializeField] private ItemDisplay resultDisplay;
    [SerializeField] private GameObject itemDisplayPrefab;
    [SerializeField] private GameObject separatorPrefab;
    [SerializeField] private Button craftButton;

    private CraftingRecipe _recipe;
    private bool _canCraft;

    public List<ItemDisplay> Ingredients { get; } = new List<ItemDisplay>();
    public ItemDisplay Result => resultDisplay;
    public CraftingInfo Crafting { get; set; }

    public CraftingRecipe Recipe
    {
        get => _recipe;
        set => SetRecipe(value);
    }

    public bool CanCraft
    {
        get => _canCraft;
        set => SetCanCraft(value);
    }

    private void SetRecipe(CraftingRecipe recipe)
    {
        _recipe = recipe;
        var result = ItemResolver.ResolveItem(recipe.Result.Item);
        result.Quantity = recipe.Result.Amount;
        resultDisplay.Item = result;

        Ingredients.Clear();
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
            Ingredients.Add(ingredientDisplay);
        }
    }

    private void SetCanCraft(bool canCraft)
    {
        _canCraft = canCraft;
        craftButton.interactable = _canCraft;
    }

    public void Craft()
    {
        Crafting.Craft(this);
    }
}
