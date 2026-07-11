targetScope = 'resourceGroup'

param keyVaultName string = 'kv-virtual-family-museum'

resource familyVault 'Microsoft.KeyVault/vaults@2026-02-01' = {
  location: resourceGroup().location
  name: keyVaultName
  properties: {
    createMode: 'default'
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enablePurgeProtection: true
    enableRbacAuthorization: true
    enableSoftDelete: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    publicNetworkAccess: 'Enabled'
    sku: {
      family: 'A'
      name: 'standard'
    }
    softDeleteRetentionInDays: 30
    tenantId: subscription().tenantId
  }
}
