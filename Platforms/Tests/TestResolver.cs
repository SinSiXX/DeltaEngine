﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using Moq;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Special resolver for unit tests that mocks all the integration classes (Window, Device, etc.)
	/// </summary>
	public class TestResolver : AutofacStarter
	{
		public TestResolver()
		{
			testCoreResolver = new TestCoreResolver(this);
			testRenderingResolver = new TestRenderingResolver(this);
			testInputResolver = new TestInputResolver(this);
			testMultimediaResolver = new TestMultimediaResolver(this);
			testPlatformsResolver = new TestPlatformsResolver(this);
			SetupVectorText();
			SetupXmlContent();
		}

		public int NumberOfVerticesDrawn { get; set; }

		private readonly List<object> registeredMocks = new List<object>();
		private readonly TestCoreResolver testCoreResolver;
		private readonly TestRenderingResolver testRenderingResolver;
		private readonly TestInputResolver testInputResolver;
		private readonly TestMultimediaResolver testMultimediaResolver;
		private readonly TestPlatformsResolver testPlatformsResolver;

		public Mock<T> RegisterMock<T>() where T : class
		{
			var mock = new Mock<T>();
			RegisterMock(mock.Object);
			return mock;
		}

		public T RegisterMock<T>(T instance) where T : class
		{
			Type instanceType = instance.GetType();
			Assert.IsFalse(registeredMocks.Any(mock => mock.GetType() == instanceType));
			registeredMocks.Add(instance);
			alreadyRegisteredTypes.AddRange(instanceType.GetInterfaces());
			RegisterInstance(instance);
			return instance;
		}

		private void SetupVectorText()
		{
			vectorTextData = new XmlData("VectorText");
			for (int i = 'A'; i <= 'Z'; i++)
				AddCharacter(i);

			for (int i = '0'; i <= '9'; i++)
				AddCharacter(i);

			AddCharacter('.');
			RegisterMock(vectorTextData);
		}

		private XmlData vectorTextData;

		private void AddCharacter(int i)
		{
			var character = new XmlData("Char" + i, vectorTextData);
			character.AddAttribute("Character", Convert.ToChar(i).ToString(CultureInfo.InvariantCulture));
			character.AddAttribute("Lines", "(0,0)-(1,1)");
		}

		private void SetupXmlContent()
		{
			var mockXmlContent = new Mock<XmlContent>("dummy");
			mockXmlContent.SetupGet(c => c.XmlData).Returns(vectorTextData);
			RegisterMock(mockXmlContent.Object);
		}

		public void SetKeyboardState(Key key, State state)
		{
			testInputResolver.KeyboardStates[(int)key] = state;
		}

		public void SetMouseButtonState(MouseButton button, State state, Point newMousePosition)
		{
			testInputResolver.MouseButtonStates[(int)button] = state;
			SetMousePosition(newMousePosition);
		}

		public void SetMousePosition(Point newMousePosition)
		{
			testInputResolver.CurrentMousePosition = newMousePosition;
		}

		public void SetTouchState(int touchIndex, State state, Point newTouchPosition)
		{
			testInputResolver.CurrentTouchPosition = newTouchPosition;
			testInputResolver.TouchStates[touchIndex] = state;
		}

		public void SetGamePadState(GamePadButton button, State state)
		{
			testInputResolver.GamePadButtonStates[(int)button] = state;
		}

		protected override void MakeSureContainerIsInitialized()
		{
			if (IsAlreadyInitialized)
				return;

			base.MakeSureContainerIsInitialized();
			foreach (var instance in registeredMocks)
				RegisterInstanceAsRunnerOrPresenterIfPossible(instance);

			testElapsedMs = GetTimeInMsForSlowTests();
		}

		protected override void RegisterInstanceAsRunnerOrPresenterIfPossible(object instance)
		{
			var renderable = instance as Renderable;
			if (renderable != null)
				Resolve<Renderer>().Add(renderable);

			base.RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
		}

		private long testElapsedMs;

		private long GetTimeInMsForSlowTests()
		{
			if (testStarted != null)
				return testStarted.ElapsedMilliseconds;

			testStarted = new Stopwatch();
			testStarted.Start();
			return 0;
		}

		private Stopwatch testStarted;

		public void AdvanceTimeAndExecuteRunners(float timeToAddInSeconds)
		{
			var simulateRunTicks = (int)Math.Round(timeToAddInSeconds * 60);
			for (int tick = 0; tick < simulateRunTicks; tick++)
			{
				RunAllRunners();
				RunAllPresenters();
			}
		}

		public override void Dispose()
		{
			WarnIfUnitTestTakesTooLong();
			base.Dispose();
		}

		//ncrunch: no coverage start
		private void WarnIfUnitTestTakesTooLong()
		{
			if (StackTraceExtensions.ContainsUnitTest() && TookLongerThan10Ms())
				Debug.WriteLine("This unit test takes too long (" + testElapsedMs + "ms, max. 10ms is " +
					"allowed), please add Category(\"Slow\") to run it nightly instead!");
		}

		private bool TookLongerThan10Ms()
		{
			testElapsedMs = GetTimeInMsForSlowTests() - testElapsedMs;
			return testElapsedMs > 10;
		}
	}
}