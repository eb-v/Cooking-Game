using UnityEngine;
using UnityEngine.UI;

public interface IOrder
{
    public MenuItem currentOrder { get; }

    public Image OrderImage { get; }

    public void SetOrder(MenuItem orderItem);

}
