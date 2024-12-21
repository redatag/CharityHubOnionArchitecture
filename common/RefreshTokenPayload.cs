using CharityHubOnionArchitecture.authConfig;
using System;
using System.Collections.Generic;

namespace CharityHubOnionArchitecture.common
{
  
    public class RefreshTokenPayload : JWTPayload
    {
        public string Uuid { get; }
        public string MobileNumber { get; }
        public string DeviceId { get; }

        public RefreshTokenPayload(
            string audience,
            string jwtId,
            DateTime expireAt,
            DateTime issuedAt,
            string uuid,
            string mobileNumber,
            string deviceId
        ) : base("refreshToken", audience, jwtId, expireAt, issuedAt)
        {
            Uuid = uuid ?? throw new ArgumentNullException(nameof(uuid));
            MobileNumber = mobileNumber ?? throw new ArgumentNullException(nameof(mobileNumber));
            DeviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));
        }

        //uncomment
        //public override IDictionary<string, object> ToMap()
        //{
        //    return new Dictionary<string, object>
        //{
        //    { "uuid", Uuid },
        //    { "type", Type },
        //    { "mobileNumber", MobileNumber },
        //    { "device_id", DeviceId }
        //};
        //}

        public static RefreshTokenPayload FromPayload(IDictionary<string, object> payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            return new RefreshTokenPayload(
                payload.TryGetValue("aud", out var audience) ? audience.ToString() : throw new ArgumentException("Missing audience"),
                payload.TryGetValue("jti", out var jwtId) ? jwtId.ToString() : throw new ArgumentException("Missing JWT ID"),
                payload.TryGetValue("exp", out var exp) ? DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp)).UtcDateTime : throw new ArgumentException("Missing expiration time"),
                payload.TryGetValue("iat", out var iat) ? DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(iat)).UtcDateTime : throw new ArgumentException("Missing issued-at time"),
                GetClaimString(payload, "uuid"),
                GetClaimString(payload, "mobileNumber"),
                GetClaimString(payload, "device_id")
            );
        }

        private static string GetClaimString(IDictionary<string, object> payload, string key)
        {
            if (!payload.TryGetValue(key, out var value) || value == null)
            {
                throw new ArgumentException($"Required claim '{key}' is missing");
            }
            return value.ToString();
        }

        public override Dictionary<string, object> ToMap()
        {
            throw new NotImplementedException();
        }
    }

}
