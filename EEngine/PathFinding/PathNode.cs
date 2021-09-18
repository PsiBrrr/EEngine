using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEngine.EEngine
{
    class PathNode
    {
        public float gCost;
        public float hCost;
        public float fCost;

        public PathNode PreviousNode;

        public PathNode()
        {

        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}
