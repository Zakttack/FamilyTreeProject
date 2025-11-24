targetScope = 'resourceGroup'

param appConfigurationName string
param keyVaultName string
param userPrincipalId string
param familyVaultRoleDefinitionId string
param familyConfigurationRoleDefinitionId string

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
