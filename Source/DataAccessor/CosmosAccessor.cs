using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AspNetCore.Sample.Common;
using AspNetCore.Sample.Common.Configurations;
using AspNetCore.Sample.Common.ErrorHandling;
using AspNetCore.Sample.Common.Trace;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace AspNetCore.Sample.DataAccessor
{
    public class CosmosAccessor
    {
        private static AppSettings _appSettings;

        protected DocumentClient Client
        {
            get;
        }

        protected Uri CollectionUri
        {
            get;
        }

        protected string DatabaseId
        {
            get;
        }

        protected string CollectionId
        {
            get;
        }

        protected CosmosAccessor(DocumentClient client, string databaseId, string collectionId)
        {
            Client = client;
            DatabaseId = databaseId;
            CollectionId = collectionId;
            CollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
        }

        public static DocumentClient CreateClient(string connectionString, AppSettings appSettings = null)
        {
            _appSettings = appSettings ?? new AppSettings
            {
                RetrySecondsForTooManyRequests = Constant.DefaultRetrySecondsForTooManyRequests,
                MaxRetryForCosmos = Constant.DefaultMaxRetryForCosmos
            };

            return CreateClientAsync(connectionString).Result;
        }

        public static async Task<DocumentClient> CreateClientAsync(string connectionString)
        {
            Guard.ArgumentNotNullOrEmpty(connectionString, nameof(connectionString));

            var (accountEndpoint, accountKey) = ParseStringIntoSettings(connectionString);
            var endPoint = new Uri(accountEndpoint);
            var client = new DocumentClient(
                endPoint,
                accountKey,
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                    RetryOptions = new RetryOptions
                    {
                        MaxRetryAttemptsOnThrottledRequests = 5,
                        MaxRetryWaitTimeInSeconds = 5
                    }
                });

            await client.OpenAsync();

            return client;
        }

        protected static async Task<T> WithRetry<T>(Func<Task<T>> func)
            where T : class
        {
            int count = 0;
            while (true)
            {
                try
                {
                    return await func();
                }
                catch (DocumentClientException ex) when (count < _appSettings.MaxRetryForCosmos)
                {
                    count++;
                    if (ex.StatusCode != null)
                    {
                        var statusCode = (int)ex.StatusCode;
                        switch (statusCode)
                        {
                            case 429:
                                // TODO: use Microsoft.Azure.DocumentDB.TransientFaultHandling which wraps the DocumentDB client for exponential backoff
                                // and handles transient failures so you don't need to do anything extra.
                                // https://talkingaboutdata.wordpress.com/2015/08/27/handling-error-429-in-documentdb/
                                var delay = ex.RetryAfter + TimeSpan.FromSeconds(_appSettings.RetrySecondsForTooManyRequests);
                                Logger.TraceWarning($"Retry {count} after {delay.Seconds} seconds due to 429, ${ex.Error.Message}");
                                await Task.Delay(delay);
                                continue;
                            case 412:
                                // PreconditionFailedException
                                throw Errors.ETagNotMatch().Exception();
                            case 404:
                                return null;
                        }
                    }

                    throw;
                }
            }
        }

        private static (string accountEndpoint, string accountKey) ParseStringIntoSettings(string connectionString)
        {
            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var splitted = connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string nameValue in splitted)
            {
                var splittedNameValue = nameValue.Split(new char[] { '=' }, 2);

                if (splittedNameValue.Length != 2)
                {
                    throw new ArgumentException("Settings must be of the form \"name=value\".", nameof(connectionString));
                }

                if (settings.ContainsKey(splittedNameValue[0]))
                {
                    throw new ArgumentException($"Duplicate setting '{splittedNameValue[0]}' found.", nameof(connectionString));
                }

                settings.Add(splittedNameValue[0], splittedNameValue[1]);
            }

            return (settings["AccountEndpoint"], settings["AccountKey"]);
        }
    }
}
