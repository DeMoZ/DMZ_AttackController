using System;

namespace Attack
{
    public class InputBus
    {
        public Action<AttackDirection> AttackClicked;
        public Action BrakeClicked;
    }
}