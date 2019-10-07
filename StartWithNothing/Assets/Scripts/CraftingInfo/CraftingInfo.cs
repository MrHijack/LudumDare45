using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingInfo : MonoBehaviour
{
    private const string OPEN_ID = "Open";
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform scrollContent;
    [SerializeField] private CraftingRecipes recipes;
    [SerializeField] private GameObject recipeDisplayPrefab;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color notAvailableColor;

    private readonly List<RecipeDisplay> recipeDisplays = new List<RecipeDisplay>();

    public bool Busy = false;
    public bool Open { get; set; }

    void Start()
    {
        var contentHeight = (recipes.Recipes.Length * 60) + ((recipes.Recipes.Length - 1) * 5);
        var size = scrollContent.sizeDelta;
        size.y = contentHeight;
        scrollContent.sizeDelta = size;

        for (int index = 0; index < recipes.Recipes.Length; index++)
        {
            var recipeDisplayObj = Instantiate(recipeDisplayPrefab, scrollContent);
            var recipeDisplay = recipeDisplayObj.GetComponent<RecipeDisplay>();
            recipeDisplay.Recipe = recipes.Recipes[index];
            recipeDisplays.Add(recipeDisplay);
            recipeDisplay.Crafting = this;
        }
    }

    public void Toggle()
    {
        if (!Busy)
        {
            if (Open)
            {
                CloseInfo();
            }
            else
            {
                OpenInfo();
            }
        }
    }

    public void ForceClose()
    {
        Open = false;
        animator.SetBool("ForceClose", true);
        animator.SetBool(OPEN_ID, false);
    }

    public void OnOpen()
    {
        animator.SetBool("ForceClose", false);
        RecalculateQuantities();
    }

    private void OpenInfo()
    {
        Open = true;
        animator.SetBool(OPEN_ID, true);
    }

    private void CloseInfo()
    {
        Open = false;
        animator.SetBool(OPEN_ID, false);
    }

    private void RecalculateQuantities()
    {
        recipeDisplays.ForEach(RecalculateQuantities);
    }

    private void RecalculateQuantities(RecipeDisplay recipeDisplay)
    {
        bool canCraft = inventory.CanPickup(recipeDisplay.Result.Item);
        recipeDisplay.Result.TextColor = canCraft ? availableColor : notAvailableColor;
        foreach (var ingredient in recipeDisplay.Ingredients)
        {
            var sufficientQuantity = inventory.SufficientQuantity(ingredient.Item);
            ingredient.TextColor = sufficientQuantity ? availableColor : notAvailableColor;
            canCraft &= sufficientQuantity;
        }
        recipeDisplay.CanCraft = canCraft;
    }

    public void Craft(RecipeDisplay recipeDisplay)
    {
        if (recipeDisplay.CanCraft)
        {
            foreach (var ingredient in recipeDisplay.Ingredients)
            {
                inventory.RemoveItem(ingredient.Item);
            }
            inventory.TryPickupItem(recipeDisplay.Result.Item);
            RecalculateQuantities();
        }
    }
}
