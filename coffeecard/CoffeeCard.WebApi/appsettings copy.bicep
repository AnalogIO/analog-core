// {
//   "AllowedHosts": "*",
//   "EnvironmentSettings": {
//     "EnvironmentType": "LocalDevelopment",
//     "MinAppVersion": "2.0.0",
//     "DeploymentUrl": "https://localhost:8080/"
//   },
//   "DatabaseSettings": {
//     "ConnectionString": "Server=mssql;Initial Catalog=master;User=sa;Password=Your_password123;",
//     "SchemaName": "dbo"
//   },
//   "IdentitySettings": {
//     "TokenKey": "local-development-token",
//     "AdminToken": "local-development-admintoken"
//   },
//   "MailgunSettings": {
//     "ApiKey": "None",
//     "Domain": "locahlhost",
//     "EmailBaseUrl": "https://localhost",
//     "MailgunApiUrl": "https://api.mailgun.net/v3"
//   },
//   "MobilePaySettings": {
//     "MerchantId": "APPDK0000000000",
//     "SubscriptionKey": "None",
//     "CertificateName": "development.pfx",
//     "CertificatePassword": "development"
//   },
//   "MobilePaySettingsV2": {
//     "ApiUrl": "https://invalidurl.test/",
//     "ApiKey": "DummyKey",
//     "ClientId": "DummyKey",
//     "PaymentPointId": "00000000-0000-0000-0000-000000000000",
//     "WebhookUrl": "https://invalidurl.test/"
//   },
//   "LoginLimiterSettings": {
//     "IsEnabled": true,
//     "MaximumLoginAttemptsWithinTimeOut": "3",
//     "TimeOutPeriodInSeconds": "900"
//   },
//   "Serilog": {
//     "Using": [
//       "Serilog.Sinks.Console"
//     ],
//     "MinimumLevel": "Information",
//     "WriteTo": [
//       {
//         "Name": "Console"
//       }
//     ]
//   }
// }

// set local environment variable

var keyvaultSecretURL = 'https://kv-${organizationPrefix}-${applicationPrefix}-${environment}.vault.azure.net/secrets'

resource webapp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'app-${organizationPrefix}-${applicationPrefix}-${environment}'
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    serverFarmId: appservicePlan.id
    reserved: true
    siteConfig: {
      numberOfWorkers: 1
      alwaysOn: false
      linuxFxVersion: 'DOTNETCORE|6.0'
      http20Enabled: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      appSettings: [
        // Import from the comment at the top
        {
          name: 'AllowedHosts'
          value: '*'
        }
        {
          name: 'EnvironmentSettings__EnvironmentType'
          value: 'LocalDevelopment'
        }
        {
          name: 'EnvironmentSettings__MinAppVersion'
          value: '2.0.0'
        }
        {
          name: 'EnvironmentSettings__DeploymentUrl'
          value: 'https://localhost:8080/'
        }
        {
          name: 'DatabaseSettings__ConnectionString'
          value: 'Server=mssql;Initial Catalog=master;User=sa;Password=Your_password123;'
        }
        {
          name: 'DatabaseSettings__SchemaName'
          value: 'dbo'
        }
        {
          name: 'IdentitySettings__TokenKey'
          value: 'local-development-token'
        }
        {
          name: 'IdentitySettings__AdminToken'
          value: 'local-development-admintoken'
        }
        {
          name: 'MailgunSettings__ApiKey'
          value: 'None'
        }
        {
          name: 'MailgunSettings__Domain'
          value: 'locahlhost'
        }
        {
          name: 'MailgunSettings__EmailBaseUrl'
          value: 'https://localhost'
        }
        {
          name: 'MailgunSettings__MailgunApiUrl'
          value: 'https://api.mailgun.net/v3'
        }
        {
          name: 'MobilePaySettings__MerchantId'
          value: 'APPDK0000000000'
        }
        {
          name: 'MobilePaySettings__SubscriptionKey'
          value: 'None'
        }
        {
          name: 'MobilePaySettings__CertificateName'
          value: 'development.pfx'
        }
        {
          name: 'MobilePaySettings__CertificatePassword'
          value: 'development'
        }
        {
          name: 'MobilePaySettingsV2__ApiUrl'
          value: 'https://invalidurl.test/'
        }
        {
          name: 'MobilePaySettingsV2__ApiKey'
          value: 'DummyKey'
        }
        {
          name: 'MobilePaySettingsV2__ClientId'
          value: 'DummyKey'
        }
        {
          name: 'MobilePaySettingsV2__PaymentPointId'
          value: '00000000-0000-0000-0000-000000000000'
        }
        {
          name: 'MobilePaySettingsV2__WebhookUrl'
          value: 'https://invalidurl.test/'
        }
        {
          name: 'LoginLimiterSettings__IsEnabled'
          value: true
        }
        // Read as a keyvault reference
        {
          name: 'LoginLimiterSettings__MaximumLoginAttemptsWithinTimeOut'
          value: '@Microsoft.KeyVault(SecretUri=${keyvaultSecretURL}/MaximumLoginAttemptsWithinTimeOut)'
        }
      ]
    }
    // https://kv-${organizationPrefix}-${applicationPrefix}-${environment}.vault.azure.net/secrets/
    httpsOnly: true
    redundancyMode: 'None'
    keyVaultReferenceIdentity: 'SystemAssigned'
  }
}
