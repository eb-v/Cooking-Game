using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public interface IFlammable {
    bool IsOnFire { get; set; }
    float BurnProgress { get; }


    GameObject FlamableGameObject { get; }

    List<FireController> FireEffects { get; }

    void Ignite();
    void Extinguish();

    void ModifyBurnProgress(float amount);

    void OnFireLogic();
}