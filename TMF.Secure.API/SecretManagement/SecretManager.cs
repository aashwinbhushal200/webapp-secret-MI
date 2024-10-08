﻿using Azure;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace TMF.Secure.API.SecretManagement
{
    public class SecretManager
    {
        private readonly SecretClient _secretClient;
        private readonly IConfiguration _configuration;

        public SecretManager(SecretClient secretClient,
                             IConfiguration configuration)
        {
            _secretClient = secretClient;
            _configuration = configuration;

        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            string secretValue = string.Empty;
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevelopment)
                secretValue = _configuration[secretName];
            else
            {
                try
                {
                    KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
                    secretValue = secret.Value;
                }
                catch (RequestFailedException ex)
                {
                    if (ex.Status == 404)
                    {
                        return secretValue;
                    }

                    else
                    {
                        throw;
                    }
                }
            }


            return secretValue;
        }
    }
}
