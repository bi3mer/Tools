namespace Tools.AI.NGram
{
    public interface ICompiledGram
    {
        string Get(string[] inData);
        string[] GetGuesses(string[] inData);
        bool HasNextStep(string[] inData);
        int GetN();
    }
}
