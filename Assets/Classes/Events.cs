using UnityEngine;
using UnityEngine.Events;

public class OnMoveInput : UnityEvent<Vector2> { }
public class OnJumpInput : UnityEvent { }

public class Move : UnityEvent { }
public class Idle : UnityEvent { }
public class HasLanded : UnityEvent { }

public class GroundedStatusChanged : UnityEvent<bool> { }

public class OnWalkStatusChange : UnityEvent<bool> { }

public class OnLeftGrabInput : UnityEvent { }

public class OnRightGrabInput : UnityEvent { }

public class OnLeftGrabReleased : UnityEvent { }
public class OnRightGrabReleased : UnityEvent { }

