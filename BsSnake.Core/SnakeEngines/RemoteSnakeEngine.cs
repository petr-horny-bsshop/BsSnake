﻿using System.Net.Http.Json;
using BsSnake.Contracts;
using BsSnake.Core.Model;

namespace BsSnake.Core.SnakeEngines
{
    internal class RemoteSnakeEngine : ISnakeEngine
    {
        private readonly string _url;

        public RemoteSnakeEngine(string url)
        {
            _url = url;
        }

        public string Url => _url;

        public async Task InitAsync(GameDto game, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            var content = JsonContent.Create(game);
            var url = _url.TrimEnd('/') + "/init";
            await httpClient.PostAsync(url, content, cancellationToken);
        }

        public async Task<ResponseDto> MoveAsync(GameDto game, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            var content = JsonContent.Create(game);
            var url = _url.TrimEnd('/') + "/move";
            var response = await httpClient.PostAsync(url, content, cancellationToken);
           
            var result = await response.Content.ReadFromJsonAsync<ResponseDto?>(cancellationToken: cancellationToken);
            if (result == null)
            {
                var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Neplatná odpověď od hada: '{rawResponse}'");
            }

            return result;
        }
    }
}
