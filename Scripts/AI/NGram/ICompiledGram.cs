using System.Collections.Generic;

namespace Tools.AI.NGram
{
    public interface ICompiledGram
    {
        ICompiledGram Clone();
        string Get(string[] inData);
        string[] GetGuesses(string[] inData);
        bool HasNextStep(string[] inData);
        int GetN();
        Dictionary<string, float> GetValues(string[] inData);
    }
}
