using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureGame_AI_implementation
{
   

    public class Adventurer
    {
        private bool hasLamp;
        private bool hasKey;

        public Adventurer()
        {
            SetLamp(false);
            SetKey(false);
        }

        public bool HasLamp()
        {
            return hasLamp;
        }

        public bool HasKey()
        {
            return hasKey;
        }

        public void SetLamp(bool b)
        {
            hasLamp = b;
        }

        public void SetKey(bool b)
        {
            hasKey = b;
        }

        public override string ToString()
        {
            return $"Adventurer[hasLamp={hasLamp}, hasKey={hasKey}]";
        }
    }
}
