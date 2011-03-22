//CryoAOP. Aspect Oriented Framework for .NET.
//Copyright (C) 2011  Gavin van der Merwe (fir3pho3nixx@gmail.com)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using CryoAOP.TestAssembly.Aspects;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class AspectTests
    {
        [Test]
        public void Should_intercept_aspects()
        {
            var wasIntercepted = false;
            var aspectTypeA = new AspectTypeA();
            aspectTypeA.WhenCalled<AspectTypeA>((i) => wasIntercepted = true);
            aspectTypeA.Method();
            Assert.That(wasIntercepted);
        }

        [Test]
        public void Mixins_should_not_cross_intercept_calls_on_types_for_instances()
        {
            var aspectAWasIntercepted = false;
            var aspectTypeA = new AspectTypeA();
            aspectTypeA.WhenCalled<AspectTypeA>((i) => aspectAWasIntercepted = true);

            var aspectBWasIntercepted = false;
            var aspectTypeB = new AspectTypeA();
            aspectTypeB.WhenCalled<AspectTypeB>((i) => aspectBWasIntercepted = true);

            aspectTypeA.Method();

            Assert.That(aspectAWasIntercepted && !aspectBWasIntercepted);
        }
    }
}