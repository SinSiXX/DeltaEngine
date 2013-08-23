﻿using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Triggers.Tests
{
	internal class Trigger2DTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			material = new Material(Shader.Position2DColorUv, "DeltaEngineLogo");
		}

		private Material material;

		[Test, CloseAfterFirstFrame]
		public void CreateTrigger()
		{
			var trigger = new Sprite(material, new Rectangle(Point.Zero, (Size)Point.One))
			{
				Color = Color.Red
			};
			trigger.Add(new TimeTrigger.Data(Color.Red, Color.Grey, 1));
			trigger.Start<CollisionTrigger>().Add(new CollisionTrigger.Data(Color.White, Color.Red));
			Assert.AreEqual(Point.Zero, trigger.Get<Rectangle>().TopLeft);
			Assert.AreEqual(Point.One, trigger.Get<Rectangle>().BottomRight);
		}

		[Test]
		public void ChangeColorIfTwoRectanglesCollide()
		{
			var sprite = new Sprite(material, new Rectangle(0.25f, 0.2f, 0.5f, 0.5f));
			sprite.Start<CollisionTrigger>().Add(new CollisionTrigger.Data(Color.Yellow, Color.Blue));
			sprite.Get<CollisionTrigger.Data>().SearchTags.Add("Creep");
			var sprite2 = new Sprite(material, new Rectangle(0.5f, 0.2f, 0.1f, 0.5f));
			sprite2.AddTag("Creep");
		}

		[Test]
		public void ChangeColorTwiceASecond()
		{
			var sprite = new Sprite(material, Rectangle.HalfCentered)
			{
				Color = Color.Green
			};
			sprite.Start<TimeTrigger>().Add(new TimeTrigger.Data(Color.Green, Color.Gold, 0.5f));
		}
	}
}