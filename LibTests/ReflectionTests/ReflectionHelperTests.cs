namespace CommonRuns.ReflectionTests
{
    public class ReflectionHelperTests
    {
        internal void Start()
        {
            var derived11 = new Derived11();
            ObjectTypeTest(derived11);
            GenericTypeFromClassModelTest(derived11);
            GenericTypeFromBaseModelTest(derived11);
            GenericTypeFromClassTypeTest(derived11);
            GenericTypeFromBaseTypeTest(derived11);
        }

        internal void ObjectTypeTest(object model)
        {
            var type = model.GetType().Name;
            // Derived11
        }

        internal void GenericTypeFromClassModelTest<T>(T model)
            where T : class
        {
            var type = model.GetType().Name;
            // Derived11
        }

        internal void GenericTypeFromBaseModelTest<T>(T model)
            where T : Base
        {
            var type = model.GetType().Name;
            // Derived11
        }

        internal void GenericTypeFromClassTypeTest<T>(T model)
            where T : class
        {
            var type = typeof(T).Name;
            // Derived11
        }

        internal void GenericTypeFromBaseTypeTest<T>(T model)
            where T : Base
        {
            var type = typeof(T).Name;
            // Derived11
        }
    }
}
