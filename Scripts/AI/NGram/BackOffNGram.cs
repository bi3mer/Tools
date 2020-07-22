namespace Tools.AI.NGram
{
    public class BackOffNGram : HierarchicalNGram
    {
        public BackOffNGram(int n, float compiledMemoryUpdate) : base(n, compiledMemoryUpdate)
        {
        }

        public override ICompiledGram Compile()
        {
            return new CompiledBackOffNGram(this);
        }
    }
}