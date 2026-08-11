targetScope = 'resourceGroup'

param storageAccountName string = 'family${uniqueString(resourceGroup().id)}drive'
param imagesContainerName string = 'images'
param templatesContainerName string = 'templates'

resource familyStorageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  kind: 'StorageV2'
  location: resourceGroup().location
  name: storageAccountName
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowCrossTenantReplication: false
    allowedCopyScope: 'AAD'
    allowSharedKeyAccess: false
    defaultToOAuthAuthentication: true
    dnsEndpointType: 'Standard'
    encryption: {
      keySource: 'Microsoft.Storage'
      requireInfrastructureEncryption: false
      services: {
        blob: {
          enabled: true
        }
      }
    }
    isHnsEnabled: true
    isLocalUserEnabled: false
    isNfsV3Enabled: false
    isSftpEnabled: false
    largeFileSharesState: 'Disabled'
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
      ipRules: []
      resourceAccessRules: []
      virtualNetworkRules: []
    }
    publicNetworkAccess: 'Enabled'
    supportsHttpsTrafficOnly: true
  }
  sku: {
    name: 'Standard_LRS'
  }
}

resource familyBlobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  name: 'default'
  parent: familyStorageAccount
}

resource familyImagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: imagesContainerName
  parent: familyBlobService
  properties: {
    publicAccess: 'None'
  }
}

resource familyTemplatesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: templatesContainerName
  parent: familyBlobService
  properties: {
    publicAccess: 'None'
  }
}

var storageBlobDataOwnerId = 'b7e6dc6d-f1e8-4753-8033-0f276bb0955b'
var objectId = '71528c84-639b-4d98-8895-a97b10ed3da7'

resource developerBlobDataOwnerAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(familyStorageAccount.id, objectId, storageBlobDataOwnerId)
  scope: familyStorageAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataOwnerId)
    principalId: objectId
    principalType: 'User'
  }
}
