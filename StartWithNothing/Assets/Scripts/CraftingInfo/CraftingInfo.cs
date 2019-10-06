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
}
