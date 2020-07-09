using System.Collections.Generic;
using UnityEngine.Assertions;
using Tools.ID;

namespace Tools.AI.NGram.Utility
{
    public class NGramIDContainer
    {
        private Dictionary<string, string> columnToId = null;
        private Dictionary<string, string> idToColumn = null;
        private IEnumerator<string> idGenerator = null;

        public NGramIDContainer(int idSize = 10)
        {
            columnToId = new Dictionary<string, string>();
            idToColumn = new Dictionary<string, string>();
            idGenerator = StringIDGenerator.GetID(idSize).GetEnumerator();
        }

        public string GetID(string token)
        {
            if (columnToId.ContainsKey(token) == false)
            {
                idGenerator.MoveNext();
                string id = idGenerator.Current;

                columnToId.Add(token, id);
                idToColumn.Add(id, token);

                return id;
            }

            return columnToId[token];
        }

        public List<string> GetIDs(IEnumerable<string> tokens)
        {
            List<string> ids = new List<string>();
            foreach (string col in tokens)
            {
                ids.Add(GetID(col));
            }

            return ids;
        }

        public string GetToken(string id)
        {
            Assert.IsTrue(idToColumn.ContainsKey(id));
            return idToColumn[id];
        }
    }
}