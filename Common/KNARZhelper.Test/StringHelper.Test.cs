using Xunit;

namespace KNARZhelper.Test
{
    public class StringHelperTest
    {
        [Theory]
        [InlineData("This is a test with ���� and other chars.", "This is a test with  and other chars")]
        [InlineData("Tr�berbrook", "Trberbrook")]
        public void TestRemoveSpecialChars(string stringWithUmlaut, string stringWithoutUmlaut)
        {
            Assert.Equal(stringWithoutUmlaut, stringWithUmlaut.RemoveSpecialChars());
        }

        [Theory]
        [InlineData("This is a test with ���� and other chars.", "This is a test with aOuss and other chars.")]
        [InlineData("Tr�berbrook", "Truberbrook")]
        public void TestRemoveDiactritics(string stringWithUmlaut, string stringWithoutUmlaut)
        {
            Assert.Equal(stringWithoutUmlaut, stringWithUmlaut.RemoveDiacritics());
        }

        [Theory]
        [InlineData("   This is   a test      .", "This is a test .")]
        [InlineData("Tr�ber  brook", "Tr�ber brook")]
        public void TestCollapesWhitespaces(string stringWithUmlaut, string stringWithoutUmlaut)
        {
            Assert.Equal(stringWithoutUmlaut, stringWithUmlaut.CollapseWhitespaces());
        }
    }
}