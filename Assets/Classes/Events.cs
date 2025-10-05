using UnityEngine;
using UnityEngine.Events;

public class OnMoveInput : UnityEvent<Vector2> { }
public class OnJumpInput : UnityEvent { }

public class Move : UnityEvent { }
public class Idle : UnityEvent { }
public class HasLanded : UnityEvent { }

public class GroundedStatusChanged : UnityEvent<bool> { }

public class OnWalkStatusChange : UnityEvent<bool> { }

public class OnLeftGrabInput : UnityEvent<bool> { }

public class OnRightGrabInput : UnityEvent<bool> { }

public class OnGrabReleased : UnityEvent { }


public class OnHandCollisionEnter : UnityEvent<GameObject, GameObject> { }
public class OnHandCollisionExit : UnityEvent { }

public class OnLeanForwardInput : UnityEvent { }
public class OnLeanBackwardInput : UnityEvent { }
public class OnLeanForwardCancel : UnityEvent { }
public class OnLeanBackwardCancel : UnityEvent { }
public class OnDetatchJoint : UnityEvent { }
public class OnReattachJoint : UnityEvent { }
public class OnSpawnIngredient : UnityEvent<Vector3> { }

public class OnCollisionTriggered : UnityEvent<GameObject> { }

public class OnExplosionTriggered : UnityEvent<Vector3, float> { }
public class Interact : UnityEvent { }

public class StopInteract : UnityEvent { }

public class EnterCounter : UnityEvent { }

public class ExitCounter : UnityEvent { }

public class ObjectGrabbed : UnityEvent<GameObject> { }
public class ObjectReleased : UnityEvent<GameObject> { }

public class OnPlaceTile : UnityEvent { }

public class RemovePlacedObject : UnityEvent<GameObject> { }

public class SkillCheckInput : UnityEvent<GameObject> { }

public class SkillCheckCompleted : UnityEvent { }

public class SetUser : UnityEvent { }

public class ObjectRemovedFromKitchenStation : UnityEvent { }
public class PlayerStoppedLookingAtKitchenStation : UnityEvent { }

public class  InteractableLookedAtChanged : UnityEvent<GameObject> { }

public class PlacedIngredient : UnityEvent<GameObject> { }

public class OnPlaceIngredientInput : UnityEvent { }

public class InteractEvent : UnityEvent<GameObject> { }

public class AlternateInteractInput : UnityEvent<GameObject> { }

public class PlayerStoppedLookingAtInteractable : UnityEvent<GameObject> { }

public class DeliveredDishEvent : UnityEvent { }

public class SkillCheckAttemptFailed : UnityEvent<float> { }

public class OnExplodeInput : UnityEvent { }

public class OnRemoveJoint : UnityEvent<string> { }

