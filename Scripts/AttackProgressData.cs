namespace Attack
{
    public record AttackProgressData(AttackPhase State, AttackDirection Direction,  float Time, float Time01)
    {
        public AttackPhase State { get; } = State;
        public AttackDirection Direction { get; } = Direction;
        public float Time { get; } = Time;
        public float Time01 { get; } = Time01;
    }
}