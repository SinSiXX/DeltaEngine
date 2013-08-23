﻿using System;
using System.IO;
using System.Linq;
using DeltaEngine.Graphics.OpenTK20;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.OpenTK;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	internal class OpenTK20Resolver : AppRunner
	{
		private readonly string[] nativeDllsNeeded = { "openal32.dll", "wrap_oal.dll" };

		public OpenTK20Resolver()
		{
			MakeSureOpenALDllsAreAvailable();
			RegisterCommonEngineSingletons();
			RegisterSingleton<FormsWindow>();
			RegisterSingleton<WindowsSystemInformation>();
			RegisterSingleton<OpenTK20Device>();
			RegisterSingleton<OpenTK20ScreenshotCapturer>();
			RegisterSingleton<OpenTKSoundDevice>();
			RegisterSingleton<WindowsMouse>();
			RegisterSingleton<WindowsKeyboard>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();
		}

		protected override void RegisterMediaTypes()
		{
			base.RegisterMediaTypes();
			Register<OpenTK20Image>();
			Register<OpenTK20Shader>();
			Register<OpenTK20Geometry>();
			Register<OpenTKSound>();
			Register<OpenTKMusic>();
			Register<OpenTKVideo>();
		}

		private void MakeSureOpenALDllsAreAvailable()
		{
			if (AreNativeDllsMissing())
				TryCopyNativeDlls();
		}

		private bool AreNativeDllsMissing()
		{
			return nativeDllsNeeded.Any(nativeDll => !File.Exists(nativeDll));
		}

		private void TryCopyNativeDlls()
		{
			try
			{
				CopyNativeDlls();
			}
			catch (Exception ex)
			{
				throw new FailedToCopyNativeOpenALDllFiles("Please install OpenAL, can't find dlls!", ex);
			}
		}

		private void CopyNativeDlls()
		{
			var path = Path.Combine("..", "..");
			while (!IsPackagesDirectory(path))
			{
				path = Path.Combine(path, "..");
				if (path.Length > 18)
					break;
			}
			foreach (var nativeDll in nativeDllsNeeded)
				File.Copy(Path.Combine(path, "packages", "OpenTKWithOpenAL.1.1.1160.61462", "NativeBinaries", "x86", nativeDll), nativeDll, true);
		}

		private static bool IsPackagesDirectory(string path)
		{
			return Directory.Exists(Path.Combine(path, "packages"));
		}

		public void RegisterFormsWindow(FormsWindow initializedWpfForumsWindow)
		{
			RegisterInstance(initializedWpfForumsWindow);
		}

		private class FailedToCopyNativeOpenALDllFiles : Exception
		{
			public FailedToCopyNativeOpenALDllFiles(string message, Exception innerException)
				: base(message,innerException)
			{
			}
		}
	}
}