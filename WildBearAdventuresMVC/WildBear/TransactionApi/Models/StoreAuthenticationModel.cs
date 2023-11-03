﻿namespace WildBearAdventuresMVC.WildBear.TransactionApi.Models
{

    /// <summary>
    /// Authentication is per store.
    /// </summary>
    public class StoreAuthenticationModel
    {
        /// <summary>
        /// Will function as clientId for the api calls
        /// </summary>
        public required string ClientGuid { get; init; }

        /// <summary>
        /// Find the ClientSecret in the Ucommerce back office
        /// </summary>
        public required string ClientSecret { get; init; }

        public required string RedirectUrl { get; init; }

        public required string BaseUrl { get; init; }

        public AuthorizeResponseModel? authorizationTokenDetails { get; set; }


    }
}
