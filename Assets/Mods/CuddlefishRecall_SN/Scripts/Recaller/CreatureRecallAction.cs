using UnityEngine;

namespace DaftAppleGames.CuddlefishRecall_SN
{
    /// <summary>
    /// Gives an active recall priority over the Cuddlefish's normal creature actions
    /// </summary>
    internal class CreatureRecallAction : CreatureAction
    {
        [SerializeField] private float arrivalTolerance = 1.5f;

        private CuteFish cuteFish;
        private PingInstance recallPing;
        private bool isRecalling;
        private int creatureIndex;

        internal bool IsRecalling => isRecalling;

        internal int CreatureIndex => creatureIndex;

        internal float DistanceToPlayer => Vector3.Distance(transform.position, Player.main.transform.position);

        internal float ArrivalTolerance => arrivalTolerance;
        
        /// <summary>
        /// Caches required components and assigns the highest action priority
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            cuteFish = GetComponent<CuteFish>();
            recallPing = GetComponent<PingInstance>();
            
            evaluatePriority = float.MaxValue;
        }

        /// <summary>
        /// Returns the recall priority while a recall is active
        /// </summary>
        public override float Evaluate(Creature targetCreature, float time) =>
            isRecalling ? evaluatePriority : 0f;

        /// <summary>
        /// Moves the Cuddlefish toward the player and completes the recall on arrival
        /// </summary>
        public override void Perform(Creature targetCreature, float time, float deltaTime)
        {
            if (Vector3.Distance(transform.position, Player.main.transform.position) < arrivalTolerance)
            {
                CompleteRecall(creatureIndex);
                return;
            }

            swimBehaviour.SwimTo(Player.main.transform.position, CuddlefishRecallPlugin.ConfigFile.RecallSwimVelocity);
        }

        internal bool BeginRecall(int index)
        {
            if (isRecalling)
            {
                return false;
            }

            creatureIndex = index;
            isRecalling = true;
            recallPing.SetLabel($"Cuddlefish {index} - Recalling");
            recallPing.SetVisible(true);

            if (creature.GetBestAction() == this || creature.TryStartAction(this))
            {
                return true;
            }

            isRecalling = false;
            recallPing.SetVisible(false);
            return false;
        }

        internal void CompleteRecall(int index)
        {
            isRecalling = false;
            recallPing.SetVisible(false);
            creature.leashPosition = transform.position;
            cuteFish.followingPlayer = true;
            ErrorMessage.AddMessage($"Cuddlefish {index} has arrived!");
        }

        internal void CompleteTeleportRecall(int index)
        {
            swimBehaviour.Idle();
            CompleteRecall(index);
        }

        internal void CancelRecall()
        {
            isRecalling = false;
            recallPing.SetVisible(false);
        }
    }
}
