using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class OnMoveInput : UnityEvent<Vector2> { }
public class OnJumpInput : UnityEvent { }
public class OnBoostInput : UnityEvent { }
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
public class ObjectReleased : UnityEvent { }

public class OnPlaceTile : UnityEvent { }

public class RemovePlacedObject : UnityEvent<GameObject> { }

public class SkillCheckInput : UnityEvent<GameObject> { }

public class SkillCheckCompleted : UnityEvent { }

public class SetUser : UnityEvent { }

public class ObjectRemovedFromKitchenStation : UnityEvent { }
public class PlayerStoppedLookingAtKitchenStation : UnityEvent { }

public class  InteractableLookedAtChanged : UnityEvent<GameObject> { }


public class OnPlaceIngredientInput : UnityEvent { }

public class InteractEvent : UnityEvent<GameObject> { }

public class AlternateInteractInput : UnityEvent<GameObject> { }

public class PlayerStoppedLookingAtInteractable : UnityEvent<GameObject> { }

public class DeliveredDishEvent : UnityEvent { }

public class SkillCheckAttemptFailed : UnityEvent<float> { }

public class OnExplodeInput : UnityEvent { }

public class OnRemoveJoint : UnityEvent<string> { }

public class OnPlayerJointReconnect : UnityEvent { }

public class JointRemoved : UnityEvent<GameObject, JointBackup> { }

public class ReleaseHeldJoint : UnityEvent<GameObject> { }

public class LimbDamaged : UnityEvent<float> { }

public class IngredientEnteredAssemblyArea : UnityEvent<GameObject> { }

public class IngredientExitedAssemblyArea : UnityEvent<GameObject> { }


public class MoveImageEvent : UnityEvent<bool> { }

public class UpdateGoalValueEvent : UnityEvent<float> { }

public class SpringUpdateEvent : UnityEvent<float> { }

public class OnButtonPressedEvent : UnityEvent { }

public class OnPlayerJoinedEvent : UnityEvent<GameObject> { }

public class OnChangeColorEvent : UnityEvent<GameObject> { }

public class OnChangeHatEvent : UnityEvent<GameObject> { }

public class PlayerReadyInputEvent : UnityEvent<GameObject> { }

public class CrateLandedEvent : UnityEvent { }

public class IngredientOrderedEvent : UnityEvent<GameObject> { }

public class DPadInteractEvent : UnityEvent<Vector2> { }

public class StartTrainEvent : UnityEvent { }

public class LeverStateChangedEvent : UnityEvent<int> { }

public class DispenserButtonPressedEvent : UnityEvent { }

public class PizzaDoughEnteredOvenEvent : UnityEvent<GameObject> { }

public class PizzaDoughExitedOvenEvent : UnityEvent<GameObject> { }

public class NewOrderAddedEvent : UnityEvent<MenuItem> { }

public class MaxOrderSetEvent : UnityEvent<int> { }

public class OrderCompletedEvent : UnityEvent<MenuItem> { }

public class OnAllPlayersReadyEvent : UnityEvent { }

public class OnNextOptionInput : UnityEvent { }
public class OnPreviousOptionInput : UnityEvent { }

public class OnNavigateInput : UnityEvent<Vector2> { }

public class SlotsFinishedEvent : UnityEvent { }

public class OnSelectInput : UnityEvent { }

public class StartFireEvent : UnityEvent { }
public class StopFireEvent : UnityEvent { }

public class StartSprinklerEvent : UnityEvent { }
public class StopSprinklerEvent : UnityEvent { }

public class GameTimeUpdatedEvent : UnityEvent<float> { }
public class GameOverEvent : UnityEvent { }
public class ShowEndGameUIEvent : UnityEvent { }
public class HideEndGameUIEvent : UnityEvent { }

public class OnCustomerInteract : UnityEvent<GameObject> { }

public class NpcReceivedCorrectOrder : UnityEvent<GameObject> { }

public class ScoreChangedEvent : UnityEvent<int> { }

public class UpdateScoreDisplayEvent : UnityEvent<int> { }

public class ReleaseCrate : UnityEvent { }

public class DroneCompletedPath : UnityEvent<GameObject> { }

public class DroneDeliveryCalled : UnityEvent<Ingredient> { }

public class InitiateDespawnTimerEvent : UnityEvent { }

public class OnGameStartEvent : UnityEvent { }

public class PlayerLookingAtObject : UnityEvent { }
public class PlayerStoppedLookingAtObject : UnityEvent { }

public class OnRespawnInput : UnityEvent<GameObject> { }

public class OnModifiersChoosenEvent : UnityEvent<List<LevelModifiers>> { }

public class OnSlotMachineAnimationCompleteEvent : UnityEvent { }

public class OnGrabStatusChanged : UnityEvent<bool> { }

public class OnInteractInput : UnityEvent<GameObject> { }

public class OnAlternateInteractInput : UnityEvent<GameObject> { }

public class OnObjectGrabbed : UnityEvent<IGrabable> { }

public class OnCutInput : UnityEvent { }

public class OnRightTriggerInput : UnityEvent { }
public class OnRightTriggerCancel : UnityEvent { }

public class SodaSelectedEvent : UnityEvent<MenuItem> { }
