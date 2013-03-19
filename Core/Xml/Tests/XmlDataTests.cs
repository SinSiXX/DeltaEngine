﻿using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Core.Xml.Tests
{
	public class XmlDataTests
	{
		[Test]
		public void Constructor()
		{
			var root = new XmlData("name");
			Assert.AreEqual("name", root.Name);
			Assert.AreEqual(0, root.Children.Count);
			Assert.AreEqual(0, root.Attributes.Count);
		}

		[Test]
		public void EmptyName()
		{
			Assert.AreEqual("_Empty", new XmlData("").Name);
			Assert.AreEqual("_Empty", new XmlData(null).Name);
		}

		[Test]
		public void SpacesRemovedFromName()
		{
			Assert.AreEqual("HelloWorld", new XmlData("Hello World").Name);
		}

		[Test]
		public void GetChild()
		{
			var root = new XmlData("root");
			new XmlData("child1", root);
			var child2 = new XmlData("child2", root);
			Assert.AreEqual(child2, root.GetChild("child2"));
		}

		[Test]
		public void GetChildren()
		{
			var root = new XmlData("root");
			var child1 = new XmlData("child", root);
			new XmlData("stepchild", root);
			var child2 = new XmlData("child", root);
			var children = root.GetChildren("child");
			Assert.AreEqual(2, children.Count);
			Assert.IsTrue(children.Contains(child1));
			Assert.IsTrue(children.Contains(child2));
		}

		[Test]
		public void GetDescendant()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual(root.Children[1], root.GetDescendant(root.Children[1].Name));
			Assert.AreEqual(root.Children[1].Children[0], root.GetDescendant("grandchild"));
			Assert.AreEqual(null, root.GetDescendant("unknown"));
		}

		private static XmlData CreateDeepTestXmlData()
		{
			XmlData root = CreateShallowTestXmlData();
			var grandchild = new XmlData("Grandchild", root.Children[1]);
			grandchild.AddAttribute("Attr5", "Value5");
			return root;
		}

		private static XmlData CreateShallowTestXmlData()
		{
			var root = new XmlData("Root");
			var child1 = new XmlData("Child1", root);
			child1.AddAttribute("Attr1", "Value1");
			child1.AddAttribute("Attr2", "Value2");
			child1.Value = "Tom";
			var child2 = new XmlData("Child2", root);
			child2.AddAttribute("Attr3", "Value3");
			child2.AddAttribute("Attr4", "Value4");
			return root;
		}

		[Test]
		public void GetDescendantWithAttribute()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual(root.Children[0], root.GetDescendant(new XmlAttribute("Attr1", "Value1")));
			Assert.AreEqual(root.Children[1].Children[0],
				root.GetDescendant(new XmlAttribute("Attr5", "Value5")));
			Assert.AreEqual(null, root.GetDescendant(new XmlAttribute("Attr5", "Value6")));
			Assert.AreEqual(null, root.GetDescendant(new XmlAttribute("Attr6", "Value5")));
		}

		[Test]
		public void GetDescendantWithAttributeAndName()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual(root.Children[0],
				root.GetDescendant(new XmlAttribute("Attr1", "Value1"), root.Children[0].Name));
			Assert.AreEqual(null,
				root.GetDescendant(new XmlAttribute("Attr1", "Value1"), root.Children[1].Name));
		}

		[Test]
		public void GetDescendantWithAttributes()
		{
			XmlData root = CreateDeepTestXmlData();
			var attributes = new List<XmlAttribute>
			{
				new XmlAttribute("Attr1", "Value1"),
				new XmlAttribute("Attr2", "Value2")
			};
			Assert.AreEqual(root.Children[0], root.GetDescendant(attributes));
			Assert.AreEqual(root.Children[1].Children[0],
				root.GetDescendant(new List<XmlAttribute> { new XmlAttribute("Attr5", "Value5") }));
			attributes.Add(new XmlAttribute("Attr3", "Value3"));
			Assert.AreEqual(null, root.GetDescendant(attributes));
		}

		[Test]
		public void GetDescendantWithAttributesAndName()
		{
			XmlData root = CreateDeepTestXmlData();
			var attributes = new List<XmlAttribute>
			{
				new XmlAttribute("Attr1", "Value1"),
				new XmlAttribute("Attr2", "Value2")
			};
			Assert.AreEqual(root.Children[0], root.GetDescendant(attributes, "child1"));
			Assert.AreEqual(null, root.GetDescendant(attributes, "child2"));
			Assert.AreEqual(root.Children[1].Children[0],
				root.GetDescendant(new List<XmlAttribute> { new XmlAttribute("Attr5", "Value5") }),
				"child5");
		}

		[Test]
		public void GetTotalNodeCount()
		{
			Assert.AreEqual(3, CreateShallowTestXmlData().GetTotalNodeCount());
			Assert.AreEqual(4, CreateDeepTestXmlData().GetTotalNodeCount());
		}

		[Test]
		public void Remove()
		{
			XmlData root = CreateDeepTestXmlData();
			root.Children[0].Remove();
			Assert.AreEqual(3, root.GetTotalNodeCount());
			root.Children[0].Remove();
			Assert.AreEqual(1, root.GetTotalNodeCount());
		}

		[Test]
		public void RemoveChild()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.IsTrue(root.RemoveChild(root.Children[0]));
			Assert.IsFalse(root.RemoveChild(new XmlData("unknown")));
		}

		[Test]
		public void AddAttribute()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", "value");
			Assert.AreEqual(1, root.Attributes.Count);
			Assert.AreEqual(new XmlAttribute("attribute", "value"), root.Attributes[0]);
		}

		[Test]
		public void RemoveAttribute()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute1", "value1");
			root.AddAttribute("attribute2", "value2");
			root.AddAttribute("attribute1", "value3");
			root.RemoveAttribute("attribute1");
			Assert.AreEqual(1, root.Attributes.Count);
			root.RemoveAttribute("attribute3");
			Assert.AreEqual(1, root.Attributes.Count);
		}

		[Test]
		public void ClearAttributes()
		{
			XmlData root = CreateShallowTestXmlData();
			XmlData child = root.Children[0];
			Assert.AreEqual(2, child.Attributes.Count);
			child.ClearAttributes();
			Assert.AreEqual(0, child.Attributes.Count);
		}

		[Test]
		public void Value()
		{
			var root = new XmlData("root") { Value = "value" };
			Assert.AreEqual("value", root.Value);
		}

		[Test]
		public void GetDescendantValue()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual("Value5", root.GetDescendantValue("Attr5"));
			Assert.AreEqual("", root.GetDescendantValue("Attr6"));
		}

		[Test]
		public void GetAttributes()
		{
			XmlData root = CreateShallowTestXmlData();
			Dictionary<string, string> attributes = root.Children[0].GetAttributes();
			Assert.AreEqual("Value1", TryGetValue(attributes, "Attr1"));
			Assert.AreEqual("Value2", TryGetValue(attributes, "Attr2"));
			Assert.AreEqual(null, TryGetValue(attributes, "Attr3"));
		}

		private static string TryGetValue(IDictionary<string, string> attributes, string attribute)
		{
			string value;
			attributes.TryGetValue(attribute, out value);
			return value;
		}

		[Test]
		public void GetValue()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", "value");
			Assert.AreEqual("value", root.GetValue("attribute"));
			Assert.AreEqual("", root.GetValue("attribute2"));
		}

		[Test]
		public void ToStringProperty()
		{
			var root = CreateShallowTestXmlData();
			Assert.AreEqual(@"XmlData=Child1: <Child1 Attr1=""Value1"" Attr2=""Value2"">Tom</Child1>",
				root.Children[0].ToString());
		}

		[Test]
		public void ToXmlString()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual(Root, root.ToXmlString());
		}

		private const string Root = @"<Root>
  <Child1 Attr1=""Value1"" Attr2=""Value2"">Tom</Child1>
  <Child2 Attr3=""Value3"" Attr4=""Value4"">
    <Grandchild Attr5=""Value5"" />
  </Child2>
</Root>";
	}
}