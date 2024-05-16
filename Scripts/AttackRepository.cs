using System.Collections.Generic;
using System.Linq;

namespace Attack
{
    public interface IAttackRepository
    {
        void Init(AttackConfig config);
        List<string> GetSequencesKeys();
        bool TryGetSequence(string code, out AttackElement element);
        float GetAttackTime(string code);
        float GetSequenceTime(string code);
        float GetFailTime(string code);
    }

    public class AttackRepository : IAttackRepository
    {
        private AttackConfig _config;
        private Dictionary<string, AttackElement> _attacks;

        public void Init(AttackConfig config)
        {
            _config = config;
            _attacks = new Dictionary<string, AttackElement>();

            foreach (var pair in config.sequences)
            {
                var sequenceCode = string.Join("", pair.Key.Select(direction => direction.ToString()));
                _attacks[sequenceCode] = pair.Value;
                _attacks[sequenceCode].Init(pair.Key, sequenceCode);
            }
        }

        public List<string> GetSequencesKeys() => _attacks.Select(item => item.Key).ToList();

        private float GetDefaultSequenceTime() => _config.sequenceTime;
        private float GetDefaultFailTime() => _config.failTime;

        public bool TryGetSequence(string code, out AttackElement element)
        {
            element = new AttackElement();

            if (!_attacks.TryGetValue(code, out var value)) return false;

            element = value;
            return true;
        }

        public float GetAttackTime(string code)
        {
            if (TryGetSequence(code, out var element))
                return element.AttackTime;

            throw new System.ArgumentOutOfRangeException();
        }

        public float GetSequenceTime(string code)
        {
            if (TryGetSequence(code, out var element))
                return element.SequenceTime ?? GetDefaultSequenceTime();

            throw new System.ArgumentOutOfRangeException();
        }

        public float GetFailTime(string code)
        {
            if (TryGetSequence(code, out var element))
                return element.SequenceTime ?? GetDefaultFailTime();

            throw new System.ArgumentOutOfRangeException();
        }
    }
}