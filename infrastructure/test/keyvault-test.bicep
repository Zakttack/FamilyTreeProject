targetScope = 'resourceGroup'

param keyVaultName string = 'kv-vfm-test'

resource familyVaultTest 'Microsoft.KeyVault/vaults@2026-02-01' = {
  location: resourceGroup().location
  name: keyVaultName
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: '71528c84-639b-4d98-8895-a97b10ed3da7'
        permissions: {
          secrets: [
            'get'
            'list'
            'set'
          ]
        }
      }
    ]
    createMode: 'default'
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableRbacAuthorization: false
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
    softDeleteRetentionInDays: 7
    tenantId: subscription().tenantId
  }
}
