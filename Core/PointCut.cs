namespace CryoAOP.Core
{
    public class PointCut
    {
        private readonly MethodInspector methodToBeIntercepted;
        private readonly MethodInspector pointCutToCallOutToMethod;

        public PointCut(MethodInspector methodToBeIntercepted, MethodInspector pointCutToCallOutToMethod)
        {
            this.methodToBeIntercepted = methodToBeIntercepted;
            this.pointCutToCallOutToMethod = pointCutToCallOutToMethod;
        }
    }
}