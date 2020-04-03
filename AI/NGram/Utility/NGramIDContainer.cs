using System.Collections.Generic;
using Tools.ID;

namespace Tools.AI.NGram.Utility
{
    public class NGramIDContainer
    {
        private Dictionary<string, string> columnToId = null;
        private Dictionary<string, string> idToColumn = null;
        private IEnumerator<string> idGenerator = null;

        public NGramIDContainer(int idSize = 15)
        {
            columnToId = new Dictionary<string, string>();
            idToColumn = new Dictionary<string, string>();
            idGenerator = StringIDGenerator.GetID(idSize).GetEnumerator();
        }
    }
}