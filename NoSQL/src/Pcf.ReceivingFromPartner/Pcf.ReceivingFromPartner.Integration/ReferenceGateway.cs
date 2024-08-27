using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class ReferenceGateway
        : IReferenceGateway
    {
        private readonly HttpClient _httpClient;

        public ReferenceGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Preference> GetPreferenceByIdAsync(Guid preferenceId)
        {
            var response = await _httpClient.GetAsync($"api/v1/preferences/{preferenceId}");
            return await response.Content.ReadFromJsonAsync<Preference>();
        }

        public async Task<List<Preference>> GetPreferenses()
        {
            var response = await _httpClient.GetAsync($"api/v1/preferences");
            return await response.Content.ReadFromJsonAsync<List<Preference>>();
                
        }


    }
}