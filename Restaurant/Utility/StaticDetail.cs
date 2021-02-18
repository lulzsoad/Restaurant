﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Utility
{
    public static class StaticDetail
    {
        public const string NoImage = "no_image.jpg";
        public const string ManagerUser = "Menadżer";
        public const string KitchenUser = "Kuchnia";
        public const string FrontDeskUser = "Obsługa";
        public const string CustomerEndUser = "Klient";
        public const string ssShoppingCartCount = "ssCartCount";


		public static string ConvertToRawHtml(string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}
	}
}
