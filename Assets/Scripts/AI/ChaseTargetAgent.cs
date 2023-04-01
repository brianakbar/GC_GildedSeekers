namespace Creazen.Seeker.AI {
    using System.Linq;
    using Creazen.Seeker.Attributes;
    using Creazen.Seeker.Combat;
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
        Vector3 previousOnLandPosition;
        bool targetHasUnlockChest = false;

        //Training
        Vector3 initialPosition;

        Idler idler;
        Mover mover;
        Chest chest;
        Fighter fighter;

        const string trainEnvironmentTag = "TrainEnvironment";

        void Update() {
            AddReward(-0.0001f);
        }

        public override void Initialize() {
            if(!trainEnvironment) {
                Transform parent = transform.parent;
                while(parent) {
                    if(parent.tag == trainEnvironmentTag) {
                        trainEnvironment = parent;
                        break;
                    }
                    parent = parent.parent;
                }
            }

            //Set Training Initial Values
            initialPosition = transform.localPosition;
            //

            idler = GetComponent<Idler>();
            mover = GetComponent<Mover>();
            chest = trainEnvironment.GetComponentInChildren<Chest>();
            fighter = GetComponent<Fighter>();

            if(mover != null) {
                mover.onLand += () => {
                    if(previousOnLandPosition != null) {
                        if(Mathf.Approximately(previousOnLandPosition.y, transform.localPosition.y)) {
                            AddReward(-0.5f);
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

            if(fighter != null) {
                fighter.onHit += (gameObject) => {
                    if(gameObject.TryGetComponent(out Health health)) {
                        AddReward(20f);
                        // var sessionObjects = trainEnvironment.GetComponentsInChildren<MonoBehaviour>(true).OfType<ISession>();
                        // foreach(ISession sessionObject in sessionObjects) {
                        //     sessionObject.Reset();
                        // }
                        // EndEpisode();
                    }
                    else {
                        AddReward(-1f);
                    }
                };
            }
        } 

        public override void OnEpisodeBegin() {
            transform.localPosition = initialPosition;
            target = null;
            previousDistanceToTarget = -1;
        }

        public override void Heuristic(in ActionBuffers actionsOut) {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            RaycastHit2D hit = Physics2D.Raycast(transform.position,  new Vector2(transform.localScale.x, 0), 1000);
            if(hit.transform != target) {
                discreteActions[2] = 0;
            }
        }

        public override void CollectObservations(VectorSensor sensor) {
            if (target == null || !target.gameObject.activeInHierarchy) {
                target = GetTarget();
            }

            float currentDistanceToTarget = Vector3.Distance(target.localPosition, transform.localPosition);
            if(IsApproximate(transform.position, target.position)) {
                if(previousDistanceToTarget != -1) {
                    if(currentDistanceToTarget > previousDistanceToTarget) {
                        AddReward(-0.2f);
                    }
                    else if(currentDistanceToTarget < previousDistanceToTarget) {
                        AddReward(0.1f);
                    }
                }
            }
            previousDistanceToTarget = currentDistanceToTarget;
            
            sensor.AddObservation(target.localPosition - transform.localPosition);

            RaycastHit2D hit = Physics2D.Raycast(transform.position,  new Vector2(transform.localScale.x, 0), 1000);
            //Debug.Log(hit.transform == target);
            sensor.AddObservation(hit.transform == target);
        }

        public override void OnActionReceived(ActionBuffers actions) {
            if(targetHasUnlockChest) {
                idler.StartAction();
                return;
            }

            int moveAction = actions.DiscreteActions[0];
            int jumpAction = actions.DiscreteActions[1];
            int attackAction = actions.DiscreteActions[2];

            if (moveAction == 0) { idler.StartAction(); }
            else if (moveAction == 1) { mover.StartAction(true); }
            else { mover.StartAction(false); }

            if (jumpAction == 1) {  
                mover.Jump();
                AddReward(-0.1f);
            }

            if(attackAction == 1) {
                if(transform.position.x < target.position.x) {
                    mover.StartAction(true);
                }
                else if(transform.position.x > target.position.x) {
                    mover.StartAction(false);
                }
                fighter.StartAction();
                if(!IsApproximate(transform.position, target.position)) {
                    AddReward(-0.5f);
                }
            }
        }

        Transform GetTarget() {
            foreach(Transform child in trainEnvironment.transform) {
                if(child.tag == targetTag) return child;
            }

            return null;
        }

        bool IsApproximate(Vector2 a, Vector2 b) {
            if(Mathf.Abs(a.y - b.y) <= 1) {
                return true;
            }
            return false;
        }
    }
}