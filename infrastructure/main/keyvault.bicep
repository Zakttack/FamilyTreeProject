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

var keyVaultSecretsOfficerRoleId = 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'

resource developerSecretsOfficerAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(familyVault.id, '71528c84-639b-4d98-8895-a97b10ed3da7', keyVaultSecretsOfficerRoleId)
  scope: familyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsOfficerRoleId)
    principalId: '71528c84-639b-4d98-8895-a97b10ed3da7'
    principalType: 'User'
  }
}
