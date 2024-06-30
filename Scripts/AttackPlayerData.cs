using DMZ.Events;

namespace Attack
{
    public class AttackPlayerData
    {
        public DMZState<AttackPhase> AttackSequenceState = new();
        public DMZState<AttackProgressData> AttackProgress = new();
        public string CurrentSequenceCode { get; set; }
        public AttackElement CurrentSequenceElement { get; set; }
    }
}