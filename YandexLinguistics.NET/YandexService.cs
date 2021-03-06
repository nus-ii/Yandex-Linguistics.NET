﻿using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YandexLinguistics.NET
{
	public abstract class YandexService
	{
		protected RestClient _client;
		protected string _key;

		public YandexService(string key, string baseUrl)
		{
			_key = key;
			_client = new RestClient(baseUrl);
		}

		protected T SendRequest<T>(RestRequest request)
		{
			RestResponse response = (RestResponse)_client.Execute(request);
			XmlAttributeDeserializer deserializer = new XmlAttributeDeserializer();
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var result = deserializer.Deserialize<T>(response);
				return result;
			}
			else
			{
				YandexError error = null;
				try
				{
					error = deserializer.Deserialize<YandexError>(response);
				}
				finally
				{
					if (error == null)
					{
						var errorMessage = !string.IsNullOrEmpty(response.ErrorMessage) ?
							response.ErrorMessage : response.Content;
						throw new YandexLinguisticsException((int)response.StatusCode, errorMessage);
					}
					else
						throw new YandexLinguisticsException(error);
				}
			}
		}
	}
}
