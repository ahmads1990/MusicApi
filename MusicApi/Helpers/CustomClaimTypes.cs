﻿namespace MusicApi.Helpers
{
    public static class CustomClaimTypes
    {
        public static readonly string ISADMIN = "isAdmin";
        public static readonly List<string> ALLOWEDTYPES = new List<string> { ISADMIN };
    }
}
