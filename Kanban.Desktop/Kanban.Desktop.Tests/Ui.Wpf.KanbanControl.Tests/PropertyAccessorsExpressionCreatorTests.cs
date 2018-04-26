﻿using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using Ui.Wpf.KanbanControl.Expressions;

namespace Ui.Wpf.KanbanControl.Tests
{
    [TestFixture]
    public class PropertyAccessorsExpressionCreatorTests
    {
        [Test]
        public void PropertyAccessorsExpressionCreator_Should_Make_Accessors_To_Any_Property()
        {
            var pa = new PropertyAccessorsExpressionCreator(typeof(MockClass));

            var aGetter = pa.TakeGetterForProperty("A");
            var bGetter = pa.TakeGetterForProperty("B");
            var subGetter = pa.TakeGetterForProperty("SubClass");
            var subParentGetter = pa.TakeGetterForProperty("SubClass.Parent");
            var subAnotherGetter = pa.TakeGetterForProperty("SubClass.AnotherParent");
            var subAGetter = pa.TakeGetterForProperty("SubClass.A");
            var subBGetter = pa.TakeGetterForProperty("SubClass.B");

            Assert.NotNull(aGetter);
            Assert.NotNull(bGetter);
            Assert.NotNull(subGetter);
            Assert.NotNull(subParentGetter);
            Assert.NotNull(subAnotherGetter);
            Assert.NotNull(subAGetter);
            Assert.NotNull(subBGetter);
        }

        [Test]
        public void PropertyAccessorsExpressionCreator_Should_Not_Be_To_Slow()
        {
            var pa = new PropertyAccessorsExpressionCreator(typeof(MockClass));

            var testObjects = Enumerable
                .Range(0, 10000)
                .Select(x =>
                {
                    var obj = new MockClass { A = x, B = x.ToString() };

                    var sub = new SubClass { A = x * 2, B = (x * 2).ToString(), Parent = obj, AnotherParent = null };

                    obj.SubClass = sub;

                    return obj;
                })
                .ToArray();


            var swGenericProperty = Stopwatch.StartNew();
            for (int i = 0; i < 5000; i++)
            {
                foreach (var obj in testObjects)
                {
                    var a = obj.A;
                    var b = obj.B;
                    var c = obj.SubClass;
                    var sa = obj.SubClass.A;
                    var sb = obj.SubClass.B;
                    var sp = obj.SubClass.Parent;
                    var sap = obj.SubClass.AnotherParent;
                }
            }
            var genericPropertyElapsed = swGenericProperty.Elapsed;

            var swExpressionProperty = Stopwatch.StartNew();

            var aGetter = pa.TakeGetterForProperty("A");
            var bGetter = pa.TakeGetterForProperty("B");
            var subGetter = pa.TakeGetterForProperty("SubClass");
            var subParentGetter = pa.TakeGetterForProperty("SubClass.Parent");
            var subAnotherGetter = pa.TakeGetterForProperty("SubClass.AnotherParent");
            var subAGetter = pa.TakeGetterForProperty("SubClass.A");
            var subBGetter = pa.TakeGetterForProperty("SubClass.B");

            for (int j = 0; j < 5000; j++)
            {
                foreach (var obj in testObjects)
                {
                    var a = aGetter(obj);
                    var b = bGetter(obj);
                    var c = subGetter(obj);
                    var sp = subParentGetter(obj);
                    var spa = subAnotherGetter(obj);
                    var sa = subAGetter(obj);
                    var sb = subBGetter(obj);
                }
            }
            var expressionPropertyElapsed = swExpressionProperty.Elapsed;

            Assert.LessOrEqual(
                Math.Abs(genericPropertyElapsed.TotalMilliseconds - expressionPropertyElapsed.TotalMilliseconds),
                genericPropertyElapsed.TotalMilliseconds * 3);
        }


        public class MockClass
        {
            public int A { get; set; }

            public string B { get; set; }

            public SubClass SubClass { get; set; }
        }

        public class SubClass
        {
            public int A { get; set; }

            public string B { get; set; }

            public MockClass Parent { get; set; }

            public MockClass AnotherParent { get; set; }
    }
    }
}
