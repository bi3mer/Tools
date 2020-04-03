namespace UnityUtility
{
    /// <summary>
    /// Simple class to generate integers from 0 to max int.
    /// </summary>
    public class SequenceIDGenerator
    {
        private int id = 0;

        public int NextID()
        {
            return ++id;
        }
    }
}