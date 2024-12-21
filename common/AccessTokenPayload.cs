
using CharityHubOnionArchitecture.authConfig;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CharityHubOnionArchitecture.common
{

    public class AccessTokenPayload : JWTPayload
    {
        public string Uuid { get; }
        public string FullName { get; }
        public string PhotoUrl { get; }
        public bool Blocked { get; }
        public string MobileNumber { get; }
        public string DeviceId { get; }
        public IReadOnlyList<string> Permissions { get; }

        public AccessTokenPayload(
            string audience,
            string jwtId,
            DateTime expireAt,
            DateTime issuedAt,
            string uuid,
            string fullName,
            string photoUrl,
            bool blocked,
            string mobileNumber,
            string deviceId,
            List<string> permissions
        ) : base("accessToken", audience, jwtId, expireAt, issuedAt)
        {
            Uuid = uuid;
            FullName = fullName;
            PhotoUrl = photoUrl;
            Blocked = blocked;
            MobileNumber = mobileNumber;
            DeviceId = deviceId;
            Permissions = permissions ?? new List<string>();
        }

        //uncomment

        //public override IDictionary<string, object> ToMap()
        //{
        //    var userData = new Dictionary<string, object>
        //{
        //    { "uuid", Uuid },
        //    { "blocked", Blocked },
        //    { "type", Type },
        //    { "device_id", DeviceId },
        //    { "mobile_number", MobileNumber },
        //    { "permissions", Permissions }
        //};

        //    if (!string.IsNullOrEmpty(FullName))
        //    {
        //        userData["full_name"] = FullName;
        //    }
        //    if (!string.IsNullOrEmpty(PhotoUrl))
        //    {
        //        userData["photo_url"] = PhotoUrl;
        //    }

        //    return userData;
        //}

        public Guid GetUserId()
        {
            return Guid.Parse(Uuid);
        }

        public bool HasPermission(IEnumerable<string> requiredPermissions)
        {
            return requiredPermissions.Any(permission => Permissions.Contains(permission));
        }

        public static AccessTokenPayload FromPayload(IDictionary<string, object> payload)
        {
            return new AccessTokenPayload(
                payload.TryGetValue("aud", out var audience) ? audience.ToString() : null,
                payload.TryGetValue("jti", out var jwtId) ? jwtId.ToString() : null,
                payload.TryGetValue("exp", out var exp) ? DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp)).UtcDateTime : DateTime.MinValue,
                payload.TryGetValue("iat", out var iat) ? DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(iat)).UtcDateTime : DateTime.MinValue,
                payload.TryGetValue("uuid", out var uuid) ? uuid.ToString() : null,
                payload.TryGetValue("full_name", out var fullName) ? fullName.ToString() : null,
                payload.TryGetValue("photo_url", out var photoUrl) ? photoUrl.ToString() : null,
                payload.TryGetValue("blocked", out var blocked) && bool.TryParse(blocked.ToString(), out var isBlocked) && isBlocked,
                payload.TryGetValue("mobile_number", out var mobileNumber) ? mobileNumber.ToString() : null,
                payload.TryGetValue("device_id", out var deviceId) ? deviceId.ToString() : null,
                payload.TryGetValue("permissions", out var permissions) && permissions is IEnumerable<object> perms
                    ? perms.Select(p => p.ToString()).ToList()
                    : new List<string>()
            );
        }

        public override Dictionary<string, object> ToMap()
        {
            throw new NotImplementedException();
        }
    }
}