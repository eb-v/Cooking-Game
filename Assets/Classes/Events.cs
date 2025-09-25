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




public class OnHandCollisionEnter : UnityEvent<GameObject> { }
public class OnHandCollisionExit : UnityEvent { }

public class OnLeanForwardInput : UnityEvent { }
public class OnLeanForwardCancel : UnityEvent { }
public class OnLeanBackwardInput : UnityEvent { }
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

public class OnObjectGrabbed : UnityEvent<GameObject, GameObject> { }
