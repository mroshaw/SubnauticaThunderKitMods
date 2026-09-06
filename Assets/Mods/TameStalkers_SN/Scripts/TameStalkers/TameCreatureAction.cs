namespace DaftAppleGames.TameStalkers_SN
{
    public class TameCreatureAction : CreatureAction
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override float Evaluate(Creature targetCreature, float time) => evaluatePriority;

        public override void Perform(Creature targetCreature, float time, float deltaTime)
        {
        }
    }
}
