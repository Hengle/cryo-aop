namespace CryoAOP.TestAssembly
{
    public class PropertyInterceptorTarget
    {
        private int someInteger = 0;
        public int SomeInteger
        {
            get { return someInteger; }
            set { someInteger = value; }
        }
    }
}
