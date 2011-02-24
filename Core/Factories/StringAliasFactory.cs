using System;
using CryoAOP.Core.Extensions;

namespace CryoAOP.Core.Factories
{
    public class StringAliasFactory
    {
        public string GenerateIdentityName(string originalName)
        {
            return "_{0}_{1}".FormatWith(Guid.NewGuid().ToString("N"), originalName);
        }
    }
}