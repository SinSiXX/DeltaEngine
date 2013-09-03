﻿using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;

namespace DeltaEngine.Rendering.Fonts
{
	/// <summary>
	/// Holds the image and data for rendering a smoothly drawn font.
	/// </summary>
	public class FontXml : XmlContent
	{
		protected FontXml(string contentName)
			: base(contentName) {}

		public static FontXml Default
		{
			get { return ContentLoader.Load<FontXml>(DefaultFontName); }
		}

		private const string DefaultFontName = "Verdana12";
		//ncrunch: no coverage start
		protected override bool AllowCreationIfContentNotFound
		{
			get { return true; }
		}
		//ncrunch: no coverage end
		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			InitializeDescriptionAndMaterial();
		}

		//ncrunch: no coverage start
		private void InitializeDescriptionAndMaterial()
		{
			if (Data == null || Data.Children.Count == 0)
			{
				Logger.Warning("Could not load '" + Name + "' font");
				return;
			}
			Description = new FontDescription(Data);
			Material = new Material(Shader.Position2DColorUv, Description.FontMapName);
			WasLoadedOk = true;
		}

		internal FontDescription Description { get; private set; }
		internal Material Material { get; private set; }
		internal bool WasLoadedOk { get; set; }

		protected override void CreateDefault()
		{
			if (Name == DefaultFontName)
			{
				if (Debugger.IsAttached)
					throw new DefaultFontContentNotFound();
				Logger.Warning("Could not load default '" + DefaultFontName + "' font");
				return;
			}
			Data = ContentLoader.Load<XmlContent>(DefaultFontName).Data;
			InitializeDescriptionAndMaterial();
		}
		//ncrunch: no coverage end
		public class DefaultFontContentNotFound : Exception {}

		protected override void DisposeData() {}
	}
}