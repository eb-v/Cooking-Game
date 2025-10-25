using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerNumber;
    public int ingredientsHandled = 0;
    public int pointsGenerated = 0;
    public int teammatesRevived = 0;
    
    // Call this whenever player picks up/handles an ingredient
    public void IncrementIngredientsHandled()
    {
        ingredientsHandled++;
        Debug.Log($"Player {playerNumber} - Ingredients Handled: {ingredientsHandled}");
    }
    
    // Call this whenever player generates points (delivers dish)
    public void AddPoints(int points)
    {
        pointsGenerated += points;
        Debug.Log($"Player {playerNumber} - Total Points: {pointsGenerated}");
    }
    
    // Call this when player revives a teammate
    public void IncrementTeammatesRevived()
    {
        teammatesRevived++;
        Debug.Log($"Player {playerNumber} - Teammates Revived: {teammatesRevived}");
    }
    
    // Get reference to this player's character/prefab
    public GameObject GetPlayerPrefab()
    {
        return gameObject;
    }
    
    // Get all stats as a formatted string (useful for debugging)
    public string GetStatsString()
    {
        return $"Player {playerNumber}\nIngredients: {ingredientsHandled}\nPoints: {pointsGenerated}\nRevives: {teammatesRevived}";
    }
}