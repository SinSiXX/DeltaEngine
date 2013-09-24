﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class SizeGraphStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is RangeGraph<Size>))
				return "";
			return (value as RangeGraph<Size>).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stringValue = value is string ? value as string : null;
			if (string.IsNullOrEmpty(stringValue))
				return null;
			var stringPartitions = stringValue.SplitAndTrim(new[] { '{', ',', ' ', '}' });
			if (stringPartitions.Length % 2 != 0 || stringPartitions.Length < 4)
				return null;
			return FillRangeFromStringPartitions(stringPartitions);
		}

		private static RangeGraph<Size> FillRangeFromStringPartitions(string[] partitions)
		{
			var sizeList = new List<Size>();
			for (int i = 0; i < partitions.Length - 1; i += 2)
			{
				if (!IsInputParsable(partitions[i]) || !IsInputParsable(partitions[i+1]))
					return null;
				try
				{
					sizeList.Add(new Size(float.Parse(partitions[i]), float.Parse(partitions[i + 1])));
				}
				catch
				{
					return null;
				}
			}
			return new RangeGraph<Size>(sizeList);
		}

		private static bool IsInputParsable(string input)
		{
			return !(input == "-0" || input.EndsWith("."));
		}
	}
}