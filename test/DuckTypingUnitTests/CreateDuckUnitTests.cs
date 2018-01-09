namespace DuckTypingUnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DuckTypingUnitTests.Resources;
    using DuckTyping;

    [TestClass]
    public class CreateDuckUnitTests
    {
        [TestMethod]
        public void Create_SimpleDuck()
        {
            Simple simple = new Simple
            {
                Name = "Duck"
            };

            var isDuckable = simple.IsDuck<ISimpleDuck>();
            Assert.IsTrue(isDuckable);

            var simpleDuck = simple.CreateDuck<ISimpleDuck>();

            Assert.IsNotNull(simpleDuck);
            Assert.AreEqual("Duck", simpleDuck.Name);
        }

        [TestMethod]
        public void Create_SingleMultipleDuck()
        {
            Multiple multiple = new Multiple
            {
                Name = "Mr Duck",
                Address = "2 Duck Lane",
                Phone = "0800 DUCK"
            };

            Assert.IsTrue(multiple.IsDuck<IMultipleDuck1>());
            Assert.IsTrue(multiple.IsDuck<IMultipleDuck2>());

            var multipleDuck = multiple.CreateDuck<IMultipleDuck1, IMultipleDuck2>();

            Assert.IsNotNull(multipleDuck);
            Assert.AreEqual("Mr Duck", ((IMultipleDuck1)multipleDuck).Name);
            Assert.AreEqual("2 Duck Lane", ((IMultipleDuck1)multipleDuck).Address);
            Assert.AreEqual("0800 DUCK", ((IMultipleDuck2)multipleDuck).Phone);
        }

        [TestMethod]
        public void Create_MultipleDucks()
        {
            Multiple multiple = new Multiple
            {
                Name = "Mr Duck",
                Address = "2 Duck Lane",
                Phone = "0800 DUCK"
            };

            Assert.IsTrue(multiple.IsDuck<IMultipleDuck1>());
            Assert.IsTrue(multiple.IsDuck<IMultipleDuck2>());

            var multipleDuck1 = multiple.CreateDuck<IMultipleDuck1>();
            var multipleDuck2 = multiple.CreateDuck<IMultipleDuck2>();

            Assert.IsNotNull(multipleDuck1);
            Assert.IsNotNull(multipleDuck2);
            
            Assert.AreEqual("Mr Duck", multipleDuck1.Name);
            Assert.AreEqual("2 Duck Lane", multipleDuck1.Address);
            Assert.AreEqual("0800 DUCK", multipleDuck2.Phone);
        }
    }
}
