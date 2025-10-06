using System.Collections.Generic;
using UnityEngine;

public class AssembledItemContainer : MonoBehaviour
{
    private AssembledItemObject assembledItemObject;

    public void SetAssembledItemObject(AssembledItemObject item)
    {
        assembledItemObject = item;
    }

    public AssembledItemObject GetAssembledItemObject()
    {
        return assembledItemObject;
    }
}
