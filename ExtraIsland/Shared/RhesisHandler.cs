using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExtraIsland.Components;

namespace ExtraIsland.Shared;

public class RhesisHandler {
    
}

public class RhesisData {
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    
    public string Catalog { get; set; } = string.Empty;
}

public class SainticData {
        
        [JsonPropertyName("code")]
        public int StatusCode { get; set; } = -1;
        
        [JsonPropertyName("data")]
        public SainticRhesisData Data { get; set; } = new SainticRhesisData();
        
        [JsonPropertyName("msg")]
        public string? Message { get; set; }
        
        [JsonPropertyName("q")]
        public QueueInfoData QueueInfo { get; set; } = new QueueInfoData();

        public class SainticRhesisData {
            
            [JsonPropertyName("author")]
            public string Author { get; set; } = string.Empty;
            
            [JsonPropertyName("author_pinyin")]
            public string AuthorPinyin { get; set; } = string.Empty;
            
            [JsonPropertyName("catalog")]
            public string Catalog { get; set; } = string.Empty;
            
            [JsonPropertyName("catalog_pinyin")]
            public string CatalogPinyin { get; set; } = string.Empty;

            [JsonPropertyName("ctime")]
            public int Ctime { get; set; } = 0;

            [JsonPropertyName("id")]
            public int Id { get; set; } = 0;

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("sentence")]
            public string Sentence { get; set; } = string.Empty;
            
            [JsonPropertyName("src_url")]
            public string SrcUrl { get; set; } = string.Empty;

            [JsonPropertyName("theme")]
            public string Theme { get; set; } = string.Empty;

            [JsonPropertyName("theme_pinyin")]
            public string ThemePinyin { get; set; } = string.Empty;
        }

        public class QueueInfoData {

            [JsonPropertyName("author")]
            public string Author { get; set; } = string.Empty;
            
            [JsonPropertyName("catalog")]
            public string Catalog { get; set; } = string.Empty;

            [JsonPropertyName("suffix")]
            public string Suffix { get; set; } = string.Empty;

            [JsonPropertyName("theme")]
            public string Theme { get; set; } = string.Empty;
        }
        
        public RhesisData ToRhesisData() {
            return new RhesisData {
                Author = Data.Author,
                Title = Data.Name,
                Content = Data.Sentence,
                Source = "SainticAPI",
                Catalog = $"{Data.Theme}-{Data.Catalog}",
            };
        }
        
        public static SainticData Fetch() {
            return (new HttpClient()).GetFromJsonAsync<SainticData>("https://open.saintic.com/api/sentence/all.json").Result!;
        }
    }

public class JinrishiciData {
        
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
        
        [JsonPropertyName("origin")]
        public string Origin { get; set; } = string.Empty;
        
        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;
        
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
        
        public RhesisData ToRhesisData() {
            return new RhesisData {
                Author = Author,
                Title = Origin,
                Content = Content,
                Source = "今日诗词API",
                Catalog = Category,
            };
        }

        public static JinrishiciData Fetch() {
            return (new HttpClient()).GetFromJsonAsync<JinrishiciData>("https://v1.jinrishici.com/all.json").Result!;
        }
    }