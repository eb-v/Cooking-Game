using UnityEngine;
using UnityEngine.UI;

public class AssemblyStationUI : MonoBehaviour
{
    [SerializeField] private AssemblyStation assemblyStation;
    [SerializeField] private Image menuItemDisplayImage; 

    private void Update()
    {
            if (assemblyStation == null)
        {
            Debug.LogError("AssemblyStation not assigned!");
            return;
        }
        
        if (menuItemDisplayImage == null)
        {
            Debug.LogError("Menu Item Display Image not assigned!");
            return;
        }

        if (assemblyStation.selectedMenuItem != null)
        {
            menuItemDisplayImage.sprite = assemblyStation.selectedMenuItem.GetFoodSprite();
        }
        else
        {
            Debug.LogWarning("No menu item selected!");
        }
    }
}