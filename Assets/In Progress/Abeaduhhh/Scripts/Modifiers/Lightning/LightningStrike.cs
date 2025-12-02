using UnityEngine;

public class LightningStrike : MonoBehaviour {
    public float strikeDistance = 200f;

    private void Start() {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, strikeDistance)) {
            Debug.Log("Lightning hit: " + hit.collider.name);

            Burnable burn = hit.collider.GetComponentInParent<Burnable>();

            if (burn != null) {
                burn.Ignite();
                Debug.Log("Burnable object ignited: " + hit.collider.name);

                //TriggerExplosion(hit.point);
            }
        } else {
            Debug.Log("Lightning hit nothing.");
        }
    }

    //private void TriggerExplosion(Vector3 explosionPos) {
    //    ExplosionSystem system = ExplosionSystem.Instance;

    //    var points = new System.Collections.Generic.List<Transform>();
    //    GameObject temp = new GameObject("TempExplosionPoint");
    //    temp.transform.position = explosionPos;
    //    points.Add(temp.transform);

    //    ExplosionData data = Resources.Load<ExplosionData>("Status Effects/Explosions/ExplosionData");

    //    if (data != null)
    //        ExplosionSystem.RunExplosionLogic(points, data);
    //    else
    //        Debug.LogWarning("ExplosionData asset not found!");

    //    Destroy(temp);
    //}

}
