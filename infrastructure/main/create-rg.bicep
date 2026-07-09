targetScope = 'subscription'

param location string = 'centralus'
param name string = 'rg-virtual-family-museum'

resource virtualFamilyMuseumGroup 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: name
  location: location
}
