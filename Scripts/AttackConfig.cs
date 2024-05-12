using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Attack
{
    [CreateAssetMenu(menuName = "Attack/" + nameof(AttackConfig), fileName = nameof(AttackConfig))]
    public class AttackConfig : SerializedScriptableObject
    {
        public float sequenceTime = 0.5f;
        public float failTime = 1f;
        
        public Dictionary<List<AttackDirection>, AttackElement> sequences = new();
    }
    
    public class AttackElement
    {
        public float AttackTime = 0.2f;
        public float? SequenceTime;
        public float? FailTime;

        public List<AttackDirection> Sequence { get; set; }
        public string SequenceCode { get; set; }
        
        public void Init(List<AttackDirection> sequence, string sequenceCode)
        {
            Sequence = sequence;
            SequenceCode = sequenceCode;
        }
    }
}