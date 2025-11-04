using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CosmeticChanger : MonoBehaviour
{
    public Material[] robotColors;
    public Sprite[] robotFaces;
    [SerializeField] private int selectedColorIndex = 0;
    [SerializeField] private int selectedFaceIndex = 0;

    public void ChangeRobotColor(GameObject player)
    {
        if (player == null)
            return;

        SkinnedMeshRenderer[] meshRenderers = player.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer renderer in meshRenderers)
        {
            if (renderer == null)
                continue; // Skip destroyed or missing renderers

            Material[] materials = renderer.materials;

            // Check if index is valid
            if (materials.Length > 1 && selectedColorIndex >= 0 && selectedColorIndex < robotColors.Length)
            {
                materials[1] = robotColors[selectedColorIndex];
                renderer.materials = materials;
            }
        }
    }

    public void ChangeRobotFace(GameObject player)
    {
        // get image compoenent from player's face
        if (player == null)
            return;

        Image faceImage = player.transform.Find("DEF_Pelvis/DEF_Body/DEF_Head/FaceCanvas/FaceImage").GetComponent<Image>();
        if (faceImage == null)
        {
            Debug.LogWarning("FaceImage component not found on player.");
            return;
        }

        faceImage.sprite = robotFaces[selectedFaceIndex];
    }


    public void NextColor()
    {
        selectedColorIndex++;
        if (selectedColorIndex >= robotColors.Length)
        {
            selectedColorIndex = 0;
        }
    }

    public void PreviousColor()
    {
        selectedColorIndex--;
        if (selectedColorIndex < 0)
        {
            selectedColorIndex = robotColors.Length - 1;
        }
    }

    public void NextFace()
    {
        selectedFaceIndex++;
        if (selectedFaceIndex >= robotFaces.Length)
        {
            selectedFaceIndex = 0;
        }
    }

    public void PreviousFace()
    {
        selectedFaceIndex--;
        if (selectedFaceIndex < 0)
        {
            selectedFaceIndex = robotFaces.Length - 1;
        }
    }
}