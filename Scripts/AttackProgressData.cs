namespace Attack
{
    public record AttackProgressData(AttackState State, AttackDirection Direction,  float Time, float Time01)
    {
        public AttackState State { get; } = State;
        public AttackDirection Direction { get; } = Direction;
        public float Time { get; } = Time;
        public float Time01 { get; } = Time01;
    }
}