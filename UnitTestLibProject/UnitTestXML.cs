using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaveLoadLogic;

namespace UnitTestLibProject
{
    [TestClass]
    public class UnitTestXML
    {
        XMLLogic testXML = new XMLLogic();

        [TestMethod]
        // Tests XML Load, it depends on if files exist or not.
        public void TestXmlBookFileExistValid()
        {
            if (testXML.Load() != null)
                Assert.IsNotNull(testXML.Load());
            else if (testXML.Load() == null)
                Assert.IsNull(testXML.Load());
        }

        [TestMethod]
        public void TestXmlAccountFileExistValid()
        {
            if (testXML.LoadAccounts() != null)
                Assert.IsNotNull(testXML.LoadAccounts());
            else if (testXML.LoadAccounts() == null)
                Assert.IsNull(testXML.LoadAccounts());
        }

    }
}
