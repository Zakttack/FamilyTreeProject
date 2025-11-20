targetScope = 'resourceGroup'

param appConfigurationName string
param keyVaultName string

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
