using NightBlade.CameraAndInput;
using UnityEngine;

namespace NightBlade
{
	public partial class InteractionPlayerCharacterController : PlayerCharacterController
	{
		protected override void Update()
		{
			if (PlayingCharacterEntity == null || !PlayingCharacterEntity.IsOwnerClient)
				return;

			CacheGameplayCameraController.FollowingEntityTransform = CameraTargetTransform;
			CacheMinimapCameraController.FollowingEntityTransform = CameraTargetTransform;
			CacheMinimapCameraController.FollowingGameplayCameraTransform = CacheGameplayCameraController.CameraTransform;

			if (CacheTargetObject != null)
				CacheTargetObject.gameObject.SetActive(_destination.HasValue);

			if (PlayingCharacterEntity.IsDead())
			{
				ClearQueueUsingSkill();
				_destination = null;
				_isFollowingTarget = false;
				CancelBuild();
				UISceneGameplay.SetTargetEntity(null);
			}
			else
			{
				UISceneGameplay.SetTargetEntity(SelectedGameEntity);

				#region World Interactable
				if (TargetEntity is IInteractableTarget interactable)
					UISceneGameplay.SetTargetInteractable(interactable);
				else
					UISceneGameplay.SetTargetInteractable(null);
				#endregion
			}

			if (_destination.HasValue)
			{
				if (CacheTargetObject != null)
					CacheTargetObject.transform.position = _destination.Value;
				if (Vector3.Distance(_destination.Value, MovementTransform.position) < StoppingDistance + 0.5f)
					_destination = null;
			}

			float deltaTime = Time.deltaTime;
			_activateInput.OnUpdate(deltaTime);
			_pickupItemInput.OnUpdate(deltaTime);
			_reloadInput.OnUpdate(deltaTime);
			_findEnemyInput.OnUpdate(deltaTime);
			_exitVehicleInput.OnUpdate(deltaTime);
			_switchEquipWeaponSetInput.OnUpdate(deltaTime);

			UpdateInput();
			#region World Interactable
			UpdateFollowTarget_Interactable();
			#endregion
			PlayingCharacterEntity.AimPosition = PlayingCharacterEntity.GetAttackAimPosition(ref _isLeftHandAttacking);
			PlayingCharacterEntity.SetSmoothTurnSpeed(turnSmoothSpeed);
		}

