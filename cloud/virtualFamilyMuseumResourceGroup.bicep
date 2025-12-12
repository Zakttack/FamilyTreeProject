targetScope = 'resourceGroup'

param appConfigurationName string
param keyVaultName string
param userPrincipalId string
param familyVaultRoleDefinitionId string
param familyConfigurationRoleDefinitionId string
param storageAccountName string

resource familyConfiguration 'Microsoft.AppConfiguration/configurationStores@2025-06-01-preview' = {
  name: appConfigurationName
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  sku: {
    name: 'free'
  }
  properties: {
    createMode: 'Default'
    disableLocalAuth: true
    enablePurgeProtection: false
    softDeleteRetentionInDays: 0
    publicNetworkAccess: 'Enabled'
  }
}

resource familyVault 'Microsoft.KeyVault/vaults@2025-05-01' = {
  location: resourceGroup().location
  name: keyVaultName
  properties: {
    createMode: 'default'
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
  }
}

resource familyVaultAdminRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: familyVault
  name: guid(familyVault.id, userPrincipalId, familyVaultRoleDefinitionId)
  properties: {
    description: 'This is an administrative role for my Family Vault.'
    principalId: userPrincipalId
    principalType: 'User'
    roleDefinitionId: familyVaultRoleDefinitionId
  }
}

resource familyConfigurationAdminRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: familyConfiguration
  name: guid(familyConfiguration.id, userPrincipalId, familyConfigurationRoleDefinitionId)
  properties: {
    description: 'This is an administrative role for my Family Configuration.'
    principalId: userPrincipalId
    principalType: 'User'
    roleDefinitionId: familyConfigurationRoleDefinitionId
  }
}

resource familyInsightsWorkspace 'Microsoft.OperationalInsights/workspaces@2025-07-01' = {
  location: resourceGroup().location
  name: 'la-virtual-family-museum'
  properties: {
    defaultDataCollectionRuleResourceId: null
    features: {
      disableLocalAuth: true
      enableDataExport: false
      enableLogAccessUsingOnlyResourcePermissions: true
      immediatePurgeDataOn30Days: false
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    retentionInDays: 30
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource familyInsights 'Microsoft.Insights/components@2020-02-02' = {
  kind: 'web'
  location: resourceGroup().location
  name: 'virtual-family-museum-insights'
  properties: {
    Application_Type: 'web'
    DisableIpMasking: false
    DisableLocalAuth: false
    ForceCustomerStorageForProfiler: false
    HockeyAppId: null
    ImmediatePurgeDataOn30Days: false
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    SamplingPercentage: 100
    WorkspaceResourceId: familyInsightsWorkspace.id
  }
}

resource familyDrive 'Microsoft.Storage/storageAccounts@2025-06-01' = {
  name: storageAccountName
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowCrossTenantReplication: false
    allowedCopyScope: 'AAD'
    allowSharedKeyAccess: true
    defaultToOAuthAuthentication: false
    dnsEndpointType: 'Standard'
    dualStackEndpointPreference: {
      publishIpv6Endpoint: false
    }
    enableExtendedGroups: false
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
          keyType: 'Account'
        }
        file: {
          enabled: true
          keyType: 'Account'
        }
        queue: {
          enabled: true
          keyType: 'Account'
        }
        table: {
          enabled: true
          keyType: 'Account'
        }
      }
    }
    isHnsEnabled: false
    isLocalUserEnabled: false
    isNfsV3Enabled: false
    isSftpEnabled: false
    keyPolicy: {
      keyExpirationPeriodInDays: 0
    }
    largeFileSharesState: 'Enabled'
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    publicNetworkAccess: 'Enabled'
    routingPreference: {
      routingChoice: 'MicrosoftRouting'
      publishInternetEndpoints: true
      publishMicrosoftEndpoints: true
    }
    supportsHttpsTrafficOnly: true
  }
}

resource familyDriveBlobService 'Microsoft.Storage/storageAccounts/blobServices@2025-06-01' = {
  parent: familyDrive
  name: 'default'
  properties: {
    automaticSnapshotPolicyEnabled: false
    changeFeed: {
      enabled: false
    }
    containerDeleteRetentionPolicy: {
      enabled: true
      days: 7
      allowPermanentDelete: true
    }
    cors: {
      corsRules: []
    }
    deleteRetentionPolicy: {
      enabled: true
      days: 7
      allowPermanentDelete: true
    }
    isVersioningEnabled: false
    lastAccessTimeTrackingPolicy: {
      enable: false
      blobType: []
      name: 'AccessTimeTracking'
      trackingGranularityInDays: 0
    }
    restorePolicy: {
      enabled: false
    }
  }
}
