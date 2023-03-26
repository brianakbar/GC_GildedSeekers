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

    public class ChaseTargetAgent : Agent {
        [SerializeField] Transform trainEnvironment;
        [SerializeField] string targetTag = "Player";

        Transform target = null;
        float previousDistanceToTarget = -1;
        int previousMoveAction = 0;
        Vector3 previousOnLandPosition;
        bool targetHasUnlockChest = false;

        //Training
        Vector3 initialPosition;

        Idler idler;
        Mover mover;
        Timer timer;
        Chest chest;

        void Update() {
            AddReward(-(Time.deltaTime/timer.InitialTime));
        }

        public override void Initialize() {
            //Set Training Initial Values
            initialPosition = transform.localPosition;
            //

            idler = GetComponent<Idler>();
            mover = GetComponent<Mover>();
            timer = trainEnvironment.GetComponentInChildren<Timer>();
            chest = trainEnvironment.GetComponentInChildren<Chest>();

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

            if(chest != null) {
                chest.onUnlock += () => {
                    AddReward(-1f);
                    targetHasUnlockChest = true;
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
                    AddReward(0.1f);
                }
            }
            previousDistanceToTarget = currentDistanceToTarget;
            
            sensor.AddObservation(target.localPosition - transform.localPosition);
        }

        public override void OnActionReceived(ActionBuffers actions) {
            if(targetHasUnlockChest) {
                idler.StartAction();
                return;
            }

            int moveAction = actions.DiscreteActions[0];
            int jumpAction = actions.DiscreteActions[1];

            if(previousMoveAction != moveAction) {
                AddReward(-0.4f);
            }
            else {
                AddReward(0.01f);
            }

            if (moveAction == 0) { idler.StartAction(); }
            else if (moveAction == 1) { mover.StartAction(true); }
            else { mover.StartAction(false); }

            if (jumpAction == 1) {  
                mover.Jump();
                AddReward(-0.1f);
            }
        }

        Transform GetTarget() {
            foreach(Transform child in trainEnvironment.transform) {
                if(child.tag == targetTag) return child;
            }

            return null;
        }
    }
}