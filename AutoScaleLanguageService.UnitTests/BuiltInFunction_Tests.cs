using System.Text;
using FluentAssertions;
using Xunit;

namespace Lakewood.AutoScale.UnitTests
{
    public class BuiltInFunction_Tests
    {
        /// <summary>
        /// Ensure that, for every MethodSignatureInfo object contained in the dictionary of
        /// built-in functions, its Name property matches the key under which it is stored.
        /// </summary>
        [Fact]
        public void Dictionary_key_matches_method_info()
        {
            var sb = new StringBuilder();

            foreach (var key in BuiltInFunction.Signatures.Keys)
            {
                MethodSignatureInfo[] signatures = BuiltInFunction.Signatures[key];
                for (int i = 0; i < signatures.Length; ++i)
                {
                    if (signatures[i].Name != key)
                    {
                        sb.AppendLine($"Mismatch: key = {key}, overload index = {i}, name = {signatures[i].Name}");
                    }
                }
            }

            sb.ToString().Should().BeEmpty();
        }
    }
}
