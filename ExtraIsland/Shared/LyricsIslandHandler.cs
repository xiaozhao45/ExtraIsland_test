using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using ClassIsland.Core.Controls.CommonDialog;

namespace ExtraIsland.Shared;

public class LyricsIslandHandler : IDisposable {

    public LyricsIslandHandler(string url = "http://127.0.0.1:50063/") {
        Url = url;
        _listener = new HttpListener();
        _listener.Prefixes.Add(Url);
        StartHttpListener();
    }

    readonly HttpClient _httpClient = new HttpClient();
    readonly HttpListener _listener;
    string Url { get; }
    bool _isListening;

    public string Lyrics { get; private set; } = string.Empty;
    public event Action? OnLyricsChanged;

    void StartHttpListener() {
        try {
            _listener.Start();
            _isListening = true;
            ListenAsync();
        }
        catch (HttpListenerException ex) {
            CommonDialog.ShowError($"启动 HTTP 监听器失败: {ex.Message}");
        }
    }

    async void ListenAsync() {
        while (_isListening && _listener.IsListening) {
            try {
                HttpListenerContext context = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleRequestAsync(context));
            }
            catch (HttpListenerException ex) when (ex.ErrorCode == 995) {
                // 操作已中止。
                // 监听器已停止，无需处理。
            }
            catch (Exception ex) {
                CommonDialog.ShowError($"监听过程中发生错误: {ex.Message}");
            }
        }
    }

    async Task HandleRequestAsync(HttpListenerContext context) {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        if (request is { 
                HttpMethod: "POST",
                Url.LocalPath: "/component/lyrics/lyrics/" }) {
            try {
                using (StreamReader reader = new StreamReader(request.InputStream,request.ContentEncoding)) {
                    string json = await reader.ReadToEndAsync();
                    string lyric = ParseLyricFromJson(json);
                    Lyrics = lyric;
                    OnLyricsChanged?.Invoke();
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/plain";
                await using (StreamWriter writer = new StreamWriter(response.OutputStream)) {
                    await writer.WriteAsync("歌词更新成功！");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"处理请求时出错: {ex.Message}");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await using StreamWriter writer = new StreamWriter(response.OutputStream);
                await writer.WriteAsync("内部服务器错误");
            }
        } else {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            await using StreamWriter writer = new StreamWriter(response.OutputStream);
            await writer.WriteAsync("未找到请求的资源");
        }

        response.Close();
    }

    static string ParseLyricFromJson(string json) {
        // 示例 JSON: { "lyric": "你的歌词"}
        try {
            JsonDocument jsonDoc = JsonDocument.Parse(json);
            if (jsonDoc.RootElement.TryGetProperty("lyric",out JsonElement lyricElement)) {
                return lyricElement.GetString() ?? "未解析到歌词";
            }
            return "未解析到歌词";
        }
        catch (JsonException) {
            return "输入解析错误";
        }
    }

    public void Dispose() {
        _isListening = false;
        if (_listener.IsListening) {
            _listener.Stop();
            _listener.Close();
        }
        _httpClient.Dispose();
    }
}