		public void UpdateFollowTarget_Interactable()
		{
			if (!_isFollowingTarget)
				return;

			IDamageableEntity targetDamageable;
			IActivatableEntity activatableEntity;
			IHoldActivatableEntity holdActivatableEntity;
			IPickupActivatableEntity pickupActivatableEntity;
			if (TryGetAttackingEntity(out targetDamageable))
			{
				if (targetDamageable.IsDeadOrHideFrom(PlayingCharacterEntity))
				{
					ClearQueueUsingSkill();
					PlayingCharacterEntity.StopMove();
					ClearTarget();
					return;
				}
				float attackDistance;
				float attackFov;
				GetAttackDistanceAndFov(_isLeftHandAttacking, out attackDistance, out attackFov);
				AttackOrMoveToEntity(targetDamageable, attackDistance, CurrentGameInstance.playerLayer.Mask | CurrentGameInstance.monsterLayer.Mask);
			}
			else if (TryGetUsingSkillEntity(out targetDamageable))
			{
				if (_queueUsingSkill.skill.IsAttack && targetDamageable.IsDeadOrHideFrom(PlayingCharacterEntity))
				{
					ClearQueueUsingSkill();
					PlayingCharacterEntity.StopMove();
					ClearTarget();
					return;
				}
				float castDistance;
				float castFov;
				GetUseSkillDistanceAndFov(_isLeftHandAttacking, out castDistance, out castFov);
				UseSkillOrMoveToEntity(targetDamageable, castDistance);
			}
			else if (TryGetDoActionEntity(out activatableEntity, TargetActionType.ClickActivate))
			{
				DoActionOrMoveToEntity(activatableEntity, activatableEntity.GetActivatableDistance(), () =>
				{
					if (activatableEntity.ShouldNotActivateAfterFollowed())
						return;
					if (!_didActionOnTarget)
					{
						_didActionOnTarget = true;
						if (activatableEntity.CanActivate())
							activatableEntity.OnActivate();
						if (activatableEntity.ShouldClearTargetAfterActivated())
							ClearTarget();
					}
				});
			}
			else if (TryGetDoActionEntity(out holdActivatableEntity, TargetActionType.HoldClickActivate))
			{
				DoActionOrMoveToEntity(holdActivatableEntity, holdActivatableEntity.GetActivatableDistance(), () =>
				{
					if (!_didActionOnTarget)
					{
						_didActionOnTarget = true;
						if (holdActivatableEntity.CanHoldActivate())
							holdActivatableEntity.OnHoldActivate();
						if (holdActivatableEntity.ShouldClearTargetAfterActivated())
							ClearTarget();
					}
				});
			}
			else if (TryGetDoActionEntity(out pickupActivatableEntity, TargetActionType.ClickActivate))
			{
				DoActionOrMoveToEntity(pickupActivatableEntity, pickupActivatableEntity.GetActivatableDistance(), () =>
				{
					if (!_didActionOnTarget)
					{
						_didActionOnTarget = true;
						if (pickupActivatableEntity.CanPickupActivate())
							pickupActivatableEntity.OnPickupActivate();
						if (pickupActivatableEntity.ShouldClearTargetAfterActivated())
							ClearTarget();
					}
				});
			}
			#region World Interactable
			else if (TryGetDoActionEntity(out IInteractableTarget interactableTarget, TargetActionType.Interact))
			{
				DoActionOrMoveToEntity( interactableTarget as ITargetableEntity, interactableTarget.GetInteractDistance(),  () =>
				{
					if (!_didActionOnTarget)
					{
						_didActionOnTarget = true;
						interactableTarget.Interact(this);
						//ClearTarget();
					}
				});
			}
			#endregion
		}

