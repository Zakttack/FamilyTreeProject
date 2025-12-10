using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualFamilyMuseumLibrary.ConstantStore.Models
{
    public sealed record FamilyInsightsConfig
    {
        public string ConnectionString {get; init;} = "";

        public override string ToString()
        {
            return "Family Insights Config:\n" +
                $"\tConnection String: {ConnectionString}";
        }
    }
}