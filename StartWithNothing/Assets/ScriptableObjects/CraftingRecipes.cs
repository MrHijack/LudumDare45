using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipes", menuName = "ScriptableObjets/CraftingRecipes")]
public class CraftingRecipes : ScriptableObject
{
    public CraftingRecipe[] Recipes;
}

[Serializable]
public class CraftingRecipe
{
    public string Name;
    public CraftingIngredient[] Ingredients;
    public CraftingResult Result;
}

[Serializable]
public class CraftingIngredient
{
    public ItemAsset Ingredient;
    public int Amount;
}

[Serializable]
public class CraftingResult
{
    public ItemAsset Item;
    public int Amount;
}
