namespace Creazen.Seeker.AI {
    using System.Linq;
    using Creazen.Seeker.Movement;
    using Creazen.Seeker.Session;
    using Creazen.Seeker.Time;
    using Creazen.Seeker.Treasure;
    using Unity.MLAgents;
    using Unity.MLAgents.Actuators;
    using Unity.MLAgents.Sensors;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class UnlockTreasureAgent : Agent {
        [SerializeField] Transform trainEnvironment;

        Transform target = null;
        bool hasUnlockChest = false;
        float previousDistanceToTarget = -1;
        Vector3 previousOnLandPosition;

        //Heuristic
        float moveValue;
        bool isJump;
        bool isOpenChest;

        //Training
        Vector3 initialPosition;

        Idler idler;
        Mover mover;
        KeyHolder keyHolder;
        Timer timer;

        void Update() {
            if(!hasUnlockChest) {
                AddReward(-(Time.deltaTime/timer.InitialTime));
            }
        }

        public override void Initialize() {
            //Set Training Initial Values
            initialPosition = transform.localPosition;
            //

            idler = GetComponent<Idler>();
            mover = GetComponent<Mover>();
            keyHolder = GetComponent<KeyHolder>();
            timer = trainEnvironment.GetComponentInChildren<Timer>();

            if(keyHolder != null) {
                keyHolder.onCatch += () => AddReward(1f);
            }

            if(mover != null) {
                mover.onLand += () => {
                    if(previousOnLandPosition != null) {
                        if(Mathf.Approximately(previousOnLandPosition.y, transform.localPosition.y)) {
                            AddReward(-0.5f);
                            Debug.Log("Approximate");
                        }
                        else {
                            AddReward(0.5f);
                        }
                    }
                    previousOnLandPosition = transform.localPosition;
                };
            }
        }

        public override void OnEpisodeBegin() {
            var sessionObjects = trainEnvironment.GetComponentsInChildren<MonoBehaviour>(true).OfType<ISession>();
            foreach(ISession sessionObject in sessionObjects) {
                sessionObject.Reset();
            }
            transform.localPosition = initialPosition;
            target = null;
            hasUnlockChest = false;
            previousDistanceToTarget = -1;
        }

        public override void CollectObservations(VectorSensor sensor) {
            if (target == null || !target.gameObject.activeInHierarchy) {
                target = GetTarget();
            }

            float currentDistanceToTarget = Vector3.Distance(target.localPosition, transform.localPosition);
            if(previousDistanceToTarget != -1) {
                if(currentDistanceToTarget > previousDistanceToTarget) {
                    AddReward(-0.2f);
                    Debug.Log("Menjauh");
                }
                else if(currentDistanceToTarget < previousDistanceToTarget) {
                    AddReward(0.2f);
                }
            }
            previousDistanceToTarget = currentDistanceToTarget;
            
            sensor.AddObservation(target.localPosition - transform.localPosition);
        }

        public override void OnActionReceived(ActionBuffers actions) {
            if(hasUnlockChest) {
                idler.StartAction();
                return;
            }

            int moveAction = actions.DiscreteActions[0];
            int jumpAction = actions.DiscreteActions[1];
            int unlockTreasureAction = actions.DiscreteActions[2];

            if (moveAction == 0) { idler.StartAction(); }
            else if (moveAction == 1) { mover.StartAction(true); }
            else { mover.StartAction(false); }

            if (jumpAction == 1) {  
                mover.Jump();
                AddReward(-0.1f);
            }

            if (unlockTreasureAction == 1) { 
                Debug.Log("Test");
                if(keyHolder.UnlockChest()) {
                    AddReward(1f);
                    hasUnlockChest = true;
                    EndEpisode();
                }
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut) {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            if(moveValue == 0f) discreteActions[0] = 0;
            else discreteActions[0] = moveValue == 1f? 1 : 2;
            discreteActions[1] = isJump? 1 : 0;
            discreteActions[2] = isOpenChest? 1 : 0;

            isJump = false;
            isOpenChest = false;
        }

        void OnMove(InputValue value) {
            moveValue = value.Get<float>();
        }

        void OnJump() {
            isJump = true;
        }

        void OnOpenChest() {
            isOpenChest = true;
        }

        Transform GetTarget() {
            Treasure.Key key = GetKey();
            if (key != null) return key.transform;

            return GetTreasure().transform;
        }

        Treasure.Key GetKey() {
            return trainEnvironment.GetComponentInChildren<Treasure.Key>();
        }

        Chest GetTreasure() {
            return trainEnvironment.GetComponentInChildren<Chest>();
        }
    }
}