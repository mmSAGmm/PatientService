namespace GenerateApp
{
    public static class SafeRandom
    {
        static SafeRandom() 
        {
            _random = new ThreadLocal<Random>(() => new Random());
        }
        
        private static ThreadLocal<Random> _random;

        public static Random? Random => _random.Value;
    }
}
