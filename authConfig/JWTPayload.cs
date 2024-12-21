
using System;
using System.Collections.Generic;

namespace CharityHubOnionArchitecture.authConfig
{
    public abstract class JWTPayload
    {
        public string Issuer { get; }
        public string Subject { get; }
        public string Type { get; }
        public string Audience { get; }
        public string JwtId { get; }
        public DateTime ExpireAt { get; }
        public DateTime IssuedAt { get; }

        protected JWTPayload(
            string type,
            string audience,
            string jwtId,
            DateTime expireAt,
            DateTime issuedAt
        )
        {
            Issuer = "https://tech-mentors.net";
            Subject = "authentication";
            Type = type;
            Audience = audience;
            JwtId = jwtId;
            ExpireAt = expireAt;
            IssuedAt = issuedAt;
        }

        public abstract Dictionary<string, object> ToMap();

        public DateTime GetExpireAt()
        {
            return ExpireAt;
        }

        public DateTime GetIssuedAt()
        {
            return IssuedAt;
        }
    }
}

