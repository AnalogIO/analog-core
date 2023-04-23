param location string = resourceGroup().location

param organizationPrefix string
param applicationPrefix string
param environment string

param sharedResourceGroupName string

param logAnalyticsWorkspaceName string

param skuCapacity int
param skuName string
param skuTier string

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-${organizationPrefix}-${applicationPrefix}-${environment}'
  location: location
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      principalType: 'Group'
      login: 'analogio-admins'
      sid: 'e1fc4d3f-0369-4250-80ad-78fb6ed443b0'
      tenantId: tenant().tenantId
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
    version: '12.0'
  }

  resource auditSettings 'auditingSettings@2021-11-01' = {
    name: 'default'
    properties: {
      state: 'Enabled'
      auditActionsAndGroups: [
        'SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP'
        'FAILED_DATABASE_AUTHENTICATION_GROUP'
        'BATCH_COMPLETED_GROUP'
      ]
      isAzureMonitorTargetEnabled: true
    }
  }

  resource devOpsAuditingSettings 'devOpsAuditingSettings@2021-11-01' = {
    name: 'default'
    properties: {
      state: 'Enabled'
      isAzureMonitorTargetEnabled: true
    }
  }

  resource securityAlerts 'securityAlertPolicies@2021-11-01' = {
    name: 'default'
    properties: {
      emailAccountAdmins: true
      emailAddresses: [
        'alerts@analogio.dk'
      ]
      state: 'Enabled'
    }
  }

  resource firewallRules 'firewallRules@2021-11-01' = {
    name: 'default'
    properties: {
      endIpAddress: '0.0.0.0' // 0.0.0.0 allow Azure services to connect
      startIpAddress: '0.0.0.0'
    }
  }
}

resource sqlServerDiagnosticSettings 'Microsoft.Sql/servers/databases/providers/diagnosticSettings@2021-05-01-preview' = {
  name: '${sqlServer.name}/master/microsoft.insights/LogAnalytics'
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'SQLSecurityAuditEvents'
        enabled: true
      }
      {
        category: 'DevOpsOperationsAudit'
        enabled: true
      }
    ]
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-11-01' = {
  name: 'sqldb-${organizationPrefix}-${applicationPrefix}-${environment}'
  parent: sqlServer
  location: location
  sku: {
    capacity: skuCapacity
    name: skuName
    tier: skuTier
  }
  properties: {
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
    zoneRedundant: false
  }

  resource auditSettings 'auditingSettings@2021-11-01' = {
    name: 'default'
    properties: {
      state: 'Enabled'
      auditActionsAndGroups: [
        'SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP'
        'FAILED_DATABASE_AUTHENTICATION_GROUP'
        'BATCH_COMPLETED_GROUP'
      ]
      isAzureMonitorTargetEnabled: true
    }
  }

  resource securityAlerts 'securityAlertPolicies@2021-11-01' = {
    name: 'default'
    properties: {
      state: 'Enabled'
      emailAccountAdmins: true
      emailAddresses: [
        'alerts@analogio.dk'
      ]
    }
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logAnalyticsWorkspaceName
  scope: resourceGroup(sharedResourceGroupName)
}

resource diagnosticSettingsSqldb 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'Audit Logs'
  scope: sqlDatabase
  properties: {
    workspaceId: logAnalyticsWorkspace.id
    logs: [
      {
        category: 'SQLSecurityAuditEvents'
        enabled: true
      }
      {
        category: 'DevOpsOperationsAudit'
        enabled: true
      }
    ]
  }
}

output sqlDatabaseConnectionString string = 'Server=tcp:${sqlServer.name}${az.environment().suffixes.sqlServerHostname};Authentication=Active Directory Default; Database=${sqlDatabase.name};Encrypt=True;TrustServerCertificate=False;'
