using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SyncTasks;

namespace SyncTasksTests
{
    [TestFixture]
    public class FilterUtilsTests
    {
        private FilterUtils _filter;

        [SetUp]
        public void Setup()
        {
            _filter = new FilterUtils("*.ilk *.idb *.obj *.pch *.pdb *.tlb *.vspscc *.vssscc *.ncb *.opensdf *.sdf *.cachefile *.dmp *.exp *.pyc fullfile.txt f.txt*",
                "ipch \"System Volume Information\" $RECYCLE.BIN Mp3 Fotos temp *wildcard");
        }

        [Test]
        public void Test_Null()
        {
            // Default one should return false when arg is null
            Assert.That(_filter.MatchesFilter(null), Is.False);

            // Dummy filter should always return false
            _filter = new FilterUtils(null, null);
            Assert.That(_filter.MatchesFilter("foobar"), Is.False);


            _filter = new FilterUtils(null, "foo");
            Assert.That(_filter.MatchesFilter(@"d:\true\foo\"), Is.True);

        }

        [Test]
        public void Test_Matches_Files()
        {
            Assert.That(_filter.MatchesFilter(@"d:\ipch"), Is.False, "ipch is a file");
            // This should match extensions only
            Assert.That(_filter.MatchesFilter(@"d:\test.ilk"), Is.True);
           
            Assert.That(_filter.MatchesFilter(@"d:\my_ilk.txt"), Is.False);

            Assert.That(_filter.MatchesFilter(@"d:\fullfile.txt_"), Is.False);
            
            Assert.That(_filter.MatchesFilter(@"d:\f.txt_"), Is.True);

        }

        [Test]
        public void Test_Matches_Directories()
        {
            Assert.That(_filter.MatchesFilter(@"d:\ipch\boo"), Is.True);
           
            Assert.That(_filter.MatchesFilter(@"d:\ipch\"), Is.True);

            Assert.That(_filter.MatchesFilter(@"d:\System Volume Information\"), Is.True);


            Assert.That(_filter.MatchesFilter(@"d:\komische Fotos\"), Is.False);
            Assert.That(_filter.MatchesFilter(@"d:\komische fotos\"), Is.False); // Case

            Assert.That(_filter.MatchesFilter(@"d:\prefix wildcard\"), Is.True);
            Assert.That(_filter.MatchesFilter(@"d:\prefix Wildcard\"), Is.True); // Case

        }
    }
}
