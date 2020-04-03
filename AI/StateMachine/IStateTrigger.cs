using System;

namespace Tools.AI.StateMachine
{
    public interface IStateTrigger<TTriggerEnum>
        where TTriggerEnum : Enum
    {
        void ActivateTrigger(TTriggerEnum key);
        bool GetTrigger(TTriggerEnum key);
    }
}