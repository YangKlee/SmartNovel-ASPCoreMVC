namespace SmartNovel.Services
{
    public class ChapterContentService
    {
        private readonly HttpClient _httpClient;

        public ChapterContentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetContentAsync(string? fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return "<p>Nội dung chương không tồn tại.</p>";

            try
            {
                string html = await _httpClient.GetStringAsync(fileUrl);

                html = html.Replace("&nbsp;", " ");

                return html;
            }
            catch
            {
                return "<p>Không thể tải nội dung chương.</p>";
            }
        }
    }
}