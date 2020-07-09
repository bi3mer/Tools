namespace Tools.AI.NGram
{
    public interface IGram
    {
        void AddData(string[] inData, string outData);
        void UpdateMemory(float percentRemembered);
        void AddGrammar(IGram gram);
        ICompiledGram Compile();
        int GetN();
    }
}
