targetScope = 'resourceGroup'
param logContainerName string = 'logs'
param imageContainerName string = 'images'
param templateContainerName string = 'templates'

resource familyTreeIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: 'familyTreeIdentity'
}

resource familyTreeVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: 'familyTreeVault'
}

resource familyTreeConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: 'familyTreeConfiguration'
}

resource familyTreeInsights 'Microsoft.Insights/components@2018-05-01-preview' = {
  name: 'familyTreeInsights'
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource familyTreeStaticStorage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: 'familytreestaticstorage'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

resource familyTreeBlobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: familyTreeStaticStorage
  name: 'default'
}

resource logContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: familyTreeBlobService
  name: logContainerName
  properties: {
    publicAccess: 'None'
  }
}

resource imageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: familyTreeBlobService
  name: imageContainerName
  properties: {
    publicAccess: 'None'
  }
}

resource templateContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: familyTreeBlobService
  name: templateContainerName
  properties: {
    publicAccess: 'None'
  }
}

resource lifecyclePolicy 'Microsoft.Storage/storageAccounts/managementPolicies@2021-09-01' = {
  parent: familyTreeStaticStorage
  name: 'default'
  properties: {
    policy: {
      rules: [
        {
          enabled: true
          name: 'MoveLogsToColdThenArchiveBeforeDeletion'
          type: 'Lifecycle'
          definition: {
            actions: {
              baseBlob: {
                delete: {
                  daysAfterModificationGreaterThan: 180
                }
                tierToCool: {
                  daysAfterModificationGreaterThan: 0
                }
                tierToArchive: {
                  daysAfterModificationGreaterThan: 90
                }
              }
            }
            filters: {
              blobTypes: ['blockBlob']
              prefixMatch: [logContainerName]
            }
          }
        }
        {
          enabled: true
          name: 'MoveTemplatesToArchiveBeforeDeletion'
          type: 'Lifecycle'
          definition: {
            actions: {
              baseBlob: {
                delete: {
                  daysAfterModificationGreaterThan: 180
                }
                tierToArchive: {
                  daysAfterModificationGreaterThan: 90
                }
              }
            }
            filters: {
              blobTypes: ['blockBlob']
              prefixMatch: [templateContainerName]
            }
          }
        }
        {
          enabled: true
          name: 'MoveImagesToArchiveOnUserRemoval'
          type: 'Lifecycle'
          definition: {
            actions: {
              baseBlob: {
                delete: {
                  daysAfterModificationGreaterThan: 180
                }
                tierToArchive: {
                  daysAfterModificationGreaterThan: 0
                }
              }
            }
            filters: {
              blobTypes: ['blockBlob']
              prefixMatch: [imageContainerName]
            }
          }
        }
      ]
    }
  }
}

resource appInsightsDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'AppInsightsToStorageLogs'
  scope: familyTreeInsights
  properties: {
    storageAccountId: familyTreeStaticStorage.id
    logs: [
      {
        category: 'AppTraces'
        enabled: true
      }
    ]
  }
}

resource storageDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'StorageLogs'
  scope: familyTreeBlobService
  properties: {
    storageAccountId: familyTreeStaticStorage.id
    logs: [
      {
        category: 'StorageRead'
        enabled: true
      }
      {
        category: 'StorageWrite'
        enabled: true
      }
      {
        category: 'StorageDelete'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'Transaction'
        enabled: true
      }
    ]
  }
}
