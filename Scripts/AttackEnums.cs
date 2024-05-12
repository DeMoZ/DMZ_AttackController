namespace Attack
{
    public enum AttackState
    {
        None,
        Idle,
        Attack,
        SequenceReady,
        SequenceFail,
    }
    
    public enum AttackDirection
    {
        L,
        R,
        D,
        U,
    }

    public enum AttackLength
    {
        Click,
        Hold,
    }
}