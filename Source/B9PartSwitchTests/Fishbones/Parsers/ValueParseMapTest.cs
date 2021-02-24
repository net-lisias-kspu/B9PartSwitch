﻿using System;
using Xunit;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitchTests.Fishbones.Parsers
{
    public class ValueParseMapTest
    {
        [Fact]
        public void TestAddParser__GetParser()
        {
            ValueParseMap map = new ValueParseMap();
            map.AddParser(Exemplars.ValueParser);

            Assert.Same(Exemplars.ValueParser, map.GetParser(typeof(string)));
        }

        [Fact]
        public void TestAddParser__GetParser__Func()
        {
            ValueParseMap map = new ValueParseMap();
            map.AddParser<string>(s => $">{s}<", s => $"<{s}>");

            Assert.Equal(">abc<", map.GetParser(typeof(string)).Parse("abc"));
            Assert.Equal("<abc>", map.GetParser(typeof(string)).Format("abc"));
        }

        [Fact]
        public void TestAddParser__Null()
        {
            ValueParseMap map = new ValueParseMap();
            Assert.Throws<ArgumentNullException>(() => map.AddParser<int>(null, i => i.ToString()));
            Assert.Throws<ArgumentNullException>(() => map.AddParser<int>(int.Parse, null));
            Assert.Throws<ArgumentNullException>(() => map.AddParser(null));
        }

        [Fact]
        public void TestGetParser__NotRegistered()
        {
            ValueParseMap map = new ValueParseMap();
            map.AddParser(Exemplars.ValueParser);

            Assert.Throws<ParseTypeNotRegisteredException>(() => map.GetParser(typeof(double)));
        }

        [Fact]
        public void TestGetParser__Null()
        {
            ValueParseMap map = new ValueParseMap();
            map.AddParser(Exemplars.ValueParser);

            Assert.Throws<ArgumentNullException>(() => map.GetParser(null));
        }

        [Fact]
        public void TestCanParse()
        {
            ValueParseMap map = new ValueParseMap();
            map.AddParser(Exemplars.DummyValueParser<string>());
            map.AddParser(Exemplars.DummyValueParser<int>());

            Assert.True(map.CanParse(typeof(string)));
            Assert.True(map.CanParse(typeof(int)));
            Assert.False(map.CanParse(typeof(bool)));
        }

        [Fact]
        public void TestCanParse__Null()
        {
            ValueParseMap map = new ValueParseMap();
            map.AddParser(Exemplars.DummyValueParser<string>());

            Assert.Throws<ArgumentNullException>(() => map.CanParse(null));
        }

        [Fact]
        public void TestCanAdd()
        {
            ValueParseMap map = new ValueParseMap();

            Assert.True(map.CanAdd(typeof(string)));

            map.AddParser(Exemplars.DummyValueParser<string>());

            Assert.False(map.CanAdd(typeof(string)));
        }

        [Fact]
        public void TestCanAdd__Null()
        {
            ValueParseMap map = new ValueParseMap();
            map.AddParser(Exemplars.DummyValueParser<string>());

            Assert.Throws<ArgumentNullException>(() => map.CanAdd(null));
        }

        [Fact]
        public void TestClone()
        {
            IValueParser parser1 = Exemplars.DummyValueParser<int>();
            IValueParser parser2 = Exemplars.DummyValueParser<bool>();
            IValueParser parser3 = Exemplars.DummyValueParser<string>();

            ValueParseMap map = new ValueParseMap();
            map.AddParser(parser1);
            map.AddParser(parser2);

            ValueParseMap clone = map.Clone();

            Assert.Same(parser1, clone.GetParser(typeof(int)));
            Assert.Same(parser2, clone.GetParser(typeof(bool)));

            clone.AddParser(parser3);

            Assert.Same(parser3, clone.GetParser(typeof(string)));
            Assert.Throws<ParseTypeNotRegisteredException>(() => map.GetParser(typeof(string)));
        }
    }
}
