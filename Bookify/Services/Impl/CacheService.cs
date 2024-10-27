﻿using Bookify.Utils;
using Microsoft.Extensions.Caching.Distributed;
using System.Buffers;
using System.Text.Json;

namespace Bookify.Services.Impl
{
    public class CacheService(IDistributedCache cache) : ICacheService
    {
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            return bytes is null ? default : Deserialize<T>(bytes);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, 
            CancellationToken cancellationToken = default)
        {
            var bytes = Serialize(value);
            await cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await cache.RemoveAsync(key, cancellationToken);
        }

        private static byte[] Serialize<T>(T value)
        {
            var buffer = new ArrayBufferWriter<byte>();

            using var writer = new Utf8JsonWriter(buffer);
            JsonSerializer.Serialize(writer, value);
            return buffer.WrittenSpan.ToArray();
        }

        private static T Deserialize<T>(byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes)!;
        }
    }
}