		public override void UpdatePointClickInput()
		{
			if (controllerMode == PlayerCharacterControllerMode.WASD)
				return;

			// If it's building something, not allow point click movement
			if (ConstructingBuildingEntity != null)
				return;

			// If it's aiming skills, not allow point click movement
			if (UICharacterHotkeys.UsingHotkey != null)
				return;

			_getMouseDown = InputManager.GetMouseButtonDown(0);
			_getMouseUp = InputManager.GetMouseButtonUp(0);
			_getMouse = InputManager.GetMouseButton(0);

			if (_getMouseDown)
			{
				_isMouseDragOrHoldOrOverUI = false;
				_mouseDownTime = Time.unscaledTime;
				_mouseDownPosition = InputManager.MousePosition();
			}
			// Read inputs
			_isPointerOverUI = UISceneGameplay.IsPointerOverUIObject();
			_isMouseDragDetected = (InputManager.MousePosition() - _mouseDownPosition).sqrMagnitude > DETECT_MOUSE_DRAG_DISTANCE_SQUARED;
			_isMouseHoldDetected = Time.unscaledTime - _mouseDownTime > DETECT_MOUSE_HOLD_DURATION;
			_isMouseHoldAndNotDrag = !_isMouseDragDetected && _isMouseHoldDetected;
			if (!_isMouseDragOrHoldOrOverUI && (_isMouseDragDetected || _isMouseHoldDetected || _isPointerOverUI))
			{
				// Detected mouse dragging or hold on an UIs
				_isMouseDragOrHoldOrOverUI = true;
			}
			// Will set move target when pointer isn't point on an UIs 
			if (!_isPointerOverUI && !_isWASDAttackInputLastFrame && (_getMouse || _getMouseUp))
			{
				// Clear target
				ClearTarget(true);
				_didActionOnTarget = false;
				// Prepare temp variables
				Transform tempTransform;
				bool tempHasMapPosition = false;
				Vector3 tempMapPosition = Vector3.zero;
				// If mouse up while cursor point to target (character, item, npc and so on)
				bool mouseUpOnTarget = _getMouseUp && !_isMouseDragOrHoldOrOverUI;
				int tempCount = FindClickObjects(out Vector3 tempVector3);
				for (int tempCounter = 0; tempCounter < tempCount; ++tempCounter)
				{
					tempTransform = _physicFunctions.GetRaycastTransform(tempCounter);
					// When holding on target, or already enter edit building mode
					if (_isMouseHoldAndNotDrag)
					{
						IHoldActivatableEntity activatable = tempTransform.GetComponent<IHoldActivatableEntity>();
						if (!activatable.IsNull() && activatable.CanHoldActivate())
						{
							SetTarget(activatable, TargetActionType.HoldClickActivate);
							_isFollowingTarget = true;
							tempHasMapPosition = false;
							break;
						}
					}
					else if (mouseUpOnTarget)
					{
						#region World Interactable
						IInteractableTarget interactable = tempTransform.GetComponentInParent<IInteractableTarget>();
						if (interactable != null)
						{
							SetTarget(interactable, TargetActionType.Interact);
							_isFollowingTarget = true;
							tempHasMapPosition = false;
							break;
						}
						#endregion

						ITargetableEntity targetable = tempTransform.GetComponent<ITargetableEntity>();
						IActivatableEntity activatable = targetable as IActivatableEntity;
						IPickupActivatableEntity pickupActivatable = targetable as IPickupActivatableEntity;
						IDamageableEntity damageable = targetable as IDamageableEntity;
						if (!targetable.IsNull() && !targetable.NotBeingSelectedOnClick())
						{
							if (!activatable.IsNull() && activatable.CanActivate())
							{
								if (activatable.ShouldBeAttackTarget())
									SetTarget(activatable, TargetActionType.Attack);
								else
									SetTarget(activatable, TargetActionType.ClickActivate);
								_isFollowingTarget = true;
								tempHasMapPosition = false;
								break;
							}
							else if (!pickupActivatable.IsNull() && pickupActivatable.CanPickupActivate())
							{
								SetTarget(pickupActivatable, TargetActionType.ClickActivate);
								_isFollowingTarget = true;
								tempHasMapPosition = false;
								break;
							}
							else if (damageable != null && !damageable.IsDeadOrHideFrom(PlayingCharacterEntity))
							{
								SetTarget(damageable, TargetActionType.Attack);
								_isFollowingTarget = true;
								tempHasMapPosition = false;
								break;
							}
						}
						if (!_physicFunctions.GetRaycastIsTrigger(tempCounter))
						{
							// Set clicked map position, it will be used if no activating entity found
							tempHasMapPosition = true;
							tempMapPosition = _physicFunctions.GetRaycastPoint(tempCounter);
							break;
						}

					} // End mouseUpOnTarget
				}
				// When clicked on map (Not touch any game entity)
				// - Clear selected target to hide selected entity UIs
				// - Set target position to position where mouse clicked
				if (tempHasMapPosition)
				{
					SelectedEntity = null;
					_targetPosition = tempMapPosition;
				}

				// Found ground position
				if (_targetPosition.HasValue)
				{
					// Close NPC dialog, when target changes
					HideNpcDialog();
					ClearQueueUsingSkill();
					_isFollowingTarget = false;
					if (PlayingCharacterEntity.IsPlayingActionAnimation())
					{
						if (pointClickInterruptCastingSkill)
							PlayingCharacterEntity.InterruptCastingSkill();
					}
					else
					{
						OnPointClickOnGround(_targetPosition.Value);
					}
				}
			}
		}
	}
}
