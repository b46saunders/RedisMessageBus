namespace RQueue.Shared
{
    public class DummyObject
    {
        public string someData { get; set; }
        public string someData2 { get; set; }
        public int someIdThatWeNeed { get; set; }

        public override string ToString()
        {
            return $"SomeData:{someData}, SomeData2:{someData2}, SomeIdThatWeNeed:{someIdThatWeNeed}";
        }
    }
    
}
