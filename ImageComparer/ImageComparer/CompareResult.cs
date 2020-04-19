namespace ImageComparer
{
    public class CompareResult
    {
        public string SourceFullName { get; set; }

        public string CompareFullName { get; set; }
        
        public int EqualElements { get; set; }

        public bool IsEqual => EqualElements == 256;
    }
}