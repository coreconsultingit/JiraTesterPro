namespace JiraTesterProTestFixture
{
    public class Tests
    {
        public TestContext TestContext { get; set; }
        [SetUp]
        public void Setup()
        {
            var user = TestContext.Parameters.Get("username");
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}