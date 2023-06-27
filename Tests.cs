using NUnit.Framework;
using SFM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASI;

namespace Codename_TALaT_CS
{
    [TestFixture]
    internal class Tests
    {
        [Test]
        public static void FileDeletion()
        {
            SFM.SFM.baseDirectory = Path.Combine(TextAdventureLauncher.gamePath, "SFM-Files");

            InternalFileEmulation internalFile1 = new("Delete", "This will be gone");

            InternalFileEmulation internalFile2 = new("DeleteSmart", "This will firstly not be deleted");
            InternalFileEmulation internalFile3 = new("DeleteSmart", "This will firstly not be deleted");


            string internalFile1Path = SFM.SFM.GetFileOfHash( internalFile1.Hash);
            internalFile1.Content = null;
            Assert.That(!Path.Exists(internalFile1Path));
            string internalFile2Path = SFM.SFM.GetFileOfHash(internalFile2.Hash);
            internalFile2.Content = "This will stay haha";
            Assert.That(Path.Exists(internalFile2Path));
            internalFile3.Content = null;
            Assert.That(!Path.Exists(internalFile2Path));




        }

    }
}
